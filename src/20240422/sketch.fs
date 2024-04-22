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

const float SEED = SEED_VALUE;
const float PI = acos(-1.0);

#define h21(f2, f1) hash(vec3(f2, f1))

vec3 hash(vec3 v) {
  uvec3 x = floatBitsToUint(v + vec3(0.1, 0.2, 0.3));
  x = (x >> 8 ^ x.yzx) * 0x456789ABu;
  x = (x >> 8 ^ x.yzx) * 0x6789AB45u;
  x = (x >> 8 ^ x.yzx) * 0x89AB4567u;
  return vec3(x) / vec3(-1u);
}

mat2 rot(float a) {
  float s = sin(a), c = cos(a);
  return mat2(c, s, -s, c);
}

void main() {
  vec2 uv = vUv, asp = resolution / min(resolution.x, resolution.y), suv = uv * 2.0 - 1.0;

  float lt = time * 100.0 / 60.0;
  float bt = floor(lt);
  float tt = tanh(fract(lt) * 3.0);
  lt = bt + tt;

  vec2 quv = suv * asp, fuv, iuv;
  quv *= rot(PI * 0.1 + quv.y * 0.315);
  quv.x -= lt * 0.5;

  vec3 h;
  float i, n = 4.0;
  for (; i < n; i++) {
    fuv = fract(quv);
    iuv = floor(quv);

    if ((h = h21(iuv, 0.2)).x < 0.3) break;

    quv *= 2.0;
  }

  i = clamp(i, 0.0, n - 1.0);
  float qScale = pow(2.0, i);

  vec2 sfuv = fuv * 2.0 - 1.0, auv = abs(sfuv);
  float edge = step(auv.x, 1.0 - 0.01 * qScale) * step(auv.y, 1.0 - 0.01 * qScale);
  float disc = smoothstep(0.015 * qScale, (0.015 + 0.005) * qScale, length(sfuv));
  float marker = edge * disc;
  if (h.y < 0.1) marker = 0.0;

  vec3 c = mix(vec3(0.01, 0.04, 0.09), vec3(0.99, 0.97, 0.91), marker);

  vec4 b = texture(backBuffer, uv);
  vec3 col = mix(b.rgb, c, 0.5);

  outColor = vec4(vec3(marker), 1.0);
  outColor = vec4(col, 1.0);
}

//----------------------------
// Reference
//----------------------------