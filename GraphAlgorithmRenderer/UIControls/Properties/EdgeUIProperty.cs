using GraphAlgorithmRenderer.Config;

namespace GraphAlgorithmRenderer.UIControls.Properties
{
    public interface IEdgeUiProperty
    {
        void FromIEdgeProperty(IEdgeProperty property);
        IEdgeProperty EdgeProperty { get; }
        void Reset();
    }
}