using Image_Compressor;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

var image = Image.Load<Rgb24>(@"R:\Image.jpg");
var output = ImageCompressor.Compress(image, 3, 100, 5);
ImageCompressor.SaveToFile(output, @"R:\compressed");
ImageCompressor.Decompress(output, Math.Max(image.Width, image.Height), Math.Max(image.Width, image.Height));
//ImageCompressor.Decompress(ImageCompressor.LoadFromFile(@"R:\compressed"), Math.Max(image.Width, image.Height), Math.Max(image.Width, image.Height));

//ImageCompressor.Decompress(ImageCompressor.LoadFromFile(@"R:\compressed"), 1024, 1024);