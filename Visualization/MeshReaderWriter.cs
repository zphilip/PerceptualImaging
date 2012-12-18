/*******************************************************************************

INTEL CORPORATION PROPRIETARY INFORMATION
This software is supplied under the terms of a license agreement or nondisclosure
agreement with Intel Corporation and may not be copied or disclosed except in
accordance with the terms of that agreement
Copyright(c) 2012 Intel Corporation. All Rights Reserved.

@Author {Blake C. Lucas (blake.c.lucas@intel.com)}
*******************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Perceptual.Foundation;
namespace Perceptual.Visualization
{
    public class MeshReaderWriter
    {
        protected int vertexCount, faceCount;
        protected StreamWriter writer;
        protected Stream dest;
        protected enum FileType { PLY, OBJ, XYZ };
        protected FileType type;
        protected string path;
        public MeshReaderWriter(int vertexCount, int faceCount, string path)
        {
            string ext = Path.GetExtension(path);
            if (ext.Equals(".obj"))
            {
                type = FileType.OBJ;
            }
            else if (ext.Equals(".xyz"))
            {
                type = FileType.XYZ;
            }
            else if (ext.Equals(".ply"))
            {
                type = FileType.PLY;
            }
            if (type != null)
            {
                System.Console.WriteLine(path);
                this.path = path;
                this.vertexCount = vertexCount;
                this.faceCount = faceCount;
                this.dest = File.Create(path);
                this.writer = new StreamWriter(dest);
                AddHeader();
            }
        }

        protected void AddHeader()
        {
            if (type == FileType.PLY && faceCount > 0)
            {
                writer.WriteLine("ply");
                writer.WriteLine("format ascii 1.0");
                writer.WriteLine("comment Created By Perceptual Computing Lab");
                writer.WriteLine("element vertex " + vertexCount);
                writer.WriteLine("property float x");
                writer.WriteLine("property float y");
                writer.WriteLine("property float z");
                writer.WriteLine("property float nx");
                writer.WriteLine("property float ny");
                writer.WriteLine("property float nz");
                writer.WriteLine("property uchar red");
                writer.WriteLine("property uchar green");
                writer.WriteLine("property uchar blue");
                writer.WriteLine("property uchar alpha");
                writer.WriteLine("element face " + faceCount);
                writer.WriteLine("property list uchar int vertex_indices");
                writer.WriteLine("end_header");
            }
            else if (type == FileType.PLY && faceCount == 0)
            {
                writer.WriteLine("ply");
                writer.WriteLine("format ascii 1.0");
                writer.WriteLine("comment Created By Perceptual Computing Lab");
                writer.WriteLine("element vertex " + vertexCount);
                writer.WriteLine("property float x");
                writer.WriteLine("property float y");
                writer.WriteLine("property float z");
                writer.WriteLine("property uchar red");
                writer.WriteLine("property uchar green");
                writer.WriteLine("property uchar blue");
                writer.WriteLine("property uchar alpha");
                writer.WriteLine("element face " + faceCount);
                writer.WriteLine("property list uchar int vertex_indices");
                writer.WriteLine("end_header");
            }
            else if (type == FileType.OBJ)
            {
                writer.WriteLine("####");
                writer.WriteLine("#");
                writer.WriteLine("# OBJ File Created by Perceptual Computing Lab");
                writer.WriteLine("#");
                writer.WriteLine("####");
                writer.WriteLine("# Object " + Path.GetFileName(path));
                writer.WriteLine("#");
                writer.WriteLine("# Vertices: " + vertexCount);
                writer.WriteLine("# Faces: " + faceCount);
                writer.WriteLine("#");
                writer.WriteLine("####");
            }

        }
        private int toColor(float x)
        {
            return (int)Math.Max(0, Math.Min((int)Math.Round(x * 255.0f), 255));
        }
        public void AddPoint(float4 vertex, float4 color)
        {
            if (type == FileType.PLY)
            {

                writer.WriteLine(
                    vertex.x.ToString("F6") + " " +
                    vertex.y.ToString("F6") + " " +
                    vertex.z.ToString("F6") + " " +
                    toColor(color.x) + " " +
                    toColor(color.y) + " " +
                    toColor(color.z) + " " +
                    toColor(color.w));
            }
            else if (type == FileType.OBJ)
            {
                writer.WriteLine("v " +
                    vertex.x.ToString("F6") + " " +
                    vertex.y.ToString("F6") + " " +
                    vertex.z.ToString("F6") + " " +
                    color.x.ToString("F6") + " " +
                    color.y.ToString("F6") + " " +
                    color.z.ToString("F6"));

            }
            else if (type == FileType.XYZ)
            {
                writer.WriteLine(
                    vertex.x.ToString("F6") + " " +
                    vertex.y.ToString("F6") + " " +
                    vertex.z.ToString("F6") + " " +
                    toColor(color.x) + " " +
                    toColor(color.y) + " " +
                    toColor(color.z));

            }
        }
        public void AddPoint(float4 vertex, float4 normal, float4 color)
        {
            if (type == FileType.PLY)
            {

                writer.WriteLine(
                    vertex.x.ToString("F6") + " " +
                    vertex.y.ToString("F6") + " " +
                    vertex.z.ToString("F6") + " " +
                    normal.x.ToString("F6") + " " +
                    normal.y.ToString("F6") + " " +
                    normal.z.ToString("F6") + " " +
                    toColor(color.x) + " " +
                    toColor(color.y) + " " +
                    toColor(color.z) + " " +
                    toColor(color.w));
            }
            else if (type == FileType.OBJ)
            {
                writer.WriteLine(
                    "vn " + normal.x.ToString("F6") + " " +
                    normal.y.ToString("F6") + " " +
                    normal.z.ToString("F6"));
                writer.WriteLine("v " +
                    vertex.x.ToString("F6") + " " +
                    vertex.y.ToString("F6") + " " +
                    vertex.z.ToString("F6") + " " +
                    color.x.ToString("F6") + " " +
                    color.y.ToString("F6") + " " +
                    color.z.ToString("F6"));

            }
            else if (type == FileType.XYZ)
            {

                writer.WriteLine(
                    vertex.x.ToString("F6") + " " +
                    vertex.y.ToString("F6") + " " +
                    vertex.z.ToString("F6") + " " +
                    normal.x.ToString("F6") + " " +
                    normal.y.ToString("F6") + " " +
                    normal.z.ToString("F6") + " " +
                    toColor(color.x) + " " +
                    toColor(color.y) + " " +
                    toColor(color.z));
            }
        }
        public void AddFace(int v1, int v2, int v3)
        {
            if (type == FileType.PLY)
            {
                writer.WriteLine("3 " + v1 + " " + v2 + " " + v3);
            }
            else if (type == FileType.OBJ)
            {
                v1++;
                v2++;
                v3++;
                writer.WriteLine("f " + v1 + "//" + v1 + " " + v2 + "//" + v2 + " " + v3 + "//" + v3);
            }
        }
        public void Close()
        {
            writer.Close();
        }
        public static void WriteTriangleList(float[] vertexes, float[] normals, float[] colors, string path)
        {
            int vertexCount = vertexes.Length / 4;
            MeshReaderWriter writer = new MeshReaderWriter(vertexCount, vertexCount / 3, path);
            for (int i = 0; i < vertexes.Length; i += 4)
            {
                int v = i / 4;
                writer.AddPoint(new float4(vertexes[i], vertexes[i + 1], vertexes[i + 2], 1), new float4(normals[3 * v], normals[3 * v + 1], normals[3 * v + 2], 1), new float4(colors[i], colors[i + 1], colors[i + 2], 1));
            }
            for (int i = 0; i < vertexCount; i += 3)
            {
                writer.AddFace(i, i + 1, i + 2);
            }
            writer.Close();
        }
    }
}
