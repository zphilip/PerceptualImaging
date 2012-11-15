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
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Examples.Shapes;
namespace PercVisualization
{
    public class SceneBBoxRender : PercRender
    {

        protected BoundingBox bbox;
        public bool Visible = true;
        protected const float GROUND_THICKNESS = 10.0f;
        protected Matrix4 GroundPlane;
        protected static SlicedSphere sphere = new SlicedSphere(5.0f, Vector3d.Zero,
         SlicedSphere.eSubdivisions.Three,
         new SlicedSphere.eDir[] { SlicedSphere.eDir.All },
         true);
        public void Initialize(BaseCameraApplication app, OpenTKWrapper.CLGLInterop.GLAdvancedRender glw)
        {
        }

        public void Draw(OpenTKWrapper.CLGLInterop.GLAdvancedRender glw)
        {

            if (Visible)
            {
                GL.Disable(EnableCap.Lighting);
                GL.Color4(0.5f, 0.5f, 0.5f, 0.5f);


                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                GL.Begin(BeginMode.Quads);
                GL.Vertex3(bbox.minPoint.x, bbox.minPoint.y, bbox.maxPoint.z);
                GL.Vertex3(bbox.maxPoint.x, bbox.minPoint.y, bbox.maxPoint.z);
                GL.Vertex3(bbox.maxPoint.x, bbox.maxPoint.y, bbox.maxPoint.z);
                GL.Vertex3(bbox.minPoint.x, bbox.maxPoint.y, bbox.maxPoint.z);
                GL.End();

                GL.LineWidth(2.0f);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

                GL.Color3(0.82f, 0.0f, 0f);
                GL.Begin(BeginMode.Quads);

                GL.Vertex3(bbox.minPoint.x, bbox.minPoint.y, bbox.minPoint.z);
                GL.Vertex3(bbox.maxPoint.x, bbox.minPoint.y, bbox.minPoint.z);
                GL.Vertex3(bbox.maxPoint.x, bbox.maxPoint.y, bbox.minPoint.z);
                GL.Vertex3(bbox.minPoint.x, bbox.maxPoint.y, bbox.minPoint.z);


                GL.Vertex3(bbox.minPoint.x, bbox.minPoint.y, bbox.maxPoint.z);
                GL.Vertex3(bbox.maxPoint.x, bbox.minPoint.y, bbox.maxPoint.z);
                GL.Vertex3(bbox.maxPoint.x, bbox.maxPoint.y, bbox.maxPoint.z);
                GL.Vertex3(bbox.minPoint.x, bbox.maxPoint.y, bbox.maxPoint.z);


                GL.Vertex3(bbox.minPoint.x, bbox.minPoint.y, bbox.minPoint.z);
                GL.Vertex3(bbox.minPoint.x, bbox.minPoint.y, bbox.maxPoint.z);
                GL.Vertex3(bbox.maxPoint.x, bbox.minPoint.y, bbox.maxPoint.z);
                GL.Vertex3(bbox.maxPoint.x, bbox.minPoint.y, bbox.minPoint.z);


                GL.Vertex3(bbox.minPoint.x, bbox.maxPoint.y, bbox.minPoint.z);
                GL.Vertex3(bbox.minPoint.x, bbox.maxPoint.y, bbox.maxPoint.z);
                GL.Vertex3(bbox.maxPoint.x, bbox.maxPoint.y, bbox.maxPoint.z);
                GL.Vertex3(bbox.maxPoint.x, bbox.maxPoint.y, bbox.minPoint.z);


                GL.Vertex3(bbox.minPoint.x, bbox.minPoint.y, bbox.minPoint.z);
                GL.Vertex3(bbox.minPoint.x, bbox.minPoint.y, bbox.maxPoint.z);
                GL.Vertex3(bbox.minPoint.x, bbox.maxPoint.y, bbox.maxPoint.z);
                GL.Vertex3(bbox.minPoint.x, bbox.maxPoint.y, bbox.minPoint.z);


                GL.Vertex3(bbox.maxPoint.x, bbox.minPoint.y, bbox.minPoint.z);
                GL.Vertex3(bbox.maxPoint.x, bbox.minPoint.y, bbox.maxPoint.z);
                GL.Vertex3(bbox.maxPoint.x, bbox.maxPoint.y, bbox.maxPoint.z);
                GL.Vertex3(bbox.maxPoint.x, bbox.maxPoint.y, bbox.minPoint.z);

                GL.End();

                GL.PushMatrix();
                GL.MultMatrix(ref GroundPlane);
                GL.Translate(0, 0, -bbox.maxPoint.z);
                GL.Color4(0.0f, 0.0f, 0.82f, 0.5f);


                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                GL.Begin(BeginMode.Quads);
                GL.Vertex3(bbox.minPoint.x, bbox.minPoint.y, bbox.maxPoint.z - GROUND_THICKNESS * 0.5f);
                GL.Vertex3(bbox.maxPoint.x, bbox.minPoint.y, bbox.maxPoint.z - GROUND_THICKNESS * 0.5f);
                GL.Vertex3(bbox.maxPoint.x, bbox.maxPoint.y, bbox.maxPoint.z - GROUND_THICKNESS * 0.5f);
                GL.Vertex3(bbox.minPoint.x, bbox.maxPoint.y, bbox.maxPoint.z - GROUND_THICKNESS * 0.5f);

                GL.End();

                GL.LineWidth(2.0f);

                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

                GL.Color3(0.0f, 0.0f, 0.82f);
                GL.Begin(BeginMode.Quads);


                GL.Vertex3(bbox.minPoint.x, bbox.minPoint.y, bbox.maxPoint.z - GROUND_THICKNESS * 0.5f);
                GL.Vertex3(bbox.maxPoint.x, bbox.minPoint.y, bbox.maxPoint.z - GROUND_THICKNESS * 0.5f);
                GL.Vertex3(bbox.maxPoint.x, bbox.maxPoint.y, bbox.maxPoint.z - GROUND_THICKNESS * 0.5f);
                GL.Vertex3(bbox.minPoint.x, bbox.maxPoint.y, bbox.maxPoint.z - GROUND_THICKNESS * 0.5f);

                GL.End();

                GL.PopMatrix();


                GL.Enable(EnableCap.Lighting);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                GL.Color3(0.5f, 0.5f, 0.8f);
                sphere.Draw();

                GL.Color3(1.0f, 1.0f, 1.0f);

            }
        }

        public void Process(BaseCameraApplication app)
        {
            bbox = app.GetPrimaryDevice().GetBoundingBox();
            GroundPlane = app.GetPrimaryDevice().GetGroundPlane();
            GroundPlane.Invert();
        }
    }
}
