using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.X3DAudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseMapEditor.Control;
using UseMapEditor.Control.MapEditorData;
using UseMapEditor.FileData;
using static UseMapEditor.Control.MapEditorData.EditorTextureData;

namespace UseMapEditor.MonoGameControl
{
    public class TileAtlasBuffer
    {
  
        public Texture2D texture;
 



        private BasicEffect basicEffect;
        private bool IsHD;
        private GraphicsDevice GraphicsDevice;

        private int blockSize;
        private int texturewidth;
        private int textureheight;

        public TileAtlasBuffer(Texture2D texture, GraphicsDevice GraphicsDevice, bool IsHD)
        {
            int tiles = 256 * 256;

            this.IsHD = IsHD;
            if (IsHD)
            {
                blockSize = 64;
            }
            else
            {
                blockSize = 32;
            }
            texturewidth = texture.Width / blockSize;
            textureheight = texture.Height / blockSize;


            this.GraphicsDevice = GraphicsDevice;
            this.texture = texture;

            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.TextureEnabled = true;
            basicEffect.Texture = texture;

            //if (texture != null)
            //{
            //    //AddTile(10, 10, new Rectangle(64, 64, 256, 256), false, false);
            //    for (int y = 0; y < 256; y++)
            //    {
            //        for (int x = 0; x < 10; x++)
            //        {
            //            _settile(x + y * 256, x * blockSize, y * blockSize, new Rectangle(blockSize * 10, blockSize * 10, blockSize, blockSize), false, false);
            //        }
            //    }

            //    for (int i = 0; i < 10; i++)
            //    {
            //        if (IsHD)
            //        {
            //            _settile(i, i * 64, 64 * 128, new Rectangle(64 * i, 64 * i, 64, 64), false, false);
            //        }
            //        else
            //        {
            //            _settile(i, i * 32, 32 * 128, new Rectangle(32 * i, 32 * i, 32, 32), false, false);
            //        }
            //    }
            //    SetData();
            //}
        }

        public  Vector3 camera2DScrollPosition = new Vector3(0, 0, -1);
        public  Vector3 camera2DScrollLookAt = new Vector3(0, 0, 0);

        private void SetCameraPosition2D(int x, int y)
        {
            camera2DScrollPosition.X = x;
            camera2DScrollPosition.Y = y;
            camera2DScrollPosition.Z = -1;
            camera2DScrollLookAt.X = x;
            camera2DScrollLookAt.Y = y;
            camera2DScrollLookAt.Z = 0;
        }
        public void Draw(MapEditor mapeditor, TileMap tileMap)
        {
            float scale = (float)mapeditor.opt_scalepercent;
            Vector2 mapPos;
            scale /= 2;
            mapPos = mapeditor.PosMapToScreen(new Vector2(0, 0), 2);


            SetCameraPosition2D(-(int)mapPos.X, -(int)mapPos.Y);

            GraphicsDevice.SetVertexBuffer(tileMap.vertexBuffer);
            GraphicsDevice.Indices = tileMap.indexBuffer;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            // set up our matrix to match basic effect.
            Viewport viewport = GraphicsDevice.Viewport;
            basicEffect.World = Matrix.Identity;
            Vector3 cameraUp = Vector3.Transform(new Vector3(0, -1, 0), Matrix.CreateRotationZ(0f));
            basicEffect.View = Matrix.CreateLookAt(camera2DScrollPosition, camera2DScrollLookAt, cameraUp);
            // We could set up the world maxtrix this way and get the expected rotation but its not really proper.
            //basicEffect.World = Matrix.Identity * Matrix.CreateRotationZ(camera2DrotationZ);
            //basicEffect.View = Matrix.CreateLookAt(camera2DScrollPosition, camera2DScrollLookAt, new Vector3(0, -1, 0));
            basicEffect.Projection = Matrix.CreateScale(scale, -scale, 1f) * Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, 1);


            foreach (var pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 256 * 256 * 2);
            }
        }

        public void RefreshTileSet(MapEditor mapeditor)
        {
            for (int y = 0; y < mapeditor.mapdata.HEIGHT; y++)
            {
                for (int x = 0; x < mapeditor.mapdata.WIDTH; x++)
                {
                    SetTIleFromMTXM(mapeditor, x, y, mapeditor.mapdata.GetTILE(x, y));
                }
            }
            //for (int y = 0; y < 10; y++)
            //{
            //    for (int x = 0; x < 10; x++)
            //    {
            //        SetTIleFromMTXM(mapeditor, x, 1, mapeditor.mapdata.GetTILE(x, y));
            //    }
            //}
            //SetData(mapeditor);
        }

        public void SetTIleFromMTXM(MapEditor mapeditor, int x, int y, ushort MTXM)
        {
            int tileindex = x + y * 256;

            ushort magatile = Global.WindowTool.MapViewer.tileSet.GetMegaTileIndex(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE, MTXM);
            int magax = magatile % texturewidth;
            int magay = magatile / textureheight;

            _settile(mapeditor, tileindex, x * blockSize, y * blockSize, new Rectangle(blockSize * magax, blockSize * magay, blockSize, blockSize), false, false);
        }


        private void _settile(MapEditor mapeditor,int tileindex, int tx, int ty, Rectangle rect, bool flippedHorizontally, bool flippedVertically)
        {
            float textureSizeX = 1f / texture.Width;
            float textureSizeY = 1f / texture.Height;

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
            mapeditor.editorTextureData.GetTileMap().vertices[vertexCount] = new VertexPositionTexture(new Vector3(tx, ty + rect.Height, 0), new Vector2(left, bottom));
            mapeditor.editorTextureData.GetTileMap().vertices[vertexCount + 1] = new VertexPositionTexture(new Vector3(tx, ty, 0), new Vector2(left, top));
            mapeditor.editorTextureData.GetTileMap().vertices[vertexCount + 2] = new VertexPositionTexture(new Vector3(tx + rect.Width, ty + rect.Height, 0), new Vector2(right, bottom));
            mapeditor.editorTextureData.GetTileMap().vertices[vertexCount + 3] = new VertexPositionTexture(new Vector3(tx + rect.Width, ty, 0), new Vector2(right, top));

            int indexCount = tileindex * 6;
            mapeditor.editorTextureData.GetTileMap().indices[indexCount] = (int)vertexCount;
            mapeditor.editorTextureData.GetTileMap().indices[indexCount + 1] = (int)(vertexCount + 1);
            mapeditor.editorTextureData.GetTileMap().indices[indexCount + 2] = (int)(vertexCount + 2);

            mapeditor.editorTextureData.GetTileMap().indices[indexCount + 3] = (int)(vertexCount + 2);
            mapeditor.editorTextureData.GetTileMap().indices[indexCount + 4] = (int)(vertexCount + 1);
            mapeditor.editorTextureData.GetTileMap().indices[indexCount + 5] = (int)(vertexCount + 3);

            //tileCount++;
        }
    }
}
