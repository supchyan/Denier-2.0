using System;
using Denier.mainContent.spiritalCircle.spiritalCursorMarker;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Denier.mainContent.spiritalCircle {
    public class squares : ModProjectile {
        SoundStyle tickSound = new SoundStyle("Denier/Sounds/tick");
        SoundStyle noteSound = new SoundStyle("Denier/Sounds/note");

        private bool playItOneTime;
        public static float oldRot;
        public static bool canShoot = false;

        public override void SetDefaults() {
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.tileCollide = false;
            Projectile.Opacity = 0.8f;
            Projectile.ignoreWater = true;
        }
        public override void OnSpawn(IEntitySource source) {

            Player player = Main.player[Projectile.owner];

            Projectile.position = player.Center - new Vector2(Projectile.width / 2f, Projectile.height / 2f);
            Projectile.rotation = oldRot + MathHelper.ToRadians(Projectile.ai[0]);
        
        }
        public override void AI() {

            Projectile.timeLeft = 2;

            if (Main.myPlayer == Projectile.owner)
                Projectile.netUpdate = true;

            Player player = Main.player[Projectile.owner];

            Projectile.velocity = player.Center - Projectile.Center;
            Projectile.rotation = oldRot + MathHelper.ToRadians(Projectile.ai[0]);

            while(Projectile.ai[1] <= 20f) {

                Projectile.scale = Projectile.ai[1]/20f;
                break;

            }

            Projectile.ai[0]++;
            Projectile.ai[1]++;

            if ((Projectile.ai[0] + 2) % 45/2 == 0 && !canShoot) {

                canShoot = true;
                playItOneTime = true;
                squaresCursor.playItOneTime = true;

                oldRot = Projectile.rotation;
            
            }

            if(canShoot) {

                Projectile.ai[0] = 0;
                Projectile.rotation = oldRot;

                if(player.statMana >= 15 && playItOneTime) {

                    playItOneTime = false;

                    SoundEngine.PlaySound(noteSound with {Volume = 2f}, Main.MouseWorld);

                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(), Projectile.Center,
                        Projectile.velocity, ModContent.ProjectileType<squaresOut>(), 0, 0, player.whoAmI
                    );
                
                }
                    
            }
        
            if(player.HasBuff<coolDownBuff>() || player.dead) {

                Projectile.Opacity -= 0.2f;
                Projectile.scale += 0.1f;

            }
                
            if(player.HeldItem.ModItem is not rifle || Projectile.Opacity <= 0.2f)
                Projectile.Kill();
        }
        public override Color? GetAlpha(Color lightColor) {
            
            if(canShoot && Main.LocalPlayer.statMana >= 15)
			    return new Color(255, 0, 0, 255) * Projectile.Opacity;
            else if(canShoot && Main.LocalPlayer.statMana < 15)
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