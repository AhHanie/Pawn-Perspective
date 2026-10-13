using UnityEngine;
using Verse;

namespace Pawn_Perspective
{
    public static class ModSettingsWindow
    {
        public static void Draw(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);
            listing.CheckboxLabeled(
                "PawnPerspective.Settings.UseAlternativeButtonPosition".Translate(),
                ref ModSettings.useAlternativeButtonPosition,
                "PawnPerspective.Settings.UseAlternativeButtonPosition.Tooltip".Translate());
            listing.End();
        }
    }
}
