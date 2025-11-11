using System;

namespace Universo
{
    public class Corpos
    {
        public bool Valido { get; set; }
        public string Nome { get; set; }
        public double Massa { get; set; }
        public double Densidade { get; set; }
        public double PosX { get; set; }
        public double PosY { get; set; }
        public double VelX { get; set; }
        public double VelY { get; set; }
        public double ForcaX { get; set; }
        public double ForcaY { get; set; }

        // Construtor padrão
        public Corpos()
        {
            this.Valido = true;
            this.Nome = "";
            this.Massa = 0;
            this.Densidade = 1;
        }

        // Construtor com parâmetros para 2D (usado no carregamento MySQL)
        public Corpos(string n, double m, double pX, double pY, double vX, double vY, double d)
        {
            this.Valido = true;
            this.Nome = n;
            this.Massa = m;
            this.PosX = pX;
            this.PosY = pY;
            this.VelX = vX;
            this.VelY = vY;
            this.Densidade = d;
            this.ForcaX = 0;
            this.ForcaY = 0;
        }

        public double Raio
        {
            get
            {
                if (Densidade <= 0) return 1; // Raio mínimo de 1 para evitar divisão por zero
                double volume = Massa / Densidade;
                // V = 4/3 * pi * r^3 -> r = (3V / 4pi)^(1/3)
                return Math.Pow((3 * volume) / (4 * Math.PI), 1.0 / 3.0);
            }
        }

        public void CopiarCorpo(Corpos cp)
        {
            this.Nome = cp.Nome;
            this.Massa = cp.Massa;
            this.Densidade = cp.Densidade;
            this.PosX = cp.PosX;
            this.PosY = cp.PosY;
            this.VelX = cp.VelX;
            this.VelY = cp.VelY;
            this.Valido = cp.Valido;
            this.ForcaX = cp.ForcaX;
            this.ForcaY = cp.ForcaY;
        }
    }
}