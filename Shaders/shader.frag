#version 330 core

out vec4 finalColor;

in vec2 texCoord;

uniform sampler2D texture0;

void main()
{
    finalColor = texture(texture0, texCoord);
}