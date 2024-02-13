#version 300 es
precision highp float;

uniform sampler2D backBuffer;
uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;

in vec2 vUv;
out vec4 outColor;

#define loop(n) for(int i; i < n; i++)
#define sat(v) clamp(v, 0.0, 1.0)

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
  lt = time * 120.0 / 60.0;
  bt = floor(lt);
  tt = tanh(fract(lt) * 4.0);
  lt = bt + tt;
  float ft = fract(time);

  h = hash(vec3(bt));

  vec2 uv = vUv, asp = resolution / min(resolution.x, resolution.y), suv = (uv * 2.0 - 1.0) * asp;

  suv.x += sin(suv.y * 1000.0 + ft * 100.0 ) * 0.005;
  vec2 fuv = floor(suv * vec2(1, 1.5) * 2.0);
  vec3 fh = hash(vec3(fuv, time));
  if (fh.x < 0.1 && h.x < 0.9) suv.x += sign(fh.y) * 0.05;
  if (0.0 < mod(floor(suv.y * 0.7 - time * 0.5), 2.0)) suv.x += sign(fh.z) * 0.05;

  vec3 rd = normalize(vec3(suv, -5.0 + length(suv) * 2.0)), ro = vec3(0.0, 0.0, 7.0);

  float t, acc;
  loop(64) {
    vec3 p = rd * t + ro;
    if (h.x < 0.9) {
      p.y -= 0.1;
      p.z -= lt * 0.6;
    } else {
      p.xy *= rot(t * 0.5);
      p.z -= ft * 6.0;
    }
    float d = sdf(p);
    if (d < 1e-4 || 1e4 < t) break;
    t += d;
    acc += exp(abs(d) * -300.0);
  }

  float c;
  if (h.y < 0.8) c = acc / 64.0 * 0.5;
  else c = fwidth(acc) * 0.02;

  float sc = step(fract(suv.y * 40.0 - fract(time) * 10.0), 0.8);
  c += sc * 0.07;

  vec4 b = texture(backBuffer, vUv);
  vec3 col = vec3(sat(c), b.rg);
  col = mix(col, b.rgb, 0.5) * 1.08;

  outColor = vec4(col, 1.0);
}

//----------------------------
// Reference
//----------------------------
// https://iquilezles.org/articles/menger/