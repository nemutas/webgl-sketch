import { FrameBuffer, Options } from './FrameBuffer'

export abstract class BackBuffer extends FrameBuffer {
  private readonly rt2: THREE.WebGLRenderTarget
  private prev: THREE.WebGLRenderTarget
  private current: THREE.WebGLRenderTarget

  constructor(renderer: THREE.WebGLRenderer, material: THREE.RawShaderMaterial, options?: Options) {
    super(renderer, material, options)

    this.rt2 = this.createRenderTarget()
    this.prev = this.renderTarget
    this.current = this.rt2
  }

  resize() {
    super.resize()
    this.rt2.setSize(this.size.width, this.size.height)
  }

  get backBuffer() {
    return this.prev.texture
  }

  private swap() {
    this.current = this.current === this.renderTarget ? this.rt2 : this.renderTarget
    this.prev = this.current === this.renderTarget ? this.rt2 : this.renderTarget
  }

  render(..._args: any[]) {
    this.renderer.setRenderTarget(this.current)
    this.renderer.render(this.scene, this.camera)

    this.swap()
  }
}
