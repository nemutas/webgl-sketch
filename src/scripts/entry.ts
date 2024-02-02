import { Canvas } from './Canvas'

const root = location.pathname
  .split('/')
  .filter((s) => s !== '')
  .at(-1)

import(import.meta.env.BASE_URL + `${root}/sketch.fs`).then((file) => {
  new Canvas(document.querySelector<HTMLCanvasElement>('canvas')!, file.default)
})
