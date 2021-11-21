using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Terraria.GameContent;

namespace Redemption.NPCs.Bosses.Cleaver
{
    public class CleaverClone : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantom Cleaver");
        }
        public override void SetDefaults()
        {
            Projectile.width = 98;
            Projectile.height = 280;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }
        public float rot;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.5f / 255f, (255 - Projectile.alpha) * 0f / 255f, (255 - Projectile.alpha) * 0f / 255f);
            switch (Projectile.localAI[0])
            {
                case 0:
                    Projectile.rotation = Projectile.DirectionTo(player.Center).ToRotation() + 1.57f;
                    rot = Projectile.rotation;
                    Projectile.alpha -= 5;
                    Projectile.velocity *= 0.97f;
                    if (Projectile.velocity.Length() < 1 && Projectile.alpha <= 0)
                    {
                        Projectile.velocity *= 0;
                        Projectile.localAI[0] = 1;
                    }
                    break;
                case 1:
                    Projectile.hostile = true;
                    Projectile.rotation = rot;
                    rot.SlowRotation((float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.57f, (float)Math.PI / 30f);
                    if (Projectile.localAI[1] == 0)
                    {
                        Projectile.velocity = Projectile.DirectionTo(player.Center) * 30;
                        Projectile.localAI[1] = 1;
                    }
                    Projectile.alpha += 3;
                    if (Projectile.alpha >= 255)
                    {
                        Projectile.Kill();
                    }
                    break;
            }
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            //target.AddBuff(ModContent.BuffType<SnippedDebuff>(), Main.expertMode ? 1200 : 600);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.Center(), targetHitbox.Size(), Projectile.Center, Projectile.Center + (Projectile.rotation + -MathHelper.PiOver2).ToRotationVector2() * 140, 58, ref collisionPoint);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Red) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            return true;
        }
    }
    public class CleaverCloneF : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/Cleaver/CleaverClone";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantom Cleaver");
        }
        public override void SetDefaults()
        {
            Projectile.width = 98;
            Projectile.height = 280;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }
        public float rot;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.5f / 255f, (255 - Projectile.alpha) * 0f / 255f, (255 - Projectile.alpha) * 0f / 255f);
            switch (Projectile.localAI[0])
            {
                case 0:
                    Projectile.rotation = Projectile.DirectionTo(Main.MouseWorld).ToRotation() + 1.57f;
                    rot = Projectile.rotation;
                    Projectile.alpha -= 5;
                    Projectile.velocity *= 0.97f;
                    if (Projectile.velocity.Length() < 1 && Projectile.alpha <= 0)
                    {
                        Projectile.velocity *= 0;
                        Projectile.localAI[0] = 1;
                    }
                    break;
                case 1:
                    Projectile.friendly = true;
                    Projectile.rotation = rot;
                    rot.SlowRotation((float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.57f, (float)Math.PI / 30f);
                    if (Projectile.localAI[1] == 0)
                    {
                        Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * 30;
                        Projectile.localAI[1] = 1;
                    }
                    Projectile.alpha += 3;
                    if (Projectile.alpha >= 255)
                    {
                        Projectile.Kill();
                    }
                    break;
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.Center(), targetHitbox.Size(), Projectile.Center, Projectile.Center + (Projectile.rotation + -MathHelper.PiOver2).ToRotationVector2() * 140, 58, ref collisionPoint);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Red) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            return true;
        }
    }
    public class CleaverClone2_Spawner : ModProjectile
    {
        public override string Texture => "Redemption/Empty";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantom Cleaver");
        }
        public override void SetDefaults()
        {
            Projectile.width = 98;
            Projectile.height = 280;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.timeLeft = 30;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.localAI[0] % 3 == 0 && Main.myPlayer == Projectile.owner)
            {
                Projectile.localAI[1] += 5;
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), new Vector2(Projectile.localAI[1], 0), Vector2.Zero, ModContent.ProjectileType<CleaverClone2>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), new Vector2(-Projectile.localAI[1], 0), Vector2.Zero, ModContent.ProjectileType<CleaverClone2>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }
    }
    public class CleaverClone2 : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/Cleaver/CleaverClone";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantom Cleaver");
        }
        public override void SetDefaults()
        {
            Projectile.width = 98;
            Projectile.height = 280;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }
        public float rot;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.5f / 255f, (255 - Projectile.alpha) * 0f / 255f, (255 - Projectile.alpha) * 0f / 255f);
            Projectile.rotation = (float)Math.PI;
            switch (Projectile.localAI[0])
            {
                case 0:
                    Projectile.alpha -= 5;
                    Projectile.velocity *= 0.97f;
                    if (Projectile.velocity.Length() < 1 && Projectile.alpha <= 0)
                    {
                        Projectile.velocity *= 0;
                        Projectile.localAI[0] = 1;
                    }
                    break;
                case 1:
                    Projectile.hostile = true;
                    if (Projectile.localAI[1] == 0)
                    {
                        Projectile.velocity.Y = 20;
                        Projectile.localAI[1] = 1;
                    }
                    Projectile.alpha += 3;
                    if (Projectile.alpha >= 255)
                    {
                        Projectile.Kill();
                    }
                    break;
            }
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
           // target.AddBuff(ModContent.BuffType<SnippedDebuff>(), Main.expertMode ? 1200 : 600);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.Center(), targetHitbox.Size(), Projectile.Center, Projectile.Center + (Projectile.rotation + -MathHelper.PiOver2).ToRotationVector2() * 140, 58, ref collisionPoint);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Red) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            return true;
        }
    }
}