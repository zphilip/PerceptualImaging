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
using OpenTKWrapper.CLGLInterop;
using System.IO;
namespace Perceptual.Foundation
{
    public enum FrameType
    {
        COLOR = 1, DEPTH = 2, TEXTURE = 4
    }
    public abstract class CameraCaptureFrame<T> where T : struct
    {
        public T[] data;
        public int Width;
        public int Height;
       
        public ComputeBuffer<T> Buffer;
        protected CLCalc.Program.MemoryObject Variable = null;
        protected FrameType FrameType;
        public CLCalc.Program.MemoryObject GetMemoryObject()
        {
            if (Variable == null) Variable = new CLCalc.Program.Variable(Buffer, (int)Buffer.Size, (int)Buffer.Count);
            return Variable;
        }
        public abstract void WriteToFile(string file);
        protected void AddHeader(FileStream stream)
        {
            byte[] w = System.BitConverter.GetBytes((Int32)Width);
            byte[] h = System.BitConverter.GetBytes((Int32)Height);
            byte[] d = System.BitConverter.GetBytes((Int32)(data.Length / (Width * Height)));
            byte[] t = System.BitConverter.GetBytes((Int32)FrameType);
            stream.BeginWrite(t, 0, t.Length, null, null);
            stream.BeginWrite(w, 0, w.Length, null, null);
            stream.BeginWrite(h, 0, h.Length, null, null);
            stream.BeginWrite(d, 0, d.Length, null, null);
        }

        protected void AddHeader(Stream stream)
        {
            byte[] w = System.BitConverter.GetBytes((Int32)Width);
            byte[] h = System.BitConverter.GetBytes((Int32)Height);
            byte[] d = System.BitConverter.GetBytes((Int32)(data.Length / (Width * Height)));
            byte[] t = System.BitConverter.GetBytes((Int32)FrameType);
            stream.Write(t, 0, t.Length);
            stream.Write(w, 0, w.Length);
            stream.Write(h, 0, h.Length);
            stream.Write(d, 0, d.Length);
        }

        protected Queue<FileStream> streams = new Queue<FileStream>();
        public void CloseAllOpenStreams()
        {
            while (streams.Count() > 0)
            {
                FileStream stream = streams.Dequeue();
                stream.Close();
            }
        }
        public void CloseFirstOpenStream()
        {
            if(streams.Count() > 0)
            {
                FileStream stream = streams.Dequeue();
                stream.Close();
            }
        }
    }
}
