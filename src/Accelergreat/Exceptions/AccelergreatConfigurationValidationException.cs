using System.ComponentModel;
using NJsonSchema.Validation;
#if !NET8_0_OR_GREATER
using System.Runtime.Serialization;
#endif

namespace Accelergreat.Exceptions;


[Serializable]
public sealed class AccelergreatConfigurationValidationException : Exception
{
    internal AccelergreatConfigurationValidationException(ValidationError validationError) 
        : base(validationError.ToString())
    {
    }

#if !NET8_0_OR_GREATER
    private AccelergreatConfigurationValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
#endif
}