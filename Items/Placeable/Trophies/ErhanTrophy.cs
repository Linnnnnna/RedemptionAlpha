using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Redemption.Tiles.Trophies;
using Terraria.GameContent.Creative;

namespace Redemption.Items.Placeable.Trophies
{
    public class ErhanTrophy : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Erhan Trophy");

			SacrificeTotal = 1;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ErhanTrophyTile>(), 0);
			Item.width = 32;
			Item.height = 32;
			Item.maxStack = 9999;
			Item.value = Item.sellPrice(0, 1, 33, 0);
			Item.rare = ItemRarityID.Blue;
		}
	}
}