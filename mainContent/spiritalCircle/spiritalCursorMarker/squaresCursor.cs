using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System.IO;
using Denier.mainContent.Buffs;

namespace Denier.mainContent.spiritalCircle.spiritalCursorMarker {
    public class squaresCursor : ModProjectile {
        SoundStyle tickSound = new SoundStyle("Denier/Sounds/tick");
        public override void SetDefaults() {

            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.tileCollide = false;
            Projectile.Opacity = 0.8f;

        }

        public static bool playItOneTime;
        public static float oldRot;
        public Vector2 projPos;
        public double projRot;
        public double projScl;
        public double projOpa;
        public override void OnSpawn(IEntitySource source) {
            Player player = Main.player[Projectile.owner];
            
            projPos = Main.MouseWorld - new Vector2(Projectile.width / 2f, Projectile.height / 2f);
            projRot = MathHelper.ToRadians(Projectile.ai[1]);
            projScl = 0f;
            projOpa = 0.8f;
        }
        public override void AI() {
            Projectile.ai[0]++;
            Projectile.ai[1]++;
            Projectile.timeLeft = 2;

            Player player = Main.player[Projectile.owner];

            projPos = Main.MouseWorld - new Vector2(Projectile.width / 2f, Projectile.height / 2f);

            while(Projectile.ai[0] <= 20f) {
                projScl = Projectile.ai[0] / 20f;
                break;
            }
            if(squares.canShoot) {
                Projectile.ai[1] = 0;
                projRot = MathHelper.ToRadians(45);           
            }
            else {
                projRot = MathHelper.ToRadians(Projectile.ai[1]);
            }
            if (player.statMana >= 15 && playItOneTime) {
                    
                playItOneTime = false;

                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity,
                    ModContent.ProjectileType<squaresOutCursor>(), 0, 0, player.whoAmI
                );
            
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
            if(squares.canShoot && Main.LocalPlayer.statMana >= 15)
			    return new Color(255, 0, 0, 255) * Projectile.Opacity;
            else if(squares.canShoot && Main.LocalPlayer.statMana < 15)
			    return Color.Gray * Projectile.Opacity;
            else
                return new Color(255, 255, 255, 255) * Projectile.Opacity;
		}
        public override void SendExtraAI(BinaryWriter writer) {
			writer.WriteVector2(projPos);
            writer.Write(projRot);
            writer.Write(projScl);
            writer.Write(projOpa);
		}
        public override void ReceiveExtraAI(BinaryReader reader) {
            projPos = reader.ReadVector2();
            projRot = reader.ReadDouble();
            projScl = reader.ReadDouble();
            projOpa = reader.ReadDouble();
        }
    }
}