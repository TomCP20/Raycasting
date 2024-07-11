using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Raycasting;

public class ComputeShader : BaseShader
{

    public ComputeShader(string path)
    {
        string shaderSource = File.ReadAllText(path);

        // compute shader
        int compute = GL.CreateShader(ShaderType.ComputeShader);
        GL.ShaderSource(compute, shaderSource);
        CompileShader(compute);

        // shader Program
        GL.AttachShader(Handle, compute);
        LinkProgram(Handle);

        GL.DetachShader(Handle, compute);
        GL.DeleteShader(compute);

        // The shader is now ready to go, but first, we're going to cache all the shader uniform locations.
        // Querying this from the shader is very slow, so we do it once on initialization and reuse those values
        // later.

        // First, we have to get the number of active uniforms in the shader.
        GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

        // Loop over all the uniforms,
        for (var i = 0; i < numberOfUniforms; i++)
        {
            // get the name of this uniform,
            var key = GL.GetActiveUniform(Handle, i, out _, out _);

            // get the location,
            var location = GL.GetUniformLocation(Handle, key);

            // and then add it to the dictionary.
            _uniformLocations.Add(key, location);
        }
    }
}