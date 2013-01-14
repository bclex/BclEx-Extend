//using System.Text;
//namespace System.Net
//{
//    public class SimpleTdsListenerContext : TdsListenerContext
//    {
//        public DummyTdsListenerWorkerRequest(TdsListenerContext context)
//            : base(context)
//        {
//            var b = new StringBuilder();
//            b.Append("<html><body>");
//            DumpRequest(Context.Request, b);
//            b.Append("</body></html>");
//            //
//            var bytes = Encoding.UTF8.GetBytes(b.ToString());
//            var r = Context.Response;
//            r.ContentLength64 = bytes.Length;
//            r.OutputStream.Write(bytes, 0, bytes.Length);
//            r.OutputStream.Close();
//        }

//        private void DumpRequest(TdsListenerRequest r, StringBuilder b)
//        {
//            b.Append("<h1>");
//            b.Append(r.HttpMethod + " " + r.Url); b.Append("</h1>");
//            b.Append("Test");
//        }
//    }
//}
