import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  server: {
    host: '127.0.0.1',
    // Keep the original Host: the API resolves the gym from the browser's domain.
    proxy: Object.fromEntries(['/api', '/Auth', '/Workouts'].map(path => [path, {
      target: process.env.API_PROXY_TARGET ?? 'https://localhost:5001', changeOrigin: false,
      secure: false, // Local ASP.NET development certificate; the production build uses same-origin requests.
    }])),
  },
});
