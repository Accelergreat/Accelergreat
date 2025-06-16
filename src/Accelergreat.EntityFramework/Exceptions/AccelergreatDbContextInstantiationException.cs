using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
#if !NET8_0_OR_GREATER
using System.Runtime.Serialization;
#endif

namespace Accelergreat.EntityFramework.Exceptions;


[Serializable]
public sealed class AccelergreatDbContextInstantiationException<TDbContext> : Exception
    where TDbContext : DbContext
{
    internal AccelergreatDbContextInstantiationException() 
        : base($"{typeof(TDbContext).Name} must have a public constructor that takes a single parameter of type {typeof(DbContextOptions<TDbContext>).Name}")
    {
            
    }

#if !NET8_0_OR_GREATER
    private AccelergreatDbContextInstantiationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
#endif
}