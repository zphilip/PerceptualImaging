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
            devices.Add(new GestureCamera(@"."));
            //devices.Add(new PlayBackDevice(@"C:\Users\Blake\Desktop\capture_head_turn"));
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
