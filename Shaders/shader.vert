#version 330 core

layout (location = 0) in vec3 vertexPos;

uniform float height;
uniform float x;


void main()
{
    gl_Position = vec4(x, vertexPos.y*height, vertexPos.z, 1.0);
}