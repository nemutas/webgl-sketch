import * as THREE from 'three'
import { Three } from './core/Three'
import vertexShader from './shader/quad.vs'
import fragmentShader from './shader/output.fs'
import { RawShaderMaterial } from './core/ExtendedMaterials'
import { MainScene } from './MainScene'
import { params } from './Params'
import { mouse2d } from './Mouse2D'

export class Canvas extends Three {
  private mainScene: MainScene
  private output: THREE.Mesh<THREE.PlaneGeometry, THREE.RawShaderMaterial, THREE.Object3DEventMap>

  constructor(canvas: HTMLCanvasElement, mainFs: string, outputFs?: string) {
    super(canvas)
    this.mainScene = new MainScene(this.renderer, mainFs)
    this.output = this.createOutput(outputFs ?? fragmentShader)

    this.disableMatrixAutoUpdate()

    this.bindAssets().then(() => {
      window.addEventListener('resize', this.resize.bind(this))
      this.renderer.setAnimationLoop(this.anime.bind(this))
    })
  }

  private async bindAssets() {
    const texturePath = params.texturePath
    if (texturePath) {
      const loader = new THREE.TextureLoader()
      const texture = await loader.loadAsync(texturePath)
      texture.wrapS = THREE.RepeatWrapping
      texture.wrapT = THREE.RepeatWrapping
      texture.userData.aspect = texture.source.data.width / texture.source.data.height
      Object.assign(this.mainScene.uniforms, { textureUnit: { value: texture } })
    }

    const cubeMapPath = params.cubeMapPath
    if (cubeMapPath) {
      const loader = new THREE.CubeTextureLoader()
      loader.setPath(cubeMapPath)
      const texture = await loader.loadAsync(['px', 'nx', 'py', 'ny', 'pz', 'nz'].map((f) => f + '.webp'))
      Object.assign(this.mainScene.uniforms, { cubeTextureUnit: { value: texture } })
    }
  }

  private createOutput(fs: string) {
    const geometry = new THREE.PlaneGeometry(2, 2)
    const material = new RawShaderMaterial({
      uniforms: {
        source: { value: null },
        resolution: { value: [this.size.width, this.size.height] },
        mouse: { value: mouse2d.position },
        time: { value: 0 },
        frame: { value: 0 },
      },
      vertexShader,
      fragmentShader: fs,
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

  private get uniforms() {
    return this.output.material.uniforms
  }

  private resize() {
    this.uniforms.resolution.value = [this.size.width, this.size.height]
    this.uniforms.time.value = 0
    this.uniforms.frame.value = 0
    this.mainScene.resize()
  }

  private anime() {
    if (params.enableStats) this.stats.update()
    this.updateTime()

    this.mainScene.render(this.time.delta)

    this.uniforms.source.value = this.mainScene.texture
    this.uniforms.mouse.value = mouse2d.position
    this.uniforms.time.value += this.time.delta
    this.uniforms.frame.value += 1
    this.render()
  }
}
