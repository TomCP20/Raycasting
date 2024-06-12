#version 330 core

out vec4 finalColor;

in vec2 texCoord;

uniform sampler2D texture0;
uniform bool isDark;

void main()
{
    finalColor = texture(texture0, texCoord);
    if (isDark)
    {
        finalColor /= 2;
    }
}