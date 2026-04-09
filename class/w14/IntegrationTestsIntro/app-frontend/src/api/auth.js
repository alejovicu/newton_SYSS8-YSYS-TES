import { api } from "../api.js"

export const signup = (username, password) => api("/signup", "POST", { username, password })
export const login = (username, password) => api("/login", "POST", { username, password })
