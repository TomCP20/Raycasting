using OpenTK.Mathematics;

namespace Raycasting;
public class GameMap
{

    public readonly int[,] worldMap =
    {
        {8,8,8,8,8,8,8,8,8,8,8,4,4,6,4,4,6,4,6,4,4,4,6,4},
        {8,0,0,0,0,0,0,0,0,0,8,4,0,0,0,0,0,0,0,0,0,0,0,4},
        {8,0,3,3,0,0,0,0,0,8,8,4,0,0,0,0,0,0,0,0,0,0,0,6},
        {8,0,0,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,6},
        {8,0,3,3,0,0,0,0,0,8,8,4,0,0,0,0,0,0,0,0,0,0,0,4},
        {8,0,0,0,0,0,0,0,0,0,8,4,0,0,0,0,0,6,6,6,0,6,4,6},
        {8,8,8,8,0,8,8,8,8,8,8,4,4,4,4,4,4,6,0,0,0,0,0,6},
        {7,7,7,7,0,7,7,7,7,0,8,0,8,0,8,0,8,4,0,4,0,6,0,6},
        {7,7,0,0,0,0,0,0,7,8,0,8,0,8,0,8,8,6,0,0,0,0,0,6},
        {7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,6,0,0,0,0,0,4},
        {7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,6,0,6,0,6,0,6},
        {7,7,0,0,0,0,0,0,7,8,0,8,0,8,0,8,8,6,4,6,0,6,6,6},
        {7,7,7,7,0,7,7,7,7,8,8,4,0,6,8,4,8,3,3,3,0,3,3,3},
        {2,2,2,2,0,2,2,2,2,4,6,4,0,0,6,0,6,3,0,0,0,0,0,3},
        {2,2,0,0,0,0,0,2,2,4,0,0,0,0,0,0,4,3,0,0,0,0,0,3},
        {2,0,0,0,0,0,0,0,2,4,0,0,0,0,0,0,4,3,0,0,0,0,0,3},
        {1,0,0,0,0,0,0,0,1,4,4,4,4,4,6,0,6,3,3,0,0,0,3,3},
        {2,0,0,0,0,0,0,0,2,2,2,1,2,2,2,6,6,0,0,5,0,5,0,5},
        {2,2,0,0,0,0,0,2,2,2,0,0,0,2,2,0,5,0,5,0,0,0,5,5},
        {2,0,0,0,0,0,0,0,2,0,0,0,0,0,2,5,0,5,0,5,0,5,0,5},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,5},
        {2,0,0,0,0,0,0,0,2,0,0,0,0,0,2,5,0,5,0,5,0,5,0,5},
        {2,2,0,0,0,0,0,2,2,2,0,0,0,2,2,0,5,0,5,0,0,0,5,5},
        {2,2,2,2,1,2,2,2,2,2,2,1,2,2,2,5,5,5,5,5,5,5,5,5}
    };

    public Player player { get; set; } = new(22, 11.5, -1, 0, 0, 0.66);

    public readonly Sprite[] sprites =
        [
            //green light in front of playerstart
            new(20.5, 11.5, 10),
            //green lights in every room
            new(18.5,4.5, 10),
            new(10.0,4.5, 10),
            new(10.0,12.5,10),
            new(3.5, 6.5, 10),
            new(3.5, 20.5,10),
            new(3.5, 14.5,10),
            new(14.5,20.5,10),
            //row of pillars in front of wall: fisheye test
            new(18.5, 10.5, 9),
            new(18.5, 11.5, 9),
            new(18.5, 12.5, 9),
            //some barrels around the map
            new(21.5, 1.5, 8),
            new(15.5, 1.5, 8),
            new(16.0, 1.8, 8),
            new(16.2, 1.2, 8),
            new(3.5,  2.5, 8),
            new(9.5, 15.5, 8),
            new(10.0, 15.1,8),
            new(10.5, 15.8,8),
        ];

    public void SpinPlayer(double rotSpeed)
    {
        Matrix2d rotMatrix = Matrix2d.CreateRotation((float)rotSpeed);
        //both camera direction and camera plane must be rotated
        Player newplayer = player;
        newplayer.dir = player.dir * rotMatrix;
        newplayer.plane = player.plane * rotMatrix;
        player = newplayer;
    }

    public void MovePlayer(double moveSpeed)
    {
        Vector2d posDelta = player.dir * moveSpeed;
        if (worldMap[(int)(player.pos.X + posDelta.X), (int)player.pos.Y] != 0) posDelta.X = 0;
        if (worldMap[(int)player.pos.X, (int)(player.pos.Y + posDelta.Y)] != 0) posDelta.Y = 0;
        Player newplayer = player;
        newplayer.pos += posDelta;
        player = newplayer;
    }

}