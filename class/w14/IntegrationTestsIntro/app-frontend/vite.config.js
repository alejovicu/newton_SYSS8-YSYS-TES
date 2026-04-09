import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  server: {
    host: true,
    port: 80,
    hmr: {
      port: 80
    },
    allowedHosts:['localhost','app-frontend','jenkins','app-backend']
  }
})