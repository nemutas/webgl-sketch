#version 300 es
precision highp float;

uniform sampler2D backBuffer;
uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;
uniform int frame;

in vec2 vUv;
out vec4 outColor;

void main() {
  vec2 uv = vUv, asp = resolution / min(resolution.x, resolution.y);
  outColor = vec4(uv, 0.0, 1.0);
}

//----------------------------
// Reference
//----------------------------