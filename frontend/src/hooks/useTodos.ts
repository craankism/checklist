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

  function getErrorMessage(err: unknown, fallback: string): string {
    return err instanceof Error ? err.message : fallback;
  }

  // Creates one todo then updates local state with server response.
  const addTodo = useCallback(async (payload: CreateTodoRequest) => {
    setError("");

    try {
      const created = await createTodo(payload);
      setTodos((current) => [created, ...current]);
    } catch (err) {
      setError(getErrorMessage(err, "Failed to create todo item."));
      throw err;
    }
  }, []);

  // Updates one todo then replaces the matching item in local state.
  const editTodo = useCallback(
    async (id: number, payload: UpdateTodoRequest) => {
      setError("");

      try {
        const updated = await updateTodo(id, payload);
        setTodos((current) =>
          current.map((item) => (item.id === id ? updated : item)),
        );
      } catch (err) {
        setError(getErrorMessage(err, "Failed to update todo item."));
        throw err;
      }
    },
    [],
  );

  // Toggles completion then updates local state with server response.
  const toggleTodoDone = useCallback(async (id: number) => {
    setError("");

    try {
      const updated = await toggleTodo(id);
      setTodos((current) =>
        current.map((item) => (item.id === id ? updated : item)),
      );
    } catch (err) {
      setError(getErrorMessage(err, "Failed to toggle todo item."));
      throw err;
    }
  }, []);

  // Deletes one todo item and removes it from local state.
  const removeTodo = useCallback(async (id: number) => {
    setError("");

    try {
      await deleteTodo(id);
      setTodos((current) => current.filter((item) => item.id !== id));
    } catch (err) {
      setError(getErrorMessage(err, "Failed to delete todo item."));
      throw err;
    }
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
