namespace Impostor.Api.Innersloth
{
    public readonly struct Announcement
    {
        public readonly int Id;
        public readonly string Message;

        public Announcement(int id, string message)
        {
            Id = id;
            Message = message;
        }
    }
}
