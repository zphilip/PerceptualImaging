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
namespace PercFoundation
{
    public class DepthCameraFrame : CameraCaptureFrame<float>
    {
        public DepthCameraFrame(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.data = new float[4 * width * height];
            this.FrameType = FrameType.DEPTH;
            this.Buffer = new ComputeBuffer<float>(CLCalc.Program.Context, ComputeMemoryFlags.CopyHostPointer | ComputeMemoryFlags.ReadWrite, data);
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
