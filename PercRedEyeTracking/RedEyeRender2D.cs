using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Perceptual.Foundation;
using Perceptual.Visualization;
using OpenTKWrapper;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTKWrapper.CLGLInterop;
namespace Perceptual.RedEye
{
    public class RedEyeRender2D : CameraOverlayRender
    {
        protected RedEyeDetector redEyeDetector;
        public RedEyeRender2D(RedEyeDetector redEyeDetector)
        {
            this.redEyeDetector = redEyeDetector;

        }
        protected int xOffset, yOffset;
        protected int irWidth;

        public override void Draw(GLAdvancedRender glw)
        {
            base.Draw(glw);
            if (redEyeDetector.FoundFace())
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                GL.LineWidth(2.0f);
                int2 leftEye = redEyeDetector.GetLeftEye2D();
                int2 rightEye = redEyeDetector.GetRightEye2D();
                int2 minPoint = redEyeDetector.GetMinPoint2D();
                int2 sz = redEyeDetector.GetFaceDimensions();
                int w = this.depthButton.GetSize().x;
                GL.PushMatrix();
                int2 offset = this.depthButton.GetLocation();
                GL.Translate(offset.x,offset.y,0);
                GL.PointSize(5.0f);
                GL.Enable(EnableCap.PointSmooth);
                GL.Begin(BeginMode.Points);
                GL.Color3(0.8f, 0.2f, 0.2f);
                GL.Vertex2(w-leftEye.x, leftEye.y);
                GL.Color3(0.8f, 0.2f, 0.2f);
                GL.Vertex2(w-rightEye.x, rightEye.y);
                GL.End();
                GL.Begin(BeginMode.Quads);
                GL.Vertex2(w-minPoint.x, minPoint.y);
                GL.Vertex2(w-minPoint.x-sz.x, minPoint.y);
                GL.Vertex2(w-minPoint.x-sz.x, minPoint.y+sz.y);
                GL.Vertex2(w-minPoint.x, minPoint.y+sz.y);
                GL.End();
                GL.PopMatrix();
                GL.PushMatrix();
                offset = this.colorButton.GetLocation();
                GL.Translate(offset.x, offset.y, 0);
                GL.PointSize(5.0f);
                GL.Enable(EnableCap.PointSmooth);
                GL.Begin(BeginMode.Points);
                GL.Color3(0.8f, 0.2f, 0.2f);
                GL.Vertex2(w - leftEye.x, leftEye.y);
                GL.Color3(0.8f, 0.2f, 0.2f);
                GL.Vertex2(w - rightEye.x, rightEye.y);
                GL.End();
                GL.Begin(BeginMode.Quads);
                GL.Vertex2(w - minPoint.x, minPoint.y);
                GL.Vertex2(w - minPoint.x - sz.x, minPoint.y);
                GL.Vertex2(w - minPoint.x - sz.x, minPoint.y + sz.y);
                GL.Vertex2(w - minPoint.x, minPoint.y + sz.y);
                GL.End();
                GL.PopMatrix();
                GL.Color3(1.0f, 1.0f, 1.0f);
            }
        }
    }
}
