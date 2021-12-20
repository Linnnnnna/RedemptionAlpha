using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Redemption.Dusts;
using Redemption.Projectiles.Hostile;
using Terraria.GameContent;
using Redemption.Biomes;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Redemption.Globals;
using Redemption.Items.Usable;
using Terraria.GameContent.ItemDropRules;
using Redemption.Items.Lore;

namespace Redemption.NPCs.Bosses.PatientZero
{
    [AutoloadBossHead]
    public class PZ : ModNPC
    {
        public override string Texture => "Redemption/NPCs/Bosses/PatientZero/PZ_Eyelid";
        public enum ActionState
        {
            Begin,
            Attacks1
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];
        public ref float TimerRand2 => ref NPC.ai[3];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Patient Zero");
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Confused,
                    BuffID.Poisoned,
                    BuffID.Venom
                }
            });

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 98;
            NPC.height = 80;
            NPC.friendly = false;
            NPC.damage = 110;
            NPC.defense = 80;
            NPC.lifeMax = 340000;
            NPC.SpawnWithHigherTime(30);
            NPC.npcSlots = 10f;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = SoundID.NPCDeath19;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.boss = true;
            NPC.noTileCollide = false;
            NPC.dontTakeDamage = true;
            NPC.lavaImmune = true;
            NPC.netAlways = true;
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/LabBossMusic2");
            SpawnModBiomes = new int[1] { ModContent.GetInstance<LabBiome>().Type };
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement("")
            });
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.ai[0] != 0 && !NPC.AnyNPCs(ModContent.NPCType<PZ_Kari>()))
            {
                for (int i = 0; i < 80; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenBlood, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, 20, default, 4f);
                    Main.dust[dustIndex].velocity *= 1.9f;
                }
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<SludgeDust>(), NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, 20, default, 1f);
        }
        public override void OnKill()
        {
            if (!LabArea.labAccess[5])
                Item.NewItem((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ModContent.ItemType<ZoneAccessPanel6>());

            NPC.SetEventFlagCleared(ref RedeBossDowned.downedPZ, -1);
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FloppyDisk7>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FloppyDisk7_1>()));
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.Heart;
        }
        public bool OpenEye;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            DespawnHandler();

            if (!player.active || player.dead)
                return;

            if (!NPC.AnyNPCs(ModContent.NPCType<PZ_Kari>()))
                RedeHelper.SpawnNPC((int)NPC.Center.X + 3, (int)NPC.Center.Y + 149, ModContent.NPCType<PZ_Kari>(), 0, NPC.whoAmI);

            switch (AIState)
            {
                case ActionState.Begin:
                    if (AITimer++ >= 486)
                    {
                        RedeSystem.Instance.TitleCardUIElement.DisplayTitle("Kari Johansson", 60, 90, 0.8f, 0, Color.Green, "Patient Zero");
                        AITimer = 0;
                        OpenEye = true;
                        NPC.dontTakeDamage = false;
                        AIState = ActionState.Attacks1;
                        NPC.netUpdate = true;
                        if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                    }
                    break;
                case ActionState.Attacks1:
                    break;
            }
        }
        private int BodyFrame;
        private int KariFrame;
        private int GrooveTimer;
        public override void FindFrame(int frameHeight)
        {
            if (AIState > ActionState.Begin && Main.netMode == NetmodeID.SinglePlayer)
            {
                if (GrooveTimer == 0)
                {
                    NPC.scale += 0.04f;
                    if (NPC.scale > 1.04f)
                        GrooveTimer = 1;
                }
                else if (GrooveTimer == 1)
                {
                    NPC.scale -= 0.04f;
                    if (NPC.scale <= 1f)
                        GrooveTimer = 2;
                }
                else if (GrooveTimer++ >= 28)
                    GrooveTimer = 0;
            }
            NPC.frameCounter++;
            if (NPC.frameCounter % 10 == 0)
            {
                BodyFrame++;
                if (BodyFrame > 7)
                    BodyFrame = 0;
            }
            if (NPC.frameCounter % 20 == 0)
            {
                KariFrame++;
                if (KariFrame > 3)
                    KariFrame = 0;
            }
            if (OpenEye && NPC.frame.Y != 0)
            {
                if (NPC.frameCounter++ >= 20)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 3 * frameHeight)
                        NPC.frame.Y = 0;
                }
            }
            else if (!OpenEye && NPC.frame.Y != 2 * frameHeight)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter >= 20)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 3 * frameHeight)
                        NPC.frame.Y = 0;
                }
            }
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }
        private void DespawnHandler()
        {
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead)
                {
                    NPC.active = false;
                }
            }
        }
        public override bool CheckDead()
        {
            NPC.life = 1;
            return false;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D BodyAni = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/PatientZero/PZ_Body").Value;
            Texture2D BodyGlowAni = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/PatientZero/PZ_Body_Glow").Value;
            Texture2D EyeAni = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/PatientZero/PZ_Pupil").Value;
            Texture2D KariAni = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/PatientZero/PZ_Kari").Value;
            Texture2D SlimeAni = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/PatientZero/PZ_Slime").Value;

            Vector2 drawCenterC = new(NPC.Center.X + 5, NPC.Center.Y + 7);
            spriteBatch.Draw(SlimeAni, drawCenterC - screenPos, new Rectangle?(new Rectangle(0, 0, SlimeAni.Width, SlimeAni.Height)), drawColor, NPC.rotation, new Vector2(SlimeAni.Width / 2f, SlimeAni.Height / 2f), 1, SpriteEffects.None, 0f);

            Vector2 drawCenterB = new(NPC.Center.X - 2, NPC.Center.Y + 14);
            int widthB = BodyAni.Height / 8;
            int yB = widthB * BodyFrame;
            spriteBatch.Draw(BodyAni, drawCenterB - screenPos, new Rectangle?(new Rectangle(0, yB, BodyAni.Width, widthB)), drawColor, NPC.rotation, new Vector2(BodyAni.Width / 2f, widthB / 2f), NPC.scale * 2, SpriteEffects.None, 0f);
            spriteBatch.Draw(BodyGlowAni, drawCenterB - screenPos, new Rectangle?(new Rectangle(0, yB, BodyAni.Width, widthB)), Color.White, NPC.rotation, new Vector2(BodyAni.Width / 2f, widthB / 2f), NPC.scale * 2, SpriteEffects.None, 0f);

            Vector2 drawCenterD = new(NPC.Center.X + 1, NPC.Center.Y + 123);
            int widthD = KariAni.Height / 4;
            int yD = widthD * KariFrame;
            spriteBatch.Draw(KariAni, drawCenterD - screenPos, new Rectangle?(new Rectangle(0, yD, KariAni.Width, widthD)), drawColor, NPC.rotation, new Vector2(KariAni.Width / 2f, widthD / 2f), NPC.scale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(EyeAni, NPC.Center - screenPos, new Rectangle?(new Rectangle(0, 0, EyeAni.Width, EyeAni.Height)), Color.White, NPC.DirectionTo(Main.player[NPC.target].Center).ToRotation(), new Vector2(EyeAni.Width / 2f, EyeAni.Height / 2f), NPC.scale, 0, 0f);

            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}