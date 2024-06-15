#version 330 core

out vec4 finalColor;

in vec2 texCoord;
in float isDark;
in float texNum;

uniform sampler2D texture0;

void main()
{
    finalColor = texture(texture0, vec2((texCoord.x + texNum)/11, texCoord.y));
    if (isDark != 0)
    {
        finalColor = vec4(finalColor.rgb/2, 1);
    }
}