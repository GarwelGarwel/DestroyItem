using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace DestroyItem
{
    public class WorkGiver_DestroyItem : WorkGiver_Scanner
    {
        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn) =>
            pawn.Map.designationManager.SpawnedDesignationsOfDef(DestroyItemDefOf.Designation_DestroyItem).Select(designation => designation.target.Thing);

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false) =>
            pawn.CanReserve(t, Settings.maxDestroyers, 0, ignoreOtherReservations: forced) && t.IsDesignatedForDestruction();

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false) => JobMaker.MakeJob(DestroyItemDefOf.Job_DestroyItem, t);
    }
}
