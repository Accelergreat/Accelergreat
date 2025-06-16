// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
namespace Accelergreat.EntityFramework.SqlServer;

/// <summary>
/// <para>Configuration for SQL Server databases managed via Entity Framework.</para>
/// <para>Override default values in Accelergreat configuration under section <b>SqlServerEntityFramework</b>.</para>
/// </summary>
public class SqlServerEntityFrameworkConfiguration
{
    internal const string ConfigurationSectionName = "SqlServerEntityFramework";

    /// <summary>
    /// Default Value: (localdb)\MSSQLLocalDB.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Default Value: SnapshotRollback.
    /// </summary>
    public SqlSeverResetStrategy ResetStrategy { get; set; } = SqlSeverResetStrategy.SnapshotRollback;

    /// <summary>
    /// Default Value: TypeConfigurations.
    /// </summary>
    public SqlSeverCreateStrategy CreateStrategy { get; set; } = SqlSeverCreateStrategy.TypeConfigurations;

    /// <summary>
    /// Default Value: null.
    /// </summary>
    public string? SnapshotDirectory { get; set; }
}