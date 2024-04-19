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

#define h21(f2, f1) hash(vec3(f2, f1))

vec3 hash(vec3 v) {
  uvec3 x = floatBitsToUint(v + vec3(0.1, 0.2, 0.3));
  x = (x >> 8 ^ x.yzx) * 0x456789ABu;
  x = (x >> 8 ^ x.yzx) * 0x6789AB45u;
  x = (x >> 8 ^ x.yzx) * 0x89AB4567u;
  return vec3(x) / vec3(-1u);
}

void main() {
  vec2 uv = vUv, asp = resolution / min(resolution.x, resolution.y);

  float lt = time * 180.0 / 60.0;
  float bt = floor(lt);
  float tt = tanh(fract(lt) * 5.0);
  lt = bt + tt;

  vec3 h1 = h21(floor(uv * asp * vec2(1000, 0)), floor(time * 30.0));
  vec3 h2 = h21(floor(floor(uv * asp * 400.0)), floor(time * 30.0));
  float c = step(h1.x, 0.995);
  c *= step(h2.x, 0.9999);
  c = 1.0 - clamp(c, 0.0, 1.0);
  c *= 0.2;

  outColor = vec4(vec3(c), 1.0);
}

//----------------------------
// Reference
//----------------------------