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
        public static readonly float InfoButtonX = 304f;
        public static readonly float InfoButtonY = 3f;
        public static readonly float InfoButtonSize = 24f;
        public static readonly float IconButtonSize = 48f;
        public static readonly float IconButtonGap = 2f;
        public static readonly float AlternativeButtonY = -9f;

        public static Rot4 currentRotation = Rot4.South;

        // True only while Page_ConfigureStartingPawns is inside its StartingPawnUtility.DrawPortraitArea call.
        public static bool drawingStartingPawnPortraitArea;

        public static Rot4 CurrentRotation
        {
            get
            {
                return currentRotation;
            }
        }

        public static void DrawControl(Rect creationRect)
        {
            Rect buttonRect;
            if (ModSettings.useAlternativeButtonPosition)
            {
                buttonRect = new Rect(
                    creationRect.center.x + StartingPawnUtility.PawnPortraitSize.x / 2f + IconButtonGap,
                    creationRect.y + AlternativeButtonY,
                    IconButtonSize,
                    IconButtonSize);
            }
            else
            {
                buttonRect = new Rect(
                    creationRect.x + InfoButtonX + InfoButtonSize + IconButtonGap,
                    creationRect.y + InfoButtonY + (InfoButtonSize - IconButtonSize) / 2f,
                    IconButtonSize,
                    IconButtonSize);
            }

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

    // StartingPawnUtility.DrawPortraitArea is also drawn by Dialog_ChooseNewWanderers, so mark the one call made by
    // the starting-pawn page. The flag only lives for the duration of that call and cannot outlive the page.
    [HarmonyPatch(typeof(Page_ConfigureStartingPawns), nameof(Page_ConfigureStartingPawns.DoWindowContents))]
    internal static class Page_ConfigureStartingPawns_DoWindowContents_Patch
    {
        [HarmonyPrefix]
        [HarmonyInfix(typeof(StartingPawnUtility), nameof(StartingPawnUtility.DrawPortraitArea))]
        private static void BeginPortraitArea()
        {
            StartingPawnPortraitRotation.drawingStartingPawnPortraitArea = true;
        }

        [HarmonyPostfix]
        [HarmonyInfix(typeof(StartingPawnUtility), nameof(StartingPawnUtility.DrawPortraitArea))]
        private static void EndPortraitArea()
        {
            StartingPawnPortraitRotation.drawingStartingPawnPortraitArea = false;
        }
    }

    // DrawPortraitArea makes exactly one PortraitsCache.Get call (the 92 x 128 portrait) and one DrawCharacterCard call,
    // so wrapping those calls touches nothing else and needs no size or styling-station filter.
    [HarmonyPatch(typeof(StartingPawnUtility), nameof(StartingPawnUtility.DrawPortraitArea))]
    internal static class StartingPawnUtility_DrawPortraitArea_Patch
    {
        [HarmonyPrefix]
        [HarmonyInfix(typeof(PortraitsCache), nameof(PortraitsCache.Get))]
        private static void RotatePortrait(ref Rot4 rotation)
        {
            if (StartingPawnPortraitRotation.drawingStartingPawnPortraitArea)
            {
                rotation = StartingPawnPortraitRotation.CurrentRotation;
            }
        }

        [HarmonyPostfix]
        [HarmonyInfix(typeof(CharacterCardUtility), nameof(CharacterCardUtility.DrawCharacterCard))]
        private static void DrawRotateButton(Rect creationRect)
        {
            if (StartingPawnPortraitRotation.drawingStartingPawnPortraitArea)
            {
                StartingPawnPortraitRotation.DrawControl(creationRect);
            }
        }
    }
}
