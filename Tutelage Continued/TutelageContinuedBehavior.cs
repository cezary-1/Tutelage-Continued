using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace TutelageContinued
{
    public class TutelageContinuedBehavior : CampaignBehaviorBase
    {
        public List<SkillObject> skillObjects = new List<SkillObject>();

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener(this, OnDailyTickParty);
            CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, OnDailyTickSettlement);
        }

        private void OnDailyTickParty(MobileParty mobileParty)
        {
            if (skillObjects == null || skillObjects.Count == 0) PopulateSkills();

            var s = TutelageContinuedSettings.Instance;
            if (s == null || mobileParty == null) return;
            if (mobileParty.LeaderHero == null) return;
            if (!s.AffectPlayer && mobileParty.ActualClan == Clan.PlayerClan) return;
            if (!s.AffectNpc && mobileParty.ActualClan != Clan.PlayerClan) return;

            var troopRoster = mobileParty?.MemberRoster?.GetTroopRoster().ToList();

            var heroes = new List<Hero>();
            heroes.AddRange(troopRoster.Where(t => t.Character.IsHero).Select(t => t.Character.HeroObject));

            if (heroes == null || heroes.Count <= 0) return;

            var teachers = new List<CharacterObject>();
            teachers.AddRange(troopRoster.Where(t => !s.OnlyHeroes || t.Character.IsHero).Select(t => t.Character));

            if (s.Army && mobileParty.Army != null)
            {
                foreach (var armyParty in mobileParty.Army.Parties)
                {
                    if (armyParty == mobileParty) continue;

                    var armyPartyRoster = armyParty.MemberRoster?.GetTroopRoster().ToList();
                    teachers.AddRange(armyPartyRoster.Where(t => !s.OnlyHeroes || t.Character.IsHero).Select(t => t.Character));
                }
            }

            if (teachers == null || teachers.Count <= 0) return;


            heroes = heroes.Distinct().ToList();
            teachers = teachers.Distinct().ToList();

            var gainedXP = new Dictionary<Hero, List<SkillObject>>();

            foreach(var hero in heroes)
            {
                if (MBRandom.RandomFloat >= s.Chance) continue;

                foreach (var skill in skillObjects)
                {
                    var heroSkill = hero.GetSkillValue(skill);
                    var teacher = teachers.Where(t =>
                    {
                        bool same = s.TeacherSame ? t.GetSkillValue(skill) >= heroSkill : t.GetSkillValue(skill) > heroSkill;

                        if (t != hero.CharacterObject && same && t.GetSkillValue(skill) >= s.Min) return true;
                        return false;
                    }).FirstOrDefault();

                    if(teacher == null ) continue;

                    var teacherSkill = teacher.GetSkillValue(skill);

                    float bonus = teacherSkill;
                    if (s.Substraction) bonus -= heroSkill;
                    bonus *= s.Multipler;

                    if (bonus != 0)
                    {
                        var heroDev = hero.HeroDeveloper;
                        if (heroDev == null) continue;
                        heroDev.AddSkillXp(skill, bonus, s.LearnRate, true);
                    }
                    
                    if (s.TeacherMult != 0 && teacher.IsHero)
                    {
                        
                        var teachHero = teacher.HeroObject;

                        if (gainedXP.TryGetValue(teachHero, out var skillList))
                        {
                            if (!s.TeacherForeach && skillList != null && skillList.Contains(skill)) continue;
                        }

                        var teachBonus = bonus * s.TeacherMult;
                        if (teachBonus != 0)
                        {
                            var teachDev = teachHero.HeroDeveloper;
                            if (teachDev == null) continue;
                            teachDev.AddSkillXp(skill, teachBonus, s.LearnRate, true);

                            var newSkillList = new List<SkillObject> { skill };

                            if (!s.TeacherForeach && skillList != null)
                            {
                                newSkillList.AddRange(skillList);
                                
                                gainedXP[teachHero] = newSkillList;
                            }
                        }

                    }

                }

            }

            
        }

        private void OnDailyTickSettlement(Settlement settlement)
        {
            if (skillObjects == null || skillObjects.Count == 0) PopulateSkills();

            var s = TutelageContinuedSettings.Instance;
            if (s == null || settlement == null) return;
            if (!settlement.IsFortification || settlement.OwnerClan == null) return;
            if (!s.AffectPlayer && settlement.OwnerClan == Clan.PlayerClan) return;
            if (!s.AffectNpc && settlement.OwnerClan != Clan.PlayerClan) return;


            var teachers = new List<CharacterObject>();
            var heroes = new List<Hero>();

            var garrisonRoster = settlement?.Town?.GarrisonParty?.MemberRoster?.GetTroopRoster();
            if( garrisonRoster != null && garrisonRoster.Count > 0)
            {
                teachers.AddRange(garrisonRoster.Select(t=> t.Character));
            }

            var heroesIn = settlement.HeroesWithoutParty
                .Where(h => h.MapFaction != null && h.MapFaction == settlement.MapFaction)
                .Where(h => s.AffectPlayer || h.Clan != Clan.PlayerClan)
                .Where(h => s.AffectNpc || h.Clan == Clan.PlayerClan)
                .ToList();
            if(heroesIn != null && heroesIn.Count > 0)
            {
                teachers.AddRange(heroesIn.Select(h=>h.CharacterObject));
                heroes.AddRange(heroesIn);
            }
            var parties = settlement.Parties
                .Where(p => p.LeaderHero != null && p.MapFaction != null && p.MapFaction == settlement.MapFaction)
                .Where(p => s.AffectPlayer || p.ActualClan != Clan.PlayerClan)
                .Where(p => s.AffectNpc || p.ActualClan == Clan.PlayerClan)
                .ToList();
            foreach (var party in parties)
            {
                var roster = party?.MemberRoster?.GetTroopRoster();

                var partyTeachers = roster.Where(t => !s.OnlyHeroes || t.Character.IsHero).Select(t => t.Character).ToList();
                if (partyTeachers != null && partyTeachers.Count > 0)
                    teachers.AddRange(partyTeachers);

                var partyHeroes = roster.Where(t => t.Character.IsHero).Select(t => t.Character.HeroObject).ToList();
                if (partyHeroes != null && partyHeroes.Count > 0)
                {
                    teachers.AddRange(partyHeroes.Select(h=> h.CharacterObject));
                    heroes.AddRange(partyHeroes);
                }
                    
            }

            if (heroes == null || heroes.Count <= 0) return;
            if (teachers == null || teachers.Count <= 0) return;

            heroes = heroes.Distinct().ToList();
            teachers = teachers.Distinct().ToList();

            var gainedXP = new Dictionary<Hero, List<SkillObject>>();

            foreach (var hero in heroes)
            {
                if(MBRandom.RandomFloat >= s.Chance) continue;

                foreach (var skill in skillObjects)
                {
                    var heroSkill = hero.GetSkillValue(skill);
                    var teacher = teachers.Where(t =>
                    {
                        bool same = s.TeacherSame ? t.GetSkillValue(skill) >= heroSkill : t.GetSkillValue(skill) > heroSkill;

                        if (t != hero.CharacterObject && same && t.GetSkillValue(skill) >= s.Min) return true;
                        return false;
                    }).FirstOrDefault();
                    if (teacher == null) continue;

                    var teacherSkill = teacher.GetSkillValue(skill);

                    float bonus = teacherSkill;
                    if (s.Substraction) bonus -= heroSkill;
                    bonus *= s.Multipler;

                    if (bonus != 0)
                    {
                        var heroDev = hero.HeroDeveloper;
                        if (heroDev == null) continue;
                        heroDev.AddSkillXp(skill, bonus, s.LearnRate, true);
                    }

                    if(s.TeacherMult != 0 && teacher.IsHero)
                    {
                        var teachHero = teacher.HeroObject;

                        if (gainedXP.TryGetValue(teachHero, out var skillList))
                        {
                            if (!s.TeacherForeach && skillList != null && skillList.Contains(skill)) continue;
                        }

                        var teachBonus = bonus * s.TeacherMult;
                        if(teachBonus != 0)
                        {
                            var teachDev = teachHero.HeroDeveloper;
                            if(teachDev == null) continue;
                            teachDev.AddSkillXp(skill, teachBonus, s.LearnRate, true);

                            var newSkillList = new List<SkillObject> { skill };

                            if (!s.TeacherForeach && skillList != null)
                            {
                                newSkillList.AddRange(skillList);

                                gainedXP[teachHero] = newSkillList;
                            }

                        }

                    }

                }

            }


        }

        private void PopulateSkills()
        {
            skillObjects.Clear();

            skillObjects = MBObjectManager.Instance.GetObjectTypeList<SkillObject>();
        }

        public override void SyncData(IDataStore dataStore) { }
    }
}
