using System.Linq;
using Verse;

namespace DestroyItem
{
    [StaticConstructorOnStartup]
    public static class Startup
    {
        static Startup()
        {
            Utility.Log($"Total {DefDatabase<ThingDef>.DefCount.ToStringCached()} ThingDefs found.");
            int patched = 0;
            foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs.Where(def => typeof(ThingWithComps).IsAssignableFrom(def.thingClass) && def.category == ThingCategory.Item && def.destroyable))
            {
                def.comps.Add(new CompProperties(typeof(CompDestructible)));
                patched++;
            }
            Utility.Log($"{patched.ToStringCached()} ThingDefs patched.");
        }
    }
}
