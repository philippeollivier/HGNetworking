using UnityEngine;

namespace HG
{
    public static class NetworkingConstants
    {
        public const int MAX_PLAYERS = 5;
        public const int SERVER_PORT = 6942;
        public const int CLIENT_PORT = 6943;

        public const int DATA_BUFFER_SIZE = 4096;
        public const int WINDOW_SIZE = 64;
        public const int TIMEOUT_TICKS = 64;
    }
    public static class Networking
    {
        //General Data Info
        public static bool isServer;

        //High Level Managers
        public static NetworkingThreadManager NetworkingThreadManager;
        public static ConnectionManager ConnectionManager;

        public static void StartNetworkingClient(int port, bool isServer = false)
        {
            Networking.NetworkingThreadManager = new NetworkingThreadManager();
            Networking.ConnectionManager = new ConnectionManager();

            Networking.isServer = isServer;
            if (!isServer)
            {
                Physics.IgnoreLayerCollision(LayerMask.NameToLayer("ServerPhysics"), LayerMask.NameToLayer("ServerPhysics"));
            }

            Debug.Log($"Started NetworkingClient [{(isServer ? "Server" : "Client")}] listening on UDP Socket {port}");
            PlatformPacketManager.OpenUDPSocket(port);
        }
    }
}
