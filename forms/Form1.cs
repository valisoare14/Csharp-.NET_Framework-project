using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Clienti_Banca;

namespace ProiectPAW
{
    public partial class dgv : Form
    {
        Banca banca;
        public dgv()
        {
            InitializeComponent();
            banca = new Banca();
            banca.Eveniment_Actualizare += banca_eveniment_actualizare;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            modificareToolStripMenuItem.Enabled = stergereToolStripMenuItem.Enabled = extragereToolStripMenuItem.Enabled = false;
        }

        private void banca_eveniment_actualizare(object sender, EventArgs e)
        {
            Banca banca = (Banca)sender;
            dataGridView1.Rows.Clear();//curatam toate liniile existente in data grid view
            if (banca.NumarClienti != 0)
            {
                List<int> solduri = new List<int>();//cream o lista de solduri pt a gestiona fereastra de vizualizare
                modificareToolStripMenuItem.Enabled = stergereToolStripMenuItem.Enabled = extragereToolStripMenuItem.Enabled = true;
                for (int i = 0; i < banca.NumarClienti; i++)
                {
                    dataGridView1.Rows.Add(banca[i].Nume, banca[i].Sold);//adaugam fiecare client in grid view
                    solduri.Add(banca[i].Sold);//adaugam soldul fiecarui clientin lista de solduri pt a prelucra valorile pentru fereastra de vizulaizare
                }
                vizualizare.Observatii = solduri;
                dataGridView1.Rows[((Banca_eventArgs)e).Index].Selected = true;//ultima linie corespunzatoare ultimului client din banca ramane selectata
                toolStripStatusLabel1.Text = banca.Capitaluri_Banca.ToString();
            }
            else
            {
                modificareToolStripMenuItem.Enabled = stergereToolStripMenuItem.Enabled = false;
                toolStripStatusLabel1.Text = "0";
            }
        }
        private void adaugareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4();
            form4.Text = form4.button1.Text = ((ToolStripMenuItem)sender).Text;
            form4.Text += " Sold";
            if (((ToolStripMenuItem)sender).Tag as string == "A")
            {
                form4.textBox1.Text = string.Empty;
                form4.textBox2.Text = "0";
            }
            if (((ToolStripMenuItem)sender).Tag as string == "M")
            {
                form4.textBox1.Text = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                form4.textBox2.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
            }
            if (DialogResult.OK == form4.ShowDialog())
            {
                if (((ToolStripMenuItem)sender).Tag as string == "A")
                {
                    Client client = new Client()
                    {
                        Nume = form4.textBox1.Text,
                        Sold = int.Parse(form4.textBox2.Text)
                    };
                    banca.Adauga(client);
                }
                else
                {
                    Client client = new Client()
                    {
                        Nume = form4.textBox1.Text.ToString(),
                        Sold = int.Parse(form4.textBox2.Text)
                    };
                    banca[dataGridView1.SelectedRows[0].Index] = client;
                }
            }
        }
        private void modificareToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void stergereToolStripMenuItem_Click(object sender, EventArgs e)
        {
            banca.StergeClient(dataGridView1.SelectedRows[0].Index);
        }

        private void extragereToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.Text = form3.button1.Text = ((ToolStripMenuItem)sender).Text;
            form3.Text += " Sold";
            form3.textBox1.Text = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            form3.textBox3.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
            if (DialogResult.OK == form3.ShowDialog())
            {
                if (((ToolStripMenuItem)sender).Tag as string == "E")
                    banca.RetragereSold(int.Parse(form3.textBox2.Text), dataGridView1.SelectedRows[0].Index);
                else
                    banca.DepunereSold(int.Parse(form3.textBox2.Text), dataGridView1.SelectedRows[0].Index);
            }
        }

        private void salvareDateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = @"C:\Users\User\source\repos\ProiectPAW";
            saveFileDialog.Filter = "Fisiere binare|*.dat|Toate fisierele|*.*";
            saveFileDialog.FilterIndex = 1;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                banca.Serializare(saveFileDialog.FileName);
            }
        }

        private void deschideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = @"C:\Users\User\source\repos\ProiectPAW";
            openFileDialog.Filter = "Fisiere de date|*.dat|Toate fisierele|*.*";
            openFileDialog.FilterIndex = 0;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                banca.DeSerizalizare(openFileDialog.FileName);
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                vizualizare.TipGrafic = 0;
            }
            else
            {
                vizualizare.TipGrafic = 1;
            }
        }
    }

    class VizualizareGrafica : Control
    {
        List<int> observatii = new List<int>();
        int tipGrafic;

        public VizualizareGrafica()
        {
            observatii = null; tipGrafic = 0; this.ResizeRedraw = true;
        }
        public List<int> Observatii
        {
            get { return observatii; }
            set
            {
                if (observatii != value)
                    observatii = value; Invalidate();
            }
        }
        public int TipGrafic
        {
            get { return tipGrafic; }
            set { if (tipGrafic != value) tipGrafic = value; Invalidate(); }
        }
        protected override void OnPaint(PaintEventArgs e)//ridica evenimentul de desenare 
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            Rectangle r = this.ClientRectangle;
            Rectangle chenar = new Rectangle()
            {
                X = r.X + 10,
                Y = r.Y + 10,
                Width = r.Width - 20,
                Height = r.Height - 20
            };
            if (tipGrafic == 0)
            {
                if (observatii != null && observatii.Count >= 2)
                {
                    float start = 0f;
                    Random random = new Random();
                    foreach (int observatie in observatii)
                    {
                        Color culoareFelie = Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));//definim o culoare pt felie
                        float unghi = ((float)observatie / observatii.Sum()) * 360;
                        g.FillPie(new SolidBrush(culoareFelie), chenar, start, unghi);
                        start += unghi;
                    }
                }
            }
            else if (tipGrafic == 1)
            {
                g.FillRectangles(Brushes.Green, Generare_bare(chenar));
            }
        }
        Rectangle[] Generare_bare(Rectangle chenar)
        {
            float latimeBara = (float)chenar.Width / (4F / 3F * observatii.Count + 1F / 3F); //latime bara 
            float distantaBara = latimeBara / 3;  //distanta dintre bare
            Rectangle[] vectorDreptunghiuriObservatii = new Rectangle[observatii.Count];
            for (int i = 0; i < vectorDreptunghiuriObservatii.Length; i++)
            {
                vectorDreptunghiuriObservatii[i].X = chenar.X + (Convert.ToInt32(distantaBara) + i * (Convert.ToInt32(latimeBara) + Convert.ToInt32(distantaBara)));
                vectorDreptunghiuriObservatii[i].Height = chenar.Height * observatii[i] / observatii.Max();
                vectorDreptunghiuriObservatii[i].Width = Convert.ToInt32(latimeBara);
                vectorDreptunghiuriObservatii[i].Y = chenar.Y + chenar.Height - vectorDreptunghiuriObservatii[i].Height;
            }
            return vectorDreptunghiuriObservatii;
        }
    }
}
