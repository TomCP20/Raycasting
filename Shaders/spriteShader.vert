#version 330 core

layout (location = 0) in float vertexY;
layout (location = 1) in float texX;


uniform float spriteheight;
uniform int xoffset;
uniform int screenwidth;

out vec2 texCoord;

void main()
{
    texCoord = vec2(texX, (vertexY + 1)/2);
    gl_Position = vec4(((float(xoffset + gl_InstanceID) / float(screenwidth)) * 2.0 - 1.0), vertexY*spriteheight, 0.0, 1.0);
}