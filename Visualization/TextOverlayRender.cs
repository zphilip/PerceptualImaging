/*******************************************************************************

INTEL CORPORATION PROPRIETARY INFORMATION
This software is supplied under the terms of a license agreement or nondisclosure
agreement with Intel Corporation and may not be copied or disclosed except in
accordance with the terms of that agreement
Copyright(c) 2012 Intel Corporation. All Rights Reserved.

@Author {Blake C. Lucas (img.science@gmail.com)}
*******************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using System.Drawing.Imaging;
using OpenTKWrapper.CLGLInterop;
using OpenTKWrapper;
using OpenTK.Graphics.OpenGL;
using Perceptual.Foundation;
namespace Perceptual.Visualization
{
    public class Message
    {
        protected string text;
        public Font font;
        public PointF location;
        public SolidBrush brush;
        protected bool dirty = true;
        public bool visible = true;
        public Message(string text, int x, int y, int sz)
        {
            this.text = text;
            this.font = new Font("Arial", sz);
            this.location = new PointF(x, y);
            brush = new SolidBrush(Color.White);
        }
        public bool isVisible()
        {
            return visible;
        }
        public void SetVisible(bool visible)
        {
            if (this.visible != visible) dirty = true;
            this.visible = visible;
        }
        public void SetText(string text)
        {
            if (!this.text.Equals(text))
            {
                this.text = text;
                this.dirty = true;
            }
        }
        public bool isDirty()
        {
            return dirty;
        }
        public void Draw(Graphics gfx)
        {
            if (visible) gfx.DrawString(text, font, brush, location); // Draw as many strings as you need
            dirty = false;
        }
    }
    public class TextOverlayRender : PercRender
    {

        public List<Message> Messages = new List<Message>();
        protected Bitmap textImage;
        protected int textTextureId;

        public TextOverlayRender()
        {
        }
        public void Process(BaseCameraApplication capture)
        {
            GL.BindTexture(TextureTarget.Texture2D, textTextureId);
            bool dirty = false;
            using (Graphics gfx = Graphics.FromImage(textImage))
            {
                gfx.Clear(Color.Transparent);

                foreach (Message message in Messages)
                {
                    if (message.isDirty())
                    {
                        dirty = true;
                        message.Draw(gfx);
                    }
                }
            }
            if (dirty)
            {
                BitmapData data = textImage.LockBits(new Rectangle(0, 0, textImage.Width, textImage.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, textImage.Width, textImage.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                textImage.UnlockBits(data);
            }
            GL.BindTexture(TextureTarget.Texture2D, 0);

        }
        public virtual void Initialize(BaseCameraApplication capture, GLAdvancedRender glw)
        {
            // Create Bitmap and OpenGL texture
            textImage = new Bitmap(glw.GLCtrl.Width, glw.GLCtrl.Height); // match window size
            textTextureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textTextureId);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);

            using (Graphics gfx = Graphics.FromImage(textImage))
            {
                gfx.Clear(Color.Transparent);

            }
            BitmapData data = textImage.LockBits(new Rectangle(0, 0, textImage.Width, textImage.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, textImage.Width, textImage.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            textImage.UnlockBits(data);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Draw(GLAdvancedRender glw)
        {
            if (textTextureId >= 0)
            {

                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

                GL.Enable(EnableCap.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, textTextureId);
                GL.Begin(BeginMode.Quads);
                GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(0f, 0f);
                GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(textImage.Width, 0f);
                GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(textImage.Width, textImage.Height);
                GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(0f, textImage.Height);
                GL.End();

                GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.Disable(EnableCap.Texture2D);
            }
        }

    }
}
