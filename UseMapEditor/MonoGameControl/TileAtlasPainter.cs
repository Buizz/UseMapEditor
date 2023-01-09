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
using static UseMapEditor.Control.MapEditor;

namespace UseMapEditor.MonoGameControl
{
    public class TileAtlasPainter
    {
        private TileSet tileSet;
        private GraphicsDevice gd;

        public TileAtlasPainter(TileSet tileSet, GraphicsDevice GraphicsDevice)
        {
            this.tileSet = tileSet;
            this.gd = GraphicsDevice;

            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.TextureEnabled = true;
        }

        private BasicEffect basicEffect;

        private Vector3 camera2DScrollPosition = new Vector3(0, 0, -1);
        private Vector3 camera2DScrollLookAt = new Vector3(0, 0, 0);
        private void SetCameraPosition2D(int x, int y)
        {
            camera2DScrollPosition.X = x;
            camera2DScrollPosition.Y = y;
            camera2DScrollPosition.Z = -1;
            camera2DScrollLookAt.X = x;
            camera2DScrollLookAt.Y = y;
            camera2DScrollLookAt.Z = 0;
        }

        public void Draw(Vector2 screen, MapEditor mapeditor, TileMap tileMap, bool IsCustomBlend = false, float alpha = 1f)
        {
            //타일맵에 기록된 이미지를 그린다.
            //Effect effect = Global.WindowTool.MapViewer.shader;

            float scale = (float)mapeditor.opt_scalepercent;

            if (mapeditor.opt_drawType == DrawType.SD)
            {
                screen /= 2;
                gd.SetVertexBuffer(tileMap.SDVertextData.vertexBuffer);
                gd.Indices = tileMap.SDVertextData.indexBuffer;
            }
            else
            {
                scale /= 2;
                gd.SetVertexBuffer(tileMap.HDVertextData.vertexBuffer);
                gd.Indices = tileMap.HDVertextData.indexBuffer;
            }

            SetCameraPosition2D(-(int)screen.X, -(int)screen.Y);

            if (IsCustomBlend)
            {
                basicEffect.TextureEnabled = false;
                basicEffect.VertexColorEnabled = true;
            }
            else
            {
                basicEffect.TextureEnabled = true;
                basicEffect.VertexColorEnabled = false;
            }
            basicEffect.Alpha = alpha;

            gd.SamplerStates[0] = SamplerState.PointClamp;

            Texture2D texture2D = tileSet.GetAtlasTileSetTexture(mapeditor.opt_drawType, mapeditor.mapdata.TILETYPE).GetTexture();

            // set up our matrix to match basic effect.
            Viewport viewport = gd.Viewport;
            basicEffect.Texture = texture2D;
            basicEffect.World = Matrix.Identity;
            Vector3 cameraUp = Vector3.Transform(new Vector3(0, -1, 0), Matrix.CreateRotationZ(0f));
            basicEffect.View = Matrix.CreateLookAt(camera2DScrollPosition, camera2DScrollLookAt, cameraUp);
            basicEffect.Projection = Matrix.CreateScale(scale, -scale, 1f) * Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, 1);


            //basicEffect.EnableDefaultLighting();

            //basicEffect.LightingEnabled = true;


            foreach (var pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                gd.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 256 * 256 * 2);
            }
        }
    }
}
