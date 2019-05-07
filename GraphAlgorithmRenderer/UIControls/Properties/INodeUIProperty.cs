using System.Collections.Generic;
using GraphAlgorithmRenderer.Config;

namespace GraphAlgorithmRenderer.UIControls.Properties
{
    public interface INodeUiProperty : IUiProperty
    {
        void FromINodeProperty(INodeProperty property);
        List<INodeProperty> NodeProperty { get; }
    }
}