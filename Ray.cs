using OpenTK.Mathematics;

namespace Raycasting;

public class Ray
{
    private readonly Vector2d pos;

    private readonly Vector2d dir;
    

    public Vector2i mapPos {get; set;}
    private readonly Vector2d deltaDist;
    private readonly Vector2i step;
    private Vector2d sideDist;

    public int side {get; set;}

    public Ray(Vector2d Pos, Vector2d Dir)
    {
        pos = Pos;
        dir = Dir;


        //which box of the map we're in
        mapPos = new((int)Math.Floor(pos.X), (int)Math.Floor(pos.Y));

        //length of ray from one x or y-side to next x or y-side
        deltaDist.X = (float)((Math.Abs(dir.X) < 1e-8) ? 1e8 : Math.Abs(1 / dir.X));
        deltaDist.Y = (float)((Math.Abs(dir.Y) < 1e-8) ? 1e8 : Math.Abs(1 / dir.Y));

        //calculate step and initial sideDist
        if (dir.X < 0)
        {
            step.X = -1;
            sideDist.X = (pos.X - mapPos.X) * deltaDist.X;
        }
        else
        {
            step.X = 1;
            sideDist.X = (float)((mapPos.X + 1.0 - pos.X) * deltaDist.X);
        }
        if (dir.Y < 0)
        {
            step.Y = -1;
            sideDist.Y = (pos.Y - mapPos.Y) * deltaDist.Y;
        }
        else
        {
            step.Y = 1;
            sideDist.Y = (float)((mapPos.Y + 1.0 - pos.Y) * deltaDist.Y);
        }
    }

    public void stepRay()
    {
        //jump to next map square, either in x-direction, or in y-direction
        Vector2i newMapPos = mapPos;
        if (sideDist.X < sideDist.Y)
        {
            sideDist.X += deltaDist.X;
            newMapPos.X += step.X;
            side = 0;
        }
        else
        {
            sideDist.Y += deltaDist.Y;
            newMapPos.Y += step.Y;
            side = 1;
        }
        mapPos = newMapPos;
    }

    public double getPerpWallDist()
    {
        double perpWallDist;
        if (side == 0) perpWallDist = Math.Abs(sideDist.X - deltaDist.X);
        else perpWallDist = Math.Abs(sideDist.Y - deltaDist.Y);
        return perpWallDist;
    }

    public double getWallX(double perpWallDist)
    {
        //calculate value of wallX
        double wallX; //where exactly the wall was hit
        if (side == 0) wallX = pos.Y + perpWallDist * dir.Y;
        else wallX = pos.X + perpWallDist * dir.X;
        wallX -= Math.Floor(wallX);

        //x coordinate on the texture
        if ((side == 0 && dir.X > 0) ^ (side == 1 && dir.Y < 0)) wallX = 1 - wallX;

        return wallX;
    }
}