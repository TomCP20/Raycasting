namespace Raycasting;

public class ShaderLinkException : Exception
{
    public ShaderLinkException() { }

    public ShaderLinkException(string message)
        : base(message) { }

    public ShaderLinkException(string message, Exception inner)
        : base(message, inner) { }

}