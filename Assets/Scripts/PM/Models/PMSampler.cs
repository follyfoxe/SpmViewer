using UnityEngine;

namespace PM.Models
{
    public class PMSampler : PMNode
    {
        public ref Sampler Source => ref Model.Model.Samplers[Index];

        public override void Create()
        {
            ref Sampler sampler = ref Source;
            Name = "Sampler " + Index;
        }

        public Material CreateMaterial(ref Shape shape)
        {
            Shader shader = Model.PMCutoutShader;
            if (shape.wDrawMode == 0x03) //blend
                shader = Model.PMBlendShader;

            Material mat = new(shader);
            mat.name = Name;
            UpdateMaterial(mat);
            return mat;
        }

        public void UpdateMaterial(Material material)
        {
            ref TexCoordTransform tex = ref Model.VirtualModel.TexCoordTransforms[Index];

            material.mainTextureOffset = new Vector2(tex.translationX, tex.translationY);
            material.mainTexture = Model.Textures[Source.textureBaseId + tex.textureFrameOffset];
        }
    }
}