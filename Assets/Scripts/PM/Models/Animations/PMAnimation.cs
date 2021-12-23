using UnityEngine;

namespace PM.Models.Animations
{
    public class PMAnimation : PMNode
    {
        public ref Animation Source => ref Model.Model.Animations[Index];
        public PMAData Data;

        public bool Loop;

        public override void Create()
        {
            ref Animation anim = ref Source;
            Name = anim.name;

            Data = new PMAData(this, 0);
            Loop = BlockUtility.GetBool(Data.BaseInfos[0].Source.bLoop);
        }

        public float GetDuration()
        {
            return Data.Keyframes[Data.Keyframes.Length - 1].Time;
        }
    }

    public class PMAData : PMANode
    {
        public ref AnimationData Source => ref Animation.Source.Data;

        public PMABaseInfo[] BaseInfos;
        public PMAKeyframe[] Keyframes;

        public PMAData(PMAnimation anim, int index) : base(anim, index) => Create();

        void Create()
        {
            ref AnimationData animData = ref Source;
            Name = "Animation Data";

            BaseInfos = new PMABaseInfo[animData.baseInfoCount];
            for (int i = 0; i < BaseInfos.Length; i++)
                BaseInfos[i] = new PMABaseInfo(this, i);

            Keyframes = new PMAKeyframe[animData.keyframeCount];
            for (int i = 0; i < Keyframes.Length; i++)
                Keyframes[i] = new PMAKeyframe(this, i);
        }
    }

    public class PMABaseInfo : PMANode
    {
        public ref AnimationBaseInfo Source => ref AnimData.Source.BaseInfos[Index];
        public PMAData AnimData { get; }

        public PMABaseInfo(PMAData animData, int index) : base(animData.Animation, index)
        {
            AnimData = animData;
            Create();
        }

        void Create()
        {
            Name = "Animation Base Info " + Index;
        }
    }

    public class PMAKeyframe : PMANode
    {
        public ref AnimationKeyFrame Source => ref AnimData.Source.KeyFrames[Index];
        public PMAData AnimData { get; }

        public PMAVectorDelta[] VertexPositionDeltas;
        public PMAVectorDelta[] VertexNormalDeltas;
        public PMATransformDelta[] TransformDeltas;
        public PMAVisibilityDelta[] VisibilityDeltas;
        public PMATexCoordDelta[] TexCoordDeltas;

        public float Time => Source.time;

        public PMAKeyframe(PMAData animData, int index) : base(animData.Animation, index)
        {
            AnimData = animData;
            Create();
        }

        void Create()
        {
            Name = "Keyframe " + Index;

            ref AnimationData animData = ref AnimData.Source;
            ref AnimationKeyFrame keyframe = ref Source;

            VertexPositionDeltas = new PMAVectorDelta[keyframe.vertexPositionDeltaCount];
            for (int i = 0; i < VertexPositionDeltas.Length; i++)
                VertexPositionDeltas[i] = new PMAVectorDelta(this, m => m.VertexPositions,
                    (int)keyframe.vertexPositionDeltaBaseIndex + i, animData.VertexPositionDeltas);

            VertexNormalDeltas = new PMAVectorDelta[keyframe.vertexNormalDeltaCount];
            for (int i = 0; i < VertexNormalDeltas.Length; i++)
                VertexNormalDeltas[i] = new PMAVectorDelta(this, m => m.VertexNormals,
                    (int)keyframe.vertexNormalDeltaBaseIndex + i, animData.VertexNormalDeltas);

            TransformDeltas = new PMATransformDelta[keyframe.groupTransformDataDeltaCount];
            for (int i = 0; i < TransformDeltas.Length; i++)
                TransformDeltas[i] = new PMATransformDelta(this, (int)keyframe.groupTransformDataDeltaBaseIndex + i);

            VisibilityDeltas = new PMAVisibilityDelta[keyframe.visibilityGroupDeltaCount];
            for (int i = 0; i < VisibilityDeltas.Length; i++)
                VisibilityDeltas[i] = new PMAVisibilityDelta(this, (int)keyframe.visibilityGroupDeltaBaseIndex + i);

            TexCoordDeltas = new PMATexCoordDelta[keyframe.textureCoordinateTransformDeltaCount];
            for (int i = 0; i < TexCoordDeltas.Length; i++)
                TexCoordDeltas[i] = new PMATexCoordDelta(this, (int)keyframe.textureCoordinateTransformDeltaBaseIndex + i);
        }
    }

    public interface IPMADelta
    {
        public int Index { get; }
        public byte IndexDelta { get; }

        public void ApplyDelta(ref AccumDelta acc, ref VirtualModel model);
    }

    public class PMAVectorDelta : PMANode, IPMADelta
    {
        public ref AnimationVectorDelta Source => ref SourceArray[Index];

        public AnimationVectorDelta[] SourceArray { get; }
        public PMAKeyframe Keyframe { get; }

        public System.Func<VirtualModel, Vector3[]> OnApplyDelta;

        public byte IndexDelta => Source.indexDelta;
        public Vector3 CoordinateDelta => new Vector3(Source.coordinateDeltaX, Source.coordinateDeltaY, Source.coordinateDeltaZ) / 16.0f;

        public PMAVectorDelta(PMAKeyframe keyframe, System.Func<VirtualModel, Vector3[]> onApplyDelta, int index, AnimationVectorDelta[] source) : base(keyframe.Animation, index)
        {
            Keyframe = keyframe;
            OnApplyDelta = onApplyDelta;
            SourceArray = source;
            Create();
        }

        void Create()
        {
            Name = "Vector Delta " + Index;
        }

        public void ApplyDelta(ref AccumDelta acc, ref VirtualModel model)
        {
            OnApplyDelta(model)[acc.TotalIndex] += CoordinateDelta;
        }
    }

    public class PMATransformDelta : PMANode, IPMADelta
    {
        public ref AnimationGroupTransformDelta Source => ref Keyframe.AnimData.Source.GroupTransformDeltas[Index];
        public PMAKeyframe Keyframe { get; }

        public byte IndexDelta => Source.indexDelta;
        public float ValueDelta => Source.valueDelta / 16.0f;

        public PMATransformDelta(PMAKeyframe keyframe, int index) : base(keyframe.Animation, index)
        {
            Keyframe = keyframe;
            Create();
        }

        void Create()
        {
            Name = "Group Transform Delta " + Index;
        }

        public void ApplyDelta(ref AccumDelta acc, ref VirtualModel model)
        {
            GroupTransform t = model.GroupTransforms[acc.TotalIndex];
            model.GroupTransforms[acc.TotalIndex] = new GroupTransform(t.transformValue + ValueDelta);
        }
    }

    public class PMATexCoordDelta : PMANode, IPMADelta
    {
        public ref AnimationTexCoordTransformDelta Source => ref Keyframe.AnimData.Source.TexCoordTransformDeltas[Index];
        public PMAKeyframe Keyframe { get; }

        public byte IndexDelta => Source.indexDelta;
        public sbyte FrameDelta => Source.wFrameExtDelta;
        public float TranslateXDelta => Source.translateXDelta;
        public float TranslateYDelta => Source.translateYDelta;

        public PMATexCoordDelta(PMAKeyframe keyframe, int index) : base(keyframe.Animation, index)
        {
            Keyframe = keyframe;
            Create();
        }

        void Create()
        {
            Name = "Texture Coordinate Transform Delta " + Index;
        }

        public void ApplyDelta(ref AccumDelta acc, ref VirtualModel model)
        {
            TexCoordTransform t = model.TexCoordTransforms[acc.TotalIndex];
            model.TexCoordTransforms[acc.TotalIndex] = new TexCoordTransform(t,
                t.translationX + TranslateXDelta, t.translationY + TranslateYDelta, (byte)(t.textureFrameOffset + FrameDelta));
        }
    }

    public class PMAVisibilityDelta : PMANode, IPMADelta
    {
        public ref AnimationGroupVisibility Source => ref Keyframe.AnimData.Source.GroupVisibilityDeltas[Index];
        public PMAKeyframe Keyframe { get; }

        public byte IndexDelta => Source.visibilityGroupId;
        public sbyte Visible => Source.bVisible;

        public PMAVisibilityDelta(PMAKeyframe keyframe, int index) : base(keyframe.Animation, index)
        {
            Keyframe = keyframe;
            Create();
        }

        void Create()
        {
            Name = "Group Visibility Delta " + Index;
        }

        public void ApplyDelta(ref AccumDelta acc, ref VirtualModel model)
        {
            model.VisibilityGroups[acc.TotalIndex] = new VisibilityGroup((byte)(Visible < 0 ? 0 : 1));
        }
    }
}