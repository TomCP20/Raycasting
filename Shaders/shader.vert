#version 330 core

layout (location = 0) in float vertexY;

uniform float height;
uniform float x;


void main()
{
    gl_Position = vec4(x, vertexY*height, 0.0, 1.0);
}