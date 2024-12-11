#version 330 core

out vec4 outColor;

uniform vec3 inputColor;

void main() {
  outColor = vec4(inputColor, 1.0);
}
