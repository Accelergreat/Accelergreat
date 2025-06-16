using System.ComponentModel;
#if !NET8_0_OR_GREATER
using System.Runtime.Serialization;
#endif

namespace Accelergreat.Web.Exceptions;


[Serializable]
public sealed class AccelergreatWebAppComponentException : Exception
{
    internal AccelergreatWebAppComponentException(string message) : base(message)
    {
    }

#if !NET8_0_OR_GREATER
    private AccelergreatWebAppComponentException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
#endif
}