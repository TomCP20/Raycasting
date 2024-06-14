#version 330 core

layout (location = 0) in float vertexX;

uniform float y;
uniform int width;

out float x;

void main()
{
    x = ((vertexX + 1)/2)*width;
    gl_Position = vec4(vertexX, y, 0.0, 1.0);
}