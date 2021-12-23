using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Models
{
    public class PMGroup : PMNode
    {
        public ref Group Source => ref Model.Model.Groups[Index];
        public ref VisibilityGroup VisibilityGroup => ref Model.VirtualModel.VisibilityGroups[Source.visibilityGroupId];

        public PMGroup NextGroup;
        public PMGroup ChildGroup;

        public PMShape Shape;
        public PMGroupTransform Transform;

        public GameObject ShapeInstance;
        public PMSubShapeInstance[] SubShapeInstances;

        public bool Visible => BlockUtility.GetBool(VisibilityGroup.bVisible);

        public override void Create()
        {
            ref Group group = ref Source;
            Name = group.name;

            if (group.shapeId >= 0)
            {
                Shape = Model.Shapes[group.shapeId];
                ShapeInstance = Shape.CreateInstance(transform, out SubShapeInstances);
            }
            Transform = new PMGroupTransform(this, group.transformBaseIndex);
            UpdateVisibility();
        }

        public void UpdateInstanceVisual()
        {
            if (Shape)
            {
                foreach (PMSubShapeInstance instance in SubShapeInstances)
                {
                    instance.UpdateMesh();
                    instance.UpdateMaterial();
                }
            }
        }

        public bool UpdateVisibility()
        {
            bool visible = Visible;
            gameObject.SetActive(visible);
            return visible;
        }
    }
}