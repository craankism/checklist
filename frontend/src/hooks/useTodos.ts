import { useCallback, useEffect, useState } from "react";
import {
  createTodo,
  deleteTodo,
  getTodos,
  toggleTodo,
  updateTodo,
} from "../services/todoService";
import type {
  CreateTodoRequest,
  TodoItem,
  UpdateTodoRequest,
} from "../types/todo";

// Provides todo state and actions so components stay focused on UI rendering.
export function useTodos() {
  const [todos, setTodos] = useState<TodoItem[]>([]);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [error, setError] = useState<string>("");

  // Loads all todo items from the backend.
  const loadTodos = useCallback(async () => {
    setIsLoading(true);
    setError("");

    try {
      const items = await getTodos();
      setTodos(items);
    } catch (err) {
      const message =
        err instanceof Error ? err.message : "Failed to load todo items.";
      setError(message);
    } finally {
      setIsLoading(false);
    }
  }, []);

  // Creates one todo then updates local state with server response.
  const addTodo = useCallback(async (payload: CreateTodoRequest) => {
    const created = await createTodo(payload);
    setTodos((current) => [created, ...current]);
  }, []);

  // Updates one todo then replaces the matching item in local state.
  const editTodo = useCallback(
    async (id: number, payload: UpdateTodoRequest) => {
      const updated = await updateTodo(id, payload);
      setTodos((current) =>
        current.map((item) => (item.id === id ? updated : item)),
      );
    },
    [],
  );

  // Toggles completion then updates local state with server response.
  const toggleTodoDone = useCallback(async (id: number) => {
    const updated = await toggleTodo(id);
    setTodos((current) =>
      current.map((item) => (item.id === id ? updated : item)),
    );
  }, []);

  // Deletes one todo item and removes it from local state.
  const removeTodo = useCallback(async (id: number) => {
    await deleteTodo(id);
    setTodos((current) => current.filter((item) => item.id !== id));
  }, []);

  useEffect(() => {
    loadTodos();
  }, [loadTodos]);

  return {
    todos,
    isLoading,
    error,
    loadTodos,
    addTodo,
    editTodo,
    toggleTodoDone,
    removeTodo,
  };
}
