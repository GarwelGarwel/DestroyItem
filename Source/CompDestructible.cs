using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace DestroyItem
{
    public class CompDestructible : ThingComp
    {
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            yield return new Command_Action
            {
                defaultLabel = "Destroy",
                defaultDesc = "Order a pawn to destroy this item",
                icon = ContentFinder<Texture2D>.Get("DestroyItem"),
                action = () => parent.Destroy()
            };
        }
    }
}
