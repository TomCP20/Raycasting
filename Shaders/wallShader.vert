#version 430 core

layout(location = 0) in float vertexY;
layout(rgba32f, binding = 1) uniform image1D imgInput;

uniform int width;

out vec2 texCoord;
out float isDark;
out float texNum;


void main()
{
    vec4 pixel = imageLoad(imgInput, gl_InstanceID);
    texCoord = vec2(pixel.g, (vertexY + 1.0) / 2.0);
    isDark = pixel.a;
    texNum = pixel.b * 7;

    gl_Position = vec4((float(gl_InstanceID) / float(width)) * 2.0 - 1.0, vertexY / (pixel.r * 24), 0.0, 1.0);
}