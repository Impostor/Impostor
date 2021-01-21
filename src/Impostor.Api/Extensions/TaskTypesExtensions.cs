using System;
using Impostor.Api.Innersloth;

namespace Impostor.Api
{
    public static class TaskTypesExtensions
    {
        public static TaskCategories GetCategory(this TaskTypes taskType)
        {
            return taskType switch
            {
                TaskTypes.SkeldAdminSwipeCard
                or TaskTypes.SkeldElectricalFixWiring
                or TaskTypes.MiraHallwayFixWiring
                or TaskTypes.MiraAdminEnterIDCode
                or TaskTypes.PolusOfficeSwipeCard
                or TaskTypes.PolusDropshipInsertKeys
                or TaskTypes.PolusOfficeScanBoardingPass
                or TaskTypes.PolusElectricalFixWiring
                => TaskCategories.CommonTask,

                TaskTypes.SkeldWeaponsClearAsteroids
                or TaskTypes.SkeldEnginesAlignEngineOutput
                or TaskTypes.SkeldMedbaySubmitScan
                or TaskTypes.SkeldMedbayInspectSample
                or TaskTypes.SkeldStorageFuelEngines
                or TaskTypes.SkeldReactorStartReactor
                or TaskTypes.SkeldO2EmptyChute
                or TaskTypes.SkeldCafeteriaEmptyGarbage
                or TaskTypes.MiraMedbaySubmitScan
                or TaskTypes.MiraBalconyClearAsteroids
                or TaskTypes.MiraElectricalDivertPowerToAdmin
                or TaskTypes.MiraElectricalDivertPowerToCafeteria
                or TaskTypes.MiraElectricalDivertPowerToCommunications
                or TaskTypes.MiraElectricalDivertPowerToLaunchpad
                or TaskTypes.MiraElectricalDivertPowerToMedbay
                or TaskTypes.MiraElectricalDivertPowerToOffice
                or TaskTypes.MiraStorageWaterPlants
                or TaskTypes.MiraReactorStartReactor
                or TaskTypes.MiraElectricalDivertPowerToGreenhouse
                or TaskTypes.PolusWeaponsDownloadData
                or TaskTypes.PolusOfficeDownloadData
                or TaskTypes.PolusElectricalDownloadData
                or TaskTypes.PolusSpecimenRoomDownloadData
                or TaskTypes.PolusO2DownloadData
                or TaskTypes.PolusSpecimenRoomStartReactor
                or TaskTypes.PolusStorageFuelEngines
                or TaskTypes.PolusBoilerRoomOpenWaterways
                or TaskTypes.PolusMedbayInspectSample
                or TaskTypes.PolusBoilerRoomReplaceWaterJug
                or TaskTypes.PolusOutsideFixWeatherNodeNode_GI
                or TaskTypes.PolusOutsideFixWeatherNodeNode_IRO
                or TaskTypes.PolusOutsideFixWeatherNodeNode_PD
                or TaskTypes.PolusOutsideFixWeatherNodeNode_TB
                or TaskTypes.PolusCommunicationsRebootWiFi
                => TaskCategories.LongTask,

                TaskTypes.SkeldCommunicationsDownloadData
                or TaskTypes.SkeldElectricalCalibrateDistributor
                or TaskTypes.SkeldNavigationChartCourse
                or TaskTypes.SkeldO2CleanO2Filter
                or TaskTypes.SkeldReactorUnlockManifolds
                or TaskTypes.SkeldElectricalDownloadData
                or TaskTypes.SkeldNavigationStabilizeSteering
                or TaskTypes.SkeldWeaponsDownloadData
                or TaskTypes.SkeldShieldsPrimeShields
                or TaskTypes.SkeldCafeteriaDownloadData
                or TaskTypes.SkeldNavigationDownloadData
                or TaskTypes.SkeldElectricalDivertPowerToShields
                or TaskTypes.SkeldElectricalDivertPowerToWeapons
                or TaskTypes.SkeldElectricalDivertPowerToCommunications
                or TaskTypes.SkeldElectricalDivertPowerToUpperEngine
                or TaskTypes.SkeldElectricalDivertPowerToO2
                or TaskTypes.SkeldElectricalDivertPowerToNavigation
                or TaskTypes.SkeldElectricalDivertPowerToLowerEngine
                or TaskTypes.SkeldElectricalDivertPowerToSecurity
                or TaskTypes.MiraElectricalDivertPowerToLaboratory
                or TaskTypes.MiraAdminChartCourse
                or TaskTypes.MiraGreenhouseCleanO2Filter
                or TaskTypes.MiraLaunchpadFuelEngines
                or TaskTypes.MiraLaboratoryAssembleArtifact
                or TaskTypes.MiraLaboratorySortSamples
                or TaskTypes.MiraAdminPrimeShields
                or TaskTypes.MiraCafeteriaEmptyGarbage
                or TaskTypes.MiraBalconyMeasureWeather
                or TaskTypes.MiraCafeteriaBuyBeverage
                or TaskTypes.MiraOfficeProcessData
                or TaskTypes.MiraLaunchpadRunDiagnostics
                or TaskTypes.MiraReactorUnlockManifolds
                or TaskTypes.PolusO2MonitorTree
                or TaskTypes.PolusSpecimenRoomUnlockManifolds
                or TaskTypes.PolusSpecimenRoomStoreArtifacts
                or TaskTypes.PolusO2FillCanisters
                or TaskTypes.PolusO2EmptyGarbage
                or TaskTypes.PolusDropshipChartCourse
                or TaskTypes.PolusMedbaySubmitScan
                or TaskTypes.PolusWeaponsClearAsteroids
                or TaskTypes.PolusOutsideFixWeatherNodeNode_CA
                or TaskTypes.PolusOutsideFixWeatherNodeNode_MLG
                or TaskTypes.PolusLaboratoryAlignTelescope
                or TaskTypes.PolusLaboratoryRepairDrill
                or TaskTypes.PolusLaboratoryRecordTemperature
                or TaskTypes.PolusOutsideRecordTemperature
                => TaskCategories.ShortTask,

                _ => throw new ArgumentException($"Invalid task type {taskType}")
            };
        }

        public static bool IsVisual(this TaskTypes taskType)
        {
            return taskType switch
            {
                TaskTypes.SkeldWeaponsClearAsteroids
                or TaskTypes.SkeldMedbaySubmitScan
                or TaskTypes.SkeldO2EmptyChute
                or TaskTypes.SkeldCafeteriaEmptyGarbage
                or TaskTypes.SkeldShieldsPrimeShields
                or TaskTypes.MiraMedbaySubmitScan
                or TaskTypes.PolusMedbaySubmitScan
                or TaskTypes.PolusWeaponsClearAsteroids
                => true,

                _ => false,
            };
        }
    }
}
