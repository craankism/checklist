import type { TodoItem } from "../types/todo";
import { TodoItemRow } from "./TodoItemRow";

interface TodoListProps {
  todos: TodoItem[];
  onToggleDone: (id: number) => Promise<void>;
  onDelete: (id: number) => Promise<void>;
  onEdit: (todo: TodoItem) => void;
  onAddGoogleReminder: (todoId: number) => Promise<void>;
}

// Renders todo list and empty state messaging.
export function TodoList({
  todos,
  onToggleDone,
  onDelete,
  onEdit,
  onAddGoogleReminder,
}: TodoListProps) {
  if (todos.length === 0) {
    return (
      <p className="empty-state">No tasks yet. Create your first task above.</p>
    );
  }

  return (
    <section aria-label="Task list" className="todo-list-section">
      <h2>Tasks</h2>
      <ul className="todo-list">
        {todos.map((todo) => (
          <TodoItemRow
            key={todo.id}
            todo={todo}
            onToggleDone={onToggleDone}
            onDelete={onDelete}
            onEdit={onEdit}
            onAddGoogleReminder={onAddGoogleReminder}
          />
        ))}
      </ul>
    </section>
  );
}
