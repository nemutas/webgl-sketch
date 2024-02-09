export function setSourceLink() {
  const source = document.querySelector<HTMLAnchorElement>('.links .source')
  if (source) {
    const p = location.href.split('/')
    const page = p[p.length - 2]
    source.href = `https://github.com/nemutas/webgl-sketch/blob/main/src/${page}/sketch.fs`
  }
}
