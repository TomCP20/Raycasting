#version 330 core

layout (location = 0) in float vertexY;
layout (location = 1) in float height;
layout (location = 2) in float x;
layout (location = 3) in float aIsDark;
layout (location = 4) in float texX;
layout (location = 5) in float aTexNum;

out vec2 texCoord;
out float isDark;
out float texNum;

void main()
{
    texCoord = vec2(texX, (vertexY + 1.0f)/2.0f);
    isDark = aIsDark;
    texNum = aTexNum;

    gl_Position = vec4(x, vertexY*height, 0.0, 1.0);
}