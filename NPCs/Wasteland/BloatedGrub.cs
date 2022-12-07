using Redemption.Biomes;
using Redemption.Buffs.Debuffs;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Wasteland
{
    public class BloatedGrub : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 2;
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Poisoned,
                    ModContent.BuffType<BileDebuff>(),
                    ModContent.BuffType<GreenRashesDebuff>(),
                    ModContent.BuffType<GlowingPustulesDebuff>(),
                    ModContent.BuffType<FleshCrystalsDebuff>()
                }
            });
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Velocity = 1f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 22;
            NPC.height = 12;
            NPC.damage = 50;
            NPC.defense = 0;
            NPC.lifeMax = 1;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = SoundID.NPCDeath32;
            NPC.value = 0f;
            NPC.knockBackResist = 0f;
            AIType = NPCID.LacBeetle;
            AnimationType = NPCID.LacBeetle;
            NPC.aiStyle = 3;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<WastelandDesertBiome>().Type };
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 2; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenBlood, Scale: 3);
            }
        }
        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            if (Main.rand.NextBool(2) || Main.expertMode)
                target.AddBuff(ModContent.BuffType<GreenRashesDebuff>(), Main.rand.Next(200, 1200));
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement(
                    "Full of protein!")
            });
        }
    }
}
