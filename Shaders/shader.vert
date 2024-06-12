#version 330 core

layout (location = 0) in float vertexY;
layout (location = 1) in float texY;

uniform float height;
uniform float x;
uniform float texX;
uniform int texNum;

out vec2 texCoord;

void main()
{
    texCoord = vec2((texX + texNum)/11, texY);

    gl_Position = vec4(x, vertexY*height, 0.0, 1.0);
}