﻿using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace DestroyItem
{
    public class Designator_DestroyItem : Designator
    {
        public override DrawStyleCategoryDef DrawStyleCategory => DrawStyleCategoryDefOf.Orders;

        protected override DesignationDef Designation => DestroyItemDefOf.Designation_DestroyItem;

        public Designator_DestroyItem()
        {
            defaultLabel = "Destroy Item";
            defaultDesc = "Order a pawn to destroy this item";
            icon = ContentFinder<Texture2D>.Get("Command");
            useMouseIcon = true;
            hotKey = DestroyItemDefOf.KeyBinding_DestroyItem;
            soundDragSustain = SoundDefOf.Designate_DragStandard;
            soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
            soundSucceeded = SoundDefOf.Designate_Deconstruct;
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 loc) =>
            loc.InBounds(Map) && (DebugSettings.godMode || !loc.Fogged(Map)) && DestructiblesInCell(loc).Any();

        public override void DesignateSingleCell(IntVec3 c)
        {
            foreach (Thing thing in DestructiblesInCell(c))
                DesignateThing(thing);
        }

        public override AcceptanceReport CanDesignateThing(Thing t) =>
            t.def.category == ThingCategory.Item && t.def.destroyable && !t.IsDesignatedForDestruction();

        public override void DesignateThing(Thing t) => t.Map.designationManager.AddDesignation(new Designation(t, Designation));

        IEnumerable<Thing> DestructiblesInCell(IntVec3 loc) => loc.GetThingList(Map).Where(thing => CanDesignateThing(thing).Accepted);

        public override void SelectedUpdate() => GenUI.RenderMouseoverBracket();
    }
}
