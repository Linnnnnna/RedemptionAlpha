using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Hostile
{
    public class DevilsTongueCloud : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pollen Cloud");
        }
        public override void SetDefaults()
        {
            Projectile.width = 136;
            Projectile.height = 132;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.timeLeft = 120;
            Projectile.GetGlobalProjectile<RedeProjectile>().Unparryable = true;
        }
        public override void AI()
        {
            Projectile.velocity *= 0.98f;
            Projectile.rotation += Projectile.velocity.X / 50;
            if (Projectile.timeLeft > 100)
                Projectile.alpha -= 2;
            else
            {
                Projectile.alpha += 2;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
            if (Projectile.ai[0] == 0)
            { 
                Projectile.ai[0] = 1;
            }
            for (int p = 0; p < Main.maxPlayers; p++)
            {
                Player target = Main.player[p];
                if (!target.active || target.dead)
                    continue;

                if (!Projectile.Hitbox.Intersects(target.Hitbox))
                    continue;

                target.AddBuff(ModContent.BuffType<DevilScentedDebuff>(), 360);
                target.AddBuff(BuffID.Confused, 40);
            }
            foreach (NPC target in Main.npc.Take(Main.maxNPCs))
            {
                if (!target.active)
                    continue;

                if (!Projectile.Hitbox.Intersects(target.Hitbox))
                    continue;

                target.AddBuff(ModContent.BuffType<DevilScentedDebuff>(), 360);
                target.AddBuff(BuffID.Confused, 40);
            }
        }
    }
}