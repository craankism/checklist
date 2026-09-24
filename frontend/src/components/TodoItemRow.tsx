import type { TodoItem } from "../types/todo";

interface TodoItemRowProps {
  todo: TodoItem;
  onToggleDone: (id: number) => Promise<void>;
  onDelete: (id: number) => Promise<void>;
  onAddGoogleReminder: (todoId: number) => Promise<void>;
}

// Renders one task row with task-level actions.
export function TodoItemRow({
  todo,
  onToggleDone,
  onDelete,
  onAddGoogleReminder,
}: TodoItemRowProps) {
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
      </div>

      <div className="todo-item-actions">
        <button type="button" onClick={() => onAddGoogleReminder(todo.id)}>
          Add to Google Calendar
        </button>
        <button type="button" onClick={() => onDelete(todo.id)}>
          Delete
        </button>
      </div>
    </li>
  );
}
