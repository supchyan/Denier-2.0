using Terraria;
using Terraria.ModLoader;

namespace Denier.Content.Utils.TerrariaOverhaulFixes {
    public class DenierTools {
        public static bool notAtAction(Player player) {
            return
            !player.dead && !player.sleeping.isSleeping && !player.mount._active &&
            player.talkNPC < 0 && player.sign < 0 &&
            !player.stoned;
        }
    }
}