#version 330 core

in float x;

uniform sampler2D texture0;
uniform vec2 floor0;
uniform vec2 floorStep;
uniform int texNum;

out vec4 finalColor;

void main()
{
    vec2 floor = floor0 + x * floorStep;

    // the cell coord is simply got from the integer parts of floorX and floorY
    ivec2 cell = ivec2(floor);

    // get the texture coordinate from the fractional part
    vec2 t = floor - cell;

    finalColor = texture(texture0, vec2(((t.x*63+0.5)/64 + texNum)/11, t.y))/2;
}