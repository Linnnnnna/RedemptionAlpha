using System.Linq;
using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.KSIII
{
    public class KS3_Reflect : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reflector Shield");
        }
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 58;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (!npc.active || npc.type != ModContent.NPCType<KS3>())
                Projectile.Kill();

            Vector2 Pos = new(npc.Center.X + 48 * npc.spriteDirection, npc.Center.Y - 12);
            Projectile.Center = Pos;
            if (npc.ai[3] != 6 || npc.ai[0] != 3)
                Projectile.Kill();

            foreach (Projectile target in Main.projectile)
            {
                if (Projectile == target || !target.active || target.minion || target.damage <= 0 || !target.friendly || target.hostile || target.GetGlobalProjectile<RedeGlobalProjectile>().TechnicallyMelee)
                    continue;

                if (!Projectile.Hitbox.Intersects(target.Hitbox))
                    continue;

                for (int m = 0; m < 4; m++)
                {
                    int dustID = Dust.NewDust(new Vector2(Projectile.Center.X - 1, Projectile.Center.Y - 1), 2, 2, DustID.Frost, 0f, 0f, 100, Color.White, 2f);
                    Main.dust[dustID].velocity = BaseUtility.RotateVector(default, new Vector2(4f, 0f), m / (float)4 * 6.28f);
                    Main.dust[dustID].noLight = false;
                    Main.dust[dustID].noGravity = true;
                }
                SoundEngine.PlaySound(SoundID.NPCHit34, Projectile.position);
                if (ProjectileID.Sets.CountsAsHoming[target.type])
                {
                    target.Kill();
                    continue;
                }
                if (Projectile.penetrate == 1)
                    Projectile.penetrate = 2;

                if (target.damage > 200)
                    target.damage = 200;

                target.damage /= 4;
                target.hostile = true;
                target.friendly = false;
                target.velocity = -target.velocity;

            }
        }
    }
}