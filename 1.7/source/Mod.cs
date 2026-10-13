using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Pawn_Perspective
{
    public class Mod: Verse.Mod
    {
        public static CachedTexture rotateButtonTex = new CachedTexture("PawnPerspective/Rotate");
        public Mod(ModContentPack content) : base(content)
        {
            GetSettings<ModSettings>();
            LongEventHandler.QueueLongEvent(Init, "PawnPerspective.LoadingLabel", doAsynchronously: true, null);
        }

        public override string SettingsCategory()
        {
            return "PawnPerspective.Settings.Title".Translate();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            ModSettingsWindow.Draw(inRect);
        }

        private void Init()
        {
            new Harmony("rimworld.sk.pawnperspective").PatchAll();
        }
    }
}
