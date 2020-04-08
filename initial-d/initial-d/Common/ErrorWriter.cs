using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace initial_d.Common
{
    public class ErrorWriter
    {
        public static void InvalidArgumentsError([CallerMemberName] string callerName = "")
        {
            Debug.WriteLine($"ERROR in {callerName} : Invalid Arguments");
        }
        public static void ExceptionError(Exception exception, [CallerMemberName] string callerName = "")
        {
            Debug.WriteLine($"ERROR in {callerName} : {exception.Message}");
        }

    }
}