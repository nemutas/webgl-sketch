#version 300 es
precision highp float;

uniform sampler2D tSource;

in vec2 vUv;
out vec4 outColor;

void main() {
  outColor = vec4(texture(tSource, vUv).rgb, 1.0);
}