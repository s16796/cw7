using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace cw6.Middlewares
{
    public class LoggerMidWare
    {
        private readonly RequestDelegate _next;
        public LoggerMidWare(RequestDelegate next) { _next = next; }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            httpContext.Request.EnableBuffering();

            if (httpContext.Request != null)
            {
                string path = httpContext.Request.Path;
                string query = httpContext.Request?.QueryString.ToString();
                string method = httpContext.Request.Method.ToString();
                string bodyStr = "";

                using (StreamReader reader
                 = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    bodyStr = await reader.ReadToEndAsync();
                    httpContext.Request.Body.Position = 0;
                }

                using (var writer = new StreamWriter("apilog.log", true))
                {
                    writer.WriteLine("API request:");
                    writer.WriteLine(DateTime.Now + " Method: " + method);
                    writer.WriteLine(DateTime.Now + " Path: " + path);
                    writer.WriteLine(DateTime.Now + " Query: " + query);
                    writer.WriteLine(DateTime.Now + " BodyStr: " + bodyStr);
                    writer.WriteLine("END OF REQUEST\n");
                }

            }

            await _next(httpContext);
        }
    }
}
