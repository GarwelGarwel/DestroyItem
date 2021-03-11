using System.Collections.Generic;
using System.Linq;
using Verse;

namespace DestroyItem
{
    public class CompDestructible : ThingComp
    {
        public override IEnumerable<Gizmo> CompGetGizmosExtra() => base.CompGetGizmosExtra();
    }
}
