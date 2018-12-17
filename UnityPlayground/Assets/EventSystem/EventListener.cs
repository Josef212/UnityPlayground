public interface EventListener
{
    EventType GetSupportedEvents();
    void RecieveEvent(EventType type, Response response);
}
