#version 300 es
precision highp float;

uniform sampler2D backBuffer;
uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;

in vec2 vUv;
out vec4 outColor;

#define loop(n) for(int i; i < n; i++)

mat2 rot(float a) {
  float s = sin(a), c = cos(a);
  return mat2(c, s, -s, c);
}

float box(vec3 p, vec3 b) {
  p = abs(p) - b;
  return max(p.x, max(p.y, p.z));
}

float sdf(vec3 p) {
  float final = 1e9;

  p.xz *= rot(time);
  p.zy *= rot(time);
  
  p = abs(p);
  vec3 edge = 0.5 - max(vec3(0.5), p) * 0.2;
  float b1 = box(p, vec3(2.5, edge.x, edge.x));
  float b2 = box(p, vec3(edge.y, 2.5, edge.y));
  float b3 = box(p, vec3(edge.z, edge.z, 2.5));
  final = min(b1, min(b2, b3));
  return final;
}

void main() {
  vec2 uv = vUv, asp = resolution / min(resolution.x, resolution.y), suv = (uv - 0.5) * asp * 2.0;
  vec3 rd = normalize(vec3(suv, -4.0 + length(suv) * 5.0)), ro = vec3(0.0, 0.0, 8.0);

  float t, acc;
  loop(64) {
    vec3 p = rd * t + ro;
    float d = sdf(p);
    d = max(abs(d), 0.01);
    acc += exp(-d * 3.0);
    t += d;
  }

  acc *= 0.03;
  vec3 col = vec3(0.00, 0.56, 1.00) * acc;
  outColor = vec4(col, 1.0);
}

//----------------------------
// Reference
//----------------------------
// 魔法使いになりたい人のためのシェーダーライブコーディング入門
// https://qiita.com/kaneta1992/items/21149c78159bd27e0860