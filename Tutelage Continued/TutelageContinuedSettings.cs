using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using TaleWorlds.Localization;

namespace TutelageContinued
{
    public sealed class TutelageContinuedSettings : AttributeGlobalSettings<TutelageContinuedSettings>
    {
        public override string Id => "TutelageContinued";
        public override string DisplayName => new TextObject("{=TN_TITLE}Tutelage Continued").ToString();
        public override string FolderName => "TutelageContinuedSettings";
        public override string FormatType => "json";

        [SettingPropertyBool(
            "{=TN_Player}Affect Player?",
            Order = 0, RequireRestart = false,
            HintText = "{=TN_Player_H}If true, mod apply to player parties/settlements. (Default: true)")]
        [SettingPropertyGroup("{=MCM_GENERAL}General")]
        public bool AffectPlayer { get; set; } = true;

        [SettingPropertyBool(
            "{=TN_Npc}Affect Npc?",
            Order = 1, RequireRestart = false,
            HintText = "{=TN_Npc_H}If true, mod apply to npc parties/settlements. (Default: true)")]
        [SettingPropertyGroup("{=MCM_GENERAL}General")]
        public bool AffectNpc { get; set; } = true;

        [SettingPropertyFloatingInteger(
            "{=TN_Multipler}Teaching Bonus Multipler?", 0f, 100f, 
            Order = 2, RequireRestart = false,
            HintText = "{=TN_Multipler_H}Multipler applied to teacher skill - learner (e.g (300 - 150) * value = bonus xp). (Default: 0.1)")]
        [SettingPropertyGroup("{=MCM_GENERAL}General")]
        public float Multipler { get; set; } = 0.1f;

        [SettingPropertyBool(
            "{=TN_OnlyHeroes}Only Heroes Teach",
            Order = 3, RequireRestart = false,
            HintText = "{=TN_OnlyHeroes_H}If true, only heroes will teach. No troops. (Default: false)")]
        [SettingPropertyGroup("{=MCM_GENERAL}General")]
        public bool OnlyHeroes { get; set; } = false;

        [SettingPropertyInteger(
            "{=TN_MinSkill}Minimum Skill Value", 0, 300,
            Order = 4, RequireRestart = false,
            HintText = "{=TN_MinSkill_H}Minimum skill value needed to teach. (Default: 100)")]
        [SettingPropertyGroup("{=MCM_GENERAL}General")]
        public int Min { get; set; } = 100;

        [SettingPropertyFloatingInteger(
            "{=TN_Chance}Chance Getting Xp", 0f, 1f, "#0%",
            Order = 5, RequireRestart = false,
            HintText = "{=TN_Chance_H}Chance to get bonus xp. (Default: 50%)")]
        [SettingPropertyGroup("{=MCM_GENERAL}General")]
        public float Chance { get; set; } = 0.5f;

        [SettingPropertyBool(
            "{=TN_LearningRate}Affected by Learning Rate Factor?",
            Order = 6, RequireRestart = false,
            HintText = "{=TN_LearningRate_H}If true, bonus xp will be affected by learning rate factor. (Default: true)")]
        [SettingPropertyGroup("{=MCM_GENERAL}General")]
        public bool LearnRate { get; set; } = true;

        [SettingPropertyBool(
            "{=TN_Substraction}Teacher Skill Substraction",
            Order = 7, RequireRestart = false,
            HintText = "{=TN_Substraction_H}If false, teacher skill substraction (teacher skill - learner skill) will be turned off. (Default: true)")]
        [SettingPropertyGroup("{=MCM_GENERAL}General")]
        public bool Substraction { get; set; } = true;

        [SettingPropertyFloatingInteger(
            "{=TN_TeacherMult}Teacher Xp Bonus Gain Mult", 0f, 100f,
            Order = 8, RequireRestart = false,
            HintText = "{=TN_TeacherMult_H}Multipler applied to bonus for teacher (teaching xp bonus multipled by this value). Set to 0 to disable teacher gain bonus xp for teaching. (Default: 0.1)")]
        [SettingPropertyGroup("{=MCM_GENERAL}General")]
        public float TeacherMult { get; set; } = 0.1f;

        [SettingPropertyBool(
            "{=TN_TeacherForeach}Teacher Gain XP For Each",
            Order = 9, RequireRestart = false,
            HintText = "{=TN_TeacherForeach_H}If true, teacher will get xp for each hero they taught. (Default: false)")]
        [SettingPropertyGroup("{=MCM_GENERAL}General")]
        public bool TeacherForeach { get; set; } = false;

        [SettingPropertyBool(
            "{=TN_TeacherSame}Teacher With Same Skill",
            Order = 9, RequireRestart = false,
            HintText = "{=TN_TeacherSame_H}If true, hero with the same skill amount as learner can teach. (Default: false)")]
        [SettingPropertyGroup("{=MCM_GENERAL}General")]
        public bool TeacherSame { get; set; } = false;

        [SettingPropertyBool(
            "{=TN_Army}Army Parties Included?",
            Order = 10, RequireRestart = false,
            HintText = "{=TN_Army_H}If true, parties are being taught and are teaching other heroes from the same army but another party. (Default: true)")]
        [SettingPropertyGroup("{=MCM_GENERAL}General")]
        public bool Army { get; set; } = true;

    }
}
