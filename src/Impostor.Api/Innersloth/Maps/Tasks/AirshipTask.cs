namespace Impostor.Api.Innersloth.Maps.Tasks
{
    public class AirshipTask : ITask
    {
        internal AirshipTask(Ids id, TaskTypes type, TaskCategories category, bool isVisual)
        {
            Id = id;
            Name = id.ToString();
            Type = type;
            Category = category;
            IsVisual = isVisual;
        }

        public enum Ids
        {
            ElectricalFixWiring = 0,
            MeetingRoomEnterIDCode = 1,
            ElectricalResetBreakers = 3,
            VaultRoomDownloadData = 4,
            BrigDownloadData = 5,
            CargoBayDownloadData = 6,
            GapRoomDownloadData = 7,
            RecordsDownloadData = 8,
            CargoBayUnlockSafe = 9,
            VentilationStartFans = 10,
            MainHallEmptyGarbage = 11,
            MedicalEmptyGarbage = 12,
            KitchenEmptyGarbage = 13,
            MainHallDevelopPhotos = 14,
            CargoBayFuelEngines = 15,
            SecurityRewindTapes = 16,
            VaultRoomPolishRuby = 17,
            ElectricalCalibrateDistributor = 18,
            CockpitStabilizeSteering = 19,
            ArmoryDownloadData = 20,
            CockpitDownloadData = 21,
            CommsDownloadData = 22,
            MedicalDownloadData = 23,
            ViewingDeckDownloadData = 24,
            ElectricalDivertPowerToArmory = 25,
            ElectricalDivertPowerToCockpit = 26,
            ElectricalDivertPowerToGapRoom = 27,
            ElectricalDivertPowerToMainHall = 28,
            ElectricalDivertPowerToMeetingRoom = 29,
            ElectricalDivertPowerToShowers = 30,
            ElectricalDivertPowerToEngine = 31,
            ShowersPickUpTowels = 32,
            LoungeCleanToilet = 33,
            VaultRoomDressMannequin = 34,
            RecordsSortRecords = 35,
            ArmoryPutAwayPistols = 36,
            ArmoryPutAwayRifles = 37,
            MainHallDecontaminate = 38,
            KitchenMakeBurger = 39,
            ShowersFixShower = 40,
        }

        public Ids Id { get; }

        int ITask.Id => (int)Id;

        public string Name { get; }

        public TaskTypes Type { get; }

        public TaskCategories Category { get; }

        public bool IsVisual { get; }
    }
}
