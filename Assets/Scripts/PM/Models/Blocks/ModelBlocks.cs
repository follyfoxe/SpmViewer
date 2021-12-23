using System;
using UnityEngine;

namespace PM.Models
{
    public readonly struct Header //BlockSize => 0x1B0;
    {
        public readonly uint wHeaderSize;
        public readonly string modelName; //[64]
        public readonly string textureName; //[64]
        public readonly string buildTime; //[64]
        public readonly uint wFlags;
        public readonly uint radius;
        public readonly uint height;
        public readonly Vector3 wBboxMin;
        public readonly Vector3 wBboxMax;
        public readonly uint shapeCount;
        public readonly uint polygonCount; // size 8
        public readonly uint vertexPositionCount;
        public readonly uint vertexPositionIndexCount; // vertex position indices
        public readonly uint vertexNormalCount;
        public readonly uint vertexNormalIndexCount; // vertex normal indices
        public readonly uint vertexColorCount; // vertex colors
        public readonly uint vertexColorIndexCount; // vertex color indices
        public readonly uint[] vertexTextureCoordinateIndexCount; // texture coordinate 0 indices size 8
        public readonly uint vertexTextureCoordinateCount; // vertex texture coordinate data
        public readonly uint textureCoordinateTransformCount;
        public readonly uint samplerCount;
        public readonly uint textureCount;
        public readonly uint subshapeCount;
        public readonly uint visibilityGroupCount;
        public readonly uint groupTransformDataCount;
        public readonly uint groupCount;
        public readonly uint animCount;
        public readonly uint pShapes;
        public readonly uint pPolygons;
        public readonly uint pVertexPositions;
        public readonly uint pVertexPositionIndices;
        public readonly uint pVertexNormals;
        public readonly uint pVertexNormalIndices;
        public readonly uint pVertexColors;
        public readonly uint pVertexColorIndices;
        public readonly uint[] pVertexTextureCoordinateIndices; // vertex texture coordinate 0 indices; size 8
        public readonly uint pVertexTextureCoordinates; // size 8
        public readonly uint pTextureCoordinateTransforms;
        public readonly uint pSamplers; // size 8
        public readonly uint pTextures; // size 0x40
        public readonly uint pSubshapes; // Subshape with one material
        public readonly uint pVisibilityGroups;
        public readonly uint pGroupTransformData;
        public readonly uint pGroups;
        public readonly uint pAnims;

        public Header(BigEndianReader reader)
        {
            wHeaderSize = reader.ReadUInt32BE();
            modelName = BlockUtility.GetString(reader.ReadChars(64));
            textureName = BlockUtility.GetString(reader.ReadChars(64));
            buildTime = BlockUtility.GetString(reader.ReadChars(64));
            wFlags = reader.ReadUInt32BE();
            radius = reader.ReadUInt32BE();
            height = reader.ReadUInt32BE();

            wBboxMin = BlockUtility.ParseVector3(reader);
            wBboxMax = BlockUtility.ParseVector3(reader);

            shapeCount = reader.ReadUInt32BE();
            polygonCount = reader.ReadUInt32BE(); // size 8
            vertexPositionCount = reader.ReadUInt32BE();
            vertexPositionIndexCount = reader.ReadUInt32BE(); // vertex position indices
            vertexNormalCount = reader.ReadUInt32BE();
            vertexNormalIndexCount = reader.ReadUInt32BE(); // vertex normal indices
            vertexColorCount = reader.ReadUInt32BE(); // vertex colors
            vertexColorIndexCount = reader.ReadUInt32BE(); // vertex color indices

            vertexTextureCoordinateIndexCount = reader.ReadUInts32BE(8); // texture coordinate 0 indices

            vertexTextureCoordinateCount = reader.ReadUInt32BE(); // vertex texture coordinate data
            textureCoordinateTransformCount = reader.ReadUInt32BE();
            samplerCount = reader.ReadUInt32BE();
            textureCount = reader.ReadUInt32BE();
            subshapeCount = reader.ReadUInt32BE();
            visibilityGroupCount = reader.ReadUInt32BE();
            groupTransformDataCount = reader.ReadUInt32BE();
            groupCount = reader.ReadUInt32BE();
            animCount = reader.ReadUInt32BE();
            pShapes = reader.ReadUInt32BE();
            pPolygons = reader.ReadUInt32BE();
            pVertexPositions = reader.ReadUInt32BE();
            pVertexPositionIndices = reader.ReadUInt32BE();
            pVertexNormals = reader.ReadUInt32BE();
            pVertexNormalIndices = reader.ReadUInt32BE();
            pVertexColors = reader.ReadUInt32BE();
            pVertexColorIndices = reader.ReadUInt32BE();

            pVertexTextureCoordinateIndices = reader.ReadUInts32BE(8); // vertex texture coordinate 0 indices

            pVertexTextureCoordinates = reader.ReadUInt32BE(); // size 8
            pTextureCoordinateTransforms = reader.ReadUInt32BE();
            pSamplers = reader.ReadUInt32BE(); // size 8
            pTextures = reader.ReadUInt32BE(); // size 0x40
            pSubshapes = reader.ReadUInt32BE(); // Subshape with one material
            pVisibilityGroups = reader.ReadUInt32BE();
            pGroupTransformData = reader.ReadUInt32BE();
            pGroups = reader.ReadUInt32BE();
            pAnims = reader.ReadUInt32BE();
        }
    }

    public readonly struct Shape //BlockSize => 168;
    {
        public readonly string name;//[64];
        public readonly uint vertexPositionDataBaseIndex;
        public readonly uint vertexPositionDataCount;
        public readonly uint vertexNormalDataBaseIndex;
        public readonly uint vertexNormalDataCount;
        public readonly uint vertexColorDataBaseIndex;
        public readonly uint vertexColorDataCount;

        public readonly uint[] vertexTextureCoordinateDataBaseIndex;//8
        public readonly uint[] vertexTextureCoordinateDataCount;

        public readonly uint subshapeBaseIndex;
        public readonly uint subshapeCount;
        public readonly uint wDrawMode;
        public readonly uint cullMode;

        public Shape(BigEndianReader reader)
        {
            vertexTextureCoordinateDataBaseIndex = new uint[8];
            vertexTextureCoordinateDataCount = new uint[8];

            name = BlockUtility.GetString(reader.ReadChars(64));
            vertexPositionDataBaseIndex = reader.ReadUInt32BE();
            vertexPositionDataCount = reader.ReadUInt32BE();
            vertexNormalDataBaseIndex = reader.ReadUInt32BE();
            vertexNormalDataCount = reader.ReadUInt32BE();
            vertexColorDataBaseIndex = reader.ReadUInt32BE();
            vertexColorDataCount = reader.ReadUInt32BE();

            vertexTextureCoordinateDataBaseIndex[0] = reader.ReadUInt32BE();
            vertexTextureCoordinateDataCount[0] = reader.ReadUInt32BE();
            vertexTextureCoordinateDataBaseIndex[1] = reader.ReadUInt32BE();
            vertexTextureCoordinateDataCount[1] = reader.ReadUInt32BE();
            vertexTextureCoordinateDataBaseIndex[2] = reader.ReadUInt32BE();
            vertexTextureCoordinateDataCount[2] = reader.ReadUInt32BE();
            vertexTextureCoordinateDataBaseIndex[3] = reader.ReadUInt32BE();
            vertexTextureCoordinateDataCount[3] = reader.ReadUInt32BE();
            vertexTextureCoordinateDataBaseIndex[4] = reader.ReadUInt32BE();
            vertexTextureCoordinateDataCount[4] = reader.ReadUInt32BE();
            vertexTextureCoordinateDataBaseIndex[5] = reader.ReadUInt32BE();
            vertexTextureCoordinateDataCount[5] = reader.ReadUInt32BE();
            vertexTextureCoordinateDataBaseIndex[6] = reader.ReadUInt32BE();
            vertexTextureCoordinateDataCount[6] = reader.ReadUInt32BE();
            vertexTextureCoordinateDataBaseIndex[7] = reader.ReadUInt32BE();
            vertexTextureCoordinateDataCount[7] = reader.ReadUInt32BE();

            subshapeBaseIndex = reader.ReadUInt32BE();
            subshapeCount = reader.ReadUInt32BE();
            wDrawMode = reader.ReadUInt32BE();
            cullMode = reader.ReadUInt32BE();
        }
    }

    public readonly struct Polygon //BlockSize => 8;
    {
        public readonly uint vertexBaseIndex;
        public readonly uint vertexCount;

        public Polygon(BigEndianReader reader)
        {
            vertexBaseIndex = reader.ReadUInt32BE();
            vertexCount = reader.ReadUInt32BE();
        }
    }

    public readonly struct TexCoordTransform //BlockSize => 24;
    {
        // internal name "frameExt" short for frame extension based on debug text, can be animated
        public readonly byte textureFrameOffset; //unsigned // offset sampler texture ID
        public readonly byte[] pad_1; //unsigned, [3]
        public readonly float translationX; // can be animated
        public readonly float translationY; // can be animated
        public readonly float scaleX;
        public readonly float scaleY;
        public readonly float rotation; //These might be animated too

        public TexCoordTransform(BigEndianReader reader)
        {
            textureFrameOffset = reader.ReadByte();
            pad_1 = reader.ReadBytes(3);
            translationX = reader.ReadSingleBE();
            translationY = reader.ReadSingleBE();
            scaleX = reader.ReadSingleBE();
            scaleY = reader.ReadSingleBE();
            rotation = reader.ReadSingleBE();
        }

        public TexCoordTransform(TexCoordTransform tex, float x, float y, byte frame)
        {
            textureFrameOffset = frame;
            pad_1 = tex.pad_1;
            translationX = x;
            translationY = y;
            scaleX = tex.scaleX;
            scaleY = tex.scaleY;
            rotation = tex.rotation;
        }
    }

    public readonly struct Sampler //BlockSize => 8;
    {
        // Texture coordinate transforms are associated with samplers by index;
        // the pair shares the same respective indices, this is not just coincidence.

        public readonly uint textureBaseId; // index into type 20 data
        public readonly uint wrapFlags;
        // 0x80000000 (less than zero): use TPL wrap flags
        // 0x8: wrap T high bit (mirror) (overrides low)
        // 0x4: wrap S high bit (mirror) (overrides low)
        // 0x2: wrap T low bit (clamp/repeat)
        // 0x1: wrap S low bit (clamp/repeat)

        public Sampler(BigEndianReader reader)
        {
            textureBaseId = reader.ReadUInt32BE();
            wrapFlags = reader.ReadUInt32BE();
        }
    }

    public readonly struct Texture //BlockSize => 0x40;
    {
        public readonly uint unk_0;
        public readonly uint tplIndex; // Index of texture in associated TPL
        public readonly uint wbUnused;
        public readonly string unk_c; //[44]
        public readonly uint[] unk_38; //[2]

        public Texture(BigEndianReader reader)
        {
            unk_0 = reader.ReadUInt32BE();
            tplIndex = reader.ReadUInt32BE();
            wbUnused = reader.ReadUInt32BE();
            unk_c = BlockUtility.GetString(reader.ReadChars(44));
            unk_38 = reader.ReadUInts32BE(2);
        }
    }

    public readonly struct SubShape //BlockSize => 108;
    {
        public readonly uint samplerCount;
        public readonly uint unk_04;
        public readonly uint tevMode;
        public readonly uint unk_0c;
        public readonly int[] samplerIndices;//[8];
        public readonly byte[] samplerSourceTextureCoordinateIndices;//[8]; // 0..7
        public readonly uint polygonBaseIndex;
        public readonly uint polygonCount;

        // Absolute base index of indices
        public readonly uint vertexPositionIndicesBaseIndex;
        public readonly uint vertexNormalBaseIndicesBaseIndex;
        public readonly uint vertexColorBaseIndicesBaseIndex;
        public readonly uint[] vertexTextureCoordinateIndicesBaseIndex;//[8];

        public SubShape(BigEndianReader reader)
        {
            samplerCount = reader.ReadUInt32BE();
            unk_04 = reader.ReadUInt32BE();
            tevMode = reader.ReadUInt32BE();
            unk_0c = reader.ReadUInt32BE();
            samplerIndices = reader.ReadInts32BE(8);
            samplerSourceTextureCoordinateIndices = reader.ReadBytes(8); // 0..7
            polygonBaseIndex = reader.ReadUInt32BE();
            polygonCount = reader.ReadUInt32BE();

            // Absolute base index of indices
            vertexPositionIndicesBaseIndex = reader.ReadUInt32BE();
            vertexNormalBaseIndicesBaseIndex = reader.ReadUInt32BE();
            vertexColorBaseIndicesBaseIndex = reader.ReadUInt32BE();
            vertexTextureCoordinateIndicesBaseIndex = reader.ReadUInts32BE(8);
        }
    }

    public readonly struct VisibilityGroup //BlockSize => 1;
    {
        public readonly byte bVisible; //unsigned
        public VisibilityGroup(BigEndianReader reader) => bVisible = reader.ReadByte();
        public VisibilityGroup(byte value) => bVisible = value;
    }

    public readonly struct GroupTransform //BlockSize => 4
    {
        public readonly float transformValue;
        public GroupTransform(BigEndianReader reader) => transformValue = reader.ReadSingleBE();
        public GroupTransform(float value) => transformValue = value;
    }

    public readonly struct Group //BlockSize => 88;
    {
        public readonly string name; //[64];
        public readonly int nextGroupId;
        public readonly int childGroupId;
        public readonly int shapeId;
        public readonly uint visibilityGroupId;
        public readonly uint transformBaseIndex;
        public readonly uint bIsJoint; // if not transform, joint

        public Group(BigEndianReader reader)
        {
            name = BlockUtility.GetString(reader.ReadChars(64));
            nextGroupId = reader.ReadInt32BE();
            childGroupId = reader.ReadInt32BE();
            shapeId = reader.ReadInt32BE();
            visibilityGroupId = reader.ReadUInt32BE();
            transformBaseIndex = reader.ReadUInt32BE();
            bIsJoint = reader.ReadUInt32BE();
        }
    }
}