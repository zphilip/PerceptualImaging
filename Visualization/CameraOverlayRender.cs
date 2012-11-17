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
using OpenTK;
using Cloo;
using OpenTKWrapper;
using OpenTKWrapper.CLGLInterop;
namespace Perceptual.Visualization
{
    public class CameraOverlayRender : ImageButtonRender
    {
        protected ImageButton depthButton;
        protected ImageButton colorButton;

        protected CLCalc.Program.Image2D rgbCopy;
        protected CLCalc.Program.Image2D irCopy;
        protected int irTextureId;
        protected int rgbTextureId;
        protected CLCalc.Program.Kernel kernelCopyImage;
        public override void Initialize(Perceptual.Foundation.BaseCameraApplication app, OpenTKWrapper.CLGLInterop.GLAdvancedRender glw)
        {
            base.Initialize(app, glw);
            int w = app.GetPrimaryDevice().GetDepthImage().Width;
            int h = app.GetPrimaryDevice().GetDepthImage().Height;
            irTextureId = Utilities.CreateTexture<float>(out irCopy, w, h);
            rgbTextureId = Utilities.CreateTexture<float>(out rgbCopy, w, h);

            depthButton = new ImageButton("IR Image", null, false, glw.GLCtrl.Width - irCopy.Width - 10, glw.GLCtrl.Height - irCopy.Height - 20, irCopy.Width, irCopy.Height);
            depthButton.SetDynamicImage(irCopy, irTextureId);

       
            colorButton = new ImageButton("Color Image", null, false, glw.GLCtrl.Width - rgbCopy.Width - 10, -30, rgbCopy.Width, rgbCopy.Height);
            colorButton.SetDynamicImage(rgbCopy, rgbTextureId);

            try
            {
                CLCalc.Program.Compile(app.GetPrimaryDevice().GetPreprocessCode() + src);
            }
            catch (BuildProgramFailureComputeException ex)
            {
                System.Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }
            kernelCopyImage = new CLCalc.Program.Kernel("CopyImage");

            Buttons.Add(depthButton);

            Buttons.Add(colorButton);
        }

        public override void Process(Perceptual.Foundation.BaseCameraApplication app)
        {
            if (Visible)
            {
                DepthCameraFrame depthFrame = app.GetPrimaryDevice().GetDepthImage();
                ColorCameraFrame colorFrame = app.GetPrimaryDevice().GetColorImage();
                TextureMapFrame textureFrame = app.GetPrimaryDevice().GetTextureImage();
                AdaptiveTemporalFilter filter = (AdaptiveTemporalFilter)app.GetImageFilter();
                CLGLInteropFunctions.AcquireGLElements(new CLCalc.Program.MemoryObject[] { irCopy, rgbCopy });
                CLCalc.Program.MemoryObject[] args = new CLCalc.Program.MemoryObject[] { 
                filter.GetDepthImage(), filter.GetTextureImage(), colorFrame.GetMemoryObject(),filter.GetMotionBuffer(), irCopy, rgbCopy };
                kernelCopyImage.Execute(args, new int[] { depthFrame.Width, depthFrame.Height });
                CLGLInteropFunctions.ReleaseGLElements(new CLCalc.Program.MemoryObject[] { irCopy, rgbCopy });
            }
        }
        #region OpenCL source
        string src = @"
        #define MIN_IR 100.0f
        inline int getIndex(int i,int j){
            return clamp(i,0,WIDTH-1)+clamp(j,0,HEIGHT-1)*WIDTH;
        }
        inline float4 computeColor(float4 pt,read_only image2d_t uvData,read_only image2d_t originalImage){
           float2 p;
           p.x=FOCAL_X*pt.x/pt.z+WIDTH/2;
           p.y=HEIGHT/2-FOCAL_Y*pt.y/pt.z;
	       float4 v=pt;
           v.w=1.0f; 
           const sampler_t smpLinear = CLK_NORMALIZED_COORDS_FALSE | CLK_ADDRESS_CLAMP | CLK_FILTER_LINEAR;
           const sampler_t normalizedSmp = CLK_NORMALIZED_COORDS_TRUE | CLK_ADDRESS_CLAMP | CLK_FILTER_LINEAR;
           float4 uv=read_imagef(uvData,smpLinear,p);
           int4 rgba=read_imagei(originalImage,normalizedSmp,(float2)(uv.x,uv.y));
           float4 color=(uv.x<0||uv.x>1.0f||uv.y<0||uv.y>1.0f||pt.w<100.0f)?(float4)(0.0f,0.0f,0.0f,1.0f):(float4)(rgba.x/255.0f,rgba.y/255.0f,rgba.z/255.0f,1.0f);
           return color;
        }


        kernel void CopyImage(
            read_only image2d_t depthData,
            read_only image2d_t uvData,
            read_only image2d_t originalImage,
            global float4* motion,
            write_only image2d_t irCopy,
            write_only image2d_t colorCopy){
            const sampler_t smpNearest = CLK_NORMALIZED_COORDS_FALSE | CLK_ADDRESS_CLAMP | CLK_FILTER_NEAREST;
            int i = get_global_id(0);
            int j = get_global_id(1);
            int2 coords = (int2)(i,j); 
            int index=i+j*WIDTH;
            float m=motion[index].w;
            float4 pt,color;
	        pt=read_imagef(depthData,smpNearest,coords);
            if(pt.w>MIN_IR){
            color=computeColor(pt,uvData,originalImage);      
            write_imagef(colorCopy,coords,color);
            float lum=clamp(sqrt(pt.w-MIN_IR)/50.0f,0.0f,1.0f);            
            color=(float4)(lum,lum,lum,1.0f);
            color=mix(color,(float4)(0.5f*lum,1.0f*lum,0.5f*lum,1.0f),m);
            write_imagef(irCopy,coords,color);
            } else {
                write_imagef(colorCopy,coords,(float4)(0,0,0,0));
                write_imagef(irCopy,coords,(float4)(0,0,0,0));
            }
        }
";
        #endregion
    }
}
