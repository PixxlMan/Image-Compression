using Image_Compressor;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

var image = Image.Load<Rgba32>(@"R:\Image.jpg");
var output = ImageCompressor.Compress(image, 1.5f, 12);
ImageCompressor.Decompress(output, Math.Max(image.Width, image.Height), Math.Max(image.Width, image.Height));

//Console.WriteLine(ImageCompressor.CalculateImageComplexity(Image.Load(@"R:\_Image.png")));

/*ImageCompressor.GetImageQuadrants(Image.Load<Rgba32>(@"R:\_Image.jpeg"), out var a, out var b, out var c, out var d);
a.SaveAsBmp(@"R:\a.bmp"); 
b.SaveAsBmp(@"R:\b.bmp");
c.SaveAsBmp(@"R:\c.bmp");
d.SaveAsBmp(@"R:\d.bmp");*/

/*Console.WriteLine(ImageCompressor.CalculateImageComplexity(a));
Console.WriteLine(ImageCompressor.CalculateImageComplexity(b));
Console.WriteLine(ImageCompressor.CalculateImageComplexity(c));
Console.WriteLine(ImageCompressor.CalculateImageComplexity(d));*/
