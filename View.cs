using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CG_LAB_2
{
    class View
    {
        Bitmap textureImage;
        int VBOtexture = -1;

        public void SetupView(int width, int height)
        {
            GL.ShadeModel(ShadingModel.Smooth);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, Bin.X, 0, Bin.Y, -1, 1);
            GL.Viewport(0, 0, width, height);

            GL.GenTextures(1, out VBOtexture);
        }

        private Color TransferFunction(short value, int minTF, int widthTF)
        {
            if (widthTF <= 0) widthTF = 1;
            int tfValue = (value - minTF) * 255 / widthTF;
            tfValue = Math.Max(0, Math.Min(255, tfValue));
            return Color.FromArgb(255, tfValue, tfValue, tfValue);
        }

        public void DrawQuads(int layerNumber, int minTF, int widthTF)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            for (int y_coord = 0; y_coord < Bin.Y - 1; y_coord++)
            {
                GL.Begin(PrimitiveType.QuadStrip);
                for (int x_coord = 0; x_coord < Bin.X; x_coord++)
                {
                    short value1 = Bin.array[x_coord + y_coord * Bin.X + layerNumber * Bin.X * Bin.Y];
                    short value2 = Bin.array[x_coord + (y_coord + 1) * Bin.X + layerNumber * Bin.X * Bin.Y];

                    GL.Color3(TransferFunction(value1, minTF, widthTF));
                    GL.Vertex2(x_coord, y_coord);

                    GL.Color3(TransferFunction(value2, minTF, widthTF));
                    GL.Vertex2(x_coord, y_coord + 1);
                }
                GL.End();
            }
        }

        public void generateTextureImage(int layerNumber, int minTF, int widthTF)
        {
            textureImage = new Bitmap(Bin.X, Bin.Y);
            for (int i = 0; i < Bin.X; i++)
            {
                for (int j = 0; j < Bin.Y; j++)
                {
                    int pixelNumber = i + j * Bin.X + layerNumber * Bin.X * Bin.Y;
                    textureImage.SetPixel(i, j, TransferFunction(Bin.array[pixelNumber], minTF, widthTF));
                }
            }
        }

        public void Load2DTexture()
        {
            if (VBOtexture == -1)
            {
                GL.GenTextures(1, out VBOtexture);
            }

            GL.BindTexture(TextureTarget.Texture2D, VBOtexture);
            BitmapData data = textureImage.LockBits(
                new Rectangle(0, 0, textureImage.Width, textureImage.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte, data.Scan0);

            textureImage.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Linear);
        }

        public void DrawTexture()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, VBOtexture);

            GL.Begin(PrimitiveType.Quads);
            GL.Color3(Color.White);
            GL.TexCoord2(0f, 0f);
            GL.Vertex2(0, 0);
            GL.TexCoord2(0f, 1f);
            GL.Vertex2(0, Bin.Y);
            GL.TexCoord2(1f, 1f);
            GL.Vertex2(Bin.X, Bin.Y);
            GL.TexCoord2(1f, 0f);
            GL.Vertex2(Bin.X, 0);
            GL.End();

            GL.Disable(EnableCap.Texture2D);
        }
    }
}

