import * as THREE from 'three'
import { Three } from './core/Three'
import vertexShader from './shader/quad.vs'
import fragmentShader from './shader/output.fs'
import { RawShaderMaterial } from './core/ExtendedMaterials'
import { MainScene } from './MainScene'
import { params } from './Params'

export class Canvas extends Three {
  private mainScene: MainScene
  private output: THREE.Mesh<THREE.PlaneGeometry, THREE.RawShaderMaterial, THREE.Object3DEventMap>

  constructor(canvas: HTMLCanvasElement, fragmentShader: string) {
    super(canvas)
    this.mainScene = new MainScene(this.renderer, fragmentShader)
    this.output = this.createOutput()

    this.disableMatrixAutoUpdate()

    this.bindAssets().then(() => {
      window.addEventListener('resize', this.resize.bind(this))
      this.renderer.setAnimationLoop(this.anime.bind(this))
    })
  }

  private async bindAssets() {
    const path = params.texturePath
    if (path) {
      const loader = new THREE.TextureLoader()
      const texture = await loader.loadAsync(path)
      texture.wrapS = THREE.RepeatWrapping
      texture.wrapT = THREE.RepeatWrapping
      texture.userData.aspect = texture.source.data.width / texture.source.data.height
      Object.assign(this.mainScene.uniforms, { image: { value: texture } })
    }
  }

  private createOutput() {
    const geometry = new THREE.PlaneGeometry(2, 2)
    const material = new RawShaderMaterial({
      uniforms: {
        tSource: { value: null },
      },
      vertexShader,
      fragmentShader,
      glslVersion: '300 es',
    })
    const mesh = new THREE.Mesh(geometry, material)
    this.scene.add(mesh)
    return mesh
  }

  private disableMatrixAutoUpdate() {
    this.camera.matrixAutoUpdate = false
    this.scene.traverse((o) => (o.matrixAutoUpdate = false))
  }

  private resize() {
    this.mainScene.resize()
  }

  private anime() {
    this.updateTime()

    this.mainScene.render(this.time.delta)

    this.output.material.uniforms.tSource.value = this.mainScene.texture
    this.render()
  }
}
