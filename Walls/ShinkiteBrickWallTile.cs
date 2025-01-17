using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Redemption.Items.Placeable.Tiles;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Walls
{
	public class ShinkiteBrickWallTile : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;
			DustType = ModContent.DustType<ShinkiteDust>();
			AddMapEntry(new Color(38, 20, 22));
		}
        public override bool CanExplode(int i, int j)
        {
            return false;
        }
    }
}