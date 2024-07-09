using OpenTK.Graphics.OpenGL4;

namespace Raycasting;

public abstract class Mesh
{
    public readonly int vertexCount;

    private readonly int vertexBufferObject;

    private readonly int vertexArrayObject;

    protected Mesh(int vertexCount)
    {
        this.vertexCount = vertexCount;
        vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(vertexArrayObject);

        vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
    }

    public void bind()
    {
        GL.BindVertexArray(vertexArrayObject);
    }

    public void delete()
    {
        GL.DeleteBuffer(vertexBufferObject);
        GL.DeleteVertexArray(vertexArrayObject);
    }
}