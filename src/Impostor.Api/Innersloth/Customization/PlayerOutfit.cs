namespace Impostor.Api.Innersloth.Customization
{
    public class PlayerOutfit
    {
        private string _playerName = string.Empty;
        private ColorType _color = (ColorType)(-1);
        private string _hatId = "missing";
        private string _petId = "missing";
        private string _skinId = "missing";
        private string _visorId = "missing";
        private string _namePlateId = "missing";

        public bool IsDirty { get; internal set; }

        public string PlayerName
        {
            get => _playerName;
            set => SetField(ref _playerName, value);
        }

        public ColorType Color
        {
            get => _color;
            set => SetField(ref _color, value);
        }

        public string HatId
        {
            get => _hatId;
            set => SetField(ref _hatId, value);
        }

        public string PetId
        {
            get => _petId;
            set => SetField(ref _petId, value);
        }

        public string SkinId
        {
            get => _skinId;
            set => SetField(ref _skinId, value);
        }

        public string VisorId
        {
            get => _visorId;
            set => SetField(ref _visorId, value);
        }

        public string NamePlateId
        {
            get => _namePlateId;
            set => SetField(ref _namePlateId, value);
        }

        public byte HatSequenceId { get; internal set; } = 0;

        public byte PetSequenceId { get; internal set; } = 0;

        public byte SkinSequenceId { get; internal set; } = 0;

        public byte VisorSequenceId { get; internal set; } = 0;

        public byte NamePlateSequenceId { get; internal set; } = 0;

        public bool IsIncomplete
        {
            get
            {
                if (!string.IsNullOrEmpty(PlayerName) && Color != (ColorType)(-1) && HatId != "missing" && PetId != "missing" && SkinId != "missing" && VisorId != "missing")
                {
                    return NamePlateId == "missing";
                }

                return true;
            }
        }

        public void Serialize(IMessageWriter writer)
        {
            writer.Write(PlayerName);
            writer.WritePacked((int)Color);
            writer.Write(HatId);
            writer.Write(PetId);
            writer.Write(SkinId);
            writer.Write(VisorId);
            writer.Write(NamePlateId);
            writer.Write(HatSequenceId);
            writer.Write(PetSequenceId);
            writer.Write(SkinSequenceId);
            writer.Write(VisorSequenceId);
            writer.Write(NamePlateSequenceId);
        }

        public void Deserialize(IMessageReader reader)
        {
            PlayerName = reader.ReadString();
            Color = (ColorType)reader.ReadPackedInt32();
            HatId = reader.ReadString();
            PetId = reader.ReadString();
            SkinId = reader.ReadString();
            VisorId = reader.ReadString();
            NamePlateId = reader.ReadString();
            HatSequenceId = reader.ReadByte();
            PetSequenceId = reader.ReadByte();
            SkinSequenceId = reader.ReadByte();
            VisorSequenceId = reader.ReadByte();
            NamePlateSequenceId = reader.ReadByte();
        }

        private void SetField<T>(ref T field, T value)
        {
            field = value;
            IsDirty = true;
        }
    }
}
