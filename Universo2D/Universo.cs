using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Universo
{
    public class Universo
    {
        public ObservableCollection<Corpos> ListaCorp { get; private set; }
        public static readonly double G = 6.67408e-11; // Constante gravitacional no SI

        public Universo()
        {
            ListaCorp = new ObservableCollection<Corpos>();
        }

        public int QtdCorp => ListaCorp.Count;

        public double Dist(Corpos c1, Corpos c2)
        {
            double dx = c1.PosX - c2.PosX;
            double dy = c1.PosY - c2.PosY;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private void ForcaG(Corpos c1, Corpos c2)
        {
            double distancia = Dist(c1, c2);
            if (distancia == 0) return;

            double forca = (G * c1.Massa * c2.Massa) / (distancia * distancia);

            double dx = c2.PosX - c1.PosX;
            double dy = c2.PosY - c1.PosY;

            double forcaX = forca * dx / distancia;
            double forcaY = forca * dy / distancia;

            lock (c1)
            {
                c1.ForcaX += forcaX;
                c1.ForcaY += forcaY;
            }

            lock (c2)
            {
                c2.ForcaX -= forcaX;
                c2.ForcaY -= forcaY;
            }
        }

        private void Colisao(Corpos c1, Corpos c2)
        {
            if (!c1.Valido || !c2.Valido || Dist(c1, c2) > c1.Raio + c2.Raio) return;

            Corpos maior = c1.Massa >= c2.Massa ? c1 : c2;
            Corpos menor = c1.Massa < c2.Massa ? c1 : c2;

            double massaTotal = maior.Massa + menor.Massa;
            
            // Conservação da quantidade de movimento (Q = mv)
            double pX = maior.VelX * maior.Massa + menor.VelX * menor.Massa;
            double pY = maior.VelY * maior.Massa + menor.VelY * menor.Massa;

            maior.VelX = pX / massaTotal;
            maior.VelY = pY / massaTotal;

            // A densidade do novo corpo é a média ponderada das densidades
            maior.Densidade = (maior.Massa * maior.Densidade + menor.Massa * menor.Densidade) / massaTotal;
            maior.Massa = massaTotal;
            maior.Nome += "+" + menor.Nome;

            menor.Valido = false;
        }

        public void CarregarCorpos(int numCorpos, int xIni, int xFim, int yIni, int yFim, int masIni, int masFim)
        {
            ListaCorp.Clear();
            Random rd = new Random();

            for (int i = 0; i < numCorpos; i++)
            {
                string nome = "cp" + i;
                double massa = rd.Next(masIni, masFim);
                double densidade = rd.Next(1000, 20000); // Densidades mais realistas (ex: água a rochas/metais)
                double posX = rd.Next(xIni, xFim);
                double posY = rd.Next(yIni, yFim);
                double velX = (rd.NextDouble() - 0.5) * 2; // Velocidades iniciais menores
                double velY = (rd.NextDouble() - 0.5) * 2;

                ListaCorp.Add(new Corpos(nome, massa, posX, posY, velX, velY, densidade));
            }
        }

        public void InteragirCorpos(int qtdSegundos)
        {
            ZerarForcas();

            // Cálculo de forças
            for (int i = 0; i < QtdCorp; i++)
            {
                for (int j = i + 1; j < QtdCorp; j++)
                {
                    ForcaG(ListaCorp[i], ListaCorp[j]);
                }
            }

            // Atualização de posição e velocidade
            Parallel.ForEach(ListaCorp, corpo =>
            {
                if (corpo.Valido)
                {
                    CalcularVelocidadePosicao(qtdSegundos, corpo);
                }
            });

            // Tratamento de colisões
            for (int i = 0; i < QtdCorp; i++)
            {
                for (int j = i + 1; j < QtdCorp; j++)
                {
                    Colisao(ListaCorp[i], ListaCorp[j]);
                }
            }

            OrganizarUniverso();
        }

        public void CopiarUniverso(Universo u)
        {
            ListaCorp.Clear();
            foreach (var corpoOriginal in u.ListaCorp)
            {
                var novoCorpo = new Corpos();
                novoCorpo.CopiarCorpo(corpoOriginal);
                ListaCorp.Add(novoCorpo);
            }
        }

        private void ZerarForcas()
        {
            foreach (var corpo in ListaCorp)
            {
                corpo.ForcaX = 0;
                corpo.ForcaY = 0;
            }
        }

        private void CalcularVelocidadePosicao(int qtdSegundos, Corpos c1)
        {
            if (c1.Massa == 0) return;

            // F = ma -> a = F/m
            double acelX = c1.ForcaX / c1.Massa;
            double acelY = c1.ForcaY / c1.Massa;

            // s = s0 + v0t + (at^2)/2
            c1.PosX += c1.VelX * qtdSegundos + (acelX * Math.Pow(qtdSegundos, 2)) / 2;
            c1.PosY += c1.VelY * qtdSegundos + (acelY * Math.Pow(qtdSegundos, 2)) / 2;
            
            // v = v0 + at
            c1.VelX += acelX * qtdSegundos;
            c1.VelY += acelY * qtdSegundos;
        }

        private void OrganizarUniverso()
        {
            var corposInvalidos = ListaCorp.Where(c => !c.Valido).ToList();
            if (corposInvalidos.Any())
            {
                foreach (var corpo in corposInvalidos)
                {
                    ListaCorp.Remove(corpo);
                }
            }
        }
    }
}
