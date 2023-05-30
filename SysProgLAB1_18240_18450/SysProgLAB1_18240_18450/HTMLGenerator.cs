using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysProgLAB1_18240_18450
{
    internal class HTMLGenerator
    {
        private static string HTMLAElement(string fileName)
        {
            return $"<li><a href=\"http://localhost:5050/preuzmi/{fileName}\" target=\"_blank\">{fileName}</a></li>";
        }
        public static string KreirajElemente(List<string> listOfFiles)
        {
            string aElements =
                listOfFiles.Count > 0 ?
                string.Join("", listOfFiles.Select(HTMLAElement))
                : "<h3>Nema fajlova koji zadovoljavaju zadate kriterijume.</h3>";

            return aElements;
        }
    }
}
