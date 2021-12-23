using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Rainbow.ImgLib.Formats.Implementation;

using PM.Models.Animations;

namespace PM.Models
{
    public class PMModel : MonoBehaviour
    {
        public Model Model;
        public VirtualModel VirtualModel;

        public PMGroup[] Groups;

        public PMShape[] Shapes;
        public PMSubShape[] SubShapes;
        public PMPolygon[] Polygons;
        public PMSampler[] Samplers;

        public Texture2D[] Textures;
        public string TexturePath;
        public bool HasTextures;

        public PMAnimation[] Animations;
        public PMAnimator Animator;

        public Shader PMCutoutShader;
        public Shader PMBlendShader;
        public Shader PMVertexShader;
        public Material PMVertexMaterial;

        public static PMModel Load(string path)
        {
            PMModel model = new GameObject().AddComponent<PMModel>();
            model.transform.SetParent(MainGui.Instance.rootTransform, false);
            model.Initialize(path);
            return model;
        }

        void Initialize(string path)
        {
            Model = new Model(File.OpenRead(path));

            TexturePath = Path.Combine(Path.GetDirectoryName(path), Model.Header.textureName + "-");
            gameObject.name = Model.Header.modelName;

            PMCutoutShader = Shader.Find("PM/PMCutout");
            PMBlendShader = Shader.Find("PM/PMBlend");
            PMVertexShader = Shader.Find("PM/PMVertex");
            PMVertexMaterial = new Material(PMVertexShader);

            VirtualModel = new VirtualModel(Model);

            CreateTextures();
            CreateNodes();

            SetVirtualModel(VirtualModel);
            Animator = gameObject.AddComponent<PMAnimator>();
        }

        public void ResetVirtualModel()
        {
            SetVirtualModel(new VirtualModel(Model));
        }

        public void SetVirtualModel(VirtualModel model)
        {
            VirtualModel = model;
            UpdateVisual();
        }

        public void UpdateVisual()
        {
            foreach (PMGroup group in Groups)
            {
                if (group.UpdateVisibility()) //Update mesh only if visible
                    group.UpdateInstanceVisual();
                group.Transform.UpdateTransform(); //Update transforms independently as we are in a hierarchy
            }
        }

        void CreateTextures()
        {
            if (!File.Exists(TexturePath))
            {
                Debug.Log($"Missing Model Texture File: '{Model.Header.textureName}-'");
                return;
            }

            TPLTextureSerializer serializer = new();
            TPLTexture tpl = (TPLTexture)serializer.Open(File.OpenRead(TexturePath));

            Textures = new Texture2D[Model.Textures.Length];
            for (int i = 0; i < Textures.Length; i++)
            {
                tpl.SelectedFrame = (int)Model.Textures[i].tplIndex;
                Textures[i] = BlockUtility.TextureFromBitmap((System.Drawing.Bitmap)tpl.GetImage());
            }
            HasTextures = true;
        }

        void CreateNodes()
        {
            Transform t = CreateFolder("Other", transform);
            Samplers = CreateAllNodes<PMSampler>(CreateFolder("Samplers", t), Model.Samplers.Length);
            Polygons = CreateAllNodes<PMPolygon>(CreateFolder("Polygons", t), Model.Polygons.Length);
            SubShapes = CreateAllNodes<PMSubShape>(CreateFolder("SubShapes", t), Model.SubShapes.Length);
            Shapes = CreateAllNodes<PMShape>(CreateFolder("Shapes", t), Model.Shapes.Length);

            CreateGroups(CreateFolder("Groups", transform));

            Animations = CreateAllNodes<PMAnimation>(CreateFolder("Animations", transform), Model.Animations.Length);
        }

        Transform CreateFolder(string name, Transform parent)
        {
            Transform folder = new GameObject(name).transform;
            folder.SetParent(parent);
            return folder;
        }

        T[] CreateAllNodes<T>(Transform parent, int count) where T : PMNode
        {
            T[] nodes = new T[count];
            for (int i = 0; i < count; i++)
            {
                T node = PMNode.Create<T>(this, i);
                node.transform.SetParent(parent, false);
                nodes[i] = node;
            }
            return nodes;
        }

        void CreateGroups(Transform root)
        {
            Groups = new PMGroup[Model.Groups.Length];

            PMGroup startGroup = PMNode.Create<PMGroup>(this, Model.Groups.Length - 1);
            startGroup.transform.SetParent(root, false);

            Stack<PMGroup> stack = new();
            stack.Push(startGroup);

            while (stack.Count > 0)
            {
                PMGroup group = stack.Pop();
                Groups[group.Index] = group;
                ref Group source = ref group.Source;

                if (source.nextGroupId >= 0)
                {
                    PMGroup next = PMNode.Create<PMGroup>(this, source.nextGroupId);
                    group.NextGroup = next;
                    next.SetParent(group.Parent, root);
                    stack.Push(next);
                }
                if (source.childGroupId >= 0)
                {
                    PMGroup child = PMNode.Create<PMGroup>(this, source.childGroupId);
                    group.ChildGroup = child;
                    child.SetParent(group);
                    stack.Push(child);
                }
            }
        }
    }
}