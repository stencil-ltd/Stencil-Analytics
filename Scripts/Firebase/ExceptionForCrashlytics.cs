using System;
using System.Diagnostics;

namespace Stencil.Analytics.Firebase
{
    public class ExceptionForCrashlytics : Exception
    {
        public override string StackTrace { get; }

        public ExceptionForCrashlytics(string message, string stackTrace) : base(message)
        {
            StackTrace = stackTrace;
        }
    }
}