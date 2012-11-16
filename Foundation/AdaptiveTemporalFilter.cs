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
    public class AdaptiveTemporalFilter : CameraDataFilter
    {
        CLCalc.Program.Kernel kernelCopyBuffer, kernelDilateFilter, kernelMedianFilter1, kernelMedianFilter2, kernelErodeFilter;
        CLCalc.Program.Variable depthBuffer;
        CLCalc.Program.Variable depthTemporalBuffer;
        CLCalc.Program.Variable depthCopyBuffer;
        CLCalc.Program.Kernel copyToTemporalBuffer;
        protected const int smoothIterations = 4;
        protected const int smoothIterations2 = 3;
        protected const int erodeIterations = 1;
        protected const int dilateIterations = 2;
        protected const int historySize = 15;
        protected const int radius1 = 1, radius2 = 2;
        protected bool once = true;
        public CLCalc.Program.Image2D depthImage;
        protected float[] depthCopy;
        CLCalc.Program.Kernel kernelCopyImage;
        protected CLCalc.Program.Value<int> historyIndex;
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
        public float[] GetDepthBackBuffer()
        {
            return depthCopy;
        }
        public CLCalc.Program.Image2D GetColorImage()
        {
            return rgbImage;
        }
        public void Process(BaseCameraApplication capture)
        {

            DepthCameraFrame depthFrame = capture.GetDevices()[0].GetDepthImage();
            TextureMapFrame textureFrame = capture.GetDevices()[0].GetTextureImage();
            CLCalc.Program.MemoryObject input = depthFrame.GetMemoryObject();
            CLCalc.Program.MemoryObject output = depthBuffer;
            CLCalc.Program.MemoryObject tmp;

            this.rgbImage = (CLCalc.Program.Image2D)capture.GetDevices()[0].GetColorImage().GetMemoryObject();
            kernelCopyBuffer.Execute(new CLCalc.Program.MemoryObject[] { input, depthCopyBuffer }, new int[] { depthFrame.Width * depthFrame.Height });


            for (int cycle = 0; cycle < smoothIterations; cycle++)
            {
                kernelMedianFilter1.Execute(new CLCalc.Program.MemoryObject[] { input, output }, new int[] { depthFrame.Width, depthFrame.Height });
                tmp = input;
                input = output;
                output = tmp;
            }
            if (input != depthFrame.GetMemoryObject())
            {
                System.Console.WriteLine("Wrong Buffer!");
                Environment.Exit(1);
            }

            input = depthCopyBuffer;
            output = depthBuffer;


            for (int cycle = 0; cycle < smoothIterations2; cycle++)
            {
                kernelMedianFilter2.Execute(new CLCalc.Program.MemoryObject[] { input, output }, new int[] { depthFrame.Width, depthFrame.Height });
                tmp = input;
                input = output;
                output = tmp;
            }


            for (int cycle = 0; cycle < erodeIterations; cycle++)
            {
                kernelErodeFilter.Execute(new CLCalc.Program.MemoryObject[] { input, output }, new int[] { depthFrame.Width, depthFrame.Height });
                tmp = input;
                input = output;
                output = tmp;
            }

            for (int cycle = 0; cycle < dilateIterations; cycle++)
            {
                kernelDilateFilter.Execute(new CLCalc.Program.MemoryObject[] { input, output }, new int[] { depthFrame.Width, depthFrame.Height });
                tmp = input;
                input = output;
                output = tmp;
            }

            if (input != depthCopyBuffer)
            {
                System.Console.WriteLine("Wrong Buffer!");
                Environment.Exit(1);
            }
            if (once)
            {
                copyToTemporalBuffer.Execute(new CLCalc.Program.MemoryObject[] { depthCopyBuffer, depthFrame.GetMemoryObject(), depthTemporalBuffer, historyIndex }, new int[] { depthFrame.Width * depthFrame.Height });
            }
            else
            {
                updateBuffer.Execute(new CLCalc.Program.MemoryObject[] { depthCopyBuffer, depthFrame.GetMemoryObject(), depthTemporalBuffer, historyIndex }, new int[] { depthFrame.Width * depthFrame.Height });
            }
            historyIndex.value--;
            if (historyIndex.value < 0)
            {
                historyIndex.value = historySize - 1;
                once = false;
            }
            kernelCopyImage.Execute(new CLCalc.Program.MemoryObject[] { depthFrame.GetMemoryObject(), textureFrame.GetMemoryObject(), uvImage, depthImage }, new int[] { depthFrame.Width, depthFrame.Height });
        }

        public void Initialize(BaseCameraApplication capture)
        {
            DepthCameraFrame frame = capture.GetDevices()[0].GetDepthImage();
            try
            {
                StreamReader reader = new StreamReader(new MemoryStream(Perceptual.Foundation.Properties.Resources.AdaptiveTemporalFilter));
                string text = reader.ReadToEnd();
                CLCalc.Program.Compile(capture.GetPrimaryDevice().GetPreprocessCode() + "\n#define HISTORY_SIZE " + historySize + "\n" + text);
                reader.Close();

            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                System.Console.WriteLine("Could not find MedianTemporalFilter.cl");
                Environment.Exit(1);
            }
            historyIndex = new CLCalc.Program.Value<int>(historySize - 1);
            updateBuffer = new CLCalc.Program.Kernel("UpdateFilter");
            copyToTemporalBuffer = new CLCalc.Program.Kernel("CopyToTemporalBuffer");
            depthBuffer = CLCalc.Program.Variable.Create(new ComputeBuffer<float>(CLCalc.Program.Context, ComputeMemoryFlags.ReadWrite, 4 * frame.Width * frame.Height));
            depthTemporalBuffer = CLCalc.Program.Variable.Create(new ComputeBuffer<float>(CLCalc.Program.Context, ComputeMemoryFlags.ReadWrite, historySize * 4 * frame.Width * frame.Height));

            depthCopyBuffer = CLCalc.Program.Variable.Create(new ComputeBuffer<float>(CLCalc.Program.Context, ComputeMemoryFlags.ReadWrite, 4 * frame.Width * frame.Height));
            depthImage = new CLCalc.Program.Image2D(depthCopy = new float[frame.Height * frame.Width * 4], frame.Width, frame.Height);
            uvImage = new CLCalc.Program.Image2D(new float[frame.Height * frame.Width * 4], frame.Width, frame.Height);

            kernelErodeFilter = new CLCalc.Program.Kernel("ErodeFilter");
            kernelDilateFilter = new CLCalc.Program.Kernel("DilateFilter");
            kernelCopyImage = new CLCalc.Program.Kernel("CopyImage");

            kernelCopyBuffer = new CLCalc.Program.Kernel("CopyDepth");
            kernelMedianFilter1 = new CLCalc.Program.Kernel("SmallFilter");
            kernelMedianFilter2 = new CLCalc.Program.Kernel("LargeFilter");
        }

    }
}
