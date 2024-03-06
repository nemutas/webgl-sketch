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

#define sat(v) clamp(v, 0.0, 1.0)

void main() {
  vec2 uv = vUv, asp = resolution / min(resolution.x, resolution.y), suv = (uv * 2.0 - 1.0) * asp;

  float dt = time - prevTime;
  float d, n = 15.0;
  for (float i = 0.0; i < n; i++) {
    float t = prevTime + dt * (i / n);
    vec2 coord = sin(vec2(4, 5) * t * 1.);
    d += smoothstep(0.02, 0.01, distance(suv * 1.5, coord));
  }

  vec3 b = texture(backBuffer, uv).rgb;
  b *= step(0.08, b);
  vec3 col = vec3(d, b.rg);
  col = sat(mix(b, col * 2.0, 0.1));
  
  outColor = vec4(col, 1.0);
}

//----------------------------
// Reference
//----------------------------
// https://zenn.dev/k_kuroguro/articles/b73e4e6e3d2c13