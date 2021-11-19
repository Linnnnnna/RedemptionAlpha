using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Placeable.Tiles;
using Redemption.Buffs.Debuffs;
using Redemption.Tiles.Plants;
using Redemption.Dusts.Tiles;
using Redemption.Tiles.Furniture.Lab;

namespace Redemption.Tiles.Tiles
{
    public class OvergrownLabPlatingTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<LabPlatingTileUnsafe>()] = true;
            DustType = ModContent.DustType<LabPlatingDust>();
            ItemDrop = ModContent.ItemType<LabPlatingUnsafe>();
            MinPick = 500;
            MineResist = 3f;
            SoundType = SoundID.Tink;
            AddMapEntry(new Color(202, 210, 210));
        }
        public override void RandomUpdate(int i, int j)
        {
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);
            if (!tileAbove.IsActive && Main.rand.NextBool(15) && tileAbove.LiquidAmount == 0)
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<LabShrub>(), true, Main.rand.Next(7));
                NetMessage.SendObjectPlacment(-1, i, j - 1, ModContent.TileType<LabShrub>(), Main.rand.Next(7), 0, -1, -1);
            }
            if (!tileAbove.IsActive && Main.tile[i, j].IsActive && Main.rand.NextBool(400))
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<BabyHiveTile>(), true);
                NetMessage.SendObjectPlacment(-1, i, j - 1, ModContent.TileType<BabyHiveTile>(), 0, 0, -1, -1);
            }
            if (Main.rand.NextBool(200))
                WorldGen.SpreadGrass(i + Main.rand.Next(-1, 1), j + Main.rand.Next(-1, 1), ModContent.TileType<LabPlatingTileUnsafe>(), Type, false, 0);
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanExplode(int i, int j) => false;
    }
}
