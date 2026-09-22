import { useEffect, useState } from "react";
import type {
  CreateTodoRequest,
  TodoItem,
  TodoPriority,
  UpdateTodoRequest,
} from "../types/todo";

interface TodoFormProps {
  editingTodo?: TodoItem | null;
  onCreate: (payload: CreateTodoRequest) => Promise<void>;
  onUpdate: (id: number, payload: UpdateTodoRequest) => Promise<void>;
  onCancelEdit: () => void;
}

// Small form component used for both create and edit flows.
export function TodoForm({
  editingTodo,
  onCreate,
  onUpdate,
  onCancelEdit,
}: TodoFormProps) {
  const [title, setTitle] = useState<string>("");
  const [description, setDescription] = useState<string>("");
  const [dueDate, setDueDate] = useState<string>("");
  const [priority, setPriority] = useState<TodoPriority>("Medium");
  const [isSubmitting, setIsSubmitting] = useState<boolean>(false);

  useEffect(() => {
    if (!editingTodo) {
      setTitle("");
      setDescription("");
      setDueDate("");
      setPriority("Medium");
      return;
    }

    setTitle(editingTodo.title);
    setDescription(editingTodo.description ?? "");
    setDueDate(editingTodo.dueDate ? editingTodo.dueDate.slice(0, 10) : "");
    setPriority(editingTodo.priority);
  }, [editingTodo]);

  // Submits create or update payload depending on edit state.
  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!title.trim()) {
      return;
    }

    setIsSubmitting(true);

    try {
      if (editingTodo) {
        await onUpdate(editingTodo.id, {
          title: title.trim(),
          description: description.trim(),
          dueDate: dueDate || null,
          priority,
          isDone: editingTodo.isDone,
        });

        onCancelEdit();
      } else {
        await onCreate({
          title: title.trim(),
          description: description.trim(),
          dueDate: dueDate || null,
          priority,
        });

        setTitle("");
        setDescription("");
        setDueDate("");
        setPriority("Medium");
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <section className="todo-form-section" aria-label="Task form">
      <h2>{editingTodo ? "Edit Task" : "Add Task"}</h2>
      <form onSubmit={handleSubmit} className="todo-form">
        <label htmlFor="todo-title">Title</label>
        <input
          id="todo-title"
          name="title"
          value={title}
          onChange={(event) => setTitle(event.target.value)}
          required
        />

        <label htmlFor="todo-description">Description</label>
        <textarea
          id="todo-description"
          name="description"
          value={description}
          onChange={(event) => setDescription(event.target.value)}
          rows={3}
        />

        <label htmlFor="todo-dueDate">Due Date</label>
        <input
          id="todo-dueDate"
          name="dueDate"
          type="date"
          value={dueDate}
          onChange={(event) => setDueDate(event.target.value)}
        />

        <label htmlFor="todo-priority">Priority</label>
        <select
          id="todo-priority"
          name="priority"
          value={priority}
          onChange={(event) => setPriority(event.target.value as TodoPriority)}
        >
          <option value="Low">Low</option>
          <option value="Medium">Medium</option>
          <option value="High">High</option>
        </select>

        <div className="todo-form-actions">
          <button type="submit" disabled={isSubmitting}>
            {isSubmitting
              ? "Saving..."
              : editingTodo
                ? "Update Task"
                : "Add Task"}
          </button>
          {editingTodo && (
            <button type="button" onClick={onCancelEdit}>
              Cancel Edit
            </button>
          )}
        </div>
      </form>
    </section>
  );
}
