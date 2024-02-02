// https://scrapbox.io/0b5vr/Cyclic_Noise

#version 300 es
precision highp float;

uniform sampler2D backBuffer;
uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;

in vec2 vUv;
out vec4 outColor;

#define loop(i, n) for(int i; i < n; i++)

vec3 cyc(vec3 p) {
  vec4 n;
  loop(i, 8) {
    p += sin(p.yzx);
    n = 2.0 * n + vec4(cross(cos(p), sin(p.zxy)), 1.0);
    p *= 2.0;
  }
  return n.xyz / n.w;
}

void main() {
  vec2 uv = vUv, suv = uv * 2.0 - 1.0;
  vec3 color = cyc(vec3(suv * 3.0, time)) * 0.5 + 0.5;

  outColor = vec4(color, 1.0);
}