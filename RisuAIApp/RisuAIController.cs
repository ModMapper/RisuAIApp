namespace RisuAIApp;
using EmbedIO;

using RisuAIApp.Models;

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public partial struct RisuAIController {
    private static readonly HttpClient http = new();

    public RisuAIController(RisuAIServer server, IHttpContext context) {
        Server = server;
        HttpContext = context;
    }

    public RisuAIServer Server { get; }

    public IHttpContext HttpContext { get; }

    public IHttpRequest Request => HttpContext.Request;

    public IHttpResponse Response => HttpContext.Response;

    
    public string Auth => Request.Headers["risu-auth"] ?? string.Empty;

    private async Task<string> GetJson(string name) {
        using var stream = HttpContext.OpenRequestStream();
        JsonDocument doc = await JsonDocument.ParseAsync(stream);
        if (doc.RootElement.TryGetProperty(name, out var prop)) {
            return prop.GetString();
        }
        return null;
    }

    private Task SendError(string msg) {
        Response.StatusCode = 400;
        return HttpContext.SendDataAsync(new { error = msg });
    }

    private Task<bool> CheckMethod(string method) {
        return Task.FromResult(true);    // 생략
    }

    private Task<bool> CheckAuth() {
        return Task.FromResult(true);    // 생략
        /*
        if (Auth.Trim() != Server.Password.Trim()) {
            await SendError("Password Incorrect");
            return false;
        }
        return true;
        */
    }

    private async Task<string> GetPath() {
        string filename = Request.Headers["file-path"];
        if (string.IsNullOrEmpty(filename)) {
            await SendError("File path required");
            return null;
        }

        if (!IsHex(filename)) {
            await SendError("Invalid Path");
            return null;
        }
        return filename;
    }

    private static bool IsHex(string filePath) {
        return HexRegex().IsMatch(filePath);
    }

    [GeneratedRegex("^[0-9A-Fa-f]+$")]
    private static partial Regex HexRegex();


    public async Task Proxy() {
        string url = Uri.UnescapeDataString(Request.Headers["risu-url"] ?? string.Empty);
        if (string.IsNullOrEmpty(url)) {
            await SendError("URL has no param");
            return;
        }
        HttpRequestMessage req = new(new(Request.HttpMethod), url);

        string headerString = Uri.UnescapeDataString(Request.Headers["risu-header"] ?? string.Empty);
        if (string.IsNullOrEmpty(url)) {
            foreach (string name in Request.Headers.Keys) {
                req.Headers.TryAddWithoutValidation(name, Request.Headers.GetValues(name));
            }
        } else {
            using JsonDocument doc = JsonDocument.Parse(headerString);
            foreach (var header in doc.RootElement.EnumerateObject()) {
                req.Headers.TryAddWithoutValidation(header.Name, header.Value.GetString());
            }
        }

        req.Content = new StreamContent(Request.InputStream);
        req.Content.Headers.Add("content-type", "application/json");

        using HttpResponseMessage res = await http.SendAsync(req);
        Response.StatusCode = (int)res.StatusCode;

        foreach (var header in res.Headers) {
            foreach (var value in header.Value) {
                Response.Headers.Add(header.Key, value);
            }
        }
        Response.Headers.Remove("content-security-policy");
        Response.Headers.Remove("content-security-policy-report-only");
        Response.Headers.Remove("clear-site-data");
        Response.Headers.Remove("Cache-Control");

        using var stream = HttpContext.OpenResponseStream(); 
        await res.Content.CopyToAsync(stream);
    }

    public async Task CheckPassword() {
        if (!await CheckMethod("POST")) return;
        /*
        string status;
        if (Server.Password.Length == 0) {
            status = "unset";
        } else if (Auth == Server.Password) {
            status = "correct";
        } else {
            status = "incorrect";
        }
        */
        string status = "correct";
        await HttpContext.SendDataAsync(new { status });
    }

    public async Task SetPassword() {
        if (!await CheckMethod("POST")) return;
        if (Server.Password.Length == 0) {
            Server.SetPassword(await GetJson("password") ?? string.Empty);
        }
        Response.StatusCode = 400;
        await HttpContext.SendStringAsync("already set", "text/plain", Encoding.UTF8);
    }

    public async Task CryptHash() {
        if (!await CheckMethod("POST")) return;
        using (SHA256 sha256 = SHA256.Create()) {
            using var stream = HttpContext.OpenRequestStream();
            string hash = Convert.ToHexString(await SHA256.HashDataAsync(stream));
            await HttpContext.SendStringAsync(hash, "text/plain", Encoding.UTF8);
        }
    }

    public async Task ReadFile() {
        if (!await CheckMethod("GET")) return;
        if (!await CheckAuth()) return;

        string filename = await GetPath();
        if (filename == null) return;

        string path = Path.Combine(Server.SavePath.FullName, filename);
        string content = null;
        if(File.Exists(path)) {
            content = Convert.ToBase64String(await File.ReadAllBytesAsync(path));
        }
        await HttpContext.SendDataAsync(new { success = true, content });
    }

    public async Task RemoveFile() {
        if (!await CheckMethod("GET")) return;
        if (!await CheckAuth()) return;

        string filename = await GetPath();
        if (filename == null) return;

        File.Delete(Path.Combine(Server.SavePath.FullName, filename));
        await HttpContext.SendDataAsync(new { success = true });
    }

    public async Task ListFiles() {
        if (!await CheckMethod("GET")) return;
        if (!await CheckAuth()) return;

        string[] files = Server.SavePath.EnumerateFiles().Select((file) => file.Name).Where(IsHex)
            .Select((name) => Encoding.UTF8.GetString(Convert.FromHexString(name))).ToArray();
        await HttpContext.SendDataAsync(new { success = true, content = files });
    }

    public async Task WriteFile() {
        if (!await CheckMethod("POST")) return;

        string filename = Request.Headers["file-path"];

        byte[] content = null;
        try {
            string encoded = await GetJson("content");
            if (encoded != null) {
                content = Convert.FromBase64String(encoded);
            }
        } catch { }

        if (string.IsNullOrEmpty(filename) || content == null) {
            await SendError("File path and content required");
            return;
        }

        if (!IsHex(filename)) {
            await SendError("Invalid Path");
            return;
        }

        string path = Path.Combine(Server.SavePath.FullName, filename);
        await File.WriteAllBytesAsync(path, content);
        await HttpContext.SendDataAsync(new { success = true });
    }

    public async Task GetFile() {
        string url = Request.Url.AbsolutePath;
        if (url.Length == 1) url = "/index.html";
        string ext = Path.GetExtension(url);
        Response.ContentEncoding = null;
        Response.ContentType = MimeType.Associations[ext];
        using var input = await FileSystem.OpenAppPackageFileAsync(url.TrimStart('/'));
        using var output = HttpContext.OpenResponseStream();
        await input.CopyToAsync(output);
    }

    public async Task BingChat() {
        var HttpContext = this.HttpContext;
        using var stream = HttpContext.OpenRequestStream();
        var req = await JsonSerializer.DeserializeAsync<Models.Request>(stream);
        if(req.Stream) {
            await SendText("Stream not supported.");
            return;
        }

        string input = Bing.MessagesToText(req.Messages);
        string output = await Bing.SendAsync(input);
        await SendText(output ?? "failed to fetch");

        async Task SendText(string text) {
            await HttpContext.SendDataAsync(
                new Response() {
                    Type = "chat.completion",
                    Created = DateTimeOffset.Now.ToUnixTimeSeconds(),
                    Usage = new() { Total = 0, Prompt = 0, Completion = 0, },
                    Choices = new MessageResult[] {
                    new MessageResult() {
                        Index = 0,
                        Message = new() {
                            Role = Message.ROLE_ASSISTANT,
                            Content = text,
                        },
                        Reason = "stop",
                    }
                }
            });
        }
    }
}
