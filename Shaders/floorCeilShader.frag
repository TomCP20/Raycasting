#version 330 core

in float x;
in vec2 floor0;
in vec2 floorStep;
in float texNum;

uniform sampler2D texture0;

out vec4 finalColor;

void main()
{
    vec2 floor = floor0 + x * floorStep;

    // the cell coord is simply got from the integer parts of floorX and floorY
    ivec2 cell = ivec2(floor);

    // get the texture coordinate from the fractional part
    vec2 texCoord = floor - cell;

    finalColor = texture(texture0, vec2((texCoord.x + texNum)/11, texCoord.y));
    finalColor = vec4(finalColor.rgb/2, 1);
}