using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Denier.Content.Buffs;
using System.IO;
using Denier.Content.Items.Denier;

namespace Denier.Content.Projectiles.SpiritalCircle {
    public class sigilCircle : ModProjectile {
        public override void SetDefaults() {
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.damage = 0;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.Opacity = 1f;
            Projectile.ignoreWater = true;
        }
        public Vector2 projPos;
        public int red = 255;
        public int green = 255;
        public int blue = 255;
        public override void OnSpawn(IEntitySource source) {
            Player player = Main.player[Projectile.owner];
            projPos = player.Center - new Vector2(Projectile.width / 2f, Projectile.height / 2f);   
        }
        public override void AI() {
            Projectile.netImportant = true;
            Projectile.netUpdate = true;
            Projectile.timeLeft = 2;
            Projectile.ai[1]++;
            
            Player player = Main.player[Projectile.owner];

            if(Projectile.ai[1] >= 45) {                
                red = 255;
                green = 0;
                blue = 0;
            }
            else {
                red = 255;
                green = 255;
                blue = 255;
            }

            double e = 2.7182812;
            float a = 120f; // max angle
            double b = -0.08759;

            Projectile.rotation = MathHelper.ToRadians(2*a*(float)Math.Pow(e,1.5f*b*Projectile.ai[1])*(float)Math.Sin(Projectile.ai[1]/6));

            projPos = player.Center - new Vector2(Projectile.width / 2f, Projectile.height / 2f);

            if(!player.HasBuff<scopingBuff>() || player.dead) {
                Projectile.ai[2]+=0.1f;
                Projectile.Opacity -= 2*Projectile.ai[2];
                Projectile.scale += Projectile.ai[2];
            }
            else {
                Projectile.ai[0]++;
                if(Projectile.ai[0]<=45f) {
                    Projectile.scale = 1f - 1f/(float)Math.Pow(e,Projectile.ai[0]/4f);
                }
            }
            if(player.altFunctionUse == 0 && sigilSquare.rotRes) {
                Projectile.ai[1]=0;
            }
            
            Projectile.position = projPos;
                
            if(player.HeldItem.ModItem is not denierRifle || Projectile.Opacity <= 0.2f)
                Projectile.Kill();
        }
        public override Color? GetAlpha(Color lightColor) {
            return new Color(red,green,blue,255);
		}
        public override void SendExtraAI(BinaryWriter writer) {
			writer.WriteVector2(projPos);
            writer.Write(red);
            writer.Write(green);
            writer.Write(blue);
		}
        public override void ReceiveExtraAI(BinaryReader reader) {
            projPos = reader.ReadVector2();
            red = reader.ReadInt32();
            green = reader.ReadInt32();
            blue = reader.ReadInt32();
        }
    }
}