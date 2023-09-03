using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Denier.mainContent.spiritalCircle.spiritalCursorMarker {
    public class squares45degCursor : ModProjectile {
        public override void SetDefaults() {
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.tileCollide = false;
            Projectile.Opacity = 0.8f;
        }
        public override void OnSpawn(IEntitySource source) {
            
            Player player = Main.player[Projectile.owner];
            
            Projectile.position = player.Center - new Vector2(Projectile.width / 2f, Projectile.height / 2f);
            Projectile.rotation = MathHelper.ToRadians(-Projectile.ai[0]) + MathHelper.ToRadians(45);
        
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
                Projectile.rotation = MathHelper.ToRadians(0);           
            }
            else {
                Projectile.rotation = MathHelper.ToRadians(-Projectile.ai[0]) + MathHelper.ToRadians(45);
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