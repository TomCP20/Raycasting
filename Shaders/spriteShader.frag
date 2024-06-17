#version 330 core

in vec2 texCoord;
in float z;

uniform sampler2DArray texture0;
uniform int texNum;
uniform float transformY;

out vec4 finalColor;

void main()
{
    if(transformY < z)
    {
        vec4 texColor = texture(texture0, vec3(texCoord.x, texCoord.y, texNum));
        if(texColor == vec4(0, 0, 0, 1))
        {
            finalColor = vec4(0.0);
        }
        else
        {
            finalColor = texColor;
        }
    }
    else
    {
        finalColor = vec4(0);
    }
}