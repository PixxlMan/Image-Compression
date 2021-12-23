namespace Image_Compressor;

public enum ColorFormat : byte
{
	RGB,
	HSV,

	_BitSize = 1,
}

public struct QuadrantData : IEquatable<QuadrantData>
{
	public byte FirstColorComponentBitDepth;
	public byte SecondColorComponentBitDepth;
	public byte ThirdColorComponentBitDepth;
	public ColorFormat ColorFormat;

	public QuadrantData(byte firstColorComponentBitDepth, byte secondColorComponentBitDepth, byte thirdColorComponentBitDepth, ColorFormat colorFormat)
	{
		FirstColorComponentBitDepth = firstColorComponentBitDepth;
		SecondColorComponentBitDepth = secondColorComponentBitDepth;
		ThirdColorComponentBitDepth = thirdColorComponentBitDepth;
		ColorFormat = colorFormat;
	}

	public static QuadrantData Read(BitBinaryReader bitBinaryReader, QuadrantData previousData)
	{
		QuadrantData constantData = previousData;

		if (!bitBinaryReader.ReadBit())
			return constantData;

		if (bitBinaryReader.ReadBit())
			constantData.FirstColorComponentBitDepth = bitBinaryReader.ReadByte();
		if (bitBinaryReader.ReadBit())
			constantData.SecondColorComponentBitDepth = bitBinaryReader.ReadByte();
		if (bitBinaryReader.ReadBit())
			constantData.ThirdColorComponentBitDepth = bitBinaryReader.ReadByte();

		constantData.ColorFormat = (ColorFormat)bitBinaryReader.ReadByte((byte)ColorFormat._BitSize, 8 - (byte)ColorFormat._BitSize);

		return constantData;
	}

	public void Write(BitBinaryWriter bitBinaryWriter, QuadrantData previousData)
	{
		if (Equals(previousData))
		{
			bitBinaryWriter.WriteBit(false);
			return;
		}
		else
			bitBinaryWriter.WriteBit(true);

		if (previousData.FirstColorComponentBitDepth != FirstColorComponentBitDepth)
		{
			bitBinaryWriter.WriteBit(true);

			bitBinaryWriter.WriteByte(FirstColorComponentBitDepth);
		}
		else
			bitBinaryWriter.WriteBit(false);

		if (previousData.SecondColorComponentBitDepth != SecondColorComponentBitDepth)
		{
			bitBinaryWriter.WriteBit(true);

			bitBinaryWriter.WriteByte(SecondColorComponentBitDepth);
		}
		else
			bitBinaryWriter.WriteBit(false);

		if (previousData.ThirdColorComponentBitDepth != ThirdColorComponentBitDepth)
		{
			bitBinaryWriter.WriteBit(true);

			bitBinaryWriter.WriteByte(ThirdColorComponentBitDepth);
		}
		else
			bitBinaryWriter.WriteBit(false);

		bitBinaryWriter.WriteByte((byte)ColorFormat, 8, 8 - (byte)ColorFormat._BitSize);
	}

	public bool Equals(QuadrantData other)
	{
		return other.FirstColorComponentBitDepth == FirstColorComponentBitDepth && other.SecondColorComponentBitDepth == SecondColorComponentBitDepth && other.ThirdColorComponentBitDepth == ThirdColorComponentBitDepth && other.ColorFormat == ColorFormat;
	}
}
