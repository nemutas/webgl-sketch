import * as THREE from 'three'
import { dpr } from '../utils'

export type Options = {
  dpr?: number
  matrixAutoUpdate?: boolean
  type?: THREE.TextureDataType
}

export abstract class FrameBuffer {
  protected readonly scene: THREE.Scene
  protected readonly camera: THREE.OrthographicCamera
  protected readonly renderTarget: THREE.WebGLRenderTarget
  private readonly screen: THREE.Mesh<THREE.PlaneGeometry, THREE.RawShaderMaterial, THREE.Object3DEventMap>

  constructor(
    protected readonly renderer: THREE.WebGLRenderer,
    material: THREE.RawShaderMaterial,
    private options?: Options,
  ) {
    this.scene = new THREE.Scene()
    this.camera = new THREE.OrthographicCamera()
    this.renderTarget = this.createRenderTarget()
    this.screen = this.createScreen(material)

    this.setMatrixAutoUpdate(options?.matrixAutoUpdate ?? false)
  }

  private get devicePixelRatio() {
    return this.options?.dpr ?? dpr()
  }

  private setMatrixAutoUpdate(v: boolean) {
    this.camera.matrixAutoUpdate = v
    this.scene.traverse((o) => (o.matrixAutoUpdate = v))
  }

  protected get size() {
    return { width: this.renderer.domElement.width * this.devicePixelRatio, height: this.renderer.domElement.height * this.devicePixelRatio }
  }

  protected createRenderTarget() {
    const rt = new THREE.WebGLRenderTarget(this.size.width, this.size.height, { type: this.options?.type ?? THREE.UnsignedByteType })
    return rt
  }

  private createScreen(material: THREE.RawShaderMaterial) {
    const geometry = new THREE.PlaneGeometry(2, 2)
    const mesh = new THREE.Mesh(geometry, material)
    this.scene.add(mesh)
    return mesh
  }

  get uniforms() {
    return this.screen.material.uniforms
  }

  resize() {
    this.renderTarget.setSize(this.size.width, this.size.height)
  }

  get texture() {
    return this.renderTarget.texture
  }

  render(..._args: any[]) {
    this.renderer.setRenderTarget(this.renderTarget)
    this.renderer.render(this.scene, this.camera)
  }
}
