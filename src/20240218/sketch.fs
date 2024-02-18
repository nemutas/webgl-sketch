#version 300 es
precision highp float;

uniform sampler2D backBuffer;
uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;
uniform int frame;

in vec2 vUv;
out vec4 outColor;

#define sat(v) clamp(v, 0.0, 1.0)

vec3 hash(vec3 v) {
  uvec3 x = floatBitsToUint(v + vec3(0.1, 0.2, 0.3));
  x = (x >> 8 ^ x.yzx) * 0x456789ABu;
  x = (x >> 8 ^ x.yzx) * 0x6789AB45u;
  x = (x >> 8 ^ x.yzx) * 0x89AB4567u;
  return vec3(x) / vec3(-1u);
}

void main() {
  vec2 uv = vUv, asp = resolution / min(resolution.x, resolution.y);
  float n = 100.0;
  vec2 fuv = floor(uv * asp * n + 0.5) / n / asp;
  vec2 px = 1.0 / n / asp;

  float cell;
  if (frame == 1) {
    cell = step(hash(vec3(fuv, 0.1)).x, 0.5);
  } else {
    if (frame % 5 == 0) {
      bool c = 0.5 < texture(backBuffer, fuv).a;
      float r;
      for (float ix = -1.0; ix <= 1.0; ix++) {
        for (float iy = -1.0; iy <= 1.0; iy++) {
          if (ix == 0.0 && iy == 0.0) continue;
          r += texture(backBuffer, fuv + px * vec2(ix, iy)).a;
        }
      }

      if (c) {
        if (r == 2.0 || r == 3.0) cell = 1.0;
        else if (r <= 1.0) cell = 0.0;
        else if (4.0 <= r) cell = 0.0;
      } else {
        cell = float(r == 3.0);
      }
    } else {
      cell = texture(backBuffer, fuv).a;
    }
  }

  vec4 b = texture(backBuffer, uv);
  vec3 col = vec3(cell, b.rg);
  col = mix(col, b.rgb, 0.5);

  vec2 m = floor((mouse * 0.5 + 0.5) * asp * n + 0.5) / n / asp;
  vec2 d = step(distance(m * asp, fuv * asp), px * 2.0);
  if (frame % 5 == 0) {
    cell += float(hash(vec3(d, float(frame))).x < 0.5) * length(d);
    cell = sat(cell);
  }

  outColor = vec4(col, cell);
}

//----------------------------
// Reference
//----------------------------