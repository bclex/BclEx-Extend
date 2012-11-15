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
using System.Net.Mail;
namespace Contoso.Net
{
	/// <summary>
    /// ShortMessageServiceCarrierID
	/// </summary>
    public enum ShortMessageServiceCarrierID
	{
		/// <summary>
		/// Verizon - vtext.com
		/// </summary>
        Verizon,
		/// <summary>
		/// Sprint - messaging.sprintpcs.com
		/// </summary>
		Sprint,
		/// <summary>
		/// AT&amp;T - txt.att.net
		/// </summary>
		ATT,
		/// <summary>
		/// T-Mobile - tmomail.net
		/// </summary>
		TMobile,
		/// <summary>
		/// AllTel - message.alltel.com
		/// </summary>
		AllTel,
		/// <summary>
		/// Cricket - mms.mycricket.com
		/// </summary>
		Cricket,
		/// <summary>
		/// Cingular - mobile.mycingular.com
		/// </summary>
		Cingular,
		/// <summary>
		/// Nextel - messaging.nextel.com
		/// </summary>
		Nextel,
		/// <summary>
		/// Unicel - utext.com
		/// </summary>
		Unicel,
		/// <summary>
		/// Virgin Mobile - vmobl.com
		/// </summary>
		VirginMobile,
		/// <summary>
		/// Northwest Missouri Cellular - mynwmcell.com
		/// </summary>
		NorthwestMissouriCellular,
		/// <summary>
		/// US Cellular - email.uscc.net
		/// </summary>
		USCellular,
	}
}