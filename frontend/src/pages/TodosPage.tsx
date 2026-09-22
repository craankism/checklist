import { useEffect, useState } from "react";
import { TodoForm } from "../components/TodoForm";
import { TodoList } from "../components/TodoList";
import { useTodos } from "../hooks/useTodos";
import {
  addGoogleCalendarReminder,
  getGoogleAuthorizeUrl,
  getGoogleConnectionStatus,
} from "../services/googleAuthService";
import type { TodoItem } from "../types/todo";

// Main page container coordinating todo and Google connection flows.
export function TodosPage() {
  const {
    todos,
    isLoading,
    error,
    addTodo,
    editTodo,
    toggleTodoDone,
    removeTodo,
  } = useTodos();
  const [editingTodo, setEditingTodo] = useState<TodoItem | null>(null);
  const [googleConnected, setGoogleConnected] = useState<boolean>(false);
  const [message, setMessage] = useState<string>("");

  useEffect(() => {
    // Loads Google connection state so UI can guide the user.
    async function loadGoogleStatus() {
      try {
        const status = await getGoogleConnectionStatus();
        setGoogleConnected(status);
      } catch {
        setGoogleConnected(false);
      }
    }

    loadGoogleStatus();
  }, []);

  // Starts OAuth flow by opening Google consent page in a separate tab/window.
  async function handleConnectGoogleAccount() {
    setMessage("");

    try {
      const url = await getGoogleAuthorizeUrl();
      window.open(url, "_blank", "noopener,noreferrer");
      setMessage(
        "Complete Google login in the opened browser tab, then refresh status.",
      );
    } catch (err) {
      const text =
        err instanceof Error
          ? err.message
          : "Could not start Google connection flow.";
      setMessage(text);
    }
  }

  // Re-checks Google status after the user completes OAuth in browser.
  async function handleRefreshGoogleStatus() {
    const status = await getGoogleConnectionStatus();
    setGoogleConnected(status);
    setMessage(
      status
        ? "Google account connected."
        : "Google account not connected yet.",
    );
  }

  // Creates a Google Calendar reminder event for one todo item.
  async function handleAddGoogleReminder(todoId: number) {
    setMessage("");

    try {
      const eventId = await addGoogleCalendarReminder(todoId);
      setMessage(`Google reminder created (Event ID: ${eventId}).`);
    } catch (err) {
      const text =
        err instanceof Error
          ? err.message
          : "Could not create Google reminder.";
      setMessage(text);
    }
  }

  return (
    <main className="app-main">
      <header>
        <h1>Checklist Desktop App</h1>
        <p>
          Simple foundation for local todo management with optional Google
          Calendar reminders.
        </p>
      </header>

      <section
        className="google-connect-section"
        aria-label="Google connection"
      >
        <h2>Google Account</h2>
        <p>Status: {googleConnected ? "Connected" : "Not Connected"}</p>
        <div className="google-connect-actions">
          <button type="button" onClick={handleConnectGoogleAccount}>
            Connect Google Account
          </button>
          <button type="button" onClick={handleRefreshGoogleStatus}>
            Refresh Google Status
          </button>
        </div>
      </section>

      <TodoForm
        editingTodo={editingTodo}
        onCreate={addTodo}
        onUpdate={editTodo}
        onCancelEdit={() => setEditingTodo(null)}
      />

      {isLoading ? (
        <p>Loading tasks...</p>
      ) : (
        <TodoList
          todos={todos}
          onToggleDone={toggleTodoDone}
          onDelete={removeTodo}
          onEdit={setEditingTodo}
          onAddGoogleReminder={handleAddGoogleReminder}
        />
      )}

      {error && <p className="error-text">Error: {error}</p>}
      {message && <p className="info-text">{message}</p>}
    </main>
  );
}
