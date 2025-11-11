using System;
using System.IO;
using System.Globalization;

namespace Universo
{
    public class GravadorTexto : GravadorUniverso
    {
        public override void GravarUniverso(Universo u, string caminho, int numInterac, int numTempoInterac)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(caminho))
                {
                    // Primeira linha: <quantidade de corpos>;<quantidade de iterações>;<tempo entre iterações>
                    sw.WriteLine($"{u.QtdCorp};{numInterac};{numTempoInterac}");

                    // Demais linhas: <Nome>;<massa>;<raio>;<PosX>;<PosY>;<VelX>;<VelY>
                    foreach (Corpos corpo in u.ListaCorp)
                    {
                        sw.WriteLine(
                            $"{corpo.Nome};" +
                            $"{corpo.Massa.ToString(CultureInfo.InvariantCulture)};" +
                            $"{corpo.Raio.ToString(CultureInfo.InvariantCulture)};" + // Raio é calculado, não a densidade
                            $"{corpo.PosX.ToString(CultureInfo.InvariantCulture)};" +
                            $"{corpo.PosY.ToString(CultureInfo.InvariantCulture)};" +
                            $"{corpo.VelX.ToString(CultureInfo.InvariantCulture)};" +
                            $"{corpo.VelY.ToString(CultureInfo.InvariantCulture)}"
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Erro ao salvar o arquivo: {ex.Message}", "Erro de Salvamento", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public override void GravarUniversoInicial(Universo u, string caminho)
        {
            // Para a configuração inicial, as iterações e o tempo são 0.
            GravarUniverso(u, caminho, 0, 0);
        }

        public override Universo CarregarUniverso(string caminho, out int numInterac, out int numTempoInterac)
        {
            numInterac = 0;
            numTempoInterac = 0;
            var universoCarregado = new Universo();

            try
            {
                using (StreamReader sr = new StreamReader(caminho))
                {
                    string linha = sr.ReadLine();
                    if (linha != null)
                    {
                        string[] header = linha.Split(';');
                        if (header.Length >= 3)
                        {
                            int.TryParse(header[1], out numInterac);
                            int.TryParse(header[2], out numTempoInterac);
                        }
                    }

                    while ((linha = sr.ReadLine()) != null)
                    {
                        string[] dados = linha.Split(';');
                        if (dados.Length == 7)
                        {
                            string nome = dados[0];
                            double massa = double.Parse(dados[1], CultureInfo.InvariantCulture);
                            double raio = double.Parse(dados[2], CultureInfo.InvariantCulture);
                            double posX = double.Parse(dados[3], CultureInfo.InvariantCulture);
                            double posY = double.Parse(dados[4], CultureInfo.InvariantCulture);
                            double velX = double.Parse(dados[5], CultureInfo.InvariantCulture);
                            double velY = double.Parse(dados[6], CultureInfo.InvariantCulture);

                            // A densidade precisa ser calculada a partir da massa e do raio.
                            // V = 4/3 * pi * r^3; d = m/V
                            double volume = (4.0 / 3.0) * Math.PI * Math.Pow(raio, 3);
                            double densidade = (volume > 0) ? massa / volume : 0;

                            var novoCorpo = new Corpos(nome, massa, posX, posY, velX, velY, densidade);
                            universoCarregado.ListaCorp.Add(novoCorpo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Ocorreu um erro ao carregar o arquivo: {ex.Message}", "Erro de Leitura", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }

            return universoCarregado;
        }
    }
}