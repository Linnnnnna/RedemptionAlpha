using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Materials.HM;
using Redemption.Globals.Player;
using Redemption.Items.Accessories.HM;
using Terraria.Audio;

namespace Redemption.Tiles.Ores
{
    public class UraniumTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            DustType = DustID.Electric;
            ItemDrop = ModContent.ItemType<Uranium>();
            MinPick = 210;
            MineResist = 7f;
            SoundType = SoundID.Tink;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Uranium");
            AddMapEntry(new Color(77, 240, 107), name);
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.0f;
            g = 0.4f;
            b = 0.0f;
        }
        public override void NearbyEffects(int i, int j, bool closer)
        {

            Player player = Main.LocalPlayer;
            Radiation modPlayer = player.GetModPlayer<Radiation>();
            var dist = (int)Vector2.Distance(player.Center / 16, new Vector2(i, j));
            if (dist <= 15 && dist > 8) //&& !modPlayer.hazmatPower && !modPlayer.HEVPower)
            {
                if (player.GetModPlayer<MullerEffect>().effect && Main.rand.NextBool(100) && !Main.dedServ)
                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/Muller1").WithVolume(.9f).WithPitchVariance(.1f), player.position);

                if (Main.rand.NextBool(80000) && modPlayer.irradiatedLevel < 2)
                    modPlayer.irradiatedLevel++;
            }
            else if (dist <= 8 && dist > 2) //&& !modPlayer.hazmatPower && !modPlayer.HEVPower)
            {
                if (player.GetModPlayer<MullerEffect>().effect && Main.rand.NextBool(100) && !Main.dedServ)
                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/Muller2").WithVolume(.9f).WithPitchVariance(.1f), player.position);

                if (Main.rand.NextBool(40000) && modPlayer.irradiatedLevel < 2)
                    modPlayer.irradiatedLevel++;
            }
            else if (dist <= 2) //&& !modPlayer.HEVPower)
            {
                if (player.GetModPlayer<MullerEffect>().effect && Main.rand.NextBool(100) && !Main.dedServ)
                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/Muller3").WithVolume(.9f).WithPitchVariance(.1f), player.position);
                if (Main.rand.NextBool(8000) && modPlayer.irradiatedLevel < 2)
                    modPlayer.irradiatedLevel++;
            }
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
        public override bool CanExplode(int i, int j)
        {
            return false;
        }
    }
}