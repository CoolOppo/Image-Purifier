# Image Purifier
*Image Purifier makes use of [OptiPNG](http://optipng.sourceforge.net/), [Gifsicle](https://github.com/kohler/gifsicle), and [mozjpeg](https://github.com/mozilla/mozjpeg) to losslessly optimize images. You must have `optipng`, `gifsicle`, and `jpegtran` available in your path for the program to function properly. Additionally, it strips all metadata, which will cause JPEGs with rotation metadata to return to an incorrect orientation.*

## Usage

```sh
purify (image|directory)
```

On Windows, you can also drag and drop an image or folder onto the executable in order to begin optimizing all of the images.

## License

Image Purifier is licensed under GNU AGPLv3. Read the full license at [LICENSE.txt](LICENSE.txt).
