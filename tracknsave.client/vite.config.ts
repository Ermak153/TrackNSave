import { fileURLToPath, URL } from 'node:url';
import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-vue';
import basicSsl from '@vitejs/plugin-basic-ssl'

export default defineConfig({
  plugins: [
    plugin(),
    basicSsl()
  ],
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url))
        }
    },
    server: {
        proxy: {
          '/api': {
          target: 'https://localhost:8081',
          changeOrigin: true,
          secure: false,
      }
    },
      watch: {
        usePolling: true,
      },
        host: '0.0.0.0',
        port: 5173
    }
})
