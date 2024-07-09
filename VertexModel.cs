using OpenTK.Graphics.OpenGL4;

namespace Raycasting;

public class VertexModel
{
    public readonly int vertexCount;

    private readonly int vertexBufferObject;

    private readonly int vertexArrayObject;

    public VertexModel(float[] vertices, int vertexCount)
    {
        this.vertexCount = vertexCount;
        vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(vertexArrayObject);

        vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 1, VertexAttribPointerType.Float, false, sizeof(float), 0);

    }

    public void delete()
    {
        GL.DeleteBuffer(vertexBufferObject);
        GL.DeleteVertexArray(vertexArrayObject);
    }
}