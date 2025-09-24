import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      axios: '/src/lib/axios.ts',
      classnames: '/src/lib/classnames.ts',
      zod: '/src/lib/zod.ts'
    }
  }
})
