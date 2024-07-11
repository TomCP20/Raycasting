#version 430 core

layout (location = 0) in float vertexX;
layout(rgba32f, binding = 0) uniform image1D imgInput;

uniform ivec2 screenSize;
uniform int ceilTexNum;
uniform int floorTexNum;

out vec2 floor;
out float texNum;

void main()
{
    vec4 pixel = imageLoad(imgInput, gl_InstanceID);
    float x = ((vertexX + 1)/2)*screenSize.x;
    floor = pixel.rg + x * pixel.ba;
    float screeny = ((float(gl_InstanceID) / float(screenSize.y)) * 2.0 - 1.0);
    if (screeny < 0)
    {
        texNum = floorTexNum;
    }
    else
    {
        texNum = ceilTexNum;
    }

    gl_Position = vec4(vertexX, screeny, 0.0, 1.0);
}