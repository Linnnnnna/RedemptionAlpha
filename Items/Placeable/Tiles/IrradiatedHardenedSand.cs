using Redemption.Tiles.Tiles;
using Terraria.ModLoader;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Redemption.Items.Placeable.Tiles
{
    public class IrradiatedHardenedSand : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<IrradiatedSand>();
            ItemTrader.ChlorophyteExtractinator.AddOption_OneWay(Type, 1, ItemID.HardenedSand, 1);
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<IrradiatedHardenedSandTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
        }
    }
}
