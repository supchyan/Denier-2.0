using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Denier.Content.Items.Denier;
using Denier.Content.DenierUtils.TerrariaOverhaulFixes;

namespace Denier.Content.GunEffects {
    public class Tentacle:ModProjectile {
        public override string Texture => "Denier/Content/Global-Textures/blankPixel";
        public override void SetDefaults() {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.damage = 0;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.Opacity = 0.01f;
            Projectile.ignoreWater = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0;
        }
        public Vector2 targetPos;
        public override void AI() {
            Projectile.netImportant = true;
            Projectile.netUpdate = true;
            Projectile.timeLeft = 2;
            Projectile.ai[0]++;

            Player player = Main.player[Projectile.owner];

            Projectile.position = player.Center - Main.MouseWorld;   
            // Projectile.velocity = ;
            
            if(player.HeldItem.ModItem is not DenierExtend || player.dead) {
                Projectile.Kill();
                return;
            }
        }
        public override Color? GetAlpha(Color lightColor) {
			return Color.Red; // So the item's sprite isn't affected by light
		}
    }
}