using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Universo
{
    public class Universo
    {
        public ObservableCollection<Corpos> ListaCorp { get; private set; }
        public static readonly double G = 6.67408e-11; // Constante gravitacional no SI

        private static readonly Random SharedRandom = new Random();

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

            // Softening para evitar forças infinitas em distâncias muito pequenas
            double softening = 1e-6;
            double invDist = 1.0 / Math.Max(distancia, softening);

            double forca = (G * c1.Massa * c2.Massa) * (invDist * invDist);

            double dx = c2.PosX - c1.PosX;
            double dy = c2.PosY - c1.PosY;

            double forcaX = forca * dx * invDist;
            double forcaY = forca * dy * invDist;

            // Atualiza forças (não é executado em paralelo neste método)
            c1.ForcaX += forcaX;
            c1.ForcaY += forcaY;

            c2.ForcaX -= forcaX;
            c2.ForcaY -= forcaY;
        }

        private void Colisao(Corpos c1, Corpos c2)
        {
            if (!c1.Valido || !c2.Valido || Dist(c1, c2) > c1.Raio + c2.Raio) return;

            Corpos maior = c1.Massa >= c2.Massa ? c1 : c2;
            Corpos menor = c1.Massa < c2.Massa ? c1 : c2;

            double massaTotal = maior.Massa + menor.Massa;
            if (massaTotal <= 0) return;

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

            // Validação mínima de ranges
            if (numCorpos <= 0) return;
            if (xFim <= xIni) xFim = xIni + 1;
            if (yFim <= yIni) yFim = yIni + 1;
            if (masFim <= masIni) masFim = masIni + 1;

            lock (SharedRandom)
            {
                for (int i = 0; i < numCorpos; i++)
                {
                    string nome = "cp" + i;
                    double massa = SharedRandom.Next(masIni, masFim);
                    double densidade = SharedRandom.Next(1000, 20000); // Densidades mais realistas (ex: água a rochas/metais)
                    double posX = SharedRandom.Next(xIni, xFim);
                    double posY = SharedRandom.Next(yIni, yFim);
                    double velX = (SharedRandom.NextDouble() - 0.5) * 2; // Velocidades iniciais menores
                    double velY = (SharedRandom.NextDouble() - 0.5) * 2;

                    ListaCorp.Add(new Corpos(nome, massa, posX, posY, velX, velY, densidade));
                }
            }
        }

        public void InteragirCorpos(int qtdSegundos)
        {
            if (qtdSegundos <= 0) return;

            // Trabalhar com snapshot para evitar problemas de concorrência com Parallel.ForEach
            List<Corpos> snapshot = ListaCorp.ToList();

            // Zerar forças
            foreach (var corpo in snapshot)
            {
                corpo.ForcaX = 0;
                corpo.ForcaY = 0;
            }

            int n = snapshot.Count;

            // Cálculo de forças (sequencial)
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    ForcaG(snapshot[i], snapshot[j]);
                }
            }

            // Atualização de posição e velocidade (paralelo seguro sobre snapshot)
            Parallel.ForEach(snapshot, corpo =>
            {
                if (corpo.Valido)
                {
                    CalcularVelocidadePosicao(qtdSegundos, corpo);
                }
            });

            // Tratamento de colisões (usar snapshot para determinar pares, mas as mudanças aplicam-se aos objetos originais)
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    Colisao(snapshot[i], snapshot[j]);
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
            double t = qtdSegundos;
            c1.PosX += c1.VelX * t + (acelX * t * t) / 2.0;
            c1.PosY += c1.VelY * t + (acelY * t * t) / 2.0;

            // v = v0 + at
            c1.VelX += acelX * t;
            c1.VelY += acelY * t;
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
