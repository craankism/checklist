import type { TodoItem } from "../types/todo";

interface TodoItemRowProps {
  todo: TodoItem;
  onToggleDone: (id: number) => Promise<void>;
  onDelete: (id: number) => Promise<void>;
  onEdit: (todo: TodoItem) => void;
  onAddGoogleReminder: (todoId: number) => Promise<void>;
}

// Renders one task row with task-level actions.
export function TodoItemRow({
  todo,
  onToggleDone,
  onDelete,
  onEdit,
  onAddGoogleReminder,
}: TodoItemRowProps) {
  // Formats an ISO date string into a short local date string.
  function formatDate(dateValue?: string | null): string {
    if (!dateValue) {
      return "No due date";
    }

    const date = new Date(dateValue);
    return Number.isNaN(date.getTime())
      ? "Invalid date"
      : date.toLocaleDateString();
  }

  return (
    <li className={`todo-item ${todo.isDone ? "todo-item--done" : ""}`}>
      <div className="todo-item-main">
        <label className="todo-check-label">
          <input
            type="checkbox"
            checked={todo.isDone}
            onChange={() => onToggleDone(todo.id)}
            aria-label={`Mark ${todo.title} as ${todo.isDone ? "not done" : "done"}`}
          />
          <span>{todo.title}</span>
        </label>

        <p className="todo-description">
          {todo.description || "No description"}
        </p>

        <p className="todo-meta">
          <span className={`priority priority-${todo.priority.toLowerCase()}`}>
            Priority: {todo.priority}
          </span>
          <span>Due: {formatDate(todo.dueDate)}</span>
        </p>
      </div>

      <div className="todo-item-actions">
        <button type="button" onClick={() => onEdit(todo)}>
          Edit
        </button>
        <button type="button" onClick={() => onAddGoogleReminder(todo.id)}>
          Add Google Calendar Reminder
        </button>
        <button type="button" onClick={() => onDelete(todo.id)}>
          Delete
        </button>
      </div>
    </li>
  );
}
