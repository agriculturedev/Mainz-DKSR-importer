namespace Importer.Constants;

public static class Websockets
{
    private const string Prefix = "wss://";
    private const string Url = "mainz-staging.dksr.city";
    public const string TreesenseSocket = "/OutboundInterfaces/outbound/TreeSense-Realtime-Data";
    public const string ParkingLotSocket = "/OutboundInterfaces/outbound/ParkinglotSNEventTypeStatement";
    
    public static string GetAuthenticatedWebsocketUrl(string username, string password, string socket)
    {
        return $"{Prefix}{Url}{socket}";
    }
}