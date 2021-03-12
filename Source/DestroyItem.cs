using System.Collections.Generic;
using System.Linq;
using Verse;

namespace DestroyItem
{
    [StaticConstructorOnStartup]
    public static class DestroyItem
    {
        static DestroyItem()
        {
            Utility.Log($"Total {DefDatabase<ThingDef>.DefCount} ThingDefs found.");
            List<ThingDef> thingDefs = DefDatabase<ThingDef>.AllDefs.Where(def => typeof(ThingWithComps).IsAssignableFrom(def.thingClass) && def.category == ThingCategory.Item && def.destroyable).ToList();
            int patched = 0;
            foreach (ThingDef def in thingDefs)
            {
                def.comps.Add(new CompProperties(typeof(CompDestructible)));
                if (def.HasComp(typeof(CompDestructible)))
                    patched++;
                else Utility.Log($"Error: Could not add CompDestructible to {def.defName} ({def.thingClass.Name})!", LogLevel.Error);
            }
            Utility.Log($"{patched} out of {thingDefs.Count} eligible ThingDefs patched.");
            
        }
    }
}
