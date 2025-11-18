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
        private const string EvaluatorSchema = "avaliador_db"; // ajustar conforme ambiente
        private const int DefaultCmdTimeout = 60;

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

        private bool IsProcedureMissing(Exception ex)
        {
            if (ex == null) return false;
            if (ex is MySqlException mex)
            {
                try { if (mex.Number == 1305) return true; } catch { }
            }
            string msg = ex.Message ?? string.Empty;
            return msg.IndexOf("PROCEDURE", System.StringComparison.OrdinalIgnoreCase) >= 0
                || msg.IndexOf("doesn't exist", System.StringComparison.OrdinalIgnoreCase) >= 0
                || msg.IndexOf("not found", System.StringComparison.OrdinalIgnoreCase) >= 0;
        }

        // --- SALVAMENTO: tenta stored procedure, faz fallback inline ---
        public int SalvarSimulacao(Universo u, string nomeSimulacao, int numInterac, int numTempoInterac)
        {
            try
            {
                return SalvarSimulacaoUsingProc(u, nomeSimulacao, numInterac, numTempoInterac);
            }
            catch (Exception ex)
            {
                if (IsProcedureMissing(ex))
                {
                    return SalvarSimulacaoInline(u, nomeSimulacao, numInterac, numTempoInterac);
                }
                MessageBox.Show($"Erro ao salvar no MySQL (proc): {ex.Message}", "Erro de BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        private int SalvarSimulacaoUsingProc(Universo u, string nomeSimulacao, int numInterac, int numTempoInterac)
        {
            int idSimulacao = -1;
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    using (var cmd = new MySqlCommand("sp_InserirSimulacao", conn, tx))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = DefaultCmdTimeout;
                        cmd.Parameters.Add("p_nome", MySqlDbType.VarChar, 255).Value = nomeSimulacao ?? string.Empty;
                        cmd.Parameters.Add("p_totalCorpos", MySqlDbType.Int32).Value = u.QtdCorp;
                        cmd.Parameters.Add("p_numInterac", MySqlDbType.Int32).Value = numInterac;
                        cmd.Parameters.Add("p_tempoInterac", MySqlDbType.Int32).Value = numTempoInterac;
                        var outParam = cmd.Parameters.Add("p_id", MySqlDbType.Int32);
                        outParam.Direction = ParameterDirection.Output;
                        cmd.ExecuteNonQuery();
                        if (outParam.Value != DBNull.Value) idSimulacao = Convert.ToInt32(outParam.Value);
                    }

                    if (idSimulacao > 0)
                    {
                        using (var cmd = new MySqlCommand("sp_InserirCorpoIteracao", conn, tx))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = DefaultCmdTimeout;
                            cmd.Parameters.Add("p_idSim", MySqlDbType.Int32);
                            cmd.Parameters.Add("p_iterNum", MySqlDbType.Int32);
                            cmd.Parameters.Add("p_nome", MySqlDbType.VarChar, 255);
                            cmd.Parameters.Add("p_massa", MySqlDbType.Double);
                            cmd.Parameters.Add("p_dens", MySqlDbType.Double);
                            cmd.Parameters.Add("p_pX", MySqlDbType.Double);
                            cmd.Parameters.Add("p_pY", MySqlDbType.Double);
                            cmd.Parameters.Add("p_vX", MySqlDbType.Double);
                            cmd.Parameters.Add("p_vY", MySqlDbType.Double);

                            foreach (var corpo in u.ListaCorp)
                            {
                                cmd.Parameters["p_idSim"].Value = idSimulacao;
                                cmd.Parameters["p_iterNum"].Value = 0;
                                cmd.Parameters["p_nome"].Value = corpo.Nome ?? string.Empty;
                                cmd.Parameters["p_massa"].Value = corpo.Massa;
                                cmd.Parameters["p_dens"].Value = corpo.Densidade;
                                cmd.Parameters["p_pX"].Value = corpo.PosX;
                                cmd.Parameters["p_pY"].Value = corpo.PosY;
                                cmd.Parameters["p_vX"].Value = corpo.VelX;
                                cmd.Parameters["p_vY"].Value = corpo.VelY;
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    tx.Commit();
                }
            }
            return idSimulacao;
        }

        private int SalvarSimulacaoInline(Universo u, string nomeSimulacao, int numInterac, int numTempoInterac)
        {
            int idSim = -1;
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    try
                    {
                        string sql = "INSERT INTO Simulacao (NomeSimulacao, TotalCorpos, NumInterac, NumTempoInterac, DataGravacao) VALUES (@nome, @total, @nI, @nT, NOW())";
                        using (var cmd = new MySqlCommand(sql, conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@nome", nomeSimulacao);
                            cmd.Parameters.AddWithValue("@total", u.QtdCorp);
                            cmd.Parameters.AddWithValue("@nI", numInterac);
                            cmd.Parameters.AddWithValue("@nT", numTempoInterac);
                            cmd.ExecuteNonQuery();
                            idSim = (int)cmd.LastInsertedId;
                        }

                        if (idSim > 0)
                        {
                            string sqlC = "INSERT INTO CorpoIteracao (IdSimulacao, IteracaoNum, Nome, Massa, Densidade, PosX, PosY, VelX, VelY) VALUES (@id, @iter, @nome, @massa, @dens, @pX, @pY, @vX, @vY)";
                            using (var cmd = new MySqlCommand(sqlC, conn, tx))
                            {
                                cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = idSim;
                                cmd.Parameters.Add("@iter", MySqlDbType.Int32);
                                cmd.Parameters.Add("@nome", MySqlDbType.VarChar);
                                cmd.Parameters.Add("@massa", MySqlDbType.Double);
                                cmd.Parameters.Add("@dens", MySqlDbType.Double);
                                cmd.Parameters.Add("@pX", MySqlDbType.Double);
                                cmd.Parameters.Add("@pY", MySqlDbType.Double);
                                cmd.Parameters.Add("@vX", MySqlDbType.Double);
                                cmd.Parameters.Add("@vY", MySqlDbType.Double);
                                cmd.Prepare();
                                foreach (var corpo in u.ListaCorp)
                                {
                                    cmd.Parameters["@iter"].Value = 0;
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
                        tx.Commit();
                        return idSim;
                    }
                    catch (Exception ex)
                    {
                        try { tx.Rollback(); } catch { }
                        MessageBox.Show($"Erro ao salvar no MySQL (inline): {ex.Message}", "Erro de BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return -1;
                    }
                }
            }
        }

        // --- SALVAR ITERAÇÃO ---
        public bool SalvarIteracao(int idSimulacao, int iteracaoNum, Universo u)
        {
            try { return SalvarIteracaoUsingProc(idSimulacao, iteracaoNum, u); }
            catch (Exception ex)
            {
                if (IsProcedureMissing(ex)) return SalvarIteracaoInline(idSimulacao, iteracaoNum, u);
                MessageBox.Show($"Erro ao salvar iteração no MySQL (proc): {ex.Message}", "Erro de BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool SalvarIteracaoUsingProc(int idSimulacao, int iteracaoNum, Universo u)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    using (var cmd = new MySqlCommand("sp_InserirCorpoIteracao", conn, tx))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = DefaultCmdTimeout;
                        cmd.Parameters.Add("p_idSim", MySqlDbType.Int32);
                        cmd.Parameters.Add("p_iterNum", MySqlDbType.Int32);
                        cmd.Parameters.Add("p_nome", MySqlDbType.VarChar, 255);
                        cmd.Parameters.Add("p_massa", MySqlDbType.Double);
                        cmd.Parameters.Add("p_dens", MySqlDbType.Double);
                        cmd.Parameters.Add("p_pX", MySqlDbType.Double);
                        cmd.Parameters.Add("p_pY", MySqlDbType.Double);
                        cmd.Parameters.Add("p_vX", MySqlDbType.Double);
                        cmd.Parameters.Add("p_vY", MySqlDbType.Double);

                        foreach (var corpo in u.ListaCorp)
                        {
                            cmd.Parameters["p_idSim"].Value = idSimulacao;
                            cmd.Parameters["p_iterNum"].Value = iteracaoNum;
                            cmd.Parameters["p_nome"].Value = corpo.Nome ?? string.Empty;
                            cmd.Parameters["p_massa"].Value = corpo.Massa;
                            cmd.Parameters["p_dens"].Value = corpo.Densidade;
                            cmd.Parameters["p_pX"].Value = corpo.PosX;
                            cmd.Parameters["p_pY"].Value = corpo.PosY;
                            cmd.Parameters["p_vX"].Value = corpo.VelX;
                            cmd.Parameters["p_vY"].Value = corpo.VelY;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                    return true;
                }
            }
        }

        private bool SalvarIteracaoInline(int idSimulacao, int iteracaoNum, Universo u)
        {
            if (idSimulacao <= 0) return false;
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    try
                    {
                        string sql = "INSERT INTO CorpoIteracao (IdSimulacao, IteracaoNum, Nome, Massa, Densidade, PosX, PosY, VelX, VelY) VALUES (@id, @iter, @nome, @massa, @dens, @pX, @pY, @vX, @vY)";
                        using (var cmd = new MySqlCommand(sql, conn, tx))
                        {
                            cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = idSimulacao;
                            cmd.Parameters.Add("@iter", MySqlDbType.Int32);
                            cmd.Parameters.Add("@nome", MySqlDbType.VarChar);
                            cmd.Parameters.Add("@massa", MySqlDbType.Double);
                            cmd.Parameters.Add("@dens", MySqlDbType.Double);
                            cmd.Parameters.Add("@pX", MySqlDbType.Double);
                            cmd.Parameters.Add("@pY", MySqlDbType.Double);
                            cmd.Parameters.Add("@vX", MySqlDbType.Double);
                            cmd.Parameters.Add("@vY", MySqlDbType.Double);
                            cmd.Prepare();

                            foreach (var corpo in u.ListaCorp)
                            {
                                cmd.Parameters["@iter"].Value = iteracaoNum;
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
                        tx.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        try { tx.Rollback(); } catch { }
                        MessageBox.Show($"Erro ao salvar iteração no MySQL (inline): {ex.Message}", "Erro de BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }
        }

        // --- CARREGAMENTO ---
        public Universo CarregarSimulacao(int idSimulacao, out int numInterac, out int numTempoInterac)
        {
            try { return CarregarSimulacaoUsingProc(idSimulacao, out numInterac, out numTempoInterac); }
            catch (Exception ex)
            {
                if (IsProcedureMissing(ex)) return CarregarSimulacaoInline(idSimulacao, out numInterac, out numTempoInterac);
                MessageBox.Show($"Erro ao carregar do MySQL (proc): {ex.Message}", "Erro de BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                numInterac = 0; numTempoInterac = 0; return null;
            }
        }

        private Universo CarregarSimulacaoUsingProc(int idSimulacao, out int numInterac, out int numTempoInterac)
        {
            numInterac = 0; numTempoInterac = 0;
            var u = new Universo();
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("sp_ObterSimulacaoMeta", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = DefaultCmdTimeout;
                    cmd.Parameters.AddWithValue("p_idSim", idSimulacao);
                    var outN = cmd.Parameters.Add("p_numInterac", MySqlDbType.Int32); outN.Direction = ParameterDirection.Output;
                    var outT = cmd.Parameters.Add("p_tempoInterac", MySqlDbType.Int32); outT.Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();
                    if (outN.Value != DBNull.Value) numInterac = Convert.ToInt32(outN.Value);
                    if (outT.Value != DBNull.Value) numTempoInterac = Convert.ToInt32(outT.Value);
                }
                using (var cmd = new MySqlCommand("sp_ObterIteracao", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure; cmd.CommandTimeout = DefaultCmdTimeout;
                    cmd.Parameters.AddWithValue("p_idSim", idSimulacao); cmd.Parameters.AddWithValue("p_iterNum", 0);
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            var c = new Corpos(
                                rdr.GetString("Nome"),
                                rdr.GetDouble("Massa"),
                                rdr.GetDouble("PosX"),
                                rdr.GetDouble("PosY"),
                                rdr.GetDouble("VelX"),
                                rdr.GetDouble("VelY"),
                                rdr.GetDouble("Densidade")
                            );
                            u.ListaCorp.Add(c);
                        }
                    }
                }
            }
            return u;
        }

        private Universo CarregarSimulacaoInline(int idSimulacao, out int numInterac, out int numTempoInterac)
        {
            numInterac = 0; numTempoInterac = 0; var u = new Universo();
            using (var conn = new MySqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                    string sql = "SELECT NumInterac, NumTempoInterac FROM Simulacao WHERE IdSimulacao = @id";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idSimulacao);
                        using (var rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read()) { numInterac = rdr.GetInt32("NumInterac"); numTempoInterac = rdr.GetInt32("NumTempoInterac"); }
                            else { MessageBox.Show($"Simulação com ID {idSimulacao} não encontrada.", "Erro de Carregamento", MessageBoxButtons.OK, MessageBoxIcon.Error); return null; }
                        }
                    }
                    string sqlC = "SELECT Nome, Massa, Densidade, PosX, PosY, VelX, VelY FROM CorpoIteracao WHERE IdSimulacao = @id AND IteracaoNum = 0 ORDER BY Id";
                    using (var cmd = new MySqlCommand(sqlC, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idSimulacao);
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                var c = new Corpos(rdr.GetString("Nome"), rdr.GetDouble("Massa"), rdr.GetDouble("PosX"), rdr.GetDouble("PosY"), rdr.GetDouble("VelX"), rdr.GetDouble("VelY"), rdr.GetDouble("Densidade"));
                                u.ListaCorp.Add(c);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao carregar do MySQL (inline): {ex.Message}", "Erro de BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
            return u;
        }

        // --- CARREGAR ITERAÇÃO ---
        public Universo CarregarIteracao(int idSimulacao, int iteracaoNum)
        {
            try { return CarregarIteracaoUsingProc(idSimulacao, iteracaoNum); }
            catch (Exception ex) { if (IsProcedureMissing(ex)) return CarregarIteracaoInline(idSimulacao, iteracaoNum); MessageBox.Show($"Erro ao carregar iteração do MySQL (proc): {ex.Message}", "Erro de BD", MessageBoxButtons.OK, MessageBoxIcon.Error); return null; }
        }

        private Universo CarregarIteracaoUsingProc(int idSimulacao, int iteracaoNum)
        {
            if (idSimulacao <= 0) return null; var u = new Universo();
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("sp_ObterIteracao", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure; cmd.CommandTimeout = DefaultCmdTimeout;
                    cmd.Parameters.AddWithValue("p_idSim", idSimulacao); cmd.Parameters.AddWithValue("p_iterNum", iteracaoNum);
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read()) u.ListaCorp.Add(new Corpos(rdr.GetString("Nome"), rdr.GetDouble("Massa"), rdr.GetDouble("PosX"), rdr.GetDouble("PosY"), rdr.GetDouble("VelX"), rdr.GetDouble("VelY"), rdr.GetDouble("Densidade")));
                    }
                }
            }
            return u;
        }

        private Universo CarregarIteracaoInline(int idSimulacao, int iteracaoNum)
        {
            if (idSimulacao <= 0) return null; var u = new Universo();
            using (var conn = new MySqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                    string sql = "SELECT Nome, Massa, Densidade, PosX, PosY, VelX, VelY FROM CorpoIteracao WHERE IdSimulacao = @id AND IteracaoNum = @iter ORDER BY Id";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idSimulacao); cmd.Parameters.AddWithValue("@iter", iteracaoNum);
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read()) u.ListaCorp.Add(new Corpos(rdr.GetString("Nome"), rdr.GetDouble("Massa"), rdr.GetDouble("PosX"), rdr.GetDouble("PosY"), rdr.GetDouble("VelX"), rdr.GetDouble("VelY"), rdr.GetDouble("Densidade")));
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao carregar iteração do MySQL (inline): {ex.Message}", "Erro de BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
            return u;
        }

        // --- LISTAR ITERAÇÕES ---
        public List<int> ListarIteracoes(int idSimulacao)
        {
            try { return ListarIteracoesUsingProc(idSimulacao); }
            catch (Exception ex) { if (IsProcedureMissing(ex)) return ListarIteracoesInline(idSimulacao); return new List<int>(); }
        }

        private List<int> ListarIteracoesUsingProc(int idSimulacao)
        {
            var lista = new List<int>(); if (idSimulacao <= 0) return lista;
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("sp_ListarIteracoes", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure; cmd.CommandTimeout = DefaultCmdTimeout; cmd.Parameters.AddWithValue("p_idSim", idSimulacao);
                    using (var rdr = cmd.ExecuteReader()) while (rdr.Read()) lista.Add(rdr.GetInt32(0));
                }
            }
            return lista;
        }

        private List<int> ListarIteracoesInline(int idSimulacao)
        {
            var lista = new List<int>(); if (idSimulacao <= 0) return lista;
            using (var conn = new MySqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open(); string sql = "SELECT DISTINCT IteracaoNum FROM CorpoIteracao WHERE IdSimulacao = @id ORDER BY IteracaoNum";
                    using (var cmd = new MySqlCommand(sql, conn)) { cmd.Parameters.AddWithValue("@id", idSimulacao); using (var rdr = cmd.ExecuteReader()) while (rdr.Read()) lista.Add(rdr.GetInt32(0)); }
                }
                catch { }
            }
            return lista;
        }

        // --- EXPORTAÇÃO: pendentes e inline ---
        public void ExportPendingSimulacoes()
        {
            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("sp_ListarSimulacoesPendentes", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure; cmd.CommandTimeout = DefaultCmdTimeout;
                        using (var rdr = cmd.ExecuteReader())
                        {
                            var ids = new List<int>(); while (rdr.Read()) ids.Add(rdr.GetInt32(0)); rdr.Close();
                            foreach (var id in ids) { try { ExportSimulacaoById(id); } catch (Exception e) { Console.WriteLine($"Falha export {id}: {e.Message}"); } }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (IsProcedureMissing(ex)) { ExportPendingSimulacoesInline(); return; }
                MessageBox.Show($"Erro ao listar simulações pendentes: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportSimulacaoById(int idSimulacao)
        {
            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("sp_ExportSimulacaoCompleta", conn)) { cmd.CommandType = CommandType.StoredProcedure; cmd.CommandTimeout = DefaultCmdTimeout; cmd.Parameters.AddWithValue("p_idSim", idSimulacao); cmd.ExecuteNonQuery(); }
                }
            }
            catch (Exception ex)
            {
                if (IsProcedureMissing(ex)) { ExportSimulacaoInline(idSimulacao); return; }
                throw;
            }
        }

        private void ExportPendingSimulacoesInline()
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                string sql = "SELECT s.IdSimulacao FROM Simulacao s LEFT JOIN ExportStatus e ON s.IdSimulacao = e.IdSimulacao WHERE (e.Exportado IS NULL OR e.Exportado = 0) AND s.TotalCorpos >= 3";
                using (var cmd = new MySqlCommand(sql, conn)) using (var rdr = cmd.ExecuteReader()) { var ids = new List<int>(); while (rdr.Read()) ids.Add(rdr.GetInt32(0)); rdr.Close(); foreach (var id in ids) { try { ExportSimulacaoInline(id); } catch (Exception e) { Console.WriteLine($"Falha export inline {id}: {e.Message}"); } } }
            }
        }

        private void ExportSimulacaoInline(int idSimulacao)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    try
                    {
                        string sqlMeta = "SELECT IdSimulacao, NomeSimulacao, TotalCorpos, NumInterac, NumTempoInterac, DataGravacao FROM Simulacao WHERE IdSimulacao = @id";
                        using (var cmd = new MySqlCommand(sqlMeta, conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@id", idSimulacao);

                            using (var readerMeta = cmd.ExecuteReader())
                            {
                                if (!readerMeta.Read())
                                {
                                    readerMeta.Close();
                                    tx.Rollback();
                                    return; // no such sim
                                }

                                int total = readerMeta.GetInt32("TotalCorpos");
                                if (total < 3)
                                {
                                    readerMeta.Close();
                                    UpsertExportStatus(conn, tx, idSimulacao, true, null);
                                    tx.Commit();
                                    return;
                                }

                                string nome = readerMeta.GetString("NomeSimulacao");
                                int nI = readerMeta.GetInt32("NumInterac");
                                int nT = readerMeta.GetInt32("NumTempoInterac");
                                DateTime data = readerMeta.GetDateTime("DataGravacao");
                                readerMeta.Close();

                                string insertSim = $"INSERT INTO {EvaluatorSchema}.Simulacao (IdSimulacao, NomeSimulacao, TotalCorpos, NumInterac, NumTempoInterac, DataGravacao) VALUES (@id, @nome, @total, @nI, @nT, @data) ON DUPLICATE KEY UPDATE NomeSimulacao=VALUES(NomeSimulacao)";
                                using (var ins = new MySqlCommand(insertSim, conn, tx))
                                {
                                    ins.Parameters.AddWithValue("@id", idSimulacao);
                                    ins.Parameters.AddWithValue("@nome", nome);
                                    ins.Parameters.AddWithValue("@total", total);
                                    ins.Parameters.AddWithValue("@nI", nI);
                                    ins.Parameters.AddWithValue("@nT", nT);
                                    ins.Parameters.AddWithValue("@data", data);
                                    ins.ExecuteNonQuery();
                                }

                                string sqlC = "SELECT Nome, Massa, Densidade, PosX, PosY, VelX, VelY, IteracaoNum FROM CorpoIteracao WHERE IdSimulacao = @id ORDER BY IteracaoNum, Id";
                                using (var cmdC = new MySqlCommand(sqlC, conn, tx))
                                {
                                    cmdC.Parameters.AddWithValue("@id", idSimulacao);

                                    using (var readerCorpos = cmdC.ExecuteReader())
                                    {
                                        var rows = new List<CorpoRow>();
                                        while (readerCorpos.Read())
                                        {
                                            rows.Add(new CorpoRow
                                            {
                                                Nome = readerCorpos.GetString("Nome"),
                                                Massa = readerCorpos.GetDouble("Massa"),
                                                Densidade = readerCorpos.GetDouble("Densidade"),
                                                PosX = readerCorpos.GetDouble("PosX"),
                                                PosY = readerCorpos.GetDouble("PosY"),
                                                VelX = readerCorpos.GetDouble("VelX"),
                                                VelY = readerCorpos.GetDouble("VelY"),
                                                IteracaoNum = readerCorpos.GetInt32("IteracaoNum")
                                            });
                                        }
                                        readerCorpos.Close();

                                        string insertC = $"INSERT INTO {EvaluatorSchema}.CorpoIteracao (IdSimulacao, IteracaoNum, Nome, Massa, Densidade, PosX, PosY, VelX, VelY) VALUES (@id, @iter, @nome, @massa, @dens, @pX, @pY, @vX, @vY) ON DUPLICATE KEY UPDATE Nome=VALUES(Nome)";
                                        using (var insC = new MySqlCommand(insertC, conn, tx))
                                        {
                                            insC.Parameters.Add("@id", MySqlDbType.Int32);
                                            insC.Parameters.Add("@iter", MySqlDbType.Int32);
                                            insC.Parameters.Add("@nome", MySqlDbType.VarChar);
                                            insC.Parameters.Add("@massa", MySqlDbType.Double);
                                            insC.Parameters.Add("@dens", MySqlDbType.Double);
                                            insC.Parameters.Add("@pX", MySqlDbType.Double);
                                            insC.Parameters.Add("@pY", MySqlDbType.Double);
                                            insC.Parameters.Add("@vX", MySqlDbType.Double);
                                            insC.Parameters.Add("@vY", MySqlDbType.Double);

                                            foreach (var r in rows)
                                            {
                                                insC.Parameters["@id"].Value = idSimulacao;
                                                insC.Parameters["@iter"].Value = r.IteracaoNum;
                                                insC.Parameters["@nome"].Value = r.Nome;
                                                insC.Parameters["@massa"].Value = r.Massa;
                                                insC.Parameters["@dens"].Value = r.Densidade;
                                                insC.Parameters["@pX"].Value = r.PosX;
                                                insC.Parameters["@pY"].Value = r.PosY;
                                                insC.Parameters["@vX"].Value = r.VelX;
                                                insC.Parameters["@vY"].Value = r.VelY;
                                                insC.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                }

                                UpsertExportStatus(conn, tx, idSimulacao, true, null);
                            }
                        }

                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        try { tx.Rollback(); } catch { }
                        UpsertExportStatus(conn, tx, idSimulacao, false, ex.Message);
                        throw;
                    }
                }
            }
        }

        private void UpsertExportStatus(MySqlConnection conn, MySqlTransaction tx, int idSimulacao, bool exported, string lastError)
        {
            string sql = "INSERT INTO ExportStatus (IdSimulacao, Exportado, LastAttempt, AttemptCount, LastError) VALUES (@id, @exp, NOW(), 1, @err) ON DUPLICATE KEY UPDATE Exportado = VALUES(Exportado), LastAttempt = NOW(), AttemptCount = AttemptCount + 1, LastError = VALUES(LastError)";
            using (var cmd = new MySqlCommand(sql, conn, tx))
            {
                cmd.Parameters.AddWithValue("@id", idSimulacao);
                cmd.Parameters.AddWithValue("@exp", exported ? 1 : 0);
                cmd.Parameters.AddWithValue("@err", (object)lastError ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }

        private class CorpoRow { public string Nome; public double Massa; public double Densidade; public double PosX; public double PosY; public double VelX; public double VelY; public int IteracaoNum; }

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
            numInterac = 0; numTempoInterac = 0;
            if (int.TryParse(idSimulacaoStr, out int idSimulacao)) return CarregarSimulacao(idSimulacao, out numInterac, out numTempoInterac);
            MessageBox.Show("O Gravador MySQL espera um ID numérico. Por favor, insira um ID válido.", "Erro de Carregamento", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return null;
        }
    }
}