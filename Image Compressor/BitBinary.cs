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
				internalByte = (byte)(internalByte | (1 << bitIndex));

			bitIndex++;

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
			internalByte = (byte)(@byte << bitIndex);
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
					binaryWriter.Write(internalByte << bitIndex);
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
			}

			return bit;
		}

		public byte ReadByte()
		{
			BitArray byteBits = new(bitArray);
			byteBits.LeftShift(bitIndex)/*.RightShift(bitIndex)*/;

			FillBitArray();
			BitArray byteSecondPart = new(bitArray);
			byteSecondPart.LeftShift(bitIndex).RightShift(bitIndex);
			byteBits.Or(byteSecondPart);

			byte[] @byte = new byte[1];
			byteBits.CopyTo(@byte, 0);
			return @byte[0];
		}

		public int ReadInt()
		{
			int val = (ReadByte() << (8 * 3)) | (ReadByte() << (8 * 2)) | (ReadByte() << (8 * 1)) | (ReadByte() << (8 * 0));

			return val;
		}

		public uint ReadUInt()
		{
			uint val = (uint)((ReadByte() << (8 * 3)) | (ReadByte() << (8 * 2)) | (ReadByte() << (8 * 1)) | (ReadByte() << (8 * 0)));

			return val;
		}
	}
}
