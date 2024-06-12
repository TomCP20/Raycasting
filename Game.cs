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
        //y pos
         0.5f,
        -0.5f,
    };


    const int vertexCount = 2;

    private int vertexBufferObject;

    private int vertexArrayObject;

    private Shader? shader;

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
        GL.VertexAttribPointer(0, 1, VertexAttribPointerType.Float, false, 1 * sizeof(float), 0);

        shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");

        shader.Use();
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        Debug.Assert(shader != null);

        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit);

        shader.Use();

        for (int x = 0; x < Size.X; x++)
        {
            var (color, lineHeight, cameraX) = Cast_Ray(x);
            //draw the pixels of the stripe as a vertical line

            GL.Uniform3(GL.GetUniformLocation(shader.Handle, "objectColor"), color);
            GL.Uniform1(GL.GetUniformLocation(shader.Handle, "height"), (float)lineHeight);
            GL.Uniform1(GL.GetUniformLocation(shader.Handle, "x"), (float)cameraX);

            GL.BindVertexArray(vertexArrayObject);
            GL.DrawArrays(PrimitiveType.Lines, 0, vertexCount);
        }

        SwapBuffers();
    }

    private (Vector3, double, double) Cast_Ray(int x)
    {
        Debug.Assert(shader != null);

        double cameraX = (x * 2.0 / Size.X) - 1.0; //x-coordinate in camera space
                                                         //calculate ray position and direction
        double rayDirX = gameMap.player.DirX + gameMap.player.PlaneX * cameraX;
        double rayDirY = gameMap.player.DirY + gameMap.player.PlaneY * cameraX;

        //which box of the map we're in
        int mapX = (int)Math.Floor(gameMap.player.PosX);
        int mapY = (int)Math.Floor(gameMap.player.PosY);

        //length of ray from current position to next x or y-side
        double sideDistX;
        double sideDistY;

        //length of ray from one x or y-side to next x or y-side
        double deltaDistX = (Math.Abs(rayDirX) < 1e-8) ? 1e8 : Math.Abs(1 / rayDirX);
        double deltaDistY = (Math.Abs(rayDirY) < 1e-8) ? 1e8 : Math.Abs(1 / rayDirY);
        double perpWallDist;

        //what direction to step in x or y-direction (either +1 or -1)
        int stepX;
        int stepY;

        int hit = 0; //was there a wall hit?
        int side = -1; //was a NS or a EW wall hit?

        //calculate step and initial sideDist
        if (rayDirX < 0)
        {
            stepX = -1;
            sideDistX = (gameMap.player.PosX - mapX) * deltaDistX;
        }
        else
        {
            stepX = 1;
            sideDistX = (mapX + 1.0 - gameMap.player.PosX) * deltaDistX;
        }
        if (rayDirY < 0)
        {
            stepY = -1;
            sideDistY = (gameMap.player.PosY - mapY) * deltaDistY;
        }
        else
        {
            stepY = 1;
            sideDistY = (mapY + 1.0 - gameMap.player.PosY) * deltaDistY;
        }

        //perform DDA
        while (hit == 0)
        {
            //jump to next map square, either in x-direction, or in y-direction
            if (sideDistX < sideDistY)
            {
                sideDistX += deltaDistX;
                mapX += stepX;
                side = 0;
            }
            else
            {
                sideDistY += deltaDistY;
                mapY += stepY;
                side = 1;
            }
            //Check if ray has hit a wall
            if (gameMap.worldMap[mapX, mapY] > 0) hit = 1;
        }

        //Calculate distance projected on camera direction (Euclidean distance would give fisheye effect!)
        if (side == 0) perpWallDist = Math.Abs(sideDistX - deltaDistX);
        else perpWallDist = Math.Abs(sideDistY - deltaDistY);

        //Calculate height of line to draw on screen
        double lineHeight = Size.Y / (perpWallDist * 250.0f);

        //choose wall color
        Vector3 color = getColor(mapX, mapY);

        //give x and y sides different brightness
        if (side == 1) { color = color / 2; }

        return (color, lineHeight, cameraX);
    }

    private Vector3 getColor(int mapX, int mapY)
    {
        return gameMap.worldMap[mapX, mapY] switch
        {
            1 => new Vector3(1.0f, 0.0f, 0.0f),
            2 => new Vector3(0.0f, 1.0f, 0.0f),
            3 => new Vector3(0.0f, 0.0f, 1.0f),
            4 => new Vector3(1.0f, 1.0f, 1.0f),
            _ => new Vector3(1.0f, 1.0f, 0.0f),
        };
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

        if (IsMouseButtonReleased(MouseButton.Left))
        {
            Console.WriteLine(MouseState.Position);
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
