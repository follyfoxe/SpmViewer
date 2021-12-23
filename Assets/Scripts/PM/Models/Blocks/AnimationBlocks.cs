namespace PM.Models
{
    public readonly struct AnimationBaseInfo //BlockSize => 0xc;
    {
        public readonly uint bLoop;
        public readonly float animStart;
        public readonly float animEnd;

        public AnimationBaseInfo(BigEndianReader reader)
        {
            bLoop = reader.ReadUInt32BE();
            animStart = reader.ReadSingleBE();
            animEnd = reader.ReadSingleBE();
        }
    }

    public readonly struct AnimationKeyFrame //BlockSize => 0x2c;
    {
        public readonly float time;
        public readonly uint vertexPositionDeltaBaseIndex;
        public readonly uint vertexPositionDeltaCount;
        public readonly uint vertexNormalDeltaBaseIndex;
        public readonly uint vertexNormalDeltaCount;
        public readonly uint textureCoordinateTransformDeltaBaseIndex;
        public readonly uint textureCoordinateTransformDeltaCount;
        public readonly uint visibilityGroupDeltaBaseIndex;
        public readonly uint visibilityGroupDeltaCount;
        public readonly uint groupTransformDataDeltaBaseIndex;
        public readonly uint groupTransformDataDeltaCount;

        public AnimationKeyFrame(BigEndianReader reader)
        {
            time = reader.ReadSingleBE();
            vertexPositionDeltaBaseIndex = reader.ReadUInt32BE();
            vertexPositionDeltaCount = reader.ReadUInt32BE();
            vertexNormalDeltaBaseIndex = reader.ReadUInt32BE();
            vertexNormalDeltaCount = reader.ReadUInt32BE();
            textureCoordinateTransformDeltaBaseIndex = reader.ReadUInt32BE();
            textureCoordinateTransformDeltaCount = reader.ReadUInt32BE();
            visibilityGroupDeltaBaseIndex = reader.ReadUInt32BE();
            visibilityGroupDeltaCount = reader.ReadUInt32BE();
            groupTransformDataDeltaBaseIndex = reader.ReadUInt32BE();
            groupTransformDataDeltaCount = reader.ReadUInt32BE();
        }
    }

    public readonly struct AnimationVectorDelta //BlockSize => 0x4;
    {
        public readonly byte indexDelta; //unsigned
        public readonly sbyte coordinateDeltaX, coordinateDeltaY, coordinateDeltaZ; //[3]; signed // * 1.f/16.f

        public AnimationVectorDelta(BigEndianReader reader)
        {
            indexDelta = reader.ReadByte();
            coordinateDeltaX = reader.ReadSByte();
            coordinateDeltaY = reader.ReadSByte();
            coordinateDeltaZ = reader.ReadSByte();
        }
    }

    public readonly struct AnimationTexCoordTransformDelta //BlockSize => 12;
    {
        public readonly byte indexDelta; //unsigned
        public readonly sbyte wFrameExtDelta; //signed
        public readonly byte[] pad_2; //[2] unsigned
        public readonly float translateXDelta;
        public readonly float translateYDelta;

        public AnimationTexCoordTransformDelta(BigEndianReader reader)
        {
            indexDelta = reader.ReadByte();
            wFrameExtDelta = reader.ReadSByte();
            pad_2 = reader.ReadBytes(2);
            translateXDelta = reader.ReadSingleBE();
            translateYDelta = reader.ReadSingleBE();
        }
    }

    public readonly struct AnimationGroupTransformDelta //BlockSize => 0x4;
    {
        public readonly byte indexDelta; //unsigned
        public readonly sbyte valueDelta; //not specified?
        public readonly sbyte tangentInDeg;
        public readonly sbyte tangentOutDeg;

        public AnimationGroupTransformDelta(BigEndianReader reader)
        {
            indexDelta = reader.ReadByte();
            valueDelta = reader.ReadSByte();
            tangentInDeg = reader.ReadSByte();
            tangentOutDeg = reader.ReadSByte();
        }
    }

    public readonly struct AnimationGroupVisibility //BlockSize => 2;
    {
        public readonly byte visibilityGroupId; //unsigned
        public readonly sbyte bVisible; //unsigned

        public AnimationGroupVisibility(BigEndianReader reader)
        {
            visibilityGroupId = reader.ReadByte();
            bVisible = reader.ReadSByte();
        }
    }

    public readonly struct AnimationData //BlockSize => 0x5c;
    {
        #region Data
        public readonly uint dataSize;

        public readonly uint baseInfoCount; // element size 0xc
        public readonly uint keyframeCount; // 0x2c
        public readonly uint vertexPositionDeltaCount; // 0x4
        public readonly uint vertexNormalDeltaCount; // 0x4
        public readonly uint textureCoordinateTransformDeltaCount;
        public readonly uint visibilityGroupDeltaCount;
        public readonly uint groupTransformDataDeltaCount; // 0x4
        public readonly uint wAnimDataType8Count; // if animation data is tightly packed, 0x8

        public readonly uint pBaseInfo;
        public readonly uint pKeyframes;
        public readonly uint pVertexPositionDeltas;
        public readonly uint pVertexNormalDeltas;
        public readonly uint pTextureCoordinateTransformDeltas; // texture coordinate animations
        public readonly uint pVisibilityGroupDeltas;
        public readonly uint pGroupTransformDataDeltas; // group transform animations
        public readonly uint wpAnimDataType8Data;

        public readonly float[] unk_44;//[6];
        #endregion

        public readonly uint BasePos;
        public readonly AnimationBaseInfo[] BaseInfos;
        public readonly AnimationKeyFrame[] KeyFrames;

        public readonly AnimationVectorDelta[] VertexPositionDeltas;
        public readonly AnimationVectorDelta[] VertexNormalDeltas;
        public readonly AnimationTexCoordTransformDelta[] TexCoordTransformDeltas;
        public readonly AnimationGroupVisibility[] GroupVisibilityDeltas;
        public readonly AnimationGroupTransformDelta[] GroupTransformDeltas;

        public AnimationData(BigEndianReader reader) : this()
        {
            BasePos = (uint)reader.BaseStream.Position;

            dataSize = reader.ReadUInt32BE();

            baseInfoCount = reader.ReadUInt32BE();
            keyframeCount = reader.ReadUInt32BE();
            vertexPositionDeltaCount = reader.ReadUInt32BE();
            vertexNormalDeltaCount = reader.ReadUInt32BE();
            textureCoordinateTransformDeltaCount = reader.ReadUInt32BE();
            visibilityGroupDeltaCount = reader.ReadUInt32BE();
            groupTransformDataDeltaCount = reader.ReadUInt32BE();
            wAnimDataType8Count = reader.ReadUInt32BE();

            pBaseInfo = reader.ReadUInt32BE();
            pKeyframes = reader.ReadUInt32BE();
            pVertexPositionDeltas = reader.ReadUInt32BE();
            pVertexNormalDeltas = reader.ReadUInt32BE();
            pTextureCoordinateTransformDeltas = reader.ReadUInt32BE();
            pVisibilityGroupDeltas = reader.ReadUInt32BE();
            pGroupTransformDataDeltas = reader.ReadUInt32BE();
            wpAnimDataType8Data = reader.ReadUInt32BE();

            unk_44 = new float[6]
            {
                reader.ReadSingleBE(), reader.ReadSingleBE(), reader.ReadSingleBE(),
                reader.ReadSingleBE(), reader.ReadSingleBE(), reader.ReadSingleBE()
            };

            long pos = reader.BaseStream.Position;

            #region Blocks
            BaseInfos = BlockUtility.ParseAll<AnimationBaseInfo>(reader, BasePos + pBaseInfo, baseInfoCount);
            KeyFrames = BlockUtility.ParseAll<AnimationKeyFrame>(reader, BasePos + pKeyframes, keyframeCount);

            if (vertexPositionDeltaCount > 0)
                VertexPositionDeltas = BlockUtility.ParseAll<AnimationVectorDelta>(reader, BasePos + pVertexPositionDeltas, vertexPositionDeltaCount);
            if (vertexNormalDeltaCount > 0)
                VertexNormalDeltas = BlockUtility.ParseAll<AnimationVectorDelta>(reader, BasePos + pVertexNormalDeltas, vertexNormalDeltaCount);
            if (textureCoordinateTransformDeltaCount > 0)
                TexCoordTransformDeltas = BlockUtility.ParseAll<AnimationTexCoordTransformDelta>(reader, BasePos + pTextureCoordinateTransformDeltas, textureCoordinateTransformDeltaCount);
            if (visibilityGroupDeltaCount > 0)
                GroupVisibilityDeltas = BlockUtility.ParseAll<AnimationGroupVisibility>(reader, BasePos + pVisibilityGroupDeltas, visibilityGroupDeltaCount);
            if (groupTransformDataDeltaCount > 0)
                GroupTransformDeltas = BlockUtility.ParseAll<AnimationGroupTransformDelta>(reader, BasePos + pGroupTransformDataDeltas, groupTransformDataDeltaCount);
            #endregion

            reader.BaseStream.Position = pos;
        }
    }

    public struct Animation //BlockSize = 16 + 4 + PaddingSize;
    {
        const int PaddingSize = 0x40 - 16 - 4;

        public readonly string name;//[16];
        // unknown how long the preceding char array is.
        // I haven't seen any other usage of data in this area,
        // so it may very well just span 60 characters.
        public readonly byte[] padding;//[0x40 - 16 - 4]; not relevant
        public readonly uint dataOffset;

        public AnimationData Data;

        public Animation(BigEndianReader reader)
        {
            name = BlockUtility.GetString(reader.ReadChars(16));
            padding = reader.ReadBytes(PaddingSize);
            dataOffset = reader.ReadUInt32BE();

            long pos = reader.BaseStream.Position;

            reader.BaseStream.Position = dataOffset;
            Data = new AnimationData(reader);

            reader.BaseStream.Position = pos;
        }
    }
}