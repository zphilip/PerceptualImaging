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
using System.Drawing.Imaging;
using Cloo;
using OpenTKWrapper;
using System.IO;
namespace Perceptual.Foundation
{
    public class ColorCameraFrame:CameraCaptureFrame<byte>
    {
        public Bitmap bitmap;


        public ColorCameraFrame(byte[] data){
            this.data=data;
            this.FrameType = FrameType.COLOR;
        }
        public Bitmap getBitmap()
        {
            return bitmap;
        }
        public ColorCameraFrame(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.data = new byte[4 * width * height];
            this.FrameType = FrameType.COLOR;
            this.Variable = new CLCalc.Program.Image2D(data, (int)width, (int)height);
           

        }
        public unsafe void Set(Bitmap bitmap)
        {
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData data = bitmap.LockBits(rect, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            this.data = new byte[bitmap.Width * bitmap.Height*4];
            byte* rgb = (byte*)data.Scan0.ToPointer();
            for (int x = 0; x < bitmap.Width * bitmap.Height; x++)
            {
                this.data[4*x] = rgb[3*x];
                this.data[4 * x+1] = rgb[3*x+1];
                this.data[4 * x+2] = rgb[3*x+2];
                this.data[4 * x + 3] = 255;
            }
               
            bitmap.UnlockBits(data);
            
        }
        public override void WriteToFile(string path)
        {
            if (streams.Count() >= 10)
            {
                CloseFirstOpenStream();
            }
            using (Stream dest = File.Create(path))
            {
                AddHeader(dest);
                
                dest.Write(data,0,data.Length);
                dest.Close();
            }

        }
    }
    
}
