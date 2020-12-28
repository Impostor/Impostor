namespace Impostor.Api.Innersloth
{
    // Could there be a better implementation for this?
    public enum DecontamDoors : byte
    {
        None = 0,

        /// <summary> Internal value: 1 </summary>
        MiraHQ_Decontam_EnterFrom_LockerRoom = 1,

        /// <summary> Internal value: 2 </summary>
        MiraHQ_Decontam_EnterFrom_Reactor = 2,

        /// <summary> Internal value: 3 </summary>
        MiraHQ_Decontam_ExitTo_Reactor = 3,

        /// <summary> Internal value: 4 </summary>
        MiraHQ_Decontam_ExitTo_LockerRoom = 4,

        /// <summary> Internal value: 1 </summary>
        Polus_TopDecontam_EnterFrom_Specimen = 5,

        /// <summary> Internal value: 2 </summary>
        Polus_TopDecontam_EnterFrom_Laboratory = 6,

        /// <summary> Internal value: 3 </summary>
        Polus_TopDecontam_ExitTo_Laboratory = 7,

        /// <summary> Internal value: 4 </summary>
        Polus_TopDecontam_ExitTo_Specimen = 8,

        /// <summary> Internal value: 1 </summary>
        Polus_BottomDecontam_EnterFrom_Admin = 9,

        /// <summary> Internal value: 2 </summary>
        Polus_BottomDecontam_EnterFrom_Specimen = 10,

        /// <summary> Internal value: 3 </summary>
        Polus_BottomDecontam_ExitTo_Specimen = 11,

        /// <summary> Internal value: 4 </summary>
        Polus_BottomDecontam_ExitTo_Admin = 12,
    }
}