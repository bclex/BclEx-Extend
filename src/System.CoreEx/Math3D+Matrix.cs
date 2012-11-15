#region License
/*
The MIT License

Copyright (c) 2008 Sky Morey

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion
namespace System
{
    public static partial class Math3D
    {
        /// <summary>
        /// Multiplies the MM.
        /// </summary>
        /// <param name="ab">The ab.</param>
        /// <param name="abOfs">The ab ofs.</param>
        /// <param name="a">A.</param>
        /// <param name="aOfs">A ofs.</param>
        /// <param name="b">The b.</param>
        /// <param name="bOfs">The b ofs.</param>
        public static void MultiplyMM(float[] ab, int abOfs, float[] a, int aOfs, float[] b, int bOfs)
        {
            for (var i = 0; i < 4; i++)
            {
                ab[abOfs + 0] =
                  (b[bOfs + 0] * a[aOfs + 0]) +
                  (b[bOfs + 1] * a[aOfs + 4]) +
                  (b[bOfs + 2] * a[aOfs + 8]) +
                  (b[bOfs + 3] * a[aOfs + 12]);
                ab[abOfs + 1] =
                  (b[bOfs + 0] * a[aOfs + 1]) +
                  (b[bOfs + 1] * a[aOfs + 5]) +
                  (b[bOfs + 2] * a[aOfs + 9]) +
                  (b[bOfs + 3] * a[aOfs + 13]);
                ab[abOfs + 2] =
                  (b[bOfs + 0] * a[aOfs + 2]) +
                  (b[bOfs + 1] * a[aOfs + 6]) +
                  (b[bOfs + 2] * a[aOfs + 10]) +
                  (b[bOfs + 3] * a[aOfs + 14]);
                ab[abOfs + 3] =
                  (b[bOfs + 0] * a[aOfs + 3]) +
                  (b[bOfs + 1] * a[aOfs + 7]) +
                  (b[bOfs + 2] * a[aOfs + 11]) +
                  (b[bOfs + 3] * a[aOfs + 15]);
                abOfs += 4;
                bOfs += 4;
            }
        }

        /// <summary>
        /// Multiplies the MV.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="rOfs">The r ofs.</param>
        /// <param name="m">The m.</param>
        /// <param name="mOfs">The m ofs.</param>
        /// <param name="v">The v.</param>
        /// <param name="vOfs">The v ofs.</param>
        public static void MultiplyMV(float[] result, int rOfs, float[] m, int mOfs, float[] v, int vOfs)
        {
            var x = v[vOfs + 0];
            var y = v[vOfs + 1];
            var z = v[vOfs + 2];
            var w = v[vOfs + 3];
            result[rOfs + 0] = m[mOfs + 0] * x + m[mOfs + 4] * y + m[mOfs + 8] * z + m[mOfs + 12] * w;
            result[rOfs + 1] = m[mOfs + 1] * x + m[mOfs + 5] * y + m[mOfs + 9] * z + m[mOfs + 13] * w;
            result[rOfs + 2] = m[mOfs + 2] * x + m[mOfs + 6] * y + m[mOfs + 10] * z + m[mOfs + 14] * w;
            result[rOfs + 3] = m[mOfs + 3] * x + m[mOfs + 7] * y + m[mOfs + 11] * z + m[mOfs + 15] * w;
        }

        /// <summary>
        /// Transposes the M.
        /// </summary>
        /// <param name="mTrans">The m trans.</param>
        /// <param name="mTransOffset">The m trans offset.</param>
        /// <param name="m">The m.</param>
        /// <param name="mOffset">The m offset.</param>
        public static void TransposeM(float[] mTrans, int mTransOffset, float[] m, int mOffset)
        {
            for (var i = 0; i < 4; i++)
            {
                var mBase = i * 4 + mOffset;
                mTrans[i + mTransOffset] = m[mBase];
                mTrans[i + 4 + mTransOffset] = m[mBase + 1];
                mTrans[i + 8 + mTransOffset] = m[mBase + 2];
                mTrans[i + 12 + mTransOffset] = m[mBase + 3];
            }
        }

        /// <summary>
        /// Inverts the M.
        /// </summary>
        /// <param name="mInv">The m inv.</param>
        /// <param name="mInvOffset">The m inv offset.</param>
        /// <param name="m">The m.</param>
        /// <param name="mOffset">The m offset.</param>
        /// <returns></returns>
        public static bool InvertM(float[] mInv, int mInvOffset, float[] m, int mOffset)
        {
            // Invert a 4 x 4 matrix using Cramer's Rule
            // array of transpose source matrix
            var src = new float[16];
            // transpose matrix
            TransposeM(src, 0, m, mOffset);
            // temp array for pairs
            var tmp = new float[12];
            // calculate pairs for first 8 elements (cofactors)
            tmp[0] = src[10] * src[15];
            tmp[1] = src[11] * src[14];
            tmp[2] = src[9] * src[15];
            tmp[3] = src[11] * src[13];
            tmp[4] = src[9] * src[14];
            tmp[5] = src[10] * src[13];
            tmp[6] = src[8] * src[15];
            tmp[7] = src[11] * src[12];
            tmp[8] = src[8] * src[14];
            tmp[9] = src[10] * src[12];
            tmp[10] = src[8] * src[13];
            tmp[11] = src[9] * src[12];
            // Holds the destination matrix while we're building it up.
            var dst = new float[16];
            // calculate first 8 elements (cofactors)
            dst[0] = tmp[0] * src[5] + tmp[3] * src[6] + tmp[4] * src[7];
            dst[0] -= tmp[1] * src[5] + tmp[2] * src[6] + tmp[5] * src[7];
            dst[1] = tmp[1] * src[4] + tmp[6] * src[6] + tmp[9] * src[7];
            dst[1] -= tmp[0] * src[4] + tmp[7] * src[6] + tmp[8] * src[7];
            dst[2] = tmp[2] * src[4] + tmp[7] * src[5] + tmp[10] * src[7];
            dst[2] -= tmp[3] * src[4] + tmp[6] * src[5] + tmp[11] * src[7];
            dst[3] = tmp[5] * src[4] + tmp[8] * src[5] + tmp[11] * src[6];
            dst[3] -= tmp[4] * src[4] + tmp[9] * src[5] + tmp[10] * src[6];
            dst[4] = tmp[1] * src[1] + tmp[2] * src[2] + tmp[5] * src[3];
            dst[4] -= tmp[0] * src[1] + tmp[3] * src[2] + tmp[4] * src[3];
            dst[5] = tmp[0] * src[0] + tmp[7] * src[2] + tmp[8] * src[3];
            dst[5] -= tmp[1] * src[0] + tmp[6] * src[2] + tmp[9] * src[3];
            dst[6] = tmp[3] * src[0] + tmp[6] * src[1] + tmp[11] * src[3];
            dst[6] -= tmp[2] * src[0] + tmp[7] * src[1] + tmp[10] * src[3];
            dst[7] = tmp[4] * src[0] + tmp[9] * src[1] + tmp[10] * src[2];
            dst[7] -= tmp[5] * src[0] + tmp[8] * src[1] + tmp[11] * src[2];
            // calculate pairs for second 8 elements (cofactors)
            tmp[0] = src[2] * src[7];
            tmp[1] = src[3] * src[6];
            tmp[2] = src[1] * src[7];
            tmp[3] = src[3] * src[5];
            tmp[4] = src[1] * src[6];
            tmp[5] = src[2] * src[5];
            tmp[6] = src[0] * src[7];
            tmp[7] = src[3] * src[4];
            tmp[8] = src[0] * src[6];
            tmp[9] = src[2] * src[4];
            tmp[10] = src[0] * src[5];
            tmp[11] = src[1] * src[4];
            // calculate second 8 elements (cofactors)
            dst[8] = tmp[0] * src[13] + tmp[3] * src[14] + tmp[4] * src[15];
            dst[8] -= tmp[1] * src[13] + tmp[2] * src[14] + tmp[5] * src[15];
            dst[9] = tmp[1] * src[12] + tmp[6] * src[14] + tmp[9] * src[15];
            dst[9] -= tmp[0] * src[12] + tmp[7] * src[14] + tmp[8] * src[15];
            dst[10] = tmp[2] * src[12] + tmp[7] * src[13] + tmp[10] * src[15];
            dst[10] -= tmp[3] * src[12] + tmp[6] * src[13] + tmp[11] * src[15];
            dst[11] = tmp[5] * src[12] + tmp[8] * src[13] + tmp[11] * src[14];
            dst[11] -= tmp[4] * src[12] + tmp[9] * src[13] + tmp[10] * src[14];
            dst[12] = tmp[2] * src[10] + tmp[5] * src[11] + tmp[1] * src[9];
            dst[12] -= tmp[4] * src[11] + tmp[0] * src[9] + tmp[3] * src[10];
            dst[13] = tmp[8] * src[11] + tmp[0] * src[8] + tmp[7] * src[10];
            dst[13] -= tmp[6] * src[10] + tmp[9] * src[11] + tmp[1] * src[8];
            dst[14] = tmp[6] * src[9] + tmp[11] * src[11] + tmp[3] * src[8];
            dst[14] -= tmp[10] * src[11] + tmp[2] * src[8] + tmp[7] * src[9];
            dst[15] = tmp[10] * src[10] + tmp[4] * src[8] + tmp[9] * src[9];
            dst[15] -= tmp[8] * src[9] + tmp[11] * src[10] + tmp[5] * src[8];
            // calculate determinant
            var det = src[0] * dst[0] + src[1] * dst[1] + src[2] * dst[2] + src[3] * dst[3];
            if (det == 0.0f) { }
            // calculate matrix inverse
            det = 1 / det;
            for (var j = 0; j < 16; j++)
                mInv[j + mInvOffset] = dst[j] * det;
            return true;
        }

        /// <summary>
        /// Orthoes the M.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="mOffset">The m offset.</param>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <param name="bottom">The bottom.</param>
        /// <param name="top">The top.</param>
        /// <param name="near">The near.</param>
        /// <param name="far">The far.</param>
        public static void OrthoM(float[] m, int mOffset, float left, float right, float bottom, float top, float near, float far)
        {
            if (left == right)
                throw new Exception("ArgumentException: left == right");
            if (bottom == top)
                throw new Exception("ArgumentException: bottom == top");
            if (near == far)
                throw new Exception("ArgumentException: near == far");
            var r_width = 1.0f / (right - left);
            var r_height = 1.0f / (top - bottom);
            var r_depth = 1.0f / (far - near);
            var x = 2.0f * (r_width);
            var y = 2.0f * (r_height);
            var z = -2.0f * (r_depth);
            var tx = -(right + left) * r_width;
            var ty = -(top + bottom) * r_height;
            var tz = -(far + near) * r_depth;
            m[mOffset + 0] = x;
            m[mOffset + 5] = y;
            m[mOffset + 10] = z;
            m[mOffset + 12] = tx;
            m[mOffset + 13] = ty;
            m[mOffset + 14] = tz;
            m[mOffset + 15] = 1.0f;
            m[mOffset + 1] = 0.0f;
            m[mOffset + 2] = 0.0f;
            m[mOffset + 3] = 0.0f;
            m[mOffset + 4] = 0.0f;
            m[mOffset + 6] = 0.0f;
            m[mOffset + 7] = 0.0f;
            m[mOffset + 8] = 0.0f;
            m[mOffset + 9] = 0.0f;
            m[mOffset + 11] = 0.0f;
        }

        /// <summary>
        /// Frustums the M.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <param name="bottom">The bottom.</param>
        /// <param name="top">The top.</param>
        /// <param name="near">The near.</param>
        /// <param name="far">The far.</param>
        public static void FrustumM(float[] m, int offset, float left, float right, float bottom, float top, float near, float far)
        {
            if (left == right)
                throw new Exception("ArgumentException: left == right");
            if (top == bottom)
                throw new Exception("ArgumentException: top == bottom");
            if (near == far)
                throw new Exception("ArgumentException: near == far");
            if (near <= 0.0f)
                throw new Exception("ArgumentException: near <= 0.0f");
            if (far <= 0.0f)
                throw new Exception("ArgumentException: far <= 0.0f");
            var r_width = 1.0f / (right - left);
            var r_height = 1.0f / (top - bottom);
            var r_depth = 1.0f / (near - far);
            var x = 2.0f * (near * r_width);
            var y = 2.0f * (near * r_height);
            var A = 2.0f * ((right + left) * r_width);
            var B = (top + bottom) * r_height;
            var C = (far + near) * r_depth;
            var D = 2.0f * (far * near * r_depth);
            m[offset + 0] = x;
            m[offset + 5] = y;
            m[offset + 8] = A;
            m[offset + 9] = B;
            m[offset + 10] = C;
            m[offset + 14] = D;
            m[offset + 11] = -1.0f;
            m[offset + 1] = 0.0f;
            m[offset + 2] = 0.0f;
            m[offset + 3] = 0.0f;
            m[offset + 4] = 0.0f;
            m[offset + 6] = 0.0f;
            m[offset + 7] = 0.0f;
            m[offset + 12] = 0.0f;
            m[offset + 13] = 0.0f;
            m[offset + 15] = 0.0f;
        }

        /// <summary>
        /// Sets the identity M.
        /// </summary>
        /// <param name="sm">The sm.</param>
        /// <param name="smOffset">The sm offset.</param>
        public static void SetIdentityM(float[] sm, int smOffset)
        {
            for (var i = 0; i < 16; i++)
                sm[smOffset + i] = 0;
            for (var i = 0; i < 16; i += 5)
                sm[smOffset + i] = 1.0f;
        }

        /// <summary>
        /// Scales the M.
        /// </summary>
        /// <param name="sm">The sm.</param>
        /// <param name="smOffset">The sm offset.</param>
        /// <param name="m">The m.</param>
        /// <param name="mOffset">The m offset.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        public static void ScaleM(float[] sm, int smOffset, float[] m, int mOffset, float x, float y, float z)
        {
            for (var i = 0; i < 4; i++)
            {
                var smi = smOffset + i;
                var mi = mOffset + i;
                sm[smi] = m[mi] * x;
                sm[4 + smi] = m[4 + mi] * y;
                sm[8 + smi] = m[8 + mi] * z;
                sm[12 + smi] = m[12 + mi];
            }
        }

        /// <summary>
        /// Scales the m2.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="mOffset">The m offset.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        public static void ScaleM2(float[] m, int mOffset, float x, float y, float z)
        {
            for (var i = 0; i < 4; i++)
            {
                var mi = mOffset + i;
                m[mi] *= x;
                m[4 + mi] *= y;
                m[8 + mi] *= z;
            }
        }

        /// <summary>
        /// Translates the M.
        /// </summary>
        /// <param name="tm">The tm.</param>
        /// <param name="tmOffset">The tm offset.</param>
        /// <param name="m">The m.</param>
        /// <param name="mOffset">The m offset.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        public static void TranslateM(float[] tm, int tmOffset, float[] m, int mOffset, float x, float y, float z)
        {
            for (var i = 0; i < 4; i++)
            {
                var tmi = tmOffset + i;
                var mi = mOffset + i;
                tm[12 + tmi] = m[mi] * x + m[4 + mi] * y + m[8 + mi] * z + m[12 + mi];
            }
        }

        /// <summary>
        /// Translates the m2.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="mOffset">The m offset.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        public static void TranslateM2(float[] m, int mOffset, float x, float y, float z)
        {
            for (var i = 0; i < 4; i++)
            {
                var mi = mOffset + i;
                m[12 + mi] += m[mi] * x + m[4 + mi] * y + m[8 + mi] * z;
            }
        }

        /// <summary>
        /// Rotates the M.
        /// </summary>
        /// <param name="rm">The rm.</param>
        /// <param name="rmOffset">The rm offset.</param>
        /// <param name="m">The m.</param>
        /// <param name="mOffset">The m offset.</param>
        /// <param name="a">A.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        public static void RotateM(float[] rm, int rmOffset, float[] m, int mOffset, float a, float x, float y, float z)
        {
            var r = new float[16];
            SetRotateM(r, 0, a, x, y, z);
            MultiplyMM(rm, rmOffset, m, mOffset, r, 0);
        }

        /// <summary>
        /// Rotates the m2.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="mOffset">The m offset.</param>
        /// <param name="a">A.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        public static void RotateM2(float[] m, int mOffset, float a, float x, float y, float z)
        {
            var temp = new float[32];
            SetRotateM(temp, 0, a, x, y, z);
            MultiplyMM(temp, 16, m, mOffset, temp, 0);
            Array.Copy(temp, 16, m, mOffset, 16);
        }

        /// <summary>
        /// Sets the rotate M.
        /// </summary>
        /// <param name="rm">The rm.</param>
        /// <param name="rmOffset">The rm offset.</param>
        /// <param name="a">A.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        public static void SetRotateM(float[] rm, int rmOffset, float a, float x, float y, float z)
        {
            rm[rmOffset + 3] = 0;
            rm[rmOffset + 7] = 0;
            rm[rmOffset + 11] = 0;
            rm[rmOffset + 12] = 0;
            rm[rmOffset + 13] = 0;
            rm[rmOffset + 14] = 0;
            rm[rmOffset + 15] = 1;
            a *= (float)(Math.PI / 180.0f);
            var s = (float)Math.Sin(a);
            var c = (float)Math.Cos(a);
            if (1.0f == x && 0.0f == y && 0.0f == z)
            {
                rm[rmOffset + 5] = c; rm[rmOffset + 10] = c;
                rm[rmOffset + 6] = s; rm[rmOffset + 9] = -s;
                rm[rmOffset + 1] = 0; rm[rmOffset + 2] = 0;
                rm[rmOffset + 4] = 0; rm[rmOffset + 8] = 0;
                rm[rmOffset + 0] = 1;
            }
            else if (0.0f == x && 1.0f == y && 0.0f == z)
            {
                rm[rmOffset + 0] = c; rm[rmOffset + 10] = c;
                rm[rmOffset + 8] = s; rm[rmOffset + 2] = -s;
                rm[rmOffset + 1] = 0; rm[rmOffset + 4] = 0;
                rm[rmOffset + 6] = 0; rm[rmOffset + 9] = 0;
                rm[rmOffset + 5] = 1;
            }
            else if (0.0f == x && 0.0f == y && 1.0f == z)
            {
                rm[rmOffset + 0] = c; rm[rmOffset + 5] = c;
                rm[rmOffset + 1] = s; rm[rmOffset + 4] = -s;
                rm[rmOffset + 2] = 0; rm[rmOffset + 6] = 0;
                rm[rmOffset + 8] = 0; rm[rmOffset + 9] = 0;
                rm[rmOffset + 10] = 1;
            }
            else
            {
                var len = Math3D.Length(x, y, z);
                if (1.0f != len)
                {
                    float recipLen = 1.0f / len;
                    x *= recipLen;
                    y *= recipLen;
                    z *= recipLen;
                }
                var nc = 1.0f - c;
                var xy = x * y;
                var yz = y * z;
                var zx = z * x;
                var xs = x * s;
                var ys = y * s;
                var zs = z * s;
                rm[rmOffset + 0] = x * x * nc + c;
                rm[rmOffset + 4] = xy * nc - zs;
                rm[rmOffset + 8] = zx * nc + ys;
                rm[rmOffset + 1] = xy * nc + zs;
                rm[rmOffset + 5] = y * y * nc + c;
                rm[rmOffset + 9] = yz * nc - xs;
                rm[rmOffset + 2] = zx * nc - ys;
                rm[rmOffset + 6] = yz * nc + xs;
                rm[rmOffset + 10] = z * z * nc + c;
            }
        }

        /// <summary>
        /// Sets the rotate euler M.
        /// </summary>
        /// <param name="rm">The rm.</param>
        /// <param name="rmOffset">The rm offset.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        public static void SetRotateEulerM(float[] rm, int rmOffset, float x, float y, float z)
        {
            x *= (float)(Math.PI / 180.0f);
            y *= (float)(Math.PI / 180.0f);
            z *= (float)(Math.PI / 180.0f);
            var cx = (float)Math.Cos(x);
            var sx = (float)Math.Sin(x);
            var cy = (float)Math.Cos(y);
            var sy = (float)Math.Sin(y);
            var cz = (float)Math.Cos(z);
            var sz = (float)Math.Sin(z);
            var cxsy = cx * sy;
            var sxsy = sx * sy;
            rm[rmOffset + 0] = cy * cz;
            rm[rmOffset + 1] = -cy * sz;
            rm[rmOffset + 2] = sy;
            rm[rmOffset + 3] = 0.0f;
            //
            rm[rmOffset + 4] = cxsy * cz + cx * sz;
            rm[rmOffset + 5] = -cxsy * sz + cx * cz;
            rm[rmOffset + 6] = -sx * cy;
            rm[rmOffset + 7] = 0.0f;
            //
            rm[rmOffset + 8] = -sxsy * cz + sx * sz;
            rm[rmOffset + 9] = sxsy * sz + sx * cz;
            rm[rmOffset + 10] = cx * cy;
            rm[rmOffset + 11] = 0.0f;
            //
            rm[rmOffset + 12] = 0.0f;
            rm[rmOffset + 13] = 0.0f;
            rm[rmOffset + 14] = 0.0f;
            rm[rmOffset + 15] = 1.0f;
        }
    }
}