# Raycasting

 Raycasting renderer in C# using OpenTK.

## Build And Run

To run this project, use the command `dotnet run` in the project's base folder.

To build this project, use the command `dotnet build` in the project's base folder.

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

![Normal](/Screenshots/example%201.png)

### Invert

The colour of the image is inverted.

![Invert](/Screenshots/example%207.png)

### Grey-Scale

The image is converted to grey scale.

![Grey-Scale](/Screenshots/example%208.png)

### Sharpen

A kernel is used to sharpen the image.

![Sharpen](/Screenshots/example%209.png)

### Blur

A kernel is used to blur the image.

![Blur](/Screenshots/example%2010.png)

### Edge-Detection

A kernel is used to detect the edges of the image.

![Edge-Detection](/Screenshots/example%2011.png)

### Embossing

A kernel is used to emboss the image.

![Embossing](/Screenshots/example%2012.png)

### Chromatic Aberration

A chromatic aberration effect is applied to the image.

![Chromatic Aberration](/Screenshots/example%2013.png)

### Colour Quantization

The colour pallet of the image is reduced.

![Colour Quantization](/Screenshots/example%2014.png)

### Ordered Dithering

A dithering effect is applied to the image.

![Ordered Dithering](/Screenshots/example%2015.png)

### Grey-Scale Ordered Dithering

A dithering effect is applied to a greyscale version of the image.

![Grey-Scale Ordered Dithering](/Screenshots/example%2016.png)

### Scanlines

Scanlines are added to the image.

![Scanlines](/Screenshots/example%2017.png)

### RGB Half-Tone

A RGB halftone effect is applied to the image.

![RGB Half-Tone](/Screenshots/example%2018.png)

### Grey-Scale Half-Tone

A greyscale halftone effect is applied to the image.

![Grey-Scale Half-Tone](/Screenshots/example%2019.png)

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
