using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.ColorSpaces.Conversion;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections;

namespace Image_Compressor
{
	public class BitBinaryWriter : IDisposable
	{
		private const byte bitDepthRed = 8;
		private const byte bitDepthGreen = 8;
		private const byte bitDepthBlue = 8;

		public BitBinaryWriter(BinaryWriter binaryWriter)
		{
			this.binaryWriter = binaryWriter;

			bitArray = new BitArray(8);
		}

		private BinaryWriter binaryWriter;

		private BitArray bitArray;

		private byte bitIndex;

		private bool disposedValue;

		public long ProcessedBits;

		private void WriteBitArray()
		{
			byte[] @byte = new byte[1];
			bitArray.CopyTo(@byte, 0);
			binaryWriter.Write(@byte[0]);
			bitIndex = 0;
			bitArray.SetAll(false);
		}

		public void WriteBit(bool bit)
		{
			bitArray.Set(7 - bitIndex, bit);

			bitIndex++;

#if Debug_Writing
			Console.Write(bit ? 1 : 0);
#endif

			ProcessedBits++;

			if (bitIndex > 7)
			{
#if Debug_Writing
				Console.Write("|");
#endif
				WriteBitArray();
			}
		}

		public void WriteByte(byte @byte)
		{
			for (int i = 0; i < sizeof(byte) * 8; i++)
			{
				bool bit = (@byte & (1 << 7 - i)) != 0;
				WriteBit(bit);
			}

#if Debug_Writing_Deep
			Console.Write("{b" + @byte + "}");
#endif
		}

		public void WriteByte(byte @byte, byte bitsLimit, byte skipOffset = 0)
		{
			for (int i = skipOffset; i < (sizeof(byte) * 8) - ((sizeof(byte) * 8) - bitsLimit); i++)
			{
				bool bit = (@byte & (1 << 7 - i)) != 0;
				WriteBit(bit);
			}

#if Debug_Writing_Deep
			Console.Write("{b" + @byte + " l=" + bitsLimit + " s=" + skipOffset + "}");
#endif
		}

		public void WriteColor(Rgb24 rgb24)
		{
			WriteByte(rgb24.R, bitDepthRed);
			WriteByte(rgb24.G, bitDepthGreen);
			WriteByte(rgb24.B, bitDepthBlue);

#if Debug_Writing_Deep
			Console.Write("{c" + @rgb24.R + "_" + @rgb24.G + "_" + @rgb24.B + "_" + "}");
#endif
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					WriteBitArray();
				}

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}

	public class BitBinaryReader
	{
		private const byte bitDepthRed = 8;
		private const byte bitDepthGreen = 8;
		private const byte bitDepthBlue = 8;

		public BitBinaryReader(BinaryReader binaryReader)
		{
			this.binaryReader = binaryReader;

			FillBitArray();

			colorSpaceConverter = new();
		}

		private BinaryReader binaryReader;

		private BitArray bitArray;

		private byte bitIndex;

		public long ProcessedBits;

		private ColorSpaceConverter colorSpaceConverter;

		private void FillBitArray()
		{
			var @byte = new byte[] { binaryReader.ReadByte() };
			bitArray = new(@byte);
		}

		public bool ReadBit()
		{
			bool bit = bitArray[7 - bitIndex];

#if Debug_Reading
			Console.Write(bit ? 1 : 0);
#endif

			bitIndex++;

			ProcessedBits++;

			if (bitIndex > 7)
			{
#if Debug_Reading
				Console.Write("|");
#endif

				FillBitArray();
				bitIndex = 0;
			}

			return bit;
		}

		public byte ReadByte()
		{
			BitArray byteBits = new(sizeof(byte) * 8);

			for (int i = 0; i < byteBits.Length; i++)
			{
				byteBits[(sizeof(byte) * 8) - i - 1] = ReadBit();
			}


			byte[] @byte = new byte[1];
			byteBits.CopyTo(@byte, 0);

#if Debug_Reading_Deep
			Console.Write("{b" + @byte[0] + "}");
#endif
			return @byte[0];
		}
		
		public byte ReadByte(byte bitsReadLimit, byte skipBits = 0)
		{
			BitArray byteBits = new(sizeof(byte) * 8);

			for (int i = skipBits; i < byteBits.Length - ((sizeof(byte) * 8) - bitsReadLimit); i++)
			{
				byteBits[(sizeof(byte) * 8) - i - 1] = ReadBit();
			}


			byte[] @byte = new byte[1];
			byteBits.CopyTo(@byte, 0);

#if Debug_Reading_Deep
			Console.Write("{b" + @byte[0] + " l=" + bitsReadLimit + " s=" + skipBits + "}");
#endif
			return @byte[0];
		}

		public int ReadInt()
		{
			int @int = BitConverter.ToInt32(new byte[] { ReadByte(), ReadByte(), ReadByte(), ReadByte() });

#if Debug_Reading_Deep
			Console.Write("{i" + @int + "}");
#endif

			return @int;
		}

		public uint ReadUInt()
		{
			uint @uint = BitConverter.ToUInt32(new byte[] { ReadByte(), ReadByte(), ReadByte(), ReadByte() });

#if Debug_Reading_Deep
			Console.Write("{u" + @uint + "}");
#endif

			return @uint;
		}

		public Rgb24 ReadColor()
		{
			Rgb24 color;
			color.R = ReadByte(bitDepthRed);
			color.G = ReadByte(bitDepthGreen);
			color.B = ReadByte(bitDepthBlue);

#if Debug_Reading_Deep
			Console.Write("{c" + color.R + "_" + color.G + "_" + color.B + "_" + "}");
#endif

			return color;
		}

		public Rgb24 ReadColor(QuadrantData quadrantData)
		{
			if (quadrantData.ColorFormat is ColorFormat.HSV)
			{
				Hsv hsv = new Hsv((float)ReadByte(quadrantData.FirstColorComponentBitDepth) * (360f/255f), (float)ReadByte(quadrantData.SecondColorComponentBitDepth) * (360f / 255f), (float)ReadByte(quadrantData.ThirdColorComponentBitDepth) * (360f / 255f));

				Rgb24 color = colorSpaceConverter.ToRgb(hsv);

#if Debug_Reading_Deep
				Console.Write("{c" + color.R + "_" + color.G + "_" + color.B + "_" + "}");
#endif

				return color;
			}

			if (quadrantData.ColorFormat is ColorFormat.RGB)
			{
				Rgb24 color;
				color.R = ReadByte(quadrantData.FirstColorComponentBitDepth);
				color.G = ReadByte(quadrantData.SecondColorComponentBitDepth);
				color.B = ReadByte(quadrantData.ThirdColorComponentBitDepth);

#if Debug_Reading_Deep
				Console.Write("{c" + color.R + "_" + color.G + "_" + color.B + "_" + "}");
#endif

				return color;
			}

			return default;
		}
	}
}
