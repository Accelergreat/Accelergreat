using System.ComponentModel;
using NJsonSchema;
using NJsonSchema.Validation;
#if !NET8_0_OR_GREATER
using System.Runtime.Serialization;
#endif

namespace Accelergreat.Exceptions;


[Serializable]
public sealed class AccelergreatConfigurationValidationsException : AggregateException
{
    internal AccelergreatConfigurationValidationsException(JsonSchema jsonSchema, string path, IEnumerable<ValidationError> validationErrors) 
        : base(
            $"Schema validation errors for \"{path}\". View schema: {jsonSchema.ExtensionData?["$id"]}",
            validationErrors.Select(x => new AccelergreatConfigurationValidationException(x)))
    {
    }

#if !NET8_0_OR_GREATER
    private AccelergreatConfigurationValidationsException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
#endif
}