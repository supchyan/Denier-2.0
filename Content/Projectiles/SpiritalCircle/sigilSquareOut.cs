using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using System.IO;
using Denier.Content.Items.Denier;

namespace Denier.Content.Projectiles.SpiritalCircle {
    public class sigilSquareOut : ModProjectile {
        private int lifeTime = 10;
        public override void SetDefaults() {
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.scale = 1f;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.Opacity = 0.8f;
            Projectile.ignoreWater = true;
        }
        public Vector2 projPos;
        public override void OnSpawn(IEntitySource source) {
            Player player = Main.player[Projectile.owner];

            projPos = player.Center - new Vector2(Projectile.width / 2f, Projectile.height / 2f);
            Projectile.rotation = MathHelper.ToRadians(45);
        }
        public override void AI() {
            Projectile.netImportant = true;
            Projectile.netUpdate = true;
            Projectile.ai[1]++;
            Projectile.timeLeft = 2;

            Player player = Main.player[Projectile.owner];

            projPos = player.Center - new Vector2(Projectile.width / 2f, Projectile.height / 2f);

            double lerpValue = Projectile.ai[1]/lifeTime;

            if (Projectile.ai[2] <= lifeTime) {
                    Projectile.scale = 1f + 1f*(float)lerpValue;
                    Projectile.Opacity = 1f - 1f*(float)lerpValue;
                }
            if (Projectile.Opacity <= 0.01f) {
                Projectile.Kill();
            }
            Projectile.position = projPos;
        }
        public override Color? GetAlpha(Color lightColor) {
            return new Color(255, 0, 0, 255)*Projectile.Opacity;
		}
        public override void SendExtraAI(BinaryWriter writer) {
			writer.WriteVector2(projPos);
		}
        public override void ReceiveExtraAI(BinaryReader reader) {
            projPos = reader.ReadVector2();
        }
    }
}