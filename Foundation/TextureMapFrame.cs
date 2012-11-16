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
using OpenTKWrapper;
using OpenTK;
using Cloo;
using System.IO;
namespace Perceptual.Foundation
{
    public class TextureMapFrame : CameraCaptureFrame<float>
    {
        public TextureMapFrame(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.data = new float[2 * width * height];
            this.FrameType = FrameType.TEXTURE;
            this.Buffer = new ComputeBuffer<float>(CLCalc.Program.Context, ComputeMemoryFlags.CopyHostPointer | ComputeMemoryFlags.ReadWrite, data);
        }
        protected float2 GetImageValue2D(int i, int j)
        {
            int index = (int)(i + j * Width);
            return new float2(data[2 * index], data[2 * index + 1]);
        }
        public float2 MapToColorImage(float2 pt, ColorCameraFrame frame)
        {

            int w = frame.Width;
            int h = frame.Height;
            int y0, x0, y1, x1;
            float dx, dy, hx, hy;
            x1 = (int)Math.Ceiling(pt.x);
            y1 = (int)Math.Ceiling(pt.y);
            x0 = (int)Math.Floor(pt.x);
            y0 = (int)Math.Floor(pt.y);
            dx = pt.x - x0;
            dy = pt.y - y0;
            // Introduce more variables to reduce computation
            hx = 1.0f - dx;
            hy = 1.0f - dy;
            // Optimized below
            float2 p00 = GetImageValue2D(x0, y0);
            float2 p01 = GetImageValue2D(x0, y1);
            float2 p11 = GetImageValue2D(x1, y1);
            float2 p10 = GetImageValue2D(x1, y0);
            return new float2(
                    w * ((p00.x * hx + p10.x * dx) * hy + (p10.x * hx + p11.x * dx) * dy),
                    h * ((p00.y * hx + p10.y * dx) * hy + (p10.y * hx + p11.y * dx) * dy));
        }

        public override void WriteToFile(string path)
        {

            if (streams.Count() >= 10)
            {
                CloseFirstOpenStream();
            }
            FileStream dest = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 1, true);
            AddHeader(dest);
            byte[] bytes = new byte[4 * data.Length];
            int index = 0;
            foreach (float val in data)
            {
                byte[] buffer = System.BitConverter.GetBytes(val);
                bytes[index++] = buffer[0];
                bytes[index++] = buffer[1];
                bytes[index++] = buffer[2];
                bytes[index++] = buffer[3];
            }
            dest.BeginWrite(bytes, 0, bytes.Length, null, null);
            streams.Enqueue(dest);
        }
    }
}
