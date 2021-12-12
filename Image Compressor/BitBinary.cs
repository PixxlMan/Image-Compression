using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Compressor
{
	public class BitBinaryWriter : IDisposable
	{
		public BitBinaryWriter(BinaryWriter binaryWriter)
		{
			this.binaryWriter = binaryWriter;
		}

		private BinaryWriter binaryWriter;

		private byte internalByte;

		private byte bitIndex;

		private bool disposedValue;

		public void WriteBit(bool bit)
		{
			if (bit)
			{
				internalByte = (byte)(internalByte | (1 << bitIndex));

				bitIndex++;
			}

			if (bitIndex > 7)
			{
				binaryWriter.Write(internalByte);
				internalByte = 0;
				bitIndex = 0;
			}
		}
		
		public void WriteByte(byte @byte)
		{
			//byte rem = (byte)(@byte >> bitIndex);
			binaryWriter.Write((byte)(internalByte | /*rem*/ (@byte >> bitIndex)));
			internalByte = 0;
		}

		public void WriteInt(int @int)
		{
			int val = internalByte | @int >> bitIndex;

			binaryWriter.Write(val);

			unchecked
			{
				internalByte = (byte)@int;
			}
		}

		public void WriteUInt(uint @uint)
		{
			uint val = internalByte | @uint >> bitIndex;

			binaryWriter.Write(val);

			unchecked
			{
				internalByte = (byte)@uint;
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					binaryWriter.Write(internalByte);
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

			internalByte = binaryReader.ReadByte();
		}

		private BinaryReader binaryReader;

		private byte internalByte;

		private byte bitIndex;

		public bool ReadBit()
		{
			bool bit = (internalByte & (1 << bitIndex)) != 0;

			bitIndex++;

			if (bitIndex > 7)
			{
				internalByte = binaryReader.ReadByte();
				bitIndex = 0;
			}

			return bit;
		}

		public byte ReadByte()
		{
			byte val = (byte)(internalByte << bitIndex);

			internalByte = binaryReader.ReadByte();

			byte mask = (byte)(0b_1111_1111 << bitIndex);

			val = (byte)((val & mask) | ((~mask) & internalByte));

			return val;
		}

		public int ReadInt()
		{
			int val = (internalByte << bitIndex) << ((8 * 3) + bitIndex);
			val = val | (binaryReader.ReadByte() << ((8 * 2) + bitIndex));
			val = val | (binaryReader.ReadByte() << ((8 * 1) + bitIndex));
			val = val | (binaryReader.ReadByte() << bitIndex);

			internalByte = binaryReader.ReadByte();

			val = val | ((internalByte << bitIndex) >> bitIndex);

			return val;
		}

		public uint ReadUInt()
		{
			uint val = (uint)((internalByte << bitIndex) << ((8 * 3) + bitIndex));
			val = (uint)(val | (binaryReader.ReadByte() << ((8 * 2) + bitIndex)));
			val = (uint)(val | (binaryReader.ReadByte() << ((8 * 1) + bitIndex)));
			val = (uint)(val | (binaryReader.ReadByte() << bitIndex));

			internalByte = binaryReader.ReadByte();

			val = (uint)(val | ((internalByte << bitIndex) >> bitIndex));

			return val;
		}
	}
}
