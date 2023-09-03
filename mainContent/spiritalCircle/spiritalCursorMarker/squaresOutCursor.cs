using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Denier.mainContent.spiritalCircle.spiritalCursorMarker {
    public class squaresOutCursor : ModProjectile {
        private float lifeTime = 30f;
        public override void SetDefaults() {
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.ignoreWater = true;
            Projectile.scale = 1f;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.tileCollide = false;
            Projectile.Opacity = 2f;
        }
        public override void OnSpawn(IEntitySource source) {
            
            Player player = Main.player[Projectile.owner];
            
            Projectile.position = player.Center - new Vector2(Projectile.width / 2f, Projectile.height / 2f);
            Projectile.rotation = MathHelper.ToRadians(45);
            
        }
        public override void AI() {

            Projectile.timeLeft = 2;

            if (Main.myPlayer == Projectile.owner)
                Projectile.netUpdate = true;

            Projectile.velocity = Main.MouseWorld - Projectile.Center;
            Projectile.rotation = MathHelper.ToRadians(45);

            double lerpValue = Projectile.ai[1]/lifeTime;

            if (Projectile.ai[1] <= lifeTime) {

                Projectile.scale = MathHelper.Lerp(Projectile.scale, 2f, (float)Math.Sqrt(lerpValue)/2);
                Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 0, (float)Math.Sqrt(lerpValue)/2);
            
            }
            if (Projectile.Opacity <= 0.01f) {

                Projectile.Kill();
            
            }

            Projectile.ai[1]++;

        }
        public override Color? GetAlpha(Color lightColor) {
            
            if(squares.canShoot && Main.LocalPlayer.statMana >= 15)
			    return new Color(255, 0, 0, 255) * Projectile.Opacity;
            else if(squares.canShoot && Main.LocalPlayer.statMana < 15)
			    return Color.Gray * Projectile.Opacity;
            else
                return new Color(255, 255, 255, 255) * Projectile.Opacity;

		}
    }
}