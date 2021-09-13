using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Usable;
using Redemption.NPCs.Friendly;
using Redemption.NPCs.PreHM;
using Redemption.Projectiles.Hostile;
using Terraria.ModLoader.Utilities;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Redemption.Globals;
using Terraria.Graphics.Shaders;
using Terraria.GameContent;
using System.IO;
using Redemption.NPCs.Bosses.Keeper;
using Terraria.Audio;

namespace Redemption.NPCs.Minibosses.SkullDigger
{
    [AutoloadBossHead]
    public class SkullDigger : ModNPC
    {
        public enum ActionState
        {
            Begin,
            Idle,
            Attacks,
            Death
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];

        public float[] oldrot = new float[5];
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                ImmuneToAllBuffsThatAreNotWhips = true
            });

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Position = new Vector2(0, 30),
                PortraitPositionYOverride = 8
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 60;
            NPC.height = 92;
            NPC.friendly = false;
            NPC.damage = 35;
            NPC.defense = 0;
            NPC.lifeMax = 2400;
            NPC.HitSound = SoundID.NPCHit3;
            NPC.DeathSound = SoundID.NPCDeath51;
            NPC.value = Item.buyPrice(0, 2, 0, 0);
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.SpawnWithHigherTime(30);
            NPC.npcSlots = 10f;
            NPC.alpha = 255;
            NPC.boss = true;
            NPC.netAlways = true;
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/SilentCaverns");
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool? CanHitNPC(NPC target) => false;

        public override void HitEffect(int hitDirection, double damage)
        {
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

                new FlavorTextBestiaryInfoElement("")
            });
        }

        public override void OnKill()
        {
            if (!RedeBossDowned.downedSkullDigger)
            {
                RedeWorld.alignment--;
                for (int p = 0; p < Main.maxPlayers; p++)
                {
                    Player player = Main.player[p];
                    if (!player.active)
                        continue;

                    CombatText.NewText(player.getRect(), Color.Gold, "-1", true, false);

                    if (!player.HasItem(ModContent.ItemType<AlignmentTeller>()))
                        continue;

                    if (!Main.dedServ)
                        RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("A saddening conclusion to this protector and mistress, but perhaps there is another way..?", 180, 30, 0, Color.DarkGoldenrod);

                }
            }
            NPC.SetEventFlagCleared(ref RedeBossDowned.downedSkullDigger, -1);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            if (Main.netMode == NetmodeID.Server || Main.dedServ)
            {
                writer.Write(ID);
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ID = reader.ReadInt32();
            }
        }

        void AttackChoice()
        {
            int attempts = 0;
            while (attempts == 0)
            {
                if (CopyList == null || CopyList.Count == 0)
                    CopyList = new List<int>(AttackList);
                ID = CopyList[Main.rand.Next(0, CopyList.Count)];
                CopyList.Remove(ID);
                NPC.netUpdate = true;

                attempts++;
            }
        }

        private bool floatTimer;

        public List<int> AttackList = new() { 0 };
        public List<int> CopyList = null;

        public int ID { get => (int)NPC.ai[3]; set => NPC.ai[3] = value; }

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            if (!RedeHelper.AnyProjectiles(ModContent.ProjectileType<SkullDigger_FlailBlade>()))
                NPC.Shoot(NPC.Center, ModContent.ProjectileType<SkullDigger_FlailBlade>(), NPC.damage, Vector2.Zero, false, SoundID.Item1.WithVolume(0), "", NPC.whoAmI);

            DespawnHandler();

            if (AIState != ActionState.Death && AIState != ActionState.Attacks)
                NPC.LookAtEntity(player);

            if (!floatTimer)
            {
                NPC.velocity.Y += 0.03f;
                if (NPC.velocity.Y > .5f)
                {
                    floatTimer = true;
                    NPC.netUpdate = true;
                }
            }
            else if (floatTimer)
            {
                NPC.velocity.Y -= 0.03f;
                if (NPC.velocity.Y < -.5f)
                {
                    floatTimer = false;
                    NPC.netUpdate = true;
                }
            }

            switch (AIState)
            {
                case ActionState.Begin:
                    NPC.dontTakeDamage = true;
                    switch (TimerRand)
                    {
                        case 0:
                            if (AITimer++ == 0)
                            {
                                if (!Main.dedServ)
                                    RedeSystem.Instance.TitleCardUIElement.DisplayTitle("Skull Digger", 60, 90, 0.8f, 0, Color.MediumPurple, "The Keeper's First Creation");
                                if (!NPC.AnyNPCs(ModContent.NPCType<Keeper>()))
                                {
                                    NPC.position = new Vector2(Main.rand.NextBool(2) ? player.Center.X - 180 : player.Center.X + 180, player.Center.Y);
                                    NPC.alpha = 255;
                                }
                                else if (!Main.dedServ)
                                    Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossKeeper");

                                NPC.velocity.Y = -6;
                            }

                            int dustIndex = Dust.NewDust(NPC.BottomLeft + new Vector2(0, 20), NPC.width, 1, DustID.DungeonSpirit);
                            Main.dust[dustIndex].velocity.Y -= 10f;
                            Main.dust[dustIndex].velocity.X *= 0f;
                            Main.dust[dustIndex].noGravity = true;

                            NPC.velocity *= 0.96f;
                            NPC.alpha -= 2;
                            if (NPC.alpha <= 0)
                            {
                                TimerRand = 1;
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        case 1:
                            AITimer++;
                            if (NPC.AnyNPCs(ModContent.NPCType<Keeper>()))
                            {
                                if (!Main.dedServ)
                                {
                                    if (AITimer == 40)
                                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Mistress Octavia, are you harmed?", 120, 30, 0.6f, "Skull Digger:", 0.3f, Color.MediumPurple, null, null, NPC.Center, 0);
                                    if (AITimer == 220)
                                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("You need not worry, I shall crush this vermin.", 120, 30, 0.6f, "Skull Digger:", 0.6f, Color.MediumPurple, null, null, NPC.Center, 0);
                                    if (AITimer == 400)
                                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Prepare to reap what you sow...", 120, 30, 0.6f, "Skull Digger:", 0.6f, Color.MediumPurple, null, null, NPC.Center, 0);
                                }
                                if (AITimer >= 580)
                                {
                                    NPC.dontTakeDamage = false;
                                    AITimer = 0;
                                    TimerRand = 0;
                                    AIState = ActionState.Idle;
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                if (!Main.dedServ)
                                {
                                    if (AITimer == 40)
                                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("I have finally found you, vermin...", 120, 30, 0.6f, "Skull Digger:", 0.3f, Color.MediumPurple, null, null, NPC.Center, 0);
                                    if (AITimer == 220)
                                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Prepare to reap what you sow...", 120, 30, 0.6f, "Skull Digger:", 0.3f, Color.MediumPurple, null, null, NPC.Center, 0);
                                }
                                if (AITimer >= 400)
                                {
                                    NPC.dontTakeDamage = false;
                                    AITimer = 0;
                                    TimerRand = 0;
                                    AIState = ActionState.Idle;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                    }
                    break;
                case ActionState.Idle:
                    NPC.Move(Vector2.Zero, 2, 20, true);
                    AITimer++;
                    switch (TimerRand)
                    {
                        case 0:
                            if (AITimer >= 5)
                                NPC.alpha += 5;
                            if (NPC.alpha >= 255)
                            {
                                NPC.velocity *= 0f;
                                NPC.position = new Vector2(Main.rand.NextBool(2) ? player.Center.X - 180 : player.Center.X + 180, player.Center.Y - 30);
                                TimerRand = 1;
                            }
                            break;
                        case 1:
                            NPC.alpha -= 5;
                            if (NPC.alpha <= 0)
                            {
                                AttackChoice();
                                AITimer = 0;
                                TimerRand = 0;
                                AIState = ActionState.Attacks;
                                NPC.netUpdate = true;
                            }
                            break;
                    }
                    break;
                case ActionState.Attacks:
                    switch (ID)
                    {
                        #region Flail Throw
                        case 0:
                            NPC.LookAtEntity(player);
                            NPC.Move(Vector2.Zero, 2, 20, true);
                            if (AITimer >= 1)
                            {
                                TimerRand = 0;
                                AITimer = 0;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                            #endregion
                    }
                    break;
                case ActionState.Death:
                    NPC.dontTakeDamage = true;
                    NPC.velocity *= 0.96f;
                    if (NPC.AnyNPCs(ModContent.NPCType<Keeper>()))
                    {
                        if (Main.rand.NextBool(3))
                        {
                            int dustIndex = Dust.NewDust(NPC.BottomLeft + new Vector2(0, 20), NPC.width, 1, DustID.DungeonSpirit);
                            Main.dust[dustIndex].velocity.Y -= 10f;
                            Main.dust[dustIndex].velocity.X *= 0f;
                            Main.dust[dustIndex].noGravity = true;
                        }
                    }
                    else
                    {
                        player.GetModPlayer<ScreenPlayer>().ScreenFocusPosition = NPC.Center;
                        player.GetModPlayer<ScreenPlayer>().lockScreen = true;
                        if (AITimer++ == 0)
                            NPC.alpha = 0;

                        if (!Main.dedServ)
                        {
                            if (AITimer == 40)
                                RedeSystem.Instance.DialogueUIElement.DisplayDialogue("My dear mistress, I have failed you.", 120, 30, 0.6f, "Skull Digger:", 0.3f, Color.MediumPurple, null, null, NPC.Center, 0);
                            if (AITimer == 220)
                                RedeSystem.Instance.DialogueUIElement.DisplayDialogue("It seems our torment stays eternal.", 120, 30, 0.6f, "Skull Digger:", 0.6f, Color.MediumPurple, null, null, NPC.Center, 0);
                        }

                        if (AITimer >= 220)
                        {
                            NPC.alpha++;

                            int dustIndex = Dust.NewDust(NPC.BottomLeft + new Vector2(0, 20), NPC.width, 1, DustID.DungeonSpirit);
                            Main.dust[dustIndex].velocity.Y -= 10f;
                            Main.dust[dustIndex].velocity.X *= 0f;
                            Main.dust[dustIndex].noGravity = true;

                            if (NPC.alpha >= 255)
                            {
                                NPC.dontTakeDamage = false;
                                player.ApplyDamageToNPC(NPC, 9999, 0, 0, false);
                            }
                        }
                        else
                        {
                            if (Main.rand.NextBool(3))
                            {
                                int dustIndex = Dust.NewDust(NPC.BottomLeft + new Vector2(0, 20), NPC.width, 1, DustID.DungeonSpirit);
                                Main.dust[dustIndex].velocity.Y -= 10f;
                                Main.dust[dustIndex].velocity.X *= 0f;
                                Main.dust[dustIndex].noGravity = true;
                            }
                        }
                    }
                    break;
            }
        }

        public override bool CheckDead()
        {
            if (AIState is ActionState.Death)
                return true;
            else
            {

                SoundEngine.PlaySound(SoundID.NPCDeath51, NPC.position);
                NPC.alpha = 0;
                NPC.life = 1;
                AITimer = 0;
                AIState = ActionState.Death;
                return false;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            for (int k = NPC.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = NPC.rotation;

            NPC.rotation = NPC.velocity.X * 0.05f;
            if (++NPC.frameCounter >= 10)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 3 * frameHeight)
                    NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D HandsTex = ModContent.Request<Texture2D>("Redemption/NPCs/Minibosses/SkullDigger/SkullDigger_Hands").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int shader = ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;

            if (!NPC.IsABestiaryIconDummy)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);

                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {
                    Vector2 oldPos = NPC.oldPos[i];
                    Main.spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(Color.LightCyan) * 0.3f, oldrot[i], NPC.frame.Size() / 2, NPC.scale + 0.1f, effects, 0);
                }

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            Rectangle rect = new(0, 0, HandsTex.Width, HandsTex.Height);
            spriteBatch.Draw(HandsTex, NPC.Center - screenPos - new Vector2(14, -32), new Rectangle?(rect), NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            float baseChance = SpawnCondition.Cavern.Chance * (RedeBossDowned.downedKeeper ? (RedeBossDowned.downedSkullDigger ?  0 : 1) : 0);
            float multiplier = spawnInfo.player.HasItem(ModContent.ItemType<SorrowfulEssence>()) ? 0.1f : 0.002f;

            return baseChance * multiplier;
        }

        public override Color? GetAlpha(Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                return NPC.GetBestiaryEntryColor();
            return null;
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
                    NPC.alpha += 2;
                    if (NPC.alpha >= 255)
                        NPC.active = false;
                    if (NPC.timeLeft > 10)
                        NPC.timeLeft = 10;
                    return;
                }
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }
    }
}