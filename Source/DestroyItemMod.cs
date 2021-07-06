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
            => destructionSpeedPower = (float)Math.Round(Mathf.Log10(Settings.destructionSpeed), 1);

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard content = new Listing_Standard();
            content.Begin(inRect);
            content.CheckboxLabeled("Instant destruction", ref Settings.instantDestruction, "Destroy all items instantly.");
            if (!Settings.instantDestruction)
            {
                content.Label($"Destruction Speed: {Settings.destructionSpeed.ToStringPercent()}", tooltip: "Relative speed of item destruction. Actual speed depends on the pawn's melee damage and general labor speed.");
                destructionSpeedPower = (float)Math.Round(content.Slider(destructionSpeedPower, -1, 1), 1);
                Settings.destructionSpeed = Mathf.Pow(10, destructionSpeedPower);
            }
            content.End();
        }

        public override string SettingsCategory() => "Destroy Item";
    }
}
