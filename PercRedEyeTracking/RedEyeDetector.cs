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
using OpenCvSharp;
using System.Diagnostics;
using Cloo;
using OpenTK;
using OpenTK.Graphics;
using OpenCvSharp.Blob;
using System.IO;
using System.Runtime.InteropServices;
using System.Globalization;
using Perceptual.Foundation;
using OpenTKWrapper;
namespace Perceptual.RedEye
{
    public struct FaceLandmarks
    {

        public int4 boundingBox2D;
        public float4 rightEye;
        public float4 leftEye;
        public float4 noseBridge;
        public float4 noseTip;
        public FaceLandmarks(int4 boundingBox)
        {
            this.boundingBox2D = boundingBox;
            rightEye = new float4(0, 0, 0, -1);
            leftEye = new float4(0, 0, 0, -1);
            noseBridge = new float4(0, 0, 0, -1);
            noseTip = new float4(0, 0, 0, -1);
        }
    }
    public class RedEyeDetector : CameraDataProcessor
    {
        public RedEyeDetector()
        {
        }
        protected AdaptiveTemporalFilter filter;
        protected int width, height;
        protected IplImage gray;
        protected IplImage erode;
        protected IplImage mask;
        protected IplImage tmp;
        protected IplImage dilate;
        protected bool foundFace = false;
        protected int2 rightEye2D, leftEye2D;
        protected int4 boundingBox2D;
        protected CLCalc.Program.Kernel kernelCopyIRImage, kernelFindFaceLandmarks;
        protected byte[] ir;
        protected CvBlobs blobs = new CvBlobs();
        protected CvMemStorage storage = new CvMemStorage();
        protected CvHaarClassifierCascade faceCascade = CvHaarClassifierCascade.FromFile("haarcascade_frontalface_alt.xml");
        protected IplImage imgLabel;
        protected float[] depthImage;
        CLCalc.Program.Variable irImageBuffer;
        protected CLCalc.Program.Variable faceDetectionBuffer;
        protected FaceLandmarks faceLandmarks;
        public FaceLandmarks GetFaceLandmarks()
        {
            return faceLandmarks;
        }
        public void Initialize(BaseCameraApplication capture)
        {
            DepthCameraFrame depthImage = capture.GetPrimaryDevice().GetDepthImage();
            this.width = depthImage.Width;
            this.height = depthImage.Height;
            this.filter = ((AdaptiveTemporalFilter)capture.GetImageFilter());
            CvSize sz = new CvSize(depthImage.Width, depthImage.Height);
            gray = new IplImage(sz, BitDepth.U8, 1);
            erode = new IplImage(sz, BitDepth.U8, 1);
            dilate = new IplImage(sz, BitDepth.U8, 1);
            tmp = new IplImage(sz, BitDepth.U8, 1);
            mask = new IplImage(sz, BitDepth.U8, 1);
            imgLabel = new IplImage(sz, BitDepth.F32, 1);
            faceDetectionBuffer = CLCalc.Program.Variable.Create(new ComputeBuffer<FaceLandmarks>(CLCalc.Program.Context, ComputeMemoryFlags.ReadWrite, 1));
            try
            {
                CLCalc.Program.Compile(capture.GetPrimaryDevice().GetPreprocessCode() + src);

            }
            catch (BuildProgramFailureComputeException ex)
            {
                System.Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }
            irImageBuffer = CLCalc.Program.Variable.Create(new ComputeBuffer<byte>(CLCalc.Program.Context, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, ir = new byte[width * height]));
            kernelCopyIRImage = new CLCalc.Program.Kernel("CopyIRImage");
            kernelFindFaceLandmarks = new CLCalc.Program.Kernel("FindFaceLandmarks");
        }
        #region OpenCL source
        string src = @"

    typedef struct 
    {
        int4 boundingBox;
        float4 rightEye;
        float4 leftEye;
        float4 bridge;
        float4 nose;
    } FaceDetection;
kernel void CopyIRImage(global float4* depthData,global uchar* irImage)
{
    int gid=get_global_id(0);
	float ir=depthData[gid].w;
	irImage[gid]=clamp((int)(255.0f*(ir-100)/1000.0f),0,255);

}
kernel void FindFaceLandmarks(int2 rightEye,int2 leftEye,int4 boundingBox,global FaceDetection* face,read_only image2d_t depthImage)
{

	const sampler_t smp = CLK_NORMALIZED_COORDS_FALSE | CLK_ADDRESS_CLAMP_TO_EDGE | CLK_FILTER_LINEAR;

    float4 rightEye4=face->rightEye=read_imagef(depthImage,smp,rightEye);
    float4 leftEye4=face->leftEye=read_imagef(depthImage,smp,leftEye);
    face->boundingBox=boundingBox;
    const float delta=0.25f;

    float4 center=0.5f*(face->rightEye+face->leftEye);
    float4 bridge=center;
   
    float2 right=(float2)(rightEye.x,rightEye.y);
    float2 left=(float2)(leftEye.x,leftEye.y);

    float d=distance(right,left);
    rightEye4.w=0;
    leftEye4.w=0;

    float4 vec=normalize(leftEye4-rightEye4);
    int N=ceil(d/delta);
    float maxDistance=0.0f;
    float4 offset;
    float l;
    float2 mid;
    for(int n=0;n<N;n++){
        float2 pt=mix(right,left,n/(float)(N-1));
        float4 depth=read_imagef(depthImage,smp,pt);  
        float4 offset=depth-rightEye4;
        offset.w=0;
        l=dot(vec,offset);
        float z=dot(offset,offset)-l*l;
        if(z>maxDistance&&depth.w>100.0f){
            bridge=depth;
            mid=pt;
            maxDistance=z;
        }  
    }
    maxDistance=0;

    face->bridge=bridge;
    float2 end=mid+(float2)(-(left.y-right.y),(left.x-right.x));
    float4 nose=bridge;
    bridge.w=0.0f;
    center=dot(vec,(bridge-rightEye4))*vec+rightEye4;

    center.w=0.0f;
    vec=normalize(bridge-center);
    float2 vec2d=0.5f*(right-left);
    int M=ceil(d/delta);
    for(int n=0;n<N;n++){
        for(int m=-M/2;m<=M/2;m++){
            float2 pt=mix(mid,end,n/(float)(N-1))+((2*m/(float)M)*vec2d);
            float4 depth=read_imagef(depthImage,smp,pt);  
            float4 offset=depth-center;
            float z=dot(vec,offset);
            if(z>maxDistance&&depth.w>100.0f){
                nose=depth;
                maxDistance=z;
            }  
        }
    }
    face->nose=nose;
}
";
        #endregion

        public void Process(BaseCameraApplication capture)
        {
            const double ScaleFactor = 1.0850;
            DepthCameraFrame depthFrame = capture.GetPrimaryDevice().GetDepthImage();
            ColorCameraFrame rgbFrame = capture.GetPrimaryDevice().GetColorImage();
            TextureMapFrame uvFrame = capture.GetPrimaryDevice().GetTextureImage();
            kernelCopyIRImage.Execute(new CLCalc.Program.MemoryObject[] { depthFrame.GetMemoryObject(), irImageBuffer }, width * height);
            CLCalc.Program.CommQueues[CLCalc.Program.DefaultCQ].Read<byte>(((ComputeBuffer<byte>)irImageBuffer.VarPointer), true, 0, width * height, gray.ImageData, null);
            storage.Clear();
            //Use OpenCV for face tracking in IR image. SDK has its own face tracker, but it only operates in RGB. Either could be used for this example.
            CvSeq<CvAvgComp> faces = Cv.HaarDetectObjects(gray, faceCascade, storage, ScaleFactor, 2, 0, new CvSize(40, 40));
            if (faces.Total > 0)
            {
                CvRect face = faces[0].Value.Rect;
                Cv.SetImageROI(gray, face);
                Cv.SetImageROI(dilate, face);
                Cv.SetImageROI(erode, face);
                Cv.SetImageROI(tmp, face);
                //Filter the image to enhance contrast between eyes/face.
                Cv.Dilate(gray, tmp);
                Cv.Dilate(tmp, dilate);
                Cv.Threshold(gray, tmp, 0, 1, ThresholdType.Binary);
                Cv.Erode(gray, erode);
                Cv.Sub(gray, erode, gray);
                Cv.Mul(gray, tmp, gray);
                Cv.SetImageROI(mask, face);
                Cv.SetImageROI(imgLabel, face);
                //Threshold out peaks.
                Cv.Threshold(gray, mask, 128, 255, ThresholdType.Binary);
                blobs.Clear();
                uint result = blobs.Label(mask, imgLabel);
                double minDistLeft = 1E10;
                double minDistRight = 1E10;
                int xCenter = face.Width / 2;
                int yCenter = (int)((face.Height) * 0.35);
                CvPoint center = new CvPoint(xCenter, yCenter);
                CvPoint right = new CvPoint(-1, -1);
                CvPoint left = new CvPoint(-1, -1);

                //Assign blobs to eyes.
                foreach (KeyValuePair<uint, CvBlob> item in blobs)
                {
                    CvBlob b = item.Value;
                    double d = CvPoint.Distance(b.Centroid, center);
                    if (b.Centroid.X < center.X)
                    {
                        if (d < minDistLeft)
                        {
                            minDistLeft = d;
                            right = b.Centroid;
                        }
                    }
                    else
                    {
                        if (d < minDistRight)
                        {
                            minDistRight = d;
                            left = b.Centroid;
                        }
                    }
                }
                if (right.X >= 0 && left.X >= 0)
                {
                    rightEye2D = new int2(right.X + face.X, right.Y + face.Y);
                    leftEye2D = new int2(left.X + face.X, left.Y + face.Y);
                    boundingBox2D = new int4(face.X, face.Y, face.Width, face.Height);
                    //Find bridge and nose. This was done in opencl to leverage read_imagef.
                    kernelFindFaceLandmarks.Execute(new CLCalc.Program.MemoryObject[] { rightEye2D, leftEye2D, boundingBox2D, faceDetectionBuffer, filter.GetDepthImage() }, 1);
                    ReadFaceLandmarksFromBuffer();
                    foundFace = true;
                }
                else
                {
                    foundFace = false;
                }
                Cv.ResetImageROI(gray);
                Cv.ResetImageROI(erode);
                Cv.ResetImageROI(dilate);
                Cv.ResetImageROI(tmp);
            }
            else
            {
                foundFace = false;
                WriteFaceLandmarksToBuffer();
            }

        }
        protected void ReadFaceLandmarksFromBuffer()
        {
            unsafe
            {
                fixed (void* ponteiro = &faceLandmarks
                    )
                {
                    CLCalc.Program.CommQueues[CLCalc.Program.DefaultCQ].Read<FaceLandmarks>((ComputeBuffer<FaceLandmarks>)(faceDetectionBuffer.VarPointer), true, 0, 1, (IntPtr)ponteiro, null);
                }
            }
        }
        public int2 GetLeftEye2D()
        {
            return leftEye2D;
        }
        public int2 GetRightEye2D()
        {
            return rightEye2D;
        }
        public int2 GetMinPoint2D()
        {
            return new int2((int)boundingBox2D.x, (int)boundingBox2D.y);
        }
        public int2 GetMaxPoint2D()
        {
            return new int2((int)(boundingBox2D.x + boundingBox2D.z), (int)(boundingBox2D.y + boundingBox2D.w));
        }
        public int2 GetFaceDimensions()
        {
            return new int2((int)(boundingBox2D.z), (int)(boundingBox2D.w));
        }
        protected void WriteFaceLandmarksToBuffer()
        {
            unsafe
            {
                fixed (void* ponteiro = &faceLandmarks
                    )
                {
                    CLCalc.Program.CommQueues[CLCalc.Program.DefaultCQ].Write<FaceLandmarks>((ComputeBuffer<FaceLandmarks>)(faceDetectionBuffer.VarPointer), true, 0, 1, (IntPtr)ponteiro, null);
                }
            }
        }
        public bool FoundFace()
        {
            return foundFace;
        }
    }
}
