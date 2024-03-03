#version 300 es
precision highp float;

uniform sampler2D source;
uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;
uniform int frame;

in vec2 vUv;
out vec4 outColor;

//  const float BAYER[16] = float[](
//    0.0,  8.0,  2.0, 10.0,
//   12.0,  4.0, 14.0,  6.0,
//    3.0, 11.0,  1.0,  9.0,
//   15.0,  7.0, 13.0,  5.0
//  );

 const float BAYER[64] = float[](
   0.0, 32.0,  8.0, 40.0,  2.0, 34.0, 10.0, 42.0,
  48.0, 16.0, 56.0, 24.0, 50.0, 18.0, 58.0, 26.0,
  12.0, 44.0,  4.0, 36.0, 14.0, 46.0,  6.0, 38.0,
  60.0, 28.0, 52.0, 20.0, 62.0, 30.0, 54.0, 22.0,
   3.0, 35.0, 11.0, 43.0,  1.0, 33.0,  9.0, 41.0,
  51.0, 19.0, 59.0, 27.0, 49.0, 17.0, 57.0, 25.0,
  15.0, 47.0,  7.0, 39.0, 13.0, 45.0,  5.0, 37.0,
  63.0, 31.0, 55.0, 23.0, 61.0, 29.0, 53.0, 21.0
 );

void main() {
  vec2 uv = vUv, asp = resolution / min(resolution.x, resolution.y);

  vec3 col = texture(source, uv).rgb;
  float luma = dot(col, vec3(0.299, 0.587, 0.114));

  // ivec2 bayerCoord = ivec2(mod(gl_FragCoord.xy, 4.0));
  // float threshold = BAYER[bayerCoord.y * 4 + bayerCoord.x] / 16.0;

  ivec2 bayerCoord = ivec2(mod(gl_FragCoord.xy, 8.0));
  float threshold = BAYER[bayerCoord.y * 8 + bayerCoord.x] / 64.0;

  float g = step(threshold, luma);
  outColor = vec4(vec3(g), 1.0);
}

//----------------------------
// Reference
//----------------------------
// https://scrapbox.io/0b5vr/Bayer_matrix
// https://github.com/hughsk/glsl-dither/tree/master
// https://ja.wikipedia.org/wiki/%E9%85%8D%E5%88%97%E3%83%87%E3%82%A3%E3%82%B6%E3%83%AA%E3%83%B3%E3%82%B0