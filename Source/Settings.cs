using Verse;

namespace DestroyItem
{
    public class Settings : ModSettings
    {
        public static float destructionSpeed = 1;
        public static bool instantDestruction = false;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref destructionSpeed, "destructionSpeed", 1);
            Scribe_Values.Look(ref instantDestruction, "instantDestruction", false);
        }
    }
}
