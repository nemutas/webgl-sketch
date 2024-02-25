#version 300 es
precision highp float;

uniform sampler2D backBuffer;
uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;
uniform int frame;
uniform sampler2D textureUnit;

in vec2 vUv;
out vec4 outColor;

#define loop(n) for(int i; i < n; i++)
#define grad(p, h) sdf(p + h) - sdf(p - h)

const float PI = acos(-1.0);
float lt, bt, tt;

mat2 rot(float a) {
  float s = sin(a), c = cos(a);
  return mat2(c, s, -s, c);
}

float box(vec3 p, vec3 b) {
  p = abs(p) - b;
  return max(p.x, max(p.y, p.z));
}

float gyroid(vec3 p, float s, float t, float b) {
  p *= s;
  p.xy *= rot(lt * 0.2);
  p.yz *= rot(lt * 0.2);
  p.zx *= rot(lt * 0.2);
  return abs(dot(sin(p), cos(p.zxy)) - b) / s - t;
}

vec3 transform(vec3 p) {
  p.xy *= rot(time * 0.1);
  p.yz *= rot(time * 0.1);
  p.zx *= rot(time * 0.1);
  return p;
}

vec3 hash(vec3 v) {
  uvec3 x = floatBitsToUint(v + vec3(0.1, 0.2, 0.3));
  x = (x >> 8 ^ x.yzx) * 0x456789ABu;
  x = (x >> 8 ^ x.yzx) * 0x6789AB45u;
  x = (x >> 8 ^ x.yzx) * 0x89AB4567u;
  return vec3(x) / vec3(-1u);
}

mat3 orthbas(vec3 z) {
  z = normalize(z);
  vec3 up = abs(z.y) < 0.999 ? vec3(0, 1, 0) : vec3(0, 0, 1);
  vec3 x = normalize(cross(up, z));
  return mat3(x, cross(z, x), z);
}

vec3 cyc(vec3 p) {
  mat3 b = orthbas(vec3(-3.0, 2.0, -1.0));
  vec4 n;
  loop(8) {
    p *= b;
    p += sin(p.yzx);
    n = 2.0 * n + vec4(cross(cos(p), sin(p.zxy)), 1.0);
    p *= 2.0;
  }
  return n.xyz / n.w;
}

float sdf(vec3 p) {
  float d = 1e9, g;
  d = min(d, box(p, vec3(0.7)));
  p += cyc(p * 0.2) * 0.5;
  g = gyroid(p, 8.0, 0.03, 1.3);
  d = max(d, g * 0.5);
  return d;
}

void main() {
  vec2 uv = vUv, asp = resolution / min(resolution.x, resolution.y), suv = (uv * 2.0 - 1.0) * asp;
  vec3 rd = normalize(vec3(suv, -5.0)), ro = vec3(0.0, 0.0, 6.0);

  lt = time * 100.0 / 60.0;
  bt = floor(lt);
  tt = tanh(fract(lt) * 5.0);
  lt = bt + tt;

  float t;
  loop(64) {
    vec3 p = rd * t + ro;
    p = transform(p);
    float d = sdf(p);
    if (d < 1e-4 || 1e5 < t) break;
    t += d;
  }

  vec3 col;
  if (t < 1e5) {
    vec3 p = rd * t + ro;
    p = transform(p);

    vec2 h = vec2(1e-4, 0.0);
    vec3 n = normalize(vec3(grad(p, h.xyy), grad(p, h.yxy), grad(p, h.yyx)));

    vec3 l = normalize(transform(vec3(1, 1, 2)));
    float diff = clamp(dot(n, l), 0.2, 1.0);
    col = vec3(diff);

    vec3 uv3 = p * 0.5 + 0.5;
    vec3 colXZ = texture(textureUnit, uv3.xz).rgb;
    vec3 colYZ = texture(textureUnit, uv3.yz).rgb;
    vec3 colXY = texture(textureUnit, uv3.xy).rgb;

    n = abs(n);
    col *= colYZ * n.x + colXZ * n.y + colXY * n.z;
  }

  outColor = vec4(col, 1.0);
}

//----------------------------
// Reference
//----------------------------
// https://youtu.be/VaYyPTw0V84?si=eVov17jGvBxXqfgg
// https://youtu.be/-adHIyjIYgk?si=nkXdxIpaVjTkXhPt