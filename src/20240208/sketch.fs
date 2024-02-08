#version 300 es
precision highp float;
precision highp int;

uniform sampler2D backBuffer;
uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;

in vec2 vUv;
out vec4 outColor;

const float PI = acos(-1.0);

mat2 rot(float a) {
  float s = sin(a), c = cos(a);
  return mat2(c, s, -s, c);
}

void main() {
  vec2 uv = vUv, asp = resolution / min(resolution.x, resolution.y);

  vec2 suv = (uv * 2.0 - 1.0) * asp * 1.5;
  vec3 pos = vec3(suv, (sqrt(1.0 - dot(suv, suv)) + 0.5) / 0.5);
  vec3 normal = normalize(pos);
  if (0.0 < step(length(suv), 1.0)) uv += normal.xy * (1.0 - normal.z);

  vec2 iuv = floor(uv * 64.0 * asp);
  float r = floor(iuv.y / 9.0);
  iuv = mod(iuv, vec2(32.0, 9.0));

  uint src = 0xbu;
  uint[9] rows = uint[](
    (0x0d8u << 16) + 0x202u,
    (0x505u << 16) + 0x104u,
    (0x5fdu << 16) + 0x3feu,
    (0x7ffu << 16) + 0x7ffu,
    (0x376u << 16) + 0x777u,
    (0x1fcu << 16) + 0x5fdu,
    (0x088u << 16) + 0x489u,
    (0x104u << 16) + 0x104u,
    0u
  );

  float t1 = time * 100.0 / 60.0;
  float t2 = t1 * 0.5;
  float bt = floor(t2);
  float tt = tanh(fract(t2) * 5.0);

  suv *= rot(-PI * 0.5 - PI / 12.0 * (bt + tt));
  float a = atan(suv.y, suv.x) / PI * 0.5 + 0.5;

  float c;
  if (int(iuv.y) < rows.length()) {
    uint b = rows[int(iuv.y)];
    if (a < 1.0 - tt) b = ~b;
    if (0.0 < mod(bt, 2.0)) b = ~b;
    float shift;
    if (0.0 < mod(floor(t1), 2.0)) shift += 16.0;
    if (0.0 < mod(r, 2.0)) shift += 8.0;
    b = (b << uint(iuv.x + shift + t1)) >> 31;
    c = float(b);
  }

  vec3 col = vec3(0.34, 1.00, 0.11) * c;

  vec3 l = vec3(1.0, 1.0, 4.0);
  float diffuse = max(dot(normal, normalize(l - pos)), 0.2);
  diffuse = diffuse * 0.7 + pow(diffuse, 30.0) * 0.3;
  col *= diffuse;

  outColor = vec4(col, 1.0);
}

//----------------------------
// Reference
//----------------------------