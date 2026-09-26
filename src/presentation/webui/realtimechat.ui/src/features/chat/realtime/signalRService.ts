import {
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
  type HubConnection,
} from "@microsoft/signalr";
import { environment } from "../../../shared/config/environment";
import type { ChatMessage } from "../types/chat";
import type {
  OnlineUser,
  RealtimeConnectionStatus,
  SystemMessage,
} from "../types/realtime";

type ConnectionStatusHandler = (status: RealtimeConnectionStatus) => void;

type MessageHandler = (message: ChatMessage) => void;
type SystemMessageHandler = (message: SystemMessage) => void;
type OnlineUsersHandler = (users: OnlineUser[]) => void;

export type SignalRConnectionLease = () => Promise<void>;

const hubEvents = Object.freeze({
  receiveMessage: "ReceiveMessage",
  receiveSystemMessage: "ReceiveSystemMessage",
  receiveOnlineUsers: "ReceiveOnlineUsers",
});

const hubMethods = Object.freeze({
  join: "Join",
  getOnlineUsers: "GetOnlineUsers",
});

const reconnectDelays = [0, 2_000, 5_000, 10_000];
const disconnectedRetryDelay = 5_000;

const connection: HubConnection = new HubConnectionBuilder()
  .withUrl(environment.signalRHubUrl, {
    withCredentials: true,
  })
  .withAutomaticReconnect(reconnectDelays)
  .configureLogging(LogLevel.Warning)
  .build();

const statusHandlers = new Set<ConnectionStatusHandler>();

let connectionStatus: RealtimeConnectionStatus = "disconnected";
let startPromise: Promise<void> | null = null;
let restartTimer: ReturnType<typeof setTimeout> | null = null;
let activeConsumers = 0;

function setConnectionStatus(status: RealtimeConnectionStatus): void {
  connectionStatus = status;

  statusHandlers.forEach((handler) => {
    handler(status);
  });
}

async function startConnection(): Promise<void> {
  if (connection.state === HubConnectionState.Connected) {
    setConnectionStatus("connected");
    return;
  }

  if (startPromise) {
    return startPromise;
  }

  setConnectionStatus("connecting");

  startPromise = connection
    .start()
    .then(() => {
      setConnectionStatus("connected");
    })
    .catch((error: unknown) => {
      setConnectionStatus("disconnected");
      throw error;
    })
    .finally(() => {
      startPromise = null;
    });

  return startPromise;
}

function clearScheduledRestart(): void {
  if (restartTimer === null) {
    return;
  }

  clearTimeout(restartTimer);
  restartTimer = null;
}

async function restoreConnectionState(): Promise<void> {
  await Promise.all([announceJoin(), requestOnlineUsers()]);
}

function scheduleConnectionRestart(): void {
  if (
    activeConsumers === 0 ||
    restartTimer !== null ||
    connection.state !== HubConnectionState.Disconnected
  ) {
    return;
  }

  setConnectionStatus("reconnecting");

  restartTimer = setTimeout(() => {
    restartTimer = null;

    if (activeConsumers === 0) {
      return;
    }

    void startConnection()
      .then(restoreConnectionState)
      .catch(() => {
        // SignalR'ın kendi denemeleri bittikten sonra bağlantıyı kontrollü sürdürür.
        scheduleConnectionRestart();
      });
  }, disconnectedRetryDelay);
}

async function stopConnectionIfUnused(): Promise<void> {
  if (activeConsumers > 0) {
    return;
  }

  clearScheduledRestart();

  if (startPromise) {
    try {
      await startPromise;
    } catch {
      return;
    }
  }

  if (
    activeConsumers > 0 ||
    connection.state === HubConnectionState.Disconnected
  ) {
    return;
  }

  await connection.stop();
  setConnectionStatus("disconnected");
}

async function acquireConnection(): Promise<SignalRConnectionLease> {
  activeConsumers += 1;

  try {
    await startConnection();
  } catch {
    scheduleConnectionRestart();
  }

  let isReleased = false;

  return async () => {
    if (isReleased) {
      return;
    }

    isReleased = true;
    activeConsumers = Math.max(0, activeConsumers - 1);

    await stopConnectionIfUnused();
  };
}

function subscribeToStatus(handler: ConnectionStatusHandler): () => void {
  statusHandlers.add(handler);
  handler(connectionStatus);

  return () => {
    statusHandlers.delete(handler);
  };
}

function onMessage(handler: MessageHandler): () => void {
  connection.on(hubEvents.receiveMessage, handler);

  return () => {
    connection.off(hubEvents.receiveMessage, handler);
  };
}

function onSystemMessage(handler: SystemMessageHandler): () => void {
  connection.on(hubEvents.receiveSystemMessage, handler);

  return () => {
    connection.off(hubEvents.receiveSystemMessage, handler);
  };
}

function onOnlineUsers(handler: OnlineUsersHandler): () => void {
  connection.on(hubEvents.receiveOnlineUsers, handler);

  return () => {
    connection.off(hubEvents.receiveOnlineUsers, handler);
  };
}

async function announceJoin(): Promise<void> {
  await connection.invoke(hubMethods.join);
}

async function requestOnlineUsers(): Promise<void> {
  await connection.invoke(hubMethods.getOnlineUsers);
}

connection.onreconnecting(() => {
  setConnectionStatus("reconnecting");
});

connection.onreconnected(() => {
  setConnectionStatus("connected");

  void restoreConnectionState().catch(() => {
    // Bir sonraki server yayını listeyi tekrar güncelleyecektir.
  });
});

connection.onclose(() => {
  setConnectionStatus("disconnected");
  scheduleConnectionRestart();
});

export const signalRService = Object.freeze({
  acquireConnection,
  subscribeToStatus,
  onMessage,
  onSystemMessage,
  onOnlineUsers,
  announceJoin,
  requestOnlineUsers,
});
