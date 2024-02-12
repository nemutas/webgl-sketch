class Params {
  private data_texture?: string
  private data_dpr?: string

  constructor() {
    const canvas = document.querySelector<HTMLCanvasElement>('canvas')!
    this.data_texture = canvas.dataset.texture
    this.data_dpr = canvas.dataset.dpr
  }

  get texturePath() {
    if (this.data_texture) {
      return import.meta.env.BASE_URL + `textures/${this.data_texture}`
    } else {
      return null
    }
  }

  get dpr() {
    const d = Number(this.data_dpr ?? window.devicePixelRatio)
    return Math.min(d, 2)
  }
}

export const params = new Params()
