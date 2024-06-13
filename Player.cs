namespace Raycasting;

public struct Player(double posX, double posY, double dirX, double dirY, double planeX, double planeY)
{
    public double posX { get; set; } = posX;
    public double posY { get; set; } = posY;
    public double dirX { get; set; } = dirX;
    public double dirY { get; set; } = dirY;
    public double planeX { get; set; } = planeX;
    public double planeY { get; set; } = planeY;
}
