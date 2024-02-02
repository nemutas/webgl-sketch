import * as THREE from 'three'

export class RawShaderMaterial extends THREE.RawShaderMaterial {
  constructor(parameters: THREE.ShaderMaterialParameters) {
    super(parameters)
    this.preprocess()
  }

  private preprocess() {
    if (this.glslVersion === '300 es') {
      this.vertexShader = this.vertexShader.replace('#version 300 es', '')
      this.fragmentShader = this.fragmentShader.replace('#version 300 es', '')
    }
  }
}
