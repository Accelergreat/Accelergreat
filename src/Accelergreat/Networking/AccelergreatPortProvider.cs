using System.Net.NetworkInformation;

namespace Accelergreat.Networking;

public static class AccelergreatPortProvider
{
    // ReSharper disable once ArrangeObjectCreationWhenTypeEvident
    private static readonly object PortLocker = new object();
    private static readonly int[] UnusablePorts = GetUsedPorts();
    private static int _port = 5000;

    public static int GetNextAvailablePort()
    {
        int value;

        lock (PortLocker)
        {
            while (UnusablePorts.Contains(_port))
            {
                _port++;
            }

            value = _port++;
        }

        return value;
    }

    private static int[] GetUsedPorts()
    {
        var ipProperties = IPGlobalProperties.GetIPGlobalProperties();

        var usedPorts =
            ipProperties.GetActiveTcpConnections()
                .Where(connection => connection.State != TcpState.Closed)
                .Select(connection => connection.LocalEndPoint)
                .Concat(ipProperties.GetActiveTcpListeners())
                .Concat(ipProperties.GetActiveUdpListeners())
                .Select(endpoint => endpoint.Port)
                .ToArray();

        return usedPorts;
    }
}