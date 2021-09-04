using Microsoft.Xna.Framework;
using Redemption.Projectiles.Pets;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs.Pets
{
	public class OcciePetBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Occie");
			Description.SetDefault("Octavia's kitty cat!");

			Main.buffNoTimeDisplay[Type] = true;
			Main.vanityPet[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.buffTime[buffIndex] = 18000;

			int projType = ModContent.ProjectileType<Occie>();

			if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[projType] <= 0)
			{
				Projectile.NewProjectile(player.GetProjectileSource_Buff(buffIndex), player.Center, Vector2.Zero, projType, 0, 0f, player.whoAmI);
			}
		}
	}
}