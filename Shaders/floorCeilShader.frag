#version 330 core

in vec2 floor;
in float texNum;

uniform sampler2DArray texture0;

out vec4 finalColor;

void main()
{
    // get the texture coordinate from the fractional part of floor
    vec2 texCoord = floor - ivec2(floor);

    finalColor = texture(texture0, vec3(texCoord.x , texCoord.y, texNum));
    finalColor = vec4(finalColor.rgb/2, 1);
}