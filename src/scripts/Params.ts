class Params {
  private data_texture?: string
  private data_dpr?: string
  private data_stats?: string

  constructor() {
    const canvas = document.querySelector<HTMLCanvasElement>('canvas')!
    this.data_texture = canvas.dataset.texture
    this.data_dpr = canvas.dataset.dpr
    this.data_stats = canvas.dataset.stats
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

  get enableStats() {
    return Boolean(this.data_stats)
  }
}

export const params = new Params()
