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

		private void WriteBitArray()
		{
			byte[] @byte = new byte[1];
			bitArray.CopyTo(@byte, 0);
			binaryWriter.Write(@byte[0]);
			bitIndex = 0;
		}


		public void WriteBit(bool bit)
		{
			bitArray.Set(bitIndex, bit);

			bitIndex++;

			Console.Write(bit ? 1 : 0);

			if (bitIndex > 7)
			{
				Console.Write("|");
				WriteBitArray();
			}
		}

		public void WriteByte(byte @byte)
		{
			for (int i = 0; i < sizeof(byte) * 8; i++)
			{
				bool bit = (@byte & (1 << i)) != 0;
				WriteBit(bit);
			}
		}

		public void WriteInt(int @int)
		{
			for (int i = 0; i < sizeof(int) * 8; i++)
			{
				bool bit = (@int & (1 << i)) != 0;
				WriteBit(bit);
			}
		}

		public void WriteUInt(uint @uint)
		{
			for (int i = 0; i < sizeof(uint) * 8; i++)
			{
				bool bit = (@uint & (1 << i)) != 0;
				WriteBit(bit);
			}
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

		private void FillBitArray()
		{
			var @byte = new byte[] { binaryReader.ReadByte() };
			bitArray = new(@byte);
		}

		public bool ReadBit()
		{
			bool bit = bitArray[7 - bitIndex];

			Console.Write(bit ? 1 : 0);

			bitIndex++;

			if (bitIndex > 7)
			{
				Console.Write("|");
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
			//Console.Write("{b" + @byte[0] + "}");
			return @byte[0];
		}

		public int ReadInt()
		{
			int @int = BitConverter.ToInt32(new byte[] { ReadByte(), ReadByte(), ReadByte(), ReadByte() });
			
			//Console.Write("{i" + @int + "}");

			return @int;
		}

		public uint ReadUInt()
		{
			uint @uint = BitConverter.ToUInt32(new byte[] { ReadByte(), ReadByte(), ReadByte(), ReadByte() });

			//Console.Write("{u" + @uint + "}");

			return @uint;
		}
	}
}
