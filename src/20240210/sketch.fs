#version 300 es
precision highp float;

uniform sampler2D backBuffer;
uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;

in vec2 vUv;
out vec4 outColor;

#define loop(n) for(int i; i < n; i++)

const float PI = acos(-1.0);

mat2 rot(float a) {
  float s = sin(a), c = cos(a);
  return mat2(c, s, -s, c);
}

vec2 pmod(vec2 p, float r) {
  float n = PI * 2.0 / r;
  float a = atan(p.x, p.y) + n * 0.5;
  a = floor(a / n) * n;
  return rot(a) * p;
}

vec3 hash(vec3 v) {
  uvec3 x = floatBitsToUint(v + vec3(0.1, 0.2, 0.3));
  x = (x >> 8 ^ x.yzx) * 0x456789ABu;
  x = (x >> 8 ^ x.yzx) * 0x6789AB45u;
  x = (x >> 8 ^ x.yzx) * 0x89AB4567u;
  return vec3(x) / vec3(-1u);
}

void main() {
  vec2 uv = vUv;
  vec2 asp = resolution / min(resolution.x, resolution.y);

  float lt = time * 120.0 / 60.0;
  float bt = floor(lt);
  float tt = tanh(fract(lt) * 5.0);
  lt = bt + tt;
  
  vec2 quv = uv * asp, fuv, iuv;
  float n = 2.0;
  loop(4) {
    n += 1.0;
    fuv = fract(quv);
    iuv = floor(quv);
    if (hash(vec3(iuv, bt)).x < 0.5) break;
    quv *= 2.0;
  }

  vec2 suv = fuv * 2.0 - 1.0;
  suv *= rot(PI / n * lt);

  float scale = 1.5 + exp((-n + 3.0) * 3.0) * 0.5;
  vec2 puv = pmod(-suv * scale, n);
  float c = step(fract(puv.x * 10.0 + puv.y * 10.0 - time * 5.0), 0.3) * step(0.5, puv.y) * step(puv.y, 0.7);
  
  vec4 b = texture(backBuffer, vUv);
  vec3 col = vec3(c, b.rg);

  outColor = vec4(col, 1.0);
}

//----------------------------
// Reference
//----------------------------