using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Denier.Content.GunEffects {
    public class DashEffect:ModProjectile {
        public override string Texture => "Denier/Content/Global-Textures/blankPixel";
        SoundStyle dashSound = new SoundStyle("Denier/Sounds/newDash");
        private bool playSound;
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

            Player player = Main.player[Projectile.owner];

            if(!playSound) {
                SoundEngine.PlaySound(dashSound with {MaxInstances = 3}, player.Center);
                playSound = true;
            }

            if(Projectile.ai[0] <= 10) {
                Lighting.AddLight(player.Center, 0.4f*(Projectile.ai[0]/10), 0f, 0f);
            }
            else if (Projectile.ai[0] >= 20) {
                Lighting.AddLight(player.Center, 0.4f*1.5f - 0.4f*(Projectile.ai[0]/20), 0f, 0f);
            }
            

            Dust dust =  Dust.NewDustDirect(player.Center, player.width/2, player.height/2, DustID.LavaMoss,0,0,1,Color.Red, 2f);
            dust.noGravity = true;   

            if(Projectile.ai[0] <= 30) {
                player.fullRotation=MathHelper.ToRadians(-Projectile.ai[0]*24*player.direction);
            } else if (Projectile.ai[0] > 30) {
                player.fullRotation = 0;
                Projectile.Kill();
            }; 
        }
    }
}