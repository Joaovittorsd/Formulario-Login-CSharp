using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Teste_SysMap.Controller;

namespace Teste_SysMap
{
    public partial class FrmUsuario : Form
    {
        Usuario usuario = new Usuario();
        public FrmUsuario()
        {
            InitializeComponent();
            dataGridView1.DataSource = Usuario.BuscarTodos();

            ConfigurarGrade();
        }

        private void ConfigurarGrade()
        {
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Arial", 9);
            dataGridView1.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9);

            dataGridView1.Columns["id"].HeaderText = "Cód Cadastro";
            dataGridView1.Columns["id"].Width = 110;
            dataGridView1.Columns["id"].Visible = false;

            dataGridView1.Columns["nome"].HeaderText = "Nome";
            dataGridView1.Columns["nome"].Width = 200;
            dataGridView1.Columns["nome"].ReadOnly = true;
            dataGridView1.Columns["nome"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns["dataNascimento"].HeaderText = "Data Nascimento";
            dataGridView1.Columns["dataNascimento"].Width = 150;
            dataGridView1.Columns["dataNascimento"].ReadOnly = true;

            dataGridView1.Columns["email"].HeaderText = "Email";
            dataGridView1.Columns["email"].Width = 250;
            dataGridView1.Columns["email"].ReadOnly = true;

            dataGridView1.Columns["senha"].HeaderText = "Senha";
            dataGridView1.Columns["senha"].Visible = false;

            dataGridView1.Columns["fotoPerfil"].HeaderText = "Foto Perfil";
            dataGridView1.Columns["fotoPerfil"].Visible = false;
        }

        private void EditarToolStripButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                TransferirGrade();

                using (var frm = new FrmUsuarioCadastro(usuario, Operacao.Alterar))
                {
                    frm.ShowDialog();
                    if (usuario.Id != -1)
                    {
                        dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["id"].Value = usuario.Id;
                        dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["nome"].Value = usuario.Nome;
                        dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["dataNascimento"].Value = usuario.DataNascimento;
                        dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["email"].Value = usuario.Email;
                        dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["senha"].Value = usuario.Senha;
                        dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["fotoPerfil"].Value = usuario.FotoPerfil;
                    }
                }
            }
        }
        private void TransferirGrade()
        {
            //Tranferir dados da grade(linha) para objeto
            usuario.Id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["id"].Value);
            usuario.Nome = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["nome"].Value.ToString();
            usuario.DataNascimento = Convert.ToDateTime(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["dataNascimento"].Value);
            usuario.Email = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["email"].Value.ToString();
            usuario.Senha = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["senha"].Value.ToString();
            byte[] bytesFotoPerfil = (byte[])(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["fotoPerfil"].Value);
            usuario.FotoPerfil = bytesFotoPerfil;

        }

        private void NovoToolStripButton_Click(object sender, EventArgs e)
        {
            ReiniciarCampos(usuario);

            FrmUsuarioCadastro frmUsuarioCadastro = new FrmUsuarioCadastro(usuario, Operacao.Adicionar);
            frmUsuarioCadastro.ShowDialog();
            dataGridView1.DataSource = Usuario.BuscarTodos();
        }

        private void ReiniciarCampos(Usuario usuario)
        {
            usuario.Id = 0;
            usuario.Nome = "";
            usuario.Email = "";
            usuario.Senha = "";
        }

        private void DeletarToolStripButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                TransferirGrade();

                using (var frmUsuarioCadastro = new FrmUsuarioCadastro(usuario, Operacao.Excluir))
                {
                    frmUsuarioCadastro.ShowDialog();

                    if (usuario.Id != -1)
                        dataGridView1.DataSource = Usuario.BuscarTodos();
                }
            }
        }

        private void ConsultarToolStripButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                TransferirGrade();

                using (var frmUsuarioCadastro = new FrmUsuarioCadastro(usuario, Operacao.Consultar))
                {
                    frmUsuarioCadastro.ShowDialog();

                    if (usuario.Id != -1)
                        dataGridView1.DataSource = Usuario.BuscarTodos();
                }
            }
        }
    }
}
