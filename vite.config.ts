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
      // rollupOptions: {
      //   input: [path.resolve(__dirname, './src/index.html'), path.resolve(__dirname, './src/template/index.html')],
      // },
    },
    plugins: [glsl()],
    server: {
      host: true,
    },
  }
})
