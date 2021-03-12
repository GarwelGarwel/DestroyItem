using RimWorld;
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
            useMouseIcon = true;
            soundDragSustain = SoundDefOf.Designate_DragStandard;
            soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
            soundSucceeded = SoundDefOf.Designate_Deconstruct;
        }

        protected override DesignationDef Designation => DestroyItemDefOf.Designation_DestroyItem;

        public override AcceptanceReport CanDesignateCell(IntVec3 loc)
        {
            if (!loc.InBounds(Map) || !DebugSettings.godMode && loc.Fogged(Map))
                return false;
            return DestructibleInCell(loc) != null;
        }

        public override void DesignateSingleCell(IntVec3 c) => DesignateThing(DestructibleInCell(c));

        public override AcceptanceReport CanDesignateThing(Thing t) => t.def.HasComp(typeof(CompDestructible)) && !t.IsDesignatedForDestruction();

        public override void DesignateThing(Thing t)
        {
            t.DesignateForDestruction();
        }

        Thing DestructibleInCell(IntVec3 loc) => loc.GetThingList(Map).FirstOrFallback(thing => CanDesignateThing(thing).Accepted);
    }
}
