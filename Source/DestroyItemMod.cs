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
                content.Label(
                    $"Destruction speed: {Settings.destructionSpeed.ToStringPercent()}",
                    tooltip: "Relative speed of item destruction. Actual speed depends on the pawn's melee damage and general labor speed.");
                destructionSpeedPower = (float)Math.Round(content.Slider(destructionSpeedPower, -1, 1), 2);
                Settings.destructionSpeed = Mathf.Pow(10, destructionSpeedPower);
            }
            content.End();
        }

        public override string SettingsCategory() => "Destroy Item";
    }
}
