using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace DestroyItem
{
    public class JobDriver_DestroyItem : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed) => pawn.Reserve(TargetThingA, job, Settings.maxDestroyers, 0, errorOnFailed: errorOnFailed);

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Utility.Log($"Making new toils for {pawn} to destroy {TargetThingA}...");
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnThingMissingDesignation(TargetIndex.A, DestroyItemDefOf.Designation_DestroyItem);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);

            int hashcode = Gen.HashCombine(pawn.GetHashCode(), TargetThingA);
            Toil destroyToil = new Toil
            {
                initAction= () =>
                {
                    if (TargetThingA is Corpse corpse && corpse.InnerPawn.RaceProps.Humanlike)
                    {
                        Utility.Log($"The item {TargetThingA} being destroyed is a humanlike corpse. All colonists will get {DestroyItemDefOf.Thought_KnowDestroyedCorpse} thoughts and {pawn} also {DestroyItemDefOf.Thought_DestroyedCorpse}.");
                        pawn.needs?.mood?.thoughts?.memories.TryGainMemory(DestroyItemDefOf.Thought_DestroyedCorpse);
                        List<Pawn> pawnsInFaction = pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction);
                        for (int i = 0; i < pawnsInFaction.Count; i++)
                            pawnsInFaction[i].needs?.mood?.thoughts?.memories.TryGainMemory(DestroyItemDefOf.Thought_KnowDestroyedCorpse);
                    }
                    else if (TargetThingA is HumanEmbryo)
                    {
                        Utility.Log($"The item {TargetThingA} being destroyed is a human embryo. {pawn} will get a {DestroyItemDefOf.Thought_DestroyedEmbryo} thought).");
                        pawn.needs?.mood?.thoughts?.memories.TryGainMemory(DestroyItemDefOf.Thought_DestroyedEmbryo);
                    }
                },

                tickAction = () =>
                {
                    if ((Find.TickManager.TicksGame + hashcode) % GenTicks.TicksPerRealSecond != 0)
                        return;
                    float hpLossAmount = pawn.GetStatValue(StatDefOf.MeleeDPS) * pawn.GetStatValue(StatDefOf.GeneralLaborSpeed) * Settings.destructionSpeed;

                    if (Settings.instantDestruction || hpLossAmount >= TargetThingA.HitPoints)
                    {
                        pawn.records?.Increment(DestroyItemDefOf.Record_ItemsDestroyed);
                        TargetThingA.HitPoints = 0;
                        CompDissolution compDissolution = TargetThingA.TryGetComp<CompDissolution>();
                        if (compDissolution != null)
                            compDissolution.TriggerDissolutionEvent(TargetThingA.stackCount);
                        else TargetThingA.Destroy();
                        ReadyForNextToil();
                    }
                    else TargetThingA.TakeDamage(new DamageInfo(DestroyItemDefOf.Damage_Destruction, hpLossAmount));
                },

                defaultCompleteMode = ToilCompleteMode.Never
            };
            if (TargetThingA.def.useHitPoints && !Settings.instantDestruction)
                destroyToil.WithProgressBar(TargetIndex.A, () => 1 - (float)TargetThingA.HitPoints / TargetThingA.MaxHitPoints);
            destroyToil.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            yield return destroyToil;

            yield break;
        }
    }
}
