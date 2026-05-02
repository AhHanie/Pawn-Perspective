using System;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Pawn_Perspective.Patches
{
    internal static class StartingPawnPortraitRotation
    {
        private const float InfoButtonX = 304f;
        private const float InfoButtonY = 3f;
        private const float InfoButtonSize = 24f;
        private const float IconButtonSize = 48f;
        private const float IconButtonGap = 2f;

        private static Rot4 currentRotation = Rot4.South;
        private static bool configureStartingPawnsOpen;

        public static Rot4 CurrentRotation
        {
            get
            {
                return currentRotation;
            }
        }

        public static bool ShouldRotatePortrait(Vector2 size, bool stylingStation)
        {
            return configureStartingPawnsOpen
                && stylingStation
                && Mathf.Approximately(size.x, StartingPawnUtility.PawnPortraitSize.x)
                && Mathf.Approximately(size.y, StartingPawnUtility.PawnPortraitSize.y);
        }

        public static bool ShouldDrawControl(Action randomizeCallback)
        {
            return configureStartingPawnsOpen && randomizeCallback != null;
        }

        public static void NotifyConfigureStartingPawnsOpened()
        {
            configureStartingPawnsOpen = true;
        }

        public static void NotifyConfigureStartingPawnsClosed()
        {
            configureStartingPawnsOpen = false;
        }

        public static void DrawControl(Rect creationRect)
        {
            Rect buttonRect = new Rect(
                creationRect.x + InfoButtonX + InfoButtonSize + IconButtonGap,
                creationRect.y + InfoButtonY + (InfoButtonSize - IconButtonSize) / 2f,
                IconButtonSize,
                IconButtonSize);

            if (Widgets.ButtonImage(buttonRect, Mod.rotateButtonTex.Texture, true, "PawnPerspective.RotatePortrait".Translate()))
            {
                CycleRotation();
                SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
            }
        }

        private static void CycleRotation()
        {
            if (currentRotation == Rot4.South)
            {
                currentRotation = Rot4.West;
                return;
            }
            if (currentRotation == Rot4.West)
            {
                currentRotation = Rot4.North;
                return;
            }
            if (currentRotation == Rot4.North)
            {
                currentRotation = Rot4.East;
                return;
            }
            currentRotation = Rot4.South;
        }
    }

    [HarmonyPatch(typeof(Page_ConfigureStartingPawns), nameof(Page_ConfigureStartingPawns.PreOpen))]
    internal static class Page_ConfigureStartingPawns_PreOpen_Patch
    {
        private static void Postfix()
        {
            StartingPawnPortraitRotation.NotifyConfigureStartingPawnsOpened();
        }
    }

    [HarmonyPatch(typeof(Page), "DoBack")]
    internal static class Page_DoBack_Patch
    {
        private static void Prefix(Page __instance)
        {
            if (__instance is Page_ConfigureStartingPawns)
            {
                StartingPawnPortraitRotation.NotifyConfigureStartingPawnsClosed();
            }
        }
    }

    [HarmonyPatch(typeof(Page), "DoNext")]
    internal static class Page_DoNext_Patch
    {
        private static void Prefix(Page __instance)
        {
            if (__instance is Page_ConfigureStartingPawns)
            {
                StartingPawnPortraitRotation.NotifyConfigureStartingPawnsClosed();
            }
        }
    }

    [HarmonyPatch(typeof(CharacterCardUtility), nameof(CharacterCardUtility.DrawCharacterCard))]
    internal static class CharacterCardUtility_DrawCharacterCard_Patch
    {
        private static void Postfix(Action randomizeCallback, Rect creationRect)
        {
            if (StartingPawnPortraitRotation.ShouldDrawControl(randomizeCallback))
            {
                StartingPawnPortraitRotation.DrawControl(creationRect);
            }
        }
    }

    [HarmonyPatch(typeof(PortraitsCache), nameof(PortraitsCache.Get))]
    internal static class PortraitsCache_Get_Patch
    {
        private static void Prefix(Vector2 size, bool stylingStation, ref Rot4 rotation)
        {
            if (StartingPawnPortraitRotation.ShouldRotatePortrait(size, stylingStation))
            {
                rotation = StartingPawnPortraitRotation.CurrentRotation;
            }
        }
    }
}
