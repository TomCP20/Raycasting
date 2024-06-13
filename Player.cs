using OpenTK.Mathematics;

namespace Raycasting;

public struct Player(double posX, double posY, double dirX, double dirY, double planeX, double planeY)
{
    public Vector2d pos { get; set; } = new Vector2d(posX, posY);
    public Vector2d dir { get; set; } = new Vector2d(dirX, dirY);
    public Vector2d plane { get; set; } = new Vector2d(planeX, planeY);
}
