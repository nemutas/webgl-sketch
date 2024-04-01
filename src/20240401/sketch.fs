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

float rand(vec2 n) {
  return fract(sin(dot(n, vec2(12.9898, 4.1414))) * 43758.5453);
}

float noise(vec2 p) {
  vec2 ip = floor(p);
  vec2 u = fract(p);
  u = u * u * (3.0 - 2.0 * u);
  float res = mix(mix(rand(ip), rand(ip + vec2(1.0, 0.0)), u.x), mix(rand(ip + vec2(0.0, 1.0)), rand(ip + vec2(1.0, 1.0)), u.x), u.y);
  return res * res;
}

float fbm(vec2 x, int octaves) {
  float v = 0.0;
  float a = 0.5;
  vec2 shift = vec2(100);
	// Rotate to reduce axial bias
  mat2 rot = mat2(cos(0.5), sin(0.5), -sin(0.5), cos(0.50));
  for (int i = 0; i < octaves; ++i) {
    v += a * noise(x);
    x = rot * x * 2.0 + shift;
    a *= 0.5;
  }
  return v;
}

float blendDarken(float base, float blend) {
  return min(blend, base);
}

vec3 blendDarken(vec3 base, vec3 blend) {
  return vec3(blendDarken(base.r, blend.r), blendDarken(base.g, blend.g), blendDarken(base.b, blend.b));
}

vec3 blendDarken(vec3 base, vec3 blend, float opacity) {
  return (blendDarken(base, blend) * opacity + base * (1.0 - opacity));
}

void main() {
  vec2 uv = vUv, asp = resolution / min(resolution.x, resolution.y), suv = (uv * 2.0 - 1.0) * asp;

  if (frame == 1) {
    vec2 auv = abs(suv);
    float c = step(length(suv), 0.1);
    vec3 col = mix(vec3(1), vec3(0), c);
    outColor = vec4(col, 1.0);
    return;
  }

  float disp = fbm(suv * 15.0, 4) * 0.005;

  vec3 t1 = texture(backBuffer, uv).rgb;
  vec3 t2 = texture(backBuffer, uv + vec2(disp, 0.0) / asp).rgb;
  vec3 t3 = texture(backBuffer, uv - vec2(disp, 0.0) / asp).rgb;
  vec3 t4 = texture(backBuffer, uv + vec2(0.0, disp) / asp).rgb;
  vec3 t5 = texture(backBuffer, uv - vec2(0.0, disp) / asp).rgb;

  vec3 col = t1;
  col = blendDarken(col, t2, 0.65);
  col = blendDarken(col, t3, 0.65);
  col = blendDarken(col, t4, 0.65);
  col = blendDarken(col, t5, 0.65);

  outColor = vec4(col, 1.0);
}

//----------------------------
// Reference
//----------------------------
// https://www.youtube.com/live/ggruH0fHPOM?si=M8GiPSTCQOTP_JXs
// https://gist.github.com/patriciogonzalezvivo/670c22f3966e662d2f83
// https://github.com/jamieowen/glsl-blend/blob/master/darken.glsl