using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Denier.mainContent.Buffs {
    public class coolDownBuff : ModBuff {
        public override void SetStaticDefaults() {
            Main.buffNoSave[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex) {
        
        }

    }
}

