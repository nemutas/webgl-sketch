class Mouse2D {
  readonly position: [number, number] = [99999, 99999]
  readonly prevPosition: [number, number] = [99999, 99999]

  constructor() {
    window.addEventListener('mousemove', this.handleMouseMove)
    window.addEventListener('touchmove', this.handleTouchMove)
  }

  private handleMouseMove = (e: MouseEvent) => {
    this.prevPosition[0] = this.position[0]
    this.prevPosition[1] = this.position[1]
    this.position[0] = (e.clientX / window.innerWidth) * 2 - 1
    this.position[1] = -1 * ((e.clientY / window.innerHeight) * 2 - 1)
  }

  private handleTouchMove = (e: TouchEvent) => {
    const { pageX, pageY } = e.touches[0]
    this.prevPosition[0] = this.position[0]
    this.prevPosition[1] = this.position[1]
    this.position[0] = (pageX / window.innerWidth) * 2 - 1
    this.position[1] = -1 * ((pageY / window.innerHeight) * 2 - 1)
  }

  lerp(t: number) {
    this.prevPosition[0] = this.prevPosition[0] * (1 - t) + this.position[0] * t
    this.prevPosition[1] = this.prevPosition[1] * (1 - t) + this.position[1] * t
    return [this.position[0] - this.prevPosition[0], this.position[1] - this.prevPosition[1]]
  }

  dispose() {
    window.removeEventListener('mousemove', this.handleMouseMove)
    window.removeEventListener('touchmove', this.handleTouchMove)
  }
}

export const mouse2d = new Mouse2D()
