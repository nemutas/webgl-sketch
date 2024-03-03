#version 300 es
precision highp float;

uniform sampler2D backBuffer;
uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;
uniform int frame;

in vec2 vUv;
out vec4 outColor;

#define loop(n) for(int i=0;i<n;i++)

vec3 cyc(vec3 p) {
  vec4 n;
  loop(8) {
    p += sin(p.yzx);
    n = 2.0 * n + vec4(cross(sin(p), cos(p.zxy)), 1.0);
    p *= 2.0;
  }
  return n.xyz / n.w;
}

void main() {
  vec2 uv = vUv, asp = resolution / min(resolution.x, resolution.y), suv = (uv * 2.0 - 1.0) * asp;
  
  vec3 n = cyc(vec3(suv * 3.0, time)) * 0.5 + 0.5;
  outColor = vec4(n, 1.0);
}

//----------------------------
// Reference
//----------------------------