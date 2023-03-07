using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Teste_SysMap.Controller;

namespace Teste_SysMap
{
    public partial class FrmUsuarioCadastro : Form
    {
        Usuario usuario = new Usuario();
        public FrmUsuarioCadastro(Usuario usuario, Operacao operacao)
        {
            InitializeComponent();
            CarregarDadosUsuario(usuario);

            if (operacao == Operacao.Adicionar && usuario.Id == 0)
                this.Text += " - Adicionar";

            else if (operacao == Operacao.Alterar)
                this.Text += " - Alterar";
            else if (operacao == Operacao.Excluir)
            {
                this.Text += " - Excluir";
                TravarControle();
                BtnCadastrar.Visible = false;
                BtnDeletar.Visible = true;
            }
            else if (operacao == Operacao.Consultar)
            {
                this.Text += " - Consultar";
                TravarControle();
                BtnCadastrar.Visible = false;
            }

            this.usuario = usuario;
        }

        private void TravarControle()
        {
            BtnFoto.Visible = false;
            TxtNome.Enabled = false;
            TextBoxData.Enabled = false;
            TxtEmail.Enabled = false;
            TxtSenha.Enabled = false;
        }

        private void CarregarDadosUsuario(Usuario usuario)
        {
            LblId.Text = usuario.Id.ToString();
            TxtNome.Text = usuario.Nome;
            if (usuario.DataNascimento != Convert.ToDateTime("01/01/0001 00:00:00")) 
                TextBoxData.Text = usuario.DataNascimento.ToString("dd/MM/yyyy");
            TxtEmail.Text = usuario.Email;
            TxtSenha.Text = usuario.Senha;
            byte[] fotoPerfil = usuario.FotoPerfil;
            if (fotoPerfil != null)
            {
                Image imagem = ImageHelper.ByteArrayToImage(fotoPerfil);
                PicturePerfilProfile.Image = ImageHelper.CreateCircularImage(imagem);
            }
        }

        private void BtnFoto_Click(object sender, EventArgs e)
        {
            /// cria um objeto OpenFileDialog para selecionar arquivos de imagem
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // define o filtro para que apenas arquivos de imagem sejam selecionáveis
            openFileDialog.Filter = "Arquivos de imagem (*.jpg, *.jpeg, *.png, *.bmp) | *.jpg; *.jpeg; *.png; *.bmp";

            // abre o diálogo de seleção de arquivo de imagem e, se o usuário selecionar um arquivo, carrega-o na PictureBox
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // carrega a imagem selecionada na PictureBox
                using (Image image = Image.FromFile(openFileDialog.FileName))
                {
                    // verifica se a resolução da imagem é maior que 564x789 pixels
                    if (image.Width > 757 || image.Height > 890)
                    {
                        // exibe uma mensagem de erro informando que a imagem é grande demais
                        MessageBox.Show("A imagem selecionada é muito grande.");
                        return;
                    }

                    // define a forma da PictureBox como circular
                    PicturePerfilProfile.Image = ImageHelper.CreateCircularImage(image);
                }
            }
        }

        private void BtnCadastrar_Click(object sender, EventArgs e)
        {
            usuario.Id = Convert.ToInt32(LblId.Text);
            usuario.Nome = TxtNome.Text;
            if (DateTime.TryParseExact(TextBoxData.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataNascimento))
            {
                usuario.DataNascimento = dataNascimento;
            }
            else
            {
                MessageBox.Show("Data de nascimento inválida!");
                return;
            }
            usuario.Email = TxtEmail.Text;
            usuario.Senha = TxtSenha.Text;
            usuario.FotoPerfil = ImageHelper.ImageToByteArray(PicturePerfilProfile.Image); // converte a imagem em um array de bytes e atribui à propriedade FotoPerfil do objeto Usuario

            try
            {
                HttpStatusCode statusCode = usuario.Salvar(usuario);
                if (statusCode == HttpStatusCode.Created)
                {
                    if (usuario.Id == 0) MessageBox.Show("Usuário cadastrado com sucesso!");
                    else MessageBox.Show("Dados Atualizados com sucesso!");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Ocorreu um erro ao cadastrar o usuário.");
                }
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse response)
                {
                    string message = $"Erro ao cadastrar o usuário: {response.StatusCode} - {response.StatusDescription}";
                    MessageBox.Show(message);
                }
                else
                {
                    MessageBox.Show("Ocorreu um erro ao cadastrar o usuário.");
                }
            }
        }
       

        private void TextBoxData_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Obtém a posição do cursor na caixa de texto
            int cursorPosition = TextBoxData.SelectionStart;

            // Verifica se a tecla pressionada é um dígito ou uma tecla de backspace
            if (char.IsDigit(e.KeyChar))
            {
                // Se a string já tiver 8 dígitos, ignora a tecla pressionada
                if (TextBoxData.Text.Length == 10)
                {
                    e.Handled = true;
                    return;
                }

                // Insere uma / depois do 2º digito
                if (TextBoxData.Text.Length == 2)
                {
                    TextBoxData.Text = TextBoxData.Text.Insert(cursorPosition, "/");
                    cursorPosition++;
                }

                // Insere uma / depois do 5º digito
                if (TextBoxData.Text.Length == 5)
                {
                    TextBoxData.Text = TextBoxData.Text.Insert(cursorPosition, "/");
                    cursorPosition++;
                }

                // Atualiza a posição do cursor
                TextBoxData.SelectionStart = cursorPosition;
            }
        }

        private void BtnDeletar_Click(object sender, EventArgs e)
        {
            var resposta = MessageBox.Show("Deseja deletar este registro?", "EXCLUIR", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if (resposta == DialogResult.Yes)
            {

                var result = usuario.Excluir(usuario);
                if (result)
                {
                    MessageBox.Show("Registro Deletado!", "DELETAR", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Não foi possível deletar o registro, contate o administrador do sistema!", "DELETAR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    usuario.Id = -1;
                }
            }
        }
    }
}
