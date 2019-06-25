using System.Collections.Generic;
using GraphAlgorithmRendererLib.Config;

namespace GraphAlgorithmRenderer.UIControls.Properties
{
    public interface IEdgeUiProperty : IUiProperty
    {
        void FromIEdgeProperty(IEdgeProperty property);
        List<IEdgeProperty> EdgeProperty { get; }    
    }
}