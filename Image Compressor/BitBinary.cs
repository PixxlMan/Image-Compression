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
			bitIndex = 0;
			binaryWriter.Write(@byte);
		}


		public void WriteBit(bool bit)
		{
			bitArray.Set(bitIndex, bit);

			bitIndex++;

			if (bitIndex > 7)
			{
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

			bitIndex++;

			if (bitIndex > 7)
			{
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
			return @byte[0];
		}

		public int ReadInt()
		{
			BitArray intBits = new(sizeof(int) * 8);

			for (int i = 0; i < intBits.Length; i++)
			{
				intBits[(sizeof(int) * 8) - i - 1] = ReadBit();
			}

			int[] @int = new int[1];
			intBits.CopyTo(@int, 0);
			return @int[0];
		}

		public uint ReadUInt()
		{
			BitArray uintBits = new(sizeof(uint) * 8);

			for (int i = 0; i < uintBits.Length; i++)
			{
				uintBits[(sizeof(uint) * 8) - i - 1] = ReadBit();
			}

			uint[] @int = new uint[1];
			uintBits.CopyTo(@int, 0);
			return @int[0];
		}
	}
}
