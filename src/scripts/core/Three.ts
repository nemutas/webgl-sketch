import * as THREE from 'three'
import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls.js'
import Stats from 'three/examples/jsm/libs/stats.module.js'
import { dpr } from '../utils'

export abstract class Three {
  readonly renderer: THREE.WebGLRenderer
  readonly camera: THREE.PerspectiveCamera
  readonly scene: THREE.Scene
  private clock: THREE.Clock
  private _stats?: Stats
  private _controls?: OrbitControls
  readonly time = { delta: 0, elapsed: 0 }

  constructor(canvas: HTMLCanvasElement) {
    this.renderer = this.createRenderer(canvas)
    this.camera = this.createCamera()
    this.scene = this.createScene()
    this.clock = new THREE.Clock()

    window.addEventListener('resize', this._resize.bind(this))
  }

  private createRenderer(canvas: HTMLCanvasElement) {
    const renderer = new THREE.WebGLRenderer({ canvas, antialias: true, alpha: true })
    renderer.setSize(window.innerWidth, window.innerHeight)
    renderer.setPixelRatio(dpr())
    return renderer
  }

  private createCamera() {
    const camera = new THREE.PerspectiveCamera(50, this.size.aspect, 0.01, 100)
    camera.position.z = 5
    return camera
  }

  private createScene() {
    const scene = new THREE.Scene()
    return scene
  }

  protected get stats() {
    if (!this._stats) {
      this._stats = new Stats()
      document.body.appendChild(this._stats.dom)
    }
    return this._stats
  }

  private _resize() {
    const { innerWidth: width, innerHeight: height } = window
    this.renderer.setSize(width, height)
    // this.camera.aspect = width / height
    // this.camera.updateProjectionMatrix()
  }

  get size() {
    const { width, height } = this.renderer.domElement
    return { width, height, aspect: width / height }
  }

  protected updateTime() {
    this.time.delta = this.clock.getDelta()
    this.time.elapsed = this.clock.getElapsedTime()
  }

  protected get controls() {
    if (!this._controls) {
      this._controls = new OrbitControls(this.camera, this.renderer.domElement)
    }
    return this._controls
  }

  protected coveredScale(imageAspect: number) {
    const screenAspect = this.size.aspect
    if (screenAspect < imageAspect) return [screenAspect / imageAspect, 1]
    else return [1, imageAspect / screenAspect]
  }

  protected render() {
    this.renderer.setRenderTarget(null)
    this.renderer.render(this.scene, this.camera)
  }

  dispose() {
    this.renderer.setAnimationLoop(null)
    this.renderer.dispose()
  }
}
