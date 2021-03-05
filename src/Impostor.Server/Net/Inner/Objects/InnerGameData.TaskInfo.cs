using System;
using Impostor.Api.Events.Managers;
using Impostor.Api.Innersloth;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Net.Messages;
using Impostor.Server.Net.State;

namespace Impostor.Server.Net.Inner.Objects
{
    internal partial class InnerGameData
    {
        public partial class TaskInfo : ITaskInfo
        {
            private readonly IEventManager _eventManager;
            private readonly Game _game;
            private readonly IInnerPlayerControl _player;

            public TaskInfo(IEventManager eventManager, Game game, IInnerPlayerControl player)
            {
                _eventManager = eventManager;
                _game = game;
                _player = player;
            }

            public uint Id { get; internal set; }

            public bool Complete { get; internal set; }

            public TaskTypes Type { get; internal set; }

            public void Serialize(IMessageWriter writer)
            {
                writer.WritePacked(Id);
                writer.Write(Complete);
            }

            public void Deserialize(IMessageReader reader)
            {
                Id = reader.ReadPackedUInt32();
                Complete = reader.ReadBoolean();
            }

            internal static TaskTypes GetType(MapTypes map, uint taskId)
            {
                return map switch
                {
                    MapTypes.Skeld => taskId switch
                    {
                        0 => TaskTypes.SkeldAdminSwipeCard,
                        1 => TaskTypes.SkeldElectricalFixWiring,
                        2 => TaskTypes.SkeldWeaponsClearAsteroids,
                        3 => TaskTypes.SkeldEnginesAlignEngineOutput,
                        4 => TaskTypes.SkeldMedbaySubmitScan,
                        5 => TaskTypes.SkeldMedbayInspectSample,
                        6 => TaskTypes.SkeldStorageFuelEngines,
                        7 => TaskTypes.SkeldReactorStartReactor,
                        8 => TaskTypes.SkeldO2EmptyChute,
                        9 => TaskTypes.SkeldCafeteriaEmptyGarbage,
                        10 => TaskTypes.SkeldCommunicationsDownloadData,
                        11 => TaskTypes.SkeldElectricalCalibrateDistributor,
                        12 => TaskTypes.SkeldNavigationChartCourse,
                        13 => TaskTypes.SkeldO2CleanO2Filter,
                        14 => TaskTypes.SkeldReactorUnlockManifolds,
                        15 => TaskTypes.SkeldElectricalDownloadData,
                        16 => TaskTypes.SkeldNavigationStabilizeSteering,
                        17 => TaskTypes.SkeldWeaponsDownloadData,
                        18 => TaskTypes.SkeldShieldsPrimeShields,
                        19 => TaskTypes.SkeldCafeteriaDownloadData,
                        20 => TaskTypes.SkeldNavigationDownloadData,
                        21 => TaskTypes.SkeldElectricalDivertPowerToShields,
                        22 => TaskTypes.SkeldElectricalDivertPowerToWeapons,
                        23 => TaskTypes.SkeldElectricalDivertPowerToCommunications,
                        24 => TaskTypes.SkeldElectricalDivertPowerToUpperEngine,
                        25 => TaskTypes.SkeldElectricalDivertPowerToO2,
                        26 => TaskTypes.SkeldElectricalDivertPowerToNavigation,
                        27 => TaskTypes.SkeldElectricalDivertPowerToLowerEngine,
                        28 => TaskTypes.SkeldElectricalDivertPowerToSecurity,
                        _ => throw new ArgumentException($"Invalid task id {taskId} for map {map}")
                    },
                    MapTypes.MiraHQ => taskId switch
                    {
                        0 => TaskTypes.MiraHallwayFixWiring,
                        1 => TaskTypes.MiraAdminEnterIDCode,
                        2 => TaskTypes.MiraMedbaySubmitScan,
                        3 => TaskTypes.MiraBalconyClearAsteroids,
                        4 => TaskTypes.MiraElectricalDivertPowerToAdmin,
                        5 => TaskTypes.MiraElectricalDivertPowerToCafeteria,
                        6 => TaskTypes.MiraElectricalDivertPowerToCommunications,
                        7 => TaskTypes.MiraElectricalDivertPowerToLaunchpad,
                        8 => TaskTypes.MiraElectricalDivertPowerToMedbay,
                        9 => TaskTypes.MiraElectricalDivertPowerToOffice,
                        10 => TaskTypes.MiraStorageWaterPlants,
                        11 => TaskTypes.MiraReactorStartReactor,
                        12 => TaskTypes.MiraElectricalDivertPowerToGreenhouse,
                        13 => TaskTypes.MiraAdminChartCourse,
                        14 => TaskTypes.MiraGreenhouseCleanO2Filter,
                        15 => TaskTypes.MiraLaunchpadFuelEngines,
                        16 => TaskTypes.MiraLaboratoryAssembleArtifact,
                        17 => TaskTypes.MiraLaboratorySortSamples,
                        18 => TaskTypes.MiraAdminPrimeShields,
                        19 => TaskTypes.MiraCafeteriaEmptyGarbage,
                        20 => TaskTypes.MiraBalconyMeasureWeather,
                        21 => TaskTypes.MiraElectricalDivertPowerToLaboratory,
                        22 => TaskTypes.MiraCafeteriaBuyBeverage,
                        23 => TaskTypes.MiraOfficeProcessData,
                        24 => TaskTypes.MiraLaunchpadRunDiagnostics,
                        25 => TaskTypes.MiraReactorUnlockManifolds,
                        _ => throw new ArgumentException($"Invalid task id {taskId} for map {map}")
                    },
                    MapTypes.Polus => taskId switch
                    {
                        0 => TaskTypes.PolusOfficeSwipeCard,
                        1 => TaskTypes.PolusDropshipInsertKeys,
                        2 => TaskTypes.PolusOfficeScanBoardingPass,
                        3 => TaskTypes.PolusElectricalFixWiring,
                        4 => TaskTypes.PolusWeaponsDownloadData,
                        5 => TaskTypes.PolusOfficeDownloadData,
                        6 => TaskTypes.PolusElectricalDownloadData,
                        7 => TaskTypes.PolusSpecimenRoomDownloadData,
                        8 => TaskTypes.PolusO2DownloadData,
                        9 => TaskTypes.PolusSpecimenRoomStartReactor,
                        10 => TaskTypes.PolusStorageFuelEngines,
                        11 => TaskTypes.PolusBoilerRoomOpenWaterways,
                        12 => TaskTypes.PolusMedbayInspectSample,
                        13 => TaskTypes.PolusBoilerRoomReplaceWaterJug,
                        14 => TaskTypes.PolusOutsideFixWeatherNodeNode_GI,
                        15 => TaskTypes.PolusOutsideFixWeatherNodeNode_IRO,
                        16 => TaskTypes.PolusOutsideFixWeatherNodeNode_PD,
                        17 => TaskTypes.PolusOutsideFixWeatherNodeNode_TB,
                        18 => TaskTypes.PolusCommunicationsRebootWiFi,
                        19 => TaskTypes.PolusO2MonitorTree,
                        20 => TaskTypes.PolusSpecimenRoomUnlockManifolds,
                        21 => TaskTypes.PolusSpecimenRoomStoreArtifacts,
                        22 => TaskTypes.PolusO2FillCanisters,
                        23 => TaskTypes.PolusO2EmptyGarbage,
                        24 => TaskTypes.PolusDropshipChartCourse,
                        25 => TaskTypes.PolusMedbaySubmitScan,
                        26 => TaskTypes.PolusWeaponsClearAsteroids,
                        27 => TaskTypes.PolusOutsideFixWeatherNodeNode_CA,
                        28 => TaskTypes.PolusOutsideFixWeatherNodeNode_MLG,
                        29 => TaskTypes.PolusLaboratoryAlignTelescope,
                        30 => TaskTypes.PolusLaboratoryRepairDrill,
                        31 => TaskTypes.PolusLaboratoryRecordTemperature,
                        32 => TaskTypes.PolusOutsideRecordTemperature,
                        _ => throw new ArgumentException($"Invalid task id {taskId} for map {map}")
                    },
                    _ => throw new ArgumentException($"Invalid task id {taskId} for map {map}"),
                };
            }
        }
    }
}
