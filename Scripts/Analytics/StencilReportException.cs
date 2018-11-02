using System;
using JetBrains.Annotations;

namespace Analytics
{
    public class StencilReportException : Exception
    {
        [CanBeNull] private string _customStackTrace;
        
        public StencilReportException(string message) : base(message)
        {
        }

        public StencilReportException(string message, [CanBeNull] string customStackTrace) : base(message)
        {
            _customStackTrace = customStackTrace;
        }

        public override string StackTrace => _customStackTrace ?? base.StackTrace;
    }
}