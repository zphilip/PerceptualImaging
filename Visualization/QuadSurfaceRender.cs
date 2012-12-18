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
using OpenTK.Graphics.OpenGL;
using Cloo;
using OpenTKWrapper.CLGLInterop;
using OpenTKWrapper;
namespace Perceptual.Visualization
{
    public class QuadSurfaceRender : PercRender
    {
        public bool ShowColor = true, Visible = true, WireFrame = false;

        protected int[] QuadMeshBufs = new int[3];
        protected float[] ColorData, PositionData, NormalData;

        protected CLCalc.Program.Variable positionBuffer, colorBuffer, normalBuffer;
        protected CLCalc.Program.Kernel kernelCopyImage;

        public void Initialize(BaseCameraApplication app, OpenTKWrapper.CLGLInterop.GLAdvancedRender glw)
        {
            try
            {
                CLCalc.Program.Compile(app.GetPrimaryDevice().GetPreprocessCode() + src);
            }
            catch (BuildProgramFailureComputeException ex)
            {
                System.Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }
            kernelCopyImage = new CLCalc.Program.Kernel("CopyImageToMesh");
            BoundingBox bbox = app.GetPrimaryDevice().GetBoundingBox();
            int w = app.GetPrimaryDevice().GetDepthImage().Width;
            int h = app.GetPrimaryDevice().GetDepthImage().Height;
            int size = w * h;
            ColorData = new float[16 * size];
            PositionData = new float[16 * size];
            NormalData = new float[12 * size];
            for (int i = 0; i < size; i++)
            {
                PositionData[4 * i] = (i / w) - w / 2;
                PositionData[4 * i + 2] = i % w - h / 2;
                PositionData[4 * i + 1] = i % 7;
                PositionData[4 * i + 3] = 1.0f;

            }

            GL.GenBuffers(3, QuadMeshBufs);

            GL.BindBuffer(BufferTarget.ArrayBuffer, QuadMeshBufs[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(ColorData.Length * sizeof(float)), ColorData, BufferUsageHint.StreamDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, QuadMeshBufs[1]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(PositionData.Length * sizeof(float)), PositionData, BufferUsageHint.StreamDraw);//Notice STREAM DRAW

            GL.BindBuffer(BufferTarget.ArrayBuffer, QuadMeshBufs[2]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(NormalData.Length * sizeof(float)), NormalData, BufferUsageHint.StreamDraw);//Notice STREAM DRAW

            colorBuffer = new CLCalc.Program.Variable(QuadMeshBufs[0], typeof(float));
            positionBuffer = new CLCalc.Program.Variable(QuadMeshBufs[1], typeof(float));
            normalBuffer = new CLCalc.Program.Variable(QuadMeshBufs[2], typeof(float));

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.Enable(EnableCap.Blend);
        }
        public void SetShowColor(bool show)
        {
            this.ShowColor = show;
        }
        public void SetShowSurface(bool show)
        {
            this.Visible = show;
        }
        public void Draw(OpenTKWrapper.CLGLInterop.GLAdvancedRender glw)
        {

            GL.Enable(EnableCap.Lighting);
            if (Visible)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                GL.EnableClientState(ArrayCap.VertexArray);
                if (ShowColor)
                {

                    GL.Disable(EnableCap.Lighting);
                    GL.EnableClientState(ArrayCap.ColorArray);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, QuadMeshBufs[0]);
                    GL.ColorPointer(4, ColorPointerType.Float, 0, 0);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, QuadMeshBufs[1]);
                    GL.VertexPointer(4, VertexPointerType.Float, 0, 0);
                    GL.DrawArrays(BeginMode.Quads, 0, PositionData.Length / 4);
                    GL.DisableClientState(ArrayCap.ColorArray);
                }
                else
                {

                    GL.Enable(EnableCap.Lighting);
                    GL.EnableClientState(ArrayCap.NormalArray);
                    GL.Color3(0.8f, 0.4f, 0.2f);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, QuadMeshBufs[2]);
                    GL.NormalPointer(NormalPointerType.Float, 0, 0);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, QuadMeshBufs[1]);
                    GL.VertexPointer(4, VertexPointerType.Float, 0, 0);
                    GL.DrawArrays(BeginMode.Quads, 0, PositionData.Length / 4);
                    GL.Color3(1.0f, 1.0f, 1.0f);

                    GL.DisableClientState(ArrayCap.NormalArray);
                }
                GL.DisableClientState(ArrayCap.VertexArray);
            }
            if (WireFrame)
            {
                GL.LineWidth(2.0f);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                GL.EnableClientState(ArrayCap.VertexArray);

                GL.EnableClientState(ArrayCap.NormalArray);
                if (ShowColor)
                {
                    GL.Color3(0.1f, 0.1f, 0.2f);
                }
                else
                {
                    GL.Color3(0.4f, 0.2f, 0.1f);
                }
                GL.BindBuffer(BufferTarget.ArrayBuffer, QuadMeshBufs[2]);
                GL.NormalPointer(NormalPointerType.Float, 0, 0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, QuadMeshBufs[1]);
                GL.VertexPointer(4, VertexPointerType.Float, 0, 0);
                GL.DrawArrays(BeginMode.Quads, 0, PositionData.Length / 4);
                GL.Color3(1.0f, 1.0f, 1.0f);

                GL.DisableClientState(ArrayCap.NormalArray);
            }
            GL.DisableClientState(ArrayCap.VertexArray);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        }

        public void Process(BaseCameraApplication app)
        {
            if (Visible || WireFrame)
            {
                DepthCameraFrame depthFrame = app.GetPrimaryDevice().GetDepthImage();
                ColorCameraFrame colorFrame = app.GetPrimaryDevice().GetColorImage();
                TextureMapFrame textureFrame = app.GetPrimaryDevice().GetTextureImage();
                CameraDataFilter filter = (CameraDataFilter)app.GetImageFilter();
                CLGLInteropFunctions.AcquireGLElements(new CLCalc.Program.MemoryObject[] { positionBuffer, colorBuffer, normalBuffer });
                CLCalc.Program.MemoryObject[] args = new CLCalc.Program.MemoryObject[] { 
                 app.GetPrimaryDevice().GetBoundingBox(),filter.GetDepthImage(),filter.GetTextureImage(),colorFrame.GetMemoryObject(),positionBuffer,colorBuffer,normalBuffer};
                kernelCopyImage.Execute(args, new int[] { depthFrame.Width, depthFrame.Height });
                CLGLInteropFunctions.ReleaseGLElements(new CLCalc.Program.MemoryObject[] { positionBuffer, colorBuffer, normalBuffer });
            }
        }
        #region OpenCL source
        string src = @"
        #define MAX_DIFF 25.0f
        typedef struct {
	        float4 minPoint;
	        float4 maxPoint;
	        float4 center;
        } BoundingBox;
        inline int getIndex(int i,int j){
            return clamp(i,0,WIDTH-1)+clamp(j,0,HEIGHT-1)*WIDTH;
        }
        inline bool InsideBoundingBox(float4 pt,BoundingBox bbox){
            return (
                pt.x>=bbox.minPoint.x&&
                pt.y>=bbox.minPoint.y&&
                pt.z>=bbox.minPoint.z&&
                pt.x<=bbox.maxPoint.x&&
                pt.y<=bbox.maxPoint.y&&
                pt.z<=bbox.maxPoint.z);
        }
        inline float4 computeColor(float4 pt,read_only image2d_t uvData,read_only image2d_t originalImage,BoundingBox bbox){
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
           if(InsideBoundingBox(pt,bbox)){
                return color;
           } else {
                float lum=(0.299f*color.x+0.587f*color.y+0.114f*color.z);
                return (float4)(lum,lum,lum,1.0f);
           }
        }
        kernel void CopyImageToMesh(
            BoundingBox bbox,
            read_only image2d_t depthData,
            read_only image2d_t uvData,
            read_only image2d_t originalImage,
            global float4* vertexBuffer, 
            global float4* colorData,
            global float* normalBuffer){

            const sampler_t smpNearest = CLK_NORMALIZED_COORDS_FALSE | CLK_ADDRESS_CLAMP | CLK_FILTER_NEAREST;
            int i = get_global_id(0);
            int j = get_global_id(1);
            int2 coords = (int2)(i,j); 
            int index=i+j*WIDTH;
	        float minD=1E10f;
	        float maxD=-1E10f;
            float minIR=1E10f;
            float4 pt,color;
            normalBuffer+=12*index;
            vertexBuffer+=4*index;
            colorData+=4*index;

	        pt=read_imagef(depthData,smpNearest,(int2)(i,j));
            color=computeColor(pt,uvData,originalImage,bbox);      
            minD=min(minD,pt.z);
            minIR=min(minIR,pt.w);
	        maxD=max(maxD,pt.z);
            vertexBuffer[0]=pt;
            colorData[0]=color; 

	        pt=read_imagef(depthData,smpNearest,(int2)(i+1,j));
            color=computeColor(pt,uvData,originalImage,bbox);
	        minD=min(minD,pt.z);
            minIR=min(minIR,pt.w);
	        maxD=max(maxD,pt.z);
	        vertexBuffer[1]=pt;
	        colorData[1]=color;

	        pt=read_imagef(depthData,smpNearest,(int2)(i+1,j+1));
            color=computeColor(pt,uvData,originalImage,bbox);
	        minD=min(minD,pt.z);
            minIR=min(minIR,pt.w);
	        maxD=max(maxD,pt.z);
	        vertexBuffer[2]=pt;
            colorData[2]=color;

	        pt=read_imagef(depthData,smpNearest,(int2)(i,j+1));
            color=computeColor(pt,uvData,originalImage,bbox);
	        minD=min(minD,pt.z);
            minIR=min(minIR,pt.w);
	        maxD=max(maxD,pt.z);
	        vertexBuffer[3]=pt;
	        colorData[3]=color;

	        if(maxD-minD>MAX_DIFF||minIR<MIN_IR||minD<MIN_DEPTH||maxD>MAX_DEPTH){
                //Hide QUAD!

		        vertexBuffer[0]=(float4)(0,0,0,0);
		        vertexBuffer[1]=(float4)(0,0,0,0);
		        vertexBuffer[2]=(float4)(0,0,0,0);
		        vertexBuffer[3]=(float4)(0,0,0,0);
	        } else {
                //SHOW QUAD!
            	vertexBuffer[0].w=1;
		        vertexBuffer[1].w=1;
		        vertexBuffer[2].w=1;
		        vertexBuffer[3].w=1;
                //COMPUTE NORMALS
                float4 norm=normalize(cross(vertexBuffer[1]-vertexBuffer[0],vertexBuffer[2]-vertexBuffer[0]));
                normalBuffer[0]=norm.x;normalBuffer[1]=norm.y;normalBuffer[2]=norm.z;            
                normalBuffer+=3;
                normalBuffer[0]=norm.x;normalBuffer[1]=norm.y;normalBuffer[2]=norm.z;            
                normalBuffer+=3;
                normalBuffer[0]=norm.x;normalBuffer[1]=norm.y;normalBuffer[2]=norm.z;            
                normalBuffer+=3;
                normalBuffer[0]=norm.x;normalBuffer[1]=norm.y;normalBuffer[2]=norm.z;    
            }
        }
";
        #endregion
    }
}
