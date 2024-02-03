#version 300 es
precision highp float;

uniform sampler2D backBuffer;
uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;

in vec2 vUv;
out vec4 outColor;

#define sat(v) clamp(v, 0.0, 1.0)

const float PI = acos(-1.0);

mat2 rot(float a) {
  float s = sin(a), c = cos(a);
  return mat2(c, s, -s, c);
}

void main() {
  vec2 uv = vUv, asp = vec2(resolution.x / resolution.y, 1.0), suv = (uv * 2.0 - 1.0) * asp;
  
  vec4 b = texture(backBuffer, uv);

  float lt = time * 120.0 / 60.0; // 120bpm
  float bt = floor(lt);
  float tt = tanh(fract(lt) * 5.0);
  lt = bt + tt;

  suv *= rot(PI * 0.25 * lt);

  vec2 auv = abs(suv), ruv = abs(rot(PI * 0.25) * suv);
  float c = step(auv.x + auv.y * 0.18, 0.12) + step(auv.y + auv.x * 0.18, 0.12);
  c += step(ruv.x + ruv.y * 0.18, 0.08) + step(ruv.y + ruv.x * 0.18, 0.08);
  auv = abs(rot(PI * 1.0 / 6.2) * suv), ruv = abs(rot(PI * 1.0 / 2.95) * suv);
  c *= step(0.008, auv.x) * step(0.008, auv.y) * step(0.008, ruv.x) * step(0.008, ruv.y);

  vec3 col = vec3(sat(c), b.rg);
  col = mix(col, b.rgb, 0.5);
  
  outColor = vec4(col, 1.0);
}