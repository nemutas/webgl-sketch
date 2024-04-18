import { mouse2d } from './Mouse2D'
import { BackBuffer } from './core/BackBuffer'
import { RawShaderMaterial } from './core/ExtendedMaterials'
import vertexShader from './shader/quad.vs'

export class MainScene extends BackBuffer {
  constructor(renderer: THREE.WebGLRenderer, fragmentShader: string) {
    fragmentShader = fragmentShader.replace('SEED_VALUE', Math.random().toFixed(5))

    const material = new RawShaderMaterial({
      uniforms: {
        backBuffer: { value: null },
        resolution: { value: [renderer.domElement.width, renderer.domElement.height] },
        mouse: { value: mouse2d.position },
        time: { value: 0 },
        prevTime: { value: 0 },
        frame: { value: 0 },
      },
      vertexShader,
      fragmentShader,
      glslVersion: '300 es',
    })

    super(renderer, material)
  }

  resize() {
    super.resize()
    this.uniforms.resolution.value = [this.size.width, this.size.height]
    this.uniforms.time.value = 0
    this.uniforms.prevTime.value = 0
    this.uniforms.frame.value = 0
  }

  render(dt: number) {
    this.uniforms.backBuffer.value = this.backBuffer
    this.uniforms.mouse.value = mouse2d.position
    this.uniforms.prevTime.value = this.uniforms.time.value
    this.uniforms.time.value += dt
    this.uniforms.frame.value += 1
    super.render()
  }
}
