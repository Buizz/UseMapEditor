using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseMapEditor.MonoGameControl;
using static UseMapEditor.Control.MapEditorData.EditorTextureData;
using static UseMapEditor.FileData.TileSet;

namespace UseMapEditor.Control.MapEditorData
{
    public class EditorTextureData
    {
        public MapEditor mapEditor;
        public void Init(MapDrawer mapDrawer, MapEditor mapEditor)
        {
            this.mapEditor = mapEditor;

            minimap = new Texture2D(mapDrawer.GraphicsDevice, 256, 256);
            minimapUnit = new Texture2D(mapDrawer.GraphicsDevice, 256, 256);
            SDTileMap = new TileMap(mapDrawer.GraphicsDevice);
            HDTileMap = new TileMap(mapDrawer.GraphicsDevice);
            CBTileMap = new TileMap(mapDrawer.GraphicsDevice);


            SDTilePalletMap = new TileMap(mapDrawer.GraphicsDevice);
            HDTilePalletMap = new TileMap(mapDrawer.GraphicsDevice);
            CBTilePalletMap = new TileMap(mapDrawer.GraphicsDevice);
        }

        public TileMap GetTileMap(MapEditor.DrawType drawType = MapEditor.DrawType.NOTHING)
        {
            if (drawType == MapEditor.DrawType.NOTHING)
            {
                drawType = mapEditor.opt_drawType;
            }
            switch (drawType)
            {
                case MapEditor.DrawType.SD:
                    return SDTileMap;
                case MapEditor.DrawType.HD:
                    return HDTileMap;
                case MapEditor.DrawType.CB:
                    return CBTileMap;
            }

            return null;
        }

        public TileMap GetTilePalletMap(MapEditor.DrawType drawType = MapEditor.DrawType.NOTHING)
        {
            if (drawType == MapEditor.DrawType.NOTHING)
            {
                drawType = mapEditor.opt_drawType;
            }
            switch (drawType)
            {
                case MapEditor.DrawType.SD:
                    return SDTilePalletMap;
                case MapEditor.DrawType.HD:
                    return HDTilePalletMap;
                case MapEditor.DrawType.CB:
                    return CBTilePalletMap;
            }

            return null;
        }


        public TileMap SDTileMap;
        public TileMap HDTileMap;
        public TileMap CBTileMap;

        public TileMap SDTilePalletMap;
        public TileMap HDTilePalletMap;
        public TileMap CBTilePalletMap;

        public class TileMap
        {
            public AtlasTileSet atlasTileSet;

            public VertexBuffer vertexBuffer;
            public IndexBuffer indexBuffer;

            public VertexPositionTexture[] vertices;
            public int[] indices;
            public void SetData()
            {
                vertexBuffer.SetData(vertices);
                indexBuffer.SetData(indices);
            }

            public TileMap(GraphicsDevice gd)
            {
                vertices = new VertexPositionTexture[256 * 256 * 4];
                indices = new int[256 * 256 * 6];

                vertexBuffer = new VertexBuffer(gd, typeof(VertexPositionTexture), 256 * 256 * 4,
    BufferUsage.WriteOnly);
                indexBuffer = new IndexBuffer(gd, typeof(int), 256 * 256 * 6, BufferUsage.WriteOnly);
            }
        }

        public Texture2D minimap;
        public Texture2D minimapUnit;
    }
}
