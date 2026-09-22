import { apiRequest } from "./apiClient";
import type {
  CreateTodoRequest,
  TodoItem,
  UpdateTodoRequest,
} from "../types/todo";

// Reads all todos from the backend API.
export async function getTodos(): Promise<TodoItem[]> {
  return apiRequest<TodoItem[]>("/api/todos");
}

// Creates one todo using the backend API.
export async function createTodo(
  payload: CreateTodoRequest,
): Promise<TodoItem> {
  return apiRequest<TodoItem>("/api/todos", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

// Updates one todo using the backend API.
export async function updateTodo(
  id: number,
  payload: UpdateTodoRequest,
): Promise<TodoItem> {
  return apiRequest<TodoItem>(`/api/todos/${id}`, {
    method: "PUT",
    body: JSON.stringify(payload),
  });
}

// Toggles completion state for one todo.
export async function toggleTodo(id: number): Promise<TodoItem> {
  return apiRequest<TodoItem>(`/api/todos/${id}/toggle`, {
    method: "PATCH",
  });
}

// Deletes one todo item by id.
export async function deleteTodo(id: number): Promise<void> {
  return apiRequest<void>(`/api/todos/${id}`, {
    method: "DELETE",
  });
}
