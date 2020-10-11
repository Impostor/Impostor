namespace Impostor.Server.Events
{
    internal readonly struct EventHandler
    {
        public EventHandler(IEventListener o, RegisteredEventListener listener)
        {
            Object = o;
            Listener = listener;
        }

        public IEventListener Object { get; }

        public RegisteredEventListener Listener { get; }

        public void Deconstruct(out IEventListener o, out RegisteredEventListener listener)
        {
            o = Object;
            listener = Listener;
        }
    }
}