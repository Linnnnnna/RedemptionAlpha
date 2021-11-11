using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Redemption.Items.Materials.HM
{
    public class Starlite : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 50;
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 26;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(0, 0, 2, 50);
            Item.rare = ItemRarityID.Lime;
        }
        public override void AddRecipes()
        {
            CreateRecipe(10)
                .AddIngredient(ItemID.TitaniumOre, 4)
                .AddIngredient(ItemID.Diamond)
                .AddTile(TileID.AdamantiteForge)
                .Register();
            CreateRecipe(10)
                .AddIngredient(ItemID.AdamantiteOre, 4)
                .AddIngredient(ItemID.Diamond)
                .AddTile(TileID.AdamantiteForge)
                .Register();
        }
    }
}