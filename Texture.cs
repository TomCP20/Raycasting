using System;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace Raycasting;


public class Texture
{
    public readonly int Handle;


    public static Texture LoadFromFiles(string[] paths)
    {
        // Generate handle
        int handle = GL.GenTexture();

        // Bind the handle
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2DArray, handle);

        // For this example, we're going to use .NET's built-in System.Drawing library to load textures.

        // OpenGL has it's texture origin in the lower left corner instead of the top left corner,
        // so we tell StbImageSharp to flip the image when loading.
        StbImage.stbi_set_flip_vertically_on_load(1);


        GL.TexImage3D(TextureTarget.Texture2DArray, 0, PixelInternalFormat.Rgba, 64, 64, paths.Length, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
        for (int i = 0; i < paths.Length; i++)
        {
            // Here we open a stream to the file and pass it to StbImageSharp to load.
            using (Stream stream = File.OpenRead(paths[i]))
            {
                ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                GL.TexSubImage3D(TextureTarget.Texture2DArray, 0, 0, 0, i, 64, 64, 1, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            }
        }


        GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

        GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

        return new Texture(handle);
    }

    public Texture(int glHandle)
    {
        Handle = glHandle;
    }

    // Activate texture
    // Multiple textures can be bound, if your shader needs more than just one.
    // If you want to do that, use GL.ActiveTexture to set which slot GL.BindTexture binds to.
    // The OpenGL standard requires that there be at least 16, but there can be more depending on your graphics card.
    public void Use(TextureUnit unit)
    {
        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.Texture2DArray, Handle);
    }
}
