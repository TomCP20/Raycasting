#version 430 core

layout (location = 0) in float vertexY;
layout(rgba32f, binding = 1) uniform image1D zbuffer;
layout(rgba32f, binding = 2) uniform image1D imgInput;



uniform float spriteheight;
uniform int xoffset;
uniform int screenwidth;

out vec2 texCoord;
out float z;

void main()
{
    z = imageLoad(zbuffer, gl_InstanceID+xoffset).r;
    float texX = imageLoad(imgInput, gl_InstanceID).r;
    texCoord = vec2(texX, (vertexY + 1)/2);
    gl_Position = vec4(((float(xoffset + gl_InstanceID) / float(screenwidth)) * 2.0 - 1.0), vertexY*spriteheight, 0.0, 1.0);
}