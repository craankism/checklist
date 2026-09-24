import { useState } from "react";
import type { CreateTodoRequest } from "../types/todo";

interface TodoFormProps {
  onCreate: (payload: CreateTodoRequest) => Promise<void>;
}

// Small quick-add form for creating a todo with only a title.
export function TodoForm({ onCreate }: TodoFormProps) {
  const [title, setTitle] = useState<string>("");
  const [isSubmitting, setIsSubmitting] = useState<boolean>(false);

  // Submits a minimal create payload.
  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!title.trim()) {
      return;
    }

    setIsSubmitting(true);

    try {
      await onCreate({ title: title.trim() });
      setTitle("");
    } catch {
      // Error is surfaced by the page-level error message.
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <section className="todo-form-section" aria-label="Task form">
      <h2>Add Task</h2>
      <form onSubmit={handleSubmit} className="todo-form">
        <label htmlFor="todo-title">Task</label>
        <input
          id="todo-title"
          name="title"
          value={title}
          onChange={(event) => setTitle(event.target.value)}
          placeholder="Type a task"
          required
        />

        <div className="todo-form-actions">
          <button type="submit" disabled={isSubmitting}>
            {isSubmitting ? "Adding..." : "Add"}
          </button>
        </div>
      </form>
    </section>
  );
}
