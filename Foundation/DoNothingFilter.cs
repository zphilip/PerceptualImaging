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
using Cloo;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTKWrapper.CLGLInterop;
using OpenTKWrapper;
using System.IO;
namespace Perceptual.Foundation
{
    public class DoNothingFilter : CameraDataFilter
    {
        CLCalc.Program.Variable depthBuffer;
        CLCalc.Program.Variable depthCopyBuffer;
        CLCalc.Program.Kernel copyToTemporalBuffer;
        public CLCalc.Program.Image2D depthImage;
        protected float[] depthCopy;
        CLCalc.Program.Kernel kernelCopyImage;
        CLCalc.Program.Kernel updateBuffer;
        protected CLCalc.Program.Image2D rgbImage;
        public CLCalc.Program.Image2D uvImage;
        public CLCalc.Program.Variable GetMotionBuffer()
        {
            return depthCopyBuffer;
        }
        public CLCalc.Program.Image2D GetDepthImage()
        {
            return depthImage;
        }
        public CLCalc.Program.Image2D GetTextureImage()
        {
            return uvImage;
        }

        public CLCalc.Program.Image2D GetColorImage()
        {
            return rgbImage;
        }
        public float[] GetDepthBackBuffer()
        {
            return depthCopy;
        }
        public void Process(BaseCameraApplication capture)
        {

            DepthCameraFrame depthFrame = capture.GetDevices()[0].GetDepthImage();
            TextureMapFrame textureFrame = capture.GetDevices()[0].GetTextureImage();
            this.rgbImage = (CLCalc.Program.Image2D)capture.GetDevices()[0].GetColorImage().GetMemoryObject();
            kernelCopyImage.Execute(new CLCalc.Program.MemoryObject[] { depthFrame.GetMemoryObject(), textureFrame.GetMemoryObject(), uvImage, depthImage }, new int[] { depthFrame.Width, depthFrame.Height });
        }

        public void Initialize(BaseCameraApplication capture)
        {
            DepthCameraFrame frame = capture.GetDevices()[0].GetDepthImage();
            try
            {
                StreamReader reader = new StreamReader(new MemoryStream(Perceptual.Foundation.Properties.Resources.AdaptiveTemporalFilter));
                string text = reader.ReadToEnd();

                CLCalc.Program.Compile(capture.GetPrimaryDevice().GetPreprocessCode() + "\n#define HISTORY_SIZE 0\n" + text);
                reader.Close();

            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                System.Console.WriteLine("Could not find DoNothingFilter.cl");
                Environment.Exit(1);
            }
            updateBuffer = new CLCalc.Program.Kernel("UpdateFilter");
            copyToTemporalBuffer = new CLCalc.Program.Kernel("CopyToTemporalBuffer");
            depthBuffer = CLCalc.Program.Variable.Create(new ComputeBuffer<float>(CLCalc.Program.Context, ComputeMemoryFlags.ReadWrite, 4 * frame.Width * frame.Height));            
            depthCopyBuffer = new CLCalc.Program.Variable(new float[4 * frame.Width * frame.Height]);
         
            depthImage = new CLCalc.Program.Image2D(new float[frame.Height * frame.Width * 4], frame.Width, frame.Height);
            uvImage = new CLCalc.Program.Image2D(new float[frame.Height * frame.Width * 4], frame.Width, frame.Height);


            kernelCopyImage = new CLCalc.Program.Kernel("CopyImage");
        }

    }
}
