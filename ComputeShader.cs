using OpenTK.Graphics.OpenGL4;

namespace Raycasting;

public class ComputeShader
{
    public readonly int Handle;

    public ComputeShader(string path)
    {
        string shaderSource = File.ReadAllText(path);

        // compute shader
        int compute = GL.CreateShader(ShaderType.ComputeShader);
        GL.ShaderSource(compute, shaderSource);
        CompileShader(compute);

        // shader Program
        Handle = GL.CreateProgram();
        GL.AttachShader(Handle, compute);
        LinkProgram(Handle);

        GL.DetachShader(Handle, compute);
        GL.DeleteShader(compute);
    }

    private static void CompileShader(int shader)
    {
        // Try to compile the shader
        GL.CompileShader(shader);

        // Check for compilation errors
        GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
        if (code != (int)All.True)
        {
            // We can use `GL.GetShaderInfoLog(shader)` to get information about the error.
            var infoLog = GL.GetShaderInfoLog(shader);
            throw new ShaderCompileException($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");
        }
    }

    private static void LinkProgram(int program)
    {
        // We link the program
        GL.LinkProgram(program);

        // Check for linking errors
        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
        if (code != (int)All.True)
        {
            // We can use `GL.GetProgramInfoLog(program)` to get information about the error.
            var infoLog = GL.GetProgramInfoLog(program);
            throw new ShaderLinkException($"Error occurred whilst linking Program({program}).\n\n{infoLog}");
        }
    }

    public void Use()
    {
        GL.UseProgram(Handle);
    }
}