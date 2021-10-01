using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Buffs.NPCBuffs;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Globals.NPC;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Melee
{
    public class ArcticWind_Proj : ModProjectile
    {
        public override string Texture => "Redemption/Textures/IceFlake";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arctic Wind");
        }
        public override void SetDefaults()
        {
            Projectile.width = 160;
            Projectile.height = 160;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 40;
            Projectile.alpha = 255;
            Projectile.GetGlobalProjectile<RedeGlobalProjectile>().Unparryable = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.rotation += 0.1f * player.direction;
            Projectile.Center = player.Center;

            for (int i = 0; i < 30; i++)
            {
                float distance = Main.rand.Next(25) * 4;
                Vector2 dustRotation = new Vector2(distance, 0f).RotatedBy(MathHelper.ToRadians(i * 12));
                Vector2 dustPosition = Projectile.Center + dustRotation;
                Vector2 nextDustPosition = Projectile.Center + dustRotation.RotatedBy(MathHelper.ToRadians(-4));
                Vector2 dustVelocity = (dustPosition - nextDustPosition + Projectile.velocity) * player.direction;
                if (Main.rand.NextBool(10))
                {
                    Dust dust = Dust.NewDustPerfect(dustPosition, DustID.IceTorch, dustVelocity, 0, Scale: 1.5f);
                    dust.noGravity = true;
                    dust.noLight = true;
                    dust.alpha += 10;
                    dust.rotation = dustRotation.ToRotation();
                }
            }

            if (Projectile.timeLeft >= 30)
                Projectile.alpha -= 6;

            if (Projectile.timeLeft <= 20)
            {
                Projectile.alpha += 3;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }

            foreach (NPC target in Main.npc.Take(Main.maxNPCs))
            {
                if (!target.active || target.friendly)
                    continue;

                if (!Projectile.Hitbox.Intersects(target.Hitbox))
                    continue;

                target.AddBuff(ModContent.BuffType<PureChillDebuff>(), 300);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            SpriteEffects effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale * 0.8f, effects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}