using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PM.Models
{
    public class PMPolygon : PMNode
    {
        public ref Polygon Source => ref Model.Model.Polygons[Index];

        public override void Create()
        {
            Name = "Polygon " + Index;
        }

        public BakedVertex[] BakeVertices(ref Shape shape, ref SubShape subShape)
        {
            ref Polygon polygon = ref Source;
            bool hasSamplers = subShape.samplerCount > 0;

            BakedVertex[] vertices = new BakedVertex[polygon.vertexCount];
            for (int i = 0; i < polygon.vertexCount; i++)
            {
                uint vertexId = shape.vertexPositionDataBaseIndex + Model.Model.VertexPositionIndices[
                    subShape.vertexPositionIndicesBaseIndex + polygon.vertexBaseIndex + i];
                uint normalId = shape.vertexNormalDataBaseIndex + Model.Model.VertexNormalIndices[
                    subShape.vertexNormalBaseIndicesBaseIndex + polygon.vertexBaseIndex + i];
                uint colorId = shape.vertexColorDataBaseIndex + Model.Model.VertexColorIndices[
                    subShape.vertexColorBaseIndicesBaseIndex + polygon.vertexBaseIndex + i];

                BakedVertex vertex = new()
                {
                    position = Model.VirtualModel.VertexPositions[vertexId],
                    normal = Model.VirtualModel.VertexNormals[normalId],
                    color = Model.Model.VertexColors[colorId]
                };

                if (hasSamplers)
                {
                    vertex.uvs = new Vector2[8];
                    for (int j = 0; j < 8; j++) //0 .. 7
                    {
                        if (shape.vertexTextureCoordinateDataCount[j] > 0)
                        {
                            uint texCoordId = shape.vertexTextureCoordinateDataBaseIndex[j] + Model.Model.VertexTexCoordIndices[j][
                            subShape.vertexTextureCoordinateIndicesBaseIndex[j] + polygon.vertexBaseIndex + i];
                            vertex.uvs[j] = Model.Model.VertexTexCoords[texCoordId];
                        }
                    }
                }
                vertices[i] = vertex;
            }
            return vertices;
        }
    }

    public struct BakedVertex
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector2[] uvs;
        public Color32 color;
    }
}