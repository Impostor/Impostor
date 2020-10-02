namespace Impostor.Server.Net.Messages.Rpc
{
    internal static class RpcMessageFlags
    {
        public const byte PlayAnimation = 0;
        public const byte CompleteTask = 1;
        public const byte SyncSettings = 2;
        public const byte SetInfected = 3;
        public const byte Exiled = 4;
        public const byte CheckName = 5;
        public const byte SetName = 6;
        public const byte CheckColor = 7;
        public const byte SetColor = 8;
        public const byte SetHat = 9;
        public const byte SetSkin = 10;
        public const byte ReportDeadBody = 11;
        public const byte MurderPlayer = 12;
        public const byte SendChat = 13;
        public const byte StartMeeting = 14;
        public const byte SetScanner = 15;
        public const byte SendChatNote = 16;
        public const byte SetPet = 17;
        public const byte SetStartCounter = 18;
        public const byte EnterVent = 19;
        public const byte ExitVent = 20;
        public const byte SnapTo = 21;
        public const byte Close = 22;
        public const byte VotingComplete = 23;
        public const byte CastVote = 24;
        public const byte ClearVote = 25;
        public const byte AddVote = 26;
        public const byte CloseDoorsOfType = 27;
        public const byte RepairSystem = 28;
        public const byte SetTasks = 29;
        public const byte UpdateGameData = 30;
    }
}