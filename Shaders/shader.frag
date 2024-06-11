#version 330 core

uniform vec3 objectColor;

out vec4 finalColor;

void main()
{
    finalColor = vec4(objectColor, 1.0);
}