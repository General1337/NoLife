﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TileEngine
{
    public class ShipLayer
    {
        List<Texture2D> tileTextures = new List<Texture2D>();
        int[,] shipBlocks;

        static int tileHeight = 32; // Tiles for movement shall be 32x32 but ship size perhaps 16x16?
        static int tileWidth = 32; // how big tiles are, also affects camera logic layers must be same 

        public int WidthInPixels
        {
            get { return shipBlocks.GetLength(1) * TileWidth; }
        }
        public int HeightInPixels
        {
            get { return shipBlocks.GetLength(0) * TileHeight; }
        }
        public static int TileWidth
        {
            get { return tileWidth; }
            set { tileWidth = (int)MathHelper.Clamp(value, 20f, 100f); }
        }
        public static int TileHeight
        {
            get { return tileHeight; }
            set { tileHeight = (int)MathHelper.Clamp(value, 20f, 100f); }
        }

        public ShipLayer(int width, int height)
        {
            shipBlocks = new int[height, width];
        }

        public ShipLayer(int[,] existingMap)
        {
            shipBlocks = (int[,])existingMap.Clone();
        }
        public static TileLayer FromFile(ContentManager content, string filename)
        {
            TileLayer tileLayer;
            bool readingTextures = false;
            bool readingLayout = false;
            List<string> textureNames = new List<string>();
            List<List<int>> tempLayout = new List<List<int>>(); // inner list = single row, list of rows is grid
            using (StreamReader reader = new StreamReader(filename))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Trim();

                    if (string.IsNullOrEmpty(line))
                        continue;
                    if (line.Contains("[Textures]"))
                    {
                        readingTextures = true;
                        readingLayout = false;
                    }
                    else if (line.Contains("[Layout]"))
                    {
                        readingLayout = true;
                        readingTextures = false;
                    }
                    else if (readingTextures)
                    {
                        textureNames.Add(line);
                    }
                    else if (readingLayout)
                    {
                        List<int> row = new List<int>();

                        string[] cells = line.Split(' ');

                        foreach (string c in cells)
                        {
                            if (!string.IsNullOrEmpty(c))
                                row.Add(int.Parse(c));
                        }

                        tempLayout.Add(row);
                    }
                }
            }

            int width = tempLayout[0].Count;
            int height = tempLayout.Count; // # of rows

            tileLayer = new TileLayer(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    tileLayer.SetCellIndex(x, y, tempLayout[y][x]);
                }
            }
            tileLayer.LoadTileTextures(content, textureNames.ToArray());

            return tileLayer;
        }

        public void LoadTileTextures(ContentManager content, params string[] textureNames)
        {
            Texture2D texture;
            foreach (string textureName in textureNames)
            {
                texture = content.Load<Texture2D>(textureName);
                tileTextures.Add(texture);
            }
        }
        public void SetCellIndex(int x, int y, int cellIndex)
        {
            shipBlocks[y, x] = cellIndex;
        }
        public void Draw(SpriteBatch batch, Camera camera)
        {
            batch.Begin();
            int tileMapWidth = shipBlocks.GetLength(1);
            int tileMapHeight = shipBlocks.GetLength(0);

            for (int x = 0; x < tileMapWidth; x++)
            {
                for (int y = 0; y < tileMapHeight; y++)
                {
                    int textureIndex = shipBlocks[y, x];
                    if (textureIndex == -1)
                    {
                        continue;
                    }
                    Texture2D texture = tileTextures[textureIndex];

                    batch.Draw(texture, new Rectangle(
                            x * TileWidth - (int)camera.Position.X,
                            y * TileHeight - (int)camera.Position.Y,
                            TileWidth,
                            TileHeight),
                        Color.White); // subtract to set camera position if cam goes down (larger) subtract so tile map shifts up
                }
      

            }
            batch.End();
        }
    }
}