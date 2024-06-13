#version 330 core

layout (location = 0) in float vertexY;
layout (location = 1) in float aTexY;

uniform float height;
uniform float x;

out float texY;

void main()
{
    texY = aTexY;

    gl_Position = vec4(x, vertexY*height, 0.0, 1.0);
}