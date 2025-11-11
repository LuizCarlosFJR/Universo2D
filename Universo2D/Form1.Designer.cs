namespace Universo
{
    partial class Form1
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlControles = new System.Windows.Forms.Panel();
            this.gbStatus = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label10 = new System.Windows.Forms.Label();
            this.txtProporcao = new System.Windows.Forms.TextBox();
            this.qtdCorposAtual = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.gbPersistencia = new System.Windows.Forms.GroupBox();
            this.btn_testaMySQL = new System.Windows.Forms.Button();
            this.rb_mysql = new System.Windows.Forms.RadioButton();
            this.rb_texto = new System.Windows.Forms.RadioButton();
            this.btn_carrega = new System.Windows.Forms.Button();
            this.btn_grava_ini = new System.Windows.Forms.Button();
            this.btn_grava = new System.Windows.Forms.Button();
            this.gbSimulacao = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btn_executa = new System.Windows.Forms.Button();
            this.qtdInterac = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.qtdTempoInterac = new System.Windows.Forms.TextBox();
            this.gbConfiguracao = new System.Windows.Forms.GroupBox();
            this.btn_aleatorio = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.valXMax = new System.Windows.Forms.TextBox();
            this.qtdCorpos = new System.Windows.Forms.TextBox();
            this.valYMax = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.masMin = new System.Windows.Forms.TextBox();
            this.masMax = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.pnlControles.SuspendLayout();
            this.gbStatus.SuspendLayout();
            this.gbPersistencia.SuspendLayout();
            this.gbSimulacao.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.gbConfiguracao.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlControles
            // 
            this.pnlControles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlControles.BackColor = System.Drawing.Color.White;
            this.pnlControles.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlControles.Controls.Add(this.gbStatus);
            this.pnlControles.Controls.Add(this.gbPersistencia);
            this.pnlControles.Controls.Add(this.gbSimulacao);
            this.pnlControles.Controls.Add(this.gbConfiguracao);
            this.pnlControles.Location = new System.Drawing.Point(945, 12);
            this.pnlControles.Name = "pnlControles";
            this.pnlControles.Size = new System.Drawing.Size(409, 478);
            this.pnlControles.TabIndex = 13;
            // 
            // gbStatus
            // 
            this.gbStatus.Controls.Add(this.label1);
            this.gbStatus.Controls.Add(this.progressBar1);
            this.gbStatus.Controls.Add(this.label10);
            this.gbStatus.Controls.Add(this.txtProporcao);
            this.gbStatus.Controls.Add(this.qtdCorposAtual);
            this.gbStatus.Controls.Add(this.label9);
            this.gbStatus.Location = new System.Drawing.Point(13, 353);
            this.gbStatus.Name = "gbStatus";
            this.gbStatus.Size = new System.Drawing.Size(381, 111);
            this.gbStatus.TabIndex = 28;
            this.gbStatus.TabStop = false;
            this.gbStatus.Text = "Status da Simulação";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Progresso:";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(18, 76);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(344, 23);
            this.progressBar1.TabIndex = 13;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(15, 25);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(72, 13);
            this.label10.TabIndex = 22;
            this.label10.Text = "Corpos Atuais";
            // 
            // txtProporcao
            // 
            this.txtProporcao.Location = new System.Drawing.Point(262, 41);
            this.txtProporcao.Name = "txtProporcao";
            this.txtProporcao.ReadOnly = true;
            this.txtProporcao.Size = new System.Drawing.Size(100, 20);
            this.txtProporcao.TabIndex = 20;
            // 
            // qtdCorposAtual
            // 
            this.qtdCorposAtual.Location = new System.Drawing.Point(18, 41);
            this.qtdCorposAtual.Name = "qtdCorposAtual";
            this.qtdCorposAtual.ReadOnly = true;
            this.qtdCorposAtual.Size = new System.Drawing.Size(100, 20);
            this.qtdCorposAtual.TabIndex = 23;
            this.qtdCorposAtual.Text = "0";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(259, 25);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(92, 13);
            this.label9.TabIndex = 21;
            this.label9.Text = "Proporção (Zoom)";
            // 
            // gbPersistencia
            // 
            this.gbPersistencia.Controls.Add(this.btn_testaMySQL);
            this.gbPersistencia.Controls.Add(this.rb_mysql);
            this.gbPersistencia.Controls.Add(this.rb_texto);
            this.gbPersistencia.Controls.Add(this.btn_carrega);
            this.gbPersistencia.Controls.Add(this.btn_grava_ini);
            this.gbPersistencia.Controls.Add(this.btn_grava);
            this.gbPersistencia.Location = new System.Drawing.Point(13, 242);
            this.gbPersistencia.Name = "gbPersistencia";
            this.gbPersistencia.Size = new System.Drawing.Size(381, 105);
            this.gbPersistencia.TabIndex = 27;
            this.gbPersistencia.TabStop = false;
            this.gbPersistencia.Text = "Persistência";
            // 
            // btn_testaMySQL
            // 
            this.btn_testaMySQL.Location = new System.Drawing.Point(18, 70);
            this.btn_testaMySQL.Name = "btn_testaMySQL";
            this.btn_testaMySQL.Size = new System.Drawing.Size(120, 23);
            this.btn_testaMySQL.TabIndex = 23;
            this.btn_testaMySQL.Text = "Testar MySQL";
            this.btn_testaMySQL.UseVisualStyleBackColor = true;
            this.btn_testaMySQL.Click += new System.EventHandler(this.btn_testaMySQL_Click);
            // 
            // rb_mysql
            // 
            this.rb_mysql.AutoSize = true;
            this.rb_mysql.Location = new System.Drawing.Point(18, 48);
            this.rb_mysql.Name = "rb_mysql";
            this.rb_mysql.Size = new System.Drawing.Size(60, 17);
            this.rb_mysql.TabIndex = 22;
            this.rb_mysql.Text = "MySQL";
            this.rb_mysql.UseVisualStyleBackColor = true;
            this.rb_mysql.CheckedChanged += new System.EventHandler(this.rb_mysql_CheckedChanged);
            // 
            // rb_texto
            // 
            this.rb_texto.AutoSize = true;
            this.rb_texto.Checked = true;
            this.rb_texto.Location = new System.Drawing.Point(18, 25);
            this.rb_texto.Name = "rb_texto";
            this.rb_texto.Size = new System.Drawing.Size(106, 17);
            this.rb_texto.TabIndex = 13;
            this.rb_texto.TabStop = true;
            this.rb_texto.Text = "Arquivo de Texto";
            this.rb_texto.UseVisualStyleBackColor = true;
            this.rb_texto.CheckedChanged += new System.EventHandler(this.rb_texto_CheckedChanged);
            // 
            // btn_carrega
            // 
            this.btn_carrega.Location = new System.Drawing.Point(262, 25);
            this.btn_carrega.Name = "btn_carrega";
            this.btn_carrega.Size = new System.Drawing.Size(100, 23);
            this.btn_carrega.TabIndex = 10;
            this.btn_carrega.Text = "Carregar";
            this.btn_carrega.UseVisualStyleBackColor = true;
            this.btn_carrega.Click += new System.EventHandler(this.btn_carrega_Click);
            // 
            // btn_grava_ini
            // 
            this.btn_grava_ini.Location = new System.Drawing.Point(146, 54);
            this.btn_grava_ini.Name = "btn_grava_ini";
            this.btn_grava_ini.Size = new System.Drawing.Size(216, 23);
            this.btn_grava_ini.TabIndex = 12;
            this.btn_grava_ini.Text = "Gravar Configuração Inicial";
            this.btn_grava_ini.UseVisualStyleBackColor = true;
            this.btn_grava_ini.Click += new System.EventHandler(this.btn_grava_ini_Click);
            // 
            // btn_grava
            // 
            this.btn_grava.Location = new System.Drawing.Point(146, 25);
            this.btn_grava.Name = "btn_grava";
            this.btn_grava.Size = new System.Drawing.Size(100, 23);
            this.btn_grava.TabIndex = 11;
            this.btn_grava.Text = "Gravar Simulação";
            this.btn_grava.UseVisualStyleBackColor = true;
            this.btn_grava.Click += new System.EventHandler(this.btn_grava_Click);
            // 
            // gbSimulacao
            // 
            this.gbSimulacao.Controls.Add(this.label4);
            this.gbSimulacao.Controls.Add(this.btn_executa);
            this.gbSimulacao.Controls.Add(this.qtdInterac);
            this.gbSimulacao.Controls.Add(this.groupBox2);
            this.gbSimulacao.Controls.Add(this.label5);
            this.gbSimulacao.Controls.Add(this.qtdTempoInterac);
            this.gbSimulacao.Location = new System.Drawing.Point(13, 122);
            this.gbSimulacao.Name = "gbSimulacao";
            this.gbSimulacao.Size = new System.Drawing.Size(381, 114);
            this.gbSimulacao.TabIndex = 26;
            this.gbSimulacao.TabStop = false;
            this.gbSimulacao.Text = "Controle da Simulação";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Num. Interações";
            // 
            // btn_executa
            // 
            this.btn_executa.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_executa.Location = new System.Drawing.Point(262, 41);
            this.btn_executa.Name = "btn_executa";
            this.btn_executa.Size = new System.Drawing.Size(100, 23);
            this.btn_executa.TabIndex = 9;
            this.btn_executa.Text = "INICIAR";
            this.btn_executa.UseVisualStyleBackColor = true;
            this.btn_executa.Click += new System.EventHandler(this.button1_Click);
            // 
            // qtdInterac
            // 
            this.qtdInterac.Location = new System.Drawing.Point(18, 41);
            this.qtdInterac.Name = "qtdInterac";
            this.qtdInterac.Size = new System.Drawing.Size(100, 20);
            this.qtdInterac.TabIndex = 2;
            this.qtdInterac.Text = "1000";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButton1);
            this.groupBox2.Controls.Add(this.radioButton2);
            this.groupBox2.Location = new System.Drawing.Point(18, 67);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(344, 37);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Modo de Execução";
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(15, 14);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(120, 17);
            this.radioButton1.TabIndex = 20;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Visual (Tempo Real)";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(159, 14);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(106, 17);
            this.radioButton2.TabIndex = 21;
            this.radioButton2.Text = "Rápida (sem vis.)";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(143, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(106, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Tempo por Interação";
            // 
            // qtdTempoInterac
            // 
            this.qtdTempoInterac.Location = new System.Drawing.Point(146, 41);
            this.qtdTempoInterac.Name = "qtdTempoInterac";
            this.qtdTempoInterac.Size = new System.Drawing.Size(100, 20);
            this.qtdTempoInterac.TabIndex = 3;
            this.qtdTempoInterac.Text = "1";
            // 
            // gbConfiguracao
            // 
            this.gbConfiguracao.Controls.Add(this.btn_aleatorio);
            this.gbConfiguracao.Controls.Add(this.label2);
            this.gbConfiguracao.Controls.Add(this.valXMax);
            this.gbConfiguracao.Controls.Add(this.qtdCorpos);
            this.gbConfiguracao.Controls.Add(this.valYMax);
            this.gbConfiguracao.Controls.Add(this.label3);
            this.gbConfiguracao.Controls.Add(this.label6);
            this.gbConfiguracao.Controls.Add(this.masMin);
            this.gbConfiguracao.Controls.Add(this.masMax);
            this.gbConfiguracao.Controls.Add(this.label7);
            this.gbConfiguracao.Controls.Add(this.label8);
            this.gbConfiguracao.Location = new System.Drawing.Point(13, 13);
            this.gbConfiguracao.Name = "gbConfiguracao";
            this.gbConfiguracao.Size = new System.Drawing.Size(381, 103);
            this.gbConfiguracao.TabIndex = 25;
            this.gbConfiguracao.TabStop = false;
            this.gbConfiguracao.Text = "Configuração do Universo";
            // 
            // btn_aleatorio
            // 
            this.btn_aleatorio.Location = new System.Drawing.Point(262, 64);
            this.btn_aleatorio.Name = "btn_aleatorio";
            this.btn_aleatorio.Size = new System.Drawing.Size(100, 23);
            this.btn_aleatorio.TabIndex = 8;
            this.btn_aleatorio.Text = "Gerar Corpos";
            this.btn_aleatorio.UseVisualStyleBackColor = true;
            this.btn_aleatorio.Click += new System.EventHandler(this.button2_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Qtd. de Corpos";
            // 
            // valXMax
            // 
            this.valXMax.Location = new System.Drawing.Point(146, 41);
            this.valXMax.Name = "valXMax";
            this.valXMax.Size = new System.Drawing.Size(100, 20);
            this.valXMax.TabIndex = 4;
            this.valXMax.Text = "1000";
            // 
            // qtdCorpos
            // 
            this.qtdCorpos.Location = new System.Drawing.Point(18, 41);
            this.qtdCorpos.Name = "qtdCorpos";
            this.qtdCorpos.Size = new System.Drawing.Size(100, 20);
            this.qtdCorpos.TabIndex = 1;
            this.qtdCorpos.Text = "100";
            // 
            // valYMax
            // 
            this.valYMax.Location = new System.Drawing.Point(262, 41);
            this.valYMax.Name = "valYMax";
            this.valYMax.Size = new System.Drawing.Size(100, 20);
            this.valYMax.TabIndex = 5;
            this.valYMax.Text = "800";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(143, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Limite X (mts)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(259, 25);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(69, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Limite Y (mts)";
            // 
            // masMin
            // 
            this.masMin.Location = new System.Drawing.Point(18, 79);
            this.masMin.Name = "masMin";
            this.masMin.Size = new System.Drawing.Size(100, 20);
            this.masMin.TabIndex = 6;
            this.masMin.Text = "10000";
            // 
            // masMax
            // 
            this.masMax.Location = new System.Drawing.Point(146, 79);
            this.masMax.Name = "masMax";
            this.masMax.Size = new System.Drawing.Size(100, 20);
            this.masMax.TabIndex = 7;
            this.masMax.Text = "500000";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 64);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(97, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "Massa Mínima (kg)";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(143, 64);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(98, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = "Massa Máxima (kg)";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1366, 745);
            this.Controls.Add(this.pnlControles);
            this.Name = "Form1";
            this.Text = "Simulador Gravitacional 2D";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.pnlControles.ResumeLayout(false);
            this.gbStatus.ResumeLayout(false);
            this.gbStatus.PerformLayout();
            this.gbPersistencia.ResumeLayout(false);
            this.gbPersistencia.PerformLayout();
            this.gbSimulacao.ResumeLayout(false);
            this.gbSimulacao.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.gbConfiguracao.ResumeLayout(false);
            this.gbConfiguracao.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_aleatorio;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox qtdCorpos;
        private System.Windows.Forms.TextBox qtdInterac;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox qtdTempoInterac;
        private System.Windows.Forms.Button btn_executa;
        private System.Windows.Forms.Button btn_grava;
        private System.Windows.Forms.Button btn_carrega;
        private System.Windows.Forms.Button btn_grava_ini;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox valYMax;
        private System.Windows.Forms.TextBox valXMax;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox masMax;
        private System.Windows.Forms.TextBox masMin;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtProporcao;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox qtdCorposAtual;
        private System.Windows.Forms.RadioButton rb_mysql;
        private System.Windows.Forms.Panel pnlControles;
        private System.Windows.Forms.GroupBox gbConfiguracao;
        private System.Windows.Forms.GroupBox gbSimulacao;
        private System.Windows.Forms.GroupBox gbPersistencia;
        private System.Windows.Forms.RadioButton rb_texto;
        private System.Windows.Forms.GroupBox gbStatus;
        private System.Windows.Forms.Button btn_testaMySQL;
    }
}