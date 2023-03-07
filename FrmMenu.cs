using System;
using System.Drawing;
using System.Windows.Forms;
using Teste_SysMap.Controller;

namespace Teste_SysMap
{
    public partial class FrmMenu : Form
    {
        public static global::Usuario usuario = new global::Usuario();
        Tarefas tarefas = new Tarefas();
        public FrmMenu()
        {
            InitializeComponent();

            using (var frmlogin = new FrmLogin())
                frmlogin.ShowDialog();

            dataGridView1.DataSource = Tarefas.BuscarTodos(usuario);
            ConfigurarGrade();

            LblNome.Text = usuario.Nome;
            CarregarFotoPerfil();
        }

        private void ConfigurarGrade()
        {
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Arial", 9);
            dataGridView1.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9);

            dataGridView1.Columns["id"].HeaderText = "Cód Tarefa";
            dataGridView1.Columns["id"].Width = 110;
            dataGridView1.Columns["id"].Visible = true;

            dataGridView1.Columns["nome"].HeaderText = "Nome";
            dataGridView1.Columns["nome"].Width = 150;
            dataGridView1.Columns["nome"].ReadOnly = true;
            dataGridView1.Columns["nome"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns["email"].HeaderText = "Email";
            dataGridView1.Columns["email"].Width = 200;
            dataGridView1.Columns["email"].ReadOnly = true;
            dataGridView1.Columns["email"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns["descricao"].HeaderText = "Descrição";
            dataGridView1.Columns["descricao"].Width = 250;
            dataGridView1.Columns["descricao"].ReadOnly = true;

            dataGridView1.Columns["data_hora"].HeaderText = "Data";
            dataGridView1.Columns["data_hora"].Width = 130;
            dataGridView1.Columns["data_hora"].ReadOnly = true;
            dataGridView1.Columns["data_hora"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns["id_Usuario"].HeaderText = "id_Usuario";
            dataGridView1.Columns["id_Usuario"].Width = 80;
            dataGridView1.Columns["id_Usuario"].Visible = false;

            this.dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            this.dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

        }

        private void FrmMenu_Load(object sender, EventArgs e)
        {
            if (usuario.Id == 0)
            {
                Close();
            }

            if (usuario.Email == "contato@sistemas.com.br")
            {
                cadastroToolStripMenuItem.Enabled = true;
            }
        }

        private void cadastroToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FrmUsuario frmUsuario = new FrmUsuario())
                frmUsuario.ShowDialog();
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            string id = "", email = "", nome = "";
           
            if (TxtCodRegistro.Text != "") id = TxtCodRegistro.Text;
            if (TxtEmail.Text != "") email = TxtEmail.Text;
            if (TxtNome.Text != "") nome = TxtNome.Text;

            if (TxtCodRegistro.Text == "" && TxtEmail.Text == "" && TxtNome.Text == "") dataGridView1.DataSource = Tarefas.BuscarTodos(usuario);
            else dataGridView1.DataSource = Tarefas.Pesquisa(usuario, id, email, nome);
        }
        private void CarregarFotoPerfil()
        {
            byte[] fotoPerfil = usuario.GetFotoPerfil(usuario);
            if (fotoPerfil != null)
            {
                Image imagem = ImageHelper.ByteArrayToImage(fotoPerfil);
                PicturePerfilProfile.Image = ImageHelper.CreateCircularImage(imagem);
                byte[] fotoPerfilEmBytes = ImageHelper.ImageToByteArray(imagem);
            }
        }

        private void BtnEditarPerfil_Click(object sender, EventArgs e)
        {
            using (var frm = new FrmUsuarioCadastro(usuario, Operacao.Alterar))
            {
                frm.ShowDialog();
            }
        }
    }
}
