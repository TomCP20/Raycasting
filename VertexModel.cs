using OpenTK.Graphics.OpenGL4;

namespace Raycasting;

public class VertexModel
{
    public readonly int vertexCount;

    private readonly int vertexBufferObject;

    private readonly int vertexArrayObject;

    public VertexModel(float[] vertices, int[] strides)
    {
        vertexCount = vertices.Length/strides.Sum();
        vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(vertexArrayObject);

        vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        int offset = 0;
        for (int i = 0; i < strides.Length; i++)
        {
            GL.EnableVertexAttribArray(i);
            GL.VertexAttribPointer(0, strides[i], VertexAttribPointerType.Float, false, sizeof(float), offset*sizeof(float));
            offset += strides[i];
        }
        

    }

    public void delete()
    {
        GL.DeleteBuffer(vertexBufferObject);
        GL.DeleteVertexArray(vertexArrayObject);
    }
}