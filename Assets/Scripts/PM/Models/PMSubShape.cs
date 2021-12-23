using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PM.Models
{
    public class PMSubShape : PMNode
    {
        public ref SubShape Source => ref Model.Model.SubShapes[Index];

        public PMSampler[] Samplers;
        public PMPolygon[] Polygons;

        public bool HasSamplers => Samplers.Length > 0;
        public int TexCoordIndex;

        public override void Create()
        {
            ref SubShape subShape = ref Source;
            Name = "SubShape " + Index;

            Samplers = new PMSampler[subShape.samplerCount];
            for (int i = 0; i < Samplers.Length; i++)
                Samplers[i] = Model.Samplers[subShape.samplerIndices[TexCoordIndex] + i];

            Polygons = new PMPolygon[subShape.polygonCount];
            for (int i = 0; i < Polygons.Length; i++)
                Polygons[i] = Model.Polygons[subShape.polygonBaseIndex + i];
        }

        public int GetSamplerIndex()
        {
            ref SubShape subShape = ref Source;
            return subShape.samplerSourceTextureCoordinateIndices[TexCoordIndex];
        }

        public PMSampler GetSampler()
        {
            int ind = GetSamplerIndex();
            if (ind < 0 || !HasSamplers) return null;
            return Samplers[ind];
        }

        public void UpdateMesh(Mesh mesh, int[] polygonOffsets, ref Shape shape)
        {
            ref SubShape subShape = ref Source;
            List<Vector3> positions = new();
            List<Vector3> normals = new();
            List<Color32> colors = new();

            mesh.GetVertices(positions);
            mesh.GetNormals(normals);
            mesh.GetColors(colors);

            for (int i = 0; i < Polygons.Length; i++)
            {
                PMPolygon polygon = Polygons[i];
                int polygonOffset = polygonOffsets[i];
                BakedVertex[] verts = polygon.BakeVertices(ref shape, ref subShape);

                for (int j = 0; j < verts.Length; j++)
                {
                    BakedVertex vertex = verts[j];
                    int pos = polygonOffset + j;
                    positions[pos] = vertex.position;
                    normals[pos] = vertex.normal;
                    colors[pos] = vertex.color;
                }
            }

            mesh.SetVertices(positions);
            mesh.SetNormals(normals);
            mesh.SetColors(colors);
            mesh.RecalculateBounds();
        }

        public Mesh BuildMesh(out int[] polygonOffsets, ref Shape shape)
        {
            Mesh mesh = new() { name = Name };
            MergedBakedVertices merged = BakeVertices(ref shape);
            int vertexCount = merged.Vertices.Count;
            bool hasSamplers = HasSamplers;

            Vector3[] positions = new Vector3[vertexCount];
            Vector3[] normals = new Vector3[vertexCount];
            Color32[] colors = new Color32[vertexCount];
            Vector2[][] uvs = new Vector2[8][];
            for (int i = 0; i < 8; i++)
                uvs[i] = new Vector2[vertexCount];

            for (int i = 0; i < vertexCount; i++)
            {
                BakedVertex vertex = merged.Vertices[i];
                positions[i] = vertex.position;
                normals[i] = vertex.normal;
                colors[i] = vertex.color;

                if (hasSamplers)
                {
                    for (int j = 0; j < 8; j++)
                        uvs[j][i] = vertex.uvs[j];
                }
            }

            mesh.SetVertices(positions);
            mesh.SetNormals(normals);
            mesh.SetColors(colors);
            mesh.SetTriangles(merged.Triangles, 0);

            if (hasSamplers)
            {
                for (int i = 0; i < 8; i++)
                    mesh.SetUVs(i, uvs[i]);
            }

            mesh.RecalculateBounds();
            polygonOffsets = merged.PolygonOffsets;
            return mesh;
        }

        public MergedBakedVertices BakeVertices(ref Shape shape)
        {
            ref SubShape subShape = ref Source;

            MergedBakedVertices merged = new();
            merged.PolygonOffsets = new int[Polygons.Length];
            merged.Vertices = new();
            merged.Triangles = new();

            for (int i = 0; i < Polygons.Length; i++)
            {
                PMPolygon polygon = Polygons[i];
                int polygonOffset = merged.Vertices.Count;
                merged.PolygonOffsets[i] = polygonOffset;

                BakedVertex[] verts = polygon.BakeVertices(ref shape, ref subShape);
                merged.Vertices.AddRange(verts);

                foreach (int tri in TriangleFan(verts.Length))
                    merged.Triangles.Add(tri + polygonOffset);
            }

            return merged;
        }

        List<int> TriangleFan(int vertexCount)
        {
            List<int> tris = new();
            if (vertexCount >= 3)
            {
                for (int i = 2; i < vertexCount; i++)
                {
                    tris.Add(0);
                    tris.Add(i - 1);
                    tris.Add(i);
                }
            }
            return tris;
        }
    }

    public struct MergedBakedVertices
    {
        public List<BakedVertex> Vertices;
        public List<int> Triangles;
        public int[] PolygonOffsets; //Offset into the vertices region where it was merged
    }
}