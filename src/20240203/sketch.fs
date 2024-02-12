// https://twitter.com/cmzw_/status/1616708959506948096

#version 300 es
precision highp float;

uniform sampler2D backBuffer;
uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;
uniform sampler2D textureUnit;

in vec2 vUv;
out vec4 outColor;

vec2 coveredUv() {
  ivec2 imgSize = textureSize(textureUnit, 0);
  float imgAsp = float(imgSize.x) / float(imgSize.y);
  float screenAsp = resolution.x / resolution.y;
  vec2 covered = imgAsp < screenAsp ? vec2(1.0, imgAsp / screenAsp) : vec2(screenAsp / imgAsp, 1.0);
  return (vUv - 0.5) * covered + 0.5;
}

void main() {
  vec2 asp = vec2(resolution.x / resolution.y, 1.0);
  vec2 suv = vUv * 2.0 - 1.0;
  suv *= asp * 1.5;
  vec3 hemiSphereNormal = normalize(vec3(suv, (sqrt(1.0 - dot(suv, suv)) + 0.5) / 0.5));
  vec2 distortion = hemiSphereNormal.xy * (1.0 - hemiSphereNormal.z);
  
  vec2 uv = coveredUv();
  uv.x += time * 0.1;
  vec4 col = mix(texture(textureUnit, uv), texture(textureUnit, uv + distortion), smoothstep(1.0, 0.99, length(suv)));

  outColor = col;
}