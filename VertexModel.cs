using OpenTK.Graphics.OpenGL4;

namespace Raycasting;

public class VertexModel
{
    public readonly int vertexCount;

    private readonly int vertexBufferObject;

    private readonly int vertexArrayObject;

    public VertexModel(float[] vertices, int[] AttribSizes)
    {
        int stride = AttribSizes.Sum();
        vertexCount = vertices.Length/stride;
        vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(vertexArrayObject);

        vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        int offset = 0;
        for (int i = 0; i < AttribSizes.Length; i++)
        {
            GL.EnableVertexAttribArray(i);
            GL.VertexAttribPointer(0, AttribSizes[i], VertexAttribPointerType.Float, false, stride*sizeof(float), offset*sizeof(float));
            offset += AttribSizes[i];
        }
        

    }

    public void delete()
    {
        GL.DeleteBuffer(vertexBufferObject);
        GL.DeleteVertexArray(vertexArrayObject);
    }
}