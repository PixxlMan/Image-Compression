using SixLabors.ImageSharp.PixelFormats;
using System.Collections;

namespace Image_Compressor
{
	public class BitBinaryWriter : IDisposable
	{
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

		public void WriteColor(Rgb24 rgb24)
		{
			WriteByte(rgb24.R);
			WriteByte(rgb24.G);
			WriteByte(rgb24.B);

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
		public BitBinaryReader(BinaryReader binaryReader)
		{
			this.binaryReader = binaryReader;

			FillBitArray();
		}

		private BinaryReader binaryReader;

		private BitArray bitArray;

		private byte bitIndex;

		public long ProcessedBits;

		private void FillBitArray()
		{
			if (binaryReader.BaseStream.Position == binaryReader.BaseStream.Length)
				return;

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

		public byte ReadByte(/*byte readLimit (for custom data sizes!)*/)
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
			color.R = ReadByte();
			color.G = ReadByte();
			color.B = ReadByte();

#if Debug_Reading_Deep
			Console.Write("{c" + color.R + "_" + color.G + "_" + color.B + "_" + "}");
#endif

			return color;
		}
	}
}
