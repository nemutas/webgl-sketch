#version 300 es
precision highp float;

uniform sampler2D backBuffer;
uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;

in vec2 vUv;
out vec4 outColor;

void main() {
  vec2 uv = vUv;
  outColor = vec4(uv, 1.0, 1.0);
}