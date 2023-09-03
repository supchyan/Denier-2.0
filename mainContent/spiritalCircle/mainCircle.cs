using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Denier.mainContent.spiritalCircle.spiritalCursorMarker;

namespace Denier.mainContent.spiritalCircle {
    public class mainCircle : ModProjectile {
        public override void SetDefaults() {
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.damage = 0;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.tileCollide = false;
            Projectile.Opacity = 0.8f;
            Projectile.ignoreWater = true;
        }
        public override void OnSpawn(IEntitySource source) {

            Player player = Main.player[Projectile.owner];
            
            Projectile.position = player.Center - new Vector2(Projectile.width / 2f, Projectile.height / 2f);
            Projectile.rotation = MathHelper.ToRadians(Projectile.ai[0] * 0.5f);

            Projectile.NewProjectile(default, player.Center, player.velocity, ModContent.ProjectileType<texts>(), 0, 0, player.whoAmI);
            Projectile.NewProjectile(default, player.Center, player.velocity, ModContent.ProjectileType<squares>(), 0, 0, player.whoAmI);
            Projectile.NewProjectile(default, player.Center, player.velocity, ModContent.ProjectileType<squares45deg>(), 0, 0, player.whoAmI);
            Projectile.NewProjectile(default, player.Center, player.velocity, ModContent.ProjectileType<squaresCursor>(), 0, 0, player.whoAmI);
            Projectile.NewProjectile(default, player.Center, player.velocity, ModContent.ProjectileType<squares45degCursor>(), 0, 0, player.whoAmI);
        
        }
        public override void AI() {

            Projectile.timeLeft = 2;
            Projectile.damage = 0;

            if (Main.myPlayer == Projectile.owner)
                Projectile.netUpdate = true;

            Player player = Main.player[Projectile.owner];

            Projectile.velocity = player.Center - Projectile.Center;

            if (!squares.canShoot)
                Projectile.rotation = MathHelper.ToRadians(Projectile.ai[0] * 0.5f);
            else
                Projectile.rotation = MathHelper.ToRadians(-Projectile.ai[0] * 0.5f);

            while(Projectile.ai[0] <= 20f) {
                Projectile.scale = Projectile.ai[0] / 20f;
                break;

            }

            Projectile.ai[0]++;
        
            if(!Main.mouseRight || player.dead) {
                
                player.AddBuff(ModContent.BuffType<coolDownBuff>(), 25);

            }
            if(player.HasBuff<coolDownBuff>() || player.dead) {

                Projectile.Opacity -= 0.2f;
                Projectile.scale += 0.1f;
                
            }
                
            if(player.HeldItem.ModItem is not rifle || Projectile.Opacity <= 0.2f) {
                Projectile.Kill();
            }
                
        }
        public override Color? GetAlpha(Color lightColor) {
            
            if(squares.canShoot && Main.LocalPlayer.statMana >= 15) {
                Lighting.AddLight(Projectile.Center, 0.5f, 0f, 0f);
                return new Color(255, 0, 0, 255) * Projectile.Opacity;
            }
            else if(squares.canShoot && Main.LocalPlayer.statMana < 15) {
                return Color.Gray * Projectile.Opacity;
            }
			    
            else {
                Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 0.5f);
                return new Color(255, 255, 255, 255) * Projectile.Opacity;
            }

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