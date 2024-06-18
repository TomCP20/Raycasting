using System.Diagnostics;
using System.Net;
using Microsoft.VisualBasic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Raycasting;

public class Game : GameWindow
{
    private readonly GameMap gameMap = new GameMap();

    private readonly float[] vertices =
    {
        //pos
         1f,
        -1f,
    };


    private const int vertexCount = 2;

    private int vertexBufferObject;

    private int vertexArrayObject;

    private ComputeShader? wallComputeShader;

    private ComputeShader? floorCeilComputeShader;

    private ComputeShader? spriteComputeShader;

    private Shader? wallShader;
    private Shader? floorCeilShader;

    private Shader? spriteShader;

    private readonly string[] paths =
    {
        "Textures/eagle.png",
        "Textures/redbrick.png",
        "Textures/purplestone.png",
        "Textures/greystone.png",
        "Textures/bluestone.png",
        "Textures/mossy.png",
        "Textures/colorstone.png",
        "Textures/wood.png",
        "Textures/barrel.png",
        "Textures/pillar.png",
        "Textures/greenlight.png",
    };

    private Texture? textureArray;

    private readonly int[] computeTextures = new int[3];
    public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) { }

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.Enable(EnableCap.Blend);

        vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(vertexArrayObject);

        vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 1, VertexAttribPointerType.Float, false, sizeof(float), 0);

        wallShader = new Shader("Shaders/wallShader.vert", "Shaders/wallShader.frag");
        floorCeilShader = new Shader("Shaders/floorCeilShader.vert", "Shaders/floorCeilShader.frag");
        spriteShader = new Shader("Shaders/spriteShader.vert", "Shaders/spriteShader.frag");

        wallComputeShader = new ComputeShader("Shaders/wallShader.comp");
        floorCeilComputeShader = new ComputeShader("Shaders/floorCeilShader.comp");
        spriteComputeShader = new ComputeShader("Shaders/spriteShader.comp");

        textureArray = Texture.LoadFromFiles(paths);
        textureArray.Use(TextureUnit.Texture0);

        GL.GenTextures(3, computeTextures);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        GL.Clear(ClearBufferMask.ColorBufferBit);

        Debug.Assert(textureArray != null);
        textureArray.Use(TextureUnit.Texture0);

        drawFloorCeil();
        drawWalls();
        drawSprites();

        SwapBuffers();
    }

    private void drawFloorCeil()
    {
        Debug.Assert(floorCeilShader != null);
        Debug.Assert(floorCeilComputeShader != null);

        // rayDir for leftmost ray (x = 0) and rightmost ray (x = w)
        Vector2 rayDir0 = (Vector2)(gameMap.player.dir - gameMap.player.plane);
        Vector2 rayDir1 = (Vector2)(gameMap.player.dir + gameMap.player.plane);

        floorCeilComputeShader.SetInt("width", Size.X);
        floorCeilComputeShader.SetInt("height", Size.Y);
        floorCeilComputeShader.SetVector2("pos", (Vector2)gameMap.player.pos);
        floorCeilComputeShader.SetVector2("rayDir0", rayDir0);
        floorCeilComputeShader.SetVector2("rayDir1", rayDir1);

        GL.ActiveTexture(TextureUnit.Texture1);
        GL.TexParameter(TextureTarget.Texture1D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture1D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
        GL.BindTexture(TextureTarget.Texture1D, computeTextures[0]);
        GL.TexImage1D(TextureTarget.Texture1D, 0, PixelInternalFormat.Rgba32f, Size.Y, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
        GL.BindImageTexture(0, computeTextures[0], 0, false, 0, TextureAccess.ReadOnly, SizedInternalFormat.Rgba32f);

        GL.DispatchCompute(1, (int)Math.Ceiling(Size.Y / 2.0f), 1);
        GL.MemoryBarrier(MemoryBarrierFlags.ShaderImageAccessBarrierBit);

        floorCeilShader.Use();
        floorCeilShader.SetInt("width", Size.X);
        floorCeilShader.SetInt("height", Size.Y);
        floorCeilShader.SetInt("floorTexNum", 3);
        floorCeilShader.SetInt("ceilTexNum", 6);


        GL.DrawArraysInstanced(PrimitiveType.Lines, 0, vertexCount, Size.Y);
    }

    private void drawWalls()
    {
        Debug.Assert(wallComputeShader != null);
        Debug.Assert(wallShader != null);
        wallComputeShader.Use();
        for (int i = 0; i < gameMap.worldMap.GetLength(0); i++)
        {
            for (int j = 0; j < gameMap.worldMap.GetLength(1); j++)
            {
                GL.Uniform1(GL.GetUniformLocation(wallComputeShader.Handle, $"map[{i * gameMap.worldMap.GetLength(0) + j}]"), gameMap.worldMap[i, j]);
            }
        }
        wallComputeShader.SetInt("mapWidth", gameMap.worldMap.GetLength(0));
        wallComputeShader.SetVector2("pos", (Vector2)gameMap.player.pos);
        wallComputeShader.SetVector2("dir", (Vector2)gameMap.player.dir);
        wallComputeShader.SetVector2("plane", (Vector2)gameMap.player.plane);

        GL.ActiveTexture(TextureUnit.Texture2);
        GL.TexParameter(TextureTarget.Texture1D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture1D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
        GL.BindTexture(TextureTarget.Texture1D, computeTextures[1]);
        GL.TexImage1D(TextureTarget.Texture1D, 0, PixelInternalFormat.Rgba32f, Size.X, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
        GL.BindImageTexture(1, computeTextures[1], 0, false, 0, TextureAccess.ReadOnly, SizedInternalFormat.Rgba32f);

        GL.DispatchCompute(Size.X, 1, 1);
        GL.MemoryBarrier(MemoryBarrierFlags.ShaderImageAccessBarrierBit);

        wallShader.Use();
        wallShader.SetInt("width", Size.X);

        //draw the pixels of the stripe as a vertical line
        GL.DrawArraysInstanced(PrimitiveType.Lines, 0, vertexCount, Size.X);
    }

    private void drawSprites()
    {
        Debug.Assert(spriteShader != null);
        Debug.Assert(spriteComputeShader != null);
        spriteShader.Use();
        int numSprites = gameMap.sprites.Length;
        int[] spriteOrder = new int[numSprites];
        double[] spriteDistance = new double[numSprites];
        //sort sprites from far to close
        for (int i = 0; i < numSprites; i++)
        {
            spriteOrder[i] = i;
            spriteDistance[i] = (gameMap.player.pos - gameMap.sprites[i].pos).LengthSquared;
        }
        Array.Sort(spriteOrder, (x, y) => spriteDistance[y].CompareTo(spriteDistance[x]));

        //after sorting the sprites, do the projection and draw them
        for (int i = 0; i < numSprites; i++)
        {
            //translate sprite position to relative to camera
            Vector2d sprite = gameMap.sprites[spriteOrder[i]].pos - gameMap.player.pos;

            //transform sprite with the inverse camera matrix
            // [ planeX   dirX ] -1                                       [ dirY      -dirX ]
            // [               ]       =  1/(planeX*dirY-dirX*planeY) *   [                 ]
            // [ planeY   dirY ]                                          [ -planeY  planeX ]

            double invDet = 1.0 / (gameMap.player.plane.X * gameMap.player.dir.Y - gameMap.player.dir.X * gameMap.player.plane.Y); //required for correct matrix multiplication

            double transformX = invDet * (gameMap.player.dir.Y * sprite.X - gameMap.player.dir.X * sprite.Y);
            double transformY = invDet * (-gameMap.player.plane.Y * sprite.X + gameMap.player.plane.X * sprite.Y); //this is actually the depth inside the screen, that what Z is in 3D

            double spriteScreenX = transformX / transformY;

            //calculate height of the sprite on screen
            double spriteHeight = Math.Abs(1 / transformY); //using 'transformY' instead of the real distance prevents fisheye
                                                            //calculate lowest and highest pixel to fill in current stripe

            //calculate width of the sprite
            double spriteWidth = Math.Abs(1 / transformY);

            int xstart = Math.Max((int)(Size.X * (-spriteWidth / 2.0 + spriteScreenX + 1.0) / 2.0), 0);
            int xend = Math.Min((int)(Size.X * (spriteWidth / 2.0 + spriteScreenX + 1.0) / 2.0), (Size.X - 1));

            if (transformY > 0 && xend - xstart > 0)
            {
                spriteComputeShader.Use();
                spriteComputeShader.SetInt("xoffset", xstart);
                spriteComputeShader.SetInt("screenwidth", Size.X);
                spriteComputeShader.SetFloat("spriteWidth", (float)spriteWidth);
                spriteComputeShader.SetFloat("spriteScreenX", (float)spriteScreenX);


                GL.ActiveTexture(TextureUnit.Texture1);
                GL.TexParameter(TextureTarget.Texture1D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture1D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
                GL.BindTexture(TextureTarget.Texture1D, computeTextures[2]);
                GL.TexImage1D(TextureTarget.Texture1D, 0, PixelInternalFormat.Rgba32f, Size.Y, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
                GL.BindImageTexture(2, computeTextures[2], 0, false, 0, TextureAccess.ReadOnly, SizedInternalFormat.Rgba32f);

                GL.DispatchCompute(Size.X, 1, 1);
                GL.MemoryBarrier(MemoryBarrierFlags.ShaderImageAccessBarrierBit);

                spriteShader.Use();
                spriteShader.SetFloat("spriteheight", (float)spriteHeight);
                spriteShader.SetInt("screenwidth", Size.X);
                spriteShader.SetInt("texNum", gameMap.sprites[spriteOrder[i]].texture);
                spriteShader.SetInt("xoffset", xstart);
                spriteShader.SetFloat("transformY", (float)transformY);
                GL.DrawArraysInstanced(PrimitiveType.Lines, 0, vertexCount, xend - xstart);
            }
        }

    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        var input = KeyboardState;

        if (input.IsKeyDown(Keys.Escape))
        {
            Close();
        }

        double moveSpeed = (float)args.Time * 5.0;
        double rotSpeed = (float)args.Time * 3.0;

        //move forward if no wall in front of you
        if (input.IsKeyDown(Keys.Up) || input.IsKeyDown(Keys.W))
        {
            gameMap.MovePlayer(moveSpeed);
        }
        //move backwards if no wall behind you
        if (input.IsKeyDown(Keys.Down) || input.IsKeyDown(Keys.S))
        {
            gameMap.MovePlayer(-moveSpeed);
        }
        //rotate to the right
        if (input.IsKeyDown(Keys.Right) || input.IsKeyDown(Keys.D))
        {
            gameMap.SpinPlayer(-rotSpeed);
        }
        //rotate to the left
        if (input.IsKeyDown(Keys.Left) || input.IsKeyDown(Keys.A))
        {
            gameMap.SpinPlayer(rotSpeed);
        }
    }


    protected override void OnResize(ResizeEventArgs e)
    {
        GL.Viewport(0, 0, Size.X, Size.Y);

        base.OnResize(e);
    }

    protected override void OnUnload()
    {
        Debug.Assert(wallShader != null);

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
        GL.UseProgram(0);

        GL.DeleteBuffer(vertexBufferObject);
        GL.DeleteVertexArray(vertexArrayObject);

        GL.DeleteProgram(wallShader.Handle);

        GL.DeleteTextures(3, computeTextures);

        base.OnUnload();
    }
}
