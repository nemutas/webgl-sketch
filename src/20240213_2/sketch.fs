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

float lt, bt, tt;
vec3 h;

mat2 rot(float a) {
  float s = sin(a), c = cos(a);
  return mat2(c, s, -s, c);
}

vec3 hash(vec3 v) {
  uvec3 x = floatBitsToUint(v + vec3(0.1, 0.2, 0.3));
  x = (x >> 8 ^ x.zxy) * 0x457890ABu;
  x = (x >> 8 ^ x.zxy) * 0x7890AB45u;
  x = (x >> 8 ^ x.zxy) * 0x90AB4578u;
  return vec3(x) / vec3(-1u);
}

vec2 pmod(vec2 p, float r) {
  float n = PI * 2.0 / r;
  float a = atan(p.x, p.y) + n * 0.5;
  a = floor(a / n) * n;
  return rot(a) * p;
}

float box(vec3 p, vec3 b) {
  p = abs(p) - b;
  return length(max(p, 0.0)) + min(max(p.x, max(p.y, p.z)), 0.0);
}

float sdCross(vec3 p) {
  float da = max(p.x, p.y);
  float db = max(p.y, p.z);
  float dc = max(p.z, p.x);
  return min(da, min(db, dc)) - 1.0;
}

float sdf(vec3 p) {
  p.y -= 0.05;
  p.z -= lt * 0.5;

  p = mod(p, 2.0) - 1.0;

  float b = box(p, vec3(1.0));
  float s = 1.0;
  loop(3) {
    vec3 a = mod(p * s, 2.0) - 1.0;
    s *= 4.0;
    vec3 r = abs(1.0 - 4.0 * abs(a));
    float c = sdCross(r) / s;
    b = max(b, c);
  }

  return b;
}

void main() {
  vec2 uv = vUv, asp = resolution / min(resolution.x, resolution.y), suv = (uv * 2.0 - 1.0) * asp;
  vec3 rd = normalize(vec3(suv, -5.0)), ro = vec3(0.0, 0.0, 7.0);

  lt = time * 80.0 / 60.0;
  bt = floor(lt);
  tt = tanh(fract(lt) * 4.0);
  lt = bt + tt;

  h = hash(vec3(bt));

  float t, acc;
  loop(64) {
    vec3 p = rd * t + ro;
    float d = sdf(p);
    if (d < 1e-4 || 1e4 < t) break;
    t += d;
    acc += exp(abs(d) * -300.0);
  }

  float c = acc / 64.0 * 0.5;

  outColor = vec4(vec3(c), 1.0);
}

//----------------------------
// Reference
//----------------------------
// https://iquilezles.org/articles/menger/