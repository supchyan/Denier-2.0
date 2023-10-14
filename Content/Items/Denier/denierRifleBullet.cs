using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.CodeAnalysis;

namespace Denier.Content.Items.Denier {
    public class denierRifleBullet:ModProjectile {
        public override void SetDefaults() {
            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.scale = 1f;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.netUpdate = true;
        }
        private float bulletDamage;
        private float bulletCrit;
        private Vector2 oldPlayerCenter;
        private Vector2 oldPlayerVelocity;
        private float oldDamage;
        private float oldCrit;
        private Vector2 distance;
        public override void OnSpawn(IEntitySource source) {
            oldDamage = Projectile.damage;
            oldCrit = Projectile.CritChance;

            oldPlayerCenter = Projectile.Center;
            oldPlayerVelocity = Main.LocalPlayer.velocity;
        }
        public override void AI() {

            if (Main.myPlayer == Projectile.owner)
                Projectile.netUpdate = true;

            Player player = Main.player[Projectile.owner];

            distance = Projectile.Center - oldPlayerCenter;

            bulletDamage = oldDamage * (
                                ((Math.Abs(distance.X) + Math.Abs(distance.Y)) / 50f) +
                                Math.Abs(oldPlayerVelocity.X) + Math.Abs(oldPlayerVelocity.Y)
                            ) / 10f;
            bulletCrit = oldCrit + (
                                ((Math.Abs(distance.X) + Math.Abs(distance.Y)) / 50f) +
                                Math.Abs(oldPlayerVelocity.X) + Math.Abs(oldPlayerVelocity.Y)
                            ) / 4f;

            Projectile.damage = (int)bulletDamage;
            Projectile.CritChance = (int)bulletCrit;

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity = Projectile.velocity * (1f - (Projectile.ai[0] / 480f));

            Projectile.ai[0]++;
            
            if(Projectile.ai[0] > 60)
                Projectile.Kill();   

            Dust shotTrail = Dust.NewDustPerfect(Projectile.Center - new Vector2(0f, 2f), DustID.PortalBolt, new Vector2(0f, 0f).DirectionTo(player.Center), 255, new Color(255f, 209f, 178f), 1f);
            shotTrail.noGravity = true;

            while (Projectile.ai[0] <= 3) {

                Lighting.AddLight(Projectile.Center, 255f / 255f, 209f / 255f, 178f / 255f);
                break;

            }

        }
        public override void OnKill(int timeLeft) {

            for (int i = 0; i < 50; i++) {
                    Vector2 gigaVelocity = Main.rand.NextVector2CircularEdge(Projectile.width / 10f, Projectile.height / 10f);
                    Dust dashDust = Dust.NewDustPerfect(Projectile.Center, DustID.PortalBolt, -gigaVelocity, 255, new Color(255f, 209f, 178f), 1f);
                    dashDust.noGravity = true;
                }

        }
        public override Color? GetAlpha(Color lightColor) {

			return new Color(255f, 209f, 178f) * Projectile.Opacity;
		
        }
    }
}