using UnityEngine;
using Verse;

namespace DestroyItem
{
    public class Designator_DestroyItem : Designator
    {
        public override int DraggableDimensions => 2;

        public Designator_DestroyItem()
        {
            defaultLabel = "Destroy Item";
            defaultDesc = "Order a pawn to destroy this item";
            icon = ContentFinder<Texture2D>.Get("Command");
        }

        protected override DesignationDef Designation => DestroyItemDefOf.Designation_DestroyItem;

        public override AcceptanceReport CanDesignateCell(IntVec3 loc)
        {
            if (!loc.InBounds(Map))
                return false;
            if (!DebugSettings.godMode && loc.Fogged(Map))
                return false;
            return DestructibleInCell(loc) != null;
        }

        public override void DesignateSingleCell(IntVec3 c) => DesignateThing(DestructibleInCell(c));

        public override AcceptanceReport CanDesignateThing(Thing t) => t.def.HasComp(typeof(CompDestructible));// && (t.Faction == null || t.Faction.IsPlayer);

        public override void DesignateThing(Thing t)
        {
            Utility.Log($"Designating {t.LabelCap} ({t.def.defName}) for destruction.");
            Map.designationManager.AddDesignation(new Designation(t, Designation));
        }

        Thing DestructibleInCell(IntVec3 loc)
        {
            foreach (Thing thing in loc.GetThingList(Map))
            {
                //Utility.Log($"Thing {thing.LabelCap} is at {loc}.");
                if (CanDesignateThing(thing).Accepted && Map.designationManager.DesignationOn(thing, Designation) == null)
                {
                    //Utility.Log($"{thing.LabelCap} @ {loc} is destructible.");
                    return thing;
                }
            }
            return null;
        }
    }
}
