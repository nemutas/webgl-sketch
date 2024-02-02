import { Canvas } from './Canvas'

export function createCanvas(fragmentShader: string) {
  new Canvas(document.querySelector<HTMLCanvasElement>('canvas')!, fragmentShader)
}
