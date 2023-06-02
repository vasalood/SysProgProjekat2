using SysProgLAB1_18240_18450;
using System.Net;
class Program
{
    const string FAVICON = "favicon.ico";

    public static void Main()
    {
        HttpListener listener = new HttpListener();
        string[] prefixes = new string[]
        {
            "http://localhost:5050/",
            "http://127.0.0.1:5050/",
        };

        foreach (string prefix in prefixes)
        {
            listener.Prefixes.Add(prefix);
        }

        listener.Start();

        while (true)
        {
            HttpListenerContext context = listener.GetContext();
            ProcessRequest(context);
        }
    }
    static async void ProcessRequest(HttpListenerContext context)
    {
        await Task.Run(() =>
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
                    requestUrl = requestUrl["preuzmi/".Length..requestUrl.Length];
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