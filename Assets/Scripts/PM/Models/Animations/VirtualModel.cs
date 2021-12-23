using UnityEngine;

namespace PM.Models.Animations
{
    public struct VirtualModel
    {
        public Vector3[] VertexPositions;
        public Vector3[] VertexNormals;
        public GroupTransform[] GroupTransforms;
        public VisibilityGroup[] VisibilityGroups;
        public TexCoordTransform[] TexCoordTransforms;
        
        public VirtualModel(Model model)
        {
            VertexPositions = (Vector3[])model.VertexPositions.Clone();
            VertexNormals = (Vector3[])model.VertexNormals.Clone();
            GroupTransforms = (GroupTransform[])model.GroupTransforms.Clone();
            VisibilityGroups = (VisibilityGroup[])model.VisibilityGroups.Clone();
            TexCoordTransforms = (TexCoordTransform[])model.TexCoordTransforms.Clone();
        }

        public VirtualModel(VirtualModel model)
        {
            VertexPositions = (Vector3[])model.VertexPositions.Clone();
            VertexNormals = (Vector3[])model.VertexNormals.Clone();
            GroupTransforms = (GroupTransform[])model.GroupTransforms.Clone();
            VisibilityGroups = (VisibilityGroup[])model.VisibilityGroups.Clone();
            TexCoordTransforms = (TexCoordTransform[])model.TexCoordTransforms.Clone();
        }
    }

    public class Vector3Curve
    {
        public AnimationCurve x, y, z;

        public Vector3Curve()
        {
            x = new AnimationCurve();
            y = new AnimationCurve();
            z = new AnimationCurve();
        }

        public void AddKey(float time, Vector3 value)
        {
            x.AddKey(time, value.x);
            y.AddKey(time, value.y);
            z.AddKey(time, value.z);
        }

        public void AddKey(Keyframe xkey, Keyframe ykey, Keyframe zkey)
        {
            x.AddKey(xkey);
            y.AddKey(ykey);
            z.AddKey(zkey);
        }

        public Vector3 Evaluate(float time)
        {
            return new Vector3(x.Evaluate(time), y.Evaluate(time), z.Evaluate(time));
        }
    }

    public class Vector4Curve
    {
        public AnimationCurve x, y, z, w;

        public Vector4Curve()
        {
            x = new AnimationCurve();
            y = new AnimationCurve();
            z = new AnimationCurve();
            w = new AnimationCurve();
        }

        public void AddKey(float time, Vector4 value)
        {
            x.AddKey(time, value.x);
            y.AddKey(time, value.y);
            z.AddKey(time, value.z);
            w.AddKey(time, value.w);
        }

        public Vector4 Evaluate(float time)
        {
            return new Vector4(x.Evaluate(time), y.Evaluate(time), z.Evaluate(time), w.Evaluate(time));
        }
    }

    public struct ModelAnimCurves
    {
        public Vector3Curve[] VertexPositions;
        public Vector3Curve[] VertexNormals;
        public AnimationCurve[] GroupTransforms;
        public AnimationCurve[] VisibilityGroups;
        public Vector3Curve[] TexCoordTransforms;

        public ModelAnimCurves(PMAKeyframe[] keyframes, VirtualModel[] modelFrames)
        {
            VirtualModel main = modelFrames[0];

            VertexPositions = CreateCurve<Vector3Curve>(keyframes, modelFrames, main.VertexPositions.Length,
                (c, i, t, m) => c.AddKey(t, m.VertexPositions[i]));

            VertexNormals = CreateCurve<Vector3Curve>(keyframes, modelFrames, main.VertexNormals.Length,
                (c, i, t, m) => c.AddKey(t, m.VertexNormals[i]));

            GroupTransforms = CreateCurve<AnimationCurve>(keyframes, modelFrames, main.GroupTransforms.Length,
                (c, i, t, m) => c.AddKey(t, m.GroupTransforms[i].transformValue));

            VisibilityGroups = CreateCurve<AnimationCurve>(keyframes, modelFrames, main.VisibilityGroups.Length,
                (c, i, t, m) => c.AddKey(new Keyframe(t, m.VisibilityGroups[i].bVisible, float.NegativeInfinity, float.NegativeInfinity)));

            TexCoordTransforms = CreateCurve<Vector3Curve>(keyframes, modelFrames, main.TexCoordTransforms.Length,
                (c, i, t, m) =>
                {
                    var tex = m.TexCoordTransforms[i];
                    c.AddKey(new Keyframe(t, tex.translationX), new Keyframe(t, tex.translationY),
                        new Keyframe(t, tex.textureFrameOffset, float.NegativeInfinity, float.NegativeInfinity));
                });
        }

        static TCurve[] CreateCurve<TCurve>(PMAKeyframe[] keyframes, VirtualModel[] modelFrames, 
            int curveCount, System.Action<TCurve, int, float, VirtualModel> addKey) where TCurve : new()
        {
            TCurve[] curves = new TCurve[curveCount];
            for (int i = 0; i < curveCount; i++)
            {
                TCurve curve = new();
                for (int j = 0; j < keyframes.Length; j++)
                    addKey(curve, i, keyframes[j].Time, modelFrames[j]);
                curves[i] = curve;
            }
            return curves;
        }

        public void Evaluate(PMModel model, float time)
        {
            for (int i = 0; i < VertexPositions.Length; i++)
                model.VirtualModel.VertexPositions[i] = VertexPositions[i].Evaluate(time);
            for (int i = 0; i < VertexNormals.Length; i++)
                model.VirtualModel.VertexNormals[i] = VertexNormals[i].Evaluate(time);

            for (int i = 0; i < GroupTransforms.Length; i++)
                model.VirtualModel.GroupTransforms[i] = new GroupTransform(GroupTransforms[i].Evaluate(time));
            for (int i = 0; i < VisibilityGroups.Length; i++)
                model.VirtualModel.VisibilityGroups[i] = new VisibilityGroup((byte)Mathf.RoundToInt(VisibilityGroups[i].Evaluate(time)));
            for (int i = 0; i < TexCoordTransforms.Length; i++)
            {
                Vector3 eval = TexCoordTransforms[i].Evaluate(time);
                model.VirtualModel.TexCoordTransforms[i] = new TexCoordTransform(model.VirtualModel.TexCoordTransforms[i],
                    eval.x, eval.y, (byte)Mathf.RoundToInt(eval.z));
            }
            model.UpdateVisual();
        }
    }
}