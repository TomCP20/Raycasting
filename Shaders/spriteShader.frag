#version 330 core

in vec2 texCoord;

uniform sampler2D texture0;
uniform int texNum;

out vec4 finalColor;

void main()
{

    vec4 texColor = texture(texture0, vec2((texCoord.x + texNum)/11, texCoord.y));
    if (texColor == vec4(0, 0, 0, 1))
    {
        finalColor = vec4(0.0);
    }
    else
    {
        finalColor = texColor;
    }
}