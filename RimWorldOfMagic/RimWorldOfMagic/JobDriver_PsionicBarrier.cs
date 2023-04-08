using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;
using Verse.AI;
using UnityEngine;

namespace TorannMagic
{
    internal class JobDriver_PsionicBarrier : JobDriver
    {
        private const TargetIndex building = TargetIndex.A;

        int age = -1;
        int barrierSearchFrequency = 1;
        int duration = 900;
        bool psiFlag;
        float psiEnergy = 0;
        List<IntVec3> barrierCells = new List<IntVec3>();

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            CompAbilityUserMight comp = pawn.GetCompAbilityUserMight();
            float radius = 2.5f;
            radius += (.75f * TM_Calc.GetSkillVersatilityLevel(pawn, TorannMagicDefOf.TM_PsionicBarrier));
            psiFlag = pawn.health.hediffSet.HasHediff(HediffDef.Named("TM_PsionicHD"));
            Toil psionicBarrier = new Toil();
            psionicBarrier.initAction = delegate
            {
                if (age > duration)
                {
                    EndJobWith(JobCondition.Succeeded);
                }
                barrierCells = new List<IntVec3>();
                GetCellList(radius);
                ticksLeftThisToil = 10;
                comp.shouldDrawPsionicShield = true;                
            };
            psionicBarrier.tickAction = delegate
            {
                //DrawBarrier(radius);
                if (Find.TickManager.TicksGame % barrierSearchFrequency == 0)
                {
                    if (psiFlag)
                    {
                        if(Rand.Chance(.15f * comp.MightData.MightPowerSkill_PsionicBarrier.FirstOrDefault((MightPowerSkill x) => x.label == "TM_PsionicBarrier_pwr").level))
                        {
                            RepelProjectiles(false, radius);
                        }
                        else
                        {
                            RepelProjectiles(psiFlag, radius);
                        }
                        if (pawn.IsColonist)
                        {
                            HealthUtility.AdjustSeverity(pawn, HediffDef.Named("TM_PsionicHD"), -.004f);
                        }
                        else
                        {
                            HealthUtility.AdjustSeverity(pawn, HediffDef.Named("TM_PsionicHD"), -.04f);
                        }
                        
                        psiEnergy = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("TM_PsionicHD")).Severity;
                        if (psiEnergy < 1)
                        {
                            EndJobWith(JobCondition.Succeeded);
                        }
                        if (psiFlag)
                        {
                            ticksLeftThisToil = (int)psiEnergy;
                        }
                    }
                    else
                    {
                        RepelProjectiles(false, radius);
                        comp.Stamina.CurLevel -= .00035f;
                    }
                }                
                age++;                
                if(!psiFlag)
                {
                    ticksLeftThisToil = Mathf.RoundToInt((float)(duration - age) / duration * 100f);
                    if (age > duration)
                    {
                        EndJobWith(JobCondition.Succeeded);
                    }
                    if(comp.Stamina.CurLevel < .01f)
                    {
                        EndJobWith(JobCondition.Succeeded);
                    }
                }               
            };
            psionicBarrier.defaultCompleteMode = ToilCompleteMode.Delay;
            psionicBarrier.defaultDuration = duration;
            psionicBarrier.WithProgressBar(TargetIndex.A, delegate
            {
                if (pawn.DestroyedOrNull() || pawn.Dead || pawn.Downed)
                {
                    return 1f;
                }
                return 1f - (float)psionicBarrier.actor.jobs.curDriver.ticksLeftThisToil/100;

            }, false, 0f);
            psionicBarrier.AddFinishAction(delegate
            {
                if(pawn.story.traits.HasTrait(TorannMagicDefOf.Faceless))
                {                    
                    CompAbilityUserMight mightComp = pawn.GetCompAbilityUserMight();
                    if (mightComp.mimicAbility != null)
                    {
                        mightComp.RemoveAbility(mightComp.mimicAbility);
                    }                  
                }
                comp.shouldDrawPsionicShield = false;
                //do soemthing?
            });
            yield return psionicBarrier;
        }

        private void RepelProjectiles(bool usePsionicEnergy, float radius)
        {
            for (int i = 0; i < barrierCells.Count; i++)
            {
                List<Thing> cellList = barrierCells[i].GetThingList(pawn.Map);
                for (int j = 0; j < cellList.Count; j++)
                {
                    if (cellList[j] is Projectile)
                    {
                        Projectile p = cellList[j] as Projectile;
                        if (p.Launcher != null && p.Launcher.Position.DistanceTo(TargetLocA) > (radius +.5f))
                        {
                            Vector3 displayEffect = barrierCells[i].ToVector3Shifted();
                            displayEffect.x += Rand.Range(-.3f, .3f);
                            displayEffect.y += Rand.Range(-.3f, .3f);
                            displayEffect.z += Rand.Range(-.3f, .3f);
                            float projectileDamage = cellList[j].def.projectile.GetDamageAmount(1, null);
                            TM_MoteMaker.ThrowGenericFleck(FleckDefOf.LightningGlow, displayEffect, Map, projectileDamage / 8f, .2f, .1f, .3f, 0, 0, 0, Rand.Range(0, 360));
                            if(usePsionicEnergy)
                            {
                                int eff = pawn.GetCompAbilityUserMight().MightData.MightPowerSkill_PsionicBarrier.FirstOrDefault((MightPowerSkill x) => x.label == "TM_PsionicBarrier_eff").level;
                                float sevReduct = (projectileDamage / (12 + eff));
                                HealthUtility.AdjustSeverity(pawn, HediffDef.Named("TM_PsionicHD"), -sevReduct);
                            }
                            else
                            {
                                pawn.GetCompAbilityUserMight().Stamina.CurLevel -= (projectileDamage / 600);
                            }
                            if(cellList[j].def.projectile.explosionRadius > 0 && cellList[j].def != TorannMagicDefOf.Projectile_FogOfTorment)
                            {
                                GenExplosion.DoExplosion(barrierCells[i], pawn.Map, cellList[j].def.projectile.explosionRadius, cellList[j].def.projectile.damageDef, pawn, (int)projectileDamage, cellList[j].def.projectile.GetArmorPenetration(1, null), cellList[j].def.projectile.soundExplode,
                                    null, cellList[j].def, null, cellList[j].def.projectile.postExplosionSpawnThingDef, cellList[j].def.projectile.postExplosionSpawnChance, cellList[j].def.projectile.postExplosionSpawnThingCount, null, cellList[j].def.projectile.applyDamageToExplosionCellsNeighbors,
                                    cellList[j].def.projectile.preExplosionSpawnThingDef, cellList[j].def.projectile.preExplosionSpawnChance, cellList[j].def.projectile.preExplosionSpawnThingCount, cellList[j].def.projectile.explosionChanceToStartFire, cellList[j].def.projectile.explosionDamageFalloff);
                            }
                            cellList[j].Destroy();
                        }
                    }
                }
            }
        }        

        private void DrawBarrier(float radius)
        {
            float drawRadius = radius * .23f;
            float num = Mathf.Lerp(drawRadius, 9.5f, drawRadius);
            Vector3 vector = TargetLocA.ToVector3Shifted();
            vector.y = Altitudes.AltitudeFor(AltitudeLayer.VisEffects);
            Vector3 s = new Vector3(num, 9.5f, num);
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(vector, Quaternion.AngleAxis(Rand.Range(0,360), Vector3.up), s);
            Graphics.DrawMesh(MeshPool.plane10, matrix, TM_MatPool.PsionicBarrier, 0);
        }

        private void GetCellList(float radius)
        {
            IEnumerable<IntVec3> outerCells = GenRadial.RadialCellsAround(TargetLocA, radius, false);
            IEnumerable<IntVec3> innerCells = GenRadial.RadialCellsAround(TargetLocA, radius - 2, false);
            barrierCells = outerCells.Except(innerCells).ToList<IntVec3>();
        }
    }
}
