using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphAlgorithmRenderer.UIControls
{
    public class ConfigGenerationException : Exception
    {
        public ConfigGenerationException(string message) : base(message)
        {
        }

        public ConfigGenerationException(string message, Exception exception) : base(message, exception)
        {
        }
    }
}