using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;
using System.Data;

namespace Universo
{
    public class GravadorMySQL : GravadorUniverso
    {
        // ⚠️ ATENÇÃO: Verifique esta ConnectionString!
        private const string ConnectionString = "Server=localhost;Database=universo_db;Uid=root;Pwd=senha;";

        // --- MÉTODOS DE TESTE E CONEXÃO ---

        public bool TestarConexao()
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Falha na conexão com o MySQL. Verifique sua ConnectionString.\n\nDetalhes do Erro:\n{ex.Message}", "Erro de Conexão", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }

        // --- LÓGICA DE SALVAMENTO (CORRIGIDA) ---

        public int SalvarSimulacao(Universo u, string nomeSimulacao, int numInterac, int numTempoInterac)
        {
            int idSimulacao = -1;

            using (var connection = new MySqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();

                    // 1. INSERIR NA TABELA SIMULACAO (Faz o INSERT e obtém o ID gerado)
                    string sqlSimulacao = "INSERT INTO Simulacao (NomeSimulacao, TotalCorpos, NumInterac, NumTempoInterac, DataGravacao) VALUES (@nomeSim, @numCorpos, @numInterac, @tempoInterac, NOW())";

                    using (var cmd = new MySqlCommand(sqlSimulacao, connection))
                    {
                        cmd.Parameters.AddWithValue("@nomeSim", nomeSimulacao);
                        cmd.Parameters.AddWithValue("@numCorpos", u.QtdCorp);
                        cmd.Parameters.AddWithValue("@numInterac", numInterac);
                        cmd.Parameters.AddWithValue("@tempoInterac", numTempoInterac);

                        cmd.ExecuteNonQuery();

                        // Obtém o ID gerado IMEDIATAMENTE após o INSERT. Isso resolve a ambiguidade.
                        idSimulacao = (int)cmd.LastInsertedId;
                    }

                    if (idSimulacao > 0)
                    {
                        // 2. INSERIR NA TABELA CORPO (Modelo 2D)
                        string sqlCorpo = "INSERT INTO Corpo (IdSimulacao, Nome, Massa, Densidade, PosX, PosY, VelX, VelY) VALUES (@idSim, @nome, @massa, @dens, @pX, @pY, @vX, @vY)";

                        using (var cmd = new MySqlCommand(sqlCorpo, connection))
                        {
                            // Define e prepara os parâmetros ANTES do loop
                            cmd.Parameters.Add("@idSim", MySqlDbType.Int32).Value = idSimulacao;
                            cmd.Parameters.Add("@nome", MySqlDbType.VarChar);
                            cmd.Parameters.Add("@massa", MySqlDbType.Double);
                            cmd.Parameters.Add("@dens", MySqlDbType.Double);
                            cmd.Parameters.Add("@pX", MySqlDbType.Double);
                            cmd.Parameters.Add("@pY", MySqlDbType.Double);
                            cmd.Parameters.Add("@vX", MySqlDbType.Double);
                            cmd.Parameters.Add("@vY", MySqlDbType.Double);
                            cmd.Prepare();

                            foreach (Corpos corpo in u.ListaCorp)
                            {
                                // Apenas atualiza os valores dentro do loop
                                cmd.Parameters["@nome"].Value = corpo.Nome;
                                cmd.Parameters["@massa"].Value = corpo.Massa;
                                cmd.Parameters["@dens"].Value = corpo.Densidade;
                                cmd.Parameters["@pX"].Value = corpo.PosX;
                                cmd.Parameters["@pY"].Value = corpo.PosY;
                                cmd.Parameters["@vX"].Value = corpo.VelX;
                                cmd.Parameters["@vY"].Value = corpo.VelY;

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    return idSimulacao;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao salvar no MySQL: {ex.Message}", "Erro de BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return -1;
                }
            }
        }

        // --- LÓGICA DE CARREGAMENTO (ESTÁVEL E 2D) ---

        public Universo CarregarSimulacao(int idSimulacao, out int numInterac, out int numTempoInterac)
        {
            numInterac = 0;
            numTempoInterac = 0;
            Universo u = new Universo();

            using (var connection = new MySqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();

                    // 1. Carregar dados da Simulação
                    string sqlSimulacao = "SELECT NumInterac, NumTempoInterac FROM Simulacao WHERE IdSimulacao = @idSim";
                    using (var cmdSim = new MySqlCommand(sqlSimulacao, connection))
                    {
                        cmdSim.Parameters.AddWithValue("@idSim", idSimulacao);
                        using (var reader = cmdSim.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                numInterac = reader.GetInt32("NumInterac");
                                numTempoInterac = reader.GetInt32("NumTempoInterac");
                            }
                            else
                            {
                                MessageBox.Show($"Simulação com ID {idSimulacao} não encontrada.", "Erro de Carregamento", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return null;
                            }
                        }
                    }

                    // 2. Carregar Corpos (Modelo 2D)
                    string sqlCorpos = "SELECT Nome, Massa, Densidade, PosX, PosY, VelX, VelY FROM Corpo WHERE IdSimulacao = @idSim";
                    using (var cmdCorpos = new MySqlCommand(sqlCorpos, connection))
                    {
                        cmdCorpos.Parameters.AddWithValue("@idSim", idSimulacao);
                        using (var reader = cmdCorpos.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Usa o construtor 2D
                                Corpos novoCorpo = new Corpos(
                                    reader.GetString("Nome"),
                                    reader.GetDouble("Massa"),
                                    reader.GetDouble("PosX"),
                                    reader.GetDouble("PosY"),
                                    reader.GetDouble("VelX"),
                                    reader.GetDouble("VelY"),
                                    reader.GetDouble("Densidade")
                                );
                                u.ListaCorp.Add(novoCorpo);
                            }
                        }
                    }
                    return u;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao carregar do MySQL: {ex.Message}", "Erro de BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
        }

        // --- IMPLEMENTAÇÃO DOS MÉTODOS ABSTRATOS DE GRAVADORUNIVERSO (Contrato) ---

        public override void GravarUniversoInicial(Universo universo, string caminho)
        {
            string nome = System.IO.Path.GetFileNameWithoutExtension(caminho);
            SalvarSimulacao(universo, nome, 0, 0);
        }

        public override void GravarUniverso(Universo universo, string caminho, int numInterac, int numTempoInterac)
        {
            string nome = System.IO.Path.GetFileNameWithoutExtension(caminho);
            SalvarSimulacao(universo, nome, numInterac, numTempoInterac);
        }

        public override Universo CarregarUniverso(string idSimulacaoStr, out int numInterac, out int numTempoInterac)
        {
            numInterac = 0;
            numTempoInterac = 0;

            if (int.TryParse(idSimulacaoStr, out int idSimulacao))
            {
                return CarregarSimulacao(idSimulacao, out numInterac, out numTempoInterac);
            }
            MessageBox.Show("O Gravador MySQL espera um ID numérico. Por favor, insira um ID válido.", "Erro de Carregamento", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return null;
        }
    }
}