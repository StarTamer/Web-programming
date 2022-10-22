using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text.Json;
using System.Reflection;
using httpserver.attributes;

namespace httpserver
{
    public class HttpServer : IDisposable
    {
        //public ServerStatus Status = ServerStatus
        private ServerSettings serverSettings;
        public HttpListener _listener;
        private readonly string Path = @"./STEAM";

        public void Start()
        {
            _listener = new HttpListener();

            serverSettings = JsonSerializer.Deserialize<ServerSettings>(File.ReadAllBytes("./settings.json"));

            _listener.Prefixes.Clear();
            _listener.Prefixes.Add($"http://localhost:{serverSettings.Port}/STEAM/");
            _listener.Start();
            Console.WriteLine("Ожидание подключений...");
            Receive();
            
        }

        public void Stop()
        {
            _listener.Stop();
            Console.WriteLine("Обработка подключений завершена");
        }

        private void Receive()
        {
            _listener.BeginGetContext(new AsyncCallback(ListenerCallback), _listener);
        }

        private void ListenerCallback(IAsyncResult result)
        {
            if (_listener.IsListening)
            {
                
                var context = _listener.EndGetContext(result);
                var request = context.Request;



                // do something with the request
                Console.WriteLine($"{request.Url}");
                Console.WriteLine($"{request.HttpMethod} {request.Url}");

                if (request.HasEntityBody)
                {
                    var body = request.InputStream;
                    var encoding = request.ContentEncoding;
                    var reader = new StreamReader(body, encoding);
                    if (request.ContentType != null)
                    {
                        Console.WriteLine("Client data content type {0}", request.ContentType);
                    }
                    Console.WriteLine("Client data content length {0}", request.ContentLength64);

                    Console.WriteLine("Start of data:");
                    string s = reader.ReadToEnd();
                    Console.WriteLine(s);
                    Console.WriteLine("End of data:");
                    reader.Close();
                    body.Close();
                }

                var response = context.Response;

/*                var exePath = AppDomain.CurrentDomain.BaseDirectory;//path to exe file
                var path = Path.Combine(exePath, "STEAM");
                var file = new StreamReader(path);


                string responseStr = file.ReadToEnd();*/
                byte[] buffer;// = File.ReadAllBytes("STEAM/index.html");//System.Text.Encoding.UTF8.GetBytes(responseStr);

                if (Directory.Exists(Path))
                {
                    buffer = getFile(context.Request.RawUrl.Replace("%20", " "));

                    if (buffer == null)
                    {
                        response.Headers.Set("Content-Type", "text/plain");

                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        string err = "404 - not found";

                        buffer = Encoding.UTF8.GetBytes(err);
                    }
                }
                else
                {
                    string err = $"Directory '{Path}' not found";

                    buffer = Encoding.UTF8.GetBytes(err);
                }


                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();

                Receive();
            }
        }

        public byte[] getFile(string rawUrl)
        {
            byte[] buffer = null;
            var filePath = Path;

            if (Directory.Exists(filePath))
            {
                filePath = filePath + "/index.html";
                if (File.Exists(filePath))
                {
                    buffer = File.ReadAllBytes(filePath);
                }
            }
            else if (File.Exists(filePath))
            {
                buffer = File.ReadAllBytes(filePath);
            }

            return buffer;
        }

        public void Dispose()
        {
            Stop();
        }

        private bool MethodHandler(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            if (request.Url.Segments.Length < 2) return false;

            string controllerName = request.Url.Segments[1].Replace("/", "");
            string[] strParams = request.Url
                .Segments
                .Skip(2)
                .Select(s => s.Replace("/", ""))
                .ToArray();

            var assembly = Assembly.GetExecutingAssembly();
            var controller = assembly.GetTypes()
                .Where(t => Attribute.IsDefined(t, typeof(HttpController)))
                .FirstOrDefault(c => c.Name.ToLower() == controllerName.ToLower());

            if (controller == null) return false;

            var test = typeof(HttpController).Name;
            var method = controller.GetMethods()
                .Where(t => t.GetCustomAttributes(true)
                    .Any(attr => attr.GetType().Name == $"Http{context.Request.HttpMethod}"))
                .FirstOrDefault();

            if (method == null) return false;

            object[] queryParams = method.GetParameters()
                .Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
                .ToArray();

            var ret = method.Invoke(Activator.CreateInstance(controller), queryParams);
            response.ContentType = "Application/json";
            byte[] buffer = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(ret));
            response.ContentLength64 = buffer.Length;

            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);

            output.Close();

            return true;
        }
    }
}
