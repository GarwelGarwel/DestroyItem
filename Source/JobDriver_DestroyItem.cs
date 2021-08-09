using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace DestroyItem
{
    public class JobDriver_DestroyItem : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed) => pawn.Reserve(TargetThingA, job, errorOnFailed: errorOnFailed);

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Utility.Log($"Making new toils for {pawn} to destroy {TargetThingA.LabelCap}...");
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnThingMissingDesignation(TargetIndex.A, DestroyItemDefOf.Designation_DestroyItem);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            Toil destroyToil = new Toil();
            destroyToil.tickAction = () =>
            {
               if (!TargetThingA.IsHashIntervalTick(GenTicks.TicksPerRealSecond))
                    return;
                float hpLossAmount = pawn.GetStatValue(StatDefOf.MeleeDPS) * pawn.GetStatValue(StatDefOf.GeneralLaborSpeed) * Settings.destructionSpeed;
                if (Settings.instantDestruction || hpLossAmount >= TargetThingA.HitPoints)
                {
                    pawn.records.Increment(DestroyItemDefOf.Record_ItemsDestroyed);
                    if (TargetThingA is Corpse corpse && corpse.InnerPawn.RaceProps.Humanlike)
                    {
                        Utility.Log($"The destroyed item was a humanlike corpse. Adding bad thoughts to {pawn} and other pawns.");
                        if (pawn.needs?.mood?.thoughts != null)
                            pawn.needs.mood.thoughts.memories.TryGainMemory(DestroyItemDefOf.Thought_DestroyedCorpse);
                        foreach (Pawn p in pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction).Where(p => pawn != p && p.needs?.mood?.thoughts != null))
                            p.needs.mood.thoughts.memories.TryGainMemory(DestroyItemDefOf.Thought_KnowDestroyedCorpse);
                    }
                    TargetThingA.HitPoints = 0;
                    TargetThingA.Destroy();
                    ReadyForNextToil();
                }
                else TargetThingA.TakeDamage(new DamageInfo(DamageDefOf.Deterioration, hpLossAmount, instigator: pawn));
            };
            destroyToil.defaultCompleteMode = ToilCompleteMode.Never;
            destroyToil.WithProgressBar(TargetIndex.A, () => 1f - (float)job.targetA.Thing.HitPoints / job.targetA.Thing.MaxHitPoints);
            destroyToil.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            yield return destroyToil;
            yield break;
        }
    }
}
