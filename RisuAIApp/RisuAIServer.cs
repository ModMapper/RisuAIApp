namespace RisuAIApp;
using EmbedIO;

using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

public class RisuAIServer {
    private readonly string PasswordFile;

    private WebServer Server { get; set; }

    public string URL { get; private set; }

    public string Password { get; private set; } = string.Empty;

    public DirectoryInfo SavePath { get; }


    public RisuAIServer() {
        string document = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        SavePath = new(Path.Combine(document, AppInfo.Name));
        PasswordFile = Path.Combine(SavePath.FullName, "_password");
    }

    public void Start() {
        SavePath.Create();
        if(File.Exists(PasswordFile)) {
            Password = File.ReadAllText(PasswordFile);
        }

        int port = 7866;
        if(!RisuAIServer.CheckPort(port)) {
            port = GetAvailablePort();
        }
        URL = $"http://localhost:{port}/";

        Server = new(URL);
        Server.OnAny(HttpHandler);
        Server.Start();
    }

    private async Task HttpHandler(IHttpContext context) {
        string url = context.Request.Url.AbsolutePath;
        RisuAIController controller = new(this, context);
        try {
            await (url switch {
                "/proxy" => controller.Proxy(),
                "/proxy2" => controller.Proxy(),
                "/api/password" => controller.CheckPassword(),
                "/api/set_password" => controller.SetPassword(),
                "/api/crypto" => controller.CryptHash(),
                "/api/read" => controller.ReadFile(),
                "/api/remove" => controller.RemoveFile(),
                "/api/list" => controller.ListFiles(),
                "/api/write" => controller.WriteFile(),
                _ => controller.GetFile()
            });
        } catch (Exception ex) {
            context.Response.StatusCode = 500;
            await context.SendStringAsync(ex.ToString(), MimeType.PlainText, Encoding.UTF8);
        }
        context.Response.Close();
    }

    public void SetPassword(string password) {
        lock(PasswordFile) {
            Password = password;
            File.WriteAllText(PasswordFile, Password);
        }
    }

    private static bool CheckPort(int port) {
        try {
            using (Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)) {
                socket.Bind(new IPEndPoint(IPAddress.Loopback, port));
            }
            return true;
        } catch {
            return false;
        }
    }

    private static int GetAvailablePort() {
        using (Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)) {
            socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
            return ((IPEndPoint)socket.LocalEndPoint).Port;
        }
    }
}
