#version 300 es
precision highp float;

uniform sampler2D source;

in vec2 vUv;
out vec4 outColor;

void main() {
  outColor = vec4(texture(source, vUv).rgb, 1.0);
}