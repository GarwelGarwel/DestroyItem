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

        public override bool TryMakePreToilReservations(bool errorOnFailed) => pawn.Reserve(TargetThingA, job, Settings.maxDestroyers, 0, errorOnFailed: errorOnFailed);

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Utility.Log($"Making new toils for {pawn} to destroy {TargetThingA}...");
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnThingMissingDesignation(TargetIndex.A, DestroyItemDefOf.Designation_DestroyItem);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);

            isDestroyingHumanlikeCorpse = TargetThingA is Corpse corpse && corpse.InnerPawn.RaceProps.Humanlike;
            isDestroyingHumanEmbryo = TargetThingA.def.defName == "HumanEmbryo";
            if (isDestroyingHumanlikeCorpse)
                Utility.Log($"The item {TargetThingA} being destroyed is a humanlike corpse. All colonists will get {DestroyItemDefOf.Thought_KnowDestroyedCorpse} thoughts and {pawn} also {DestroyItemDefOf.Thought_DestroyedCorpse}.");
            else if (isDestroyingHumanEmbryo)
                Utility.Log($"The item {TargetThingA} being destroyed is a human embryo. {pawn} will get a {DestroyItemDefOf.Thought_DestroyedEmbryo} thought).");

            Toil destroyToil = new Toil
            {
                tickAction = () =>
                {
                    if (Gen.HashCombine(Gen.HashCombine(0, pawn), TargetThingA.HashOffsetTicks()) % GenTicks.TicksPerRealSecond != 0)
                        return;
                    float hpLossAmount = pawn.GetStatValue(StatDefOf.MeleeDPS) * pawn.GetStatValue(StatDefOf.GeneralLaborSpeed) * Settings.destructionSpeed;

                    if (isDestroyingHumanlikeCorpse)
                    {
                        pawn.needs?.mood?.thoughts?.memories.TryGainMemory(DestroyItemDefOf.Thought_DestroyedCorpse);
                        List<Pawn> pawnsInFaction = pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction);
                        for (int i = 0; i < pawnsInFaction.Count; i++)
                            pawnsInFaction[i].needs?.mood?.thoughts?.memories.TryGainMemory(DestroyItemDefOf.Thought_KnowDestroyedCorpse);
                    }
                    else if (isDestroyingHumanEmbryo)
                        pawn.needs?.mood?.thoughts?.memories.TryGainMemory(DestroyItemDefOf.Thought_DestroyedEmbryo);

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
            if (TargetThingA.def.useHitPoints)
                destroyToil.WithProgressBar(TargetIndex.A, () => 1 - (float)TargetThingA.HitPoints / TargetThingA.MaxHitPoints);
            destroyToil.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            yield return destroyToil;
            yield break;
        }
    }
}
