using System;
using UnityEngine;

namespace PM.Models
{
    public class PMGroupTransform
    {
        public PMGroup Group { get; }

        public uint BaseIndex { get; }
        public ModelTransform Transform;

        public PMGroupTransform(PMGroup group, uint baseIndex)
        {
            BaseIndex = baseIndex;
            Group = group;

            UpdateTransform();
        }

        public void UpdateTransform()
        {
            Transform = new ModelTransform(Group.Model.VirtualModel.GroupTransforms, BaseIndex);
            Apply();
        }

        public void Apply()
        {
            bool isJoint = BlockUtility.GetBool(Group.Source.bIsJoint);
            Matrix4x4 m = Matrix4x4.identity;

            if (isJoint) //pos scale rot + joint rot ?
            {
                m *= Matrix4x4.Translate(Transform.Translation);
                m *= Matrix4x4.Scale(Transform.Scale);
                m *= Matrix4x4.Rotate(Quaternion.Euler(Transform.RotationIn2Deg * 2f));
                m *= Matrix4x4.Rotate(Quaternion.Euler(Transform.JointPostRotationInDeg));
            }
            else //pos scale rot + transform pivot and offsets ?
            {
                m *= Matrix4x4.Translate(Transform.Translation);

                m *= Matrix4x4.Translate(Transform.TransformRotationPivot);
                m *= Matrix4x4.Rotate(Quaternion.Euler(Transform.RotationIn2Deg * 2f));
                m *= Matrix4x4.Translate(-Transform.TransformRotationPivot);

                m *= Matrix4x4.Translate(Transform.TransformScalePivot);
                m *= Matrix4x4.Scale(Transform.Scale);
                m *= Matrix4x4.Translate(-Transform.TransformScalePivot);
            }

            Group.transform.localPosition = m.GetColumn(3);
            Group.transform.localRotation = Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1)); //m.rotation;
            Group.transform.localScale = new Vector3(m.GetColumn(0).magnitude, m.GetColumn(1).magnitude, m.GetColumn(2).magnitude); //m.lossyScale
        }
    }

    public class ModelTransform
    {
        public const int Size = BlockUtility.Vector3Size * 8;
        public const int SizeInFloats = Size / sizeof(float);

        //naming incorrect ?
        public Vector3 Translation { get => GetVector(0); set => SetVector(0, value); } //v1
        public Vector3 Scale { get => GetVector(1); set => SetVector(1, value); } //v2
        public Vector3 RotationIn2Deg { get => GetVector(2); set => SetVector(2, value); } //v3
        public Vector3 JointPostRotationInDeg { get => GetVector(3); set => SetVector(3, value); } //v4
        public Vector3 TransformRotationPivot { get => GetVector(4); set => SetVector(4, value); } //v5
        public Vector3 TransformScalePivot { get => GetVector(5); set => SetVector(5, value); } //v6
        public Vector3 TransformRotationOffset { get => GetVector(6); set => SetVector(6, value); } //v7
        public Vector3 TransformScaleOffset { get => GetVector(7); set => SetVector(7, value); } //v8
        //1 7 2 8 4 5 3 6

        public float[] floats = new float[SizeInFloats];

        public ModelTransform(GroupTransform[] source, uint startIndex)
        {
            for (int i = 0; i < SizeInFloats; i++)
                floats[i] = source[startIndex + i].transformValue;
        }

        public Vector3 GetVector(int index)
        {
            int baseInd = index * 3;
            return new Vector3(floats[baseInd], floats[baseInd + 1], floats[baseInd + 2]);
        }

        public void SetVector(int index, Vector3 val)
        {
            int baseInd = index * 3;
            floats[baseInd] = val.x;
            floats[baseInd + 1] = val.y;
            floats[baseInd + 2] = val.z;
        }
    }
}