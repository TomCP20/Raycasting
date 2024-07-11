# Raycasting

 Raycasting renderer in C# using OpenTK.

## Build And Run

To run this project use the command `dotnet run` in the base folder of the project.

To build this project use the command `dotnet build` in the base of folder the project.

## Controls

* press <kbd>W</kbd> or <kbd>↑</kbd> to move the player forwards.
* press <kbd>S</kbd> or <kbd>↓</kbd> to move the player backwards.
* press <kbd>A</kbd> or <kbd>←</kbd> to pan the camera left.
* press <kbd>D</kbd> or <kbd>→</kbd> to pan the camera right.
* press <kbd>ESC</kbd> to quit.
* press <kbd>F</kbd> to toggle fullscreen.
* press <kbd>O</kbd> to cycle throught the [post-processing modes](#post-processing-modes).

## Post-Processing Modes

### Normal

The default mode, no post-processing effects are applied.

### Invert

The colour of the image is inverted.

### Grey-Scale

The image is converted to grey scale.

### Sharpen

A kernel is used to sharpen the image.

### Blur

A kernel is used to blur the image.

### Edge-Detection

A kernel is used to detect th edges of the image.

## References

[Lode's Computer Graphics Tutorial: Raycasting](https://lodev.org/cgtutor/raycasting.html)

[LearnOpenTK](https://opentk.net/learn/index.html)

[Learn OpenGL](https://learnopengl.com/)

[PyOpenGL RayCasting](https://www.youtube.com/watch?v=p61mCoASwZ0)

## Screenshots

![1](/Screenshots/example%201.png)

![2](/Screenshots/example%202.png)

![3](/Screenshots/example%203.png)

![4](/Screenshots/example%204.png)

![5](/Screenshots/example%205.png)

![6](/Screenshots/example%206.png)
