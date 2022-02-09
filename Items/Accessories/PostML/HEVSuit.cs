using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Redemption.Items.Accessories.HM;
using Redemption.Items.Materials.PostML;
using Redemption.Items.Usable.Potions;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Items.Accessories.PostML
{
    public class HEVSuit : ModItem
    {
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Mod.AddEquipTexture(new EquipTexture(), this, EquipType.Head, $"{Texture}_{EquipType.Head}");
                Mod.AddEquipTexture(new EquipTexture(), this, EquipType.Body, $"{Texture}_{EquipType.Body}");
                Mod.AddEquipTexture(new EquipTexture(), this, EquipType.Legs, $"{Texture}_{EquipType.Legs}");
            }
        }

        private void SetupDrawing()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                int equipSlotHead = Mod.GetEquipSlot(Name, EquipType.Head);
                int equipSlotBody = Mod.GetEquipSlot(Name, EquipType.Body);
                int equipSlotLegs = Mod.GetEquipSlot(Name, EquipType.Legs);

                ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
                ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
                ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;
                ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
            }
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("HEV Suit");
            Tooltip.SetDefault("Grants immunity to the Abandoned Lab and Wasteland water"
                + "\nGreatly extends underwater breathing"
                + "\nGrants immunity to Radioactive Fallout and all infection debuffs"
                + "\nGrants protection against up to mid-level radiation");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            SetupDrawing();
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 36;
            Item.value = Item.buyPrice(1, 0, 0, 0);
            Item.rare = ItemRarityID.Purple;
            Item.accessory = true;
            Item.canBePlacedInVanityRegardlessOfConditions = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var p = player.GetModPlayer<HEVSuitPlayer>();
            p.HideVanity = hideVisual;
            p.VanityOn = true;

            player.buffImmune[ModContent.BuffType<HeavyRadiationDebuff>()] = true;
            player.buffImmune[ModContent.BuffType<RadioactiveFalloutDebuff>()] = true;
            player.buffImmune[ModContent.BuffType<GreenRashesDebuff>()] = true;
            player.buffImmune[ModContent.BuffType<GlowingPustulesDebuff>()] = true;
            player.buffImmune[ModContent.BuffType<FleshCrystalsDebuff>()] = true;
            player.buffImmune[ModContent.BuffType<ShockDebuff>()] = true;
            player.RedemptionPlayerBuff().HEVSuit = true;
            player.RedemptionPlayerBuff().WastelandWaterImmune = true;
            player.accDivingHelm = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RedeRecipe.HazmatSuitRecipeGroup)
                .AddIngredient(ModContent.ItemType<GasMask>())
                .AddIngredient(ModContent.ItemType<CrystalSerum>(), 4)
                .AddIngredient(ModContent.ItemType<RawXenium>(), 4)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
    public class HEVSuitPlayer : ModPlayer
    {
        public bool HideVanity;
        public bool ForceVanity;
        public bool VanityOn;

        public override void ResetEffects()
        {
            VanityOn = HideVanity = ForceVanity = false;
        }
        public override void UpdateVisibleVanityAccessories()
        {
            for (int n = 13; n < 18 + Player.GetAmountOfExtraAccessorySlotsToShow(); n++)
            {
                Item item = Player.armor[n];
                if (item.type == ModContent.ItemType<HEVSuit>())
                {
                    HideVanity = false;
                    ForceVanity = true;
                }
            }
        }
        public override void FrameEffects()
        {
            if ((VanityOn || ForceVanity) && !HideVanity)
            {
                var hazmatCostume = ModContent.GetInstance<HEVSuit>();
                Player.head = Mod.GetEquipSlot(hazmatCostume.Name, EquipType.Head);
                Player.body = Mod.GetEquipSlot(hazmatCostume.Name, EquipType.Body);
                Player.legs = Mod.GetEquipSlot(hazmatCostume.Name, EquipType.Legs);
            }
        }
    }
}