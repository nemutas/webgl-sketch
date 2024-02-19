#version 300 es
precision highp float;

uniform sampler2D backBuffer;
uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;
uniform int frame;
uniform samplerCube cubeTextureUnit;

in vec2 vUv;
out vec4 outColor;

#define loop(n) for(int i; i < n; i++)

const float PI = acos(-1.0);
float lt, bt, tt;

mat2 rot(float a) {
  float s = sin(a), c = cos(a);
  return mat2(c, s, -s, c);
}

float sdf(vec3 p) {
  p.xy *= rot(lt * PI * 0.05);
  p.yz *= rot(lt * PI * 0.10);
  p.zx *= rot(lt * PI * 0.15);

  vec3 bp = abs(p) - 0.5 + sin(lt * PI * 0.25) * 0.25;
  float b = max(bp.x, max(bp.y, bp.z));

  return max(b, dot(normalize(vec3(1)), abs(p)) - 0.43);
}

void main() {
  vec2 uv = vUv, asp = resolution / min(resolution.x, resolution.y), suv = (uv * 2.0 - 1.0) * asp;
  vec3 rd = normalize(vec3(suv, -5.0)), ro = vec3(0.0, 0.0, 5.0);
  
  lt = time * 120.0 / 60.0;
  bt = floor(lt);
  tt = tanh(fract(lt) * 5.0);
  lt = bt + tt;

  float t;
  loop(64) {
    vec3 p = rd * t + ro;
    float d = sdf(p);
    if (d < 1e-4 || 1e4 < t) break;
    t += d;
  }

  vec3 col;
  if (t < 1e4) {
    vec3 p = rd * t + ro;
    vec2 e = vec2(1e-2, 0.0);
    vec3 n = normalize(vec3(
      sdf(p + e.xyy) - sdf(p - e.xyy),
      sdf(p + e.yxy) - sdf(p - e.yxy),
      sdf(p + e.yyx) - sdf(p - e.yyx)
    ));
    vec3 reflection = reflect(rd, n);
    col = texture(cubeTextureUnit, reflection).rgb;
  }

  outColor = vec4(col, 1.0);
}

//----------------------------
// Reference
//----------------------------
// リアルタイムグラフィックスの数学 ― GLSLではじめるシェーダプログラミング
// hash関数：https://www.shadertoy.com/view/XlXcW4