using Terraria;
using Terraria.ModLoader;
using Denier.Content.Items.Denier;

namespace Denier.Content.Utils {
    public class Commands:ModPlayer {
        private bool _showStats;
        private bool _clearChat;
        public override void PostUpdate() {
            if(Main.chatText == ".get-denier") {
                Main.chatText = "";
                Item.NewItem(Entity.GetSource_DropAsItem(), Main.LocalPlayer.Center, default, ModContent.ItemType<DenierRifle>(), 1, false, 0);
            }
            if(Main.chatText == ".get-extend") {
                Main.chatText = "";
                Item.NewItem(Entity.GetSource_DropAsItem(), Main.LocalPlayer.Center, default, ModContent.ItemType<DenierExtend>(), 1, false, 0);
            }
            if(Main.chatText == ".show-stats") {
                Main.chatText = "";
                _showStats = !_showStats;
            }
            if(_showStats) {
                showStats();
                _clearChat = false;
            } else if(!_clearChat) {
                Main.NewText("\n\n\n\n\n\n\n\n\n");
                _clearChat = true;
            };
        }
        public void showStats() {
            Main.NewText("-----");
            Main.NewText("Bullet Distance: " + DenierBullet.distance);
            Main.NewText("Owner velocity: " + DenierBullet.oldPlayerVelocity);
            Main.NewText("-----");
            Main.NewText("Damage multiplier: "+(DenierBullet.distance/2f+DenierBullet.oldPlayerVelocity)/30f);
            Main.NewText("Final damage: "+(int)DenierBullet.bulletDamage);
            Main.NewText("-----");
            Main.NewText("Crit bonus: "+(DenierBullet.distance+DenierBullet.oldPlayerVelocity)/8f);
            Main.NewText("Final crit: "+(int)DenierBullet.bulletCrit);
            Main.NewText("-----");
        }
    }
}


