using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace Denier.mainContent.spiritalCircle.spiritalCursorMarker {
    public class squaresCursor : ModProjectile {
        SoundStyle tickSound = new SoundStyle("Denier/Sounds/tick");
        public override void SetDefaults() {

            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.tileCollide = false;
            Projectile.Opacity = 0.8f;

        }

        public static bool playItOneTime;
        public static float oldRot;
        public override void OnSpawn(IEntitySource source) {

            Player player = Main.player[Projectile.owner];
            
            Projectile.position = player.Center - new Vector2(Projectile.width / 2f, Projectile.height / 2f);
            Projectile.rotation = MathHelper.ToRadians(Projectile.ai[0]);
            
        }
        public override void AI() {

            Projectile.timeLeft = 2;

            if (Main.myPlayer == Projectile.owner)
                Projectile.netUpdate = true;

            Player player = Main.player[Projectile.owner];

            Projectile.velocity = Main.MouseWorld - Projectile.Center;

            while(Projectile.ai[1] <= 20f) {
                Projectile.scale = Projectile.ai[1]/20f;
                break;
            }

            Projectile.ai[0]++;
            Projectile.ai[1]++;

            if(squares.canShoot) {
                Projectile.ai[0] = 0;
                Projectile.rotation = MathHelper.ToRadians(45);           
            }
            else {
                Projectile.rotation = MathHelper.ToRadians(Projectile.ai[0]);
            }

            if (player.statMana >= 15 && playItOneTime) {
                    
                playItOneTime = false;

                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity,
                    ModContent.ProjectileType<squaresOutCursor>(), 0, 0, player.whoAmI
                );
            
            }
        
            if(player.HasBuff<coolDownBuff>() || player.dead) {
                
                Projectile.Opacity -= 0.2f;
                Projectile.scale += 0.1f;
            
            }
                
            if(player.HeldItem.ModItem is not rifle || Projectile.Opacity <= 0.2f)
                Projectile.Kill();

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