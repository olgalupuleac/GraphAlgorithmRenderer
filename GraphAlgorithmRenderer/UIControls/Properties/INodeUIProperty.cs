using System.Collections.Generic;
using GraphAlgorithmRendererLib.Config;

namespace GraphAlgorithmRenderer.UIControls.Properties
{
    public interface INodeUiProperty : IUiProperty
    {
        void FromINodeProperty(INodeProperty property);
        List<INodeProperty> NodeProperty { get; }
    }
}