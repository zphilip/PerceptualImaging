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
using OpenTK;
using Cloo;
using OpenTK.Graphics.OpenGL;
using OpenTKWrapper;
using System.Drawing;
using OpenTK;
namespace Perceptual.Foundation
{
    public struct float4
    {
        public float x, y, z, w;
        public float4(float[] val)
        {
            x = val[0]; y = val[1]; z = val[2]; w = val[3];
        }
        public float4(float val)
        {
            this.x = this.y = this.z = this.w = val;
        }
        public float4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
        public float4(PXCMPoint3DF32 pt)
        {
            this.x = pt.x;
            this.y = pt.y;
            this.z = pt.z;
            this.w = 1.0f;
        }
        public bool inside(BoundingBox bbox)
        {
            return (
                this.x >= bbox.minPoint.x &&
                this.y >= bbox.minPoint.y &&
                this.z >= bbox.minPoint.z &&
                this.x <= bbox.maxPoint.x &&
                this.y <= bbox.maxPoint.y &&
                this.z <= bbox.maxPoint.z);
        }

        public static implicit operator CLCalc.Program.MemoryObject(float4 pt)
        {
            return new CLCalc.Program.Value<float4>(pt);
        }
        public static implicit operator OpenTK.Vector4(float4 M)
        {
            return new Vector4(M.x, M.y, M.z, M.w);
        }
        public static implicit operator float4(Vector4 M)
        {
            return new float4(M.X, M.Y, M.Z, M.W);
        }
        public float dot(float4 pt)
        {
            return (float)(this.x * pt.x + this.y * pt.y + this.z * pt.z + this.w * pt.w);
        }
        public static float4 operator +(float4 c1, float4 c2)
        {
            return new float4(c1.x + c2.x, c1.y + c2.y, c1.z + c2.z, c1.w + c2.w);
        }
        public static float4 operator -(float4 c1, float4 c2)
        {
            return new float4(c1.x - c2.x, c1.y - c2.y, c1.z - c2.z, c1.w - c2.w);
        }
        public static float4 operator /(float4 c1, float4 c2)
        {
            return new float4(c1.x / c2.x, c1.y / c2.y, c1.z / c2.z, c1.w / c2.w);
        }
        public static float4 operator *(float4 c1, float4 c2)
        {
            return new float4(c1.x * c2.x, c1.y * c2.y, c1.z * c2.z, c1.w * c2.w);
        }

        public static float4 operator /(float4 c1, float a)
        {
            return new float4(c1.x / a, c1.y / a, c1.z / a, c1.w / a);
        }
        public static float4 operator *(float4 c1, float a)
        {
            return new float4(c1.x * a, c1.y * a, c1.z * a, c1.w * a);
        }

        public static float4 operator *(float a, float4 c1)
        {
            return new float4(c1.x * a, c1.y * a, c1.z * a, c1.w * a);
        }
        public float distance(float4 pt2)
        {
            return (float)Math.Sqrt((this.x - pt2.x) * (this.x - pt2.x) + (this.y - pt2.y) * (this.y - pt2.y) + (this.z - pt2.z) * (this.z - pt2.z));
        }

        public float4 min(float4 v2)
        {
            return new float4(Math.Min(this.x, v2.x), Math.Min(this.y, v2.y), Math.Min(this.z, v2.z), Math.Min(this.w, v2.w));
        }
        public float4 max(float4 v2)
        {
            return new float4(Math.Max(this.x, v2.x), Math.Max(this.y, v2.y), Math.Max(this.z, v2.z), Math.Max(this.w, v2.w));
        }
        public float length()
        {
            return (float)Math.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z + this.w * this.w);
        }
        public string ToString()
        {
            return "(" + x + "," + y + "," + z + "," + w + ")";
        }
    }
    public struct float2
    {
        public float x, y;
        public float2(float[] val)
        {
            x = val[0]; y = val[1];
        }
        public float2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public static implicit operator OpenTK.Vector2(float2 M)
        {
            return new Vector2(M.x, M.y);
        }

        public static implicit operator CLCalc.Program.MemoryObject(float2 pt)
        {
            return new CLCalc.Program.Value<float2>(pt);
        }

        public static implicit operator float2(Vector2 M)
        {
            return new float2(M.X, M.Y);
        }
        public float dot(float2 pt)
        {
            return (float)(this.x * pt.x + this.y * pt.y);
        }
        public float distance(float2 pt2)
        {
            return (float)Math.Sqrt((this.x - pt2.x) * (this.x - pt2.x) + (this.y - pt2.y) * (this.y - pt2.y));
        }
        public static float2 operator +(float2 c1, float2 c2)
        {
            return new float2(c1.x + c2.x, c1.y + c2.y);
        }
        public static float2 operator -(float2 c1, float2 c2)
        {
            return new float2(c1.x - c2.x, c1.y - c2.y);
        }
        public static float2 operator /(float2 c1, float2 c2)
        {
            return new float2(c1.x / c2.x, c1.y / c2.y);
        }
        public static float2 operator *(float2 c1, float2 c2)
        {
            return new float2(c1.x * c2.x, c1.y * c2.y);
        }

        public static float2 operator /(float2 c1, float a)
        {
            return new float2(c1.x / a, c1.y / a);
        }
        public static float2 operator *(float2 c1, float a)
        {
            return new float2(c1.x * a, c1.y * a);
        }
        public float2 min(float2 v2)
        {
            return new float2(Math.Min(this.x, v2.x), Math.Min(this.y, v2.y));
        }
        public float2 max(float2 v2)
        {
            return new float2(Math.Max(this.x, v2.x), Math.Max(this.y, v2.y));
        }

        public static float2 operator *(float a, float2 c1)
        {
            return new float2(c1.x * a, c1.y * a);
        }
    }
    public struct float3
    {
        public float x, y, z;
        public float3(float[] val)
        {
            x = val[0]; y = val[1]; z = val[2];
        }

        public static implicit operator CLCalc.Program.MemoryObject(float3 pt)
        {
            return new CLCalc.Program.Value<float3>(pt);
        }
        public float dot(float3 pt)
        {
            return (float)(this.x * pt.x + this.y * pt.y + this.z * pt.z);
        }
        public float distance(float3 pt2)
        {
            return (float)Math.Sqrt((this.x - pt2.x) * (this.x - pt2.x) + (this.y - pt2.y) * (this.y - pt2.y) + (this.z - pt2.z) * (this.z - pt2.z));
        }
        public static implicit operator OpenTK.Vector3(float3 M)
        {
            return new Vector3(M.x, M.y, M.z);
        }
        public static implicit operator float3(Vector3 M)
        {
            return new float3(M.X, M.Y, M.Z);
        }
        public float3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public static float3 operator +(float3 c1, float3 c2)
        {
            return new float3(c1.x + c2.x, c1.y + c2.y, c1.z + c2.z);
        }
        public static float3 operator -(float3 c1, float3 c2)
        {
            return new float3(c1.x - c2.x, c1.y - c2.y, c1.z - c2.z);
        }
        public static float3 operator /(float3 c1, float3 c2)
        {
            return new float3(c1.x / c2.x, c1.y / c2.y, c1.z / c2.z);
        }
        public static float3 operator *(float3 c1, float3 c2)
        {
            return new float3(c1.x * c2.x, c1.y * c2.y, c1.z * c2.z);
        }

        public static float3 operator /(float3 c1, float a)
        {
            return new float3(c1.x / a, c1.y / a, c1.z / a);
        }
        public static float3 operator *(float3 c1, float a)
        {
            return new float3(c1.x * a, c1.y * a, c1.z * a);
        }

        public static float3 operator *(float a, float3 c1)
        {
            return new float3(c1.x * a, c1.y * a, c1.z * a);
        }
    }
    public struct int4
    {
        public int x, y, z, w;
        public int4(int[] val)
        {
            x = val[0]; y = val[1]; z = val[2]; w = val[3];
        }
        public int4(int val)
        {
            this.x = this.y = this.z = this.w = val;
        }
        public int4(int x, int y, int z, int w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public static implicit operator CLCalc.Program.MemoryObject(int4 pt)
        {
            return new CLCalc.Program.Value<int4>(pt);
        }
        public int4 min(int4 v2)
        {
            return new int4(Math.Min(this.x, v2.x), Math.Min(this.y, v2.y), Math.Min(this.z, v2.z), Math.Min(this.w, v2.w));
        }
        public int4 max(int4 v2)
        {
            return new int4(Math.Max(this.x, v2.x), Math.Max(this.y, v2.y), Math.Max(this.z, v2.z), Math.Max(this.w, v2.w));
        }
        public static int4 operator +(int4 c1, int4 c2)
        {
            return new int4(c1.x + c2.x, c1.y + c2.y, c1.z + c2.z, c1.w + c2.w);
        }
        public static int4 operator -(int4 c1, int4 c2)
        {
            return new int4(c1.x - c2.x, c1.y - c2.y, c1.z - c2.z, c1.w - c2.w);
        }
        public static int4 operator /(int4 c1, int4 c2)
        {
            return new int4(c1.x / c2.x, c1.y / c2.y, c1.z / c2.z, c1.w / c2.w);
        }
        public static int4 operator *(int4 c1, int4 c2)
        {
            return new int4(c1.x * c2.x, c1.y * c2.y, c1.z * c2.z, c1.w * c2.w);
        }

        public static int4 operator /(int4 c1, int a)
        {
            return new int4(c1.x / a, c1.y / a, c1.z / a, c1.w / a);
        }
        public static int4 operator *(int4 c1, int a)
        {
            return new int4(c1.x * a, c1.y * a, c1.z * a, c1.w * a);
        }

        public static int4 operator *(int a, int4 c1)
        {
            return new int4(c1.x * a, c1.y * a, c1.z * a, c1.w * a);
        }
        public int distance(int4 pt2)
        {
            return (int)Math.Sqrt((this.x - pt2.x) * (this.x - pt2.x) + (this.y - pt2.y) * (this.y - pt2.y) + (this.z - pt2.z) * (this.z - pt2.z));
        }
        public int length()
        {
            return (int)Math.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z + this.w * this.w);
        }
        public string ToString()
        {
            return "(" + x + "," + y + "," + z + "," + w + ")";
        }
    }
    public struct int2
    {
        public int x, y;
        public int2(int[] val)
        {
            x = val[0]; y = val[1];
        }
        public int2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static implicit operator CLCalc.Program.MemoryObject(int2 pt)
        {
            return new CLCalc.Program.Value<int2>(pt);
        }
        public int distance(int2 pt2)
        {
            return (int)Math.Sqrt((this.x - pt2.x) * (this.x - pt2.x) + (this.y - pt2.y) * (this.y - pt2.y));
        }
        public static int2 operator +(int2 c1, int2 c2)
        {
            return new int2(c1.x + c2.x, c1.y + c2.y);
        }
        public static int2 operator -(int2 c1, int2 c2)
        {
            return new int2(c1.x - c2.x, c1.y - c2.y);
        }
        public static int2 operator /(int2 c1, int2 c2)
        {
            return new int2(c1.x / c2.x, c1.y / c2.y);
        }
        public static int2 operator *(int2 c1, int2 c2)
        {
            return new int2(c1.x * c2.x, c1.y * c2.y);
        }

        public static int2 operator /(int2 c1, int a)
        {
            return new int2(c1.x / a, c1.y / a);
        }
        public static int2 operator *(int2 c1, int a)
        {
            return new int2(c1.x * a, c1.y * a);
        }

        public static int2 operator *(int a, int2 c1)
        {
            return new int2(c1.x * a, c1.y * a);
        }
        public int2 min(int2 v2)
        {
            return new int2(Math.Min(this.x, v2.x), Math.Min(this.y, v2.y));
        }
        public int2 max(int2 v2)
        {
            return new int2(Math.Max(this.x, v2.x), Math.Max(this.y, v2.y));
        }
    }
    public struct int3
    {
        public int x, y, z;
        public int3(int[] val)
        {
            x = val[0]; y = val[1]; z = val[2];
        }

        public int distance(int3 pt2)
        {
            return (int)Math.Sqrt((this.x - pt2.x) * (this.x - pt2.x) + (this.y - pt2.y) * (this.y - pt2.y) + (this.z - pt2.z) * (this.z - pt2.z));
        }
        public int3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public static int3 operator +(int3 c1, int3 c2)
        {
            return new int3(c1.x + c2.x, c1.y + c2.y, c1.z + c2.z);
        }
        public static int3 operator -(int3 c1, int3 c2)
        {
            return new int3(c1.x - c2.x, c1.y - c2.y, c1.z - c2.z);
        }
        public static int3 operator /(int3 c1, int3 c2)
        {
            return new int3(c1.x / c2.x, c1.y / c2.y, c1.z / c2.z);
        }
        public static int3 operator *(int3 c1, int3 c2)
        {
            return new int3(c1.x * c2.x, c1.y * c2.y, c1.z * c2.z);
        }

        public static int3 operator /(int3 c1, int a)
        {
            return new int3(c1.x / a, c1.y / a, c1.z / a);
        }
        public static int3 operator *(int3 c1, int a)
        {
            return new int3(c1.x * a, c1.y * a, c1.z * a);
        }

        public static int3 operator *(int a, int3 c1)
        {
            return new int3(c1.x * a, c1.y * a, c1.z * a);
        }
    }

    public struct Camera
    {
        public float3 orig, target;
        public float3 dir, x, y;
    }

    public struct RenderingConfig
    {
        public float4 fgColor;
        public float4 bgColor;
        public float4 backFaceColor;
        public float4 light;
        public float zNear;
        public float zFar;
        public float focalX;
        public float focalY;
        public Int32 sensorWidth;
        public Int32 sensorHeight;
        public Int32 colorMode;
        public Camera camera;
    }
    public struct Matrix4f
    {

        public readonly static Matrix4f Identity = new Matrix4f(new float[]{
            1,0,0,0,
            0,1,0,0,
            0,0,1,0,
            0,0,0,1
        });
        public readonly static Matrix4f Zero = new Matrix4f(new float[]{
            0,0,0,0,
            0,0,0,0,
            0,0,0,0,
            0,0,0,0
        });

        public static implicit operator CLCalc.Program.MemoryObject(Matrix4f pt)
        {
            return new CLCalc.Program.Value<Matrix4f>(pt);
        }
        public float m00, m01, m02, m03,
              m10, m11, m12, m13,
              m20, m21, m22, m23,
              m30, m31, m32, m33;
        public Matrix4f(float[] M)
        {
            m00 = M[0];
            m01 = M[1];
            m02 = M[2];
            m03 = M[3];
            m10 = M[4];
            m11 = M[5];
            m12 = M[6];
            m13 = M[7];
            m20 = M[8];
            m21 = M[9];
            m22 = M[10];
            m23 = M[11];
            m30 = M[12];
            m31 = M[13];
            m32 = M[14];
            m33 = M[15];

        }
        public static Matrix4f operator *(Matrix4f M2, float scale)
        {
            Matrix4f M = new Matrix4f();
            M.m00 = M2.m00 * scale; M.m01 = M2.m01 * scale; M.m02 = M2.m02 * scale; M.m03 = M2.m03 * scale;
            M.m10 = M2.m10 * scale; M.m11 = M2.m11 * scale; M.m12 = M2.m12 * scale; M.m13 = M2.m13 * scale;
            M.m20 = M2.m20 * scale; M.m21 = M2.m21 * scale; M.m22 = M2.m22 * scale; M.m23 = M2.m23 * scale;
            M.m30 = M2.m30 * scale; M.m31 = M2.m31 * scale; M.m32 = M2.m32 * scale; M.m33 = M2.m33 * scale;
            return M;
        }
        public static float4 operator *(Matrix4f M, float4 v)
        {
            float4 p = new float4();
            p.x = M.m00 * v.x + M.m01 * v.y + M.m02 * v.z + M.m03 * v.w;
            p.y = M.m10 * v.x + M.m11 * v.y + M.m12 * v.z + M.m13 * v.w;
            p.z = M.m20 * v.x + M.m21 * v.y + M.m22 * v.z + M.m23 * v.w;
            p.w = M.m30 * v.x + M.m31 * v.y + M.m32 * v.z + M.m33 * v.w;
            return p;
        }
        public static Matrix4f operator *(Matrix4f M1, Matrix4f M2)
        {
            Matrix4f M = new Matrix4f();
            float4[] c = new float4[4];
            float4[] r = new float4[4];
            c[0] = new float4(M2.m00, M2.m10, M2.m20, M2.m30);
            c[1] = new float4(M2.m01, M2.m11, M2.m21, M2.m31);
            c[2] = new float4(M2.m02, M2.m12, M2.m22, M2.m32);
            c[3] = new float4(M2.m03, M2.m13, M2.m23, M2.m33);

            r[0] = new float4(M1.m00, M1.m01, M1.m02, M1.m03);
            r[1] = new float4(M1.m10, M1.m11, M1.m12, M1.m13);
            r[2] = new float4(M1.m20, M1.m21, M1.m22, M1.m23);
            r[3] = new float4(M1.m30, M1.m31, M1.m32, M1.m33);

            M.m00 = r[0].dot(c[0]);
            M.m00 = r[0].dot(c[0]);
            M.m01 = r[0].dot(c[1]);
            M.m02 = r[0].dot(c[2]);
            M.m03 = r[0].dot(c[3]);
            M.m10 = r[1].dot(c[0]);
            M.m11 = r[1].dot(c[1]);
            M.m12 = r[1].dot(c[2]);
            M.m13 = r[1].dot(c[3]);
            M.m20 = r[2].dot(c[0]);
            M.m21 = r[2].dot(c[1]);
            M.m22 = r[2].dot(c[2]);
            M.m23 = r[2].dot(c[3]);
            M.m30 = r[3].dot(c[0]);
            M.m31 = r[3].dot(c[1]);
            M.m32 = r[3].dot(c[2]);
            M.m33 = r[3].dot(c[3]);

            return M;
        }
        public Matrix4f Transpose()
        {
            Matrix4f modelView = new Matrix4f();
            modelView.m00 = this.m00;
            modelView.m01 = this.m10;
            modelView.m02 = this.m20;
            modelView.m03 = this.m30;
            modelView.m10 = this.m01;
            modelView.m11 = this.m11;
            modelView.m12 = this.m21;
            modelView.m13 = this.m31;
            modelView.m20 = this.m02;
            modelView.m21 = this.m12;
            modelView.m22 = this.m22;
            modelView.m23 = this.m32;
            modelView.m30 = this.m03;
            modelView.m31 = this.m13;
            modelView.m32 = this.m23;
            modelView.m33 = this.m33;
            return modelView;
        }
        public static Matrix4f operator /(Matrix4f M, float scale)
        {
            M.m00 /= scale; M.m01 /= scale; M.m02 /= scale; M.m03 /= scale;
            M.m10 /= scale; M.m11 /= scale; M.m12 /= scale; M.m13 /= scale;
            M.m20 /= scale; M.m21 /= scale; M.m22 /= scale; M.m23 /= scale;
            M.m30 /= scale; M.m31 /= scale; M.m32 /= scale; M.m33 /= scale;
            return M;
        }

        public static implicit operator OpenTK.Matrix4(Matrix4f M)
        {
            OpenTK.Matrix4 m = new OpenTK.Matrix4();
            m.M11 = M.m00;
            m.M12 = M.m01;
            m.M13 = M.m02;
            m.M14 = M.m03;
            m.M21 = M.m10;
            m.M22 = M.m11;
            m.M23 = M.m12;
            m.M24 = M.m13;
            m.M31 = M.m20;
            m.M32 = M.m21;
            m.M33 = M.m22;
            m.M34 = M.m23;
            m.M41 = M.m30;
            m.M42 = M.m31;
            m.M43 = M.m32;
            m.M44 = M.m33;
            return m;
        }
        public static implicit operator Matrix4f(OpenTK.Matrix4 m)
        {
            Matrix4f M = new Matrix4f();
            M.m00 = m.M11;
            M.m10 = m.M21;
            M.m20 = m.M31;
            M.m30 = m.M41;
            M.m01 = m.M12;
            M.m11 = m.M22;
            M.m21 = m.M32;
            M.m31 = m.M42;
            M.m02 = m.M13;
            M.m12 = m.M23;
            M.m22 = m.M33;
            M.m32 = m.M43;
            M.m03 = m.M14;
            M.m13 = m.M24;
            M.m23 = m.M34;
            M.m33 = m.M44;
            return M;
        }

        public Matrix4f(float val)
        {
            m00 = val;
            m01 = val;
            m02 = val;
            m03 = val;
            m10 = val;
            m11 = val;
            m12 = val;
            m13 = val;
            m20 = val;
            m21 = val;
            m22 = val;
            m23 = val;
            m30 = val;
            m31 = val;
            m32 = val;
            m33 = val;
        }

    }
    public struct BoundingBox
    {
        public float4 minPoint;
        public float4 maxPoint;
        public float4 center;
        public BoundingBox(float4 minPoint, float4 maxPoint, float4 center)
        {
            this.minPoint = minPoint;
            this.maxPoint = maxPoint;
            this.center = center;
        }
        public BoundingBox(float4 minPoint, float4 maxPoint)
        {
            this.minPoint = minPoint;
            this.maxPoint = maxPoint;
            this.center = new float4(0.5f * (minPoint.x + maxPoint.x), 0.5f * (minPoint.y + maxPoint.y), 0.5f * (minPoint.z + maxPoint.z), 1.0f);
        }

        public static implicit operator CLCalc.Program.MemoryObject(BoundingBox pt)
        {
            return new CLCalc.Program.Value<BoundingBox>(pt);
        }
        public BoundingBox(float val)
        {
            this.minPoint = new float4(val);
            this.maxPoint = new float4(val);
            this.center = new float4(val);
        }
        public bool contains(Vector3 pt)
        {
            return (
                pt.X >= minPoint.x &&
                pt.Y >= minPoint.y &&
                pt.Z >= minPoint.z &&
                pt.X <= maxPoint.x &&
                pt.Y <= maxPoint.y &&
                pt.Z <= maxPoint.z);
        }
    }
    public static class Utilities
    {
        public static Matrix4f Transpose(OpenTK.Matrix4 m)
        {
            Matrix4f modelView = new Matrix4f();
            modelView.m00 = m.M11;
            modelView.m01 = m.M21;
            modelView.m02 = m.M31;
            modelView.m03 = m.M41;
            modelView.m10 = m.M12;
            modelView.m11 = m.M22;
            modelView.m12 = m.M32;
            modelView.m13 = m.M42;
            modelView.m20 = m.M13;
            modelView.m21 = m.M23;
            modelView.m22 = m.M33;
            modelView.m23 = m.M43;
            modelView.m30 = m.M14;
            modelView.m31 = m.M24;
            modelView.m32 = m.M34;
            modelView.m33 = m.M44;
            return modelView;
        }
        public static int CreateTexture<T>(Bitmap image, out CLCalc.Program.Image2D CLTexture2D) where T : struct
        {

            int textureId = -1;
            GL.Enable(EnableCap.Texture2D);

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            GL.GenTextures(1, out textureId);

            if (textureId == -1)
            {
                CLTexture2D = null;
                return textureId;
            }
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, image.Width, image.Height);
            System.Drawing.Imaging.BitmapData bitmapdata = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height,
                0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, bitmapdata.Scan0);
            image.UnlockBits(bitmapdata);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            CLTexture2D = new CLCalc.Program.Image2D(textureId);

            GL.Disable(EnableCap.Texture2D);
            return textureId;
        }
        public static int CreateTexture<T>(out CLCalc.Program.Image2D CLTexture2D, int width, int height) where T : struct
        {

            int textureId = -1;
            GL.Enable(EnableCap.Texture2D);

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            GL.GenTextures(1, out textureId);

            if (textureId == -1)
            {
                CLTexture2D = null;
                return textureId;
            }
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            Bitmap image = new Bitmap(width, height);
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, image.Width, image.Height);
            System.Drawing.Imaging.BitmapData bitmapdata = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height,
                0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, bitmapdata.Scan0);
            image.UnlockBits(bitmapdata);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            CLTexture2D = new CLCalc.Program.Image2D(textureId);

            GL.Disable(EnableCap.Texture2D);
            return textureId;
        }
    }
}
