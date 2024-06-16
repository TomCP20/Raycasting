#version 430 core

layout (location = 0) in float vertexY;

uniform int width;

out vec2 texCoord;
out float isDark;
out float texNum;

layout(rgba32f, binding = 0) uniform image1D imgInput;

void main()
{
    vec4 pixel = imageLoad(imgInput, gl_InstanceID);
    texCoord = vec2(pixel.g, (vertexY + 1.0f)/2.0f);
    isDark = pixel.a;
    texNum = pixel.b * 7;

    gl_Position = vec4((float(gl_InstanceID)/float(width))*2.0f-1.0f, vertexY/(pixel.r*24), 0.0, 1.0);
}