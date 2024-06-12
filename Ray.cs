namespace Raycasting;

public class Ray
{
    private double DirX;
    private double DirY;

    public int mapX;
    public int mapY;
    private double deltaDistX;
    private double deltaDistY;
    private int stepX;
    private int stepY;
    private double sideDistX;
    private double sideDistY;

    public int hit = 0;

    public int side;

    public Ray(double posX, double posY, double DirX, double DirY)
    {
        this.DirX = DirX;
        this.DirY = DirY;

        //which box of the map we're in
        mapX = (int)Math.Floor(posX);
        mapY = (int)Math.Floor(posY);

        //length of ray from one x or y-side to next x or y-side
        deltaDistX = (Math.Abs(DirX) < 1e-8) ? 1e8 : Math.Abs(1 / DirX);
        deltaDistY = (Math.Abs(DirY) < 1e-8) ? 1e8 : Math.Abs(1 / DirY);

        //calculate step and initial sideDist
        if (DirX < 0)
        {
            stepX = -1;
            sideDistX = (posX - mapX) * deltaDistX;
        }
        else
        {
            stepX = 1;
            sideDistX = (mapX + 1.0 - posX) * deltaDistX;
        }
        if (DirY < 0)
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
}