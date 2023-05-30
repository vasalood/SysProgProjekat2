using SysProgLAB1_18240_18450;
using System.Net;
class Program
{
    const string FAVICON = "favicon.ico";

    static async Task Main()
    {
        HttpListener listener = new HttpListener();
        string[] prefixes = new string[]
        {
            "http://localhost:5050/",
            "http://127.0.0.1:5050/",
        };

        Parallel.ForEach(prefixes, listener.Prefixes.Add);

        listener.Start();

        while (true)
        {
            ProcessRequest(await listener.GetContextAsync());
        }
    }
    static void ProcessRequest(HttpListenerContext context)
    {
        _ = Task.Run(() =>
        {
            try
            {
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                string requestUrl = request.RawUrl ?? "/";
                requestUrl = requestUrl.TrimStart('/');

                if (requestUrl.Contains("%20"))
                    requestUrl = requestUrl.Replace("%20", " ");

                if (requestUrl == FAVICON)
                    return;

                if (requestUrl.StartsWith("preuzmi/"))
                {
                    requestUrl = requestUrl["preuzmi/".Length..];
                    ServerBusinessLogic.ZahtevPreuzimanjaFajla(requestUrl, response);
                }
                else ServerBusinessLogic.ZahtevPrikazivanjaListeFajlova(requestUrl, response);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Zahtev nije uspesno obradjen zbog: {e.Message}");
            }
        });
    }
}