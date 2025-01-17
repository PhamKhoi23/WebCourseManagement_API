using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCourseManagement2_Models.RequestModels.CodeInputRequests;
using WebCourseManagement2_Models.ResponseModels;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace WebCourseManagement2_Business.Implements
{
    public class ToolCompilerService
    {
        public static async Task<DataResultCode> RunCodeCSharp(string code)
        {
            try
            {
                string input = FixConsoleWriteLine(code);
                var script = CSharpScript.Create(input, ScriptOptions.Default.WithImports("System", "System.IO", "System.Diagnostics", "System.Collections.Generic"));
                var result = await script.RunAsync();
                string runResult = result.ReturnValue?.ToString() ?? "";
                if (runResult.Contains("ERROR###"))
                {
                    return new DataResultCode
                    {
                        Success = false,
                        Exception = "The code took too long to execute and has been terminated!."
                    };
                }
                return new DataResultCode
                {
                    Success = true,
                    Result = runResult.Split("RUN###")[0],
                    Time = runResult.Split("RUN###")[1] + "ms"
                };
            }
            catch (CompilationErrorException ex)
            {
                return new DataResultCode
                {
                    Success = false,
                    Exception = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new DataResultCode
                {
                    Success = false,
                    Exception = ex.Message
                };
            }
        }

        public static string FixConsoleWriteLine(string code)
        {
            code = "StringWriter stringWriter = new StringWriter(); Stopwatch stopwatch = new Stopwatch();stopwatch.Start();" +
                "TimeSpan timeSpan  = new TimeSpan();" + code;
            code = code.Replace("Console.Write", "stringWriter.Write");
            code = code.Replace("Console.WriteLine", "stringWriter.WriteLine");
            code += "stopwatch.Stop(); Console.WriteLine(\"RUN###\" + timeSpan.TotalMilliseconds); return stringWriter.ToString();";
            code = code.Replace("Console.WriteLine", "stringWriter.WriteLine");
            return code;
        }
        private static string FixLoopForever(string code)
        {
            code = code.Replace("\n", "");
            code = code.Replace("\t", "");
            code = code.Replace("\r", "");
            while (code.Contains("do "))
            {
                code = code.Replace("do ", "do");
            }
            code = code.Replace("do{", "do{timeSpan = stopwatch.Elapsed;if (timeSpan.TotalMilliseconds > 3000) { Console.WriteLine(\"ERROR###\");break;}");
            string result = "";
            //int index = code.IndexOf("while");
            //for(int i=0;i<code.Length; i++)
            //{

            //}
            return code;
        }
        /// <summary>
        /// Chưa tìm được cách sử dụng khi call API chạy console thì rơi vào OperationCanceledException nên có thể sử lý được nhưng ko hiểu sao ko thể dùng được khi ở dạng call API 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="timeoutMilliseconds"></param>
        /// <returns></returns>
        /// <exception cref="OperationCanceledException"></exception>
        public static async Task<DataResultCode> RunCodeCSharp(string code, int timeoutMilliseconds)
        {
            var cts = new CancellationTokenSource(timeoutMilliseconds);

            try
            {
                var script = CSharpScript.Create(FixConsoleWriteLine(code), ScriptOptions.Default.WithImports("System", "System.IO", "System.Diagnostics"));
                var runTask = script.RunAsync(cancellationToken: cts.Token);
                var completedTask = await Task.WhenAny(runTask, Task.Delay(timeoutMilliseconds, cts.Token));
                if (runTask != completedTask)
                {

                    cts.Cancel();
                    return new DataResultCode
                    {
                        Success = false,
                        Exception = "The code took too long to execute and has been terminated."
                    };
                }
                string runResult = runTask.Result.ReturnValue?.ToString() ?? "";
                return new DataResultCode
                {
                    Success = true,
                    Result = runResult.Split("RUN###")[0],
                    Time = runResult.Split("RUN###")[1] + "ms"
                };
            }
            catch (AggregateException ex)
            {
                return new DataResultCode
                {
                    Success = false,
                    Exception = "The code took too long to execute and has been terminated."
                };
            }
            catch (ArgumentNullException ex)
            {

                return new DataResultCode
                {
                    Success = false,
                    Exception = "The code took too long to execute and has been terminated."
                };
            }
            catch (ArgumentOutOfRangeException ex)
            {

                return new DataResultCode
                {
                    Success = false,
                    Exception = "The code took too long to execute and has been terminated."
                };
            }
            catch (ObjectDisposedException ex)
            {

                return new DataResultCode
                {
                    Success = false,
                    Exception = "The code took too long to execute and has been terminated."
                };
            }
            catch (TaskCanceledException ex)
            {

                return new DataResultCode
                {
                    Success = false,
                    Exception = "The code took too long to execute and has been terminated."
                };
            }
            catch (ArgumentException ex)
            {

                return new DataResultCode
                {
                    Success = false,
                    Exception = "The code took too long to execute and has been terminated."
                };
            }
            catch (OperationCanceledException ex)
            {
                throw new OperationCanceledException("");

            }
            catch (CompilationErrorException ex)
            {
                return new DataResultCode
                {
                    Success = false,
                    Exception = "The code took too long to execute and has been terminated."
                };
            }
            catch (Exception ex)
            {

                return new DataResultCode
                {
                    Success = false,
                    Exception = ex.Message
                };
            }
        }
    }
}
