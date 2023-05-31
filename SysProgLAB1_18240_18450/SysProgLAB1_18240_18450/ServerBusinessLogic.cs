using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SysProgLAB1_18240_18450
{
    internal class ServerBusinessLogic
    {
        private static Cache _cache = new(3);
        private static Task<ConcurrentQueue<string>> PretraziKljucnuRecAsync(string path, string keyword)
        {
            return Task.Run(() =>
            {
                ConcurrentQueue<string> returnQueue = new ConcurrentQueue<string>();

                if (!string.IsNullOrEmpty(keyword))
                {
                    string[] files = Directory.GetFiles(path);

                    Parallel.ForEach(files, file =>
                    {
                        string fileName = Path.GetFileName(file);
                        if (fileName.Contains(keyword))
                        {
                            returnQueue.Enqueue(fileName);
                        }
                    });
                }
                return returnQueue;
            });
        }
        public static async void ZahtevPrikazivanjaListeFajlova(string requestUrl, HttpListenerResponse response)
        {
            string elementi;
            string res = "<html><head><title>";

            if (_cache.SadrziKljuc(requestUrl))
            {
                elementi = _cache.CitajIzKesa(requestUrl);
            }
            else
            {
                elementi = HTMLGenerator.KreirajElemente(await PretraziKljucnuRecAsync(Directory.GetCurrentDirectory(), requestUrl));
                _cache.UpisiUKes(requestUrl, elementi);
            }

            if (elementi.Contains("<h3>"))
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.StatusDescription = "Not Found";
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.OK;
                response.StatusDescription = "OK";
            }

            res += $"{response.StatusCode} - {response.StatusDescription}</title><body><ul></h2>{elementi}</ul></body></html>";

            byte[] resBinary = Encoding.UTF8.GetBytes(res);
            response.ContentLength64 = resBinary.Length;
            response.OutputStream.Write(resBinary, 0, resBinary.Length);
            response.OutputStream.Close();
        }
        public static void ZahtevPreuzimanjaFajla(string requestUrl, HttpListenerResponse response)
        {
            using (FileStream fs = new(requestUrl, FileMode.Open))
            {
                byte[] byteArray = new byte[fs.Length];
                fs.Read(byteArray, 0, byteArray.Length);
                response.ContentLength64 = byteArray.Length;
                response.AppendHeader("Content-Disposition", "attachment");
                response.OutputStream.Write(byteArray, 0, byteArray.Length);
                response.OutputStream.Close();
            }
        }
    }
}
