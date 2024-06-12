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
        //y pos y tex coord
         0.5f,  1.0f,
        -0.5f,  0.0f,
    };


    const int vertexCount = 2;

    private int vertexBufferObject;

    private int vertexArrayObject;

    private Shader? shader;

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

        shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
        shader.Use();

        texture = Texture.LoadFromFile("Textures/atlas.gif");
        texture.Use(TextureUnit.Texture0);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        Debug.Assert(shader != null);
        Debug.Assert(texture != null);

        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit);

        texture.Use(TextureUnit.Texture0);
        shader.Use();

        for (int x = 0; x < Size.X; x++)
        {
            var (lineHeight, cameraX, wallX, isDark, texNum) = Cast_Ray(x);

            GL.Uniform1(GL.GetUniformLocation(shader.Handle, "height"), (float)lineHeight);
            GL.Uniform1(GL.GetUniformLocation(shader.Handle, "x"), (float)cameraX);
            GL.Uniform1(GL.GetUniformLocation(shader.Handle, "texX"), (float)wallX);
            GL.Uniform1(GL.GetUniformLocation(shader.Handle, "texNum"), texNum);

            GL.Uniform1(GL.GetUniformLocation(shader.Handle, "isDark"), isDark);

            //draw the pixels of the stripe as a vertical line
            GL.BindVertexArray(vertexArrayObject);
            GL.DrawArrays(PrimitiveType.Lines, 0, vertexCount);
        }

        SwapBuffers();
    }

    private (double, double, double, int, int) Cast_Ray(int x)
    {
        Debug.Assert(shader != null);

        double cameraX = (x * 2.0 / Size.X) - 1.0; //x-coordinate in camera space
        
        //calculate ray position and direction
        double rayDirX = gameMap.player.DirX + gameMap.player.PlaneX * cameraX;
        double rayDirY = gameMap.player.DirY + gameMap.player.PlaneY * cameraX;

        Ray ray = new Ray(gameMap.player.PosX, gameMap.player.PosY, rayDirX, rayDirY);

        //perform DDA
        while (true)
        {
            //jump to next map square, either in x-direction, or in y-direction
            ray.step();
            //Check if ray has hit a wall
            if (gameMap.worldMap[ray.mapX, ray.mapY] > 0) break;
        }

        //Calculate distance projected on camera direction (Euclidean distance would give fisheye effect!)
        double perpWallDist = ray.getPerpWallDist();

        //Calculate height of line to draw on screen
        double lineHeight = Size.Y / (perpWallDist * 250.0f);

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
        Debug.Assert(shader != null);

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
        GL.UseProgram(0);

        GL.DeleteBuffer(vertexBufferObject);
        GL.DeleteVertexArray(vertexArrayObject);

        GL.DeleteProgram(shader.Handle);

        base.OnUnload();
    }
}
