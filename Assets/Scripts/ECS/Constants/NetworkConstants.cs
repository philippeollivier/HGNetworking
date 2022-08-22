static class NetworkConstants {
    public const int MAX_PACKET_SIZE = 512;
    public static int MIN_HEADER_SIZE = 4 + 4 + 4 + 4 + 4; //4 Bytes for ConnectionId, PacketId, MoveManager Header, EventManager Header, GhostManager Header
}