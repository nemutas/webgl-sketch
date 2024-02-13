#version 300 es
precision highp float;

uniform sampler2D backBuffer;
uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;

in vec2 vUv;
out vec4 outColor;

vec2 hash(vec2 v) {
  uvec2 x = floatBitsToUint(v + vec2(0.1, 0.2));
  x = (x >> 8 ^ x.yx) * 0x456789ABu;
  x = (x >> 8 ^ x.yx) * 0x6789AB45u;
  x = (x >> 8 ^ x.yx) * 0x89AB4567u;
  return vec2(x) / vec2(-1u);
}

mat2 rot(float a) {
  float s = sin(a), c = cos(a);
  return mat2(c, s, -s, c);
}

vec3 voronoi(vec2 p) {
  vec2 n = floor(p + 0.5);
  float dist = sqrt(2.0);
  vec2 id;
  for (float j = -1.0; j <= 1.0; j++) {
    for (float i = -1.0; i <= 1.0; i++) {
      vec2 grid = n + vec2(i, j);
      vec2 h = hash(grid) - 0.5;
      vec2 jitter = rot(time * sign(h).x * h.y * 5.0) * h;
      if (distance(grid + jitter, p) <= dist) {
        dist = distance(grid + jitter, p);
        id = grid;
      }
    }
  }
  return vec3(id, dist);
}

void main() {
  vec2 uv = vUv, asp = resolution / min(resolution.x, resolution.y);
  vec3 v = voronoi(uv * asp * 10.0);
  vec2 vid = hash(v.xy);
  vec3 col = vec3(vid, 0.9) + pow(v.z, 5.0);
  outColor = vec4(col, 1.0);
}

//----------------------------
// Reference
//----------------------------
// リアルタイムグラフィックスの数学 ― GLSLではじめるシェーダプログラミング
// https://amzn.asia/d/9rQTjZC