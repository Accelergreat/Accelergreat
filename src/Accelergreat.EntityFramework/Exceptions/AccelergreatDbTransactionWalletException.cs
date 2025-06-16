using System.ComponentModel;
#if !NET8_0_OR_GREATER
using System.Runtime.Serialization;
#endif

namespace Accelergreat.EntityFramework.Exceptions;


[Serializable]
public sealed class AccelergreatDbTransactionWalletException : Exception
{
    internal AccelergreatDbTransactionWalletException() : base("Transaction has already been set")
    {
    }

#if !NET8_0_OR_GREATER
    private AccelergreatDbTransactionWalletException(SerializationInfo info, StreamingContext context) 
        : base(info, context)
    {
    }
#endif
}