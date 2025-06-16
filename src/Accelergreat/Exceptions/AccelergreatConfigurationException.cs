using System.ComponentModel;
#if !NET8_0_OR_GREATER
using System.Runtime.Serialization;
#endif

namespace Accelergreat.Exceptions;


[Serializable]
public sealed class AccelergreatConfigurationException : Exception
{
    internal AccelergreatConfigurationException(string sectionName, string propertyName, string message) 
        : base($"Accelergreat configuration {sectionName}:{propertyName} {message}.")
    {
    }

#if !NET8_0_OR_GREATER
    private AccelergreatConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
#endif
}