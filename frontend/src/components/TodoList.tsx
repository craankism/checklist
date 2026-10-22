import type { TodoItem } from "../types/todo";
import { TodoItemRow } from "./TodoItemRow";

interface TodoListProps {
  todos: TodoItem[];
  onToggleDone: (id: number) => Promise<void>;
  onDelete: (id: number) => Promise<void>;
  onAddGoogleReminder: (todoId: number) => Promise<void>;
}

// Renders todo list and empty state messaging.
export function TodoList({
  todos,
  onToggleDone,
  onDelete,
  onAddGoogleReminder,
}: TodoListProps) {
  if (todos.length === 0) {
    return (
      <p className="empty-state">No tasks yet. Create your first task above.</p>
    );
  }

  return (
    <section aria-label="Task list" className="todo-list-section">
      <div className="todo-list-header">
        <h2>Tasks</h2>
        <span className="todo-count-chip">{todos.length}</span>
      </div>
      <ul className="todo-list">
        {todos.map((todo) => (
          <TodoItemRow
            key={todo.id}
            todo={todo}
            onToggleDone={onToggleDone}
            onDelete={onDelete}
            onAddGoogleReminder={onAddGoogleReminder}
          />
        ))}
      </ul>
    </section>
  );
}
