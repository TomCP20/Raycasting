namespace Raycasting;

public struct Player(double posX, double posY, double dirX, double dirY, double planeX, double planeY)
{
    public double PosX { get; set; } = posX;
    public double PosY { get; set; } = posY;
    public double DirX { get; set; } = dirX;
    public double DirY { get; set; } = dirY;
    public double PlaneX { get; set; } = planeX;
    public double PlaneY { get; set; } = planeY;
}
