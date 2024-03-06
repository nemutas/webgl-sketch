#version 300 es
precision highp float;

uniform sampler2D backBuffer;
uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;
uniform int frame;

in vec2 vUv;
out vec4 outColor;

void main() {
  vec2 uv = vUv, asp = resolution / min(resolution.x, resolution.y);
  vec2 p = uv * asp * 6.0;

  for (float i = 0.0; i < 8.0; i++) {
    p.x += sin(p.y + i + time * 0.3);
    p *= mat2(6, -8, 8, 6) / 8.0;
  }

  vec3 col = sin(p.xyx * 0.3 + vec3(0, 1, 2)) * 0.5 + 0.5;
  outColor = vec4(col, 1.0);
}

//----------------------------
// Reference
//----------------------------
// https://twitter.com/XorDev/status/1765032092721648044
// https://www.shadertoy.com/view/lXXXzS

//----------------------------
// Commnets by @XorDev
//----------------------------
/**
"Cheap Turbulence" by @XorDev

Simulating proper fluid dynamics can be complicated and requires a back buffer or multi-pass setup.

Sometimes, you just want to emulate some smoke or something simple, and you don't want to go through all that trouble.

This method is very simple! Start with pixel coordinates and scale them down as desired, then with a for loop,
you should do a sine wave offset. In my case I'm doing "p.x+=sin(p.y)".
To animate it, you can add a time offset to the sine wave, and it also helps to shift each iteration with the
iterator "i" to break up visible patterns.

Next, you want to rotate the coordinates and scale down. It could be as simple as p=p.yx/0.8 or a rotation matrix like mat2(.8,-.6,.6,.8).

Now the resulting p coordinates will appear turbulent, and you can use these coordinates in color function.
My color equation looks like this:

fragColor=sin(p.xyxy*.3+vec4(0,1,2,3))*.5+.5

Smooth, continious equations look best
*/
