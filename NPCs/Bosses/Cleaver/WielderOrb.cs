using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Cleaver
{
    public class WielderOrb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shield Orb");
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.alpha = 255;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        readonly double dist = 60; //Radius of orbit
        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }
            Projectile.timeLeft = 50;
            Player player = Main.player[Projectile.owner];
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.5f / 255f, (255 - Projectile.alpha) * 0f / 255f, (255 - Projectile.alpha) * 0f / 255f);
            if (Projectile.alpha <= 0) { Projectile.alpha = 0; }
            else { Projectile.alpha -= 3; }
            double deg = Projectile.ai[1]; //Degrees of orbit
            double rad = deg * (Math.PI / 180);
            NPC host = Main.npc[(int)Projectile.ai[0]];
            Projectile.position.X = host.Center.X - (int)(Math.Cos(rad) * dist) - Projectile.width / 2;
            Projectile.position.Y = host.Center.Y - (int)(Math.Sin(rad) * dist) - Projectile.height / 2;
            Projectile.ai[1] += 5f; //Orbit Speed
            if (host.life <= 0 || !host.active || host.type != ModContent.NPCType<Wielder>())
            {
                Projectile.Kill();
            }
            float num = 8f;
            Vector2 vector = new Vector2(Projectile.position.X + Projectile.width * 0.5f, Projectile.position.Y + Projectile.height * 0.5f);
            float hostX = host.position.X + (host.width / 2);
            float hostY = host.position.Y + (host.height / 2);
            hostX = (int)(hostX / 8f) * 8;
            hostY = (int)(hostY / 8f) * 8;
            vector.X = (int)(vector.X / 8f) * 8;
            vector.Y = (int)(vector.Y / 8f) * 8;
            hostX -= vector.X;
            hostY -= vector.Y;
            float rootXY = (float)Math.Sqrt(hostX * hostX + hostY * hostY);
            if (rootXY == 0f)
            {
                hostX = Projectile.velocity.X;
                hostY = Projectile.velocity.Y;
            }
            else
            {
                rootXY = num / rootXY;
                hostX *= rootXY;
                hostY *= rootXY;
            }
            var list = Main.projectile.Where(x => x.Hitbox.Intersects(Projectile.Hitbox));
            foreach (var proj in list)
            {
                if (Projectile != proj && proj.minionSlots == 0 && proj.friendly && proj.damage > 5)
                {
                    if (!Main.dedServ)
                    {
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/BallFire").WithPitchVariance(.9f), Projectile.position);
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.LifeDrain, 0f, 0f, 100, default, 1f);
                        dust.velocity = -Projectile.DirectionTo(dust.position) * 2f;
                    }
                    proj.Kill();
                }
            }
        }
    }
}