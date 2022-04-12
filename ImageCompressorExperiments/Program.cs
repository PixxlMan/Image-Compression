using Image_Compressor;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

var image = Image.Load<Rgb24>(@"R:\Image.jpg");
var output = ImageCompressor.Compress(image, 3f, 100, 5);
ImageCompressor.SaveToFile(output, @"R:\compressed");
ImageCompressor.Decompress(output, Math.Max(image.Width, image.Height), Math.Max(image.Width, image.Height));
//ImageCompressor.Decompress(ImageCompressor.LoadFromFile(@"R:\compressed"), Math.Max(image.Width, image.Height), Math.Max(image.Width, image.Height));


//ImageCompressor.Decompress(ImageCompressor.LoadFromFile(@"R:\compressed"), 1024, 1024);


/*File.Delete(@"R:\test.bin");
using var stream = File.OpenWrite(@"R:\test.bin");
using var binaryWriter = new BinaryWriter(stream);
using var bitBinaryWriter = new BitBinaryWriter(binaryWriter);

bitBinaryWriter.WriteBit(false);
bitBinaryWriter.WriteBit(true);
bitBinaryWriter.WriteBit(false);
bitBinaryWriter.WriteByte(127);
bitBinaryWriter.WriteByte(85);
bitBinaryWriter.WriteUInt(1337);
bitBinaryWriter.WriteInt(420);

bitBinaryWriter.Dispose();
binaryWriter.Dispose();
stream.Dispose();


Console.WriteLine();


using var readStream = File.OpenRead(@"R:\test.bin");
using var binaryReader = new BinaryReader(readStream);
var bitBinaryReader = new BitBinaryReader(binaryReader);

bitBinaryReader.ReadBit();
bitBinaryReader.ReadBit();
bitBinaryReader.ReadBit();
bitBinaryReader.ReadByte();
bitBinaryReader.ReadByte();
bitBinaryReader.ReadUInt();
bitBinaryReader.ReadInt();*/