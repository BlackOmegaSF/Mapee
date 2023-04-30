﻿using CommonUtilities.Factory;
using MapScanner;
using System.Windows;
using WorldEditor;

namespace Mapper
{
    public class MapRenderer : IMapRenderer<MapRenderArgs>
    {
        public IChunkRenderer<ChunkRenderArgs> ChunkRenderer { get; set; }
        public IFactory<CanvasArgs, ICanvas> CanvasFactory { get; set; }

        public ILogger<ChunkError> Logger { get; set; }

        public MapRenderer(IChunkRenderer<ChunkRenderArgs> chunkRenderer)
        {
            ChunkRenderer = chunkRenderer;
            CanvasFactory = new CanvasFactory();
            Logger = new ConsoleWriteLogger<ChunkError>();
        }

        public void Render(MapRenderArgs input, out ICanvas canvas)
        {
            GetDimensions(input.Chunks, out Coords topLeft, out Size size);
            canvas = CanvasFactory.Create(new CanvasArgs(topLeft, size, Direction.North));

            ICanvas local = canvas;
            Parallel.For(0, input.Chunks.Count, i =>
            {
                try
                {
                    ChunkRenderer.Render(new ChunkRenderArgs(input.Chunks[i], input.StepProvider), local);
                }
                catch (Exception e)
                {
                    Logger.Log(new ChunkError(e, new Coords(input.Chunks[i].Coords.X, input.Chunks[i].Coords.Z)));
                }
            });
        }

        private static void GetDimensions(IList<IScannedChunk> chunks, out Coords topLeft, out Size size)
        {
            int topLeftX = int.MinValue, topLeftZ = int.MaxValue;
            int bottomRightX = int.MaxValue, bottomRightZ = int.MinValue;

            for (int i = 0; i < chunks.Count; i++)
            {
                IScannedChunk chunk = chunks[i];

                if (chunk.Coords.X > topLeftX) topLeftX = chunk.Coords.X;
                if (chunk.Coords.Z < topLeftZ) topLeftZ = chunk.Coords.Z;
                if (chunk.Coords.X < bottomRightX) bottomRightX = chunk.Coords.X;
                if (chunk.Coords.Z > bottomRightZ) bottomRightZ = chunk.Coords.Z;
            }

            topLeft = new Coords(topLeftX * 16 + 15, topLeftZ * 16);
            size = new Size((bottomRightZ - topLeftZ) * 16 + 16, (topLeftX - bottomRightX) * 16 + 16);
        }
    }
}
