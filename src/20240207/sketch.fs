#version 300 es
precision highp float;
precision highp int;

uniform sampler2D backBuffer;
uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;

in vec2 vUv;
out vec4 outColor;

#define loop(i, n) for(int i; i < n; i++)

vec3 hash(vec3 v) {
  uvec3 x = floatBitsToUint(v + vec3(0.1, 0.2, 0.3));
  x = (x >> 8 ^ x.zxy) * 0x456789ABu;
  x = (x >> 8 ^ x.zxy) * 0x6789AB45u;
  x = (x >> 8 ^ x.zxy) * 0x89AB4567u;
  return vec3(x) / vec3(-1u);
}

void main() {
  vec2 uv = vUv, asp = resolution / min(resolution.x, resolution.y);
  
  float lt = time * 30.0 / 60.0;
  float bt = floor(lt);

  float sp = 32.0;
  loop(i, 3) {
    if(hash(vec3(vec2(0.1, 0.2) * float(i),  + bt)).x < 0.5) break;
    sp *= 1.5;
  }

  vec2 iuv = floor(uv * asp * sp);
  float x = mod(floor(iuv.y / 9.0), 2.0) * 8.0;
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
    uint b = rows[int(iuv.y)];
    float shift = 0.0 < sign(x) ? iuv.x : 32.0 - iuv.x;
    b = (b << uint(shift + x + lt * 4.0)) >> 31;
    c = float(b);
  }

  vec4 b = texture(backBuffer, uv);
  vec3 col = vec3(c, b.rg);

  outColor = vec4(col, 1.0);
}