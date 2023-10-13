using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using System.IO;

namespace Denier.mainContent.spiritalCircle.spiritalCursorMarker {
    public class squaresOutCursor : ModProjectile {
        private float lifeTime = 30f;
        public override void SetDefaults() {
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.ignoreWater = true;
            Projectile.scale = 1f;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.tileCollide = false;
            Projectile.Opacity = 2f;
        }
        public Vector2 projPos;
        public double projRot;
        public double projScl;
        public double projOpa;
        public override void OnSpawn(IEntitySource source) {
            Player player = Main.player[Projectile.owner];
            
            projPos = Main.MouseWorld - new Vector2(Projectile.width / 2f, Projectile.height / 2f);
            projRot = MathHelper.ToRadians(45);
            projScl = 1f;
            projOpa = 2f;
        }
        public override void AI() {
            Projectile.ai[1]++;
            Projectile.timeLeft = 2;

            Player player = Main.player[Projectile.owner];

            projPos = Main.MouseWorld - new Vector2(Projectile.width / 2f, Projectile.height / 2f);
            projRot = MathHelper.ToRadians(45);

            double lerpValue = Projectile.ai[1]/lifeTime;

            if (Projectile.ai[1] <= lifeTime) {
                projScl = MathHelper.Lerp(Projectile.scale, 2f, (float)Math.Sqrt(lerpValue)/2);
                projOpa = MathHelper.Lerp(Projectile.Opacity, 0, (float)Math.Sqrt(lerpValue)/2);
            }
            if (projOpa <= 0.01f) {
                Projectile.Kill();
            }
            Projectile.position = projPos;
            Projectile.rotation = (float)projRot;
            Projectile.scale = (float)projScl;
            Projectile.Opacity = (float)projOpa;
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