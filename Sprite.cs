
using OpenTK.Mathematics;

namespace Raycasting;
public struct Sprite(double X, double Y, int Texture)
{
    public Vector2d pos { get; set; } = new Vector2d(X, Y);
    public int texture { get; set; } = Texture;
};