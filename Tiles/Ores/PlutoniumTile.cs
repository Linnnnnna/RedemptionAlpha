using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Materials.PostML;
using Redemption.Globals.Player;
using Redemption.Items.Accessories.HM;
using Terraria.Audio;
using Redemption.BaseExtension;

namespace Redemption.Tiles.Ores
{
    public class PlutoniumTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileShine2[Type] = true;
            Main.tileShine[Type] = 975;
            TileID.Sets.Ore[Type] = true;
            Main.tileOreFinderPriority[Type] = 680;
            DustType = DustID.Electric;
            MinPick = 220;
            MineResist = 6f;
            HitSound = SoundID.Tink;
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Plutonium");
            AddMapEntry(new Color(133, 253, 255), name);
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.2f;
            g = 0.2f;
            b = 0.4f;
        }
        public override void NearbyEffects(int i, int j, bool closer)
        {
            Player player = Main.LocalPlayer;
            Radiation modPlayer = player.RedemptionRad();
            BuffPlayer suit = player.RedemptionPlayerBuff();
            var dist = (int)Vector2.Distance(player.Center / 16, new Vector2(i, j));
            if (dist <= 30 && dist > 18 && !suit.hazmatSuit && !suit.HEVSuit)
            {
                if (player.GetModPlayer<MullerEffect>().effect && Main.rand.NextBool(100) && !Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.Muller1, player.position);

                if (Main.rand.NextBool(80000) && modPlayer.irradiatedLevel < 2)
                    modPlayer.irradiatedLevel++;
            }
            else if (dist <= 18 && dist > 10 && !suit.hazmatSuit && !suit.HEVSuit)
            {
                if (player.GetModPlayer<MullerEffect>().effect && Main.rand.NextBool(100) && !Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.Muller2, player.position);

                if (Main.rand.NextBool(40000) && modPlayer.irradiatedLevel < 3)
                    modPlayer.irradiatedLevel++;
            }
            else if (dist <= 10 && dist > 4 && !suit.HEVSuit)
            {
                if (player.GetModPlayer<MullerEffect>().effect && Main.rand.NextBool(100) && !Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.Muller3, player.position);

                if (Main.rand.NextBool(8000) && modPlayer.irradiatedLevel < 3)
                    modPlayer.irradiatedLevel++;
            }
            else if (dist <= 4)
            {
                if (player.GetModPlayer<MullerEffect>().effect && Main.rand.NextBool(100) && !Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.Muller4, player.position);

                if (Main.rand.NextBool(1000) && modPlayer.irradiatedLevel < 3)
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