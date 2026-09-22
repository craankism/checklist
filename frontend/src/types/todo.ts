// This enum mirrors backend priority values so UI and API stay aligned.
export type TodoPriority = "Low" | "Medium" | "High";

// This interface defines the todo payload returned by the backend API.
export interface TodoItem {
  id: number;
  title: string;
  description?: string | null;
  dueDate?: string | null;
  priority: TodoPriority;
  isDone: boolean;
  createdAt: string;
  updatedAt: string;
}

// This interface defines the payload used when creating a todo item.
export interface CreateTodoRequest {
  title: string;
  description?: string;
  dueDate?: string | null;
  priority: TodoPriority;
}

// This interface defines the payload used when updating a todo item.
export interface UpdateTodoRequest {
  title: string;
  description?: string;
  dueDate?: string | null;
  priority: TodoPriority;
  isDone: boolean;
}
