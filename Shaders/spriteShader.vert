#version 330 core

layout (location = 0) in float vertexY;
layout (location = 1) in float x;
layout (location = 2) in float texX;


uniform float height;

out vec2 texCoord;

void main()
{
    texCoord = vec2(texX, (vertexY + 1)/2);
    gl_Position = vec4(x, vertexY*height, 0.0, 1.0);
}