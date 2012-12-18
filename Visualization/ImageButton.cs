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
using OpenTKWrapper.CLGLInterop;
using OpenTKWrapper;
using OpenTK.Graphics.OpenGL;
using Perceptual.Foundation;
namespace Perceptual.Visualization
{
    public interface ImageButtonObserver
    {
        void OnMouseOver(ImageButton button);
        void OnMouseClick(ImageButton button);
    }
    public class ImageButton
    {
        public Bitmap bitmap;
        protected bool rollover = false;
        protected int texture = -1;
        public int xpos, ypos, width, height;
        protected bool visible = true;
        protected bool active = true;
        protected bool selected = false;
        protected string name;
        protected bool dynamicVisible = false;
        protected bool highlight = false;
        protected bool enabled = true;
        protected bool dragAndDrop = false;
        protected Color4 selectedColor = new Color4(1, 1, 1, -1);
        public int minX = -1, minY = -1, maxX = -1, maxY = -1;
        protected Color4 tint = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
        protected Color4 bgColor = new Color4(1.0f, 1.0f, 1.0f, 0.0f);
        protected Color4 fgColor = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
        public void SetTint(Color4 c)
        {
            this.tint = c;
        }
        public void SetSelectedColor(Color4 c)
        {
            this.selectedColor = c;
        }
        public void SetBackgroundColor(Color4 c)
        {
            this.bgColor = c;
        }
        public void SetForegroundColor(Color4 c)
        {
            this.fgColor = c;
        }
        public Color4 GetForegroundColor()
        {
            return fgColor;
        }
        public Color4 GetBackgroundColor()
        {
            return bgColor;
        }
        public int2 GetLocation()
        {
            return new int2(xpos, ypos);
        }
        public int2 GetSize()
        {
            return new int2(width, height);
        }
        public void SetDragBoundingBox(int minx, int miny, int maxx, int maxy)
        {
            this.minX = minx;
            this.minY = miny;
            this.maxX = maxx;
            this.maxY = maxy;
        }
        public int2 ClampPosition(int x, int y)
        {
            if (minX < 0 || minY < 0 || maxX < 0 || maxY < 0)
            {
                return new int2(x, y);
            }
            else
            {
                return new int2(Math.Max(minX, Math.Min(x, maxX)), Math.Max(minY, Math.Min(y, maxY)));
            }
        }
        protected List<ImageButtonObserver> observers = new List<ImageButtonObserver>();
        public void AddObserver(ImageButtonObserver observer)
        {
            observers.Add(observer);
        }
        public string GetName()
        {
            return name;
        }
        public void SetVisible(bool visible)
        {
            this.visible = visible;
        }
        public void SetDynamicImageVisible(bool visible)
        {
            this.dynamicVisible = visible;
        }
        public void SetHighlighted(bool highlight)
        {
            this.highlight = highlight;
        }
        public void SetEnabled(bool enabled)
        {
            this.enabled = enabled;
        }
        public void SetDragAndDrop(bool dnd)
        {
            this.dragAndDrop = dnd;
        }
        public bool isDragEnabled()
        {
            return dragAndDrop;
        }
        public bool isSelected()
        {
            return selected;
        }
        public void SetSelected(bool selected)
        {

            if (this.selected == true && selected == false)
            {
                foreach (ImageButtonObserver observer in observers)
                {
                    observer.OnMouseClick(this);
                }
            }
            this.selected = selected;
        }
        public bool isVisible()
        {
            return visible;
        }
        public void SetRollover(bool rollover)
        {
            this.rollover = rollover;
            if (rollover)
            {
                foreach (ImageButtonObserver observer in observers)
                {
                    observer.OnMouseOver(this);
                }
            }
        }
        protected int textureId = -1;
        CLCalc.Program.Image2D image = null;
        public CLCalc.Program.Image2D GetImage2D()
        {
            return image;
        }
        public void SetDynamicImage(CLCalc.Program.Image2D image, int textureId)
        {
            this.textureId = textureId;
            this.image = image;
            this.dynamicVisible = true;
        }
        public void SetDynamicImage(CLCalc.Program.Image2D image)
        {
            this.image = image;
            this.dynamicVisible = true;
        }
        public ImageButton()
        {
        }
        public void UpdateBitmap()
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
        public ImageButton(string name, bool active, int xpos, int ypos, int width, int height)
            : this(name, null, active, xpos, ypos, width, height)
        {
        }
        public ImageButton(string name, Bitmap staticImage, bool active, int xpos, int ypos, int width, int height)
        {
            this.name = name;
            this.active = active;
            this.bitmap = staticImage;
            this.width = width;
            this.height = height;
            this.xpos = xpos;
            this.ypos = ypos;
            if (bitmap != null)
            {
                GL.Enable(EnableCap.Texture2D);
                GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
                GL.GenTextures(1, out texture);

                if (texture != -1)
                {
                    UpdateBitmap();
                }
                else
                {
                    System.Console.WriteLine("Could not create texture!");
                    Environment.Exit(1);
                }
            }
            else
            {
                texture = -1;
            }
        }
        public bool MouseOver(int x, int y)
        {
            return (enabled && active && visible && x >= xpos && y >= ypos && x <= xpos + width && y <= ypos + height);
        }
        public void Draw()
        {
            if (visible)
            {
                GL.PushMatrix();

                GL.Translate(xpos + width * 0.5f, ypos + height * 0.5f, 0);

                if (!selected && rollover)
                {
                    GL.Scale(1.1f, 1.1f, 1);
                }
                else if (selected)
                {
                    GL.Scale(0.9f, 0.9f, 1);
                }
                GL.Disable(EnableCap.Lighting);
                GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);
                GL.Disable(EnableCap.Texture2D);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                GL.Color4((selected && selectedColor.A > 0) ? selectedColor : bgColor);
                GL.Begin(BeginMode.Quads);
                GL.Vertex2(-width * 0.5f, height * 0.5f);
                GL.Vertex2(width * 0.5f, height * 0.5f);
                GL.Vertex2(width * 0.5f, -height * 0.5f);
                GL.Vertex2(-width * 0.5f, -height * 0.5f);
                GL.End();


                GL.Enable(EnableCap.Texture2D);
                GL.Color4(tint);
                if (dynamicVisible && image != null)
                {

                    if (textureId >= 0 && image.CreatedFromGLBuffer)
                    {
                        GL.BindTexture(TextureTarget.Texture2D, textureId);
                        GL.Begin(BeginMode.Quads);
                        GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(-width * 0.5f, height * 0.5f);
                        GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(width * 0.5f, height * 0.5f);
                        GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(width * 0.5f, -height * 0.5f);
                        GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(-width * 0.5f, -height * 0.5f);
                        GL.End();
                        GL.BindTexture(TextureTarget.Texture2D, 0);
                    }
                    else
                    {
                        image.ReadFromDeviceToBuffer();
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
                        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height,
                            0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.Float, image.BackingBuffer);
                        GL.Begin(BeginMode.Quads);
                        GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(-width * 0.5f, height * 0.5f);
                        GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(width * 0.5f, height * 0.5f);
                        GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(width * 0.5f, -height * 0.5f);
                        GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(-width * 0.5f, -height * 0.5f);
                        GL.End();
                    }
                }
                if (!enabled)
                {
                    GL.Color3(0.25f, 0.25f, 0.25f);
                }
                else if (highlight)
                {
                    GL.Color3(1.0f, 0.8f, 0.1f);
                }
                else
                {
                    GL.Color4(fgColor);
                }

                if (texture >= 0)
                {
                    GL.BindTexture(TextureTarget.Texture2D, texture);
                    GL.Begin(BeginMode.Quads);
                    GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(-width * 0.5f, height * 0.5f);
                    GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(width * 0.5f, height * 0.5f);
                    GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(width * 0.5f, -height * 0.5f);
                    GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(-width * 0.5f, -height * 0.5f);
                    GL.End();
                    GL.BindTexture(TextureTarget.Texture2D, 0);
                }


                GL.PopMatrix();

                GL.Disable(EnableCap.Texture2D);
                GL.Enable(EnableCap.Lighting);
                GL.Color3(1.0f, 1.0f, 1.0f);
            }
        }
    }
}
