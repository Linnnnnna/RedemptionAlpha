using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Cleaver
{
    [AutoloadBossHead]
    public class VlitchCleaver : ModNPC
    {
        public float[] oldrot = new float[6];

        public enum ActionState
        {
            Intro,
            Begin,
            Idle,
            Attacks,
            Death,
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public int AITimer;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Omega Cleaver");
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.TrailCacheLength[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.width = 98;
            NPC.height = 280;
            NPC.friendly = false;
            NPC.damage = 160;
            NPC.defense = 60;
            NPC.lifeMax = 55000;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.value = 600f;
            NPC.boss = true;
            NPC.knockBackResist = 0.0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossVlitch1");
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 80; i++)
                {
                    int dustIndex = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.LifeDrain, 0f, 0f, 100, default, 1.5f);
                    Main.dust[dustIndex].velocity *= 1.9f;
                }
                for (int i = 0; i < 45; i++)
                {
                    int dustIndex = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.SparksMech, 0f, 0f, 100, default, 1.2f);
                    Main.dust[dustIndex].velocity *= 1.8f;
                }
                for (int i = 0; i < 25; i++)
                {
                    int dustIndex = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Smoke, 0f, 0f, 100, default, 1.2f);
                    Main.dust[dustIndex].velocity *= 1.8f;
                }
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.LifeDrain, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.LifeDrain, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }

        public int floatTimer;
        public float rot;
        public float dist;
        public int repeat;


        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }

        public override bool CheckActive()
        {
            Player player = Main.player[NPC.target];
            return !player.active || player.dead || Main.dayTime;
        }

        public override void AI()
        {
            for (int k = NPC.oldPos.Length - 1; k > 0; k--)
            {
                oldrot[k] = oldrot[k - 1];
            }
            oldrot[0] = NPC.rotation;

            if (AIState >= ActionState.Idle && AIState != ActionState.Death)
            {
                NPC.dontTakeDamage = false;
            }
            else
            {
                NPC.dontTakeDamage = true;
            }

            NPC host = Main.npc[(int)NPC.ai[3]];
            DespawnHandler();
            Player player = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest(true);

            Vector2 DefaultPos = new Vector2(host.spriteDirection == 1 ? -180 : 180, -60);
            Vector2 PosLeft = RedeHelper.PolarVector(-200, host.rotation);
            Vector2 PosRight = RedeHelper.PolarVector(200, host.rotation);
            Vector2 PosPlayer = new Vector2(NPC.Center.X > player.Center.X ? 300 : -300, -80);
            Vector2 PosPlayer2 = new Vector2(NPC.Center.X > player.Center.X ? 600 : -600, -80);
            Vector2 PosPlayer3 = new Vector2(NPC.Center.X > player.Center.X ? 200 : -200, -160);
            Vector2 PosPlayer3Check = new Vector2(NPC.Center.X > player.Center.X ? player.Center.X + 200 : player.Center.X - 200, player.Center.Y - 160);

            if (NPC.AnyNPCs(ModContent.NPCType<Wielder>()))
            {
                switch (AIState)
                {
                    case ActionState.Intro:
                        {
                            NPC.rotation = host.spriteDirection == 1 ? (float)-Math.PI / 2 : (float)Math.PI / 2;
                            NPC.velocity.X = host.spriteDirection == 1 ? 40 : -40;
                            if (NPC.Distance(host.Center) < 200)
                            {
                                for (int i = 0; i < 30; i++)
                                {
                                    int dustIndex = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.LifeDrain, 0f, 0f, 100, default, 3f);
                                    Main.dust[dustIndex].velocity *= 10f;
                                    Main.dust[dustIndex].noGravity = true;
                                }
                                SoundEngine.PlaySound(SoundID.Item74, (int)NPC.position.X, (int)NPC.position.Y);
                                host.ai[3] = 1;
                                NPC.velocity *= 0;
                                AIState = ActionState.Begin;
                                NPC.netUpdate = true;
                            }
                        }                       
                        break;

                    case ActionState.Begin:
                        AITimer++;
                        if (!Main.dedServ)
                        {
                            RedeSystem.Instance.TitleCardUIElement.DisplayTitle("Omega Cleaver", 60, 90, 0.8f, 0, Color.Red,
                                "1st Omega Prototype");
                        }
                        player.GetModPlayer<ScreenPlayer>().Rumble(20, 7);
                        rot = NPC.rotation;
                        if (AITimer > 20) { AIState = ActionState.Idle; AITimer = 0; NPC.netUpdate = true; }
                        break;

                    case ActionState.Idle:
                        if (host.ai[0] == 3)
                        {
                            NPC.ai[1] = 0;
                            AITimer = 0;
                            NPC.MoveToNPC(host, DefaultPos, 24, 20);
                            if (NPC.Distance(host.Center) < 200 || host.ai[3] == 2)
                            {
                                host.ai[3] = 2;
                                rot.SlowRotation(0, (float)Math.PI / 60f);
                                NPC.rotation = rot;
                            }
                            else
                            {
                                NPC.rotation += NPC.velocity.X / 50;
                                rot = NPC.rotation;
                            }
                        }
                        if (host.ai[3] >= 3)
                        {
                            AIState = ActionState.Attacks;
                        }
                        break;

                    case ActionState.Attacks:
                        if (host.ai[0] == 3)
                        {
                            AIState = ActionState.Idle;
                        }
                        switch (host.ai[3])
                        {
                            case 3:
                                AITimer++;
                                if (AITimer < 80)
                                {
                                    NPC.MoveToNPC(host, DefaultPos, 8, 2);
                                    rot.SlowRotation(NPC.DirectionTo(host.Center).ToRotation() - 1.57f, (float)Math.PI / 30f);
                                    NPC.rotation = rot;
                                }
                                else
                                {
                                    NPC.MoveToNPC(host, DefaultPos, 16, 1);
                                    AITimer = 100;
                                    NPC.rotation = NPC.DirectionTo(host.Center).ToRotation() - 1.57f;
                                }
                                break;
                            #region Swing
                            case 4:
                                rot = NPC.rotation;
                                NPC.rotation = NPC.DirectionTo(host.Center).ToRotation() - 1.57f;
                                if (AITimer == 0 || AITimer >= 100)
                                {
                                    SoundEngine.PlaySound(SoundID.Item71, NPC.position);
                                    NPC.velocity = NPC.DirectionTo(host.Center).RotatedBy(NPC.spriteDirection == 1 ? -Math.PI / 2 : Math.PI / 2) * 40;
                                    AITimer = 1;
                                }
                                else
                                {
                                    AITimer++;
                                    if (AITimer < 20)
                                    {
                                        NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y) + RedeHelper.PolarVector(134, NPC.rotation + (float)-Math.PI / 2), ModContent.ProjectileType<OmegaBlast>(), 92, RedeHelper.PolarVector(2, NPC.rotation + (float)-Math.PI / 2), false, SoundID.Item1.WithVolume(0));
                                        NPC.velocity -= NPC.velocity.RotatedBy(Math.PI / 2) * NPC.velocity.Length() / NPC.Distance(host.Center);
                                    }
                                    else
                                    {
                                        NPC.velocity *= .7f;
                                    }
                                }

                                break;
                            #endregion
                            #region Stab
                            case 5:
                                if (AITimer == 0 || AITimer >= 100)
                                {
                                    AITimer = 1;
                                    NPC.netUpdate = true;
                                }
                                else
                                {
                                    AITimer++;
                                    if (AITimer < 50)
                                    {
                                        NPC.velocity *= .94f;
                                        rot.SlowRotation(NPC.DirectionTo(player.Center).ToRotation() + 1.57f, (float)Math.PI / 30f);
                                        NPC.rotation = rot;
                                    }
                                    else if (AITimer <= 70)
                                    {
                                        NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + 1.57f;
                                    }
                                    if (AITimer == 50)
                                    {
                                        NPC.Dash(40, true, SoundID.Item74, player.Center);
                                    }
                                    if (AITimer == 70 && RedeHelper.Chance(.4f))
                                    {
                                        host.ai[2] = 60;
                                        AITimer = 0;
                                        NPC.netUpdate = true;
                                    }
                                    if (AITimer > 70)
                                    {
                                        NPC.velocity *= .94f;
                                    }
                                }
                                break;
                            #endregion
                            #region Speen I
                            case 6:
                                if (AITimer == 0 || AITimer >= 100)
                                {
                                    AITimer = 1;
                                    NPC.netUpdate = true;
                                }
                                else
                                {
                                    AITimer++;
                                    if (AITimer % 10 == 0)
                                    {
                                        NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y) + RedeHelper.PolarVector(134, NPC.rotation + (float)-Math.PI / 2), ModContent.ProjectileType<OmegaBlast>(), 92, RedeHelper.PolarVector(5, NPC.rotation + (float)-Math.PI / 2), false, SoundID.Item1.WithVolume(0));
                                    }
                                    if (AITimer < 60)
                                    {
                                        NPC.MoveToNPC(host, PosLeft, 18, 20);
                                    }
                                    else
                                    {
                                        NPC.MoveToNPC(host, PosRight, 18, 20);
                                    }
                                    if (AITimer > 120)
                                    {
                                        AITimer = 1;
                                        NPC.netUpdate = true;
                                    }
                                    NPC.rotation += NPC.velocity.X / 30;
                                }
                                break;
                            #endregion
                            #region Swing Spin
                            case 7:
                                rot = NPC.rotation;
                                NPC.rotation = NPC.DirectionTo(host.Center).ToRotation() - 1.57f;
                                if ((AITimer == 0 || AITimer >= 100) && NPC.ai[1] == 0)
                                {
                                    SoundEngine.PlaySound(SoundID.Item71, NPC.position);
                                    NPC.velocity = NPC.DirectionTo(host.Center).RotatedBy(NPC.spriteDirection == 1 ? -Math.PI / 2 : Math.PI / 2) * 30;
                                    AITimer = 1;
                                    NPC.ai[1] = 1;
                                }
                                else
                                {
                                    AITimer++;
                                    if (AITimer < 40)
                                    {
                                        NPC.velocity -= NPC.velocity.RotatedBy(Math.PI / 2) * NPC.velocity.Length() / NPC.Distance(host.Center);
                                    }
                                    else
                                    {
                                        NPC.velocity *= .7f;
                                    }
                                }
                                break;
                            #endregion
                            #region Sword Burst
                            case 8:
                                if ((AITimer == 0 || AITimer >= 100) && NPC.ai[1] == 0)
                                {
                                    AITimer = 1;
                                    NPC.ai[1] = 1;
                                    NPC.netUpdate = true;
                                }
                                else
                                {
                                    AITimer++;
                                    rot.SlowRotation(NPC.DirectionTo(player.Center).ToRotation() + 1.57f, (float)Math.PI / 30f);
                                    NPC.rotation = rot;
                                    if (AITimer < 100)
                                    {
                                        if (NPC.Distance(PosPlayer2) < 300 || AITimer > 80)
                                        {
                                            AITimer = 100;
                                            NPC.velocity *= .96f;
                                        }
                                        else
                                        {
                                            NPC.Move(PosPlayer2, 15, 5, true);
                                        }
                                    }
                                    else
                                    {
                                        NPC.velocity *= .96f;
                                        if (AITimer >= 130 && AITimer % 5 == 0 && AITimer < 200)
                                        {
                                            NPC.Shoot(NPC.Center, ModContent.ProjectileType<CleaverClone>(), NPC.damage, new Vector2(Main.rand.NextFloat(-6, 7), Main.rand.NextFloat(-6, 7)), false, SoundID.Item1.WithVolume(0));
                                        }
                                    }
                                }
                                break;
                            #endregion
                            #region Red Prism
                            case 9:
                                if (NPC.ai[1] == 0)
                                {
                                    AITimer = 0;
                                    if (player.Center.X > NPC.Center.X) { NPC.ai[1] = 1; }
                                    else { NPC.ai[1] = 2; ; }
                                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/NebSound2").WithPitchVariance(.9f), NPC.position);
                                }
                                else
                                {
                                    AITimer++;
                                    if (AITimer < 100)
                                    {
                                        rot.SlowRotation(0, (float)Math.PI / 40f);
                                        NPC.rotation = rot;
                                        if (NPC.Distance(PosPlayer) < 300 || AITimer > 40)
                                        {
                                            NPC.rotation = 0;
                                            //NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y) + RedeHelper.PolarVector(134, NPC.rotation + (float)-Math.PI / 2), ModContent.ProjectileType<RedPrism>(), NPC.damage, RedeHelper.PolarVector(9, NPC.rotation + (float)-Math.PI / 2), false, SoundID.Item1.WithVolume(0), ai0: NPC.whoAmI); 
                                            // TODO: Make this not crash
                                            AITimer = 100;
                                        }
                                        else
                                        {
                                            NPC.Move(PosPlayer, 18, 5, true);
                                        }
                                    }
                                    else
                                    {
                                        if (NPC.ai[1] == 1) { NPC.rotation += 0.01f; }
                                        else { NPC.rotation -= 0.01f; }
                                        NPC.rotation *= 1.01f;
                                        NPC.velocity *= .9f;
                                    }
                                }
                                break;
                            #endregion
                            #region Blade Pillars
                            case 10:
                                switch (NPC.ai[1])
                                {
                                    case 0:
                                        AITimer = 0;
                                        NPC.ai[1] = 1;
                                        NPC.netUpdate = true;
                                        break;
                                    case 1:
                                        AITimer++;
                                        if (AITimer < 60)
                                        {
                                            NPC.rotation += NPC.velocity.X / 30;
                                            rot = NPC.rotation;
                                        }
                                        else
                                        {
                                            rot.SlowRotation((float)Math.PI, (float)Math.PI / 20f);
                                            NPC.rotation = rot;
                                        }
                                        if (AITimer > 80)
                                        {
                                            NPC.rotation = (float)Math.PI;
                                            NPC.ai[1] = 2;
                                            AITimer = 0;
                                            NPC.netUpdate = true;
                                        }
                                        else
                                        {
                                            NPC.Move(new Vector2(0, -400), NPC.Distance(player.Center) < 700 ? 18 : 35, 3, true);
                                        }
                                        break;
                                    case 2:
                                        AITimer++;
                                        if (AITimer < 50) { NPC.velocity *= 0.9f; }
                                        if (AITimer == 20)
                                        {
                                            NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y), ModContent.ProjectileType<CleaverClone2_Spawner>(), NPC.damage, Vector2.Zero, false, SoundID.Item1.WithVolume(0));
                                            // TODO: Make them spawn properly
                                        }
                                        if (AITimer == 50)
                                        {
                                            NPC.velocity.Y = 20;
                                        }
                                        if (NPC.Center.Y > player.Center.Y + 1000)
                                        {
                                            NPC.velocity *= 0f;
                                            host.ai[2] = 800;
                                            NPC.netUpdate = true;
                                        }
                                        break;
                                }
                                break;
                            #endregion
                            #region Twin Blade Slice
                            case 11:
                                switch (NPC.ai[1])
                                {
                                    case 0:
                                        AITimer = 0;
                                        NPC.ai[1] = 1;
                                        NPC.netUpdate = true;
                                        break;
                                    case 1:
                                        AITimer++;
                                        rot.SlowRotation(player.Center.X > NPC.Center.X ? 0.78f : 5.49f, (float)Math.PI / 20f);
                                        NPC.rotation = rot;
                                        if (NPC.Distance(PosPlayer3Check) < 30 || AITimer > 60)
                                        {
                                            NPC.rotation = player.Center.X > NPC.Center.X ? 0.78f : 5.49f;
                                            repeat = player.Center.X > NPC.Center.X ? 0 : 1;
                                            NPC.ai[1] = 2;
                                            AITimer = 0;
                                            NPC.netUpdate = true;
                                        }
                                        else
                                        {
                                            NPC.Move(PosPlayer3, NPC.Distance(player.Center) < 700 ? 16 : 35, 3, true);
                                        }
                                        break;
                                    case 2:
                                        AITimer++;
                                        NPC.velocity *= 0f;
                                        if (AITimer > 10)
                                        {
                                            SoundEngine.PlaySound(SoundID.Item71, NPC.position);
                                            host.ai[2] = 200;
                                            NPC.ai[1] = 3;
                                            AITimer = 0;
                                            NPC.netUpdate = true;
                                        }
                                        break;
                                    case 3:
                                        AITimer++;
                                        if (AITimer == 1) { NPC.velocity.X = repeat == 0 ? 15 : -15; }
                                        if (AITimer == 11) { NPC.velocity.X = repeat == 0 ? -15 : 15; }
                                        NPC.velocity.Y = 26;
                                        NPC.rotation += repeat == 0 ? 0.1f : -0.1f;
                                        if (AITimer > 20)
                                        {
                                            host.ai[2] = 300;
                                            SoundEngine.PlaySound(SoundID.Item74, NPC.position);
                                            rot = NPC.rotation;
                                            NPC.velocity *= 0f;
                                            NPC.ai[1] = 4;
                                            AITimer = 0;
                                            NPC.netUpdate = true;
                                        }
                                        break;
                                    case 4:
                                        AITimer++;
                                        NPC.velocity.X = repeat == 0 ? 40 : -40;
                                        NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + 1.57f;
                                        if (repeat == 0)
                                        {
                                            if (NPC.Center.X > player.Center.X + 200)
                                            {
                                                NPC.ai[1] = 5;
                                                AITimer = 0;
                                                NPC.netUpdate = true;
                                            }
                                        }
                                        else
                                        {
                                            if (NPC.Center.X < player.Center.X - 200)
                                            {
                                                NPC.ai[1] = 5;
                                                AITimer = 0;
                                                NPC.netUpdate = true;
                                            }
                                        }
                                        break;
                                    case 5:
                                        AITimer++;
                                        if (AITimer < 30)
                                        {
                                            NPC.rotation += NPC.velocity.X / 60;
                                            rot = NPC.rotation;
                                        }
                                        else
                                        {
                                            rot.SlowRotation(repeat == 0 ? 5.49f : 0.78f, (float)Math.PI / 20f);
                                            NPC.rotation = rot;
                                        }
                                        if (NPC.Distance(PosPlayer3Check) < 30 || AITimer > 60)
                                        {
                                            NPC.rotation = repeat == 0 ? 5.49f : 0.78f;
                                            repeat = player.Center.X > NPC.Center.X ? 0 : 1;
                                            NPC.ai[1] = 6;
                                            AITimer = 0;
                                            NPC.netUpdate = true;
                                        }
                                        else
                                        {
                                            NPC.Move(PosPlayer3, NPC.Distance(player.Center) < 700 ? 16 : 35, 3, true);
                                        }
                                        break;
                                    case 6:
                                        AITimer++;
                                        NPC.velocity *= 0f;
                                        if (AITimer > 10)
                                        {
                                            host.ai[2] = 200;
                                            SoundEngine.PlaySound(SoundID.Item71, NPC.position);
                                            NPC.ai[1] = 7;
                                            AITimer = 0;
                                            NPC.netUpdate = true;
                                        }
                                        break;
                                    case 7:
                                        AITimer++;
                                        if (AITimer == 1) { NPC.velocity.X = repeat == 0 ? 15 : -15; }
                                        if (AITimer == 11) { NPC.velocity.X = repeat == 0 ? -15 : 15; }
                                        NPC.velocity.Y = 26;
                                        NPC.rotation += repeat == 0 ? 0.1f : -0.1f;
                                        if (AITimer > 20)
                                        {
                                            if (NPC.life < (int)(NPC.lifeMax * 0.6f))
                                            {
                                                rot = NPC.rotation;
                                                NPC.velocity *= 0f;
                                                AITimer = 0;
                                                NPC.ai[1] = 8;
                                                NPC.netUpdate = true;
                                            }
                                            else
                                            {
                                                repeat = 0;
                                                NPC.velocity *= 0f;
                                                host.ai[2] = 1000;
                                                AITimer = 0;
                                                NPC.netUpdate = true;
                                            }
                                        }
                                        break;
                                    case 8:
                                        rot.SlowRotation(0, (float)Math.PI / 20f);
                                        NPC.rotation = rot;
                                        if (NPC.rotation == 0)
                                        {
                                            if (AITimer == 0)
                                            {
                                                SoundEngine.PlaySound(SoundID.Item74, NPC.position);
                                                AITimer = 1;
                                                NPC.netUpdate = true;
                                            }
                                            NPC.velocity.Y = -26;
                                            if (NPC.Center.Y < player.Center.Y - 160)
                                            {
                                                NPC.velocity *= 0;
                                                AITimer = 0;
                                                NPC.ai[1] = 9;
                                                NPC.netUpdate = true;
                                            }
                                        }
                                        break;
                                    case 9:
                                        AITimer++;
                                        rot.SlowRotation(repeat == 0 ? 0.78f : 5.49f, (float)Math.PI / 20f);
                                        NPC.rotation = rot;
                                        if (AITimer > 10)
                                        {
                                            host.ai[2] = 200;
                                            SoundEngine.PlaySound(SoundID.Item71, NPC.position);
                                            NPC.rotation = player.Center.X > NPC.Center.X ? 0.78f : 5.49f;
                                            NPC.ai[1] = 10;
                                            AITimer = 0;
                                            NPC.netUpdate = true;
                                        }
                                        break;
                                    case 10:
                                        AITimer++;
                                        if (AITimer == 1) { NPC.velocity.X = repeat == 0 ? 17 : -17; }
                                        if (AITimer == 16) { NPC.velocity.X = repeat == 0 ? -17 : 17; }
                                        NPC.velocity.Y = 20;
                                        NPC.rotation += repeat == 0 ? 0.12f : -0.12f;
                                        if (AITimer > 30)
                                        {
                                            repeat = 0;
                                            NPC.velocity *= 0f;
                                            host.ai[2] = 1000;
                                            AITimer = 0;
                                            NPC.netUpdate = true;
                                        }
                                        break;
                                }
                                break;
                            #endregion

                            case 20:
                                if (NPC.life < (int)(NPC.lifeMax * .75f))
                                {
                                    host.ai[2] = 1;
                                }
                                else { host.ai[2] = -1; }
                                break;
                            case 21:
                                if (NPC.life < (int)(NPC.lifeMax * .3f))
                                {
                                    host.ai[2] = 1;
                                }
                                else { host.ai[2] = -1; }
                                break;
                            case 22:
                                if (NPC.life < (int)(NPC.lifeMax * .9f))
                                {
                                    host.ai[2] = 1;
                                }
                                else { host.ai[2] = -1; }
                                break;


                        }
                        break;
                }
            }
        }


        public override void FindFrame(int frameHeight)
        {
            NPC host = Main.npc[(int)NPC.ai[3]];
            if (host.ai[3] == 5 || host.ai[3] == 9 || host.ai[3] == 10 || (host.ai[3] == 11 && (NPC.ai[1] == 4 || NPC.ai[1] == 8)))
            {
                NPC.frameCounter++;
                if (NPC.frameCounter >= 10)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += 282;
                    if (NPC.frame.Y >= 1410)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y = 282;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D glowMask = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Glow").Value;
            Texture2D trail = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Trail").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Rectangle rectangle = NPC.frame;
            Vector2 origin2 = rectangle.Size() / 2f;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                Vector2 value4 = NPC.oldPos[i];
                Main.EntitySpriteDraw(trail, value4 + NPC.Size / 2f - Main.screenPosition + new Vector2(0, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.Red * 0.5f, oldrot[i], origin2, NPC.scale, effects, 0);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            spriteBatch.Draw(glowMask, NPC.Center - Main.screenPosition, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        public override void BossHeadRotation(ref float rotation)
        {
            rotation = NPC.rotation;
        }

        private void DespawnHandler()
        {
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.velocity *= 0.96f;
                NPC.velocity.Y -= 1;
                if (NPC.timeLeft > 10)
                {
                    NPC.timeLeft = 10;
                }
                return;
            }
        }
    }
}