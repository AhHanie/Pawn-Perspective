using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Pawn_Perspective
{
    public class Mod: Verse.Mod
    {
        public static CachedTexture rotateButtonTex = new CachedTexture("PawnPerspective/Rotate");
        public Mod(ModContentPack content) : base(content)
        {
            LongEventHandler.QueueLongEvent(Init, "PawnPerspective.LoadingLabel", doAsynchronously: true, null);
        }

        private void Init()
        {
            new Harmony("rimworld.sk.pawnperspective").PatchAll();
        }
    }
}
