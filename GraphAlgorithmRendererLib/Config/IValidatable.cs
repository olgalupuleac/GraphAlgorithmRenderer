using System;

namespace GraphAlgorithmRendererLib.Config
{
    public interface IValidatable
    {
        void Validate();
    }

    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message)
        {
        }
    }
}