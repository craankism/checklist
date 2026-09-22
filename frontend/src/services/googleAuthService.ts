import { apiRequest } from "./apiClient";

interface AuthorizeUrlResponse {
  url: string;
}

interface GoogleAuthStatusResponse {
  isConnected: boolean;
}

interface ReminderResponse {
  eventId: string;
}

// Gets the Google OAuth authorization URL from the backend.
export async function getGoogleAuthorizeUrl(): Promise<string> {
  const response = await apiRequest<AuthorizeUrlResponse>(
    "/api/google-auth/authorize-url",
  );
  return response.url;
}

// Reads whether the local user has already connected Google.
export async function getGoogleConnectionStatus(): Promise<boolean> {
  const response = await apiRequest<GoogleAuthStatusResponse>(
    "/api/google-auth/status",
  );
  return response.isConnected;
}

// Requests a reminder event for one todo item.
export async function addGoogleCalendarReminder(
  todoId: number,
): Promise<string> {
  const response = await apiRequest<ReminderResponse>(
    `/api/google-calendar/todos/${todoId}/reminder`,
    {
      method: "POST",
    },
  );

  return response.eventId;
}
