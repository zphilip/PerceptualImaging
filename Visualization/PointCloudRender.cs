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
using Cloo;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTKWrapper.CLGLInterop;
using OpenTKWrapper;
using Perceptual.Foundation;
namespace Perceptual.Visualization
{
    public class PointCloudRender : PercRender
    {
        protected float[] vertexData;
        protected float[] PositionData, ColorData;
        protected int[] bufs;
        public bool Visisble = true;
        protected CLCalc.Program.Variable positions = null;
        protected CLCalc.Program.Variable colors = null;
        protected CLCalc.Program.Kernel kernelCopyBmp;
        public void Draw(GLAdvancedRender glw)
        {

            if (Visisble)
            {
                GL.Disable(EnableCap.Lighting);
                GL.BindBuffer(BufferTarget.ArrayBuffer, bufs[0]);
                GL.ColorPointer(4, ColorPointerType.Float, 0, 0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, bufs[1]);
                GL.VertexPointer(4, VertexPointerType.Float, 0, 0);


                GL.EnableClientState(ArrayCap.VertexArray);
                GL.EnableClientState(ArrayCap.ColorArray);

                GL.DrawArrays(BeginMode.Points, 0, PositionData.Length / 4);

                GL.DisableClientState(ArrayCap.VertexArray);
                GL.DisableClientState(ArrayCap.ColorArray);
                GL.Enable(EnableCap.Lighting);

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            }

        }
        public void Process(BaseCameraApplication capture)
        {
            if (Visisble)
            {
                DepthCameraFrame depthFrame = capture.GetPrimaryDevice().GetDepthImage();
                ColorCameraFrame colorFrame = capture.GetPrimaryDevice().GetColorImage();
                TextureMapFrame textureFrame = capture.GetPrimaryDevice().GetTextureImage();
                if (depthFrame != null && colorFrame != null)
                {

                    CLCalc.Program.MemoryObject[] args = new CLCalc.Program.MemoryObject[] { depthFrame.GetMemoryObject(), textureFrame.GetMemoryObject(), colorFrame.GetMemoryObject(), positions, colors };
                    CLGLInteropFunctions.AcquireGLElements(args);
                    kernelCopyBmp.Execute(args, new int[] { depthFrame.Width, depthFrame.Height });
                    CLGLInteropFunctions.ReleaseGLElements(args);
                }
            }

        }
        public void Initialize(BaseCameraApplication capture, GLAdvancedRender glw)
        {
            try
            {
                CLCalc.Program.Compile(capture.GetPrimaryDevice().GetPreprocessCode() + src);

            }
            catch (BuildProgramFailureComputeException ex)
            {
                System.Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }
            DepthCameraFrame frame = capture.GetDevices()[0].GetDepthImage();
            kernelCopyBmp = new CLCalc.Program.Kernel("CopyImageToPointCloud");
            int size = frame.Width * frame.Height;
            bufs = new int[4];

            ColorData = new float[4 * size];
            PositionData = new float[4 * size];

            GL.GenBuffers(2, bufs);

            GL.BindBuffer(BufferTarget.ArrayBuffer, bufs[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(ColorData.Length * sizeof(float)), ColorData, BufferUsageHint.StreamDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, bufs[1]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(PositionData.Length * sizeof(float)), PositionData, BufferUsageHint.StreamDraw);//Notice STREAM DRAW
            GL.Enable(EnableCap.PointSmooth);
            GL.PointSize(4.0f);
            positions = new CLCalc.Program.Variable(bufs[1], typeof(float));
            colors = new CLCalc.Program.Variable(bufs[0], typeof(float));
        }
        #region OpenCL source
        string src = @"
    kernel void CopyImageToPointCloud(global float4* depthData,global float2* uvData, __read_only image2d_t originalImage, global float4* vertexData, global float4* colorData)
    {
       const sampler_t smp = CLK_NORMALIZED_COORDS_TRUE | //Natural coordinates
             CLK_ADDRESS_CLAMP | //Clamp to zeros
             CLK_FILTER_LINEAR; //Don't interpolate
       int x = get_global_id(0);
       int y = get_global_id(1);
   
       int2 coords = (int2)(x,y);
   
       int index=x+y*get_global_size(0);
       float4 value=depthData[index];
       float2 uv=uvData[index];
       int4 rgba=read_imagei(originalImage,smp,uv);
       float4 color=(uv.x<0||uv.x>1.0f||uv.y<0||uv.y>1.0f)?(float4)(0,0,0,0):(float4)(rgba.x/255.0f,rgba.y/255.0f,rgba.z/255.0f,clamp(value.w/100.0f,0.0f,1.0f));
        value.w=(value.w<100||value.z<200||value.z>2000)?0.0f:1.0f;
        value.x=value.x;    
        value.z=value.z;
       vertexData[index]=value;
       colorData[index]=color;
    }
";
        #endregion

    }
}
