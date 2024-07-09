using OpenTK.Graphics.OpenGL4;

namespace Raycasting;

public class LineMesh : Mesh
{
    public LineMesh() : base(2)
    {
        float[] vertices = [ 1f, -1f];
        
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);   

        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 1, VertexAttribPointerType.Float, false, sizeof(float), 0);
    }
}