namespace PM.Models.Animations
{
    public abstract class PMANode
    {
        public PMModel Model => Animation.Model;
        public PMAnimation Animation { get; private set; }

        public string Name { get; protected set; }
        public int Index { get; private set; }

        public PMANode(PMAnimation animation, int index)
        {
            Animation = animation;
            Index = index;
        }
    }
}