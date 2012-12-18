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

namespace Perceptual.Foundation
{
    public abstract class CameraDevice
    {
        public abstract bool Initialize();
        protected DepthCameraFrame depthFrame;
        protected ColorCameraFrame colorFrame;
        protected TextureMapFrame textureFrame;
        protected BoundingBox bbox = new BoundingBox(new float4(-300, -250, 250, 1), new float4(300, 250, 650, 1));
        protected OpenTK.Matrix4 GroundPlane = OpenTK.Matrix4.Identity;
        protected float focalX = 1.0f, focalY = 1.0f;
        protected float centerX = 0, centerY = 0;
        protected float minDepth = 200.0f;
        protected float maxDepth = 3000.0f;
        protected float minIR = 100.0f;
        public float GetMinIR()
        {
            return minIR;
        }
        public float GetCameraCenterX()
        {
            return centerX;
        }
        public float GetCameraCenterY()
        {
            return centerY;
        }
        public float GetMinDepth()
        {
            return minDepth;
        }
        public float GetMaxDepth()
        {
            return maxDepth;
        }
        public string GetPreprocessCode()
        {
            string code =
             "\n#define WIDTH " + depthFrame.Width +
             "\n#define HEIGHT " + depthFrame.Height +
             "\n#define CENTER_X " + centerX.ToString(".0000") + "f" +
             "\n#define CENTER_Y " + centerY.ToString(".0000") + "f" +
             "\n#define COLOR_WIDTH " + colorFrame.Width +
             "\n#define COLOR_HEIGHT " + colorFrame.Height +
             "\n#define MIN_IR " + minIR +
             "\n#define MIN_DEPTH " + minDepth.ToString(".0000") + "f" +
             "\n#define MAX_DEPTH " + maxDepth.ToString(".0000") + "f" +
             "\n#define FOCAL_X " + focalX.ToString(".0000") + "f" +
             "\n#define FOCAL_Y " + focalY.ToString(".0000") + "f\n";
            return code;
        }
        public Matrix4f GetGroundPlane()
        {
            return GroundPlane;
        }
        public void SetGroundPlane(OpenTK.Matrix4 M)
        {
            this.GroundPlane = M;
        }

        public float GetFocalLengthX()
        {
            return focalX;
        }
        public float GetFocalLengthY()
        {
            return focalY;
        }
        public BoundingBox GetBoundingBox()
        {
            return bbox;
        }
        public void SetBoundingBox(BoundingBox bbox)
        {
            this.bbox = bbox;
        }
        public CameraDevice(string outputDirectory)
        {
            this.outputDirectory = outputDirectory;
        }
        public DepthCameraFrame GetDepthImage()
        {
            return depthFrame;
        }
        public ColorCameraFrame GetColorImage()
        {
            return colorFrame;
        }
        public TextureMapFrame GetTextureImage()
        {
            return textureFrame;
        }
        public abstract unsafe void GetNextFrame();

        protected string outputDirectory = @"C:\";
        protected int id = 1;
        public void SetOutputDirectory(string outputDirectory)
        {
            this.outputDirectory = outputDirectory;
        }
        public string GetOutputDirectory()
        {
            return outputDirectory;
        }
        public void SetFrameId(int id)
        {
            this.id = id;
        }
        public virtual void RecordCurrentFrame()
        {
            WriteToDirectory(outputDirectory, id);
            id++;
        }

        public abstract void Dispose();
        public void WriteToDirectory(string dir, int id)
        {

            if (GetDepthImage() != null) GetDepthImage().WriteToFile(dir + "\\img" + id.ToString("00000") + ".depth");
            if (GetColorImage() != null) GetColorImage().WriteToFile(dir + "\\img" + id.ToString("00000") + ".color");
            if (GetTextureImage() != null) GetTextureImage().WriteToFile(dir + "\\img" + id.ToString("00000") + ".texture");
        }



    }
}
