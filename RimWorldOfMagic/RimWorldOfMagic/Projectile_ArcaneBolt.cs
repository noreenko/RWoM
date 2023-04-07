using Verse;
using UnityEngine;

namespace TorannMagic
{
    [StaticConstructorOnStartup]
    class Projectile_ArcaneBolt : VFECore.Abilities.AbilityProjectile
    {
        private int rotationOffset;

        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            Map map = Map;
            base.Impact(hitThing);
            ThingDef def = this.def;
            Pawn pawn = launcher as Pawn;
            CompAbilityUserMagic comp = pawn.GetCompAbilityUserMagic();
            GenExplosion.DoExplosion(Position, map, this.def.projectile.explosionRadius, TMDamageDefOf.DamageDefOf.TM_Arcane, launcher,  Mathf.RoundToInt(Rand.Range(5,this.def.projectile.GetDamageAmount(1))* comp.arcaneDmg), 1, this.def.projectile.soundExplode, def, equipmentDef, intendedTarget.Thing);
        }

        public override void Draw()
        {
            rotationOffset += Rand.Range(20, 36);
            if(rotationOffset > 360)
            {
                rotationOffset = 0;
            }
            Mesh mesh = MeshPool.GridPlane(def.graphicData.drawSize);
            Graphics.DrawMesh(mesh, DrawPos, (Quaternion.AngleAxis(rotationOffset, Vector3.up) * ExactRotation), def.DrawMatSingle, 0);
            Comps_PostDraw();
        }
    }    
}


