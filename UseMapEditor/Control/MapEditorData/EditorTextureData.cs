using KGySoft.CoreLibraries;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseMapEditor.FileData;
using UseMapEditor.MonoGameControl;
using static UseMapEditor.Control.MapEditor;
using static UseMapEditor.Control.MapEditorData.EditorTextureData;
using static UseMapEditor.FileData.CImage;
using static UseMapEditor.FileData.TileSet;

namespace UseMapEditor.Control.MapEditorData
{
    public class EditorTextureData
    {
        public MapEditor mapeditor;
        public MapDrawer mapDrawer;
        public void Init(MapDrawer mapDrawer, MapEditor mapEditor)
        {
            this.mapeditor = mapEditor;
            this.mapDrawer = mapDrawer;

            minimap = new Texture2D(mapDrawer.GraphicsDevice, 256, 256);
            minimapUnit = new Texture2D(mapDrawer.GraphicsDevice, 256, 256);
            tileMap = new TileMap(mapDrawer);
            tilePaletteMap = new TileMap(mapDrawer);
        }

        public Texture2D minimap;
        public Texture2D minimapUnit;

        public TileMap tileMap;
        public TileMap tilePaletteMap;

        public void TilePaletteRefresh()
        {
            tilePaletteMap.Clear();


            int width = 0;
            int height = 0;

            if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.ALLTILE)
            {
                width = (int)(mapeditor.tile_PalleteSelectEnd.X - mapeditor.tile_PalleteSelectStart.X) + 1;
                height = (int)(mapeditor.tile_PalleteSelectEnd.Y - mapeditor.tile_PalleteSelectStart.Y) + 1;
            }
            else if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.PASTE)
            {
                width = (int)mapeditor.tile_CopyedTileSize.X;
                height = (int)mapeditor.tile_CopyedTileSize.Y;
            }

            bool IsOneTile = false;
            if (width == 1 && height == 1)
            {
                IsOneTile = true;
                width = mapeditor.brush_x;
                height = mapeditor.brush_y;
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    ushort MTXM = 0;
                    if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.ALLTILE)
                    {
                        int group, index;
                        group = (int)mapeditor.tile_PalleteSelectStart.Y;
                        index = (int)mapeditor.tile_PalleteSelectStart.X;


                        if (!IsOneTile)
                        {
                            group += y;
                            index += x;
                        }
                        MTXM = (ushort)(group * 16 + index);
                    }
                    else if (mapeditor.tile_BrushMode == Control.MapEditor.TileSetBrushMode.PASTE)
                    {
                        if (IsOneTile)
                        {
                            ushort newMtxm = mapeditor.Tile_GetCopyedTile(0, 0);
                            if (newMtxm == ushort.MaxValue) continue;
                            MTXM = newMtxm;
                        }
                        else
                        {
                            ushort newMtxm = mapeditor.Tile_GetCopyedTile(x, y);
                            if (newMtxm == ushort.MaxValue) continue;
                            MTXM = newMtxm;
                        }
                    }
                    int megaindex = mapDrawer.tileSet.GetMegaTileIndex(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE, MTXM);

                    if (mapeditor.TilePalleteTransparentBlack && megaindex == 0) continue;
                    mapeditor.editorTextureData.tilePaletteMap.SetTIleFromMTXM(mapeditor.mapdata.TILETYPE, x, y, MTXM);
                }
            }

            mapeditor.editorTextureData.tilePaletteMap.Apply();
        }


        public class TileMap
        {
            public MapDrawer mapDrawer;
            public class VertextData
            {
                public VertexBuffer vertexBuffer;
                public IndexBuffer indexBuffer;
                public VertexPositionColorTexture[] vertices;
                public int[] indices;

                public void Clear()
                {
                    vertices = new VertexPositionColorTexture[256 * 256 * 4];
                    indices = new int[256 * 256 * 6];
                }
                public void SetData()
                {
                    vertexBuffer.SetData(vertices);
                    indexBuffer.SetData(indices);
                }

                public VertextData(GraphicsDevice gd)
                {
                    vertices = new VertexPositionColorTexture[256 * 256 * 4];
                    indices = new int[256 * 256 * 6];

                    vertexBuffer = new VertexBuffer(gd, typeof(VertexPositionColorTexture), 256 * 256 * 4,
                        BufferUsage.WriteOnly);
                    indexBuffer = new IndexBuffer(gd, typeof(int), 256 * 256 * 6, BufferUsage.WriteOnly);
                }
            }

            public VertextData SDVertextData;
            public VertextData HDVertextData;

            public TileMap(MapDrawer mapDrawer)
            {
                this.mapDrawer = mapDrawer;

                SDVertextData = new VertextData(mapDrawer.GraphicsDevice);
                HDVertextData = new VertextData(mapDrawer.GraphicsDevice);
            }

            public void Clear()
            {
                SDVertextData.Clear();
                HDVertextData.Clear();
            }

            public void AllTileDraw(MapEditor mapEditor)
            {
                SDVertextData.Clear();
                HDVertextData.Clear();
                for (int x = 0; x < mapEditor.mapdata.WIDTH; x++)
                {
                    for (int y = 0; y < mapEditor.mapdata.HEIGHT; y++)
                    {
                        SetTIleFromMTXM(mapEditor.mapdata.TILETYPE, x, y, mapEditor.mapdata.GetTILE(x, y));
                    }
                }
            }
            public void Apply()
            {
                SDVertextData.SetData();
                HDVertextData.SetData();
            }

            public void SetTIleFromMTXM(TileType tileType, int x, int y, ushort MTXM)
            {
                int tileindex = x + y * 256;

                {
                    ushort magatile = mapDrawer.tileSet.GetMegaTileIndex(DrawType.SD, tileType, MTXM);
                    Texture2D texture2D = mapDrawer.tileSet.GetAtlasTileSetTexture(DrawType.SD, tileType).GetTexture();
                    int magax = magatile % (texture2D.Width / 32);
                    int magay = magatile / (texture2D.Height / 32);
                    _settile(texture2D, SDVertextData, tileindex, x * 32, y * 32, new Rectangle(32 * magax, 32 * magay, 32, 32), false, false);
                }

                {
                    ushort magatile = mapDrawer.tileSet.GetMegaTileIndex(DrawType.HD, tileType, MTXM);
                    Texture2D texture2D = mapDrawer.tileSet.GetAtlasTileSetTexture(DrawType.HD, tileType).GetTexture();
                    int magax = magatile % (texture2D.Width / 64);
                    int magay = magatile / (texture2D.Height / 64);
                    _settile(texture2D, HDVertextData, tileindex, x * 64, y * 64, new Rectangle(64 * magax, 64 * magay, 64, 64), false, false);
                }
            }
            private void _settile(Texture2D texture2D, VertextData vertex, int tileindex, int tx, int ty, Rectangle rect, bool flippedHorizontally, bool flippedVertically)
            {
                float textureSizeX = 1f / texture2D.Width;
                float textureSizeY = 1f / texture2D.Height;

                float left = rect.Left * textureSizeX;
                float right = rect.Right * textureSizeX;
                float bottom = rect.Bottom * textureSizeY;
                float top = rect.Top * textureSizeY;

                if (flippedHorizontally)
                {
                    float temp = left;
                    left = right;
                    right = temp;
                }
                if (flippedVertically)
                {
                    float temp = top;
                    top = bottom;
                    bottom = temp;
                }


                int vertexCount = tileindex * 4;
                vertex.vertices[vertexCount] = new VertexPositionColorTexture(new Vector3(tx, ty + rect.Height, 0), Color.BlueViolet, new Vector2(left, bottom));
                vertex.vertices[vertexCount + 1] = new VertexPositionColorTexture(new Vector3(tx, ty, 0), Color.BlueViolet, new Vector2(left, top));
                vertex.vertices[vertexCount + 2] = new VertexPositionColorTexture(new Vector3(tx + rect.Width, ty + rect.Height, 0), Color.BlueViolet, new Vector2(right, bottom));
                vertex.vertices[vertexCount + 3] = new VertexPositionColorTexture(new Vector3(tx + rect.Width, ty, 0), Color.BlueViolet, new Vector2(right, top));

                //vertex.vertices[vertexCount] = new VertexPositionColor(new Vector3(tx, ty + rect.Height, 0), Color.Blue);
                //vertex.vertices[vertexCount + 1] = new VertexPositionColor(new Vector3(tx, ty, 0), Color.Blue);
                //vertex.vertices[vertexCount + 2] = new VertexPositionColor(new Vector3(tx + rect.Width, ty + rect.Height, 0), Color.Blue);
                //vertex.vertices[vertexCount + 3] = new VertexPositionColor(new Vector3(tx + rect.Width, ty, 0), Color.Blue);



                //vertex.vertices[vertexCount] = new VertexPositionTexture(new Vector3(tx, ty + rect.Height, 0), new Vector2(left, bottom));
                //vertex.vertices[vertexCount + 1] = new VertexPositionTexture(new Vector3(tx, ty, 0), new Vector2(left, top));
                //vertex.vertices[vertexCount + 2] = new VertexPositionTexture(new Vector3(tx + rect.Width, ty + rect.Height, 0), new Vector2(right, bottom));
                //vertex.vertices[vertexCount + 3] = new VertexPositionTexture(new Vector3(tx + rect.Width, ty, 0), new Vector2(right, top));




                int indexCount = tileindex * 6;
                vertex.indices[indexCount] = (int)vertexCount;
                vertex.indices[indexCount + 1] = (int)(vertexCount + 1);
                vertex.indices[indexCount + 2] = (int)(vertexCount + 2);

                vertex.indices[indexCount + 3] = (int)(vertexCount + 2);
                vertex.indices[indexCount + 4] = (int)(vertexCount + 1);
                vertex.indices[indexCount + 5] = (int)(vertexCount + 3);
            }
        }
    }
}
