namespace Impostor.Api.Innersloth.Maps.Tasks
{
    public class PolusTask : ITask
    {
        internal PolusTask(Ids id, TaskTypes type, TaskCategories category, bool isVisual = false)
        {
            Id = id;
            Name = id.ToString();
            Type = type;
            Category = category;
            IsVisual = isVisual;
        }

        public enum Ids
        {
            OfficeSwipeCard = 0,
            DropshipInsertKeys = 1,
            OfficeScanBoardingPass = 2,
            ElectricalFixWiring = 3,
            WeaponsDownloadData = 4,
            OfficeDownloadData = 5,
            ElectricalDownloadData = 6,
            SpecimenRoomDownloadData = 7,
            O2DownloadData = 8,
            SpecimenRoomStartReactor = 9,
            StorageFuelEngines = 10,
            BoilerRoomOpenWaterways = 11,
            MedbayInspectSample = 12,
            BoilerRoomReplaceWaterJug = 13,
            OutsideFixWeatherNodeNode_GI = 14,
            OutsideFixWeatherNodeNode_IRO = 15,
            OutsideFixWeatherNodeNode_PD = 16,
            OutsideFixWeatherNodeNode_TB = 17,
            CommunicationsRebootWiFi = 18,
            O2MonitorTree = 19,
            SpecimenRoomUnlockManifolds = 20,
            SpecimenRoomStoreArtifacts = 21,
            O2FillCanisters = 22,
            O2EmptyGarbage = 23,
            DropshipChartCourse = 24,
            MedbaySubmitScan = 25,
            WeaponsClearAsteroids = 26,
            OutsideFixWeatherNodeNode_CA = 27,
            OutsideFixWeatherNodeNode_MLG = 28,
            LaboratoryAlignTelescope = 29,
            LaboratoryRepairDrill = 30,
            LaboratoryRecordTemperature = 31,
            OutsideRecordTemperature = 32,
        }

        public Ids Id { get; }

        int ITask.Id => (int)Id;

        public string Name { get; }

        public TaskTypes Type { get; }

        public TaskCategories Category { get; }

        public bool IsVisual { get; }
    }
}
