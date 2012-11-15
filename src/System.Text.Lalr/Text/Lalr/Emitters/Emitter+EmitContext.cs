using System.Collections.Generic;
using System.Diagnostics;
using System.CodeDom;

namespace System.Text.Lalr.Emitters
{
    public partial class Emitter
    {
        private static Type GetMinimumSizeType(int minValue, int maxValue, out Type arrayType)
        {
            if (minValue >= 0)
            {
                if (maxValue <= 255)
                {
                    arrayType = typeof(byte[]);
                    return typeof(byte);
                }
                else if (maxValue < 65535)
                {
                    arrayType = typeof(ushort[]);
                    return typeof(ushort);
                }
                else
                {
                    arrayType = typeof(uint[]);
                    return typeof(uint);
                }
            }
            else if (minValue >= -127 && maxValue <= 127)
            {
                arrayType = typeof(sbyte[]);
                return typeof(sbyte);
            }
            else if (minValue >= -32767 && maxValue < 32767)
            {
                arrayType = typeof(short[]);
                return typeof(short);
            }
            else
            {
                arrayType = typeof(int[]);
                return typeof(int);
            }
        }

        private class EmitContext
        {
            public Type ACTIONTYPE;
            public Type CODETYPE;
            public CodeTypeReference MINORTYPE = new CodeTypeReference("Minor");
            public string TOKENTYPE;
            //
            public int NOCODE; public CodeVariableReferenceExpression pNOCODE = new CodeVariableReferenceExpression("NOCODE");
            public int WILDCARD; public CodeVariableReferenceExpression pWILDCARD = new CodeVariableReferenceExpression("WILDCARD");
            public int STATES; public CodeVariableReferenceExpression pSTATES = new CodeVariableReferenceExpression("STATES");
            public int RULES;
            public int ERRORSYMBOL; public CodeVariableReferenceExpression pERRORSYMBOL = new CodeVariableReferenceExpression("STATES");
            //public string ERRSYMDT;
            //public bool ALLBACK;
            public bool TRACKMAXSTACKDEPTH = false;
            public int STACKDEPTH; public CodePrimitiveExpression pSTACKDEPTH;
            public bool NDEBUG = false;
            public bool FALLBACK;
            //
            public int ACTIONS; public CodeVariableReferenceExpression pACTIONS = new CodeVariableReferenceExpression("ACTIONS");
            public int SHIFT_COUNT; public CodeVariableReferenceExpression pSHIFT_COUNT = new CodeVariableReferenceExpression("SHIFT_COUNT");
            public int SHIFT_USE_DFLT; public CodeVariableReferenceExpression pSHIFT_USE_DFLT = new CodeVariableReferenceExpression("SHIFT_USE_DFLT");
            public int SHIFT_MIN;
            public int SHIFT_MAX;
            public int REDUCE_COUNT; public CodeVariableReferenceExpression pREDUCE_COUNT = new CodeVariableReferenceExpression("REDUCE_COUNT");
            public int REDUCE_USE_DFLT; public CodeVariableReferenceExpression pREDUCE_USE_DFLT = new CodeVariableReferenceExpression("REDUCE_USE_DFLT");
            public int REDUCE_MIN;
            public int REDUCE_MAX;

            public string ParseArg;
            public string ParseArgType;
            public Action<CodeStatementCollection> ParseARG_STORE;
            public Action<CodeStatementCollection> ParseARG_FETCH;
            public CodeVariableReferenceExpression pNO_ACTION = new CodeVariableReferenceExpression("NO_ACTION");
            public CodeVariableReferenceExpression pACCEPT_ACTION = new CodeVariableReferenceExpression("ACCEPT_ACTION");
            public CodeVariableReferenceExpression pERROR_ACTION = new CodeVariableReferenceExpression("ERROR_ACTION");
            public bool NOERRORRECOVERY = false;

            public EmitContext(Context ctx, CodeTypeDeclaration target)
            {
                TOKENTYPE = FixupType(ctx.TokenType ?? "object");
                Type codeArrayType;
                CODETYPE = GetMinimumSizeType(0, ctx.Symbols.Length, out codeArrayType);
                Type actionArrayType;
                ACTIONTYPE = GetMinimumSizeType(0, ctx.States + ctx.Rules + 5, out actionArrayType);
                NOCODE = ctx.Symbols.Length;
                if (ctx.Wildcard != null)
                    WILDCARD = ctx.Wildcard.ID;
                STACKDEPTH = (ctx.StackSize != null ? int.Parse(ctx.StackSize) : 100);
                var extraArg = ctx.ExtraArg;
                if (!string.IsNullOrEmpty(extraArg))
                {
                    var i = extraArg.Length;
                    while (i >= 1 && char.IsWhiteSpace(extraArg[i - 1])) i--;
                    while (i >= 1 && (char.IsLetterOrDigit(extraArg[i - 1]) || extraArg[i - 1] == '_')) i--;
                    ParseArg = extraArg.Substring(i);
                    ParseArgType = extraArg.Substring(0, i - 1);
                    ParseARG_FETCH = s => s.Add(new CodeAssignStatement(new CodeVariableReferenceExpression(extraArg), new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_" + ParseArg)));
                    ParseARG_STORE = s => s.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_" + ParseArg), new CodeVariableReferenceExpression(ParseArg)));
                }
                STATES = ctx.States;
                RULES = ctx.Rules;
                if (ctx.ErrorSymbol.Uses > 0)
                {
                    ERRORSYMBOL = ctx.ErrorSymbol.ID;
                    //ERRSYMDT = "yy" + ctx.ErrorSymbol.DataTypeID.ToString();
                }
                if (ctx.HasFallback)
                    FALLBACK = true;
                //
                //pNOCODE = new CodePrimitiveExpression(NOCODE);
                //pWILDCARD = new CodePrimitiveExpression(WILDCARD);
                //pSTATES = new CodePrimitiveExpression(STATES);
                //pERRORSYMBOL = new CodePrimitiveExpression(ERRORSYMBOL);
                pSTACKDEPTH = new CodePrimitiveExpression(STACKDEPTH);
                //
                target.Members.Add(new CodeMemberField(typeof(int), "NOCODE") { Attributes = MemberAttributes.Family | MemberAttributes.Const, InitExpression = new CodePrimitiveExpression(NOCODE) });
                target.Members.Add(new CodeMemberField(typeof(int), "WILDCARD") { Attributes = MemberAttributes.Family | MemberAttributes.Const, InitExpression = new CodePrimitiveExpression(WILDCARD) });
                target.Members.Add(new CodeMemberField(typeof(int), "STATES") { Attributes = MemberAttributes.Family | MemberAttributes.Const, InitExpression = new CodePrimitiveExpression(STATES) });
                target.Members.Add(new CodeMemberField(typeof(int), "ERRORSYMBOL") { Attributes = MemberAttributes.Family | MemberAttributes.Const, InitExpression = new CodePrimitiveExpression(ERRORSYMBOL) });

            }

            public static string FixupType(string type)
            {
                switch (type)
                {
                    case "int": return "Int32";
                    case "object": return "Object";
                    default: return type;
                }
            }
        }
    }
}
