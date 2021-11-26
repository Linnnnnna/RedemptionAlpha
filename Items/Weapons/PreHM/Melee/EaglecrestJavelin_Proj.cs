using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Redemption.Gores.Misc;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Globals;
using Redemption.Globals.Player;
using Redemption.Buffs.NPCBuffs;
using Terraria.Graphics.Shaders;
using Redemption.Projectiles.Melee;
using Redemption.Base;
using Redemption.Globals.NPC;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class EaglecrestJavelin_Proj : ModProjectile
    {
        public float[] oldrot = new float[4];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eaglecrest Javelin");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
        }

        public override bool? CanHitNPC(NPC target) => !target.friendly && Projectile.ai[0] >= 1;

        private float glow;
        public override void AI()
        {
            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = Projectile.rotation;

            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            if (Main.myPlayer == Projectile.owner)
            {
                if (Projectile.ai[0] == 0)
                {
                    Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
                    Projectile.Center = new Vector2(playerCenter.X, playerCenter.Y - 16);
                    player.heldProj = Projectile.whoAmI;
                    player.itemTime = 20;
                    player.itemAnimation = 20;
                    Projectile.spriteDirection = player.direction;
                    Projectile.rotation = MathHelper.PiOver2 * player.direction;

                    glow += 0.015f;
                    glow = MathHelper.Clamp(glow, 0, 0.4f);
                    if (glow >= 0.4 && Projectile.localAI[0] == 0)
                    {
                        DustHelper.DrawCircle(Projectile.Center, DustID.Sandnado, 2, 2, 2, 1, 2, nogravity: true);
                        SoundEngine.PlaySound(SoundID.Item88, Projectile.position);
                        Projectile.localAI[0] = 1;
                    }
                    if (!player.channel)
                    {
                        if (Projectile.localAI[0] == 1)
                        {
                            Projectile.ai[0] = 1;
                            SoundEngine.PlaySound(SoundID.Item19, Projectile.position);
                            Projectile.velocity = RedeHelper.PolarVector(22, (Main.MouseWorld - player.Center).ToRotation());
                        }
                        else
                            Projectile.Kill();
                    }
                }
                if (Projectile.ai[0] >= 1)
                {
                    Projectile.tileCollide = true;
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                    Projectile.velocity.Y += 0.2f;
                }
            }

            if (Projectile.ai[0] == 0)
            {
                player.itemRotation = MathHelper.ToRadians(-90f * player.direction);
                player.bodyFrame.Y = 5 * player.bodyFrame.Height;
            }
            else if (Projectile.ai[0] < 31)
                player.itemRotation -= MathHelper.ToRadians(-20f * player.direction);
        }
        public override void Kill(int timeLeft)
        {
            if (Projectile.ai[0] >= 1)
            {
                Player player = Main.player[Projectile.owner];
                if (Projectile.DistanceSQ(player.Center) < 800 * 800)
                    player.GetModPlayer<ScreenPlayer>().ScreenShakeIntensity = 5;

                SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, Projectile.position);
                for (int i = 0; i < 10; i++)
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone,
                        -Projectile.velocity.X * 0.01f, -Projectile.velocity.Y * 0.6f, Scale: 2);
                for (int i = 0; i < 10; i++)
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Sandnado,
                        -Projectile.velocity.X * 0.01f, -Projectile.velocity.Y, Scale: 2);
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            int shader = ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;
            float scale = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1.2f, 1.1f, 1.2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
                Color color = new Color(255, 180, 0) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos + new Vector2(8, 8), null, color * glow, oldrot[k], origin, Projectile.scale * scale, spriteEffects, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}