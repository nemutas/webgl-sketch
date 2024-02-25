#version 300 es
precision highp float;

uniform sampler2D source;
uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;
uniform int frame;

in vec2 vUv;
out vec4 outColor;

void main() {
  vec2 uv = vUv, asp = resolution / min(resolution.x, resolution.y);

  float n = 20.0;
  uv = floor(uv * asp * n) / asp / n;
  vec3 source = texture(source, uv).rgb;
  float gray = dot(source, vec3(0.333333));
  
  outColor = vec4(vec3(gray), 1.0);
}