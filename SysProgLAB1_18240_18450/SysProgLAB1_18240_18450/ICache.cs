using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysProgLAB1_18240_18450
{
    internal interface ICache
    {
        string CitajIzKesa(string key);
        bool SadrziKljuc(string key);
        void UpisiUKes(string key, string value);
    }
}
