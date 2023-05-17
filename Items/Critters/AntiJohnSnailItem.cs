using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Redemption.NPCs.Critters;

namespace Redemption.Items.Critters
{
    public class AntiJohnSnailItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Anti-John Snail");
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(silver: 80);
            Item.rare = ItemRarityID.Blue;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.bait = 80;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
        }
        public override bool? UseItem(Player player)
        {
            int index = NPC.NewNPC(new EntitySource_SpawnNPC(), (int)(player.position.X + Main.rand.Next(-20, 20)), (int)player.position.Y,
                ModContent.NPCType<AntiJohnSnail>());

            if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
                NetMessage.SendData(MessageID.SyncNPC, number: index);

            return true;
        }
    }
}