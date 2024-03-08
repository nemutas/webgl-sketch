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

#define sat(v) clamp(v, 0.0, 1.0)

const float PI = acos(-1.0);
const float p = 3.0, q = 10.0;

float sdSegment(in vec2 p, in vec2 a, in vec2 b) {
  vec2 pa = p - a, ba = b - a;
  float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
  return length(pa - ba * h);
}

vec3 torusknot(float t) {
  float r = 1.0 + 0.5 * cos(2.0 * PI * p * t);
  float theta = 2.0 * PI * q * t;
  float z = 0.5 * sin(2.0 * PI * p * t);
  return vec3(r * cos(theta), r * sin(theta), z);
}

void main() {
  vec2 uv = vUv, asp = resolution / min(resolution.x, resolution.y), suv = (uv * 2.0 - 1.0) * asp;
  vec4 b = texture(backBuffer, uv);

  float fps = round(1.0 / (time - prevTime));
  // float speed = 0.1;
  float speed = 3.0; // star
  // float speed = 2.0; // rectangle
  vec2 p1 = torusknot(fract(prevTime * speed)).xy;
  vec2 p2 = torusknot(fract(time * speed)).xy;
  float l = smoothstep(0.01, 0.00, sdSegment(suv * 2.0, p1, p2));

  b.rgb *= step(0.1, b.rgb);
  vec3 col = vec3(sat(l), b.rg);
  col = mix(b.rgb, col * 5.0, 0.2);

  outColor = vec4(col, 1.0);
}

//----------------------------
// Reference
//----------------------------
// https://ja.wikipedia.org/wiki/%E3%83%88%E3%83%BC%E3%83%A9%E3%82%B9%E7%B5%90%E3%81%B3%E7%9B%AE
// https://iquilezles.org/articles/distfunctions2d/