#version 300 es
precision highp float;

uniform sampler2D backBuffer;
uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;

in vec2 vUv;
out vec4 outColor;

const float PI = acos(-1.0);

void main() {
  vec2 uv = vUv, asp = vec2(resolution.x / resolution.y, 1.0), suv = (uv * 2.0 - 1.0) * asp;

  vec2 puv = vec2(atan(suv.y, suv.x), length(suv));
  float c = step(fract(puv.x / PI * 5.0 + sin(puv.y * 10.0 - time)), 0.5);

  vec4 b = texture(backBuffer, vUv);
  vec3 col = mix(vec3(c), b.rgb, 0.5);
  
  outColor = vec4(col, 1.0);
}