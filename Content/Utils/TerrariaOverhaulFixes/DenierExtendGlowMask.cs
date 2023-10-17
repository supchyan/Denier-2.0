using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Denier.Content.Items.Denier;

namespace Denier.Content.Utils.TerrariaOverhaulFixes {
    public class DenierExtendGlowMask:ModProjectile {
        public override string Texture => "Denier/Content/Items/Denier/Textures/denierExtendGlowMask";
        public override void SetDefaults() {
            Projectile.width = 64;
            Projectile.height = 17;
            Projectile.damage = 0;
            Projectile.scale = 1;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.Opacity = 1f;
            Projectile.ignoreWater = true;
            Projectile.hide = true;
        }
        public override void OnSpawn(IEntitySource source) {
            Player player = Main.player[Projectile.owner];
            Projectile.position = player.Center;
        }
        public override void AI() {
            Projectile.netImportant = true;
            Projectile.netUpdate = true;
            
            Player player = Main.player[Projectile.owner];
            // player.heldProj = Projectile.whoAmI;

            Projectile.ai[0]++;

            Projectile.rotation=player.itemRotation;
            Projectile.spriteDirection = player.direction;
            Projectile.position = player.Center - new Vector2(Projectile.width/2f,Projectile.height/2f);

            if((player.HeldItem.ModItem is not DenierExtend) || !DenierTools.notAtAction(player))
                Projectile.Kill();
        }
        private SpriteEffects spriteEffects;
        private Vector2 origin;
        private float lerpAlpha;
        public override bool PreDraw(ref Color lightColor) {
            Player player = Main.player[Projectile.owner];

			Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
			
			Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);

            if(player.gravDir == 1) {
                origin = sourceRectangle.Size()/2f+new Vector2(-18f*player.direction,0f);

                if(player.direction != 1) {
                    spriteEffects = SpriteEffects.FlipHorizontally;
                } else {
                    spriteEffects = SpriteEffects.None;
                }
            } else {
                if(player.direction != 1) {
                    origin = sourceRectangle.Size()/2f+new Vector2(-18f*player.direction,0f)-new Vector2(Projectile.width/2f,0f);
                    spriteEffects = SpriteEffects.None;
                    Projectile.rotation = Projectile.rotation + MathHelper.ToRadians(180);
                } else {
                    origin = sourceRectangle.Size()/2f+new Vector2(-18f*player.direction,0f);
                    spriteEffects = SpriteEffects.FlipVertically;
                }
            }
			
			Color drawColor = Projectile.GetAlpha(lightColor);

            lerpAlpha = (float)Math.Sqrt(Math.Abs(Math.Cos(Projectile.ai[0]/20f)));

			Main.EntitySpriteDraw(texture,
				Projectile.Center-Main.screenPosition+new Vector2(0f, Projectile.gfxOffY),
				sourceRectangle, Color.White*lerpAlpha, Projectile.rotation, origin, 2*Projectile.scale, spriteEffects, 0);
			return false;
		}
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) {
			overWiresUI.Add(index);
		}
    }
}