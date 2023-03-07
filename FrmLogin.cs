using System;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Microsoft.VisualBasic;
using System.Net.Mail;
using System.Web;
using System.ServiceModel.Security.Tokens;
using System.Linq;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace Teste_SysMap
{
    public partial class FrmLogin : Form
    {
        
        string vCode;
        bool autenticado = false;
        global::Usuario usuario = new global::Usuario();
        public FrmLogin()
        {
            InitializeComponent();

            this.usuario = FrmMenu.usuario;
        }

        private void LblCadastro_Click(object sender, EventArgs e)
        {
            ChamarCadastro();
        }

        private void ChamarCadastro()
        {
            PanelLogin.SendToBack();
            // Remove o BtnClose do PanelLogin
            PanelLogin.Controls.Remove(BtnClose);

            // Adiciona o BtnClose ao PanelCadastro
            PanelCadastro.Controls.Add(BtnClose);
            BtnClose.BringToFront();
        }

        private void LblLogin_Click(object sender, EventArgs e)
        {
            ChamarLogin();
        }

        private void ChamarLogin()
        {
            PanelCadastro.SendToBack();
            // Remove o BtnClose do PanelLogin
            PanelCadastro.Controls.Remove(BtnClose);

            // Adiciona o BtnClose ao PanelCadastro
            PanelLogin.Controls.Add(BtnClose);
            BtnClose.BringToFront();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
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
            else if (e.KeyChar != '\b')
            {
                // Ignora qualquer tecla que não seja um dígito ou backspace
                e.Handled = true;
            }
        }

        private void BtnCadastrar_Click(object sender, EventArgs e)
        {
            if (TxtCodConfirmation.Text == vCode)
            {

                usuario.Nome = TxtNome.Text;
                if (DateTime.TryParse(TextBoxData.Text, out DateTime dataNascimento)) usuario.DataNascimento = dataNascimento;
                else
                {
                    MessageBox.Show("Data de nascimento inválida!");
                    return;
                }
                usuario.Email = TxtEmail.Text;
                usuario.Senha = TxtSenha.Text;
                usuario.FotoPerfil = ImageHelper.ImageToByteArray(PicturePerfilProfile.Image);// converte a imagem em um array de bytes e atribui à propriedade FotoPerfil do objeto Usuario

                try
                {
                    HttpStatusCode statusCode = usuario.Salvar(usuario);
                    if (statusCode == HttpStatusCode.Created)
                    {
                        MessageBox.Show("Usuário cadastrado com sucesso!");
                        LimparDados();
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
            else
            {

            }
        }

        private void LimparDados()
        {
            TxtNome.Text = "";
            TextBoxData.Text = "";
            TxtEmail.Text = "";
            TxtSenha.Text = "";
            LblCod.Visible = false;
            TxtCodConfirmation.Visible = false;
            ChamarLogin();
        }

        private void SendMail()
        {

            // Configuração do e-mail
            string emailFrom = "vittor.testesysmap@gmail.com";//email criado para envior de codo no email
            string emailPassword = "enutlgdbegybwmvz";
            string emailHost = "smtp.gmail.com";
            int emailPort = 587;
            bool emailEnableSsl = true;

            // Gerar código de verificação
            Random random = new Random();
            int verificationCode = random.Next(100000, 999999);
            vCode = "S - " + verificationCode.ToString();

            // Configuração do e-mail
            string emailTo = TxtEmail.Text;
            string emailSubject = "Seu Código de Verificação";
            string emailBody = $"Olá,\n\nEste e-mail contém seu código de verificação para acessar o aplicativo. Por favor, use o código abaixo para fazer login no aplicativo:\n\n{vCode}\n\nSe você não solicitou este código, por favor ignore este e-mail.\n\nAtenciosamente,\nSua equipe de suporte";


            // Enviar e-mail
            SmtpClient smtpClient = new SmtpClient(emailHost, emailPort);
            smtpClient.EnableSsl = emailEnableSsl;
            smtpClient.Credentials = new NetworkCredential(emailFrom, emailPassword);

            MailMessage mailMessage = new MailMessage(emailFrom, emailTo, emailSubject, emailBody);

            try
            {
                smtpClient.Send(mailMessage);
                MessageBox.Show($"Foi enviado um código para o seu e-mail ({emailTo}). Verifique seu e-mail para obter o código.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocorreu um erro ao enviar o e-mail: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnCod_Click(object sender, EventArgs e)
        {
            //API Para verificação de Email valido
            using (var client = new HttpClient())
            {
                var email = TxtEmail.Text;
                var timeout = "10";
                var url = $"https://api.usebouncer.com/v1/email/verify?email={email}&timeout={timeout}";

                // Substitua "<API Key>" pela sua chave de API
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("x-api-key", "IlZtRgkLgUBg7DGf4mC1gkmOlwCvckWZJsqqMRwH");

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var json = JObject.Parse(responseBody);

                    var status = (string)json["reason"];
                    if (status == "invalid_email")
                    {
                        MessageBox.Show("Endereço de e-mail inválido!");
                    }
                    else if (status == "accepted_email")
                    {
                        //Envio de Cod para confirmação de cadastro
                        SendMail();
                        BtnCod.Visible = false;
                        BtnCadastrar.Visible = true;
                        LblCod.Visible = true;
                        TxtCodConfirmation.Visible = true;
                        TxtCodConfirmation.Text = "S - ";
                    }
                }
                else
                {
                    MessageBox.Show("Não foi possível verificar o endereço de e-mail.");
                }
            }
        }


        private void BtnLogin_Click(object sender, EventArgs e)
        {
            autenticado = ValidarUsuario();

            if (autenticado)
            {
                this.Close();
            }
        }
        private bool ValidarUsuario()
        {
            usuario.Email = TxtLogEmail.Text;

            var resposta = usuario.LoginUsuario(usuario, TxtLogSenha.Text);
            if (resposta == "ok")
                return true;
            else
            {
                MessageBox.Show(resposta);
                return false;
            }
        }
    }
}
