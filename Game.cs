using System.Diagnostics;
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

    readonly float[] vertices =
    {
        //pos
         1f,
        -1f,
    };


    const int vertexCount = 2;

    private int vertexBufferObject;

    private int vertexArrayObject;

    private Shader? wallShader;
    private Shader? floorCeilShader;

    private Shader? spriteShader;

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
        GL.VertexAttribPointer(0, 1, VertexAttribPointerType.Float, false, sizeof(float), 0);

        wallShader = new Shader("Shaders/wallShader.vert", "Shaders/wallShader.frag");
        floorCeilShader = new Shader("Shaders/floorCeilShader.vert", "Shaders/floorCeilShader.frag");
        spriteShader = new Shader("Shaders/spriteShader.vert", "Shaders/spriteShader.frag");

        texture = Texture.LoadFromFile("Textures/atlas.gif");
        texture.Use(TextureUnit.Texture0);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        Debug.Assert(texture != null);

        base.OnRenderFrame(args);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        texture.Use(TextureUnit.Texture0);

        drawFloorCeil();
        double[] ZBuffer = drawWalls();
        drawSprites(ZBuffer);

        SwapBuffers();
    }

    private void drawFloorCeil()
    {
        Debug.Assert(floorCeilShader != null);
        floorCeilShader.Use();
        GL.Uniform1(GL.GetUniformLocation(floorCeilShader.Handle, "width"), Size.X);

        Vector2[] floor0s = new Vector2[Size.Y];
        Vector2[] floorSteps = new Vector2[Size.Y];
        float[] ys = new float[Size.Y];
        float[] texNums = new float[Size.Y];

        // rayDir for leftmost ray (x = 0) and rightmost ray (x = w)
        Vector2 rayDir0 = (Vector2)(gameMap.player.dir - gameMap.player.plane);
        Vector2 rayDir1 = (Vector2)(gameMap.player.dir + gameMap.player.plane);

        //Size.Y / 2
        for (int y = 0; y < Math.Floor(Size.Y / 2.0f); y++)
        {
            // Current y position compared to the center of the screen (the horizon)
            int p = -(y - Size.Y / 2);

            // Vertical position of the camera.
            float posZ = (float)(0.5 * Size.Y);

            // Horizontal distance from the camera to the floor for the current row.
            // 0.5 is the z position exactly in the middle between floor and ceiling.
            float rowDistance = posZ / p;

            // calculate the real world step vector we have to add for each x (parallel to camera plane)
            // adding step by step avoids multiplications with a weight in the inner loop

            floorSteps[y] = rowDistance * (rayDir1 - rayDir0) / Size.X;
            floorSteps[Size.Y - y - 1] = rowDistance * (rayDir1 - rayDir0) / Size.X;

            // real world coordinates of the leftmost column. This will be updated as we step to the right.
            floor0s[y] = (Vector2)(gameMap.player.pos + rowDistance * rayDir0);
            floor0s[Size.Y - y - 1] = (Vector2)(gameMap.player.pos + rowDistance * rayDir0);

            texNums[y] = 3;
            texNums[Size.Y - y - 1] = 6;

            ys[y] = (float)((y * 2.0 / Size.Y) - 1.0);
            ys[Size.Y - y - 1] = (float)(((Size.Y - y - 1) * 2.0 / Size.Y) - 1.0);

        }

        bufferInstanceDataVector2(floor0s, 1);
        bufferInstanceDataVector2(floorSteps, 2);
        bufferInstanceDataFloat(ys, 3);
        bufferInstanceDataFloat(texNums, 4);

        GL.DrawArraysInstanced(PrimitiveType.Lines, 0, vertexCount, Size.Y);
    }

    private double[] drawWalls()
    {
        Debug.Assert(wallShader != null);
        wallShader.Use();

        float[] heights = new float[Size.X];
        float[] cameraXs = new float[Size.X];
        float[] isDarks = new float[Size.X];
        float[] texXs = new float[Size.X];
        float[] texNums = new float[Size.X];

        double[] ZBuffer = new double[Size.X];

        for (int x = 0; x < Size.X; x++)
        {
            cameraXs[x] = (float)((x * 2.0 / Size.X) - 1.0); //x-coordinate in camera space

            //calculate ray position and direction
            Vector2d rayDir = gameMap.player.dir + (gameMap.player.plane * cameraXs[x]);

            Ray ray = new Ray(gameMap.player.pos, rayDir);

            //perform DDA
            while (gameMap.worldMap[ray.mapPos.X, ray.mapPos.Y] == 0) //Check if ray has hit a wall
            {
                //jump to next map square, either in x-direction, or in y-direction
                ray.stepRay();
            }

            //Calculate distance projected on camera direction (Euclidean distance would give fisheye effect!)
            double perpWallDist = ray.getPerpWallDist();

            //Calculate height of line to draw on screen
            heights[x] = (float)(Size.Y / (perpWallDist * 500.0f));

            //texturing calculations
            texNums[x] = gameMap.worldMap[ray.mapPos.X, ray.mapPos.Y] - 1; //1 subtracted from it so that texture 0 can be used!

            //calculate value of wallX
            texXs[x] = (float)ray.getWallX(perpWallDist);

            isDarks[x] = ray.side;

            ZBuffer[x] = perpWallDist;
        }

        bufferInstanceDataFloat(heights, 1);
        bufferInstanceDataFloat(cameraXs, 2);
        bufferInstanceDataFloat(isDarks, 3);
        bufferInstanceDataFloat(texXs, 4);
        bufferInstanceDataFloat(texNums, 5);

        //draw the pixels of the stripe as a vertical line
        GL.DrawArraysInstanced(PrimitiveType.Lines, 0, vertexCount, Size.X);

        return ZBuffer;
    }

    private void drawSprites(double[] ZBuffer)
    {
        Debug.Assert(spriteShader != null);
        spriteShader.Use();
        int numSprites = gameMap.sprites.Length;
        int[] spriteOrder = new int[numSprites];
        double[] spriteDistance = new double[numSprites];
        //sort sprites from far to close
        for (int i = 0; i < numSprites; i++)
        {
            spriteOrder[i] = i;
            spriteDistance[i] = (gameMap.player.pos.X - gameMap.sprites[i].pos.X) * (gameMap.player.pos.X - gameMap.sprites[i].pos.X) + (gameMap.player.pos.Y - gameMap.sprites[i].pos.Y) * (gameMap.player.pos.Y - gameMap.sprites[i].pos.Y);
        }
        sortSprites(spriteOrder, spriteDistance, numSprites);

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

            double spriteScreenX = (transformX / transformY);

            //calculate height of the sprite on screen
            double spriteHeight = Math.Abs(1 / transformY); //using 'transformY' instead of the real distance prevents fisheye
                                                            //calculate lowest and highest pixel to fill in current stripe

            //calculate width of the sprite
            double spriteWidth = Math.Abs(1 / transformY);

            GL.Uniform1(GL.GetUniformLocation(spriteShader.Handle, "height"), (float)spriteHeight);
            for (double x = -spriteWidth / 2 + spriteScreenX; x < spriteWidth / 2 + spriteScreenX; x += 2.0 / Size.X)
            {
                if (transformY > 0 && x >= -1 && x <= 1 && transformY < ZBuffer[(int)(Size.X*(x+1)/2)])
                {
                    GL.Uniform1(GL.GetUniformLocation(spriteShader.Handle, "x"), (float)x);
                    GL.DrawArrays(PrimitiveType.Lines, 0, vertexCount);
                }
            }


            //loop through every vertical stripe of the sprite on screen
            /*
            for (int stripe = (int)drawStartX; stripe < drawEndX; stripe++)
            {
                double texX = (stripe - (-spriteWidth / 2 + spriteScreenX)) / spriteWidth;
                //the conditions in the if are:
                //1) it's in front of camera plane so you don't see things behind you
                //2) it's on the screen (left)
                //3) it's on the screen (right)
                //4) ZBuffer, with perpendicular distance
                if (transformY > 0 && transformY < ZBuffer[stripe])
                {
                    for (int y = (int)drawStartY; y < drawEndY; y++) //for every pixel of the current stripe
                    {
                        double d = y - (Size.Y + spriteHeight)/2; //256 and 128 factors to avoid floats
                        double texY = d / spriteHeight;
                    }
                }
            }
            */
        }

    }

    private void sortSprites(int[] spriteOrder, double[] spriteDistance, int numSprites)
    {
        Tuple<double, int>[] sprites = new Tuple<double, int>[numSprites];
        for (int i = 0; i < numSprites; i++)
        {
            sprites[i] = new Tuple<double, int>(spriteDistance[i], spriteOrder[i]);
        }
        Array.Sort(sprites, (x, y) => y.Item1.CompareTo(x.Item1));
        // restore in reverse order to go from farthest to nearest
        for (int i = 0; i < numSprites; i++)
        {
            (spriteDistance[i], spriteOrder[i]) = sprites[i];
        }
    }

    private void bufferInstanceDataFloat(float[] data, int attributeIndex)
    {
        int instanceVBO = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, instanceVBO);
        GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * data.Length, data, BufferUsageHint.StaticDraw);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        GL.EnableVertexAttribArray(attributeIndex);
        GL.BindBuffer(BufferTarget.ArrayBuffer, instanceVBO); // this attribute comes from a different vertex buffer
        GL.VertexAttribPointer(attributeIndex, 1, VertexAttribPointerType.Float, false, sizeof(float), IntPtr.Zero);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.VertexAttribDivisor(attributeIndex, 1); // tell OpenGL this is an instanced vertex attribute.
    }

    private void bufferInstanceDataVector2(Vector2[] data, int attributeIndex)
    {
        int instanceVBO = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, instanceVBO);
        GL.BufferData(BufferTarget.ArrayBuffer, Vector2.SizeInBytes * data.Length, data, BufferUsageHint.StaticDraw);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        GL.EnableVertexAttribArray(attributeIndex);
        GL.BindBuffer(BufferTarget.ArrayBuffer, instanceVBO); // this attribute comes from a different vertex buffer
        GL.VertexAttribPointer(attributeIndex, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, IntPtr.Zero);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.VertexAttribDivisor(attributeIndex, 1); // tell OpenGL this is an instanced vertex attribute.
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
