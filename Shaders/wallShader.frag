#version 330 core

out vec4 finalColor;

in vec2 texCoord;
in float isDark;
in float texNum;

uniform sampler2D texture0;

void main()
{
    vec2 texCoord = vec2(((texCoord.x*63+0.5)/64 + texNum)/11, (texCoord.y*63+0.5)/64);

    finalColor = texture(texture0, texCoord);
    if (isDark != 0)
    {
        finalColor = vec4(finalColor.rgb/2, 1);
    }
}