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
//using System.Collections;
//using System.Linq;
//namespace System.ComponentModel.DataAnnotations
//{
//    /// <summary>
//    /// RequiredIfAttribute
//    /// </summary>
//    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
//    public class RequiredIfAttribute : ValidationAttribute
//    {
//        private readonly string _condition;

//        public RequiredIfAttribute(string condition)
//        {
//            _condition = condition;
//        }

//        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
//        {
//            var conditionFunction = CreateExpression(validationContext.ObjectType, _condition);
//            return ((bool)conditionFunction.DynamicInvoke(validationContext.ObjectInstance) && value == null ? new ValidationResult(FormatErrorMessage(null)) : null);
//        }

//        private Delegate CreateExpression(Type objectType, string expression)
//        {
//            var lambdaExpression = DynamicExpression.ParseLambda(objectType, typeof(bool), expression);
//            return lambdaExpression.Compile();
//        }
//    }
//}
