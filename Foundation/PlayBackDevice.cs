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
using System.IO;
using OpenTKWrapper;
using System.Runtime.InteropServices;
namespace PercFoundation
{
    public class PlayBackDevice:CameraDevice
    {
        protected int counter = 0;
        protected string inputDirectory;
        protected string[] depthFiles;
        protected string[] colorFiles;
        protected string[] textureFiles;
        protected string[] faceFiles;
        protected int numFrames = 0;
        public PlayBackDevice(string inputDirectory):base(inputDirectory){
            this.inputDirectory = inputDirectory;
            this.focalX = 224.502f;
            this.focalY = 230.494f;

        }
        public override bool Initialize()
        {
            depthFiles = Directory.GetFiles(inputDirectory, "*.depth");
            colorFiles = Directory.GetFiles(inputDirectory, "*.color");
            textureFiles = Directory.GetFiles(inputDirectory, "*.texture");
            faceFiles = Directory.GetFiles(inputDirectory, "*.face");
            numFrames=Math.Max(Math.Max(depthFiles.Length,colorFiles.Length),textureFiles.Length);
            return (numFrames>0);
        }
        public override void Dispose()
        {
            counter = 0;   
        }
        public override unsafe void GetNextFrame()
        {
            if (counter < depthFiles.Length)
            {
                depthFrame = (DepthCameraFrame)(ReadFromFile<float>(depthFiles[counter]));
            }
            if (counter < colorFiles.Length)
            {
                colorFrame = (ColorCameraFrame)(ReadFromFile<byte>(colorFiles[counter]));
            }
            if (counter < textureFiles.Length)
            {
                textureFrame = (TextureMapFrame)(ReadFromFile<float>(textureFiles[counter]));
            }
            counter++;
            if (counter == numFrames)
            {
                counter = 0;
                //System.Console.WriteLine("No More Frames!");
                //Environment.Exit(1);
            }
        }
        public CameraCaptureFrame<DataType> ReadFromFile<DataType>(string file) where DataType:struct
        {
            using (Stream stream = File.OpenRead(file))
            {
                byte[] buffer = new byte[4];
                stream.Read(buffer, 0, buffer.Length);
                int t = (int)System.BitConverter.ToInt32(buffer, 0);
                stream.Read(buffer, 0, buffer.Length);
                int w = (int)System.BitConverter.ToInt32(buffer, 0);
                stream.Read(buffer, 0, buffer.Length);
                int h = (int)System.BitConverter.ToInt32(buffer, 0);
                stream.Read(buffer, 0, buffer.Length);
                int d = (int)System.BitConverter.ToInt32(buffer, 0);
                //System.Console.WriteLine("File: {0} : [{1},{2},{3},{4}]", file,t, w, h, d);
                
                switch (t)
                {
                    case (int)FrameType.COLOR: 
                        if(colorFrame==null)colorFrame=new ColorCameraFrame(w,h);
                        stream.Read(colorFrame.data, 0, colorFrame.data.Length);
                        stream.Close();
                        ((CLCalc.Program.Image2D)colorFrame.GetMemoryObject()).WriteToDevice(colorFrame.data);
                        return colorFrame as CameraCaptureFrame<DataType>;
              
                    case (int)FrameType.DEPTH:
                        if(depthFrame==null)depthFrame = new DepthCameraFrame(w,h);
                        for (int i = 0; i < depthFrame.data.Length; i++)
                        {
                            stream.Read(buffer, 0, buffer.Length);
                            depthFrame.data[i] = System.BitConverter.ToSingle(buffer,0);
                        }

                        stream.Close();
                        ((CLCalc.Program.Variable)depthFrame.GetMemoryObject()).WriteToDevice(depthFrame.data);
                        return depthFrame as CameraCaptureFrame<DataType>;
                    case (int)FrameType.TEXTURE:
                        if(textureFrame==null)textureFrame = new TextureMapFrame(w,h);
                        for (int i = 0; i < textureFrame.data.Length; i++)
                        {
                            stream.Read(buffer, 0, buffer.Length);
                            textureFrame.data[i] = System.BitConverter.ToSingle(buffer,0);
                        }
                        stream.Close();
                        ((CLCalc.Program.Variable)textureFrame.GetMemoryObject()).WriteToDevice(textureFrame.data);
                        return textureFrame as CameraCaptureFrame<DataType>;
                    default: return null;
                        
                }
            }
        }
        public override void RecordCurrentFrame()
        {
            //Do Nothing! Data is already recorded!
            //throw new NotImplementedException();
        }
    }
}
