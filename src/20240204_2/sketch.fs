#version 300 es
precision highp float;
precision highp int;

uniform sampler2D backBuffer;
uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;

in vec2 vUv;
out vec4 outColor;

uvec3 uhash(vec3 p) {
  uvec3 x = floatBitsToUint(p + vec3(0.1, 0.2, 0.3));
  x = (x >> 8 ^ x.yzx) * 0x456789ABu;
  x = (x >> 8 ^ x.yzx) * 0x6789AB45u;
  x = (x >> 8 ^ x.yzx) * 0x89AB4567u;
  return x;
}

// vec3 hash(vec3 p) {
//   uvec3 x = floatBitsToUint(p + vec3(0.1, 0.2, 0.3));
//   x = (x >> 8 ^ x.yzx) * 0x456789ABu;
//   x = (x >> 8 ^ x.yzx) * 0x6789AB45u;
//   x = (x >> 8 ^ x.yzx) * 0x89AB4567u;
//   return vec3(x) / vec3(-1u);
// }

void main() {
  vec2 uv = vUv, asp = vec2(resolution.x / resolution.y, 1.0);
  uv *= vec2(32.0, 32.0 / asp);

  uint[16] a = uint[](
    uint(time),
    0xbu,
    9u,
    0xbu ^ 9u,
    0xffffffffu,
    0xffffffffu + uint(time),
    floatBitsToUint(floor(time)),
    floatBitsToUint(-floor(time)),
    floatBitsToUint(11.5625),
    // ---
    floatBitsToUint(1.0),
    floatBitsToUint(2.0),
    floatBitsToUint(3.0),
    3u,
    uint(3.0),
    -1u,
    uhash(vec3(floor(time * 10.0))).x
  );

  float c;
  if (a.length() - 1 < int(uv.y)) c = 0.0;
  else {
    uint b = a[int(uv.y)];
    b = (b << uint(uv.x)) >> 31;
    c = float(b);
  }

  if (fract(uv.x) < 0.05 || fract(uv.y) < 0.05) c = 0.1;
  if (int(uv.x) == 1 && fract(uv.x) < 0.05) c = 0.5;
  if (int(uv.x) == 9 && fract(uv.x) < 0.05) c = 0.5;
  if (int(uv.y) == 9 && fract(uv.y) < 0.05) c = 0.5;

  outColor = vec4(vec3(c), 1.0);
}

//----------------------------
// Reference
//----------------------------
// リアルタイムグラフィックスの数学 ― GLSLではじめるシェーダプログラミング
// hash関数：https://www.shadertoy.com/view/XlXcW4