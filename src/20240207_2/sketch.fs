#version 300 es
precision highp float;
precision highp int;

uniform sampler2D backBuffer;
uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;

in vec2 vUv;
out vec4 outColor;

#define sat(v) clamp(v, 0.0, 1.0)

const float PI = acos(-1.0);

vec3 hash(vec3 v) {
  uvec3 x = floatBitsToUint(v + vec3(0.1, 0.2, 0.3));
  x = (x >> 8 ^ x.zxy) * 0x456789ABu;
  x = (x >> 8 ^ x.zxy) * 0x6789AB45u;
  x = (x >> 8 ^ x.zxy) * 0x89AB4567u;
  return vec3(x) / vec3(-1u);
}

mat2 rot(float a) {
  float s = sin(a), c = cos(a);
  return mat2(c, s, -s, c);
}

void main() {
  vec2 uv = vUv, asp = resolution / min(resolution.x, resolution.y);
  float t = time * 50.0 / 60.0;
  float bt = floor(t);
  float tt = tanh(fract(t) * 3.0);
  
  uv = (uv - 0.5) * asp;
  uv = vec2(atan(uv.y, uv.x) / PI, length(uv));
  uv.y = exp(-uv.y * 3.5);
  vec2 iuv = floor(uv * 64.0);
  float x = mod(floor(iuv.y / 9.0), 2.0);
  iuv = mod(iuv, vec2(32.0, 9.0));

  uint[8] rows = uint[](
    (0x0d8u << 16) + 0x0d8u,
    (0x505u << 16) + 0x505u,
    (0x5fdu << 16) + 0x5fdu,
    (0x7ffu << 16) + 0x7ffu,
    (0x376u << 16) + 0x376u,
    (0x1fcu << 16) + 0x1fcu,
    (0x088u << 16) + 0x088u,
    (0x104u << 16) + 0x104u
  );

  float c;
  if (int(iuv.y) < rows.length()) {
    float s = 0.0 < sign(x) ? iuv.x : 32.0 - iuv.x;
    uint b = rows[int(iuv.y)];
    b = (b << uint(s + t * 2.0)) >> 31;
    c = float(b);
  }

  float center = step(distance((vUv - 0.5) * asp + 0.5, vec2(0.5)), 0.05);
  c = sat(1.0 - c + center);
  
  uv = (vUv - 0.5) * asp;
  uv *= rot(-PI * 0.5 - PI * (bt + tt) * 1.0 / 12.0);
  uv = vec2(atan(uv.y, uv.x) / PI, length(uv));
  if (0.0 < mod(bt, 2.0)) c = mix(1.0 - c, c, step(uv.x * 0.5 + 0.5, fract(1.0 - tt)));
  else                    c = mix(c, 1.0 - c, step(uv.x * 0.5 + 0.5, fract(1.0 - tt)));

  outColor = vec4(vec3(c) * 0.9, 1.0);
}

//----------------------------
// Reference
//----------------------------