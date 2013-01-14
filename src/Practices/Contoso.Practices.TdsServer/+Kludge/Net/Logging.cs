#region Foreign-License
// .Net40 Kludge
#endregion
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Reflection;
namespace System.Net
{
    [FriendAccessAllowed]
    internal class Logging
    {
        #region internal
        private static _MethodInfo _headerEncodingInfo = Type.GetType("System.Net.WebHeaderCollection+HeaderEncoding").GetMethod("GetString", BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(byte[]), typeof(int), typeof(int) }, null);
        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true, ExactSpelling = true)]
        internal static extern uint GetCurrentThreadId();
        #endregion
        private const string AttributeNameMaxSize = "maxdatasize";
        private const string AttributeNameTraceMode = "tracemode";
        private const string AttributeValueProtocolOnly = "protocolonly";
        private const int DefaultMaxDumpSize = 0x400;
        private const bool DefaultUseProtocolTextOnly = false;
        private static volatile bool s_AppDomainShutdown;
        private static TraceSource s_CacheTraceSource;
        private static TraceSource s_HttpListenerTraceSource;
        private static object s_InternalSyncObject;
        private static volatile bool s_LoggingEnabled = true;
        private static volatile bool s_LoggingInitialized;
        private static TraceSource s_SocketsTraceSource;
        private static TraceSource s_TraceSourceHttpName;
        private static TraceSource s_WebSocketsTraceSource;
        private static TraceSource s_WebTraceSource;
        private static readonly string[] SupportedAttributes = new string[] { "maxdatasize", "tracemode" };
        private const string TraceSourceCacheName = "System.Net.Cache";
        private const string TraceSourceHttpListenerName = "System.Net.HttpListener";
        private const string TraceSourceHttpName = "System.Net.Http";
        private const string TraceSourceSocketsName = "System.Net.Sockets";
        private const string TraceSourceWebName = "System.Net";
        private const string TraceSourceWebSocketsName = "System.Net.WebSockets";

        private Logging() { }

        private static void AppDomainUnloadEvent(object sender, EventArgs e)
        {
            Close();
            s_AppDomainShutdown = true;
        }

        [FriendAccessAllowed]
        internal static void Associate(TraceSource traceSource, object objA, object objB)
        {
            if (ValidateSettings(traceSource, TraceEventType.Information))
                PrintLine(traceSource, TraceEventType.Information, 0, "Associating " + (GetObjectName(objA) + "#" + ValidationHelper.HashString(objA)) + " with " + (GetObjectName(objB) + "#" + ValidationHelper.HashString(objB)));
        }

        private static void Close()
        {
            if (s_WebTraceSource != null)
                s_WebTraceSource.Close();
            if (s_HttpListenerTraceSource != null)
                s_HttpListenerTraceSource.Close();
            if (s_SocketsTraceSource != null)
                s_SocketsTraceSource.Close();
            if (s_WebSocketsTraceSource != null)
                s_WebSocketsTraceSource.Close();
            if (s_CacheTraceSource != null)
                s_CacheTraceSource.Close();
            if (s_TraceSourceHttpName != null)
                s_TraceSourceHttpName.Close();
        }

        internal static void Dump(TraceSource traceSource, object obj, string method, IntPtr bufferPtr, int length)
        {
            if ((ValidateSettings(traceSource, TraceEventType.Verbose) && (bufferPtr != IntPtr.Zero)) && (length >= 0))
            {
                byte[] destination = new byte[length];
                Marshal.Copy(bufferPtr, destination, 0, length);
                Dump(traceSource, obj, method, destination, 0, length);
            }
        }

        internal static void Dump(TraceSource traceSource, object obj, string method, byte[] buffer, int offset, int length)
        {
            if (ValidateSettings(traceSource, TraceEventType.Verbose))
                if (buffer == null)
                    PrintLine(traceSource, TraceEventType.Verbose, 0, "(null)");
                else if (offset > buffer.Length)
                    PrintLine(traceSource, TraceEventType.Verbose, 0, "(offset out of range)");
                else
                {
                    PrintLine(traceSource, TraceEventType.Verbose, 0, "Data from " + GetObjectName(obj) + "#" + ValidationHelper.HashString(obj) + "::" + method);
                    var maxDumpSizeSetting = GetMaxDumpSizeSetting(traceSource);
                    if (length > maxDumpSizeSetting)
                    {
                        PrintLine(traceSource, TraceEventType.Verbose, 0, "(printing " + maxDumpSizeSetting.ToString(NumberFormatInfo.InvariantInfo) + " out of " + length.ToString(NumberFormatInfo.InvariantInfo) + ")");
                        length = maxDumpSizeSetting;
                    }
                    if (length < 0 || length > (buffer.Length - offset))
                        length = buffer.Length - offset;
                    if (GetUseProtocolTextSetting(traceSource))
                    {
                        var msg = "<<" + (string)_headerEncodingInfo.Invoke(null, new object[] { buffer, offset, length }) + ">>";
                        PrintLine(traceSource, TraceEventType.Verbose, 0, msg);
                    }
                    else
                    {
                        do
                        {
                            var num2 = Math.Min(length, 0x10);
                            var str2 = string.Format(CultureInfo.CurrentCulture, "{0:X8} : ", new object[] { offset });
                            for (int i = 0; i < num2; i++)
                                str2 = str2 + string.Format(CultureInfo.CurrentCulture, "{0:X2}", new object[] { buffer[offset + i] }) + ((i == 7) ? '-' : ' ');
                            for (int j = num2; j < 0x10; j++)
                                str2 = str2 + "   ";
                            str2 = str2 + ": ";
                            for (int k = 0; k < num2; k++)
                                str2 = str2 + ((buffer[offset + k] < 0x20 || buffer[offset + k] > 0x7e ? '.' : ((char)buffer[offset + k])));
                            PrintLine(traceSource, TraceEventType.Verbose, 0, str2);
                            offset += num2;
                            length -= num2;
                        }
                        while (length > 0);
                    }
                }
        }

        internal static void Enter(TraceSource traceSource, string msg)
        {
            if (ValidateSettings(traceSource, TraceEventType.Information))
                PrintLine(traceSource, TraceEventType.Verbose, 0, msg);
        }

        internal static void Enter(TraceSource traceSource, string method, string parameters)
        {
            if (ValidateSettings(traceSource, TraceEventType.Information))
                Enter(traceSource, method + "(" + parameters + ")");
        }

        [FriendAccessAllowed]
        internal static void Enter(TraceSource traceSource, object obj, string method, object paramObject)
        {
            if (ValidateSettings(traceSource, TraceEventType.Information))
                Enter(traceSource, GetObjectName(obj) + "#" + ValidationHelper.HashString(obj), method, paramObject);
        }

        [FriendAccessAllowed]
        internal static void Enter(TraceSource traceSource, object obj, string method, string param)
        {
            if (ValidateSettings(traceSource, TraceEventType.Information))
                Enter(traceSource, GetObjectName(obj) + "#" + ValidationHelper.HashString(obj), method, param);
        }

        internal static void Enter(TraceSource traceSource, string obj, string method, object paramObject)
        {
            if (ValidateSettings(traceSource, TraceEventType.Information))
            {
                var str = string.Empty;
                if (paramObject != null)
                    str = GetObjectName(paramObject) + "#" + ValidationHelper.HashString(paramObject);
                Enter(traceSource, obj + "::" + method + "(" + str + ")");
            }
        }

        internal static void Enter(TraceSource traceSource, string obj, string method, string param)
        {
            if (ValidateSettings(traceSource, TraceEventType.Information))
                Enter(traceSource, obj + "::" + method + "(" + param + ")");
        }

        [FriendAccessAllowed]
        internal static void Exception(TraceSource traceSource, object obj, string method, System.Exception e)
        {
            if (ValidateSettings(traceSource, TraceEventType.Error))
            {
                var msg = SR.GetString(SR.net_log_exception, GetObjectLogHash(obj), method, e.Message);
                if (!ValidationHelper.IsBlankString(e.StackTrace))
                    msg = msg + "\r\n" + e.StackTrace;
                PrintLine(traceSource, TraceEventType.Error, 0, msg);
            }
        }

        internal static void Exit(TraceSource traceSource, string msg)
        {
            if (ValidateSettings(traceSource, TraceEventType.Information))
                PrintLine(traceSource, TraceEventType.Verbose, 0, "Exiting " + msg);
        }

        internal static void Exit(TraceSource traceSource, string method, string parameters)
        {
            if (ValidateSettings(traceSource, TraceEventType.Information))
                Exit(traceSource, method + "() " + parameters);
        }

        [FriendAccessAllowed]
        internal static void Exit(TraceSource traceSource, object obj, string method, object retObject)
        {
            if (ValidateSettings(traceSource, TraceEventType.Information))
            {
                var retValue = string.Empty;
                if (retObject != null)
                    retValue = GetObjectName(retObject) + "#" + ValidationHelper.HashString(retObject);
                Exit(traceSource, obj, method, retValue);
            }
        }

        [FriendAccessAllowed]
        internal static void Exit(TraceSource traceSource, object obj, string method, string retValue)
        {
            if (ValidateSettings(traceSource, TraceEventType.Information))
                Exit(traceSource, GetObjectName(obj) + "#" + ValidationHelper.HashString(obj), method, retValue);
        }

        internal static void Exit(TraceSource traceSource, string obj, string method, object retObject)
        {
            if (ValidateSettings(traceSource, TraceEventType.Information))
            {
                var retValue = string.Empty;
                if (retObject != null)
                    retValue = GetObjectName(retObject) + "#" + ValidationHelper.HashString(retObject);
                Exit(traceSource, obj, method, retValue);
            }
        }

        internal static void Exit(TraceSource traceSource, string obj, string method, string retValue)
        {
            if (ValidateSettings(traceSource, TraceEventType.Information))
            {
                if (!ValidationHelper.IsBlankString(retValue))
                    retValue = "\t-> " + retValue;
                Exit(traceSource, obj + "::" + method + "() " + retValue);
            }
        }

        private static int GetMaxDumpSizeSetting(TraceSource traceSource)
        {
            var num = 0x400;
            if (traceSource.Attributes.ContainsKey("maxdatasize"))
                try { num = int.Parse(traceSource.Attributes["maxdatasize"], NumberFormatInfo.InvariantInfo); }
                catch (System.Exception exception)
                {
                    if (exception is ThreadAbortException || exception is StackOverflowException || exception is OutOfMemoryException)
                        throw;
                    traceSource.Attributes["maxdatasize"] = num.ToString(NumberFormatInfo.InvariantInfo);
                }
            return num;
        }

        [FriendAccessAllowed]
        internal static string GetObjectLogHash(object obj)
        {
            return (GetObjectName(obj) + "#" + ValidationHelper.HashString(obj));
        }

        private static string GetObjectName(object obj)
        {
            if (obj is Uri || obj is IPAddress || obj is IPEndPoint)
                return obj.ToString();
            return obj.GetType().Name;
        }

        internal static uint GetThreadId()
        {
            var currentThreadId = GetCurrentThreadId();
            if (currentThreadId == 0)
                currentThreadId = (uint)Thread.CurrentThread.GetHashCode();
            return currentThreadId;
        }

        private static bool GetUseProtocolTextSetting(TraceSource traceSource)
        {
            var flag = false;
            if (traceSource.Attributes["tracemode"] == "protocolonly")
                flag = true;
            return flag;
        }

        private static void InitializeLogging()
        {
            lock (InternalSyncObject)
            {
                if (!s_LoggingInitialized)
                {
                    bool flag = false;
                    s_WebTraceSource = new NclTraceSource("System.Net");
                    s_HttpListenerTraceSource = new NclTraceSource("System.Net.HttpListener");
                    s_SocketsTraceSource = new NclTraceSource("System.Net.Sockets");
                    s_WebSocketsTraceSource = new NclTraceSource("System.Net.WebSockets");
                    s_CacheTraceSource = new NclTraceSource("System.Net.Cache");
                    s_TraceSourceHttpName = new NclTraceSource("System.Net.Http");
                    try
                    {
                        flag = (((s_WebTraceSource.Switch.ShouldTrace(TraceEventType.Critical) || s_HttpListenerTraceSource.Switch.ShouldTrace(TraceEventType.Critical)) || (s_SocketsTraceSource.Switch.ShouldTrace(TraceEventType.Critical) || s_WebSocketsTraceSource.Switch.ShouldTrace(TraceEventType.Critical))) || s_CacheTraceSource.Switch.ShouldTrace(TraceEventType.Critical)) || s_TraceSourceHttpName.Switch.ShouldTrace(TraceEventType.Critical);
                    }
                    catch (SecurityException)
                    {
                        Close();
                        flag = false;
                    }
                    if (flag)
                    {
                        var currentDomain = AppDomain.CurrentDomain;
                        currentDomain.UnhandledException += new UnhandledExceptionEventHandler(Logging.UnhandledExceptionHandler);
                        currentDomain.DomainUnload += new EventHandler(Logging.AppDomainUnloadEvent);
                        currentDomain.ProcessExit += new EventHandler(Logging.ProcessExitEvent);
                    }
                    s_LoggingEnabled = flag;
                    s_LoggingInitialized = true;
                }
            }
        }

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        internal static bool IsVerbose(TraceSource traceSource)
        {
            return ValidateSettings(traceSource, TraceEventType.Verbose);
        }

        [FriendAccessAllowed]
        internal static void PrintError(TraceSource traceSource, string msg)
        {
            if (ValidateSettings(traceSource, TraceEventType.Error))
                PrintLine(traceSource, TraceEventType.Error, 0, msg);
        }

        [FriendAccessAllowed]
        internal static void PrintError(TraceSource traceSource, object obj, string method, string msg)
        {
            if (ValidateSettings(traceSource, TraceEventType.Error))
                PrintLine(traceSource, TraceEventType.Error, 0, GetObjectName(obj) + "#" + ValidationHelper.HashString(obj) + "::" + method + "() - " + msg);
        }

        internal static void PrintInfo(TraceSource traceSource, string msg)
        {
            if (ValidateSettings(traceSource, TraceEventType.Information))
                PrintLine(traceSource, TraceEventType.Information, 0, msg);
        }

        [FriendAccessAllowed]
        internal static void PrintInfo(TraceSource traceSource, object obj, string msg)
        {
            if (ValidateSettings(traceSource, TraceEventType.Information))
                PrintLine(traceSource, TraceEventType.Information, 0, GetObjectName(obj) + "#" + ValidationHelper.HashString(obj) + " - " + msg);
        }

        internal static void PrintInfo(TraceSource traceSource, object obj, string method, string param)
        {
            if (ValidateSettings(traceSource, TraceEventType.Information))
                PrintLine(traceSource, TraceEventType.Information, 0, GetObjectName(obj) + "#" + ValidationHelper.HashString(obj) + "::" + method + "(" + param + ")");
        }

        internal static void PrintLine(TraceSource traceSource, TraceEventType eventType, int id, string msg)
        {
            traceSource.TraceEvent(eventType, id, ("[" + GetThreadId().ToString("d4", CultureInfo.InvariantCulture) + "] ") + msg);
        }

        [FriendAccessAllowed]
        internal static void PrintWarning(TraceSource traceSource, string msg)
        {
            if (ValidateSettings(traceSource, TraceEventType.Warning))
                PrintLine(traceSource, TraceEventType.Warning, 0, msg);
        }

        internal static void PrintWarning(TraceSource traceSource, object obj, string method, string msg)
        {
            if (ValidateSettings(traceSource, TraceEventType.Warning))
                PrintLine(traceSource, TraceEventType.Warning, 0, GetObjectName(obj) + "#" + ValidationHelper.HashString(obj) + "::" + method + "() - " + msg);
        }

        private static void ProcessExitEvent(object sender, EventArgs e)
        {
            Close();
            s_AppDomainShutdown = true;
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            var exceptionObject = (Exception)args.ExceptionObject;
            Exception(Web, sender, "UnhandledExceptionHandler", exceptionObject);
        }

        private static bool ValidateSettings(TraceSource traceSource, TraceEventType traceLevel)
        {
            if (!s_LoggingEnabled)
                return false;
            if (!s_LoggingInitialized)
                InitializeLogging();
            if (traceSource == null || !traceSource.Switch.ShouldTrace(traceLevel))
                return false;
            if (s_AppDomainShutdown)
                return false;
            return true;
        }

        [FriendAccessAllowed]
        internal static TraceSource Http
        {
            get
            {
                if (!s_LoggingInitialized)
                    InitializeLogging();
                if (!s_LoggingEnabled)
                    return null;
                return s_TraceSourceHttpName;
            }
        }

        internal static TraceSource HttpListener
        {
            get
            {
                if (!s_LoggingInitialized)
                    InitializeLogging();
                if (!s_LoggingEnabled)
                    return null;
                return s_HttpListenerTraceSource;
            }
        }

        private static object InternalSyncObject
        {
            get
            {
                if (s_InternalSyncObject == null)
                {
                    object obj2 = new object();
                    Interlocked.CompareExchange(ref s_InternalSyncObject, obj2, null);
                }
                return s_InternalSyncObject;
            }
        }

        [FriendAccessAllowed]
        internal static bool On
        {
            get
            {
                if (!s_LoggingInitialized)
                    InitializeLogging();
                return s_LoggingEnabled;
            }
        }

        internal static TraceSource RequestCache
        {
            get
            {
                if (!s_LoggingInitialized)
                    InitializeLogging();
                if (!s_LoggingEnabled)
                    return null;
                return s_CacheTraceSource;
            }
        }

        internal static TraceSource Sockets
        {
            get
            {
                if (!s_LoggingInitialized)
                    InitializeLogging();
                if (!s_LoggingEnabled)
                    return null;
                return s_SocketsTraceSource;
            }
        }

        internal static TraceSource Web
        {
            get
            {
                if (!s_LoggingInitialized)
                    InitializeLogging();
                if (!s_LoggingEnabled)
                    return null;
                return s_WebTraceSource;
            }
        }

        internal static TraceSource WebSockets
        {
            get
            {
                if (!s_LoggingInitialized)
                    InitializeLogging();
                if (!s_LoggingEnabled)
                    return null;
                return s_WebSocketsTraceSource;
            }
        }

        private class NclTraceSource : TraceSource
        {
            internal NclTraceSource(string name)
                : base(name) { }
            protected override string[] GetSupportedAttributes() { return Logging.SupportedAttributes; }
        }
    }
}

