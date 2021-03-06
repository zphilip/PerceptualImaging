﻿/*******************************************************************************

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
using OpenTKWrapper;
namespace Perceptual.Foundation
{
    public interface CameraDataFilter
    {
        void Process(BaseCameraApplication capture);
        void Initialize(BaseCameraApplication capture);
        CLCalc.Program.Variable GetMotionBuffer();
        CLCalc.Program.Image2D GetDepthImage();
        CLCalc.Program.Image2D GetTextureImage();
        CLCalc.Program.Image2D GetColorImage();
    }
}
