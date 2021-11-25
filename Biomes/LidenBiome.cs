using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using System;
using Terraria;
using Terraria.ModLoader;
namespace Redemption.Biomes
{
    public class LidenBiome : ModBiome
    {
        public override string BestiaryIcon => "Textures/Bestiary/Wasteland";
		public override string BackgroundPath => "Textures/MapBackgrounds/LidenMapBG";
		public override Color? BackgroundColor => base.BackgroundColor;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Liden");
        }

        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

        public override bool IsBiomeActive(Player player)
        {
            return false;
        }
    }
}