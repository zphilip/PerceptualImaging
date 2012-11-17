using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTKWrapper.CLGLInterop;
using OpenTKWrapper;
using Cloo;
using OpenTK.Examples.Shapes;
using Perceptual.Foundation;
namespace Perceptual.RedEye
{
    public class RedEyeRender3D : PercRender
    {
        protected RedEyeDetector redEyeDetector;
        protected SlicedSphere sphere;
        public RedEyeRender3D(RedEyeDetector redEyeDetector)
        {
            this.redEyeDetector = redEyeDetector;
            sphere = new SlicedSphere(5.0f, Vector3d.Zero,
                                     SlicedSphere.eSubdivisions.Three,
                                     new SlicedSphere.eDir[] { SlicedSphere.eDir.All },
                                     true);

        }
        public void Initialize(BaseCameraApplication capture, GLAdvancedRender glw)
        {
        }
        public void Draw(GLAdvancedRender glw)
        {
            if (redEyeDetector.FoundFace())
            {

                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                GL.Enable(EnableCap.Lighting);
                FaceLandmarks faceDetection = redEyeDetector.GetFaceLandmarks();
                float4 leftEye = faceDetection.leftEye;
                float4 rightEye = faceDetection.rightEye;
                float4 bridge = faceDetection.bridge;
                float4 nose = faceDetection.nose;

                GL.PushMatrix();
                GL.Color3(0.0f, 0.0f, 0.5f);
                GL.Translate(leftEye.x, leftEye.y, leftEye.z);
                sphere.Draw();

                GL.PopMatrix();

                GL.PushMatrix();
                GL.Color3(0.0f, 0.5f, 0.0f);
                GL.Translate(rightEye.x, rightEye.y, rightEye.z);
                sphere.Draw();

                GL.PopMatrix();

                GL.PushMatrix();
                GL.Color3(0.7f, 0.5f, 0.1f);
                GL.Translate(bridge.x, bridge.y, bridge.z);
                sphere.Draw();

                GL.PopMatrix();

                GL.PushMatrix();
                GL.Color3(0.5f, 0.1f, 0.1f);
                GL.Translate(nose.x, nose.y, nose.z);
                sphere.Draw();

                GL.PopMatrix();

                GL.Color3(1.0f, 1.0f, 1.0f);
            }
        }
        public void Process(BaseCameraApplication app)
        {

        }
    }
}
