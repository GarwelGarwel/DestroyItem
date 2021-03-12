﻿using RimWorld;
using System.Collections.Generic;
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
                Thing item = job.targetA.Thing;
                float hpDestructionPerTick = pawn.GetStatValue(StatDefOf.MeleeDPS) * pawn.GetStatValue(StatDefOf.GeneralLaborSpeed) / GenTicks.TicksPerRealSecond;
                if (item.HitPoints > hpDestructionPerTick)
                    item.TakeDamage(new DamageInfo(DamageDefOf.Deterioration, hpDestructionPerTick));
                else
                {
                    item.HitPoints = 0;
                    item.Destroy();
                }
                if (item.Destroyed)
                    ReadyForNextToil();
            };
            destroyToil.defaultCompleteMode = ToilCompleteMode.Never;
            destroyToil.WithProgressBar(TargetIndex.A, () => 1f - (float)job.targetA.Thing.HitPoints / job.targetA.Thing.MaxHitPoints);
            destroyToil.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            yield return destroyToil;
            yield break;
        }
    }
}