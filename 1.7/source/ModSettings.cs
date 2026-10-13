using Verse;

namespace Pawn_Perspective
{
    public class ModSettings : Verse.ModSettings
    {
        public static bool useAlternativeButtonPosition = false;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref useAlternativeButtonPosition, "useAlternativeButtonPosition", false);
        }
    }
}
