#version 330 core

in vec4 inputColor;
out vec4 outColor;

void main() {
  // outColor = vec4(0.4, 0.5, 0.9, 1.0);
  outColor = inputColor;
}
