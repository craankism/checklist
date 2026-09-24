import { useState } from "react";
import { TodoForm } from "../components/TodoForm";
import { TodoList } from "../components/TodoList";
import { useTodos } from "../hooks/useTodos";
import { addGoogleCalendarReminder } from "../services/googleCalendarService";
import type { TodoItem } from "../types/todo";

// Main page container coordinating todo workflows.
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
  const [message, setMessage] = useState<string>("");

  // Opens a prefilled Google Calendar reminder page for one todo item.
  async function handleAddGoogleReminder(todoId: number) {
    setMessage("");

    try {
      const reminderUrl = await addGoogleCalendarReminder(todoId);
      window.open(reminderUrl, "_blank", "noopener,noreferrer");
      setMessage("Opened Google Calendar reminder in your default browser.");
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
          Simple foundation for local todo management with Google Calendar
          reminder links.
        </p>
      </header>

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
