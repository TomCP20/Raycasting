#version 330 core

layout (location = 0) in float vertexX;
layout (location = 1) in vec2 aFloor0;
layout (location = 2) in vec2 aFloorStep;
layout (location = 3) in float y;
layout (location = 4) in float aTexNum;

uniform int width;

out float x;
out vec2 floor0;
out vec2 floorStep;
out float texNum;

void main()
{
    x = ((vertexX + 1)/2)*width;
    floor0 = aFloor0;
    floorStep = aFloorStep;
    texNum = aTexNum;

    gl_Position = vec4(vertexX, y, 0.0, 1.0);
}