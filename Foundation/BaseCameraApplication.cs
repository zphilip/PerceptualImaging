using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloo;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using OpenTK.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTKWrapper.CLGLInterop;
using OpenTKWrapper;
using System.Diagnostics;
namespace Perceptual.Foundation
{
    public partial class BaseCameraApplication : Form
    {
        protected List<CameraDevice> devices = new List<CameraDevice>();
        protected List<PercRender> renderers = new List<PercRender>();
        protected List<CameraDataFilter> filters = new List<CameraDataFilter>();
        protected List<CameraDataProcessor> processors = new List<CameraDataProcessor>();
        protected Timer timer = new Timer();
        protected bool enableProfiling = false;
        protected const int REFRESH_INTERVAL = 16;
        protected Stopwatch stopwatch = new Stopwatch();
        protected virtual bool OnMouseMove(MouseEventArgs e) { return false; }
        protected virtual bool OnMouseDown(MouseEventArgs e) { return false; }
        protected virtual bool OnMouseUp(MouseEventArgs e) { return false; }
        protected virtual bool OnKeyPressed(KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
                return true;
            }
            else if (e.KeyCode == Keys.F11)
            {
                SwitchToFullScreen();
            }
            return false; 
        }
        protected virtual bool Setup() { return false; }
        public void SetEnableProfiling(bool enable)
        {
            this.enableProfiling = enable;
        }
        private bool fullscreen = false;
        private System.Drawing.Size originalClientSize;
        public void SwitchToFullScreen(){
            if (!fullscreen)
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.TopMost = true;
                this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                this.Location = new Point(0, 0);
                this.originalClientSize = this.ClientSize;

                this.ClientSize = Screen.PrimaryScreen.Bounds.Size;
                DisplayDevice.Default.ChangeResolution(ClientSize.Width, ClientSize.Height, 32, 60.0f);
            }
            else
            {
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.TopMost = false;
                this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                this.ClientSize = originalClientSize;
                DisplayDevice.Default.RestoreResolution();
            }
            fullscreen = !fullscreen;
        }
        public bool Initialize()
        {
            foreach (CameraDevice device in devices)
            {
                if (!device.Initialize()) return false;
            }
            return true;
        }

        public List<CameraDevice> GetDevices()
        {
            return devices;
        }
        public CameraDevice GetPrimaryDevice()
        {
            return devices.First();
        }
        public CameraDataFilter GetImageFilter()
        {
            return filters.Last();
        }

        public OpenTKWrapper.CLGLInterop.GLAdvancedRender GetRenderContext()
        {
            return glw;
        }
        public void Dispose()
        {

            foreach (CameraDevice device in devices)
            {
                device.Dispose();
            }
        }
        protected ComputeContext context;
        protected GLAdvancedRender glw;

        protected bool isRecording = false;

        protected bool isRunning = true;


        OpenTK.Matrix4 currentPose = OpenTK.Matrix4.Identity;
        OpenTK.Matrix4 currentPoseInverse = OpenTK.Matrix4.Identity;

        int count = 0;

        public void SetRunning(bool running)
        {
            this.isRunning = running;
        }
        private void DoAdvancedDraw()
        {
            if (enableProfiling) stopwatch.Restart();
            foreach (PercRender render in renderers)
            {
                render.Draw(glw);
            }
            if (enableProfiling)
            {
                CLCalc.Program.CommQueues[CLCalc.Program.DefaultCQ].Finish();
                stopwatch.Stop();
                long ms = stopwatch.ElapsedMilliseconds;
                System.Console.WriteLine("Draw Frame Rate " + (1000.0f / ms).ToString("F1") + " fps");
            }
        }

        private Vector initEye, initCenter, initTrans, initUp, initEsq,initFront;
        private double initDistEye;
        private float zNear,zFar;
        private Matrix4 initModelView;
        public void ResetView()
        {
            glw.center = this.initCenter;
            glw.eye = this.initEye;
            glw.distEye = this.initDistEye;
            glw.up = this.initUp;
            glw.trans = this.initTrans;
            glw.front = this.initFront;
            glw.esq = this.initEsq;
            glw.zNear = this.zNear;
            glw.zFar = this.zFar;
            glw.modelView = this.initModelView;
            glw.ConsolidateRepositioning();
        }
        public void SetDefaultView(){
            this.initCenter = glw.center;
            this.initEye = glw.eye;
            this.initDistEye = glw.distEye;
            this.initUp = glw.up;
            this.initTrans = glw.trans;
            this.initEsq = glw.esq;
            this.initFront = glw.front;
            this.zNear = glw.zNear;
            this.zFar = glw.zFar;
            this.initModelView = glw.modelView;
        }
        protected void Start(object sender, EventArgs e)
        {

            //Creates OpenCL/GL shared environment. 1 line   :-)
            foreach (ComputePlatform platform in ComputePlatform.Platforms)
            {
                System.Console.WriteLine("PLATFORM " + platform.ToString());
                foreach (ComputeDevice device in platform.Devices)
                {
                    System.Console.WriteLine("DEVICE " + device.ToString() + " : " + device.Type);
                }
            }
            glw = new GLAdvancedRender(this, true, ComputeDeviceTypes.Gpu);



            glw.DrawAxes = false;
            glw.MouseMode = GLAdvancedRender.MouseMoveMode.RotateModel;
            glw.eye.z = 1000.0f;
            glw.distEye = 1000.0f;
            glw.SetAdvancedDrawMethod(DoAdvancedDraw);
            glw.SetKeyPressedMethod(OnKeyPressed);
            glw.SetMouseDownMethod(OnMouseDown);
            glw.SetMouseUpMethod(OnMouseUp);
            glw.SetMouseMoveMethod(OnMouseMove);
            glw.ClearColor = new float[] { 0.0f, 0.0f, 0.0f };
            glw.modelView = new OpenTK.Matrix4(-0.8457233f, 0, -0.3078182f, 0, 0, 0.9f, 0, 0, 0.3078182f, 0, -0.8457233f, 0, -138.5182f, 0, 380.5755f, 1);
            SetDefaultView();
            //Sets mouse mode to translate
            glw.MouseMode = GLAdvancedRender.MouseMoveMode.RotateModel;

            GL.PointSize(3.0f);
            if (!Setup())
            {
                System.Console.WriteLine("Failed to Setup Application.");
                Environment.Exit(1);
            }
            if (!Initialize())
            {
                System.Console.WriteLine("Failed to Initialize Application.");
                Environment.Exit(1);
            }
            float4 center = GetPrimaryDevice().GetBoundingBox().center;
            GetPrimaryDevice().GetNextFrame();
            BoundingBox bbox = GetPrimaryDevice().GetBoundingBox();

            Vector3 pitchAxis = new Vector3(-1, 0, 0);
            Matrix4 M = OpenTK.Matrix4.CreateTranslation(0, -bbox.minPoint.y, -bbox.center.z) * OpenTK.Matrix4.CreateFromAxisAngle(pitchAxis, 0.5f * (float)Math.PI);

            GetPrimaryDevice().SetGroundPlane(M);
            foreach (CameraDataFilter f in filters)
            {
                f.Initialize(this);
            }
            foreach (CameraDataProcessor processor in processors)
            {
                processor.Initialize(this);
            }
            foreach (PercRender render in renderers)
            {
                render.Initialize(this, glw);
            }

            timer.Tick += ProcessFrame;
            timer.Interval = REFRESH_INTERVAL;
            timer.Enabled = true;
            timer.Start();
        }
        private void ProcessFrame(object sender, EventArgs e)
        {

            if (timer.Enabled)
            {
                long ms;

                if (enableProfiling) stopwatch.Restart();
                if (isRunning)
                {

                    GetPrimaryDevice().GetNextFrame();
                    if (isRecording)
                    {
                        GetPrimaryDevice().RecordCurrentFrame();
                    }

                    foreach (CameraDataFilter filter in filters)
                    {
                        filter.Process(this);
                    }
                }
                if (enableProfiling)
                {
                    CLCalc.Program.CommQueues[CLCalc.Program.DefaultCQ].Finish();
                    stopwatch.Stop();
                    ms = stopwatch.ElapsedMilliseconds;
                    System.Console.WriteLine("Filter Frame Rate " + (1000.0f / ms).ToString("F1") + " fps");
                    stopwatch.Restart();
                }
                if (isRunning)
                {
                    foreach (CameraDataProcessor processor in processors)
                    {
                        processor.Process(this);
                    }
                }
                if (enableProfiling)
                {
                    CLCalc.Program.CommQueues[CLCalc.Program.DefaultCQ].Finish();
                    stopwatch.Stop();
                    ms = stopwatch.ElapsedMilliseconds;
                    System.Console.WriteLine("Compute Process Frame Rate " + (1000.0f / ms).ToString("F1") + " fps");
                    stopwatch.Restart();
                }
                foreach (PercRender render in renderers)
                {
                    render.Process(this);
                }
                CLCalc.Program.CommQueues[CLCalc.Program.DefaultCQ].Finish();
                if (enableProfiling)
                {
                    stopwatch.Stop();
                    ms = stopwatch.ElapsedMilliseconds;
                    System.Console.WriteLine("Render Process Rate Rate " + (1000.0f / ms).ToString("F1") + " fps");
                }
            }
            glw.ReDraw();

        }
        protected void Release(object sender, FormClosingEventArgs e)
        {
            timer.Stop();
            Dispose();
            CLCalc.Program.CommQueues[CLCalc.Program.DefaultCQ].Finish();
            if (context != null) context.Dispose();

        }
        protected void InitializeComponent(System.Drawing.Size sz, string name)
        {
            this.SuspendLayout();
            this.ClientSize = sz;
            this.Name = name;
            this.Text = name;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BaseCameraApplication));

            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Release);
            this.Load += new System.EventHandler(this.Start);
            this.ResumeLayout(false);

        }
        protected void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BaseCameraApplication));
            this.SuspendLayout();
            // 
            // BaseCameraApplication
            // 
            this.ClientSize = new System.Drawing.Size(1024, 768);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BaseCameraApplication";
            this.Text = "Base Camera Application";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Release);
            this.Load += new System.EventHandler(this.Start);
            this.ResumeLayout(false);

        }
    }
}
