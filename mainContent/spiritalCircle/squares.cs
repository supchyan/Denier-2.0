using System;
using Denier.mainContent.spiritalCircle.spiritalCursorMarker;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;
using System.IO;
using Denier.mainContent.Buffs;

namespace Denier.mainContent.spiritalCircle {
    public class squares : ModProjectile {
        SoundStyle tickSound = new SoundStyle("Denier/Sounds/tick");
        SoundStyle noteSound = new SoundStyle("Denier/Sounds/note");

        private bool playItOneTime;
        public static float oldRot;
        public static bool canShoot;

        public override void SetDefaults() {
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.Opacity = 0.8f;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.netUpdate = true;
        }
        public Vector2 projPos;
        public static double projRot;
        public double projScl;
        public double projOpa;
        public override void OnSpawn(IEntitySource source) {
            Player player = Main.player[Projectile.owner];

            projPos = player.Center - new Vector2(Projectile.width / 2f, Projectile.height / 2f);
            projRot = 0f;
            projScl = 0f;
            projOpa = 0.8f;      
        }
        public override void AI() {
            Projectile.ai[0]++;
            if(!canShoot) {
                Projectile.ai[1]++;
            }
            Projectile.timeLeft = 2;

            Player player = Main.player[Projectile.owner];

            projPos = player.Center - new Vector2(Projectile.width / 2f, Projectile.height / 2f);
            projRot = MathHelper.ToRadians(Projectile.ai[1]);

            while(Projectile.ai[0] <= 20f) {
                projScl = Projectile.ai[0] / 20f;
                break;
            }
            if(Projectile.ai[1] % 45 == 0 && !canShoot) {
                canShoot = true;
                playItOneTime = true;
                squaresCursor.playItOneTime = true;

                oldRot = Projectile.rotation;
            }
            if(canShoot) {
                if(player.statMana >= 15 && playItOneTime) {
                    playItOneTime = false;

                    SoundEngine.PlaySound(noteSound with {Volume = 2f}, Main.MouseWorld);

                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(), Projectile.Center,
                        Projectile.velocity, ModContent.ProjectileType<squaresOut>(), 0, 0, player.whoAmI
                    );
                }   
            }

            if(!player.HasBuff<scopingBuff>() || player.dead) {
                projOpa -= 0.2f;
                projScl += 0.1f;
            }
            Projectile.position = projPos;
            Projectile.rotation = (float)projRot;
            Projectile.scale = (float)projScl;
            Projectile.Opacity = (float)projOpa;
                
            if(player.HeldItem.ModItem is not rifle || Projectile.Opacity <= 0.2f)
                Projectile.Kill();
        }
        public override Color? GetAlpha(Color lightColor) {
            if(canShoot && Main.LocalPlayer.statMana >= 15)
			    return new Color(255, 0, 0, 255) * Projectile.Opacity;
            else if(canShoot && Main.LocalPlayer.statMana < 15)
			    return Color.Gray * Projectile.Opacity;
            else
                return new Color(255, 255, 255, 255) * Projectile.Opacity;
		}
        public override bool PreDraw(ref Color lightColor) {
			SpriteEffects spriteEffects = SpriteEffects.None;

			Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];

			Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, frameHeight);
			Vector2 origin = sourceRectangle.Size() / 2f;

			Color drawColor = Projectile.GetAlpha(lightColor);

			Main.EntitySpriteDraw(texture,
				Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
				sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale * 0.75f, spriteEffects, 0);

			return false;
		}
        public override void SendExtraAI(BinaryWriter writer) {
			writer.WriteVector2(projPos);
            writer.Write(projScl);
            writer.Write(projOpa);
		}
        public override void ReceiveExtraAI(BinaryReader reader) {
            projPos = reader.ReadVector2();
            projScl = reader.ReadDouble();
            projOpa = reader.ReadDouble();
        }
    }
}