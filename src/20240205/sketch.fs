#version 300 es
precision highp float;
precision highp int;

uniform sampler2D backBuffer;
uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;

in vec2 vUv;
out vec4 outColor;

void main() {
  vec2 uv = vUv, asp = vec2(resolution.x / resolution.y, 1.0);
  vec2 iuv = floor(uv * vec2(32.0, 32.0 / asp.x));
  float x = floor(iuv.y / 8.0) * 8.0;
  iuv.y = fract(iuv.y / 8.0) * 8.0;

  uint[8] rows = uint[](
    0x00d800d8u,
    0x05050505u,
    0x05fd05fdu,
    0x07ff07ffu,
    0x03760376u,
    0x01fc01fcu,
    0x00880088u,
    0x01040104u
  );

  float t = time * 100.0 / 60.0 + x;

  float c;
  uint b = rows[int(iuv.y)];
  b = (b << uint(iuv.x + t)) >> 31;
  c = float(b);

  outColor = vec4(vec3(c), 1.0);
}