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
using Perceptual.Foundation;
using OpenTKWrapper;
using OpenTKWrapper.CLGLInterop;
namespace Perceptual.Visualization
{
    public abstract class ImageButtonRender : PercRender
    {
        public List<ImageButton> Buttons = new List<ImageButton>();

        protected int initScreenWidth, initScreenHeight;
        protected float scaleX = 1, scaleY = 1;
        public virtual void Initialize(BaseCameraApplication app, GLAdvancedRender glw)
        {
            initScreenWidth = glw.GLCtrl.Width;
            initScreenHeight = glw.GLCtrl.Height;
        }
        public bool Visible = true;
        public void Draw(GLAdvancedRender glw)
        {
            scaleX = initScreenWidth / (float)glw.GLCtrl.Width;
            scaleY = initScreenHeight / (float)glw.GLCtrl.Height;
            if (current != null)
            {
                glw.GLCtrl.Cursor = System.Windows.Forms.Cursors.Hand;
            }
            else
            {
                glw.GLCtrl.Cursor = System.Windows.Forms.Cursors.Arrow;
            }
            if (Visible)
            {
                lock (this)
                {
                    foreach (ImageButton b in Buttons)
                    {
                        b.Draw();
                    }
                }
            }
        }
        protected ImageButton current = null;
        public void ProcessMouseMove(int xpos, int ypos)
        {
            if (Visible)
            {
                xpos = (int)Math.Round(xpos * scaleX);
                ypos = (int)Math.Round(ypos * scaleY);
                lock (this)
                {
                    bool found = false;
                    foreach (ImageButton button in Buttons)
                    {
                        if (button.MouseOver(xpos, ypos))
                        {
                            current = button;
                            current.SetRollover(true);
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {

                        if (current != null)
                        {
                            current.SetRollover(false);
                        }

                        current = null;
                    }
                }
            }
        }
        public void ProcessMouseButton(bool mouseDown)
        {
            if (Visible)
            {
                lock (this)
                {
                    if (mouseDown)
                    {
                        if (current != null)
                        {
                            current.SetSelected(true);
                        }
                    }
                    else
                    {
                        if (current != null)
                        {
                            current.SetSelected(false);
                            current.SetRollover(false);
                        }
                    }
                }
            }
        }
        public abstract void Process(BaseCameraApplication app);


    }
}
