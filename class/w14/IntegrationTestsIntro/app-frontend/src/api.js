//By default it will point to the container's url. so it will work only in jenkins.
// If you want to try it locally, then you need to re deploy but pointing to localhost instead.
const BACKEND_URL = import.meta.env.VITE_BACKEND_URL;

export async function api(path, method = 'GET', body = null, token = null) {
  const headers = { 'Content-Type': 'application/json' };
  if (token) headers['Authorization'] = `Bearer ${token}`;

  const res = await fetch(`${BACKEND_URL}${path}`, {
    method,
    headers,
    body: body ? JSON.stringify(body) : null,
  });

  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || `${res.status} ${res.statusText}`);
  }

  return res.json();
}
