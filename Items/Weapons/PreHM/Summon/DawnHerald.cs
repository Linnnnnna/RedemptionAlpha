using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Projectiles.Minions;
using Redemption.Projectiles.Ranged;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Summon
{
    public class DawnHerald : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Summons a Rooster Booster in a nest that crows an empowering aura\n" +
                "Within the aura, players gain increased jump height and 20% increased movement speed\n" +
                "Right-click to disable the sentry"); */
            Item.ResearchUnlockCount = 1;

            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Summon;
            Item.sentry = true;
            Item.width = 38;
            Item.height = 38;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.value = Item.sellPrice(0, 0, 24, 0);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.DD2_DefenseTowerSpawn;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<DawnHerald_Rooster>();
            Item.mana = 20;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override bool CanUseItem(Player player)
        {
            Tile tile = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);
            if (tile.HasUnactuatedTile && Main.tileSolid[tile.TileType] && !Main.tileCut[tile.TileType])
                return false;

            return true;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            int floor = BaseWorldGen.GetFirstTileFloor((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);
            position = new Vector2(Main.MouseWorld.X, floor * 16 - 30);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (!proj.active || proj.type != type || proj.owner != player.whoAmI)
                        continue;
                    proj.timeLeft = 2;
                }
                return false;
            }
            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer, player.direction);
            projectile.originalDamage = Item.damage;

            player.UpdateMaxTurrets();
            return false;
        }
    }
}
