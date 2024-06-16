#version 330 core

out vec4 finalColor;

in vec2 texCoord;
in float isDark;
in float texNum;

uniform sampler2DArray texture0;

void main()
{
    finalColor = texture(texture0, vec3(texCoord.x , texCoord.y, texNum));
    if (isDark != 0)
    {
        finalColor = vec4(finalColor.rgb/2, 1);
    }
}