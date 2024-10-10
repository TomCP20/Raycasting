using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Raycasting;

const int screenWidth = 640;
const int screenHeight = 480;

NativeWindowSettings nativeWindowSettings = new NativeWindowSettings()
{
    ClientSize = new Vector2i(screenWidth, screenHeight),
    Title = "Raycasting",
    // This is needed to run on macos
    Flags = ContextFlags.ForwardCompatible,
};

using (Game window = new Game(GameWindowSettings.Default, nativeWindowSettings))
{
    window.Run();
}