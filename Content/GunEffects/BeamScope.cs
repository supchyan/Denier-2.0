using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Denier.Content.Items.Denier;
using Denier.Content.DenierUtils.TerrariaOverhaulFixes;

namespace Denier.Content.GunEffects {
    public class BeamScope:ModProjectile {
        public override string Texture => "Denier/Content/Global-Textures/beam";
        private float lerpAlpha;
        private const float beamLength = 2500f;
        public override void SetDefaults() {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.damage = 0;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.Opacity = 1f;
            Projectile.ignoreWater = true;
        }
        public override void AI() {
            Projectile.netImportant = true;
            Projectile.netUpdate = true;
            Projectile.timeLeft = 2;
            Projectile.ai[0]++;

            Player player = Main.player[Projectile.owner];

            Projectile.position = player.Center - new Vector2(Projectile.width/2,Projectile.height/2);
            Projectile.velocity = (new Vector2(player.Center.X+beamLength*player.direction,player.Center.Y)-Projectile.Center).RotatedBy(player.itemRotation);
            if(Projectile.velocity.X==0 || Projectile.velocity.Y==0) {
                Projectile.velocity = Vector2.Zero;
            }
            Main.NewText(Projectile.velocity);
            Projectile.rotation = player.itemRotation;
            
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
        // public override bool PreDraw(ref Color lightColor) {
        //     Player player = Main.player[Projectile.owner];
        // 	Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
        //     float distance = (Projectile.Center-player.Center).Length();
        //     int calcLength = (int)distance;
        //     Rectangle sourceRectangle = new Rectangle(0,0,calcLength,Projectile.height);
        //     Vector2 origin = sourceRectangle.Size()/2f - new Vector2(distance/2*player.direction,0);
        //     lerpAlpha = (float)Math.Sqrt(Math.Abs(Math.Cos(Projectile.ai[0]/20f)));
        //     Projectile.alpha = (int)(255*lerpAlpha);

        // 	Main.EntitySpriteDraw(texture,
        // 		player.Center-Main.screenPosition,
        // 		sourceRectangle, Color.Red*lerpAlpha, Projectile.rotation, origin, 1f, SpriteEffects.None, 0);
        // 	return true;
        // }
    }
}