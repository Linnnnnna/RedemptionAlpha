using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Placeable.Tiles;
using Redemption.Dusts.Tiles;
using Redemption.Tiles.Furniture.Lab;

namespace Redemption.Tiles.Tiles
{
    public class LabPlatingTileUnsafe : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<OvergrownLabPlatingTile>()] = true;
            Main.tileMerge[ModContent.TileType<OvergrownLabPlatingTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<LabPlatingTile>()] = true;
            Main.tileMerge[ModContent.TileType<LabPlatingTile>()][Type] = true;
            DustType = ModContent.DustType<LabPlatingDust>();
            MinPick = 500;
            MineResist = 3f;
            HitSound = CustomSounds.MetalHit;
            AddMapEntry(new Color(202, 210, 210));
        }
        public override void RandomUpdate(int i, int j)
        {
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);
            if (!tileAbove.HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(600))
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<BabyHiveTile>(), true);
                NetMessage.SendObjectPlacement(-1, i, j - 1, ModContent.TileType<BabyHiveTile>(), 0, 0, -1, -1);
            }
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanExplode(int i, int j) => false;
    }
}
