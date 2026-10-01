let token: string | null = null;
let refreshTask: Promise<boolean> | null = null;
export class ApiError extends Error {
  constructor(public status: number, message: string) { super(message); }
}
export const setToken = (value: string | null) => { token = value; };

export async function restoreSession(): Promise<boolean> {
  if (refreshTask) return refreshTask;
  refreshTask = (async () => {
    const response = await fetch('/api/session/refresh', { method: 'POST', credentials: 'same-origin' });
    if (!response.ok) { token = null; return false; }
    token = (await response.json()).token;
    return true;
  })().finally(() => { refreshTask = null; });
  return refreshTask;
}

export async function api<T>(path: string, method = 'GET', body?: unknown, retry = true): Promise<T> {
  const response = await fetch(path, {
    method, credentials: 'same-origin',
    headers: { ...(body !== undefined ? { 'Content-Type': 'application/json' } : {}), ...(token ? { Authorization: `Bearer ${token}` } : {}) },
    body: body === undefined ? undefined : JSON.stringify(body),
  });
  if (response.status === 401 && retry && token && !path.startsWith('/api/session')) {
    if (await restoreSession()) return api(path, method, body, false);
    window.dispatchEvent(new Event('session-expired'));
  }
  if (!response.ok) {
    const error = await response.json().catch(() => ({}));
    throw new ApiError(response.status, error.detail ?? error.message ?? error.error ??
      (error.errors ? Object.values(error.errors).flat().join(' ') : null) ??
      ({ 401: 'Войдите в аккаунт.', 403: 'У вас нет доступа к этому действию.', 429: 'Слишком много запросов. Попробуйте через минуту.' }[response.status] ?? 'Не удалось выполнить запрос.'));
  }
  const text = await response.text();
  return text ? JSON.parse(text) : undefined as T;
}
