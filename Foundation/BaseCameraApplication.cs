using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTKWrapper.CLGLInterop;
using OpenTKWrapper;
namespace PercFoundation
{
    public partial class BaseCameraApplication : Form
    {
        protected List<CameraDevice> devices = new List<CameraDevice>();
        protected List<PercRender> renderers = new List<PercRender>();
        protected List<CameraDataFilter> filters = new List<CameraDataFilter>();
        protected List<CameraDataProcessor> processors = new List<CameraDataProcessor>();
        protected Timer timer = new Timer();
        protected const int REFRESH_INTERVAL = 20;

        protected virtual bool OnMouseMove(MouseEventArgs e) { return false; }
        protected virtual bool OnMouseDown(MouseEventArgs e) { return false; }
        protected virtual bool OnMouseUp(MouseEventArgs e) { return false; }
        protected virtual bool OnKeyPressed(KeyEventArgs e) { return false; }
        protected virtual bool Setup() { return false; }
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


        private void DoAdvancedDraw()
        {
            foreach (PercRender render in renderers)
            {
                render.Draw(glw);
            }

        }



        protected void Start(object sender, EventArgs e)
        {

            //Creates OpenCL/GL shared environment. 1 line   :-)
            glw = new GLAdvancedRender(this, true, -1);
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
            glw.modelView = new OpenTK.Matrix4(-0.8457233f, 0, -0.3078182f, 0, 0, 0.9f, 0, 0, 0.3078182f, 0, -0.8457233f, 0, -138.5182f, 0, 380.5755f, 1);
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
                foreach (CameraDataProcessor processor in processors)
                {
                    processor.Process(this);
                }


                foreach (PercRender render in renderers)
                {
                    render.Process(this);
                }
            }
            glw.ReDraw();

        }
        protected void Release(object sender, FormClosingEventArgs e)
        {
            timer.Stop();
            Dispose();
            if (context != null) context.Dispose();

        }
        protected void InitializeComponent(System.Drawing.Size sz,string name)
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
