import { apiRequest } from "./apiClient";

interface ReminderResponse {
  reminderUrl: string;
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

  return response.reminderUrl;
}
