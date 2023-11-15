namespace Laboratory.Core
{
    public class EventCallback
    {
        public EventCallback(object callback)
        {
            Callback = callback;
        }
        public object Callback { get; private set; }
    }
}