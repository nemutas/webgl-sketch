export function setSourceLink() {
  const source = document.querySelector<HTMLAnchorElement>('.links .source')
  if (source) {
    const p = location.href.split('/')
    const page = p[p.length - 2]
    source.href = `https://github.com/nemutas/webgl-sketch/blob/main/src/${page}/sketch.fs`
  }
}

export function dpr() {
  let dpr = window.devicePixelRatio
  const canvas = document.querySelector<HTMLCanvasElement>('canvas')
  if (canvas?.dataset.dpr) dpr = Number(canvas.dataset.dpr)
  return dpr
}
