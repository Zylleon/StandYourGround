using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;
using RimWorld;
using Verse.Sound;

namespace StandYourGround
{
    public class StandYourGroundSettings : ModSettings
    {
        public static RimWorld.HostilityResponseMode violentDefault = RimWorld.HostilityResponseMode.Attack;
        public static RimWorld.HostilityResponseMode pacifistDefault = RimWorld.HostilityResponseMode.Flee;

        public static RimWorld.HostilityResponseMode childDefault = RimWorld.HostilityResponseMode.Flee;

        public static bool flagAreaRestriction = true;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref flagAreaRestriction, "flagAreaRestriction", true);
            Scribe_Values.Look(ref violentDefault, "violentDefault", RimWorld.HostilityResponseMode.Attack);
            Scribe_Values.Look(ref pacifistDefault, "pacifistDefault", RimWorld.HostilityResponseMode.Flee);
            Scribe_Values.Look(ref childDefault, "childDefault", RimWorld.HostilityResponseMode.Flee);
        }
    }


    public class StandYourGroundMod : Mod
    {
        StandYourGroundSettings settings;

        public StandYourGroundMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<StandYourGroundSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            inRect.width = 450f;
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);

            listing.Label("ZSYG_DefaultHostilityResponse".Translate());

            listing.Gap();

            List <RimWorld.HostilityResponseMode> responses = new List<RimWorld.HostilityResponseMode>();
            responses.Add(RimWorld.HostilityResponseMode.Attack);
            responses.Add(RimWorld.HostilityResponseMode.Flee);
            responses.Add(RimWorld.HostilityResponseMode.Ignore);

            //RimWorld.HostilityResponseModeUtility.GetLabel(RimWorld.HostilityResponseMode.Attack);
            if(listing.ButtonTextLabeled("ZSYG_Default".Translate(), RimWorld.HostilityResponseModeUtility.GetLabel(StandYourGroundSettings.violentDefault)))
            {
                List<FloatMenuOption> responseList = new List<FloatMenuOption>();
                foreach(RimWorld.HostilityResponseMode h in responses)
                {
                    responseList.Add(new FloatMenuOption(RimWorld.HostilityResponseModeUtility.GetLabel(h), delegate { StandYourGroundSettings.violentDefault = h; }));
                }
                Find.WindowStack.Add(new FloatMenu(responseList));

            }

            List<RimWorld.HostilityResponseMode> paxResponses = new List<RimWorld.HostilityResponseMode>();
            paxResponses.Add(RimWorld.HostilityResponseMode.Flee);
            paxResponses.Add(RimWorld.HostilityResponseMode.Ignore);

            if (listing.ButtonTextLabeled("ZSYG_Pacifist".Translate(), RimWorld.HostilityResponseModeUtility.GetLabel(StandYourGroundSettings.pacifistDefault)))
            {
                List<FloatMenuOption> responseList = new List<FloatMenuOption>();
                foreach (RimWorld.HostilityResponseMode h in paxResponses)
                {
                    responseList.Add(new FloatMenuOption(RimWorld.HostilityResponseModeUtility.GetLabel(h), delegate { StandYourGroundSettings.pacifistDefault = h; }));
                }
                Find.WindowStack.Add(new FloatMenu(responseList));

            }


            if (listing.ButtonTextLabeled("ZSYG_Children".Translate(), RimWorld.HostilityResponseModeUtility.GetLabel(StandYourGroundSettings.childDefault)))
            {
                List<FloatMenuOption> responseList = new List<FloatMenuOption>();
                foreach (RimWorld.HostilityResponseMode h in responses)
                {
                    responseList.Add(new FloatMenuOption(RimWorld.HostilityResponseModeUtility.GetLabel(h), delegate { StandYourGroundSettings.childDefault = h; }));
                }
                Find.WindowStack.Add(new FloatMenu(responseList));

            }


            listing.GapLine();
            listing.Gap();

            listing.Label("ZSYG_ApplyDefaultsDesc".Translate());

            listing.Gap();
            if (listing.ButtonText("ZSYG_ApplyDefaults".Translate(), RimWorld.HostilityResponseModeUtility.GetLabel(StandYourGroundSettings.childDefault)))
            {
                SoundDefOf.Click.PlayOneShotOnCamera(null);
                int numApplied = 0;
                foreach (Pawn pawn in PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_FreeColonists)
                {
                    if (pawn.RaceProps.Humanlike && !pawn.Dead && pawn.Faction?.IsPlayer == true)
                    {
                        if (pawn.ageTracker.CurLifeStage.developmentalStage.Juvenile())
                        {
                            pawn.playerSettings.hostilityResponse = StandYourGroundSettings.childDefault;
                        }
                        else if (pawn.WorkTagIsDisabled(WorkTags.Violent))
                        {
                            pawn.playerSettings.hostilityResponse = StandYourGroundSettings.pacifistDefault;

                        }
                        else
                        {
                            pawn.playerSettings.hostilityResponse = StandYourGroundSettings.violentDefault;
                        }
                        numApplied++;
                    }

                }

                Messages.Message(string.Format("ZYSG_ApplyDefaultsSuccess".Translate(), numApplied), MessageTypeDefOf.TaskCompletion, false);
            }

            listing.End();
        }

        public override string SettingsCategory()
        {
            return "ZSYG_ModName".Translate();
        }
    }
}
