using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace Accelergreat.EntityFramework;


internal static class DatabaseNameFactory
{
    internal static string NewDatabaseName<TDbContext>() where TDbContext : DbContext
    {
        var contextTypeName = typeof(TDbContext).Name[..Math.Min(8, typeof(TDbContext).Name.Length)];
        var uniqueRef = Guid.NewGuid().ToString("N")[..8];

        return $"Accelergreat_{contextTypeName}_{uniqueRef}";
    }
}