// Central API base URL so every service call is routed consistently.
const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5050";

// Shared helper for JSON requests with simple error handling.
export async function apiRequest<T>(
  path: string,
  options?: RequestInit,
): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    headers: {
      "Content-Type": "application/json",
      ...(options?.headers ?? {}),
    },
    ...options,
  });

  if (!response.ok) {
    const errorBody = await response.text();
    throw new Error(
      errorBody || `Request failed with status ${response.status}`,
    );
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return (await response.json()) as T;
}
