using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace DestroyItem
{
    public class JobDriver_DestroyItem : JobDriver
    {
        bool isDestroyingHumanlikeCorpse;
        bool isDestroyingHumanEmbryo;

        public override bool TryMakePreToilReservations(bool errorOnFailed) => pawn.Reserve(TargetThingA, job, errorOnFailed: errorOnFailed);

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Utility.Log($"Making new toils for {pawn} to destroy {TargetThingA}...");
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnThingMissingDesignation(TargetIndex.A, DestroyItemDefOf.Designation_DestroyItem);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);

            isDestroyingHumanlikeCorpse = TargetThingA is Corpse corpse && corpse.InnerPawn.RaceProps.Humanlike;
            isDestroyingHumanEmbryo = TargetThingA.def.defName == "HumanEmbryo";
            if (isDestroyingHumanlikeCorpse)
                Utility.Log($"The item {TargetThingA} being destroyed is a humanlike corpse. {pawn} will get {DestroyItemDefOf.Thought_DestroyedCorpse} and other colonists {DestroyItemDefOf.Thought_KnowDestroyedCorpse} thoughts.");
            else if (isDestroyingHumanEmbryo)
                Utility.Log($"The item {TargetThingA} being destroyed is a human embryo. {pawn} will get {DestroyItemDefOf.Thought_DestroyedEmbryo} thought).");

            Toil destroyToil = new Toil
            {
                tickAction = () =>
                {
                    if (!TargetThingA.IsHashIntervalTick(GenTicks.TicksPerRealSecond))
                        return;
                    float hpLossAmount = pawn.GetStatValue(StatDefOf.MeleeDPS) * pawn.GetStatValue(StatDefOf.GeneralLaborSpeed) * Settings.destructionSpeed;

                    if (isDestroyingHumanlikeCorpse)
                    {
                        pawn.needs?.mood?.thoughts?.memories.TryGainMemory(DestroyItemDefOf.Thought_DestroyedCorpse);
                        foreach (Pawn p in pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction).Where(p => pawn != p))
                            p.needs?.mood?.thoughts?.memories.TryGainMemory(DestroyItemDefOf.Thought_KnowDestroyedCorpse);
                    }
                    else if (isDestroyingHumanEmbryo)
                        pawn.needs?.mood?.thoughts?.memories.TryGainMemory(DestroyItemDefOf.Thought_DestroyedEmbryo);

                    if (Settings.instantDestruction || hpLossAmount >= TargetThingA.HitPoints)
                    {
                        pawn.records.Increment(DestroyItemDefOf.Record_ItemsDestroyed);
                        TargetThingA.HitPoints = 0;
                        TargetThingA.Destroy();
                        ReadyForNextToil();
                    }
                    else TargetThingA.TakeDamage(new DamageInfo(DestroyItemDefOf.Damage_Destruction, hpLossAmount));
                },

                defaultCompleteMode = ToilCompleteMode.Never
            };
            destroyToil.WithProgressBar(TargetIndex.A, () => 1 - (float)job.targetA.Thing.HitPoints / job.targetA.Thing.MaxHitPoints);
            destroyToil.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            yield return destroyToil;
            yield break;
        }
    }
}
