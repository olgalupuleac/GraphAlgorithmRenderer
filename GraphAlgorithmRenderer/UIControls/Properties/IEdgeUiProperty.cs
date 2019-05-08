using System.Collections.Generic;
using GraphAlgorithmRenderer.Config;

namespace GraphAlgorithmRenderer.UIControls.Properties
{
    public interface IEdgeUiProperty : IUiProperty
    {
        void FromIEdgeProperty(IEdgeProperty property);
        List<IEdgeProperty> EdgeProperty { get; }    
    }
}