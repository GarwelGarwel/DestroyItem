using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace DestroyItem
{
    public class CompDestructible : ThingComp
    {
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (parent.Map.designationManager.DesignationOn(parent, DestroyItemDefOf.Designation_DestroyItem) != null)
                yield break;
            yield return new Command_Action
            {
                defaultLabel = "Destroy",
                defaultDesc = "Order a pawn to destroy this item",
                icon = ContentFinder<Texture2D>.Get("Command"),
                action = () => parent.Map.designationManager.AddDesignation(new Designation(parent, DestroyItemDefOf.Designation_DestroyItem))
            };
        }
    }
}
