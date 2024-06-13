using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Raycasting;

public class Game : GameWindow
{
    private readonly GameMap gameMap = new GameMap();

    readonly float[] vertices =
    {
        //pos y tex coord
         1f,  1.0f,
        -1f,  0.0f,
    };


    const int vertexCount = 2;

    private int vertexBufferObject;

    private int vertexArrayObject;

    private Shader? wallShader;
    private Shader? floorShader;

    private Texture? texture;
    public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) { }

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

        vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(vertexArrayObject);
        vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 1, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);

        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 1, VertexAttribPointerType.Float, false, 2 * sizeof(float), 1 * sizeof(float));

        wallShader = new Shader("Shaders/wallShader.vert", "Shaders/wallShader.frag");
        floorShader = new Shader("Shaders/floorShader.vert", "Shaders/floorShader.frag");

        texture = Texture.LoadFromFile("Textures/atlas.gif");
        texture.Use(TextureUnit.Texture0);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        Debug.Assert(texture != null);

        base.OnRenderFrame(args);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        texture.Use(TextureUnit.Texture0);

        drawFloor();
        drawWalls();

        SwapBuffers();
    }

    private void drawFloor()
    {
        Debug.Assert(floorShader != null);
        floorShader.Use();
        for (int y = 0; y < Size.Y/2; y++)
        {
            GL.Uniform1(GL.GetUniformLocation(floorShader.Handle, "y"), (float)((y * 2.0 / Size.Y) - 1.0));
            GL.BindVertexArray(vertexArrayObject);
            GL.DrawArrays(PrimitiveType.Lines, 0, vertexCount);
        }

    }

    private void drawWalls()
    {
        Debug.Assert(wallShader != null);
        wallShader.Use();

        for (int x = 0; x < Size.X; x++)
        {
            (double lineHeight, double cameraX, double wallX, int isDark, int texNum) = Cast_Ray(x);

            GL.Uniform1(GL.GetUniformLocation(wallShader.Handle, "height"), (float)lineHeight);
            GL.Uniform1(GL.GetUniformLocation(wallShader.Handle, "x"), (float)cameraX);
            GL.Uniform1(GL.GetUniformLocation(wallShader.Handle, "texX"), (float)wallX);
            GL.Uniform1(GL.GetUniformLocation(wallShader.Handle, "texNum"), texNum);

            GL.Uniform1(GL.GetUniformLocation(wallShader.Handle, "isDark"), isDark);

            //draw the pixels of the stripe as a vertical line
            GL.BindVertexArray(vertexArrayObject);
            GL.DrawArrays(PrimitiveType.Lines, 0, vertexCount);
        }
    }

    private (double, double, double, int, int) Cast_Ray(int x)
    {
        Debug.Assert(wallShader != null);

        double cameraX = (x * 2.0 / Size.X) - 1.0; //x-coordinate in camera space

        //calculate ray position and direction
        double rayDirX = gameMap.player.dirX + gameMap.player.planeX * cameraX;
        double rayDirY = gameMap.player.dirY + gameMap.player.planeY * cameraX;

        Ray ray = new Ray(gameMap.player.posX, gameMap.player.posY, rayDirX, rayDirY);

        //perform DDA
        while (gameMap.worldMap[ray.mapX, ray.mapY] == 0) //Check if ray has hit a wall
        {
            //jump to next map square, either in x-direction, or in y-direction
            ray.step();
        }

        //Calculate distance projected on camera direction (Euclidean distance would give fisheye effect!)
        double perpWallDist = ray.getPerpWallDist();

        //Calculate height of line to draw on screen
        double lineHeight = Size.Y / (perpWallDist * 500.0f);

        //texturing calculations
        int texNum = gameMap.worldMap[ray.mapX, ray.mapY] - 1; //1 subtracted from it so that texture 0 can be used!

        //calculate value of wallX
        double wallX = ray.getWallX(perpWallDist);

        return (lineHeight, cameraX, wallX, ray.side, texNum);
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
        if (input.IsKeyDown(Keys.Up))
        {
            gameMap.MovePlayer(moveSpeed);
        }
        //move backwards if no wall behind you
        if (input.IsKeyDown(Keys.Down))
        {
            gameMap.MovePlayer(-moveSpeed);
        }
        //rotate to the right
        if (input.IsKeyDown(Keys.Right))
        {
            gameMap.SpinPlayer(-rotSpeed);
        }
        //rotate to the left
        if (input.IsKeyDown(Keys.Left))
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

        base.OnUnload();
    }
}
