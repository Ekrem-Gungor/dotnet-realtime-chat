export interface ChatMessage {
  id: string;
  senderUserId: number | null;
  senderUserName: string;
  message: string;
  sendAt: string;
}

export interface CreateChatMessageRequest {
  message: string;
}
