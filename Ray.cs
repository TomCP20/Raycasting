namespace Raycasting;

public class Ray
{
    private readonly double posX;

    private readonly double posY;
    private readonly double dirX;
    private readonly double dirY;

    public int mapX {get; set;}
    public int mapY {get; set;}
    private readonly double deltaDistX;
    private readonly double deltaDistY;
    private readonly int stepX;
    private readonly int stepY;
    private double sideDistX;
    private double sideDistY;

    public int side {get; set;}

    public Ray(double PosX, double PosY, double DirX, double DirY)
    {
        posX = PosX;
        posY = PosY;
        dirX = DirX;
        dirY = DirY;


        //which box of the map we're in
        mapX = (int)Math.Floor(posX);
        mapY = (int)Math.Floor(posY);

        //length of ray from one x or y-side to next x or y-side
        deltaDistX = (Math.Abs(dirX) < 1e-8) ? 1e8 : Math.Abs(1 / dirX);
        deltaDistY = (Math.Abs(dirY) < 1e-8) ? 1e8 : Math.Abs(1 / dirY);

        //calculate step and initial sideDist
        if (dirX < 0)
        {
            stepX = -1;
            sideDistX = (posX - mapX) * deltaDistX;
        }
        else
        {
            stepX = 1;
            sideDistX = (mapX + 1.0 - posX) * deltaDistX;
        }
        if (dirY < 0)
        {
            stepY = -1;
            sideDistY = (posY - mapY) * deltaDistY;
        }
        else
        {
            stepY = 1;
            sideDistY = (mapY + 1.0 - posY) * deltaDistY;
        }
    }

    public void step()
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
    }

    public double getPerpWallDist()
    {
        double perpWallDist;
        if (side == 0) perpWallDist = Math.Abs(sideDistX - deltaDistX);
        else perpWallDist = Math.Abs(sideDistY - deltaDistY);
        return perpWallDist;
    }

    public double getWallX(double perpWallDist)
    {
        //calculate value of wallX
        double wallX; //where exactly the wall was hit
        if (side == 0) wallX = posY + perpWallDist * dirY;
        else wallX = posX + perpWallDist * dirX;
        wallX -= Math.Floor(wallX);

        //x coordinate on the texture
        if ((side == 0 && dirX > 0) ^ (side == 1 && dirY < 0)) wallX = 1 - wallX;

        return wallX;
    }
}