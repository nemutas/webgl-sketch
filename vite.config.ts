import path from 'path'
import { defineConfig } from 'vite'
import glsl from 'vite-plugin-glsl'

function resolvedPath(root: string) {
  const r = root !== '' ? `/${root}` : ''
  return path.resolve(__dirname, `./src${r}/index.html`)
}

export default defineConfig(() => {
  return {
    root: './src',
    publicDir: '../public',
    base: '/webgl-sketch/',
    build: {
      outDir: '../dist',
      rollupOptions: {
        // prettier-ignore
        input: [
          '',
          'template', 'template_postprocessing',
          '20240202', '20240203', '20240203_2', '20240204', '20240204_2', '20240205', '20240206', '20240206_2', '20240207', '20240207_2', '20240208', '20240210', '20240211', '20240213', '20240213_2', '20240214', '20240217', '20240218', '20240219', '20240221', '20240225',
          '20240303', '20240304', '20240306', '20240306_2', '20240308', '20240311', '20240312',
          '20240401', '20240418', '20240419', '20240422', '20240425',
        ].map((str) => resolvedPath(str)),
      },
    },
    plugins: [glsl()],
    server: {
      host: true,
    },
  }
})
