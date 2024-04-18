#version 300 es
precision highp float;

uniform sampler2D backBuffer;
uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;
uniform float prevTime;
uniform int frame;
uniform float seed;
uniform sampler2D textureUnit;

in vec2 vUv;
out vec4 outColor;

#define h21(v2, s) hash(vec3(v2, s))
const float PI = acos(-1.0);

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

float easeInOutCubic(float x) {
  return x < 0.5 ? 4.0 * x * x * x : 1.0 - pow(-2.0 * x + 2.0, 3.0) / 2.0;
}

void main() {
  vec2 uv = vUv, asp = resolution / min(resolution.x, resolution.y), suv = (uv * 2.0 - 1.0) * asp;
  ivec2 tSize = textureSize(textureUnit, 0);
  float tAspect = float(tSize.x) / float(tSize.y);
  vec4 b = texture(backBuffer, uv);
  float rnd = floor(seed * 100.0) / 100.0;

  float lt = time * 20.0 / 60.0;
  float bt = floor(lt);
  float tt = easeInOutCubic(tanh(fract(lt) * 3.0));
  // float tt = tanh(fract(lt) * 5.0);
  lt = bt + tt;

  vec2 quv = suv, fuv, iuv;
  vec3 h;
  for (int i = 0; i < 4; i++) {
    fuv = fract(quv);
    iuv = floor(quv);
    if ((h = h21(iuv, float(i) + bt + rnd)).x < 0.4) break;
    quv *= 2.0;
  }

  vec2 mapUv = fuv;
  mapUv += (h21(iuv, floor(time * 10.0)).xy * 2.0 - 1.0) * 0.02;
  mapUv = (mapUv - 0.5) * rot(floor(h21(iuv, bt + rnd).x * 4.0) / 4.0 * PI * 2.0) + 0.5;
  mapUv.x = floor(h.y * tAspect) / tAspect + mapUv.x / tAspect;
  vec4 text = texture(textureUnit, mapUv);
  vec3 txt = text.rgb * text.a;

  vec2 auv = abs(fuv * 2.0 - 1.0);
  txt *= step(auv.x, 0.9) * step(auv.y, 0.9);

  txt *= 1.0 - h21(uv, time).x * 0.3;
  txt *= 1.0 - step(h21(floor(uv * vec2(1000, 0)), time).x, 0.5) * 0.3;

  txt *= step(h.z, tt);

  outColor = vec4(mix(b.rgb, txt, 0.2), 1.0);
}

//----------------------------
// Reference
//----------------------------