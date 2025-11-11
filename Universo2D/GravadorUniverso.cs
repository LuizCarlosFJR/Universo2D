using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Universo
{
    public abstract class GravadorUniverso
    {
        public abstract void GravarUniversoInicial(Universo universo, string caminho);
        public abstract void GravarUniverso(Universo universo, string caminho, int numInterac, int numTempoInterac);
        public abstract Universo CarregarUniverso(string caminho, out int numInterac, out int numTempoInterac);
    }
}