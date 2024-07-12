#version 330 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D screenTexture;
uniform int mode;
uniform ivec2 screenSize;

vec3 convolution(float[9] kernel)
{
    vec2 offsets[9] = vec2[](
        vec2(-1.0/screenSize.x, 1.0/screenSize.y),  // top-left
        vec2(0.0,               1.0/screenSize.y),  // top-center
        vec2(1.0/screenSize.x,  1.0/screenSize.y),  // top-right
        vec2(-1.0/screenSize.x, 0.0),               // center-left
        vec2(0.0,          0.0),             // center-center
        vec2(1.0/screenSize.x,  0.0),               // center-right
        vec2(-1.0/screenSize.x, -1.0/screenSize.y), // bottom-left
        vec2(0.0,               -1.0/screenSize.y), // bottom-center
        vec2(1.0/screenSize.x,  -1.0/screenSize.y)  // bottom-right    
    );
    vec3 sampleTex[9];
    for(int i = 0; i < 9; i++)
    {
        sampleTex[i] = vec3(texture(screenTexture, TexCoords + offsets[i]));
    }
    vec3 col = vec3(0.0);
    for(int i = 0; i < 9; i++) col += sampleTex[i] * kernel[i];
    return col;
}

vec3 Quantization(float n, vec3 col)
{
    return floor(col * (n-1.0) + 0.5)/(n-1.0);
}

float Greyscale(vec3 col)
{
    return 0.2126 * col.r + 0.7152 * col.g + 0.0722 * col.b;
}

void main()
{
    const mat4 bayer4 = mat4(
        0, 8, 2, 10,
        12, 4, 14, 6,
        3, 11, 1, 9,
        15, 7, 13, 5
    );

    if (mode == 0)
    {
        vec3 col = texture(screenTexture, TexCoords).rgb;
        FragColor = vec4(col, 1.0);
    } 
    else if (mode == 1)
    {
        vec3 col = texture(screenTexture, TexCoords).rgb;
        FragColor = vec4(1 - col, 1.0);
    } 
    else if (mode == 2)
    {
        vec3 col = texture(screenTexture, TexCoords).rgb;
        float average = 0.2126 * col.r + 0.7152 * col.g + 0.0722 * col.b;
        FragColor = vec4(average, average, average, 1.0);
    }
    else if (mode == 3)
    {
        const float kernel[9] = float[](
            -1, -1, -1,
            -1,  9, -1,
            -1, -1, -1);

        FragColor = vec4(convolution(kernel), 1.0);
    }
    else if (mode == 4)
    {
        const float kernel[9] = float[](
            1.0 / 16, 2.0 / 16, 1.0 / 16,
            2.0 / 16, 4.0 / 16, 2.0 / 16,
            1.0 / 16, 2.0 / 16, 1.0 / 16  
        );

        FragColor = vec4(convolution(kernel), 1.0);
    }
    else if (mode == 5)
    {
        const float kernel[9] = float[](
            1,  1, 1,
            1, -8, 1,
            1,  1, 1  
        );

        FragColor = vec4(convolution(kernel), 1.0);
    }
    else if (mode == 6)
    {
        const float kernel[9] = float[](
            -2, -1, 0,
            -1,  1, 1,
             0,  1, 2
        );
        FragColor = vec4(convolution(kernel), 1.0);
    }
    else if (mode == 7)
    {
        float amount = 10.0;
        float r = texture(screenTexture, TexCoords-vec2(amount/screenSize.x, 0.0)).r;
        float g = texture(screenTexture, TexCoords).g;
        float b = texture(screenTexture, TexCoords+vec2(amount/screenSize.x, 0.0)).b;
        FragColor = vec4(r, g, b, 1.0);
    }
    else if (mode == 8)
    {
        vec3 col = texture(screenTexture, TexCoords).rgb;
        FragColor = vec4(Quantization(3, col), 1.0);
    }
    else if (mode == 9)
    {
        vec3 col = texture(screenTexture, TexCoords).rgb;
        ivec2 pixelpos = ivec2(TexCoords*screenSize)%4;
        float m = (bayer4[pixelpos.x][pixelpos.y]/16.0)-0.5;
        FragColor = vec4(Quantization(2, col + m), 1.0);
    }
    else if (mode == 10)
    {
        vec3 col = texture(screenTexture, TexCoords).rgb;
        ivec2 pixelpos = ivec2(TexCoords*screenSize)%4;
        float m = (bayer4[pixelpos.x][pixelpos.y]/16.0)-0.5;
        FragColor = vec4(Quantization(2, (vec3(Greyscale(col) + m))), 1.0);
    }
}