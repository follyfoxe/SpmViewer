using UnityEngine;

namespace PM.Models
{
    public class PMSubShapeInstance : PMNode
    {
        public MeshFilter MeshFilter;
        public MeshRenderer MeshRenderer;

        public PMSampler Sampler;
        public PMShape Shape;
        public PMSubShape SubShape;
        public Mesh Mesh;
        public int[] PolygonOffsets;

        public override void Create() { }

        public void Initialize(PMShape shape, PMSubShape subShape)
        {
            Shape = shape;
            SubShape = subShape;
            ref Shape s = ref shape.Source;

            Name = subShape.Name + " (Instance)";

            MeshFilter = gameObject.AddComponent<MeshFilter>();
            Mesh = subShape.BuildMesh(out PolygonOffsets, ref s);
            MeshFilter.sharedMesh = Mesh;

            MeshRenderer = gameObject.AddComponent<MeshRenderer>();
            Sampler = subShape.GetSampler();
            if (Sampler)
                MeshRenderer.sharedMaterial = Sampler.CreateMaterial(ref s);
            else
                MeshRenderer.sharedMaterial = Model.PMVertexMaterial;
        }

        public void UpdateMesh()
        {
            SubShape.UpdateMesh(Mesh, PolygonOffsets, ref Shape.Source);
        }

        public void UpdateMaterial()
        {
            if (Sampler)
                Sampler.UpdateMaterial(MeshRenderer.sharedMaterial);
        }
    }
}