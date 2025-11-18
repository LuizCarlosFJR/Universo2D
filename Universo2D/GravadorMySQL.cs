using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;
using System.Data;
using System.Collections.Generic;

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

        // --- LÓGICA DE SALVAMENTO (COM ITERAÇÕES) ---

        /// <summary>
        /// Salva a simulação (metadados) e também grava a iteração 0 (estado inicial) na tabela CorpoIteracao.
        /// Para salvar outras iterações utilize SalvarIteracao(idSimulacao, iteracaoNum, u).
        /// </summary>
        public int SalvarSimulacao(Universo u, string nomeSimulacao, int numInterac, int numTempoInterac)
        {
            int idSimulacao = -1;

            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. INSERIR NA TABELA SIMULACAO (Faz o INSERT e obtém o ID gerado)
                        string sqlSimulacao = "INSERT INTO Simulacao (NomeSimulacao, TotalCorpos, NumInterac, NumTempoInterac, DataGravacao) VALUES (@nomeSim, @numCorpos, @numInterac, @tempoInterac, NOW())";

                        using (var cmd = new MySqlCommand(sqlSimulacao, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@nomeSim", nomeSimulacao);
                            cmd.Parameters.AddWithValue("@numCorpos", u.QtdCorp);
                            cmd.Parameters.AddWithValue("@numInterac", numInterac);
                            cmd.Parameters.AddWithValue("@tempoInterac", numTempoInterac);

                            cmd.ExecuteNonQuery();
                            idSimulacao = (int)cmd.LastInsertedId;
                        }

                        if (idSimulacao > 0)
                        {
                            // 2. Inserir o snapshot da iteração 0 na tabela CorpoIteracao
                            //    Schema esperado para CorpoIteracao: (IdSimulacao INT, IteracaoNum INT, Nome VARCHAR, Massa DOUBLE, Densidade DOUBLE, PosX DOUBLE, PosY DOUBLE, VelX DOUBLE, VelY DOUBLE)
                            string sqlCorpoIter = "INSERT INTO CorpoIteracao (IdSimulacao, IteracaoNum, Nome, Massa, Densidade, PosX, PosY, VelX, VelY) VALUES (@idSim, @iterNum, @nome, @massa, @dens, @pX, @pY, @vX, @vY)";

                            using (var cmd = new MySqlCommand(sqlCorpoIter, connection, transaction))
                            {
                                cmd.Parameters.Add("@idSim", MySqlDbType.Int32).Value = idSimulacao;
                                cmd.Parameters.Add("@iterNum", MySqlDbType.Int32).Value = 0;
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

                        transaction.Commit();
                        return idSimulacao;
                    }
                    catch (Exception ex)
                    {
                        try { transaction.Rollback(); } catch { }
                        MessageBox.Show($"Erro ao salvar no MySQL: {ex.Message}", "Erro de BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return -1;
                    }
                }
            }
        }

        /// <summary>
        /// Salva um snapshot de uma iteração já existente de uma simulação previamente criada (idSimulacao).
        /// Use este método durante a execução da simulação para gravar cada iteração.
        /// </summary>
        public bool SalvarIteracao(int idSimulacao, int iteracaoNum, Universo u)
        {
            if (idSimulacao <= 0) return false;

            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string sqlCorpoIter = "INSERT INTO CorpoIteracao (IdSimulacao, IteracaoNum, Nome, Massa, Densidade, PosX, PosY, VelX, VelY) VALUES (@idSim, @iterNum, @nome, @massa, @dens, @pX, @pY, @vX, @vY)";

                        using (var cmd = new MySqlCommand(sqlCorpoIter, connection, transaction))
                        {
                            cmd.Parameters.Add("@idSim", MySqlDbType.Int32).Value = idSimulacao;
                            cmd.Parameters.Add("@iterNum", MySqlDbType.Int32).Value = iteracaoNum;
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

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        try { transaction.Rollback(); } catch { }
                        MessageBox.Show($"Erro ao salvar iteração no MySQL: {ex.Message}", "Erro de BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }
        }

        // --- LÓGICA DE CARREGAMENTO (LEITURA DE SNAPSHOTS POR ITERAÇÃO) ---

        /// <summary>
        /// Carrega a simulação lendo os metadados e o snapshot da iteração 0 (estado inicial) da tabela CorpoIteracao.
        /// Não realiza cálculos: todos os valores de posição e velocidade são lidos do banco.
        /// </summary>
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

                    // 2. Carregar Corpos do snapshot da iteração 0 (sem cálculo)
                    string sqlCorposIter0 = "SELECT Nome, Massa, Densidade, PosX, PosY, VelX, VelY FROM CorpoIteracao WHERE IdSimulacao = @idSim AND IteracaoNum = 0 ORDER BY Id";
                    using (var cmdCorpos = new MySqlCommand(sqlCorposIter0, connection))
                    {
                        cmdCorpos.Parameters.AddWithValue("@idSim", idSimulacao);
                        using (var reader = cmdCorpos.ExecuteReader())
                        {
                            while (reader.Read())
                            {
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

        /// <summary>
        /// Carrega um snapshot específico de iteração para a simulação indicada.
        /// </summary>
        public Universo CarregarIteracao(int idSimulacao, int iteracaoNum)
        {
            if (idSimulacao <= 0) return null;
            Universo u = new Universo();

            using (var connection = new MySqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    string sqlCorposIter = "SELECT Nome, Massa, Densidade, PosX, PosY, VelX, VelY FROM CorpoIteracao WHERE IdSimulacao = @idSim AND IteracaoNum = @iterNum ORDER BY Id";
                    using (var cmd = new MySqlCommand(sqlCorposIter, connection))
                    {
                        cmd.Parameters.AddWithValue("@idSim", idSimulacao);
                        cmd.Parameters.AddWithValue("@iterNum", iteracaoNum);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
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
                    MessageBox.Show($"Erro ao carregar iteração do MySQL: {ex.Message}", "Erro de BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
        }

        /// <summary>
        /// Retorna uma lista com as iterações disponíveis para uma simulação (útil para replay completo).
        /// </summary>
        public List<int> ListarIteracoes(int idSimulacao)
        {
            var lista = new List<int>();
            if (idSimulacao <= 0) return lista;

            using (var connection = new MySqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    string sql = "SELECT DISTINCT IteracaoNum FROM CorpoIteracao WHERE IdSimulacao = @idSim ORDER BY IteracaoNum";
                    using (var cmd = new MySqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@idSim", idSimulacao);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(reader.GetInt32(0));
                            }
                        }
                    }
                }
                catch { }
            }
            return lista;
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