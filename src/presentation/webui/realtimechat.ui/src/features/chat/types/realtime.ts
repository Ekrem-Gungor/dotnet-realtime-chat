export type RealtimeConnectionStatus =
  | "disconnected"
  | "connecting"
  | "connected"
  | "reconnecting";

export interface SystemMessage {
  joinedUserName: string;
  senderUserName: string;
  message: string;
  sendAt: string;
}

export interface OnlineUser {
  id: number;
  userName: string;
  isOnline: boolean;
}
