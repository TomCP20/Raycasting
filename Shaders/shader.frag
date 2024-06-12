#version 330 core

out vec4 finalColor;

in float texY;

uniform sampler2D texture0;
uniform bool isDark;
uniform float texX;
uniform int texNum;

void main()
{
    vec2 texCoord = vec2((texX + texNum)/11, texY);

    finalColor = texture(texture0, texCoord);
    if (isDark)
    {
        finalColor /= 2;
    }
}