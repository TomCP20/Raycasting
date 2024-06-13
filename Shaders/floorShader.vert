#version 330 core

layout (location = 0) in float vertexX;

uniform float y;

void main()
{
    gl_Position = vec4(vertexX, y, 0.0, 1.0);
}