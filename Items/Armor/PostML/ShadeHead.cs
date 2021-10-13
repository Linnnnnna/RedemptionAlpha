using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Redemption.Globals.Player;
using System;
using Redemption.Rarities;
using Redemption.DamageClasses;

namespace Redemption.Items.Armor.PostML
{
    [AutoloadEquip(EquipType.Head)]
    public class ShadeHead : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadehead");
            Tooltip.SetDefault("10% increased ritual damage"
                + "\n15% increased ritual critical strike chance"
                + "\n[c/bdffff:Spirit Level +2]");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 40;
            Item.sellPrice(gold: 5);
            Item.rare = ModContent.RarityType<SoullessRarity>();
            Item.defense = 24;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<ShadeBody>() && legs.type == ModContent.ItemType<ShadeLeggings>();
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<RitualistClass>() += .10f;
            player.GetCritChance<RitualistClass>() += 15;
            RedePlayer modPlayer = player.GetModPlayer<RedePlayer>();
            modPlayer.maxSpiritLevel += 2;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Double tap DOWN to convert all absorable spirits to offensive shadesouls that home onto enemies";

        }
    }
}