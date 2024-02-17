#version 300 es
precision highp float;

uniform sampler2D backBuffer;
uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;

in vec2 vUv;
out vec4 outColor;

vec3 bh;

#define loop(n) for(int i; i < n; i++)

mat2 rot(float a) {
  float s = sin(a), c = cos(a);
  return mat2(c, s, -s, c);
}

vec3 hash(vec3 v) {
  uvec3 x = floatBitsToUint(v + vec3(0.1, 0.2, 0.3));
  x = (x >> 8 ^ x.yzx) * 0x456789ABu;
  x = (x >> 8 ^ x.yzx) * 0x6789AB45u;
  x = (x >> 8 ^ x.yzx) * 0x89AB4567u;
  return vec3(x) / vec3(-1u);
}

float box(vec3 p, vec3 b) {
  p = abs(p) - b;
  return max(p.x, max(p.y, p.z));
}

float sdf(vec3 p) {
  float d = 1e9;

  if (bh.x < 0.7) {
    p.xy *= rot(time * 0.5);
    p.yz *= rot(time * 0.4);
    p.zx *= rot(time * 0.3);
  }

  vec2 cb = vec2(0.45, 1.0);
  d = min(d, box(p, cb.xxy));
  d = min(d, box(p, cb.xyx));
  d = min(d, box(p, cb.yxx));
  d = max(-d, box(p, vec3(0.5)));
  return d;
}

void main() {
  vec2 uv = vUv, asp = resolution / min(resolution.x, resolution.y), suv = (uv * 2.0 - 1.0) * asp;
  suv = floor(suv * 50.0) / 50.0;
  vec3 rd = normalize(vec3(suv, -5.0)), ro = vec3(0.0, 0.0, 5.0);

  float lt = time * 100.0 / 60.0;
  float bt = floor(lt);
  bh = hash(vec3(bt));

  float t;
  loop(64) {
    vec3 p = rd * t + ro;
    float d = sdf(p);
    if (d < 1e-4 || 1e4 < t) break;
    t += d;
  }

  vec3 h = hash(vec3(suv, step(t, 1e4)));
  outColor = vec4(vec3(step(h.x, 0.5)), 1.0);
}

//----------------------------
// Reference
//----------------------------
// https://x.com/matthen2/status/1758504190987423850?s=20