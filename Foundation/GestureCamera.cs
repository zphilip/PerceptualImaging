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
using OpenTKWrapper;
namespace PercFoundation
{
    public class GestureCamera : CameraDevice
    {
        private PXCMSession session;
        private UtilMCapture capture;
        private pxcmStatus sts;
        private PXCMImage[] images = new PXCMImage[2];
        private PXCMScheduler.SyncPoint[] sps = new PXCMScheduler.SyncPoint[2];
        private PXCMGesture gesture;

        static pxcmStatus OnGesure(ref PXCMGesture.Gesture data, Boolean active)
        {
            Console.WriteLine("[gesture] label={0}, active={1}, timeStamp={2}", data.label, active, data.timeStamp);
            return pxcmStatus.PXCM_STATUS_NO_ERROR;
        }

        static PXCMGesture.Gesture.OnGesture OnGesture { get; set; }

        public GestureCamera(string output)
            : base(output)
        {
            this.focalX = 224.502f;
            this.focalY = 230.494f;
            this.bbox = new BoundingBox(new float4(-300, -250, 250, 1), new float4(300, 250, 650, 1));
            this.GroundPlane = OpenTK.Matrix4.Identity;
            this.minDepth = 200.0f;
            this.maxDepth = 3000.0f;
        }
        public override bool Initialize()
        {
            sts = PXCMSession.CreateInstance(out session);


            if (sts < pxcmStatus.PXCM_STATUS_NO_ERROR)
            {
                Console.WriteLine("Failed to create the SDK session");
                return false;
            }
            PXCMSession.ImplDesc desc;
            session.QueryImpl(0, out desc);
            capture = new UtilMCapture(session);
            PXCMCapture.VideoStream.DataDesc dataDesc = new PXCMCapture.VideoStream.DataDesc();

            PXCMBase tmp1;
            sts = session.CreateImpl(PXCMFaceAnalysis.CUID, out tmp1);


            PXCMCapture.VideoStream.DataDesc.StreamDesc stream;


            stream = new PXCMCapture.VideoStream.DataDesc.StreamDesc();
            stream.format = PXCMImage.ColorFormat.COLOR_FORMAT_NV12;
            dataDesc.streams[0] = stream;

            stream = new PXCMCapture.VideoStream.DataDesc.StreamDesc();
            stream.format = PXCMImage.ColorFormat.COLOR_FORMAT_VERTICES;
            stream.sizeMin.width = 320;
            stream.sizeMin.height = 240;
            stream.sizeMax.width = 320;
            stream.sizeMax.height = 240;
            dataDesc.streams[1] = stream;

            sts = capture.LocateStreams(ref dataDesc);
            if (sts < pxcmStatus.PXCM_STATUS_NO_ERROR)
            {
                Console.WriteLine("Failed to locate a capture module");
                capture.Dispose();
                session.Dispose();
                return false;
            }

            return true;
        }
        protected uint startIndex = 0;

        PXCMFaceAnalysis.Detection.Data face_data;
        PXCMFaceAnalysis.Landmark.PoseData pdata;
        PXCMFaceAnalysis.Landmark.LandmarkData data;
        public override unsafe void GetNextFrame()
        {
            PXCMImage.ImageData refEquals;

            sts = capture.ReadStreamAsync(images, out sps[0]);
            PXCMScheduler.SyncPoint.SynchronizeEx(sps);
            images[1].AcquireAccess(PXCMImage.Access.ACCESS_READ, out refEquals);

            short* depth = (short*)refEquals.buffer.planes[0];

            ushort* confidence = (ushort*)refEquals.buffer.planes[1];
            float* uv = (float*)refEquals.buffer.planes[2];

            int width = (int)images[1].imageInfo.width;
            int height = (int)images[1].imageInfo.height;
            if (depthFrame == null) this.depthFrame = new DepthCameraFrame(width, height);
            if (textureFrame == null) this.textureFrame = new TextureMapFrame(width, height);

            for (int i = 0; i < width * height; i++)
            {

                this.textureFrame.data[2 * i] = uv[2 * i];
                this.textureFrame.data[2 * i + 1] = uv[2 * i + 1];
                this.depthFrame.data[4 * i] = depth[3 * i];
                this.depthFrame.data[4 * i + 1] = depth[3 * i + 1];
                this.depthFrame.data[4 * i + 2] = depth[3 * i + 2];
                this.depthFrame.data[4 * i + 3] = confidence[i];
            }


            images[1].ReleaseAccess(ref refEquals);
            ((CLCalc.Program.Variable)depthFrame.GetMemoryObject()).WriteToDevice(depthFrame.data);
            ((CLCalc.Program.Variable)textureFrame.GetMemoryObject()).WriteToDevice(textureFrame.data);


            images[0].AcquireAccess(PXCMImage.Access.ACCESS_READ, PXCMImage.ColorFormat.COLOR_FORMAT_RGB32, out refEquals);

            byte* rgb = (byte*)refEquals.buffer.planes[0];

            width = (int)images[0].imageInfo.width;
            height = (int)images[0].imageInfo.height;
            if (colorFrame == null) this.colorFrame = new ColorCameraFrame(width, height);

            int offset = 0;
            for (int i = 0; i < width * height; i++)
            {

                this.colorFrame.data[offset] = rgb[offset + 2];
                this.colorFrame.data[offset + 1] = rgb[offset + 1];
                this.colorFrame.data[offset + 2] = rgb[offset];
                this.colorFrame.data[offset + 3] = rgb[offset + 3];
                offset += 4;
            }

            images[0].ReleaseAccess(ref refEquals);
            ((CLCalc.Program.Image2D)colorFrame.GetMemoryObject()).WriteToDevice(colorFrame.data);

            foreach (PXCMScheduler.SyncPoint s in sps) if (s != null) s.Dispose();
            foreach (PXCMImage i in images) if (i != null) i.Dispose();
        }

        public override void Dispose()
        {

            depthFrame.CloseAllOpenStreams();
            colorFrame.CloseAllOpenStreams();
            textureFrame.CloseAllOpenStreams();
            capture.Dispose();
            session.Dispose();
        }
    }
}
