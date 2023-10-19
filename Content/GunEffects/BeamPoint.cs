using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Denier.Content.Items.Denier;
using Denier.Content.DenierUtils.TerrariaOverhaulFixes;

namespace Denier.Content.GunEffects {
    public class BeamPoint:ModProjectile {
        public override string Texture => "Denier/Content/Global-Textures/blankPixel";
        private float lerpAlpha;
        private const float beamLength = 10f;
        public override void SetDefaults() {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.damage = 0;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.Opacity = 1f;
            Projectile.ignoreWater = true;
        }
        public float Distance {
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
        public override void AI() {
            Projectile.netImportant = true;
            Projectile.netUpdate = true;
            Projectile.timeLeft = 2;
            // Projectile.ai[0]++;

            Player player = Main.player[Projectile.owner];

            Projectile.position = player.Center - new Vector2(0f,0f);
            
            Projectile.velocity = (new Vector2(player.Center.X+1f*player.direction,player.Center.Y)-Projectile.Center).RotatedBy(player.itemRotation);
            Projectile.rotation = player.itemRotation;

            Vector2 beamVelocity = Vector2.Normalize(Projectile.velocity);
			if (beamVelocity.HasNaNs()) {
				beamVelocity = -Vector2.UnitY;
			}

            int uuid = Projectile.GetByUUID(Projectile.owner, Projectile.whoAmI);
            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<ExampleLastPrismBeam>()] == 0) {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, beamVelocity, ModContent.ProjectileType<ExampleLastPrismBeam>(), 1, 1, Projectile.owner, 0, uuid);
            }
            
            if((player.HeldItem.ModItem is not DenierExtend) || !DenierTools.notAtAction(player)) {
                Projectile.Kill();
            }
        }
        public override Color? GetAlpha(Color lightColor) {
			return Color.Red; // So the item's sprite isn't affected by light
		}
        public override bool OnTileCollide(Vector2 oldVelocity) {
            return false;
        }
    }
}