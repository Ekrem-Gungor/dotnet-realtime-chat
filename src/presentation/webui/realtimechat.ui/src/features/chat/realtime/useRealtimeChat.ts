import { useEffect, useState } from "react";
import type { ChatMessage } from "../types/chat";
import type {
  OnlineUser,
  RealtimeConnectionStatus,
  SystemMessage,
} from "../types/realtime";
import { signalRService, type SignalRConnectionLease } from "./signalRService";

interface UseRealtimeChatOptions {
  onMessage: (message: ChatMessage) => void;
  onSystemMessage?: (message: SystemMessage) => void;
}

interface UseRealtimeChatResult {
  connectionStatus: RealtimeConnectionStatus;
  connectionError: string | null;
  onlineUsers: OnlineUser[];
}

export function useRealtimeChat({
  onMessage,
  onSystemMessage,
}: UseRealtimeChatOptions): UseRealtimeChatResult {
  const [connectionStatus, setConnectionStatus] =
    useState<RealtimeConnectionStatus>("disconnected");

  const [connectionError, setConnectionError] = useState<string | null>(null);

  const [onlineUsers, setOnlineUsers] = useState<OnlineUser[]>([]);

  useEffect(() => {
    return signalRService.subscribeToStatus((status) => {
      setConnectionStatus(status);

      if (status === "connected") {
        setConnectionError(null);
      }
    });
  }, []);

  useEffect(() => {
    const unsubscribeMessage = signalRService.onMessage(onMessage);

    const unsubscribeSystemMessage = onSystemMessage
      ? signalRService.onSystemMessage(onSystemMessage)
      : () => {};

    const unsubscribeOnlineUsers = signalRService.onOnlineUsers((users) => {
      setOnlineUsers(users.filter((user) => user.isOnline));
    });

    return () => {
      unsubscribeMessage();
      unsubscribeSystemMessage();
      unsubscribeOnlineUsers();
    };
  }, [onMessage, onSystemMessage]);

  useEffect(() => {
    let isDisposed = false;
    let releaseConnection: SignalRConnectionLease | null = null;

    async function connect() {
      try {
        const acquiredConnection = await signalRService.acquireConnection();

        if (isDisposed) {
          await acquiredConnection();
          return;
        }

        releaseConnection = acquiredConnection;

        try {
          await Promise.all([
            signalRService.announceJoin(),
            signalRService.requestOnlineUsers(),
          ]);
        } catch {
          if (!isDisposed) {
            setConnectionError(
              "Bağlantı kuruldu ancak başlangıç bilgileri alınamadı.",
            );
          }
        }
      } catch {
        if (!isDisposed) {
          setConnectionError("Gerçek zamanlı bağlantı kurulamadı.");
        }
      }
    }

    void connect();

    return () => {
      isDisposed = true;

      if (releaseConnection) {
        void releaseConnection();
      }
    };
  }, []);

  return {
    connectionStatus,
    connectionError,
    onlineUsers,
  };
}
