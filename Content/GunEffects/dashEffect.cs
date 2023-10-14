using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;
using System.IO;

namespace Denier.Content.GunEffects {
    public class dashEffect:ModProjectile {
        public override string Texture => "Denier/Content/Global-Textures/blankPixel";

        public override void SetDefaults() {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.damage = 0;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.Opacity = 0.01f;
            Projectile.ignoreWater = true;
        }
        public override void AI() {
            Projectile.netImportant = true;
            Projectile.netUpdate = true;
            Projectile.timeLeft = 2;
            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 30) Projectile.Kill();

            Player player = Main.player[Projectile.owner];
            Dust dust =  Dust.NewDustDirect(player.Center, player.width, player.height, DustID.LavaMoss,-player.velocity.X,-player.velocity.Y,1,Color.Red, 3f);
            dust.noGravity = true;
        }
    }
}