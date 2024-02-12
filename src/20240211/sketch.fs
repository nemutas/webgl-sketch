#version 300 es
precision highp float;

uniform sampler2D backBuffer;
uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;

in vec2 vUv;
out vec4 outColor;

#define loop(n) for(int i; i < n; i++)

float lt, bt, tt;

vec3 cyc(vec3 p) {
  vec4 n;
  loop(8) {
    p += sin(p.yzx);
    n = 2.0 * n + vec4(cross(cos(p), sin(p.zxy)), 1.0);
    p *= 2.0;
  }
  return n.xyz / n.w;
}

mat2 rot(float a) {
  float s = sin(a), c = cos(a);
  return mat2(c, s, -s, c);
}

float sdf(vec3 p) {
  p.y += 13.0;
  p.yz *= rot(-time * 0.01);
  p += cyc(p * 2.0 + lt) * 0.25;
  float final = length(p) - 13.0;
  return final * 0.7;
}

void main() {
  vec2 uv = vUv, asp = resolution / min(resolution.x, resolution.y), suv = (uv * 2.0 - 1.0) * asp;
  vec3 rd = normalize(vec3(suv, -5.0)), ro = vec3(0.0, 0.0, 7.0);

  lt = time * 30.0 / 60.0;
  bt = floor(lt);
  tt = tanh(fract(lt) * 5.0);
  lt = bt + tt;

  float t, acc;
  loop(80) {
    vec3 p = rd * t + ro;
    float d = sdf(p);
    if (d < 1e-4 || 1e4 < t) break;
    t += d;
    acc += exp(abs(d) * -3.0);
  }

  float c = acc / 80.0 * 1.5;

  vec4 b = texture(backBuffer, vUv);
  vec3 col = mix(vec3(c), b.rgb, 0.7);

  outColor = vec4(col, 1.0);
}

//----------------------------
// Reference
//----------------------------