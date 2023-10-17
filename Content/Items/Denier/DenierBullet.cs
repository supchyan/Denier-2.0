using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace Denier.Content.Items.Denier {
    public class DenierBullet:ModProjectile {
        public override string Texture => "Denier/Content/Items/Denier/Textures/denierBullet";
        public SoundStyle shotSound = new SoundStyle("Denier/Sounds/shot");
        public override void SetDefaults() {
            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.scale = 1f;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 3;
        }
        public static float bulletDamage;
        public static float bulletCrit;
        private Vector2 oldPlayerCenter;
        public static float oldPlayerVelocity;
        private float oldDamage;
        private float oldCrit;
        public static float distance;
        private bool playSound;
        public override void OnSpawn(IEntitySource source) {
            oldDamage = Projectile.damage;
            oldCrit = Projectile.CritChance;

            Player player = Main.player[Projectile.owner];

            oldPlayerCenter = player.Center;
            oldPlayerVelocity = (float)Math.Round(player.velocity.Length());
        }
        public override void AI() {
            Projectile.netImportant = true;
            Projectile.netUpdate = true;

            Player player = Main.player[Projectile.owner];

            if(!playSound) {
                SoundEngine.PlaySound(shotSound with {MaxInstances = 3}, player.Center);
                playSound = true;
            }

            // showStats();

            distance = (float)Math.Round(Projectile.Center.Distance(oldPlayerCenter)/16);

            bulletDamage = oldDamage * (distance/2f+oldPlayerVelocity)/30f;
            bulletCrit = oldCrit + (distance+oldPlayerVelocity)/8f;

            Projectile.damage = (int)bulletDamage;
            Projectile.CritChance = (int)bulletCrit;

            Projectile.rotation = Projectile.velocity.ToRotation();            

            Projectile.ai[0]++;

            if(!DenierExtend.equiped) {
                Projectile.velocity = Projectile.velocity*(1f-(Projectile.ai[0]/480f));
            }
            else {
                Projectile.velocity = Projectile.velocity*(1f-(Projectile.ai[0]/1250f));
            }
            
            if(Projectile.velocity.Length() < 0.1f)
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