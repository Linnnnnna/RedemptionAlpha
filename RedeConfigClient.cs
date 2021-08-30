using Terraria.ModLoader.Config;

namespace Redemption
{
    public class RedeConfigClient : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        public static RedeConfigClient Instance;

        [Label("[c/ff0000:Disable Lore Elements]")]
        [Tooltip("WARNING: Disables all boss dialogue, alignment, and certain events from the mod (Don't use while a boss is alive)\nThis mod is heavily based on the lore, so I would not recommend this.")]
        public bool NoLoreElements;
    }
}