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
using System.Diagnostics;
namespace System
{
    /// <summary>
    /// Math3D
    /// </summary>
    public static partial class Math3D
    {
        //	angle indexes
        /// <summary>
        /// PITCH
        /// </summary>
        public const int PITCH = 0; // up / down 
        /// <summary>
        /// YAW
        /// </summary>
        public const int YAW = 1; // left / right 
        /// <summary>
        /// ROLL
        /// </summary>
        public const int ROLL = 2; // fall over 
        //
        private static float shortratio = 360.0f / 65536.0f;
        private static float piratio = (float)(Math.PI / 360.0);

        /// <summary>
        /// Sets the specified v1.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        public static void Set(float[] v1, float[] v2)
        {
            v1[0] = v2[0];
            v1[1] = v2[1];
            v1[2] = v2[2];
        }

        /// <summary>
        /// Vectors the subtract.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <param name="c">The c.</param>
        public static void VectorSubtract(float[] a, float[] b, float[] c)
        {
            c[0] = a[0] - b[0];
            c[1] = a[1] - b[1];
            c[2] = a[2] - b[2];
        }
        /// <summary>
        /// Vectors the subtract2.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <param name="c">The c.</param>
        public static void VectorSubtract2(short[] a, short[] b, int[] c)
        {
            c[0] = a[0] - b[0];
            c[1] = a[1] - b[1];
            c[2] = a[2] - b[2];
        }

        /// <summary>
        /// Vectors the add.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <param name="to">To.</param>
        public static void VectorAdd(float[] a, float[] b, float[] to)
        {
            to[0] = a[0] + b[0];
            to[1] = a[1] + b[1];
            to[2] = a[2] + b[2];
        }

        /// <summary>
        /// Vectors the copy.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        public static void VectorCopy(float[] from, float[] to)
        {
            to[0] = from[0];
            to[1] = from[1];
            to[2] = from[2];
        }
        /// <summary>
        /// Vectors the copy2.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        public static void VectorCopy2(short[] from, short[] to)
        {
            to[0] = from[0];
            to[1] = from[1];
            to[2] = from[2];
        }
        /// <summary>
        /// Vectors the copy3.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        public static void VectorCopy3(short[] from, float[] to)
        {
            to[0] = from[0];
            to[1] = from[1];
            to[2] = from[2];
        }
        /// <summary>
        /// Vectors the copy4.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        public static void VectorCopy4(float[] from, short[] to)
        {
            to[0] = (short)from[0];
            to[1] = (short)from[1];
            to[2] = (short)from[2];
        }

        /// <summary>
        /// Vectors the clear.
        /// </summary>
        /// <param name="a">A.</param>
        public static void VectorClear(float[] a)
        {
            a[0] = a[1] = a[2] = 0;
        }

        /// <summary>
        /// Vectors the equals.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <returns></returns>
        public static bool VectorEquals(float[] v1, float[] v2)
        {
            return !(v1[0] != v2[0] || v1[1] != v2[1] || v1[2] != v2[2]);
        }

        /// <summary>
        /// Vectors the negate.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        public static void VectorNegate(float[] from, float[] to)
        {
            to[0] = -from[0];
            to[1] = -from[1];
            to[2] = -from[2];
        }

        /// <summary>
        /// Vectors the set.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        public static void VectorSet(float[] v, float x, float y, float z)
        {
            v[0] = (x);
            v[1] = (y);
            v[2] = (z);
        }

        /// <summary>
        /// Vectors the MA.
        /// </summary>
        /// <param name="veca">The veca.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="vecb">The vecb.</param>
        /// <param name="to">To.</param>
        public static void VectorMA(float[] veca, float scale, float[] vecb, float[] to)
        {
            to[0] = veca[0] + scale * vecb[0];
            to[1] = veca[1] + scale * vecb[1];
            to[2] = veca[2] + scale * vecb[2];
        }

        /// <summary>
        /// Vectors the normalize.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <returns></returns>
        public static float VectorNormalize(float[] v)
        {

            float length = VectorLength(v);
            if (length != 0.0f)
            {
                float ilength = 1.0f / length;
                v[0] *= ilength;
                v[1] *= ilength;
                v[2] *= ilength;
            }
            return length;
        }

        /// <summary>
        /// Vectors the length.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <returns></returns>
        public static float VectorLength(float[] v)
        {
            return (float)Math.Sqrt(v[0] * v[0] + v[1] * v[1] + v[2] * v[2]);
        }

        /// <summary>
        /// Lengthes the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        /// <returns></returns>
        public static float Length(float x, float y, float z)
        {
            return (float)Math.Sqrt(x * x + y * y + z * z);
        }

        /// <summary>
        /// Vectors the inverse.
        /// </summary>
        /// <param name="v">The v.</param>
        public static void VectorInverse(float[] v)
        {
            v[0] = -v[0];
            v[1] = -v[1];
            v[2] = -v[2];
        }

        /// <summary>
        /// Vectors the scale.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="result">The result.</param>
        public static void VectorScale(float[] v, float scale, float[] result)
        {
            result[0] = v[0] * scale;
            result[1] = v[1] * scale;
            result[2] = v[2] * scale;
        }

        /// <summary>
        /// Vectoes the yaw.
        /// </summary>
        /// <param name="vec">The vec.</param>
        /// <returns></returns>
        public static float VectoYaw(float[] vec)
        {
            float yaw;
            if (/*vec[YAW] == 0 &&*/
                vec[PITCH] == 0)
            {
                yaw = 0;
                if (vec[YAW] > 0)
                    yaw = 90;
                else if (vec[YAW] < 0)
                    yaw = -90;
            }
            else
            {
                yaw = (int)(Math.Atan2(vec[YAW], vec[PITCH]) * 180 / Math.PI);
                if (yaw < 0)
                    yaw += 360;
            }
            return yaw;
        }

        /// <summary>
        /// Vectoes the angles.
        /// </summary>
        /// <param name="value1">The value1.</param>
        /// <param name="angles">The angles.</param>
        public static void VectoAngles(float[] value1, float[] angles)
        {
            float yaw;
            float pitch;
            if (value1[1] == 0 && value1[0] == 0)
            {
                yaw = 0;
                if (value1[2] > 0)
                    pitch = 90;
                else
                    pitch = 270;
            }
            else
            {
                if (value1[0] != 0)
                    yaw = (int)(Math.Atan2(value1[1], value1[0]) * 180 / Math.PI);
                else if (value1[1] > 0)
                    yaw = 90;
                else
                    yaw = -90;
                if (yaw < 0)
                    yaw += 360;
                var forward = (float)Math.Sqrt(value1[0] * value1[0] + value1[1] * value1[1]);
                pitch = (int)(Math.Atan2(value1[2], forward) * 180 / Math.PI);
                if (pitch < 0)
                    pitch += 360;
            }
            angles[PITCH] = -pitch;
            angles[YAW] = yaw;
            angles[ROLL] = 0;
        }

        private static float[][] m = new float[][] { new float[3], new float[3], new float[3] };
        private static float[][] im = new float[][] { new float[3], new float[3], new float[3] };
        private static float[][] tmpmat = new float[][] { new float[3], new float[3], new float[3] };
        private static float[][] zrot = new float[][] { new float[3], new float[3], new float[3] };
        // to reduce garbage
        private static float[] vr = { 0, 0, 0 };
        private static float[] vup = { 0, 0, 0 };
        private static float[] vf = { 0, 0, 0 };

        /// <summary>
        /// Rotates the point around vector.
        /// </summary>
        /// <param name="dst">The DST.</param>
        /// <param name="dir">The dir.</param>
        /// <param name="point">The point.</param>
        /// <param name="degrees">The degrees.</param>
        public static void RotatePointAroundVector(float[] dst, float[] dir, float[] point, float degrees)
        {
            vf[0] = dir[0];
            vf[1] = dir[1];
            vf[2] = dir[2];
            //
            PerpendicularVector(vr, dir);
            CrossProduct(vr, vf, vup);
            //
            m[0][0] = vr[0];
            m[1][0] = vr[1];
            m[2][0] = vr[2];
            //
            m[0][1] = vup[0];
            m[1][1] = vup[1];
            m[2][1] = vup[2];
            //
            m[0][2] = vf[0];
            m[1][2] = vf[1];
            m[2][2] = vf[2];
            //
            im[0][0] = m[0][0];
            im[0][1] = m[1][0];
            im[0][2] = m[2][0];
            im[1][0] = m[0][1];
            im[1][1] = m[1][1];
            im[1][2] = m[2][1];
            im[2][0] = m[0][2];
            im[2][1] = m[1][2];
            im[2][2] = m[2][2];
            //
            zrot[0][2] = zrot[1][2] = zrot[2][0] = zrot[2][1] = 0.0f;
            //
            zrot[2][2] = 1.0F;
            //
            zrot[0][0] = zrot[1][1] = (float)Math.Cos(DEG2RAD(degrees));
            zrot[0][1] = (float)Math.Sin(DEG2RAD(degrees));
            zrot[1][0] = -zrot[0][1];
            //
            R_ConcatRotations(m, zrot, tmpmat);
            R_ConcatRotations(tmpmat, im, zrot);
            for (int i = 0; i < 3; i++)
                dst[i] = zrot[i][0] * point[0] + zrot[i][1] * point[1] + zrot[i][2] * point[2];
        }

        /// <summary>
        /// Makes the normal vectors.
        /// </summary>
        /// <param name="forward">The forward.</param>
        /// <param name="right">The right.</param>
        /// <param name="up">Up.</param>
        public static void MakeNormalVectors(float[] forward, float[] right, float[] up)
        {
            // this rotate and negat guarantees a vector not colinear with the original
            right[1] = -forward[0];
            right[2] = forward[1];
            right[0] = forward[2];
            var d = DotProduct(right, forward);
            VectorMA(right, -d, forward, right);
            VectorNormalize(right);
            CrossProduct(right, forward, up);
        }

        /// <summary>
        /// SHORs the t2 ANGLE.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        public static float SHORT2ANGLE(short x)
        {
            return (x * shortratio);
        }

        // R_ConcatTransforms
        /// <summary>
        /// R_s the concat transforms.
        /// </summary>
        /// <param name="in1">The in1.</param>
        /// <param name="in2">The in2.</param>
        /// <param name="result">The result.</param>
        public static void R_ConcatTransforms(float[][] in1, float[][] in2, float[][] result)
        {
            result[0][0] = in1[0][0] * in2[0][0] + in1[0][1] * in2[1][0] + in1[0][2] * in2[2][0];
            result[0][1] = in1[0][0] * in2[0][1] + in1[0][1] * in2[1][1] + in1[0][2] * in2[2][1];
            result[0][2] = in1[0][0] * in2[0][2] + in1[0][1] * in2[1][2] + in1[0][2] * in2[2][2];
            result[0][3] = in1[0][0] * in2[0][3] + in1[0][1] * in2[1][3] + in1[0][2] * in2[2][3] + in1[0][3];
            result[1][0] = in1[1][0] * in2[0][0] + in1[1][1] * in2[1][0] + in1[1][2] * in2[2][0];
            result[1][1] = in1[1][0] * in2[0][1] + in1[1][1] * in2[1][1] + in1[1][2] * in2[2][1];
            result[1][2] = in1[1][0] * in2[0][2] + in1[1][1] * in2[1][2] + in1[1][2] * in2[2][2];
            result[1][3] = in1[1][0] * in2[0][3] + in1[1][1] * in2[1][3] + in1[1][2] * in2[2][3] + in1[1][3];
            result[2][0] = in1[2][0] * in2[0][0] + in1[2][1] * in2[1][0] + in1[2][2] * in2[2][0];
            result[2][1] = in1[2][0] * in2[0][1] + in1[2][1] * in2[1][1] + in1[2][2] * in2[2][1];
            result[2][2] = in1[2][0] * in2[0][2] + in1[2][1] * in2[1][2] + in1[2][2] * in2[2][2];
            result[2][3] = in1[2][0] * in2[0][3] + in1[2][1] * in2[1][3] + in1[2][2] * in2[2][3] + in1[2][3];
        }

        // concatenates 2 matrices each [3][3].
        /// <summary>
        /// R_s the concat rotations.
        /// </summary>
        /// <param name="in1">The in1.</param>
        /// <param name="in2">The in2.</param>
        /// <param name="result">The result.</param>
        public static void R_ConcatRotations(float[][] in1, float[][] in2, float[][] result)
        {
            result[0][0] = in1[0][0] * in2[0][0] + in1[0][1] * in2[1][0] + in1[0][2] * in2[2][0];
            result[0][1] = in1[0][0] * in2[0][1] + in1[0][1] * in2[1][1] + in1[0][2] * in2[2][1];
            result[0][2] = in1[0][0] * in2[0][2] + in1[0][1] * in2[1][2] + in1[0][2] * in2[2][2];
            result[1][0] = in1[1][0] * in2[0][0] + in1[1][1] * in2[1][0] + in1[1][2] * in2[2][0];
            result[1][1] = in1[1][0] * in2[0][1] + in1[1][1] * in2[1][1] + in1[1][2] * in2[2][1];
            result[1][2] = in1[1][0] * in2[0][2] + in1[1][1] * in2[1][2] + in1[1][2] * in2[2][2];
            result[2][0] = in1[2][0] * in2[0][0] + in1[2][1] * in2[1][0] + in1[2][2] * in2[2][0];
            result[2][1] = in1[2][0] * in2[0][1] + in1[2][1] * in2[1][1] + in1[2][2] * in2[2][1];
            result[2][2] = in1[2][0] * in2[0][2] + in1[2][1] * in2[1][2] + in1[2][2] * in2[2][2];
        }

        /// <summary>
        /// Projects the point on plane.
        /// </summary>
        /// <param name="dst">The DST.</param>
        /// <param name="p">The p.</param>
        /// <param name="normal">The normal.</param>
        public static void ProjectPointOnPlane(float[] dst, float[] p, float[] normal)
        {
            var inv_denom = 1.0F / DotProduct(normal, normal);
            var d = DotProduct(normal, p) * inv_denom;
            dst[0] = normal[0] * inv_denom;
            dst[1] = normal[1] * inv_denom;
            dst[2] = normal[2] * inv_denom;
            dst[0] = p[0] - d * dst[0];
            dst[1] = p[1] - d * dst[1];
            dst[2] = p[2] - d * dst[2];
        }

        private static float[][] PLANE_XYZ = { new float[] { 1, 0, 0 }, new float[] { 0, 1, 0 }, new float[] { 0, 0, 1 } };

        // assumes "src" is normalized
        /// <summary>
        /// Perpendiculars the vector.
        /// </summary>
        /// <param name="dst">The DST.</param>
        /// <param name="src">The SRC.</param>
        public static void PerpendicularVector(float[] dst, float[] src)
        {
            int pos;
            int i;
            float minelem = 1.0F;
            // find the smallest magnitude axially aligned vector 
            for (pos = 0, i = 0; i < 3; i++)
                if (Math.Abs(src[i]) < minelem)
                {
                    pos = i;
                    minelem = Math.Abs(src[i]);
                }
            // project the point onto the plane defined by src
            ProjectPointOnPlane(dst, PLANE_XYZ[pos], src);
            //normalize the result 
            VectorNormalize(dst);
        }

        /// <summary>
        /// Boxes the on plane side.
        /// </summary>
        /// <param name="emins">The emins.</param>
        /// <param name="emaxs">The emaxs.</param>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        public static int BoxOnPlaneSide(float[] emins, float[] emaxs, Plane3 p)
        {
            Debug.Assert(emins.Length == 3 && emaxs.Length == 3, "vec3_t bug");
            float dist1, dist2;
            int sides;
            // fast axial cases
            if (p.Type < 3)
            {
                if (p.Distance <= emins[p.Type])
                    return 1;
                if (p.Distance >= emaxs[p.Type])
                    return 2;
                return 3;
            }
            // general case
            switch (p.Signbits)
            {
                case 0:
                    dist1 = p.normal[0] * emaxs[0] + p.normal[1] * emaxs[1] + p.normal[2] * emaxs[2];
                    dist2 = p.normal[0] * emins[0] + p.normal[1] * emins[1] + p.normal[2] * emins[2];
                    break;
                case 1:
                    dist1 = p.normal[0] * emins[0] + p.normal[1] * emaxs[1] + p.normal[2] * emaxs[2];
                    dist2 = p.normal[0] * emaxs[0] + p.normal[1] * emins[1] + p.normal[2] * emins[2];
                    break;
                case 2:
                    dist1 = p.normal[0] * emaxs[0] + p.normal[1] * emins[1] + p.normal[2] * emaxs[2];
                    dist2 = p.normal[0] * emins[0] + p.normal[1] * emaxs[1] + p.normal[2] * emins[2];
                    break;
                case 3:
                    dist1 = p.normal[0] * emins[0] + p.normal[1] * emins[1] + p.normal[2] * emaxs[2];
                    dist2 = p.normal[0] * emaxs[0] + p.normal[1] * emaxs[1] + p.normal[2] * emins[2];
                    break;
                case 4:
                    dist1 = p.normal[0] * emaxs[0] + p.normal[1] * emaxs[1] + p.normal[2] * emins[2];
                    dist2 = p.normal[0] * emins[0] + p.normal[1] * emins[1] + p.normal[2] * emaxs[2];
                    break;
                case 5:
                    dist1 = p.normal[0] * emins[0] + p.normal[1] * emaxs[1] + p.normal[2] * emins[2];
                    dist2 = p.normal[0] * emaxs[0] + p.normal[1] * emins[1] + p.normal[2] * emaxs[2];
                    break;
                case 6:
                    dist1 = p.normal[0] * emaxs[0] + p.normal[1] * emins[1] + p.normal[2] * emins[2];
                    dist2 = p.normal[0] * emins[0] + p.normal[1] * emaxs[1] + p.normal[2] * emaxs[2];
                    break;
                case 7:
                    dist1 = p.normal[0] * emins[0] + p.normal[1] * emins[1] + p.normal[2] * emins[2];
                    dist2 = p.normal[0] * emaxs[0] + p.normal[1] * emaxs[1] + p.normal[2] * emaxs[2];
                    break;
                default:
                    dist1 = dist2 = 0;
                    Debug.Assert(false, "BoxOnPlaneSide bug");
                    break;
            }
            sides = 0;
            if (dist1 >= p.Distance)
                sides = 1;
            if (dist2 < p.Distance)
                sides |= 2;
            Debug.Assert(sides != 0, "BoxOnPlaneSide(): sides == 0 bug");
            return sides;
        }

        //	this is the slow, general version
        private static float[][] corners = new float[][] { new float[3], new float[3] };
        /// <summary>
        /// Boxes the on plane side slow.
        /// </summary>
        /// <param name="emins">The emins.</param>
        /// <param name="emaxs">The emaxs.</param>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        public static int BoxOnPlaneSideSlow(float[] emins, float[] emaxs, Plane3 p)
        {
            for (int i = 0; i < 3; i++)
            {
                if (p.normal[i] < 0)
                {
                    corners[0][i] = emins[i];
                    corners[1][i] = emaxs[i];
                }
                else
                {
                    corners[1][i] = emins[i];
                    corners[0][i] = emaxs[i];
                }
            }
            var dist1 = DotProduct(p.normal, corners[0]) - p.Distance;
            var dist2 = DotProduct(p.normal, corners[1]) - p.Distance;
            var sides = 0;
            if (dist1 >= 0)
                sides = 1;
            if (dist2 < 0)
                sides |= 2;
            return sides;
        }

        /// <summary>
        /// Angles the vectors.
        /// </summary>
        /// <param name="angles">The angles.</param>
        /// <param name="forward">The forward.</param>
        /// <param name="right">The right.</param>
        /// <param name="up">Up.</param>
        public static void AngleVectors(float[] angles, float[] forward, float[] right, float[] up)
        {
            float cr = 2.0f * piratio;
            float angle = (float)(angles[YAW] * (cr));
            float sy = (float)Math.Sin(angle);
            float cy = (float)Math.Cos(angle);
            angle = (float)(angles[PITCH] * (cr));
            float sp = (float)Math.Sin(angle);
            float cp = (float)Math.Cos(angle);
            if (forward != null)
            {
                forward[0] = cp * cy;
                forward[1] = cp * sy;
                forward[2] = -sp;
            }
            if (right != null || up != null)
            {
                angle = (float)(angles[ROLL] * (cr));
                float sr = (float)Math.Sin(angle);
                cr = (float)Math.Cos(angle);
                if (right != null)
                {
                    right[0] = (-sr * sp * cy + cr * sy);
                    right[1] = (-sr * sp * sy + -cr * cy);
                    right[2] = -sr * cp;
                }
                if (up != null)
                {
                    up[0] = (cr * sp * cy + sr * sy);
                    up[1] = (cr * sp * sy + -sr * cy);
                    up[2] = cr * cp;
                }
            }
        }

        /// <summary>
        /// G_s the project source.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="distance">The distance.</param>
        /// <param name="forward">The forward.</param>
        /// <param name="right">The right.</param>
        /// <param name="result">The result.</param>
        public static void G_ProjectSource(float[] point, float[] distance, float[] forward, float[] right, float[] result)
        {
            result[0] = point[0] + forward[0] * distance[0] + right[0] * distance[1];
            result[1] = point[1] + forward[1] * distance[0] + right[1] * distance[1];
            result[2] = point[2] + forward[2] * distance[0] + right[2] * distance[1] + distance[2];
        }

        /// <summary>
        /// Dots the product.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public static float DotProduct(float[] x, float[] y)
        {
            return x[0] * y[0] + x[1] * y[1] + x[2] * y[2];
        }

        /// <summary>
        /// Crosses the product.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <param name="cross">The cross.</param>
        public static void CrossProduct(float[] v1, float[] v2, float[] cross)
        {
            cross[0] = v1[1] * v2[2] - v1[2] * v2[1];
            cross[1] = v1[2] * v2[0] - v1[0] * v2[2];
            cross[2] = v1[0] * v2[1] - v1[1] * v2[0];
        }

        //public static int Q_log2(int val)
        //{
        //    int answer = 0;
        //    while ((val >>= 1) > 0)
        //        answer++;
        //    return answer;
        //}

        /// <summary>
        /// DEs the g2 RAD.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <returns></returns>
        public static float DEG2RAD(float v)
        {
            return (v * (float)Math.PI) / 180.0f;
        }

        /// <summary>
        /// Anglemods the specified a.
        /// </summary>
        /// <param name="a">A.</param>
        /// <returns></returns>
        public static float anglemod(float a)
        {
            return (float)(shortratio) * ((int)(a / (shortratio)) & 65535);
        }

        /// <summary>
        /// ANGLs the e2 SHORT.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        public static short ANGLE2SHORT(float x)
        {
            return (short)((short)((x) / shortratio) & 0xffff);
        }

        /// <summary>
        /// Lerps the angle.
        /// </summary>
        /// <param name="a2">The a2.</param>
        /// <param name="a1">The a1.</param>
        /// <param name="frac">The frac.</param>
        /// <returns></returns>
        public static float LerpAngle(float a2, float a1, float frac)
        {
            if (a1 - a2 > 180)
                a1 -= 360;
            if (a1 - a2 < -180)
                a1 += 360;
            return a2 + frac * (a1 - a2);
        }

        /// <summary>
        /// Calcs the fov.
        /// </summary>
        /// <param name="fov_x">The fov_x.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        public static float CalcFov(float fov_x, float width, float height)
        {
            double a = 0.0f;
            double x;
            if (fov_x < 1.0f || fov_x > 179.0f)
                throw new ArgumentOutOfRangeException("fox_v", "bad fov: " + fov_x.ToString());
            x = width / Math.Tan(fov_x * piratio);
            a = Math.Atan(height / x);
            a = a / piratio;
            return (float)a;
        }

        /// <summary>
        /// 
        /// </summary>
        public static float[][] VertexNormals = { new float[] {
            -0.525731f, 0.000000f, 0.850651f }, new float[] {
            -0.442863f, 0.238856f, 0.864188f }, new float[] {
            -0.295242f, 0.000000f, 0.955423f }, new float[] {
            -0.309017f, 0.500000f, 0.809017f }, new float[] {
            -0.162460f, 0.262866f, 0.951056f }, new float[] {
            0.000000f, 0.000000f, 1.000000f }, new float[] {
            0.000000f, 0.850651f, 0.525731f }, new float[] {
            -0.147621f, 0.716567f, 0.681718f }, new float[] {
            0.147621f, 0.716567f, 0.681718f }, new float[] {
            0.000000f, 0.525731f, 0.850651f }, new float[] {
            0.309017f, 0.500000f, 0.809017f }, new float[] {
            0.525731f, 0.000000f, 0.850651f }, new float[] {
            0.295242f, 0.000000f, 0.955423f }, new float[] {
            0.442863f, 0.238856f, 0.864188f }, new float[] {
            0.162460f, 0.262866f, 0.951056f }, new float[] {
            -0.681718f, 0.147621f, 0.716567f }, new float[] {
            -0.809017f, 0.309017f, 0.500000f }, new float[] {
            -0.587785f, 0.425325f, 0.688191f }, new float[] {
            -0.850651f, 0.525731f, 0.000000f }, new float[] {
            -0.864188f, 0.442863f, 0.238856f }, new float[] {
            -0.716567f, 0.681718f, 0.147621f }, new float[] {
            -0.688191f, 0.587785f, 0.425325f }, new float[] {
            -0.500000f, 0.809017f, 0.309017f }, new float[] {
            -0.238856f, 0.864188f, 0.442863f }, new float[] {
            -0.425325f, 0.688191f, 0.587785f }, new float[] {
            -0.716567f, 0.681718f, -0.147621f }, new float[] {
            -0.500000f, 0.809017f, -0.309017f }, new float[] {
            -0.525731f, 0.850651f, 0.000000f }, new float[] {
            0.000000f, 0.850651f, -0.525731f }, new float[] {
            -0.238856f, 0.864188f, -0.442863f }, new float[] {
            0.000000f, 0.955423f, -0.295242f }, new float[] {
            -0.262866f, 0.951056f, -0.162460f }, new float[] {
            0.000000f, 1.000000f, 0.000000f }, new float[] {
            0.000000f, 0.955423f, 0.295242f }, new float[] {
            -0.262866f, 0.951056f, 0.162460f }, new float[] {
            0.238856f, 0.864188f, 0.442863f }, new float[] {
            0.262866f, 0.951056f, 0.162460f }, new float[] {
            0.500000f, 0.809017f, 0.309017f }, new float[] {
            0.238856f, 0.864188f, -0.442863f }, new float[] {
            0.262866f, 0.951056f, -0.162460f }, new float[] {
            0.500000f, 0.809017f, -0.309017f }, new float[] {
            0.850651f, 0.525731f, 0.000000f }, new float[] {
            0.716567f, 0.681718f, 0.147621f }, new float[] {
            0.716567f, 0.681718f, -0.147621f }, new float[] {
            0.525731f, 0.850651f, 0.000000f }, new float[] {
            0.425325f, 0.688191f, 0.587785f }, new float[] {
            0.864188f, 0.442863f, 0.238856f }, new float[] {
            0.688191f, 0.587785f, 0.425325f }, new float[] {
            0.809017f, 0.309017f, 0.500000f }, new float[] {
            0.681718f, 0.147621f, 0.716567f }, new float[] {
            0.587785f, 0.425325f, 0.688191f }, new float[] {
            0.955423f, 0.295242f, 0.000000f }, new float[] {
            1.000000f, 0.000000f, 0.000000f }, new float[] {
            0.951056f, 0.162460f, 0.262866f }, new float[] {
            0.850651f, -0.525731f, 0.000000f }, new float[] {
            0.955423f, -0.295242f, 0.000000f }, new float[] {
            0.864188f, -0.442863f, 0.238856f }, new float[] {
            0.951056f, -0.162460f, 0.262866f }, new float[] {
            0.809017f, -0.309017f, 0.500000f }, new float[] {
            0.681718f, -0.147621f, 0.716567f }, new float[] {
            0.850651f, 0.000000f, 0.525731f }, new float[] {
            0.864188f, 0.442863f, -0.238856f }, new float[] {
            0.809017f, 0.309017f, -0.500000f }, new float[] {
            0.951056f, 0.162460f, -0.262866f }, new float[] {
            0.525731f, 0.000000f, -0.850651f }, new float[] {
            0.681718f, 0.147621f, -0.716567f }, new float[] {
            0.681718f, -0.147621f, -0.716567f }, new float[] {
            0.850651f, 0.000000f, -0.525731f }, new float[] {
            0.809017f, -0.309017f, -0.500000f }, new float[] {
            0.864188f, -0.442863f, -0.238856f }, new float[] {
            0.951056f, -0.162460f, -0.262866f }, new float[] {
            0.147621f, 0.716567f, -0.681718f }, new float[] {
            0.309017f, 0.500000f, -0.809017f }, new float[] {
            0.425325f, 0.688191f, -0.587785f }, new float[] {
            0.442863f, 0.238856f, -0.864188f }, new float[] {
            0.587785f, 0.425325f, -0.688191f }, new float[] {
            0.688191f, 0.587785f, -0.425325f }, new float[] {
            -0.147621f, 0.716567f, -0.681718f }, new float[] {
            -0.309017f, 0.500000f, -0.809017f }, new float[] {
            0.000000f, 0.525731f, -0.850651f }, new float[] {
            -0.525731f, 0.000000f, -0.850651f }, new float[] {
            -0.442863f, 0.238856f, -0.864188f }, new float[] {
            -0.295242f, 0.000000f, -0.955423f }, new float[] {
            -0.162460f, 0.262866f, -0.951056f }, new float[] {
            0.000000f, 0.000000f, -1.000000f }, new float[] {
            0.295242f, 0.000000f, -0.955423f }, new float[] {
            0.162460f, 0.262866f, -0.951056f }, new float[] {
            -0.442863f, -0.238856f, -0.864188f }, new float[] {
            -0.309017f, -0.500000f, -0.809017f }, new float[] {
            -0.162460f, -0.262866f, -0.951056f }, new float[] {
            0.000000f, -0.850651f, -0.525731f }, new float[] {
            -0.147621f, -0.716567f, -0.681718f }, new float[] {
            0.147621f, -0.716567f, -0.681718f }, new float[] {
            0.000000f, -0.525731f, -0.850651f }, new float[] {
            0.309017f, -0.500000f, -0.809017f }, new float[] {
            0.442863f, -0.238856f, -0.864188f }, new float[] {
            0.162460f, -0.262866f, -0.951056f }, new float[] {
            0.238856f, -0.864188f, -0.442863f }, new float[] {
            0.500000f, -0.809017f, -0.309017f }, new float[] {
            0.425325f, -0.688191f, -0.587785f }, new float[] {
            0.716567f, -0.681718f, -0.147621f }, new float[] {
            0.688191f, -0.587785f, -0.425325f }, new float[] {
            0.587785f, -0.425325f, -0.688191f }, new float[] {
            0.000000f, -0.955423f, -0.295242f }, new float[] {
            0.000000f, -1.000000f, 0.000000f }, new float[] {
            0.262866f, -0.951056f, -0.162460f }, new float[] {
            0.000000f, -0.850651f, 0.525731f }, new float[] {
            0.000000f, -0.955423f, 0.295242f }, new float[] {
            0.238856f, -0.864188f, 0.442863f }, new float[] {
            0.262866f, -0.951056f, 0.162460f }, new float[] {
            0.500000f, -0.809017f, 0.309017f }, new float[] {
            0.716567f, -0.681718f, 0.147621f }, new float[] {
            0.525731f, -0.850651f, 0.000000f }, new float[] {
            -0.238856f, -0.864188f, -0.442863f }, new float[] {
            -0.500000f, -0.809017f, -0.309017f }, new float[] {
            -0.262866f, -0.951056f, -0.162460f }, new float[] {
            -0.850651f, -0.525731f, 0.000000f }, new float[] {
            -0.716567f, -0.681718f, -0.147621f }, new float[] {
            -0.716567f, -0.681718f, 0.147621f }, new float[] {
            -0.525731f, -0.850651f, 0.000000f }, new float[] {
            -0.500000f, -0.809017f, 0.309017f }, new float[] {
            -0.238856f, -0.864188f, 0.442863f }, new float[] {
            -0.262866f, -0.951056f, 0.162460f }, new float[] {
            -0.864188f, -0.442863f, 0.238856f }, new float[] {
            -0.809017f, -0.309017f, 0.500000f }, new float[] {
            -0.688191f, -0.587785f, 0.425325f }, new float[] {
            -0.681718f, -0.147621f, 0.716567f }, new float[] {
            -0.442863f, -0.238856f, 0.864188f }, new float[] {
            -0.587785f, -0.425325f, 0.688191f }, new float[] {
            -0.309017f, -0.500000f, 0.809017f }, new float[] {
            -0.147621f, -0.716567f, 0.681718f }, new float[] {
            -0.425325f, -0.688191f, 0.587785f }, new float[] {
            -0.162460f, -0.262866f, 0.951056f }, new float[] {
            0.442863f, -0.238856f, 0.864188f }, new float[] {
            0.162460f, -0.262866f, 0.951056f }, new float[] {
            0.309017f, -0.500000f, 0.809017f }, new float[] {
            0.147621f, -0.716567f, 0.681718f }, new float[] {
            0.000000f, -0.525731f, 0.850651f }, new float[] {
            0.425325f, -0.688191f, 0.587785f }, new float[] {
            0.587785f, -0.425325f, 0.688191f }, new float[] {
            0.688191f, -0.587785f, 0.425325f }, new float[] {
            -0.955423f, 0.295242f, 0.000000f }, new float[] {
            -0.951056f, 0.162460f, 0.262866f }, new float[] {
            -1.000000f, 0.000000f, 0.000000f }, new float[] {
            -0.850651f, 0.000000f, 0.525731f }, new float[] {
            -0.955423f, -0.295242f, 0.000000f }, new float[]{
            -0.951056f, -0.162460f, 0.262866f }, new float[]{
            -0.864188f, 0.442863f, -0.238856f }, new float[]{
            -0.951056f, 0.162460f, -0.262866f }, new float[]{
            -0.809017f, 0.309017f, -0.500000f }, new float[]{
            -0.864188f, -0.442863f, -0.238856f }, new float[]{
            -0.951056f, -0.162460f, -0.262866f }, new float[]{
            -0.809017f, -0.309017f, -0.500000f }, new float[]{
            -0.681718f, 0.147621f, -0.716567f }, new float[]{
            -0.681718f, -0.147621f, -0.716567f }, new float[]{
            -0.850651f, 0.000000f, -0.525731f }, new float[]{
            -0.688191f, 0.587785f, -0.425325f }, new float[]{
            -0.587785f, 0.425325f, -0.688191f }, new float[]{
            -0.425325f, 0.688191f, -0.587785f }, new float[]{
            -0.425325f, -0.688191f, -0.587785f }, new float[]{
            -0.587785f, -0.425325f, -0.688191f }, new float[]{
            -0.688191f, -0.587785f, -0.425325f }
        };
    }
}