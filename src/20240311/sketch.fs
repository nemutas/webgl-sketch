#version 300 es
precision highp float;

uniform sampler2D backBuffer;
uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;
uniform float prevTime;
uniform int frame;

in vec2 vUv;
out vec4 outColor;

vec3 hash(vec3 v) {
  uvec3 x = floatBitsToUint(v + vec3(0.1, 0.2, 0.3));
  x = (x >> 8 ^ x.yzx) * 0x456789ABu;
  x = (x >> 8 ^ x.yzx) * 0x6789AB45u;
  x = (x >> 8 ^ x.yzx) * 0x89AB4567u;
  return vec3(x) / vec3(-1u);
}

float pattern(vec2 uv, float n) {
  float c = 0.0;
  vec2 auv = abs(uv * 2.0 - 1.0);
  float s = 1.0 / n;
  for (float i = 1.0; i < n; i++) {
    float a = step(auv.x, 1.0 - s * i) * step(auv.y, 1.0 - s * i);
    c = mix(c, mod(i, 2.0) == 0.0 ? 0.0 : 1.0, a);
  }
  return c;
}

void main() {
  vec2 uv = vUv, asp = resolution / min(resolution.x, resolution.y), suv = (uv * 2.0 - 1.0) * asp;

  float bt = floor(time * 30.0 / 60.0);
  
  vec2 quv = uv * asp * 1.1, fuv, iuv;
  vec3 h;
  for (int i = 0; i < 5; i++) {
    fuv = fract(quv);
    iuv = floor(quv);
    if (0 < i && (h = hash(vec3(iuv, bt))).x < (0.2 + float(i) * 0.1)) break;
    quv *= 2.0;
  }

  // 1, 3, 4, 5, 6
  float n = ceil(h.y * 6.0);
  if (n == 2.0) n = 1.0;
  float c = pattern(fuv, n);

  if (h.z < 0.5) c = 1.0 - c;

  vec3 col = mix(vec3(1) * 0.93, vec3(1), c);

  vec3 b = texture(backBuffer, uv).rgb;
  col = mix(b, col, 0.5);

  outColor = vec4(col, 1.0);
}

//----------------------------
// Reference
//----------------------------
// https://unshift.jp/