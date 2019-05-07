using GraphAlgorithmRenderer.Config;

namespace GraphAlgorithmRenderer.UIControls.Properties
{
    public interface INodeUiProperty
    {
        void FromINodeProperty(INodeProperty property);
        INodeProperty NodeProperty { get; }
        void Reset();
    }
}