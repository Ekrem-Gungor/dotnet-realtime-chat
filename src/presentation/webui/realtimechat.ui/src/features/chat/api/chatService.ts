import { apiRequest } from "../../../shared/api/httpClient";
import type { ChatMessage, CreateChatMessageRequest } from "../types/chat";

async function getMessages(): Promise<ChatMessage[]> {
  return apiRequest<ChatMessage[]>("/api/Chat/messages");
}

async function createMessage(
  request: CreateChatMessageRequest,
): Promise<ChatMessage> {
  return apiRequest<ChatMessage>("/api/Chat/create", {
    method: "POST",
    json: request,
  });
}

export const chatService = Object.freeze({
  getMessages,
  createMessage,
});
