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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Perceptual.Foundation;
using Perceptual.Visualization;
namespace Perceptual.RedEye
{
    public partial class RedEyeWindow : BaseCameraApplication
    {
        public RedEyeWindow()
        {
            InitializeComponent(new System.Drawing.Size(1024, 768), "Red Eye Tracking");
        }
        protected override bool Setup()
        {
            //devices.Add(new GestureCamera(@"."));
            devices.Add(new PlayBackDevice(@"C:\Users\PerC\Desktop\capture"));
            AdaptiveTemporalFilter filter;
            RedEyeDetector detector;
            processors.Add(detector = new RedEyeDetector());
            filters.Add(filter = new AdaptiveTemporalFilter());
            renderers.Add(new CameraSetupRender(CameraSetupRender.SceneType.GL3D));
            renderers.Add(new QuadSurfaceRender());
            renderers.Add(new RedEyeRender3D(detector));
            renderers.Add(new CameraSetupRender(CameraSetupRender.SceneType.GL2D));
            renderers.Add(new RedEyeRender2D(detector));
            return true;
        }
    }
}
