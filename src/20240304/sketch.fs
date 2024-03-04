#version 300 es
precision highp float;

uniform sampler2D backBuffer;
uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;
uniform int frame;

in vec2 vUv;
out vec4 outColor;

#define loop(n) for(int i;i<n;i++)
#define grad(e) sdf(p + e) - sdf(p - e)
#define sat(v) clamp(v, 0.0, 1.0)

const float PI = acos(-1.0);
float lt, bt, tt;

mat2 rot(float a) {
  float s = sin(a), c = cos(a);
  return mat2(c, s, -s, c);
}

float box(vec3 p, vec3 b) {
  p = abs(p) - b;
  return length(max(p, 0.0)) + min(max(p.x, max(p.y, p.z)), 0.0);
}

float sdf(vec3 p) {
  vec3 b = vec3(1.3, 0.3, 0.3);
  if (mod(bt, 2.0) == 0.0) {
    p.x -= b.x;
    p.yz *= rot(p.x / b.x * PI * 0.5 * tt - lt * 0.1);
    p.x += b.x;
  } else {
    p.x += b.x;
    p.yz *= rot(p.x / b.x * PI * 0.5 * (1.0 - tt) - lt * 0.1);
    p.x -= b.x;
  }
  return box(p, vec3(1.5, 0.3, 0.3)) - 0.08;
}

void main() {
  vec2 uv = vUv, asp = resolution / min(resolution.x, resolution.y), suv = (uv * 2.0 - 1.0) * asp;
  vec3 rd = normalize(vec3(suv, -2.0)), ro = vec3(0.0, 0.0, 2.5);
  lt = time * 80.0 / 60.0;
  bt = floor(lt);
  tt = tanh(fract(lt) * 5.0);
  lt = bt + tt;

  float t;
  loop(64) {
    vec3 p = rd * t + ro;
    float d = sdf(p) * 0.8;
    if (d < 1e-4 || 1e4 < t) break;
    t += d;
  }

  vec3 col = vec3(0.20, 0.22, 0.17);
  if (t < 1e4) {
    vec3 p = rd * t + ro;
    vec2 e = vec2(1e-4, 0.0);
    vec3 n = normalize(vec3(grad(e.xyy), grad(e.yxy), grad(e.yyx)));
    vec3 l = vec3(0.0, 0.0, 3.0);
    vec3 LP = normalize(p - l);
    float shade = sat(dot(-LP, n));
    vec2 fuv = fract(uv * 100.0 * asp);

    if (shade < 0.4) {
      shade = smoothstep(0.38, 0.48, distance(fuv, vec2(0.5)));
    } else if (shade < 0.6) {
      shade = 1.0 - sat(step(abs(fuv.x), 0.2) + step(abs(fuv.y), 0.2));
    } else if (shade < 0.8) {
      fuv *= rot(PI * 0.25);
      shade = step(0.08, abs(fuv.y));
    } else {
      shade = 1.0;
    }
    vec3 shadeCol = vec3(0.49, 0.52, 0.41);
    col = mix(shadeCol, vec3(0.94, 1.00, 0.79), shade);

    // float f = pow(1.0 - dot(-rd, n), 1.0);
    // f = smoothstep(0.6, 0.601, f);
    // col = mix(col, shadeCol, f);
  }

  outColor = vec4(col, 1.0);
}

//----------------------------
// Reference
//----------------------------
// https://unshift.co.jp/labs/transformRoundedCube/