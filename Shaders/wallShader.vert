#version 330 core

layout (location = 0) in float vertexY;
layout (location = 1) in float aTexY;
layout (location = 2) in float height;
layout (location = 3) in float x;
layout (location = 4) in float aIsDark;
layout (location = 5) in float aTexX;
layout (location = 6) in float aTexNum;

out float texY;
out float isDark;
out float texX;
out float texNum;

void main()
{
    texY = aTexY;
    isDark = aIsDark;
    texX = aTexX;
    texNum = aTexNum;

    gl_Position = vec4(x, vertexY*height, 0.0, 1.0);
}