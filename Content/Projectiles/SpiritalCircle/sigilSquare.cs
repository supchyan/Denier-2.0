using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;
using System.IO;
using Denier.Content.Buffs;
using Denier.Content.Items.Denier;

namespace Denier.Content.Projectiles.SpiritalCircle {
    public class sigilSquare : ModProjectile {
        SoundStyle tickSound = new SoundStyle("Denier/Sounds/tick");
        SoundStyle noteSound = new SoundStyle("Denier/Sounds/note");

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
        private bool playItOneTime;
        public static float oldRot;
        public static bool canShoot;
        public static bool rotRes;
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
                oldRot = Projectile.rotation;

                red = 255;
                green = 0;
                blue = 0;

                if(!canShoot) {
                    canShoot = true;
                    playItOneTime = true;
                } else
                if(canShoot && player.statMana >= 15 && playItOneTime) {
                    playItOneTime = false;
                    SoundEngine.PlaySound(noteSound with {Volume = 2f}, Main.MouseWorld);
                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(), Projectile.Center,
                        Projectile.velocity, ModContent.ProjectileType<sigilSquareOut>(), 0, 0, player.whoAmI
                    );
                } 
            }
            else {
                red = 255;
                green = 255;
                blue = 255;
            }
            if(player.altFunctionUse == 0 && rotRes) {
                Projectile.ai[1]=0;
                rotRes = false;
            }
            double e = 2.7182812;
            float a = 240f; // max angle
            double b = -0.08759;

            Projectile.rotation = MathHelper.ToRadians(2*a*(float)Math.Pow(e,1.5f*b*Projectile.ai[1])*(float)Math.Sin(Projectile.ai[1]/6))+(float)Math.PI/4;

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
            
            Projectile.position = projPos;
                
            if(player.HeldItem.ModItem is not denierRifle || Projectile.Opacity <= 0.2f)
                Projectile.Kill();
        }
        public override Color? GetAlpha(Color lightColor) {
            return new Color(red,green,blue,255);
		}
        public override void SendExtraAI(BinaryWriter writer) {
			writer.WriteVector2(projPos);
            writer.Write(canShoot);
            writer.Write(red);
            writer.Write(green);
            writer.Write(blue);
		}
        public override void ReceiveExtraAI(BinaryReader reader) {
            projPos = reader.ReadVector2();
            canShoot = reader.ReadBoolean();
            red = reader.ReadInt32();
            green = reader.ReadInt32();
            blue = reader.ReadInt32();
        }
    }
}