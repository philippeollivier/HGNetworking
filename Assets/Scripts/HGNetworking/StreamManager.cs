public class StreamManager
{
    public Connection connection;

    public EventManager eventManager;
    public MoveManager moveManager;
    public GhostManager ghostManager;

    public StreamManager(Connection connection)
    {
        this.connection = connection;
        eventManager = new EventManager();
        moveManager = new MoveManager();
        ghostManager = new GhostManager();
    }

    public virtual void WriteToPacket() { }
    public virtual void ReadFromPacket(Packet packet) { }
    public virtual void ProcessNotification(bool success, int packetId) { }
    public virtual void WriteToAllConnections() { }

    protected virtual bool MoreInfoToWrite() { return false; }
}
