namespace Impostor.Api.Innersloth
{
    public static class DecontamDoorsParser
    {
        public static DecontamDoors Parse(byte MapId, SystemTypes SystemType, byte amount)
        {
            if (amount < 1 || amount > 4) return DecontamDoors.None;
            if (MapId == 0 || MapId > 2) return DecontamDoors.None;

            byte offset = 0;
            
            if (MapId == 1 && SystemType != SystemTypes.Decontamination) return DecontamDoors.None;
            if (MapId == 2)
            {
                if (SystemType == SystemTypes.TopDecontaminationPolus) offset += 4;
                else if (SystemType == SystemTypes.Decontamination) offset += 8;
                else return DecontamDoors.None;
            }

            return (DecontamDoors)(amount + offset);
        }
    }
}