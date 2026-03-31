namespace NetworkAnalyzer.Models;

public class NetworkInterfaceInfo
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string IpAddress { get; set; } = "";
    public string SubnetMask { get; set; } = "";
    public string MacAddress { get; set; } = "";
    public string Status { get; set; } = "";
    public long Speed { get; set; }
    public string SpeedFormatted => FormatSpeed(Speed);
    public string InterfaceType { get; set; } = "";
    
    private static string FormatSpeed(long speed)
    {
        if (speed >= 1_000_000_000)
            return $"{speed / 1_000_000_000.0:F1} Gbps";
        if (speed >= 1_000_000)
            return $"{speed / 1_000_000.0:F1} Mbps";
        if (speed >= 1_000)
            return $"{speed / 1_000.0:F1} Kbps";
        return $"{speed} bps";
    }
}