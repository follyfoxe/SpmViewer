using UnityEngine;

namespace PM.Models
{
    public abstract class PMNode : MonoBehaviour
    {
        public PMNode Parent => transform.parent.GetComponent<PMNode>();

        public string Name { get => gameObject.name; set => gameObject.name = value; }
        public PMModel Model { get; private set; }
        public int Index { get; private set; }

        public abstract void Create();

        public void SetParent(PMNode node) => SetParent(node, Model.transform);
        public void SetParent(PMNode node, Transform root)
        {
            Transform t = node ? node.transform : root;
            transform.SetParent(t, false);
        }

        public static T Create<T>(PMModel model, int index) where T : PMNode
        {
            T node = new GameObject().AddComponent<T>();
            node.Model = model;
            node.Index = index;
            node.SetParent(null);
            node.Create();
            return node;
        }
    }
}