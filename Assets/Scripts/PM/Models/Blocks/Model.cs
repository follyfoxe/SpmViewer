using System.Text;
using System.IO;
using UnityEngine;

namespace PM.Models
{
    public class Model
    {
        public readonly Header Header;

        public readonly Shape[] Shapes;
        public readonly Polygon[] Polygons;

        public readonly Vector3[] VertexPositions;
        public readonly uint[] VertexPositionIndices;
        public readonly Vector3[] VertexNormals;
        public readonly uint[] VertexNormalIndices;
        public readonly Color32[] VertexColors;
        public readonly uint[] VertexColorIndices;

        public readonly uint[][] VertexTexCoordIndices;

        public readonly Vector2[] VertexTexCoords;
        public readonly TexCoordTransform[] TexCoordTransforms;
        public readonly Sampler[] Samplers;
        public readonly Texture[] Textures;
        public readonly SubShape[] SubShapes;
        public readonly VisibilityGroup[] VisibilityGroups;
        public readonly GroupTransform[] GroupTransforms;
        public readonly Group[] Groups;

        public readonly Animation[] Animations;

        public Model(Stream stream)
        {
            BigEndianReader reader = new(stream, Encoding.UTF8);

            Header = new(reader);

            Shapes = BlockUtility.ParseAll<Shape>(reader, Header.pShapes, Header.shapeCount);
            Polygons = BlockUtility.ParseAll<Polygon>(reader, Header.pPolygons, Header.polygonCount);

            VertexPositions = BlockUtility.ParseAll(BlockUtility.ParseVector3, reader, Header.pVertexPositions, Header.vertexPositionCount);
            VertexPositionIndices = BlockUtility.ParseAll(r => r.ReadUInt32BE(), reader, Header.pVertexPositionIndices, Header.vertexPositionIndexCount);
            VertexNormals = BlockUtility.ParseAll(BlockUtility.ParseVector3, reader, Header.pVertexNormals, Header.vertexNormalCount);
            VertexNormalIndices = BlockUtility.ParseAll(r => r.ReadUInt32BE(), reader, Header.pVertexNormalIndices, Header.vertexNormalIndexCount);
            VertexColors = BlockUtility.ParseAll(BlockUtility.ParseColor32, reader, Header.pVertexColors, Header.vertexColorCount);
            VertexColorIndices = BlockUtility.ParseAll(r => r.ReadUInt32BE(), reader, Header.pVertexColorIndices, Header.vertexColorIndexCount);

            VertexTexCoordIndices = new uint[8][];
            for (int i = 0; i < 8; i++)
                VertexTexCoordIndices[i] = BlockUtility.ParseAll(r => r.ReadUInt32BE(), reader, Header.pVertexTextureCoordinateIndices[i], Header.vertexTextureCoordinateIndexCount[i]);

            VertexTexCoords = BlockUtility.ParseAll(BlockUtility.ParseVector2, reader, Header.pVertexTextureCoordinates, Header.vertexTextureCoordinateCount);
            TexCoordTransforms = BlockUtility.ParseAll<TexCoordTransform>(reader, Header.pTextureCoordinateTransforms, Header.textureCoordinateTransformCount);
            Samplers = BlockUtility.ParseAll<Sampler>(reader, Header.pSamplers, Header.samplerCount);
            Textures = BlockUtility.ParseAll<Texture>(reader, Header.pTextures, Header.textureCount);
            SubShapes = BlockUtility.ParseAll<SubShape>(reader, Header.pSubshapes, Header.subshapeCount);
            VisibilityGroups = BlockUtility.ParseAll<VisibilityGroup>(reader, Header.pVisibilityGroups, Header.visibilityGroupCount);
            GroupTransforms = BlockUtility.ParseAll<GroupTransform>(reader, Header.pGroupTransformData, Header.groupTransformDataCount);
            Groups = BlockUtility.ParseAll<Group>(reader, Header.pGroups, Header.groupCount);

            Animations = BlockUtility.ParseAll<Animation>(reader, Header.pAnims, Header.animCount);

            reader.Dispose();
        }
    }
}