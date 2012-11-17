using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Perceptual.Foundation;
namespace Perceptual.Visualization
{
    public partial class MainWindow : BaseCameraApplication
    {

        protected PointCloudRender pointCloud;
        protected QuadSurfaceRender quadSurf;
        protected AdaptiveTemporalFilter filter;
        protected SceneBBoxRender sceneRender;
        protected ImageButtonRender overlayRender;
        protected Message hints;
        public MainWindow()
        {
            InitializeComponent(new System.Drawing.Size(1024, 768), "Camera Data Visualization");
        }
        protected override bool Setup()
        {
            devices.Add(new GestureCamera(@"."));
            //devices.Add(new PlayBackDevice(@"C:\"));
            TextOverlayRender textRender;
            filters.Add(filter = new AdaptiveTemporalFilter());
            renderers.Add(new CameraSetupRender(CameraSetupRender.SceneType.GL3D));
            renderers.Add(quadSurf = new QuadSurfaceRender());
            renderers.Add(pointCloud = new PointCloudRender());
            renderers.Add(sceneRender = new SceneBBoxRender());
            renderers.Add(new CameraSetupRender(CameraSetupRender.SceneType.GL2D));
            renderers.Add(overlayRender = new CameraOverlayRender());
            renderers.Add(textRender = new TextOverlayRender());
            textRender.Messages.Add(hints = new Message("", 0, glw.GLCtrl.Height - 200, 10));
            UpdateHints();
            return true;
        }
        protected override bool OnKeyPressed(KeyEventArgs e)
        {
            if (e.KeyData == Keys.C)
            {

                if (quadSurf != null)
                {
                    quadSurf.ShowColor = !quadSurf.ShowColor;

                }
                UpdateHints();
                return true;
            }
            else
                if (e.KeyData == Keys.M)
                {

                    if (quadSurf != null)
                    {
                        quadSurf.Visible = !quadSurf.Visible;

                    }
                    UpdateHints();
                    return true;
                }
                else
                    if (e.KeyData == Keys.W)
                    {

                        if (quadSurf != null)
                        {
                            quadSurf.WireFrame = !quadSurf.WireFrame;

                        }
                        UpdateHints();
                        return true;
                    }
                    else if (e.KeyData == Keys.P)
                    {

                        if (pointCloud != null)
                        {
                            pointCloud.Visisble = !pointCloud.Visisble;

                        }
                        UpdateHints();
                        return true;
                    }
                    else if (e.KeyData == Keys.B)
                    {

                        if (sceneRender != null)
                        {
                            sceneRender.Visible = !sceneRender.Visible;

                        }
                        UpdateHints();
                        return true;
                    }
                    else if (e.KeyData == Keys.O)
                    {

                        if (overlayRender != null)
                        {
                            overlayRender.Visible = !overlayRender.Visible;

                        }
                        UpdateHints();
                        return true;
                    }
                    else if (e.KeyData == Keys.Space)
                    {
                        isRunning = !isRunning;

                        UpdateHints();
                        return true;
                    }
                    else if (e.KeyData == Keys.R)
                    {
                        isRecording = !isRecording;
                        if (!isRecording)
                        {
                            GetPrimaryDevice().GetColorImage().CloseAllOpenStreams();
                            GetPrimaryDevice().GetTextureImage().CloseAllOpenStreams();
                            GetPrimaryDevice().GetDepthImage().CloseAllOpenStreams();
                        }
                        UpdateHints();
                        return true;
                    }
            return false;
        }

        protected void UpdateHints()
        {
            hints.SetText(
    @"M - Mesh (" + ((quadSurf.Visible) ? "On" : "Off") + @")
C - Mesh Color (" + ((quadSurf.ShowColor) ? "On" : "Off") + @")
W - Wireframe (" + ((quadSurf.WireFrame) ? "On" : "Off") + @")
P - Point Cloud (" + ((pointCloud.Visisble) ? "On" : "Off") + @")
B - Bounding Box (" + ((sceneRender.Visible) ? "On" : "Off") + @")
O - Overlay (" + ((overlayRender.Visible) ? "On" : "Off") + @")
R - Record (" + ((isRecording) ? "On" : "Off") + @")
Space - Capture (" + ((isRunning) ? "On" : "Off") + @")");
        }
        protected override bool OnMouseMove(MouseEventArgs e)
        {
            return overlayRender.ProcessMouseMove(e.X, e.Y);
        }

        protected override bool OnMouseDown(MouseEventArgs e)
        {
            return overlayRender.ProcessMouseButton(true);
        }

        protected override bool OnMouseUp(MouseEventArgs e)
        {

            return overlayRender.ProcessMouseButton(false);
        }

    }
}
