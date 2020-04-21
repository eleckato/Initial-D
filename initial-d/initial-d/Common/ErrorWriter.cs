using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.IO;

namespace initial_d.Common
{
    /// <summary>
    /// Write error on console referencing the method caller and the file
    /// </summary>
    public class ErrorWriter
    {
        /// <summary>
        /// Print a Invalid Arguments error on the Debug Console, with the caller name and the file of it
        /// <para> callerName and callerFilePath are filled automatically </para>
        /// </summary>
        public static void InvalidArgumentsError([CallerMemberName] string callerName = "", [CallerFilePath]string callerFilePath = null)
        {
            var callerFile = Path.GetFileName(callerFilePath);

            Debug.WriteLine("----------------------------------------------------------------");
            Debug.WriteLine($"ERROR in {callerName}()  :  Invalid Arguments\non file  :  {callerFile}");
            Debug.WriteLine("----------------------------------------------------------------");
        }

        /// <summary>
        /// Print the message of an Exception on the Debug Console, with the caller name and the file of it
        /// <para> callerName and callerFilePath are filled automatically </para>
        /// </summary>
        /// <param name="exception"> Exception from where to extract the message </param>
        public static void ExceptionError(Exception exception, [CallerMemberName] string callerName = "", [CallerFilePath]string callerFilePath = null)
        {
            var callerFile = Path.GetFileName(callerFilePath);

            Debug.WriteLine("----------------------------------------------------------------");
            Debug.WriteLine($"ERROR in {callerName}()  :  {exception.Message}\non file  :  {callerFile}");
            Debug.WriteLine("----------------------------------------------------------------");
        }

        /// <summary>
        /// Print a custom message on the Debug Console, with the caller name and the file of it
        /// <para> callerName and callerFilePath are filled automatically </para>
        /// </summary>
        /// <param name="message"> Error message to write </param>
        public static void CustomError(string message, [CallerMemberName] string callerName = "", [CallerFilePath]string callerFilePath = null)
        {
            var callerFile = Path.GetFileName(callerFilePath);

            Debug.WriteLine("----------------------------------------------------------------");
            Debug.WriteLine($"ERROR in {callerName}()  :  {message}\non file  :  {callerFile}");
            Debug.WriteLine("----------------------------------------------------------------");
        }

    }
}