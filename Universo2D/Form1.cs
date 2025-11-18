using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Collections.Generic;


namespace Universo
{
    public partial class Form1 : Form
    {
        private Graphics g;
        private Universo U, Uinicial;
        private int numCorpos, numInterac, numTempoInterac;
        private Timer simulationTimer;
        private int currentIteration;

        // --- Variáveis de Persistência ---
        private GravadorUniverso gravadorAtivo;
        private readonly GravadorTexto gravadorTexto = new GravadorTexto();
        private readonly GravadorMySQL gravadorMySQL = new GravadorMySQL();

        // MySQL runtime control
        private bool saveToMySQL = false;
        private int currentDBSimId = -1;

        // Replay (load all iterations) support
        private Timer replayTimer;
        private List<int> replayIterations;
        private int replayIndex;


        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            U = new Universo();
            Uinicial = new Universo();
            simulationTimer = new Timer
            {
                Interval = 20
            };
            simulationTimer.Tick += SimulationTimer_Tick;

            replayTimer = new Timer { Interval = 200 };
            replayTimer.Tick += ReplayTimer_Tick;

            // Define o gravador inicial (Texto como padrão)
            gravadorAtivo = gravadorTexto;

            // Ligação de eventos
            this.rb_texto.CheckedChanged += new EventHandler(this.rb_texto_CheckedChanged);
            this.rb_mysql.CheckedChanged += new EventHandler(this.rb_mysql_CheckedChanged);
        }

        // --- MÉTODOS ORIGINAIS E DE SIMULAÇÃO ---

        private void button2_Click(object sender, EventArgs e)
        {
            if (int.TryParse(qtdCorpos.Text, out numCorpos) &&
                int.TryParse(valXMax.Text, out int xMax) &&
                int.TryParse(valYMax.Text, out int yMax) &&
                int.TryParse(masMin.Text, out int mMin) &&
                int.TryParse(masMax.Text, out int mMax))
            {
                progressBar1.Value = 0;
                U.CarregarCorpos(numCorpos, 0, xMax, 0, yMax, mMin, mMax);
                Uinicial.CopiarUniverso(U);
                qtdCorposAtual.Text = U.QtdCorp.ToString();
                this.Refresh();
            }
            else
            {
                MessageBox.Show("Por favor, insira valores numéricos válidos para todos os campos.", "Erro de Entrada", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(qtdInterac.Text, out numInterac) || !int.TryParse(qtdTempoInterac.Text, out numTempoInterac))
            {
                MessageBox.Show("Insira valores numéricos válidos para interações e tempo.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (numInterac <= 0)
            {
                MessageBox.Show("O número de interações deve ser maior que zero.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (numTempoInterac <= 0)
            {
                MessageBox.Show("O tempo por interação deve ser maior que zero.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Limites práticos para evitar estouro ou longas execuções acidentais
            if (numInterac > 1000000)
            {
                var res = MessageBox.Show("Número de iterações muito alto (mais de 1.000.000). Deseja continuar?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (res != DialogResult.Yes) return;
            }

            if (numTempoInterac > 3600)
            {
                var res2 = MessageBox.Show("Tempo por iteração muito alto (mais de 3600s). Deseja continuar?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (res2 != DialogResult.Yes) return;
            }

            if (U.QtdCorp == 0)
            {
                MessageBox.Show("Nenhum universo carregado para simular.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            progressBar1.Maximum = numInterac;
            progressBar1.Value = 0;
            currentIteration = 0;

            // Ajusta botões para evitar múltiplos inícios
            btn_executa.Enabled = false;
            btn_parar.Enabled = false;

            // Se gravador ativo é MySQL, perguntar se quer gravar cada iteração
            saveToMySQL = false;
            currentDBSimId = -1;
            if (gravadorAtivo is GravadorMySQL)
            {
                var resp = MessageBox.Show("Deseja gravar cada iteração desta simulação no MySQL?", "Gravar Iterações", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (resp == DialogResult.Yes)
                {
                    string nomeSimulacao = Interaction.InputBox("Digite um NOME para a simulação (no MySQL):", "Salvar no MySQL", "Simulacao Gravada");
                    if (!string.IsNullOrWhiteSpace(nomeSimulacao))
                    {
                        // Cria registro da simulação e salva iteração 0 (estado atual)
                        int idGerado = gravadorMySQL.SalvarSimulacao(U, nomeSimulacao, numInterac, numTempoInterac);
                        if (idGerado > 0)
                        {
                            currentDBSimId = idGerado;
                            saveToMySQL = true;
                        }
                        else
                        {
                            MessageBox.Show("Falha ao criar registro da simulação no MySQL. Gravação de iterações será desativada.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }

            if (radioButton1.Checked)
            {
                // Modo visual: habilita parar
                btn_parar.Enabled = true;
                simulationTimer.Start();
            }
            else if (radioButton2.Checked)
            {
                // Modo rápido: não há timer, execução imediata
                for (int i = 0; i < numInterac; i++)
                {
                    U.InteragirCorpos(numTempoInterac);
                    currentIteration = i + 1;
                    progressBar1.Value = currentIteration;
                    // Salva iteração no MySQL, se ativado
                    if (saveToMySQL && currentDBSimId > 0)
                    {
                        gravadorMySQL.SalvarIteracao(currentDBSimId, currentIteration, U);
                    }
                }
                this.Refresh();
                MessageBox.Show("Simulação concluída!");
                // Reabilita botões
                btn_executa.Enabled = true;
                btn_parar.Enabled = false;

                // reset save flag
                saveToMySQL = false;
                currentDBSimId = -1;
            }
        }

        private void SimulationTimer_Tick(object sender, EventArgs e)
        {
            if (currentIteration < numInterac)
            {
                U.InteragirCorpos(numTempoInterac);
                currentIteration++;
                progressBar1.Value = currentIteration;
                // Salva iteração no MySQL, se ativado
                if (saveToMySQL && currentDBSimId > 0)
                {
                    gravadorMySQL.SalvarIteracao(currentDBSimId, currentIteration, U);
                }
                this.Refresh();
            }
            else
            {
                simulationTimer.Stop();
                btn_executa.Enabled = true;
                btn_parar.Enabled = false;
                MessageBox.Show("Simulação concluída!");

                // reset save flag
                saveToMySQL = false;
                currentDBSimId = -1;
            }
        }

        private void btn_parar_Click(object sender, EventArgs e)
        {
            // Para a simulação em andamento (modo visual)
            if (simulationTimer.Enabled)
            {
                simulationTimer.Stop();
            }
            if (replayTimer.Enabled)
            {
                replayTimer.Stop();
            }
            // Ajusta estados dos botões
            btn_executa.Enabled = true;
            btn_parar.Enabled = false;

            // If stopping manual save session, keep the DB record but disable further per-iteration saves
            saveToMySQL = false;
        }

        private void ReplayTimer_Tick(object sender, EventArgs e)
        {
            if (replayIterations == null) return;
            if (replayIndex < replayIterations.Count)
            {
                int iterNum = replayIterations[replayIndex];
                var loaded = gravadorMySQL.CarregarIteracao(currentDBSimId, iterNum);
                if (loaded != null)
                {
                    U = loaded;
                    qtdCorposAtual.Text = U.QtdCorp.ToString();
                    progressBar1.Value = Math.Min(progressBar1.Maximum, iterNum);
                    this.Refresh();
                }
                replayIndex++;
            }
            else
            {
                replayTimer.Stop();
                MessageBox.Show("Replay concluído.");
                btn_executa.Enabled = true;
                btn_parar.Enabled = false;
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs pe)
        {
            if (U == null || U.QtdCorp == 0) return;

            g = pe.Graphics;
            g.Clear(Color.Black);

            // Lógica de Proporção e Desenho
            float minX = float.MaxValue, minY = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue;
            foreach (var cp in U.ListaCorp)
            {
                if (cp.Valido)
                {
                    minX = Math.Min(minX, (float)cp.PosX);
                    minY = Math.Min(minY, (float)cp.PosY);
                    maxX = Math.Max(maxX, (float)cp.PosX);
                    maxY = Math.Max(maxY, (float)cp.PosY);
                }
            }

            float W = this.ClientSize.Width - 50;
            float H = this.ClientSize.Height - 50;
            float rangeX = maxX - minX;
            float rangeY = maxY - minY;

            float prop = 1;
            if (rangeX > W || rangeY > H)
            {
                prop = Math.Max(rangeX / W, rangeY / H);
            }
            if (prop == 0) prop = 1;

            txtProporcao.Text = (1 / prop).ToString("F2");
            qtdCorposAtual.Text = U.QtdCorp.ToString();

            // Desenha os corpos
            foreach (var cp in U.ListaCorp)
            {
                if (cp.Valido)
                {
                    float posX = ((float)(cp.PosX - minX) / prop) + 25;
                    float posY = ((float)(cp.PosY - minY) / prop) + 25;
                    float raio = (float)cp.Raio / prop;
                    if (raio < 1) raio = 1;

                    g.FillEllipse(Brushes.White, posX - raio, posY - raio, raio * 2, raio * 2);
                }
            }
        }

        public void Form1_Load(object sender, EventArgs e)
        {
            // Garante que o gravador ativo esteja correto na inicialização
            if (rb_mysql.Checked)
            {
                gravadorAtivo = gravadorMySQL;
            }
            else
            {
                gravadorAtivo = gravadorTexto;
            }
        }

        // --- MÉTODOS DE PERSISTÊNCIA E EVENTOS ---

        private void rb_texto_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_texto.Checked)
            {
                gravadorAtivo = gravadorTexto;
            }
        }

        private void rb_mysql_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_mysql.Checked)
            {
                gravadorAtivo = gravadorMySQL;
            }
        }

        private void btn_testaMySQL_Click(object sender, EventArgs e)
        {
            if (gravadorMySQL.TestarConexao())
            {
                MessageBox.Show("Conexão com o MySQL estabelecida com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btn_grava_Click(object sender, EventArgs e)
        {
            if (U.QtdCorp == 0)
            {
                MessageBox.Show("Não há corpos no Universo a serem salvos.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int.TryParse(qtdInterac.Text, out int interac);
            int.TryParse(qtdTempoInterac.Text, out int tempo);

            if (gravadorAtivo is GravadorMySQL)
            {
                // Pede o NOME e captura o ID gerado pelo banco.
                string nomeSimulacao = Interaction.InputBox("Digite um NOME para a simulação:", "Salvar no MySQL", "Simulacao Gravada");
                if (string.IsNullOrWhiteSpace(nomeSimulacao)) return;

                // CORREÇÃO: Chamamos o método público que delega para o salvamento e retorna o ID.
                int idGerado = gravadorMySQL.SalvarSimulacao(U, nomeSimulacao, interac, tempo);

                if (idGerado > 0)
                {
                    MessageBox.Show($"Simulação '{nomeSimulacao}' salva com SUCESSO!\nID Gerado pelo Banco: {idGerado}\n\nUse este ID para carregar.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // A falha é tratada dentro do SalvarSimulacao, mas essa mensagem finaliza o ciclo.
                    MessageBox.Show("Falha ao gravar. O ID do Banco não foi obtido (Verifique o log de erros).", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                using (SaveFileDialog sfd = new SaveFileDialog { Filter = "Arquivos Universo|*.uni", Title = "Salvar arquivo de simulação" })
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        gravadorAtivo.GravarUniverso(U, sfd.FileName, interac, tempo);
                    }
                }
            }
        }

        private void btn_grava_ini_Click(object sender, EventArgs e)
        {
            if (Uinicial.QtdCorp == 0)
            {
                MessageBox.Show("Não há configuração inicial para salvar.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int interac = 0;
            int tempo = 0;

            if (gravadorAtivo is GravadorMySQL)
            {
                string nomeSimulacao = Interaction.InputBox("Digite um NOME para a configuração inicial:", "Salvar no MySQL", "Configuracao Inicial");
                if (string.IsNullOrWhiteSpace(nomeSimulacao)) return;

                // CORREÇÃO: Chamamos o método público que delega para o salvamento e retorna o ID.
                int idGerado = gravadorMySQL.SalvarSimulacao(Uinicial, nomeSimulacao, interac, tempo);

                if (idGerado > 0)
                {
                    MessageBox.Show($"Configuração inicial salva com SUCESSO!\nID Gerado pelo Banco: {idGerado}\n\nUse este ID para carregar.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                using (SaveFileDialog sfd = new SaveFileDialog { Filter = "Arquivos Universo|*.uni", Title = "Salvar arquivo inicial" })
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        gravadorAtivo.GravarUniversoInicial(Uinicial, sfd.FileName);
                    }
                }
            }
        }

        // Este botão "Carregar" agora carrega qualquer simulação, seja por arquivo ou ID.
        private void btn_carrega_Click(object sender, EventArgs e)
        {
            string inputCaminhoOuId = "";

            if (gravadorAtivo is GravadorMySQL)
            {
                // Pede o ID real gerado pelo banco
                inputCaminhoOuId = Interaction.InputBox("Digite o ID da simulação (número) para carregar do MySQL:", "Carregar Simulação", "1");
                if (string.IsNullOrWhiteSpace(inputCaminhoOuId)) return;
            }
            else
            {
                using (OpenFileDialog ofd = new OpenFileDialog { Filter = "Arquivos Universo|*.uni", Title = "Abrir arquivo de simulação" })
                {
                    if (ofd.ShowDialog() != DialogResult.OK) return;
                    inputCaminhoOuId = ofd.FileName;
                }
            }

            if (gravadorAtivo is GravadorMySQL)
            {
                if (!int.TryParse(inputCaminhoOuId, out int idSim)) return;
                // Pergunta se o usuário deseja carregar todas as iterações para replay
                var resp = MessageBox.Show("Deseja carregar TODAS as iterações para replay (sim) ou apenas o estado inicial (não)?", "Carregar iterações", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (resp == DialogResult.Cancel) return;

                // Carrega metadados + estado inicial
                Universo universoCarregado = gravadorMySQL.CarregarSimulacao(idSim, out int numInteracOut, out int numTempoInteracOut);
                if (universoCarregado == null) return;

                U = universoCarregado;
                Uinicial.CopiarUniverso(U);

                qtdCorpos.Text = U.QtdCorp.ToString();
                qtdInterac.Text = numInteracOut.ToString();
                qtdTempoInterac.Text = numTempoInteracOut.ToString();
                progressBar1.Value = 0;
                progressBar1.Maximum = numInteracOut > 0 ? numInteracOut : 100;
                this.Refresh();

                if (resp == DialogResult.Yes)
                {
                    // Preparar replay: listar iterações e iniciar timer
                    var iters = gravadorMySQL.ListarIteracoes(idSim);
                    if (iters == null || iters.Count == 0)
                    {
                        MessageBox.Show("Nenhuma iteração encontrada para esta simulação.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    currentDBSimId = idSim;
                    replayIterations = iters;
                    replayIndex = 0;
                    // Ajusta intervalo do replay (usar tempo por interação em ms se disponível)
                    int intervalMs = Math.Max(50, numTempoInteracOut * 10); // heurística
                    replayTimer.Interval = intervalMs;
                    btn_parar.Enabled = true;
                    btn_executa.Enabled = false;
                    replayTimer.Start();
                }
            }
            else
            {
                Universo universoCarregado = gravadorAtivo.CarregarUniverso(inputCaminhoOuId, out int numInteracOut, out int numTempoInteracOut);

                if (universoCarregado != null)
                {
                    U = universoCarregado;
                    Uinicial.CopiarUniverso(U);

                    qtdCorpos.Text = U.QtdCorp.ToString();
                    qtdInterac.Text = numInteracOut.ToString();
                    qtdTempoInterac.Text = numTempoInteracOut.ToString();
                    progressBar1.Value = 0;
                    progressBar1.Maximum = numInteracOut > 0 ? numInteracOut : 100;

                    this.Refresh();
                }
            }
        }

        // Métodos vazios esperados pelo designer
        private void radioButton3_CheckedChanged(object sender, EventArgs e) { }
        private void btn_carregaSimulacao_Click(object sender, EventArgs e) { }
    }
}