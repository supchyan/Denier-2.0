using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Denier.mainContent.spiritalCircle {
    public class squaresOut : ModProjectile {
        private float lifeTime = 30f;
        public override void SetDefaults() {
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.scale = 1f;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.tileCollide = false;
            Projectile.Opacity = 2f;
            Projectile.ignoreWater = true;
        }
        public override void OnSpawn(IEntitySource source) {

            Player player = Main.player[Projectile.owner];

            Projectile.position = player.Center - new Vector2(Projectile.width / 2f, Projectile.height / 2f);
            Projectile.rotation = squares.oldRot;
        
        }
        public override void AI() {

            Projectile.timeLeft = 2;
            if (Main.myPlayer == Projectile.owner)
                Projectile.netUpdate = true;

            Player player = Main.player[Projectile.owner];

            Projectile.velocity = player.Center - Projectile.Center;
            Projectile.rotation = squares.oldRot;

            double lerpValue = Projectile.ai[1]/lifeTime;

            if (Projectile.ai[1] <= lifeTime) {
                Projectile.scale = MathHelper.Lerp(Projectile.scale, 2f, (float)Math.Sqrt(lerpValue)/2);
                Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 0, (float)Math.Sqrt(lerpValue)/2);
            }

            if (Projectile.Opacity <= 0.1f) {
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
        public override bool PreDraw(ref Color lightColor) {

			SpriteEffects spriteEffects = SpriteEffects.None;

			Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];

			Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, frameHeight);
			Vector2 origin = sourceRectangle.Size() / 2f;

			Color drawColor = Projectile.GetAlpha(lightColor);


			Main.EntitySpriteDraw(texture,
				Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
				sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale * 0.75f, spriteEffects, 0);

			return false;
            
		}
    }
}