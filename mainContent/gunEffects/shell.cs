using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Denier.mainContent.spiritalCircle.spiritalCursorMarker;

namespace Denier.mainContent.gunEffects {
    public class shell : ModProjectile {

        private int oldDir;
        private float offsetX;
        private float offsetY;
        private float offsetRot;
        Random rnd = new Random();

        public override void SetDefaults() {
            Projectile.width = 8;
            Projectile.height = 4;
            Projectile.damage = 0;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.tileCollide = false;
            Projectile.Opacity = 1f;
            Projectile.ignoreWater = true;
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

            Player player = Main.player[Projectile.owner];

            Projectile.ai[0]++;

            Projectile.velocity = new Vector2(0f, -5f + offsetY) + new Vector2(-2f + offsetX, 0f) * oldDir + new Vector2(0, 20f * Projectile.ai[0] / 60f);
            Projectile.rotation = MathHelper.ToRadians(Projectile.ai[0] * oldDir * offsetRot);

            if(Projectile.ai[0] == 120f)
                Projectile.Kill();

        }
        public override Color? GetAlpha(Color lightColor) {
            
            return Color.White * Projectile.Opacity; 

		}
    }
}