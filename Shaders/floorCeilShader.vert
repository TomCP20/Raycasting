#version 430 core

layout (location = 0) in float vertexX;
layout(rgba32f, binding = 0) uniform image1D imgInput;

uniform int width;
uniform int height;
uniform int ceilTexNum;
uniform int floorTexNum;

out vec2 floor;
out float texNum;

void main()
{
    vec4 pixel = imageLoad(imgInput, gl_InstanceID);
    float x = ((vertexX + 1)/2)*width;
    float y = ((float(gl_InstanceID) / float(height)) * 2.0 - 1.0);
    floor = pixel.rg + x * pixel.ba;
    if (y < 0)
    {
        texNum = floorTexNum;
    }
    else
    {
        texNum = ceilTexNum;
    }

    gl_Position = vec4(vertexX, y, 0.0, 1.0);
}