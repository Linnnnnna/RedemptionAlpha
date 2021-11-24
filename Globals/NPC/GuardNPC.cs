using Microsoft.Xna.Framework;
using Redemption.Globals.Player;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.NPCs.Friendly;
using Redemption.NPCs.PreHM;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Globals.NPC
{
    public class GuardNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;
        public int GuardPoints;
        public bool IgnoreArmour;
        public bool GuardBroken;

        public void GuardHit(Terraria.NPC npc, ref double damage, LegacySoundStyle sound, float dmgReduction = 0.25f)
        {
            if (IgnoreArmour || npc.HasBuff(BuffID.BrokenArmor) || GuardPoints < 0 || GuardBroken)
                return;

            damage = (int)(damage * dmgReduction);
            SoundEngine.PlaySound(sound, npc.position);
            CombatText.NewText(npc.getRect(), Colors.RarityPurple, (int)damage, true, true);
            GuardPoints -= (int)damage;
            damage = 0;
            IgnoreArmour = false;
        }
        public void GuardBreakCheck(Terraria.NPC npc, int dustType, LegacySoundStyle sound, int dustAmount = 10, float dustScale = 1)
        {
            if (GuardPoints > 0 || GuardBroken)
                return;

            SoundEngine.PlaySound(sound, npc.position);
            CombatText.NewText(npc.getRect(), Colors.RarityPurple, "Guard Broken!", false, true);
            for (int i = 0; i < dustAmount; i++)
                Dust.NewDust(npc.position + npc.velocity, npc.width, npc.height, dustType, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f, Scale: dustScale);
            GuardBroken = true;
        }
        public override void ModifyHitByItem(Terraria.NPC npc, Terraria.Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            if (GuardPoints <= 0)
                return;

            if (ItemTags.Psychic.Has(item.type))
                IgnoreArmour = true;
            if (item.hammer > 0)
                damage *= 3;
        }
        public override void ModifyHitByProjectile(Terraria.NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (GuardPoints <= 0)
                return;

            if (ProjectileTags.Psychic.Has(projectile.type))
                IgnoreArmour = true;
        }
    }
}