#version 300 es
precision highp float;

uniform sampler2D backBuffer;
uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;
uniform int frame;

in vec2 vUv;
out vec4 outColor;

#define loop(n) for(int i; i < n; i++)

const float PI = acos(-1.0);

mat2 rot(float a) {
  float s = sin(a), c = cos(a);
  return mat2(c, s, -s, c);
}

void main() {
  vec2 uv = vUv, asp = resolution / min(resolution.x, resolution.y), suv = (uv * 2.0 - 1.0) * asp;

  float lt = time * 30.0 / 60.0;
  float bt = floor(lt);
  float tt = tanh(fract(lt) * 5.0);

  suv *= 1.0 - length(suv) * (1.0 - tt);

  vec2 z = suv * rot(PI * 0.5) * 0.5, c = vec2(0.25, 0.58) * (tt * tt * 0.05 + 0.95);
  if (mod(bt, 2.0) == 0.0) {
    z = vec2(0) + ((1.0 - tt * tt) * 0.5), c = suv + vec2(-0.5, 0.0);
  }

  float j;
  loop(100) {
    z = vec2(z.x * z.x - z.y * z.y,  2.0 * z.x * z.y) + c;
    if (4.0 < dot(z, z)) break;
    j++;
  }

  float lum = j / 80.0;
  lum = smoothstep(0.1, 1.0, lum);

  outColor = vec4(vec3(lum), 1.0);
}

//----------------------------
// Reference
//----------------------------
// z(n+1) = z(n)^2 + c. z, c: complex number
// https://wgld.org/d/glsl/g005.html
// https://wgld.org/d/glsl/g006.html