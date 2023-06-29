using System;
using UnityEngine;
using Verse;

namespace DestroyItem
{
    public class DestroyItemMod : Mod
    {
        float destructionSpeedPower;

        public DestroyItemMod(ModContentPack content)
            : base(content)
        {
            GetSettings<Settings>();
            destructionSpeedPower = (float)Math.Round(Mathf.Log10(Settings.destructionSpeed), 1);
        }

        public override void DoSettingsWindowContents(Rect rect)
        {
            Listing_Standard content = new Listing_Standard();
            content.Begin(rect);
            content.CheckboxLabeled("Instant destruction", ref Settings.instantDestruction, "Destroy items instantly.");
            if (!Settings.instantDestruction)
            {
                destructionSpeedPower = (float)Math.Round(content.SliderLabeled(
                    $"Destruction speed: {Settings.destructionSpeed.ToStringPercent()}",
                    destructionSpeedPower,
                    -1,
                    1,
                    0.30f,
                    "Relative speed of item destruction. Actual speed depends on the pawn's melee damage and general labor speed."), 2);
                Settings.destructionSpeed = Mathf.Pow(10, destructionSpeedPower);
                Settings.maxDestroyers = Mathf.RoundToInt(content.SliderLabeled(
                    $"Max simultaneous destroyers: {Settings.maxDestroyers.ToStringCached()}",
                    Settings.maxDestroyers,
                    1,
                    8,
                    0.30f,
                    "Max number of pawns that can be destroying the same item together. Default: 2."));
            }
            content.End();
        }

        public override string SettingsCategory() => "Destroy Item";
    }
}
