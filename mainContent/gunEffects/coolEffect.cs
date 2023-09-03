using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Denier.mainContent.spiritalCircle.spiritalCursorMarker;

namespace Denier.mainContent.gunEffects {
    public class coolEffect : ModProjectile {

        private int valueY;
        public override void SetStaticDefaults() {
            Main.projFrames[Projectile.type] = 2;
        }
        public override void SetDefaults() {
            Projectile.width = 17;
            Projectile.height = 17;
            Projectile.damage = 0;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.tileCollide = false;
            Projectile.Opacity = 1f;
            Projectile.ignoreWater = true;
        }
        public override void OnSpawn(IEntitySource source) {
            
            Player player = Main.player[Projectile.owner];

            Projectile.scale = 0f;
            Projectile.Opacity = 1f;
            
            Projectile.velocity = new Vector2(110f, 0).RotatedBy((Main.MouseWorld - player.Center).ToRotation());
            Projectile.position = player.Center - new Vector2(Projectile.width/2f, Projectile.height/2f);
            Projectile.rotation = (Main.MouseWorld - player.Center).ToRotation();

            Lighting.AddLight(Projectile.Center, 25.5f, 8f, 0f);

        }
        public override void AI() {
            
            Player player = Main.player[Projectile.owner];

            Projectile.ai[0]++;

            Projectile.velocity = new Vector2(115f, 0f).RotatedBy((Main.MouseWorld - player.Center).ToRotation());
            Projectile.position = player.Center - new Vector2(Projectile.width/2f, Projectile.height/2f);
            Projectile.rotation = (Main.MouseWorld - player.Center).ToRotation();

            if(Projectile.ai[0] < 4)
            Projectile.scale = 1.6f * (Projectile.ai[0] / 4f);
            else
            Projectile.scale = 1.6f - 0.2f * (Projectile.ai[0] / 14f);

            if(Projectile.ai[0] == 14f)
                Projectile.Kill();

        }
        public override bool PreDraw(ref Color lightColor) {

			SpriteEffects spriteEffects = SpriteEffects.None;

			Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
			

			Rectangle sourceRectangle = new Rectangle(0, valueY, texture.Width, frameHeight);
			Vector2 origin = sourceRectangle.Size() / 2f;

			Color drawColor = Projectile.GetAlpha(lightColor);

            switch (Main.GameUpdateCount / 7 % 2) {
                case 0:
                    valueY = 0;
                    break;
                case 1:
                    valueY = 17;
                    break;
            }

			Main.EntitySpriteDraw(texture,
				Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
				sourceRectangle, Color.White * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

			return false;
            
		}
    }
}