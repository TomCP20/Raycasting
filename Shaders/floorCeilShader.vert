#version 330 core

layout (location = 0) in float vertexX;
layout (location = 1) in vec2 aFloor0;
layout (location = 2) in vec2 aFloorStep;

uniform int width;
uniform int height;
uniform int ceilTexNum;
uniform int floorTexNum;

out float x;
out vec2 floor;
out float texNum;

void main()
{
    x = ((vertexX + 1)/2)*width;
    float y = ((float(gl_InstanceID) / float(height)) * 2.0 - 1.0);
    floor = aFloor0 + x * aFloorStep;
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