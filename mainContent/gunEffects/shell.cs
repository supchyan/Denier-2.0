using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using System.IO;

namespace Denier.mainContent.gunEffects {
    public class shell : ModProjectile {
        public int oldDir;
        public double offsetX;
        public double offsetY;
        public double offsetRot;
        public Random rnd = new Random();
        public override void SetDefaults() {
            Projectile.width = 8;
            Projectile.height = 4;
            Projectile.damage = 0;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.Opacity = 1f;
            Projectile.ignoreWater = true;

            Projectile.netImportant = true;
            Projectile.netUpdate = true;
        }
        public override void OnSpawn(IEntitySource source) {
            offsetX = rnd.Next(-5, 2);
            offsetY = rnd.Next(-5, 2);
            offsetRot = rnd.Next(1, 10);
            
            Player player = Main.player[Projectile.owner];

            oldDir = player.direction;

            Projectile.position = player.Center + new Vector2(50f * player.direction, 0f);
        }
        public override void AI() {
            Projectile.ai[0]++;

            Projectile.velocity = new Vector2(0f, -5f + (float)offsetY) + new Vector2(-2f + (float)offsetX, 0f) * oldDir + new Vector2(0, 20f * Projectile.ai[0] / 60f);
            Projectile.rotation = MathHelper.ToRadians(Projectile.ai[0] * (float)oldDir * (float)offsetRot);

            if(Projectile.ai[0] == 120f)
                Projectile.Kill();
        }
        public override void SendExtraAI(BinaryWriter writer) {
			writer.Write(offsetX);
            writer.Write(offsetY);
            writer.Write(offsetRot);
            writer.Write(oldDir);
		}
        public override void ReceiveExtraAI(BinaryReader reader) {
            offsetX = reader.ReadDouble();
            offsetY = reader.ReadDouble();
            offsetRot = reader.ReadDouble();
            oldDir = reader.ReadInt32();
        }
        public override Color? GetAlpha(Color lightColor) {
            return Color.White * Projectile.Opacity; 
		}
    }
}