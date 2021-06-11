namespace Impostor.Api.Innersloth.Maps.Tasks
{
    public class AirshipTask : ITask
    {
        internal AirshipTask(Ids id, TaskTypes type, TaskCategories category, bool isVisual = false)
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
            ElectricalResetBreakers = 2,
            VaultRoomDownloadData = 3,
            BrigDownloadData = 4,
            CargoBayDownloadData = 5,
            GapRoomDownloadData = 6,
            RecordsDownloadData = 7,
            CargoBayUnlockSafe = 8,
            VentilationStartFans = 9,
            MainHallEmptyGarbage = 10,
            MedicalEmptyGarbage = 11,
            KitchenEmptyGarbage = 12,
            MainHallDevelopPhotos = 13,
            CargoBayFuelEngines = 14,
            SecurityRewindTapes = 15,
            VaultRoomPolishRuby = 16,
            ElectricalCalibrateDistributor = 17,
            CockpitStabilizeSteering = 18,
            ArmoryDownloadData = 19,
            CockpitDownloadData = 20,
            CommsDownloadData = 21,
            MedicalDownloadData = 22,
            ViewingDeckDownloadData = 23,
            ElectricalDivertPowerToArmory = 24,
            ElectricalDivertPowerToCockpit = 25,
            ElectricalDivertPowerToGapRoom = 26,
            ElectricalDivertPowerToMainHall = 27,
            ElectricalDivertPowerToMeetingRoom = 28,
            ElectricalDivertPowerToShowers = 29,
            ElectricalDivertPowerToEngine = 30,
            ShowersPickUpTowels = 31,
            LoungeCleanToilet = 32,
            VaultRoomDressMannequin = 33,
            RecordsSortRecords = 34,
            ArmoryPutAwayPistols = 35,
            ArmoryPutAwayRifles = 36,
            MainHallDecontaminate = 37,
            KitchenMakeBurger = 38,
            ShowersFixShower = 39,
        }

        public Ids Id { get; }

        int ITask.Id => (int)Id;

        public string Name { get; }

        public TaskTypes Type { get; }

        public TaskCategories Category { get; }

        public bool IsVisual { get; }
    }
}
