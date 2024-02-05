export function setSourceLink() {
  const source = document.querySelector<HTMLAnchorElement>('.links .source')
  if (source) {
    const page = location.href.split('/').at(-2)
    source.href = `https://github.com/nemutas/webgl-sketch/blob/main/src/${page}/sketch.fs`
  }
}
