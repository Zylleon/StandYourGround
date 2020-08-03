using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using HarmonyLib;

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
                if (pawn.WorkTagIsDisabled(WorkTags.Violent))
                {
                    //pawn.playerSettings.hostilityResponse = RimWorld.HostilityResponseMode.Ignore;
                    pawn.playerSettings.hostilityResponse = StandYourGroundSettings.pacifistDefault;

                }
                else
                {
                    pawn.playerSettings.hostilityResponse = StandYourGroundSettings.violentDefault;
                    //pawn.playerSettings.hostilityResponse = RimWorld.HostilityResponseMode.Attack;
                }

                //if (StandYourGroundSettings.flagAreaRestriction)
                //{
                //    if (pawn.Map != null)
                //    {
                //        Log.Message("... checking area");

                //        List<Pawn> colonists = pawn.Map.mapPawns.FreeColonists;
                //        if (colonists.Count > 2)
                //        {
                //            //Area area = colonists.GroupBy(c => c.playerSettings.AreaRestriction)
                //            //     .Select(y => new { Area = y.Key, Count = y.Count() }).OrderByDescending(g => g.Count).First().Area;

                //            Area area = colonists.Where(c => c != pawn).GroupBy(c => c.playerSettings.AreaRestriction)
                //                 .Select(y => new { Area = y.Key, Count = y.Count() }).OrderByDescending(g => g.Count).First().Area;

                //            if (area != null)
                //            {
                //                Log.Message("Found area " + area.Label);
                //                Log.Message("Found area " + area);

                //                pawn.playerSettings.AreaRestriction = area;

                //                Log.Message("Pawn assigned to " + pawn.playerSettings.AreaRestriction.Label);
                //            }


                //        }
                //    }


                //}

            }
        }
    }

}
