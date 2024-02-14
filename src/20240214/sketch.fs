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

mat2 rot(float a) {
  float s = sin(a), c = cos(a);
  return mat2(c, s, -s, c);
}

vec3 hash(vec3 v) {
  uvec3 x = floatBitsToUint(v + vec3(0.1, 0.2, 0.3));
  x = (x >> 8 ^ x.zxy) * 0x456789ABu;
  x = (x >> 8 ^ x.zxy) * 0x6789AB45u;
  x = (x >> 8 ^ x.zxy) * 0x89AB4567u;
  return vec3(x) / vec3(-1u);
}

float box(vec3 p, vec3 b) {
  p = abs(p) - b;
  return max(p.x, max(p.y, p.z));
  // return length(max(p, 0.0)) + min(max(p.x, max(p.y, p.z)), 0.0);
}

float sdf(vec3 p) {
  p.xy *= rot(-p.z * PI * 0.03);
  p.z += fract(-time) * 3.0;

  p = mod(p, 3.0) - 1.5;
  
  float d = 1e4;
  d = min(d, length(p.xz) - 0.05);
  d = min(d, length(p.yz) - 0.05);
  d = min(d, length(p.xy) - 0.05);
  d = max(d, box(p, vec3(0.5)));
  return d;
}

void main() {
  vec2 uv = vUv, asp = resolution / min(resolution.x, resolution.y), suv = (uv * 2.0 - 1.0) * asp;
  vec3 rd = normalize(vec3(suv, -2.0 - length(suv) * 0.5)), ro = vec3(0.0, 0.0, 10.0);

  float t, acc;
  loop(64) {
    vec3 p = rd * t + ro;
    float d = sdf(p) * 0.7;
    if (d < 1e-4 || 1e5 < t) break;
    t += d;
    acc += exp(abs(d) * -20.0);
  }

  float c = acc / 64.0;

  vec4 b = texture(backBuffer, uv);
  vec3 col = vec3(sat(c), b.rg);
  col = mix(col, b.rgb, 0.3);

  outColor = vec4(col, 1.0);
}

//----------------------------
// Reference
//----------------------------