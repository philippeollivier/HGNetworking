public class ClientStreamManager : StreamManager
{
    public ClientStreamManager(Connection connection) : base(connection)
    {
    }

    override public void WriteToPacket()
    {
        bool hasInfo = MoreInfoToWrite();

        while (hasInfo)
        {
            using (Packet packet = new Packet())
            {
                int remainingBytes = NetworkConstants.MAX_PACKET_SIZE - NetworkConstants.MIN_HEADER_SIZE;

                //Create new packet
                bool validPacket = connection.GetPacket(packet, PacketType.Regular);
                if (!validPacket) {  break; }

                //Write info from each manager into packet in priority order (Move, Event, Ghost)
                //remainingBytes -= moveManager.WriteToPacket(remainingBytes, packet);
                remainingBytes -= eventManager.WriteToPacket(remainingBytes, packet);

                //Send packet through connection manager
                connection.SendPacket(packet);

                //Check if there is more info that needs to be sent
                hasInfo = MoreInfoToWrite();
            }
        }
    }

    override public void ReadFromPacket(Packet packet)
    {
        //Read info and send to appropriate manager (Event, Move, Ghost)
        eventManager.ReadFromPacket(packet);
        //ghostManager.ReadFromPacket(packet);
    }

    override public void ProcessNotification(bool success, int packetId)
    {
        eventManager.ProcessNotification(success, packetId);
        //ghostManager.ProcessNotification(success, packetId);
    }
    override protected bool MoreInfoToWrite()
    {
        return eventManager.HasMoreDataToWrite(); //moveManager.HasMoreDataToWrite() ||
    }
}
