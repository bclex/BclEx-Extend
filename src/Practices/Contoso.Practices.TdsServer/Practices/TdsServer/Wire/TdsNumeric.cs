namespace Contoso.Practices.TdsServer.Wire
{
    public struct TdsNumeric
    {
        public byte Precision;
        public byte Scale;
        public byte[] Array = new byte[33];
    }
}

