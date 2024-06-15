#version 330 core

layout (location = 0) in float vertexY;

uniform float x;
uniform float height;

void main()
{
    gl_Position = vec4(x, vertexY*height, 0.0, 1.0);
}