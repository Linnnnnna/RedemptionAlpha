using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Dusts;
using Redemption.Projectiles.Pets;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs.Pets
{
	public class XenoemiaBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
			Main.vanityPet[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
        {
            if (Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y) > 1f && !player.rocketFrame)
            {
                if (Main.rand.NextBool(10))
                {
                    int floor = BaseWorldGen.GetFirstTileFloor((int)player.Center.X / 16, (int)player.Center.Y / 16);
                    Vector2 position = new(player.Center.X, floor * 16);

                    Projectile.NewProjectile(player.GetSource_Buff(buffIndex), position, Vector2.Zero, ModContent.ProjectileType<Xenoemia_Proj>(), 0, 0, Main.myPlayer);
                }
            }
            if (Main.rand.NextBool(10))
                Dust.NewDust(player.Center - new Vector2(100, 100), 200, 200, ModContent.DustType<DustSteam>(), newColor: Color.Green);
            if (Main.rand.NextBool(80))
                Dust.NewDust(player.Center - new Vector2(150, 150), 300, 300, ModContent.DustType<XenoemiaDust>());

            player.buffTime[buffIndex] = 18000;

			int projType = ModContent.ProjectileType<DummyPet_Proj>();

			if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[projType] <= 0)
			{
				Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, projType, 0, 0f, player.whoAmI);
			}
		}
	}
}