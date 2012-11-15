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
    /// <summary>
    /// Plane3
    /// </summary>
    public class Plane3
    {
        /// <summary>
        /// 
        /// </summary>
        public float[] normal = new float[3];
        /// <summary>
        /// 
        /// </summary>
        public float Distance;
        // This is for fast side tests, 0=xplane, 1=yplane, 2=zplane and 3=arbitrary.
        /// <summary>
        /// 
        /// </summary>
        public byte Type;
        // This represents signx + (signy<<1) + (signz << 1).
        /// <summary>
        /// 
        /// </summary>
        public byte Signbits; // signx + (signy<<1) + (signz<<1)
        /// <summary>
        /// 
        /// </summary>
        public byte[] Pad = { 0, 0 };

        /// <summary>
        /// Sets the specified c.
        /// </summary>
        /// <param name="c">The c.</param>
        public void Set(Plane3 c)
        {
            Math3D.Set(normal, c.normal);
            Distance = c.Distance;
            Type = c.Type;
            Signbits = c.Signbits;
            Pad[0] = c.Pad[0];
            Pad[1] = c.Pad[1];
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            Math3D.VectorClear(normal);
            Distance = 0;
            Type = 0;
            Signbits = 0;
            Pad[0] = 0;
            Pad[1] = 0;
        }
    }
}