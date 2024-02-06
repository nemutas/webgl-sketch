#version 300 es
precision highp float;

uniform sampler2D backBuffer;
uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;

in vec2 vUv;
out vec4 outColor;

#define loop(n) for(int i; i < n; i++)
#define sat(v) clamp(v, 0.0, 1.0)

const float PI = acos(-1.0);

mat2 rot(float a) {
  float s = sin(a), c = cos(a);
  return mat2(c, s, -s, c);
}

float sdf(vec3 p) {
  float final = 1e9;

  vec3 bp = p;

  loop(5) {
    bp = abs(bp) - 0.5;
    bp.xy *= rot(1.0 * PI * 0.25);
    bp.xz *= rot(1.0 * PI * 0.25 - time * 0.5);
  }

  bp = abs(bp) - vec3(1.0, 0.3, 0.01);
  float b = max(bp.x, max(bp.y, bp.z));
  float c = p.z - 0.5;
  return max(b, c);
}

float raymarch(vec3 rd, vec3 ro) {
  float t;
  loop(128) {
    vec3 p = rd * t + ro;
    float d = sdf(p);
    if (d < 1e-5 || 1e9 < t) break;
    t += d;
  }
  return t;
}

vec3 calcNormal(vec3 p) {
  float h = 1e-4;
  vec2 k = vec2(1, -1);
  return normalize(vec3(
    k.xyy * sdf(p + k.xyy * h) +
    k.yxy * sdf(p + k.yxy * h) +
    k.yyx * sdf(p + k.yyx * h) +
    k.xxx * sdf(p + k.xxx * h)
  ));
}

void main() {
  vec2 uv = vUv, asp = resolution / min(resolution.x, resolution.y), suv = (uv - 0.5) * asp * 2.0;
  vec3 rd = normalize(vec3(suv, -5.0)), ro = vec3(0.0, 0.0, 15.0);

  float t = raymarch(rd, ro);

  vec3 col;
  if (t < 1e9) {
    vec3 p = rd * t + ro;
    vec3 normal = calcNormal(p);

    vec3 l = vec3(5.0, 5.0, 10.0);
    float d = clamp(dot(normal, normalize(l - p)), 0.2, 1.0);
    col = vec3(d) * 0.5;

    vec3 reflection = reflect(rd, normal);
    ro = p + normal * 1e-4 * 2.0;
    t = raymarch(reflection, ro);
    if (t < 1e9) {
      vec3 rp = reflection * t + ro;
      normal = calcNormal(rp);
      d = clamp(dot(normal, normalize(l - rp)), 0.2, 1.0);
      col += vec3(d) * 0.5;
    }
  }

  outColor = vec4(col, 1.0);
}

//----------------------------
// Reference
//----------------------------
// Iterated Function System（IFS）
// https://qiita.com/kaneta1992/items/21149c78159bd27e0860