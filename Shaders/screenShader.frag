#version 330 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D screenTexture;
uniform int mode;

void main()
{
    vec3 col = texture(screenTexture, TexCoords).rgb;
    if (mode == 0)
    {
        FragColor = vec4(col, 1.0);
    }
    else if (mode == 1)
    {
        FragColor = vec4(1 - col, 1.0);
    }
    else if (mode == 2)
    {
        float average = 0.2126 * col.r + 0.7152 * col.g + 0.0722 * col.b;
        FragColor = vec4(average, average, average, 1.0);
    }
    
} 