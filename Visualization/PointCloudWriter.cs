using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloo;
using OpenTKWrapper;
using Perceptual.Foundation;
namespace Perceptual.Visualization
{
    public class PointCloudWriter : CameraDataProcessor
    {
        protected string outputDir;
        protected int counter = 0;
        protected float[] points;
        protected float[] colors;
        protected CLCalc.Program.Kernel kernelCopyImage;
        protected CLCalc.Program.Variable pointBuffer, colorBuffer;
        public PointCloudWriter(string outputDir)
        {
            this.outputDir = outputDir;
        }
        public void Initialize(BaseCameraApplication app)
        {
            try
            {
                CLCalc.Program.Compile(app.GetPrimaryDevice().GetPreprocessCode() + src);
            }
            catch (BuildProgramFailureComputeException ex)
            {
                System.Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }
            int width = app.GetPrimaryDevice().GetDepthImage().Width;

            int height = app.GetPrimaryDevice().GetDepthImage().Height;
            pointBuffer = CLCalc.Program.Variable.Create(new ComputeBuffer<float>(CLCalc.Program.Context, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, points = new float[width * height * 4]));
            colorBuffer = CLCalc.Program.Variable.Create(new ComputeBuffer<float>(CLCalc.Program.Context, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, colors = new float[width * height * 4]));

            kernelCopyImage = new CLCalc.Program.Kernel("CopyToPoinCloud");
        }

        public void Process(BaseCameraApplication app)
        {
            CameraDataFilter filter = app.GetImageFilter();
            kernelCopyImage.Execute(new CLCalc.Program.MemoryObject[] { 
                app.GetPrimaryDevice().GetDepthImage().GetMemoryObject(),
                filter.GetTextureImage(),
                filter.GetColorImage(),
                pointBuffer,
                colorBuffer}, new int[] { filter.GetDepthImage().Width, filter.GetDepthImage().Height });
            pointBuffer.ReadFromDeviceTo(points);
            int vertexCount = points.Length / 4;
            /*
            for (int i = 0; i < points.Length; i += 4)
            {

                float4 pt = new float4(points[i], points[i + 1], points[i + 2], points[i + 3]);
                if (pt.z>200.0&&pt.z < 2000.0f&&pt.w>100.0f)
                {
                    vertexCount++;
                }
            }
             * */
            colorBuffer.ReadFromDeviceTo(colors);
            string path = outputDir + "pointcloud" + counter.ToString("0000") + ".xyz";
            MeshReaderWriter writer = new MeshReaderWriter(vertexCount, 0, path);
            float minDepth = app.GetPrimaryDevice().GetMinDepth();
            float maxDepth = app.GetPrimaryDevice().GetMaxDepth();
            for (int i = 0; i < points.Length; i += 4)
            {
                float4 pt = new float4(points[i], points[i + 1], points[i + 2], points[i + 3]);
                float4 rgb = new float4(colors[i], colors[i + 1], colors[i + 2], colors[i + 3]);
                if (pt.z > minDepth && pt.z < maxDepth && pt.w > 100.0f)
                {
                    writer.AddPoint(pt, rgb);
                }
                else
                {
                    writer.AddPoint(new float4(), new float4());
                }
            }
            writer.Close();
            counter++;
        }
        string src = @"        
        kernel void CopyToPoinCloud(
            global float4* depthData,
            read_only image2d_t uvData,
            read_only image2d_t originalImage,
            global float4* points,
            global float4* colors)
        {
           const sampler_t smpLinear = CLK_NORMALIZED_COORDS_FALSE | CLK_ADDRESS_CLAMP | CLK_FILTER_LINEAR;
           const sampler_t smpNearest = CLK_NORMALIZED_COORDS_FALSE | CLK_ADDRESS_CLAMP | CLK_FILTER_NEAREST;
           const sampler_t normalizedSmp = CLK_NORMALIZED_COORDS_TRUE | CLK_ADDRESS_CLAMP | CLK_FILTER_LINEAR;

           int i = get_global_id(0);
           int j = get_global_id(1);
           int2 coords = (int2)(i,j); 
           int index=i+j*get_global_size(0);
           float2 p;
           float4 pt=depthData[index];          

           p.x=FOCAL_X*pt.x/pt.z+CENTER_X;
           p.y=CENTER_Y-FOCAL_Y*pt.y/pt.z;

           float4 uv=read_imagef(uvData,smpLinear,p);
           int4 rgba=read_imagei(originalImage,normalizedSmp,(float2)(uv.x,uv.y));
           float4 color=(uv.x<0||uv.x>1.0f||uv.y<0||uv.y>1.0f)?(float4)(0.0f,0.0f,0.0f,0.0f):(float4)(rgba.x/255.0f,rgba.y/255.0f,rgba.z/255.0f,1.0f);
            points[index]=pt;
            colors[index]=color;
        }";
    }
}
