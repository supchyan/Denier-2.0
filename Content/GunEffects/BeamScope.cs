using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Denier.Content.Items.Denier;
using Denier.Content.Utils.TerrariaOverhaulFixes;

namespace Denier.Content.GunEffects {
    public class BeamScope:ModProjectile {
        public override string Texture => "Denier/Content/Global-Textures/blankPixel";
        // SoundStyle beamSound = new SoundStyle(" ");
        private bool playSound;
        public override void SetDefaults() {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.damage = 0;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.Opacity = 0.01f;
            Projectile.ignoreWater = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0;
        }
        public Vector2 targetPos;
        public override void AI() {
            Projectile.netImportant = true;
            Projectile.netUpdate = true;
            Projectile.timeLeft = 2;
            Projectile.ai[0]++;

            Player player = Main.player[Projectile.owner];

            if(!playSound) {
                // SoundEngine.PlaySound(beamSound with {MaxInstances = 3}, player.Center);
                playSound = true;
            }

            Projectile.position = player.Center;

            for (int i = 0; i < Main.maxNPCs; i++) {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy()) {
                    targetPos = npc.Center;
                    Vector2 distance = targetPos-player.Center;
                      
                    Dust beam =  Dust.NewDustPerfect(player.Center, DustID.LifeDrain,30f*distance.SafeNormalize(Vector2.Zero),1,Color.Red, 1f);
                    beam.noGravity = true;  
                }
                else {
                    targetPos = Vector2.Zero;
                }
            }     
            
            if(player.HeldItem.ModItem is not DenierExtend || player.dead)
                Projectile.Kill();
        }
    }
}