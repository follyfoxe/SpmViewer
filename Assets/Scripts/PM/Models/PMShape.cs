using System.Collections.Generic;
using UnityEngine;

namespace PM.Models
{
    public class PMShape : PMNode
    {
        public ref Shape Source => ref Model.Model.Shapes[Index];
        public PMSubShape[] SubShapes;

        public override void Create()
        {
            ref Shape shape = ref Source;
            Name = shape.name;

            SubShapes = new PMSubShape[shape.subshapeCount];
            for (int i = 0; i < SubShapes.Length; i++)
                SubShapes[i] = Model.SubShapes[shape.subshapeBaseIndex + i];
        }

        public GameObject CreateInstance(Transform parent, out PMSubShapeInstance[] instances)
        {
            ref Shape shape = ref Source;

            GameObject s = new(Name + " (Instance)");
            s.transform.SetParent(parent, false);

            instances = new PMSubShapeInstance[SubShapes.Length];
            for (int sub = 0; sub < SubShapes.Length; sub++)
            {
                PMSubShape subShape = SubShapes[sub];
                PMSubShapeInstance subInstance = Create<PMSubShapeInstance>(Model, subShape.Index);
                subInstance.transform.SetParent(s.transform);
                subInstance.Initialize(this, subShape);
                instances[sub] = subInstance;
            }
            return gameObject;
        }
    }
}