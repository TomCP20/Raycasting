#version 330 core

in float texY;

uniform sampler2D texture0;
uniform float texX;
uniform int texNum;

out vec4 finalColor;

void main()
{

    vec4 texColor = texture(texture0, vec2(((texX*63+0.5)/64 + texNum)/11, texY));
    if (texColor == vec4(0, 0, 0, 1))
    {
        finalColor = vec4(0.0);
    }
    else
    {
        finalColor = texColor;
    }

}