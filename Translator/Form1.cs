using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace Translator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            comboBox1.Text = "Avtomatik aniqlash";
            comboBox2.Text = "O'zbekcha";
        }

        private const int shadow = 0x00020000;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle = shadow;
                return cp;
            }
        }

        public async Task<string> TranslateText(string input, string kirish, string chiqish)
        {
            try
            {
                switch(kirish)
                {
                    case "O'zbekcha": kirish = "uz"; break;
                    case "Avtomatik aniqlash": kirish = "auto"; break;
                    case "Inglizcha": kirish = "en"; break;
                    case "Ruscha": kirish = "ru"; break;
                    default: break;
                }
                switch (chiqish)
                {
                    case "O'zbekcha": chiqish = "uz"; break;
                    case "Inglizcha": chiqish = "en"; break;
                    case "Ruscha": chiqish = "ru"; break;
                    default: break;
                }
                string url = String.Format
                ("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}",
                 $"{kirish}", $"{chiqish}", Uri.EscapeUriString(input));
                HttpClient httpClient = new HttpClient();
                string result = httpClient.GetStringAsync(url).Result;
                var jsonData = new JavaScriptSerializer().Deserialize<List<dynamic>>(result);
                var translationItems = jsonData[0];
                string translation = "";
                foreach (object item in translationItems)
                {
                    IEnumerable translationLineObject = item as IEnumerable;
                    IEnumerator translationLineString = translationLineObject.GetEnumerator();
                    translationLineString.MoveNext();
                    translation += string.Format(" {0}", Convert.ToString(translationLineString.Current));
                }
                if (translation.Length > 1) { translation = translation.Substring(1); };
                return translation;
            }
            catch (Exception)
            {
                if (textBox1.Text == "") MessageBox.Show("Matn kiritilmagan!");
                else if (textBox1.Text.Contains("#")) MessageBox.Show("Noma'lum simbvollar mavjud!");
                else MessageBox.Show("Internet aloqasi mavjud emas!");
            }
            return null;
        }
        private async void button1_Click_1Async(object sender, EventArgs e)
        {
            if (textBox1.Text != null && textBox1.Text.Length > 0)
            {
                try
                {
                    var input_language = comboBox1.Text;
                    var output_language = comboBox2.Text;
                    textBox2.Text = await TranslateText(textBox1.Text, input_language, output_language);
                }
                catch (Exception)
                {
                    MessageBox.Show("Internet aloqasi yo'q!");
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(comboBox1.Text != "Avtomatik aniqlash")
            {
                string t = comboBox1.Text;
                comboBox1.Text = comboBox2.Text;
                comboBox2.Text = t;
            }
        }
        Point last;
        private void label4_MouseDown(object sender, MouseEventArgs e)
        {
            last = new Point(e.X, e.Y);
        }

        private void label4_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - last.X;
                this.Top += e.Y - last.Y;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            textBox1.Text = Clipboard.GetText();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox2.Text);
        }
    }
}
