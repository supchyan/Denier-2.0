using System;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Denier.Content.Buffs;
using Denier.Content.Items.Denier;

namespace Denier.Content.Projectiles.SpiritalCircle {
    public class SigilSquare45deg : ModProjectile {
        public override string Texture => "Denier/Content/Projectiles/SpiritalCircle/Textures/sigilSquare45deg";
        public override void SetDefaults() {
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.Opacity = 1f;
            Projectile.ignoreWater = true;
        }
        public Vector2 projPos;
        public static bool rotRes;
        public int red = 255;
        public int green = 255;
        public int blue = 255;
        private double e = 2.7182812;
        private double b = -0.08759;
        public override void OnSpawn(IEntitySource source) {
            Player player = Main.player[Projectile.owner];
            projPos = player.Center - new Vector2(Projectile.width / 2f, Projectile.height / 2f);   
        }
        public override void AI() {
            Projectile.netImportant = true;
            Projectile.netUpdate = true;
            Projectile.timeLeft = 2;
            Projectile.ai[1]++;

            Lighting.AddLight(Projectile.Center, red/255*Projectile.Opacity, green/255, blue/255);
            
            Player player = Main.player[Projectile.owner];
            if(!DenierExtend.equiped) {
                if(Projectile.ai[1] >= 45 && player.statMana >= 15) {                
                    red = 255;
                    green = 0;
                    blue = 0;
                } else {
                    red = 255;
                    green = 255;
                    blue = 255;
                }
                float a = 240f; // max angle
                Projectile.rotation = -MathHelper.ToRadians(2*a*(float)Math.Pow(e,1.5f*b*Projectile.ai[1])*(float)Math.Sin(Projectile.ai[1]/6));
            } else {
                if(Projectile.ai[1] >= 10 && player.statMana >= 20) {                
                    red = 255;
                    green = 0;
                    blue = 0;
                } else {
                    red = 255;
                    green = 255;
                    blue = 255;
                }
                float a = 90f; // max angle
                Projectile.rotation = -MathHelper.ToRadians(2*a*(float)Math.Pow(e,1.5f*b*Projectile.ai[1])*(float)Math.Sin(Projectile.ai[1]/6));
            }
            

            projPos = player.Center - new Vector2(Projectile.width / 2f, Projectile.height / 2f);

            int dieTime = 10;
            if(!player.HasBuff<scopingBuff>() || player.dead) {
                Projectile.ai[2]+=0.8f;
                double lerpValue = Projectile.ai[2]/dieTime;

                if (Projectile.ai[2] <= dieTime) {
                    Projectile.scale = 1f + 0.2f*(float)lerpValue;
                    Projectile.Opacity = 1f - 1f*(float)lerpValue;
                }
            }
            else {
                Projectile.ai[2]=0;
                Projectile.ai[0]++;
                if(Projectile.ai[0]<=45f) {
                    Projectile.scale = 1f - 1f/(float)Math.Pow(e,Projectile.ai[0]/4f);
                }
                else {
                    Projectile.scale = 1f;
                }
                Projectile.Opacity = 1f;
            }
            if(player.altFunctionUse == 0 && rotRes) {
                Projectile.ai[1]=0;
                rotRes = false;
            }
            
            Projectile.position = projPos;
                
            if((player.HeldItem.ModItem is not DenierRifle && player.HeldItem.ModItem is not DenierExtend) || Projectile.Opacity <= 0.2f)
                Projectile.Kill();
        }
        public override Color? GetAlpha(Color lightColor) {
            return new Color(red,green,blue,255)*Projectile.Opacity;
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