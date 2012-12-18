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
using System.Numerics;
using OpenTK;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using System.IO;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using MathNet.Numerics.LinearAlgebra.Single;
using MathNet.Numerics.LinearAlgebra.Generic.Factorization;
using MathNet.Numerics.LinearAlgebra.Generic;
namespace Perceptual.Foundation
{
    [Serializable()]
    public struct float4
    {
        public float x, y, z, w;
        public float4(float[] val)
        {
            x = val[0]; y = val[1]; z = val[2]; w = val[3];
        }
        public float4(Vector3 v)
        {
            this.x = v.X;
            this.y = v.Y;
            this.z = v.Z;
            this.w = 1.0f;
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
        public float4 cross(float4 v1)
        {
            float4 cross = new float4();
            cross.x = this.y * v1.z - this.z * v1.y;
            cross.y = this.z * v1.x - this.x * v1.z;
            cross.z = this.x * v1.y - this.y * v1.x;
            return cross;
        }
        public override string ToString()
        {
            return "(" + x + "," + y + "," + z + "," + w + ")";
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

        public static implicit operator Vector4(float4 M)
        {
            return new Vector4(M.x, M.y, M.z, M.w);
        }
        public static implicit operator Color4(float4 M)
        {
            return new Color4(M.x, M.y, M.z, M.w);
        }
        public static implicit operator float4(Color4 M)
        {
            return new float4(M.R,M.G,M.B,M.A);
        }
        public static implicit operator Vector3(float4 M)
        {
            return new Vector3(M.x, M.y, M.z);
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
    }
    public struct float2
    {
        public float x, y;
        public float2(float[] val)
        {
            x = val[0]; y = val[1];
        }
        public override string ToString()
        {
            return "(" + x + "," + y + ")";
        }
        public float2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public static implicit operator Vector2(float2 M)
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
        public float3(float val)
        {
            x = val; y = val; z = val;
        }
        public override string ToString()
        {
            return "(" + x + "," + y + "," + z + ")";
        }
        public float length()
        {
            return (float)Math.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z);
        }
        public float3(float4 pt)
        {
            this.x = pt.x;
            this.y = pt.y;
            this.z = pt.z;
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
        public static implicit operator Vector3(float3 M)
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
        public static float3 operator +(float3 c1, float c2)
        {
            return new float3(c1.x + c2, c1.y + c2, c1.z + c2);
        }
        public static float3 operator -(float3 c1, float c2)
        {
            return new float3(c1.x - c2, c1.y - c2, c1.z - c2);
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
        public int length()
        {
            return Math.Abs(x) + Math.Abs(y) + Math.Abs(z) + Math.Abs(w);
        }
        public override string ToString()
        {
            return "(" + x + "," + y + "," + z + "," + w + ")";
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
        public static bool operator ==(int4 c1, int4 c2)
        {
            return (c1.x == c2.x && c1.y == c2.y && c1.z == c2.z && c1.w == c2.w);
        }
        public static bool operator !=(int4 c1, int4 c2)
        {
            return (c1.x != c2.x || c1.y != c2.y || c1.z != c2.z || c1.w != c2.w);
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

    }
    public struct int2
    {
        public int x, y;
        public int2(int[] val)
        {
            x = val[0]; y = val[1];
        }
        public override string ToString()
        {
            return "(" + x + "," + y + ")";
        }
        public int2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int length()
        {
            return Math.Abs(x) + Math.Abs(y);
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
        public static bool operator ==(int2 c1, int2 c2)
        {
            return (c1.x == c2.x && c1.y == c2.y);
        }
        public static bool operator !=(int2 c1, int2 c2)
        {
            return (c1.x != c2.x || c1.y != c2.y);
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
        public override string ToString()
        {
            return "(" + x + "," + y + "," + z + ")";
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
        public static bool operator ==(int3 c1, int3 c2)
        {
            return (c1.x == c2.x && c1.y == c2.y && c1.z == c2.z);
        }
        public static bool operator !=(int3 c1, int3 c2)
        {
            return (c1.x != c2.x || c1.y != c2.y || c1.z != c2.z);
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

        public static implicit operator Matrix4(Matrix4f M)
        {
            Matrix4 m = new Matrix4();
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
        public override string ToString()
        {
            return "\n" + m00 + " " +
                   m01 + " " +
                   m02 + " " +
                   m03 + "\n" +
                   m10 + " " +
                   m11 + " " +
                   m12 + " " +
                   m13 + " " +
                   m20 + "\n" +
                   m21 + " " +
                   m22 + " " +
                   m23 + " " +
                   m30 + " " +
                   m31 + " " +
                   m32 + " " +
                   m33 + "\n";
        }
        public static implicit operator Matrix4f(Matrix4 m)
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
        public override string ToString()
        {
            return "[" + minPoint + "," + maxPoint + "] : " + center;
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
        public static Matrix4f Transpose(Matrix4 m)
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



        public static Quaternion ToQuaternion(Matrix4 m)
        {

            Quaternion q = new Quaternion();
            float val = m.M11;
            if (m.M11 + m.M22 + m.M33 > 0.0f)
            {
                float t = +m.M11 + m.M22 + m.M33 + 1.0f;
                float s = (float)(0.5f / Math.Sqrt(t));
                q.W = s * t;
                q.Z = (m.M21 - m.M12) * s;
                q.Y = (m.M13 - m.M31) * s;
                q.X = (m.M32 - m.M23) * s;
            }
            else if (m.M11 > m.M22 && m.M11 > m.M33)
            {
                float t = +m.M11 - m.M22 - m.M33 + 1.0f;
                float s = (float)(0.5f / Math.Sqrt(t));
                q.X = s * t;
                q.Y = (m.M21 + m.M12) * s;
                q.Z = (m.M13 + m.M31) * s;
                q.W = (m.M32 - m.M23) * s;
            }
            else if (m.M22 > m.M33)
            {
                float t = -m.M11 + m.M22 - m.M33 + 1.0f;
                float s = (float)(0.5f / Math.Sqrt(t));
                q.Y = s * t;
                q.X = (m.M21 + m.M12) * s;
                q.W = (m.M13 - m.M31) * s;
                q.Z = (m.M32 + m.M23) * s;
            }
            else
            {
                float t = -m.M11 - m.M22 + m.M33 + 1.0f;
                float s = (float)(0.5f / Math.Sqrt(t));
                q.Z = s * t;
                q.W = (m.M21 - m.M12) * s;
                q.X = (m.M13 + m.M31) * s;
                q.Y = (m.M32 + m.M23) * s;
            }
            q.Conjugate();
            return q;
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
        public static Matrix4 RigidMotion(float4[] source, float4[] target)
        {
            Vector3 mean1 = new Vector3();
            Vector3 mean2 = new Vector3();
            for (int i = 0; i < source.Length; i++)
            {
                float4 pt1 = source[i];
                float4 pt2 = target[i];
                mean1.X += pt1.x;
                mean1.Y += pt1.y;
                mean1.Z += pt1.z;
                mean2.X += pt2.x;
                mean2.Y += pt2.y;
                mean2.Z += pt2.z;
            }
            mean1 *= (1.0f / source.Length);
            mean2 *= (1.0f / source.Length);
            Matrix4f S = new Matrix4f(0);

            for (int i = 0; i < source.Length; i++)
            {
                float4 pt1 = source[i];
                float4 pt2 = target[i];
                S.m00 += (pt1.x - mean1.X) * (pt2.x - mean2.X); S.m01 += (pt1.x - mean1.X) * (pt2.y - mean2.Y); S.m02 += (pt1.x - mean1.X) * (pt2.z - mean2.Z);
                S.m10 += (pt1.y - mean1.Y) * (pt2.x - mean2.X); S.m11 += (pt1.y - mean1.Y) * (pt2.y - mean2.Y); S.m12 += (pt1.y - mean1.Y) * (pt2.z - mean2.Z);
                S.m20 += (pt1.z - mean1.Z) * (pt2.x - mean2.X); S.m21 += (pt1.z - mean1.Z) * (pt2.y - mean2.Y); S.m22 += (pt1.z - mean1.Z) * (pt2.z - mean2.Z);
            }
            S = S / (float)source.Length;
            DenseMatrix Q = new DenseMatrix(4, 4);
            float tr = S.m00 + S.m11 + S.m22;
            float4 delta = new float4(S.m12 - S.m21, S.m20 - S.m02, S.m01 - S.m10, 0);
            Q[0, 0] = tr; Q[0, 1] = delta.x; Q[0, 2] = delta.y; Q[0, 3] = delta.z;
            Q[1, 0] = delta.x; Q[1, 1] = S.m00 + S.m00 - tr; Q[1, 2] = S.m01 + S.m10; Q[1, 3] = S.m02 + S.m20;
            Q[2, 0] = delta.y; Q[2, 1] = S.m10 + S.m01; Q[2, 2] = S.m11 + S.m11 - tr; Q[2, 3] = S.m12 + S.m21;
            Q[3, 0] = delta.z; Q[3, 1] = S.m20 + S.m02; Q[3, 2] = S.m21 + S.m12; Q[3, 3] = S.m22 + S.m22 - tr;
            Evd<float> evd = Q.Evd();
            double maxEigenValue = -1.0E10f;
            Quaternion qr = new Quaternion();

            for (int i = 0; i < 4; i++)
            {
                Complex ev = evd.EigenValues()[i];
                if (ev.Real > maxEigenValue)
                {
                    qr.W = evd.EigenVectors()[0, i];
                    qr.X = evd.EigenVectors()[1, i];
                    qr.Y = evd.EigenVectors()[2, i];
                    qr.Z = evd.EigenVectors()[3, i];
                    maxEigenValue = ev.Real;
                }
            }
            Matrix4 R = Matrix4.Rotate(qr);
            if (float.IsNaN(R.M11)) R = Matrix4.Identity;
            Vector3 T = mean2 - Vector3.Transform(mean1, R);
            Matrix4 MT = Matrix4.CreateTranslation(T.X, T.Y, T.Z);
            MT = (R * MT);
            return MT;
        }
        public static void WriteRawImage2D(CLCalc.Program.Image2D imageBuffer, string path)
        {
            System.Console.WriteLine("SAVING IMAGE " + path);
            int sz = imageBuffer.Width * imageBuffer.Height;
            float[] data = new float[sz * 4];
            float[] data2 = new float[sz * 4];
            imageBuffer.ReadFromDeviceTo(data);
            for (int i = 0; i < data.Length; i++)
            {
                int off = i / 4;
                int img = i % 4;
                data2[off + img * sz] = data[i];
            }

            using (Stream dest = File.Create(path))
            {

                foreach (float val in data2)
                {
                    byte[] buffer = System.BitConverter.GetBytes(val);
                    dest.Write(buffer, 0, buffer.Length);
                }
                dest.Close();
            }
        }

        public static CLCalc.Program.MemoryObject ToMemoryObject(float pt)
        {
            return new CLCalc.Program.Value<float>(pt);
        }

        public static CLCalc.Program.MemoryObject ToMemoryObject(int pt)
        {
            return new CLCalc.Program.Value<int>(pt);
        }

        public static CLCalc.Program.MemoryObject ToMemoryObject(char pt)
        {
            return new CLCalc.Program.Value<char>(pt);
        }

        public static CLCalc.Program.MemoryObject ToMemoryObject(byte pt)
        {
            return new CLCalc.Program.Value<byte>(pt);
        }

        public static CLCalc.Program.MemoryObject ToMemoryObject(long pt)
        {
            return new CLCalc.Program.Value<long>(pt);
        }
        public static FileStream WriteAsyncRawImage2D(CLCalc.Program.Image2D imageBuffer, string path)
        {
            System.Console.WriteLine("ASYNC SAVING IMAGE " + path);
            FileStream dest = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 1, true);
            int sz = imageBuffer.Width * imageBuffer.Height;
            float[] data = new float[sz * 4];
            float[] data2 = new float[sz * 4];
            imageBuffer.ReadFromDeviceTo(data);
            for (int i = 0; i < data.Length; i++)
            {
                int off = i / 4;
                int img = i % 4;
                data2[off + img * sz] = data[i];
            }

            byte[] bytes = new byte[4 * data2.Length];
            int index = 0;
            foreach (float val in data2)
            {
                byte[] buffer = System.BitConverter.GetBytes(val);
                bytes[index++] = buffer[0];
                bytes[index++] = buffer[1];
                bytes[index++] = buffer[2];
                bytes[index++] = buffer[3];
            }
            dest.BeginWrite(bytes, 0, bytes.Length, null, null);
            return dest;
        }
        public static void WriteByteDataToFile(CLCalc.Program.Variable dataBuffer, int width, int height, int comp, string path)
        {

            System.Console.WriteLine("SAVING IMAGE " + path);
            int sz = width * height * comp;
            byte[] data = new byte[sz];
            dataBuffer.ReadFromDeviceTo(data);
            int[] data2 = new int[sz];
            for (int i = 0; i < data.Length; i++)
            {
                int off = i / comp;
                int img = i % comp;
                data2[off + img * sz] = data[i];
            }
            using (Stream dest = File.Create(path))
            {
                foreach (byte val in data2)
                {
                    dest.WriteByte(val);
                }
                dest.Close();
            }
        }
        public static void WriteIntDataToFile(CLCalc.Program.Variable dataBuffer, int width, int height, int comp, string path)
        {

            System.Console.WriteLine("SAVING IMAGE " + path);
            int sz = width * height * comp;
            int[] data = new int[sz];
            dataBuffer.ReadFromDeviceTo(data);
            int[] data2 = new int[sz];
            for (int i = 0; i < data.Length; i++)
            {
                int off = i / comp;
                int img = i % comp;
                data2[off + img * sz] = data[i];
            }
            using (Stream dest = File.Create(path))
            {

                foreach (int val in data2)
                {
                    byte[] buffer = System.BitConverter.GetBytes(val);
                    dest.Write(buffer, 0, buffer.Length);
                }
                dest.Close();
            }
        }
        public static void WriteIntDataToFile(CLCalc.Program.Variable dataBuffer, int rows, int cols, int slices, int comp, string path)
        {

            System.Console.WriteLine("SAVING IMAGE " + path);
            int sz = rows * cols * slices;
            int[] data = new int[sz * comp];
            int[] data2 = new int[sz * comp];
            dataBuffer.ReadFromDeviceTo(data);
            for (int i = 0; i < data.Length; i++)
            {
                int off = i / comp;
                int img = i % comp;
                data2[off + img * sz] = data[i];
            }
            using (Stream dest = File.Create(path))
            {

                foreach (int val in data2)
                {
                    byte[] buffer = System.BitConverter.GetBytes(val);
                    dest.Write(buffer, 0, buffer.Length);
                }
                dest.Close();
            }
        }
        public static void WriteIntDataToFile(CLCalc.Program.Variable dataBuffer, int rows, int cols, int slices, int comp, int offset, string path)
        {

            System.Console.WriteLine("SAVING IMAGE " + path);
            int sz = rows * cols * slices;
            int[] data = new int[sz * comp + offset];
            int[] data2 = new int[sz * comp];
            dataBuffer.ReadFromDeviceTo(data);
            for (int i = offset; i < data.Length; i++)
            {
                int off = (i - offset) / comp;
                int img = (i - offset) % comp;
                data2[off + img * sz] = data[i];
            }
            using (Stream dest = File.Create(path))
            {

                for (int i = 0; i < data2.Length; i++)
                {
                    int val = data2[i];
                    byte[] buffer = System.BitConverter.GetBytes(val);
                    dest.Write(buffer, 0, buffer.Length);
                }
                dest.Close();
            }
        }
        public static void WriteFloatDataToFile(float[] data, int rows, int cols, int slices, int comp, string path)
        {

            System.Console.WriteLine("SAVING IMAGE " + path);
            int sz = rows * cols * slices;
            float[] data2 = new float[sz * comp];
            for (int i = 0; i < data.Length; i++)
            {
                int off = i / comp;
                int img = i % comp;
                data2[off + img * sz] = data[i];
            }
            using (Stream dest = File.Create(path))
            {

                foreach (float val in data2)
                {
                    byte[] buffer = System.BitConverter.GetBytes(val);
                    dest.Write(buffer, 0, buffer.Length);
                }
                dest.Close();
            }
        }
        public static void WriteFloatDataToFile(CLCalc.Program.Variable dataBuffer, int rows, int cols, int slices, int comp, string path)
        {

            System.Console.WriteLine("SAVING IMAGE " + path);
            int sz = rows * cols * slices;
            float[] data = new float[sz * comp];
            float[] data2 = new float[sz * comp];
            dataBuffer.ReadFromDeviceTo(data);
            for (int i = 0; i < data.Length; i++)
            {
                int off = i / comp;
                int img = i % comp;
                data2[off + img * sz] = data[i];
            }
            using (Stream dest = File.Create(path))
            {

                foreach (float val in data2)
                {
                    byte[] buffer = System.BitConverter.GetBytes(val);
                    dest.Write(buffer, 0, buffer.Length);
                }
                dest.Close();
            }
        }

        public static void WriteFloatDataToFile(float[] data, int width, int height, int comp, string path)
        {

            System.Console.WriteLine("SAVING DATA " + path);
            int sz = width * height * comp;
            float[] data2 = new float[sz];
            for (int i = 0; i < data.Length; i++)
            {
                int off = i / comp;
                int img = i % comp;
                data2[off + img * sz] = data[i];
            }
            using (Stream dest = File.Create(path))
            {

                foreach (float val in data2)
                {
                    byte[] buffer = System.BitConverter.GetBytes(val);
                    dest.Write(buffer, 0, buffer.Length);
                }
                dest.Close();
            }
        }
        public static Matrix4 ReadMatrixFromFile(string file)
        {

            float[] data = new float[16];
            using (Stream stream = File.OpenRead(file))
            {
                byte[] buffer = new byte[4];
                for (int i = 0; i < data.Length; i++)
                {
                    stream.Read(buffer, 0, buffer.Length);
                    data[i] = System.BitConverter.ToSingle(buffer, 0);
                }
                stream.Close();
            }
            Matrix4 CurrentPose = new Matrix4();
            CurrentPose.M11 = data[0]; CurrentPose.M12 = data[1]; CurrentPose.M13 = data[2]; CurrentPose.M14 = data[3];
            CurrentPose.M21 = data[4]; CurrentPose.M22 = data[5]; CurrentPose.M23 = data[6]; CurrentPose.M24 = data[7];
            CurrentPose.M31 = data[8]; CurrentPose.M32 = data[9]; CurrentPose.M33 = data[10]; CurrentPose.M34 = data[11];
            CurrentPose.M41 = data[12]; CurrentPose.M42 = data[13]; CurrentPose.M43 = data[14]; CurrentPose.M44 = data[15];
            return CurrentPose;
        }
        public static float[] ReadFloatFromFile(string file, int size)
        {

            float[] data = new float[size];
            using (Stream stream = File.OpenRead(file))
            {
                byte[] buffer = new byte[4];
                for (int i = 0; i < size; i++)
                {
                    stream.Read(buffer, 0, buffer.Length);
                    data[i] = System.BitConverter.ToSingle(buffer, 0);
                }
                stream.Close();
            }
            float[] data2 = new float[size];
            for (int i = 0; i < data.Length; i++)
            {
                int off = i / 4;
                int img = i % 4;
                data2[i] = data[off + img * (size / 4)];
            }
            return data2;
        }
        public static int[] ReadIntFromFile(string file, int size)
        {

            int[] data = new int[size];
            using (Stream stream = File.OpenRead(file))
            {
                byte[] buffer = new byte[4];
                for (int i = 0; i < size; i++)
                {
                    stream.Read(buffer, 0, buffer.Length);
                    data[i] = System.BitConverter.ToInt32(buffer, 0);
                }
                stream.Close();
            }
            return data;
        }
        public static byte[] ReadByteFromFile(string file, int size)
        {

            byte[] data = new byte[size];
            using (Stream stream = File.OpenRead(file))
            {

                stream.Read(data, 0, size);
                stream.Close();
            }
            return data;
        }
        public static void WriteFloatDataToFile(CLCalc.Program.Variable dataBuffer, int width, int height, int comp, string path)
        {

            System.Console.WriteLine("SAVING IMAGE " + path);
            int sz = width * height * comp;
            float[] data = new float[sz];
            dataBuffer.ReadFromDeviceTo(data);
            float[] data2 = new float[sz];
            for (int i = 0; i < data.Length; i++)
            {
                int off = i / comp;
                int img = i % comp;
                data2[off + img * sz] = data[i];
            }
            using (Stream dest = File.Create(path))
            {

                foreach (float val in data2)
                {
                    byte[] buffer = System.BitConverter.GetBytes(val);
                    dest.Write(buffer, 0, buffer.Length);
                }
                dest.Close();
            }
        }
        #region Look-up table for marching cubes
        public static int[] a2iTriangleConnectionTable = new int[]{
-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
8,0,3,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
9,1,0,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
3,8,9,9,1,3,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
10,2,1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
0,1,10,10,8,0,10,2,3,3,8,10,-1,-1,-1,-1,
0,9,10,10,2,0,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
8,9,10,10,2,8,2,3,8,-1,-1,-1,-1,-1,-1,-1,
11,3,2,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
2,11,8,8,0,2,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
3,0,9,9,11,3,9,1,2,2,11,9,-1,-1,-1,-1,
11,8,9,9,1,11,1,2,11,-1,-1,-1,-1,-1,-1,-1,
1,10,11,11,3,1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
10,11,8,8,0,10,0,1,10,-1,-1,-1,-1,-1,-1,-1,
9,10,11,11,3,9,3,0,9,-1,-1,-1,-1,-1,-1,-1,
11,8,9,9,10,11,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
7,4,8,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
0,3,7,7,4,0,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
4,9,1,1,7,4,1,0,8,8,7,1,-1,-1,-1,-1,
1,3,7,7,4,1,4,9,1,-1,-1,-1,-1,-1,-1,-1,
10,7,4,10,4,1,1,4,8,1,8,2,2,8,7,2,
4,0,1,4,1,10,4,10,7,7,10,2,7,2,3,-1,
2,0,8,2,8,7,2,7,10,10,7,4,10,4,9,-1,
10,2,3,4,9,10,3,7,4,4,10,3,-1,-1,-1,-1,
8,3,2,2,4,8,2,11,7,7,4,2,-1,-1,-1,-1,
4,0,2,2,11,4,11,7,4,-1,-1,-1,-1,-1,-1,-1,
3,0,8,11,7,4,11,4,9,11,9,2,1,2,9,-1,
11,1,2,7,9,1,7,4,9,1,11,7,-1,-1,-1,-1,
10,11,7,10,7,4,10,4,1,1,4,8,1,8,3,-1,
7,4,0,10,11,7,0,1,10,7,0,10,-1,-1,-1,-1,
8,3,0,10,11,7,4,9,10,10,7,4,-1,-1,-1,-1,
9,10,11,11,4,9,11,7,4,-1,-1,-1,-1,-1,-1,-1,
9,4,5,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
8,4,5,5,3,8,5,9,0,0,3,5,-1,-1,-1,-1,
4,5,1,1,0,4,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
5,1,3,3,8,5,8,4,5,-1,-1,-1,-1,-1,-1,-1,
1,9,4,4,2,1,4,5,10,10,2,4,-1,-1,-1,-1,
0,1,9,8,4,5,8,5,10,8,10,3,2,3,10,-1,
2,0,4,4,5,2,5,10,2,-1,-1,-1,-1,-1,-1,-1,
5,8,4,10,3,8,10,2,3,8,5,10,-1,-1,-1,-1,
11,4,5,11,5,2,2,5,9,2,9,3,3,9,4,3,
11,8,4,11,4,5,11,5,2,2,5,9,2,9,0,-1,
5,1,2,5,2,11,5,11,4,4,11,3,4,3,0,-1,
4,5,1,11,8,4,1,2,11,4,1,11,-1,-1,-1,-1,
3,1,9,3,9,4,3,4,11,11,4,5,11,5,10,-1,
9,0,1,11,8,4,5,10,11,11,4,5,-1,-1,-1,-1,
11,3,0,5,10,11,0,4,5,5,11,0,-1,-1,-1,-1,
10,11,8,8,5,10,8,4,5,-1,-1,-1,-1,-1,-1,-1,
5,9,8,8,7,5,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
3,7,5,5,9,3,9,0,3,-1,-1,-1,-1,-1,-1,-1,
7,5,1,1,0,7,0,8,7,-1,-1,-1,-1,-1,-1,-1,
5,1,3,3,7,5,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
7,5,10,7,10,2,7,2,8,8,2,1,8,1,9,-1,
1,9,0,7,5,10,2,3,7,7,10,2,-1,-1,-1,-1,
10,2,0,7,5,10,0,8,7,10,0,7,-1,-1,-1,-1,
3,7,5,5,2,3,5,10,2,-1,-1,-1,-1,-1,-1,-1,
9,8,3,9,3,2,9,2,5,5,2,11,5,11,7,-1,
2,11,7,9,0,2,7,5,9,9,2,7,-1,-1,-1,-1,
3,0,8,5,1,2,11,7,5,5,2,11,-1,-1,-1,-1,
7,5,1,1,11,7,1,2,11,-1,-1,-1,-1,-1,-1,-1,
7,5,10,10,11,7,1,9,8,8,3,1,-1,-1,-1,-1,
1,9,0,7,5,10,10,11,7,-1,-1,-1,-1,-1,-1,-1,
8,3,0,10,11,7,7,5,10,-1,-1,-1,-1,-1,-1,-1,
10,11,7,7,5,10,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
6,10,5,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
8,5,6,8,6,3,3,6,10,3,10,0,0,10,5,0,
9,5,6,6,0,9,6,10,1,1,0,6,-1,-1,-1,-1,
8,9,5,8,5,6,8,6,3,3,6,10,3,10,1,-1,
2,1,5,5,6,2,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
6,2,3,6,3,8,6,8,5,5,8,0,5,0,1,-1,
6,2,0,0,9,6,9,5,6,-1,-1,-1,-1,-1,-1,-1,
3,8,9,6,2,3,9,5,6,3,9,6,-1,-1,-1,-1,
2,10,5,5,3,2,5,6,11,11,3,5,-1,-1,-1,-1,
0,2,10,0,10,5,0,5,8,8,5,6,8,6,11,-1,
1,2,10,9,5,6,9,6,11,9,11,0,3,0,11,-1,
10,1,2,8,9,5,6,11,8,8,5,6,-1,-1,-1,-1,
3,1,5,5,6,3,6,11,3,-1,-1,-1,-1,-1,-1,-1,
8,0,1,6,11,8,1,5,6,6,8,1,-1,-1,-1,-1,
9,3,0,5,11,3,5,6,11,3,9,5,-1,-1,-1,-1,
11,8,9,9,6,11,9,5,6,-1,-1,-1,-1,-1,-1,-1,
7,6,10,10,8,7,10,5,4,4,8,10,-1,-1,-1,-1,
3,7,6,3,6,10,3,10,0,0,10,5,0,5,4,-1,
9,5,4,0,8,7,0,7,6,0,6,1,10,1,6,-1,
5,4,9,3,7,6,10,1,3,3,6,10,-1,-1,-1,-1,
1,5,4,1,4,8,1,8,2,2,8,7,2,7,6,-1,
1,5,4,4,0,1,7,6,2,2,3,7,-1,-1,-1,-1,
4,9,5,2,0,8,7,6,2,2,8,7,-1,-1,-1,-1,
5,4,9,3,7,6,6,2,3,-1,-1,-1,-1,-1,-1,-1,
6,11,7,5,4,8,5,8,3,5,3,10,2,10,3,-1,
6,11,7,0,2,10,5,4,0,0,10,5,-1,-1,-1,-1,
2,10,1,9,5,4,11,7,6,0,8,3,-1,-1,-1,-1,
2,10,1,9,5,4,6,11,7,-1,-1,-1,-1,-1,-1,-1,
7,6,11,1,5,4,8,3,1,1,4,8,-1,-1,-1,-1,
7,6,11,1,5,4,4,0,1,-1,-1,-1,-1,-1,-1,-1,
0,8,3,11,7,6,4,9,5,-1,-1,-1,-1,-1,-1,-1,
9,5,4,11,7,6,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
9,4,6,6,10,9,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
10,9,0,10,0,3,10,3,6,6,3,8,6,8,4,-1,
0,4,6,6,10,0,10,1,0,-1,-1,-1,-1,-1,-1,-1,
6,10,1,8,4,6,1,3,8,8,6,1,-1,-1,-1,-1,
4,6,2,2,1,4,1,9,4,-1,-1,-1,-1,-1,-1,-1,
0,1,9,6,2,3,8,4,6,6,3,8,-1,-1,-1,-1,
2,0,4,4,6,2,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
4,6,2,2,8,4,2,3,8,-1,-1,-1,-1,-1,-1,-1,
4,6,11,4,11,3,4,3,9,9,3,2,9,2,10,-1,
4,6,11,11,8,4,2,10,9,9,0,2,-1,-1,-1,-1,
2,10,1,4,6,11,3,0,4,4,11,3,-1,-1,-1,-1,
2,10,1,4,6,11,11,8,4,-1,-1,-1,-1,-1,-1,-1,
9,4,6,3,1,9,6,11,3,9,6,3,-1,-1,-1,-1,
9,0,1,11,8,4,4,6,11,-1,-1,-1,-1,-1,-1,-1,
0,4,6,6,3,0,6,11,3,-1,-1,-1,-1,-1,-1,-1,
11,8,4,4,6,11,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
10,9,8,8,7,10,7,6,10,-1,-1,-1,-1,-1,-1,-1,
0,3,7,10,9,0,7,6,10,0,7,10,-1,-1,-1,-1,
10,7,6,1,8,7,1,0,8,7,10,1,-1,-1,-1,-1,
1,3,7,7,10,1,7,6,10,-1,-1,-1,-1,-1,-1,-1,
8,7,6,1,9,8,6,2,1,1,8,6,-1,-1,-1,-1,
0,1,9,6,2,3,3,7,6,-1,-1,-1,-1,-1,-1,-1,
6,2,0,0,7,6,0,8,7,-1,-1,-1,-1,-1,-1,-1,
3,7,6,6,2,3,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
11,7,6,9,8,3,2,10,9,9,3,2,-1,-1,-1,-1,
6,11,7,0,2,10,10,9,0,-1,-1,-1,-1,-1,-1,-1,
6,11,7,8,3,0,2,10,1,-1,-1,-1,-1,-1,-1,-1,
1,2,10,7,6,11,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
11,7,6,9,8,3,3,1,9,-1,-1,-1,-1,-1,-1,-1,
6,11,7,0,1,9,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
0,8,3,6,11,7,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
7,6,11,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
11,6,7,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
3,11,6,6,0,3,6,7,8,8,0,6,-1,-1,-1,-1,
9,6,7,9,7,0,0,7,11,0,11,1,1,11,6,1,
1,3,11,1,11,6,1,6,9,9,6,7,9,7,8,-1,
10,6,7,7,1,10,7,11,2,2,1,7,-1,-1,-1,-1,
2,3,11,10,6,7,10,7,8,10,8,1,0,1,8,-1,
9,10,6,9,6,7,9,7,0,0,7,11,0,11,2,-1,
11,2,3,9,10,6,7,8,9,9,6,7,-1,-1,-1,-1,
6,7,3,3,2,6,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
0,2,6,6,7,0,7,8,0,-1,-1,-1,-1,-1,-1,-1,
7,3,0,7,0,9,7,9,6,6,9,1,6,1,2,-1,
9,1,2,7,8,9,2,6,7,7,9,2,-1,-1,-1,-1,
7,3,1,1,10,7,10,6,7,-1,-1,-1,-1,-1,-1,-1,
0,7,8,1,6,7,1,10,6,7,0,1,-1,-1,-1,-1,
6,7,3,9,10,6,3,0,9,6,3,9,-1,-1,-1,-1,
8,9,10,10,7,8,10,6,7,-1,-1,-1,-1,-1,-1,-1,
4,8,11,11,6,4,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
6,4,0,0,3,6,3,11,6,-1,-1,-1,-1,-1,-1,-1,
6,4,9,6,9,1,6,1,11,11,1,0,11,0,8,-1,
11,6,4,1,3,11,4,9,1,11,4,1,-1,-1,-1,-1,
8,11,2,8,2,1,8,1,4,4,1,10,4,10,6,-1,
2,3,11,4,0,1,10,6,4,4,1,10,-1,-1,-1,-1,
6,4,9,9,10,6,0,8,11,11,2,0,-1,-1,-1,-1,
11,2,3,9,10,6,6,4,9,-1,-1,-1,-1,-1,-1,-1,
2,6,4,4,8,2,8,3,2,-1,-1,-1,-1,-1,-1,-1,
6,4,0,0,2,6,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
0,8,3,6,4,9,1,2,6,6,9,1,-1,-1,-1,-1,
2,6,4,4,1,2,4,9,1,-1,-1,-1,-1,-1,-1,-1,
4,8,3,10,6,4,3,1,10,10,4,3,-1,-1,-1,-1,
6,4,0,0,10,6,0,1,10,-1,-1,-1,-1,-1,-1,-1,
0,8,3,6,4,9,9,10,6,-1,-1,-1,-1,-1,-1,-1,
6,4,9,9,10,6,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
4,7,11,11,9,4,11,6,5,5,9,11,-1,-1,-1,-1,
4,7,8,9,0,3,9,3,11,9,11,5,6,5,11,-1,
0,4,7,0,7,11,0,11,1,1,11,6,1,6,5,-1,
7,8,4,1,3,11,6,5,1,1,11,6,-1,-1,-1,-1,
6,5,10,11,2,1,11,1,9,11,9,7,4,7,9,-1,
1,9,0,8,4,7,10,6,5,3,11,2,-1,-1,-1,-1,
6,5,10,0,4,7,11,2,0,0,7,11,-1,-1,-1,-1,
4,7,8,3,11,2,6,5,10,-1,-1,-1,-1,-1,-1,-1,
2,6,5,2,5,9,2,9,3,3,9,4,3,4,7,-1,
4,7,8,2,6,5,9,0,2,2,5,9,-1,-1,-1,-1,
7,3,0,0,4,7,1,2,6,6,5,1,-1,-1,-1,-1,
4,7,8,2,6,5,5,1,2,-1,-1,-1,-1,-1,-1,-1,
5,10,6,3,1,9,4,7,3,3,9,4,-1,-1,-1,-1,
8,4,7,6,5,10,9,0,1,-1,-1,-1,-1,-1,-1,-1,
6,5,10,0,4,7,7,3,0,-1,-1,-1,-1,-1,-1,-1,
8,4,7,10,6,5,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
9,8,11,11,6,9,6,5,9,-1,-1,-1,-1,-1,-1,-1,
9,6,5,0,11,6,0,3,11,6,9,0,-1,-1,-1,-1,
11,6,5,0,8,11,5,1,0,0,11,5,-1,-1,-1,-1,
5,1,3,3,6,5,3,11,6,-1,-1,-1,-1,-1,-1,-1,
10,6,5,8,11,2,1,9,8,8,2,1,-1,-1,-1,-1,
5,10,6,11,2,3,1,9,0,-1,-1,-1,-1,-1,-1,-1,
10,6,5,8,11,2,2,0,8,-1,-1,-1,-1,-1,-1,-1,
3,11,2,5,10,6,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
3,2,6,9,8,3,6,5,9,3,6,9,-1,-1,-1,-1,
0,2,6,6,9,0,6,5,9,-1,-1,-1,-1,-1,-1,-1,
3,0,8,5,1,2,2,6,5,-1,-1,-1,-1,-1,-1,-1,
5,1,2,2,6,5,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
5,10,6,3,1,9,9,8,3,-1,-1,-1,-1,-1,-1,-1,
0,1,9,6,5,10,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
5,10,6,3,0,8,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
5,10,6,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
7,11,10,10,5,7,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
5,7,8,5,8,0,5,0,10,10,0,3,10,3,11,-1,
11,10,1,11,1,0,11,0,7,7,0,9,7,9,5,-1,
5,7,8,8,9,5,3,11,10,10,1,3,-1,-1,-1,-1,
1,5,7,7,11,1,11,2,1,-1,-1,-1,-1,-1,-1,-1,
3,11,2,5,7,8,0,1,5,5,8,0,-1,-1,-1,-1,
0,9,5,11,2,0,5,7,11,11,0,5,-1,-1,-1,-1,
3,11,2,5,7,8,8,9,5,-1,-1,-1,-1,-1,-1,-1,
5,7,3,3,2,5,2,10,5,-1,-1,-1,-1,-1,-1,-1,
8,0,2,5,7,8,2,10,5,8,2,5,-1,-1,-1,-1,
1,2,10,7,3,0,9,5,7,7,0,9,-1,-1,-1,-1,
10,1,2,8,9,5,5,7,8,-1,-1,-1,-1,-1,-1,-1,
1,5,7,7,3,1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
1,5,7,7,0,1,7,8,0,-1,-1,-1,-1,-1,-1,-1,
5,7,3,3,9,5,3,0,9,-1,-1,-1,-1,-1,-1,-1,
8,9,5,5,7,8,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
8,11,10,10,5,8,5,4,8,-1,-1,-1,-1,-1,-1,-1,
10,5,4,3,11,10,4,0,3,3,10,4,-1,-1,-1,-1,
9,5,4,11,10,1,0,8,11,11,1,0,-1,-1,-1,-1,
9,5,4,11,10,1,1,3,11,-1,-1,-1,-1,-1,-1,-1,
2,1,5,8,11,2,5,4,8,2,5,8,-1,-1,-1,-1,
2,3,11,4,0,1,1,5,4,-1,-1,-1,-1,-1,-1,-1,
4,9,5,2,0,8,8,11,2,-1,-1,-1,-1,-1,-1,-1,
4,9,5,2,3,11,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
8,5,4,3,10,5,3,2,10,5,8,3,-1,-1,-1,-1,
4,0,2,2,5,4,2,10,5,-1,-1,-1,-1,-1,-1,-1,
4,9,5,10,1,2,0,8,3,-1,-1,-1,-1,-1,-1,-1,
2,10,1,4,9,5,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
3,1,5,5,8,3,5,4,8,-1,-1,-1,-1,-1,-1,-1,
1,5,4,4,0,1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
3,0,8,5,4,9,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
5,4,9,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
11,10,9,9,4,11,4,7,11,-1,-1,-1,-1,-1,-1,-1,
8,4,7,10,9,0,3,11,10,10,0,3,-1,-1,-1,-1,
1,0,4,11,10,1,4,7,11,1,4,11,-1,-1,-1,-1,
7,8,4,1,3,11,11,10,1,-1,-1,-1,-1,-1,-1,-1,
11,4,7,2,9,4,2,1,9,4,11,2,-1,-1,-1,-1,
7,8,4,9,0,1,3,11,2,-1,-1,-1,-1,-1,-1,-1,
2,0,4,4,11,2,4,7,11,-1,-1,-1,-1,-1,-1,-1,
4,7,8,2,3,11,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
9,4,7,2,10,9,7,3,2,2,9,7,-1,-1,-1,-1,
8,4,7,10,9,0,0,2,10,-1,-1,-1,-1,-1,-1,-1,
1,2,10,7,3,0,0,4,7,-1,-1,-1,-1,-1,-1,-1,
7,8,4,1,2,10,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
7,3,1,1,4,7,1,9,4,-1,-1,-1,-1,-1,-1,-1,
7,8,4,1,9,0,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
7,3,0,0,4,7,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
8,4,7,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
10,9,8,8,11,10,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
11,10,9,9,3,11,9,0,3,-1,-1,-1,-1,-1,-1,-1,
8,11,10,10,0,8,10,1,0,-1,-1,-1,-1,-1,-1,-1,
11,10,1,1,3,11,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
9,8,11,11,1,9,11,2,1,-1,-1,-1,-1,-1,-1,-1,
11,2,3,9,0,1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
8,11,2,2,0,8,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
2,3,11,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
10,9,8,8,2,10,8,3,2,-1,-1,-1,-1,-1,-1,-1,
10,9,0,0,2,10,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
8,3,0,10,1,2,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
1,2,10,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
9,8,3,3,1,9,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
0,1,9,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
3,0,8,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1};
    }
        #endregion
}
