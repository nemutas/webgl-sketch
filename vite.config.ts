import path from 'path'
import { defineConfig } from 'vite'
import glsl from 'vite-plugin-glsl'

export default defineConfig(() => {
  return {
    root: './src',
    publicDir: '../public',
    base: '/webgl-sketch/',
    build: {
      outDir: '../dist',
      rollupOptions: {
        input: [
          path.resolve(__dirname, './src/index.html'),
          path.resolve(__dirname, './src/template/index.html'),
          path.resolve(__dirname, './src/20240202/index.html'),
          path.resolve(__dirname, './src/20240203/index.html'),
          path.resolve(__dirname, './src/20240203_2/index.html'),
          path.resolve(__dirname, './src/20240204/index.html'),
          path.resolve(__dirname, './src/20240204_2/index.html'),
          path.resolve(__dirname, './src/20240205/index.html'),
          path.resolve(__dirname, './src/20240206/index.html'),
          path.resolve(__dirname, './src/20240206_2/index.html'),
          path.resolve(__dirname, './src/20240207/index.html'),
          path.resolve(__dirname, './src/20240207_2/index.html'),
          path.resolve(__dirname, './src/20240208/index.html'),
          path.resolve(__dirname, './src/20240210/index.html'),
        ],
      },
    },
    plugins: [glsl()],
    server: {
      host: true,
    },
  }
})
