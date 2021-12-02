using Image_Compressor;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

ImageCompressor.Compress(Image.Load<Rgba32>(@"R:\_Image.jpeg"), 2, 5);

//Console.WriteLine(ImageCompressor.CalculateImageComplexity(Image.Load(@"R:\_Image.png")));

/*mageCompressor.GetImageQuadrants(Image.Load<Rgba32>(@"R:\_Image.jpeg"), out var a, out var b, out var c, out var d);
a.SaveAsBmp(@"R:\a.bmp");
b.SaveAsBmp(@"R:\b.bmp");
c.SaveAsBmp(@"R:\c.bmp");
d.SaveAsBmp(@"R:\d.bmp");*/

/*Console.WriteLine(ImageCompressor.CalculateImageComplexity(a));
Console.WriteLine(ImageCompressor.CalculateImageComplexity(b));
Console.WriteLine(ImageCompressor.CalculateImageComplexity(c));
Console.WriteLine(ImageCompressor.CalculateImageComplexity(d));*/
