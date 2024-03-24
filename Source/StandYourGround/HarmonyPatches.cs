using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using HarmonyLib;
using RimWorld;

namespace StandYourGround
{
    public static class HarmonyPatches
    {
        [StaticConstructorOnStartup]
        public static class StandYourGround
        {
            static StandYourGround()
            {
                Harmony harmony = new Harmony("zylle.StandYourground");
                Log.Message("Initializing Stand Your Ground");
                harmony.PatchAll();
            }
           
        }
    }


    [HarmonyPatch(typeof(RimWorld.PawnComponentsUtility), "AddAndRemoveDynamicComponents")]
    internal static class HostilityResponsePatch
    {
        static void Postfix(Pawn pawn)
        {
            if (pawn.RaceProps.Humanlike && !pawn.Dead && pawn.Faction?.IsPlayer == true)
            {
                if (!PawnUtility.EverBeenColonistOrTameAnimal(pawn))
                {
                    if(pawn.ageTracker.CurLifeStage == LifeStageDefOf.HumanlikeChild)
                    {
                        pawn.playerSettings.hostilityResponse = StandYourGroundSettings.childDefault;
                    }
                    if (pawn.WorkTagIsDisabled(WorkTags.Violent))
                    {
                        pawn.playerSettings.hostilityResponse = StandYourGroundSettings.pacifistDefault;

                    }
                    else
                    {
                        pawn.playerSettings.hostilityResponse = StandYourGroundSettings.violentDefault;
                    }
                }

            }
        }
    }


    [HarmonyPatch(typeof(RimWorld.LifeStageWorker_HumanlikeAdult), "Notify_LifeStageStarted")]
    internal static class AdultResponsePatch
    {
        static void Postfix(Pawn pawn, LifeStageDef previousLifeStage)
        {
            if (PawnUtility.EverBeenColonistOrTameAnimal(pawn))
            {
                if (previousLifeStage != null)
                {
                    if (previousLifeStage?.developmentalStage.Juvenile() == true)
                    {
                        if (pawn.WorkTagIsDisabled(WorkTags.Violent))
                        {
                            pawn.playerSettings.hostilityResponse = StandYourGroundSettings.pacifistDefault;

                        }
                        else
                        {
                            pawn.playerSettings.hostilityResponse = StandYourGroundSettings.violentDefault;
                        }
                    }
                }
            }
        }
    }



    [HarmonyPatch(typeof(RimWorld.LifeStageWorker_HumanlikeChild), "Notify_LifeStageStarted")]
    internal static class ChildResponsePatch
    {
        static void Postfix(Pawn pawn, LifeStageDef previousLifeStage)
        {
            if (previousLifeStage != null && previousLifeStage.developmentalStage.Baby())
            {
                if (pawn.WorkTagIsDisabled(WorkTags.Violent))
                {
                    pawn.playerSettings.hostilityResponse = StandYourGroundSettings.pacifistDefault;
                }
                else
                {
                    pawn.playerSettings.hostilityResponse = StandYourGroundSettings.childDefault;
                }
            }
        }
    }


}
