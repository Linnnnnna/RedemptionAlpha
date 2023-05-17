using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using SubworldLibrary;
using Redemption.Tiles.Tiles;
using Terraria.WorldBuilding;
using Terraria.IO;

namespace Redemption.WorldGeneration.Misc
{
    public class PlaygroundSub : Subworld
    {
        public override int Width => 2000;
        public override int Height => 2000;
        public override bool NormalUpdates => false;
        public override bool ShouldSave => false;
        public override bool NoPlayerSaving => false;
        public override List<GenPass> Tasks => new()
        {
            new ThingPass1("Loading", 1)
        };
        public override void Load()
        {
            Main.cloudAlpha = 0;
            Main.numClouds = 0;

            Main.dayTime = false;
            Main.time = 0;
            Main.rainTime = 0;
            Main.raining = false;
            Main.maxRaining = 0f;
        }
    }
    public class ThingPass1 : GenPass
    {
        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Loading";
            Main.spawnTileY = 1000;
            Main.spawnTileX = 1000;
            Main.worldSurface = 1040; // Where underground starts
            Main.rockLayer = 1200; // Where rock layer starts

            // BTW this code was generated by OpenAI as a test, it doesnt actually create spikes though, just uses perlin noise
            // Generate a noise map using FastNoise
            int mapWidth = Main.maxTilesX;
            int mapHeight = Main.maxTilesY;
            FastNoiseLite noise = new();
            noise.SetSeed(WorldGen.genRand.Next());
            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            noise.SetFrequency(0.01f);
            noise.SetFractalOctaves(4);
            float[,] noiseMap = new float[mapWidth, mapHeight];
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    noiseMap[x, y] = noise.GetNoise(x, y);
                }
            }

            // Iterate through the noise map and add spikes
            int spikeLength = 10; // Set the length of the spikes
            int spikeHeight = 10; // Set the height of the spikes
            float spikeThreshold = 0.6f; // Set the threshold for adding spikes
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    // Check if the current position meets the threshold for adding a spike
                    if (noiseMap[x, y] > spikeThreshold)
                    {
                        // Add a spike
                        for (int i = 0; i < spikeLength; i++)
                        {
                            int spikeX = x + i;
                            int spikeY = y - spikeHeight + i;
                            if (spikeX < mapWidth && spikeY < mapHeight)
                            {
                                if (WorldGen.InWorld(spikeX, spikeY))
                                    WorldGen.PlaceTile(spikeX, spikeY, ModContent.TileType<ShinkiteBrickTile>(), true);
                            }
                        }
                    }
                }
            }
        }
        public ThingPass1(string name, float loadWeight) : base(name, loadWeight)
        {
        }
    }
}