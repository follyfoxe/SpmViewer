using UnityEngine;

namespace PM.Models.Animations
{
    public class DeltaCurve
    {
        public PMAnimation Animation { get; }
        public PMAData AnimData => Animation.Data;
        public PMAKeyframe[] Keyframes => AnimData.Keyframes;

        public VirtualModel[] Frames;

        VirtualModel VirtualModel;
        AccumDelta[] VirtualDeltas;

        public DeltaCurve(PMAnimation anim)
        {
            Animation = anim;
        }

        public void Bake()
        {
            VirtualModel = new VirtualModel(Animation.Model.Model);
            VirtualDeltas = new AccumDelta[] //Types of delta
            {
                new AccumDelta(k => k.VertexPositionDeltas),
                new AccumDelta(k => k.VertexNormalDeltas),
                new AccumDelta(k => k.TransformDeltas),
                new AccumDelta(k => k.VisibilityDeltas),
                new AccumDelta(k => k.TexCoordDeltas)
            };

            Frames = new VirtualModel[Keyframes.Length];

            for (int i = 0; i < Keyframes.Length; i++)
            {
                PMAKeyframe keyframe = Keyframes[i];
                for (int j = 0; j < VirtualDeltas.Length; j++)
                    ApplyDeltas(j, VirtualDeltas[j].TypeFunc(keyframe));
                Frames[i] = new VirtualModel(VirtualModel);
            }
        }

        void ApplyDeltas(int typeIndex, IPMADelta[] deltas)
        {
            VirtualDeltas[typeIndex].TotalIndex = 0;
            for (int i = 0; i < deltas.Length; i++)
                VirtualDeltas[typeIndex].ApplyDelta(ref VirtualModel, deltas[i]);
        }
    }

    public struct AccumDelta
    {
        public System.Func<PMAKeyframe, IPMADelta[]> TypeFunc;
        public int TotalIndex;

        public AccumDelta(System.Func<PMAKeyframe, IPMADelta[]> type)
        {
            TotalIndex = 0;
            TypeFunc = type;
        }

        public void ApplyDelta(ref VirtualModel model, IPMADelta delta)
        {
            TotalIndex += delta.IndexDelta;
            delta.ApplyDelta(ref this, ref model);
        }
    }
}