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
using PercFoundation;
using OpenTKWrapper;
using OpenTKWrapper.CLGLInterop;
using OpenTK;
using OpenTK.Graphics.OpenGL;
namespace PercVisualization
{
    public class CameraSetupRender : PercRender
    {
        public enum SceneType { GL2D, GL3D };
        protected SceneType sceneType;
        protected float minDepth, maxDepth;
        protected BoundingBox bbox;
        protected Matrix4 GroundPlane = Matrix4.Identity;
        protected int screenWidth, screenHeight;
        public CameraSetupRender(SceneType type)
        {
            this.sceneType = type;
        }
        public void Initialize(BaseCameraApplication app, GLAdvancedRender glw)
        {
            minDepth = app.GetPrimaryDevice().GetMinDepth();
            maxDepth = app.GetPrimaryDevice().GetMaxDepth();
            bbox = app.GetPrimaryDevice().GetBoundingBox();
            screenWidth = glw.GLCtrl.Width;
            screenHeight = glw.GLCtrl.Height;
        }
        public void Draw(GLAdvancedRender glw)
        {
            GL.Viewport(0, 0, glw.GLCtrl.Width, glw.GLCtrl.Height);
            if (sceneType == SceneType.GL3D)
            {
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                GL.Translate(glw.trans.x / (double)glw.GLCtrl.Width, -glw.trans.y / (double)glw.GLCtrl.Height, 0);
                OpenTK.Matrix4d m1 = OpenTK.Matrix4d.CreatePerspectiveFieldOfView(Math.PI * 0.25f,
                    (glw.GLCtrl.Width) / (float)glw.GLCtrl.Height, minDepth, maxDepth);
                GL.MultMatrix(ref m1);

                OpenTK.Matrix4d m2 = OpenTK.Matrix4d.LookAt(glw.eye.x, glw.eye.y, glw.eye.z, glw.center.x, glw.center.y, glw.center.z,
                    glw.up.x, glw.up.y, glw.up.z);

                GL.MultMatrix(ref m2);
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();

                GL.MultMatrix(ref glw.modelView);

                GL.Translate(bbox.center.x, bbox.center.y, bbox.center.z);
                GL.Rotate(90.0f, new Vector3d(1, 0, 0));
                GL.Translate(-bbox.center.x, -bbox.center.y, -bbox.center.z);

                GL.Translate(0, 0, bbox.maxPoint.z);

                GL.MultMatrix(ref GroundPlane);

                GL.Enable(EnableCap.DepthTest);
                GL.Enable(EnableCap.Lighting);
            }
            else if (sceneType == SceneType.GL2D)
            {
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                GL.Ortho(0, screenWidth, screenHeight, 0.0f, 0.0, 100.0);
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();
                GL.Disable(EnableCap.Lighting);
                GL.Disable(EnableCap.DepthTest);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            }
        }
        public void Process(BaseCameraApplication app)
        {
            this.GroundPlane = app.GetPrimaryDevice().GetGroundPlane();
        }
    }
}
