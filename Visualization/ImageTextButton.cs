
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTKWrapper.CLGLInterop;
using OpenTKWrapper;
using OpenTK.Graphics.OpenGL;
using Perceptual.Foundation;
namespace Perceptual.Visualization
{
    public class ImageTextButton : ImageButton
    {
        public Message message;
        public ImageTextButton(Message message, bool active, int xpos, int ypos, int width, int height)
        {
            this.name = name;
            this.active = active;
            bitmap = new Bitmap(width, height);
            this.width = width;
            this.height = height;
            this.xpos = xpos;
            this.ypos = ypos;
            this.message = message;
            GL.Enable(EnableCap.Texture2D);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);
            GL.GenTextures(1, out texture);
            if (texture != -1)
            {
                GL.BindTexture(TextureTarget.Texture2D, texture);
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);
                System.Drawing.Imaging.BitmapData bitmapdata = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height,
                    0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bitmapdata.Scan0);
                bitmap.UnlockBits(bitmapdata);
                GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.Disable(EnableCap.Texture2D);

            }
            update();
        }
        public void update()
        {
            using (Graphics gfx = Graphics.FromImage(bitmap))
            {
                Color c = message.brush.Color;             
                gfx.Clear(Color.FromArgb(0,c.R,c.G,c.B));
                message.Draw(gfx);
            }

            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, texture);
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.Disable(EnableCap.Texture2D);
        }
    }
}
