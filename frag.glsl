#version 330 core

in vec4 inputColor;
in vec2 texCoord;
out vec4 outColor;

uniform sampler2D uTexture;

void main() {
  // outColor = vec4(0.4, 0.5, 0.9, 1.0);
  // outColor = inputColor;
  outColor = texture(uTexture, texCoord);
}
