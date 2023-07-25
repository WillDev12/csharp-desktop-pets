using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace csharp_desktop_pet
{
    public partial class Form1 : Form
    {
        Random rnd = new Random();
        string path = Directory.GetCurrentDirectory().ToString();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StreamReader r = new StreamReader(@"data\pet.json");
            string jsonString = r.ReadToEnd();
            pet newPet = JsonConvert.DeserializeObject<pet>(jsonString);

            this.Size = new Size(newPet.Width, newPet.Height);

            idle("idle");
        }

        private void playnewSound()
        {
            string[] idleimages = Directory.GetFiles(path + @"\sounds");
            int count = idleimages.Count() - 1;
            if (count >= 0 )
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(idleimages[0]);
                player.Play();
            }
        }

        private void idle(string directory)
        {
            string[] idleimages = Directory.GetFiles(path + @"\images\" + directory);
            int idlecount = 0;
            foreach (string f in idleimages) {
                idlecount += 1;
            }
            idlecount = idlecount - 1;
            int selected = 0;

            if (idlecount < 0 )
            {
                if (directory == "hover" || directory == "click")
                {
                    idle("idle");
                }
                else
                {
                    MessageBox.Show("Please select at lease ONE idle image.", "Desktop Pet");
                    this.Close();
                }
            } 
            else
            {
                selected = rnd.Next(0, idlecount);
                pictureBox1.Image = Image.FromFile(idleimages[selected]);
            }
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void pictureBox1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Task responseTask = Task.Run(() => {
                playnewSound();
                idle("click");
            });

            responseTask.ContinueWith(t => Console.WriteLine("In ContinueWith"));
            responseTask.Wait();

            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            } 
            else
            {
                DialogResult dialogResult = MessageBox.Show("Would you like to close this pet?", "Desktop Pets", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    this.Close();
                }
            }
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            idle("hover");
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            idle("idle");
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            idle("hover");
        }
    }

    class pet
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
