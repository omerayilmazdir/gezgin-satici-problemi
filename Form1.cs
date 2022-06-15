using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Collections;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace WindowsFormsApplication35
{
    public partial class Form1 : Form
    {
        public static int Mesafe;
        List<PictureBox> items = new List<PictureBox>();


        public static Point[] points = new Point[10];


        public static Graphics gObject;


        public static int endusukmaliyet = int.MaxValue;
        public static string endusukmaliyetstring = null;
        public string enkisaguzergah = null;
        public int RastgeleSayi = 0;
        public int[] distance;
        public bool[] shortestPathTreeSet;
        public string sondurak;
        Bitmap CurrentParticle;
        DirectedWeightedGraph g = new DirectedWeightedGraph();
        public Form1()
        {
            InitializeComponent();
            pictureBox2.AllowDrop = true;
            PictureBox_Particle3.AllowDrop = true;
        }
        public static MySqlConnectionStringBuilder build = new MySqlConnectionStringBuilder();
        public static MySqlConnection baglanti;
        int SehirSayisi = 0;
        Random Rastgele = new Random();
        int ToplamMesafe = 0;
        

        private void Form1_Load(object sender, EventArgs e)
        {
            build.Server = "localhost";
            build.UserID = "root";
            build.Database = "kuryedatabase";
            build.Password = "safranbolu78";
            baglanti = new MySqlConnection(build.ToString());
            baglanti.Open();
            //Sorgu (Emir Listesi)
            MySqlCommand command = new MySqlCommand("TRUNCATE TABLE konumlar", baglanti);
            //Okuyucu nesnesi (Kamyon)
            MySqlDataReader Okuyucu = command.ExecuteReader();
            //Bilgiler Diziye yükleniyor
            listBox1.Items.Clear();
            listBox2.Items.Clear();

            baglanti.Close();
            baglanti.Open();
            //Sorgu (Emir Listesi)
            MySqlCommand commandd = new MySqlCommand("TRUNCATE TABLE mesafeler", baglanti);
            //Okuyucu nesnesi (Kamyon)
            commandd.ExecuteReader();
            baglanti.Close();
            


        }

        private void MakePicturebox(MouseEventArgs e)
        {

            int X, Y;
            X = e.X;
            Y = e.Y;
            PictureBox point = new PictureBox();
            point.Height = 15;
            point.Width = 15;
            if (items.Count() > 0)
            {
                System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
                gp.AddEllipse(0, 0, point.Width - 3, point.Height - 3);
                Region rg = new Region(gp);
                point.Region = rg;

                for (int i = 0; i < points.Length; i++)
                {
                    if (items.Count() == i)
                    {
                        points[i] = new Point(X, Y);
                      
                    }
                }

            }

            if (items.Count() == 0)
            {
                points[0] = new Point(X, Y);
                

            }
            point.BackColor = Color.Black;
            point.Location = new Point(X, Y);
            items.Add(point);
            
            this.Controls.Add(point);
            point.BringToFront();

        }
        private void MakeText(PaintEventArgs e)
        {
            Font drawFont = new Font("Arial", 32);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            float x = 150.0F;
            float y = 50.0F;
            // Set format of string.
            StringFormat drawFormat = new StringFormat();
            drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;

            e.Graphics.DrawString(textBox3.Text, drawFont, drawBrush, x, y, drawFormat);



        }

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {

            if (checkBox1.Checked == true && textBox3.Text != "")
            {

                int X, Y;
                string SehirAdi;
      
                X = e.X;
                Y = e.Y;
                SehirAdi = textBox3.Text;

                BilgileriKaydet(SehirAdi, X, Y);

            }

        }
        //VT bilgileri kaydediyoruz.
        public void BilgileriKaydet(string SehirAdi, int X, int Y)
        {
            // Bağlantı adresini tanımlama (Köprü kuruluyor)
            baglanti.Open();
            MySqlCommand command = new MySqlCommand("INSERT INTO Konumlar(SehirAdi, X, Y) VALUES ('" +
           SehirAdi + "'," + X + "," + Y + ")", baglanti);
            MySqlDataReader reader = command.ExecuteReader();

            baglanti.Close();
            BilgileriOku();
        }

        public void BilgileriOku()
        {
            // Bağlantı adresini tanımlama (Köprü kuruluyor)
            baglanti.Open();
            //Sorgu (Emir Listesi)

            MySqlCommand command = new MySqlCommand("SELECT * FROM Konumlar", baglanti);

            //Okuyucu nesnesi (Kamyon)
            MySqlDataReader Okuyucu = command.ExecuteReader();
            string noktaadi="armut";
            //Bilgiler Sayfaya yükleniyor
            listBox1.Items.Clear();
            while (Okuyucu.Read())
            {
                listBox1.Items.Add(Okuyucu["ID"].ToString() + "," +
               Okuyucu["SehirAdi"].ToString() + "," + Okuyucu["X"].ToString() + "," +
               Okuyucu["Y"].ToString());
                 noktaadi = Okuyucu["SehirAdi"].ToString();


            }
            g.InsertVertex(noktaadi);
            checkBox1.Checked = false;
            baglanti.Close();
            Okuyucu.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            //Veritabanından Diziye okutuyoruz.
            string[,] Sehirler = new string[100, 4];
            // Bağlantı adresini tanımlama (Köprü kuruluyor)
            baglanti.Open();
            //Sorgu (Emir Listesi)
            MySqlCommand command = new MySqlCommand("SELECT * FROM Konumlar", baglanti);
            //Okuyucu nesnesi (Kamyon)
            MySqlDataReader Okuyucu = command.ExecuteReader();
            //Bilgiler Diziye yükleniyor
            int i = 0;
            while (Okuyucu.Read())
            {

                Sehirler[i, 0] = Okuyucu["ID"].ToString();
                Sehirler[i, 1] = Okuyucu["SehirAdi"].ToString();
                Sehirler[i, 2] = Okuyucu["X"].ToString();
                Sehirler[i, 3] = Okuyucu["Y"].ToString();
                i = i + 1;
            }
            SehirSayisi = i;
            //Mesafe Hesaplama
            for (int j = 0; j < SehirSayisi; j++)
            {
                int ID1 = Convert.ToInt16(Sehirler[j, 0]);
                string Sehir1 = Sehirler[j, 1];
                int X1 = Convert.ToInt16(Sehirler[j, 2]);
                int Y1 = Convert.ToInt16(Sehirler[j, 3]);
                for (int k = 0; k < SehirSayisi; k++)
                {
                    int ID2 = Convert.ToInt16(Sehirler[k, 0]);
                    string Sehir2 = Sehirler[k, 1];
                    int X2 = Convert.ToInt16(Sehirler[k, 2]);
                    int Y2 = Convert.ToInt16(Sehirler[k, 3]);
                    Mesafe = Convert.ToInt16(Math.Sqrt((Y2 - Y1) * (Y2 - Y1) +
                   (X2 - X1) * (X2 - X1)));
                    if (Mesafe < 100)
                    {
                        // MessageBox.Show(Mesafe.ToString());
                        Mesafe = Mesafe * 2;
                    }
                    else if(Mesafe <200)
                    {
                        //  MessageBox.Show(Mesafe.ToString());
                        Mesafe = Mesafe * 4;
                    }
                    else if(Mesafe<300)
                    {
                        {
                            //  MessageBox.Show(Mesafe.ToString());
                            Mesafe = Mesafe * 6;
                        }
                    }
                    if (Mesafe != 0)
                    {
                        listBox2.Items.Add(Sehir1 + "," + Sehir2 + "=" +
                       Mesafe.ToString());
                        baglanti.Close();
                        baglanti.Open();
                        MySqlCommand commandD = new MySqlCommand("INSERT INTO mesafeler(IDsehir1, IDsehir2, Mesafe, status) VALUES(" + ID1 + ", " + ID2 + ", " + Mesafe + "," + 0 + ")", baglanti);
                        commandD.ExecuteNonQuery();
                        g.InsertEdge(Sehir1, Sehir2, Mesafe);


                    }
                }
            }
            baglanti.Close();
            Okuyucu.Close();

        }

        private void button2_Click(object sender, EventArgs e)
        {

            // Bağlantı adresini tanımlama (Köprü kuruluyor)
            baglanti.Open();
            //Sorgu (Emir Listesi)
            MySqlCommand command = new MySqlCommand("SELECT * FROM mesafeler", baglanti);
            //Okuyucu nesnesi (Kamyon)
            MySqlDataReader Okuyucu = command.ExecuteReader();
            //Bilgiler Sayfaya yükleniyor
            listBox2.Items.Clear();
            while (Okuyucu.Read())
            {
                listBox2.Items.Add(Okuyucu["IDsehir1"].ToString() + "," +
               Okuyucu["IDsehir2"].ToString() + "=" + Okuyucu["Mesafe"].ToString());
            }
            baglanti.Close();
            Okuyucu.Close();

        }
        //KROMOZOMLARI OLUŞTUR.

        private void button3_Click(object sender, EventArgs e)
        {
            //int BaslangicGenSayisi = 100;
            //string A ,B;
            string YeniKromozom = null;


            string EskiKromozom = null;
            Boolean varmi = false;
            switch (items.Count())
            {
                case 3:
                    EskiKromozom = "1,2,3";
                    break;

                case 4:
                    EskiKromozom = "1,2,3,4";
                    break;

                case 5:
                    EskiKromozom = "1,2,3,4,5";
                    break;

                case 6:
                    EskiKromozom = "1,2,3,4,5,6";
                    break;

                case 7:
                    EskiKromozom = "1,2,3,4,5,6,7";
                    break;

                case 8:
                    EskiKromozom = "1,2,3,4,5,6,7,8";
                    break;

                default:
                    EskiKromozom = "1,2";
                    break;


            }
            int permutasyon = faktoriyel(items.Count() - 1);
            //Kaç tane ilk kromozom oluşturacaksa o kadar dönecek
            for (int i = 0; i < permutasyon; i++)
            {
                YeniKromozom = YeniKromozomOlustur(EskiKromozom);
                if (ToplamMesafe != 0)
                {
                    for (int k = 0; k < listBox3.Items.Count; k++)
                    {
                        if (listBox3.Items[k].ToString().Contains(YeniKromozom + "=" + ToplamMesafe))
                        {
                            i--;
                            varmi = true;
                            break;
                        }
                    }
                    if (varmi == false)
                    {
                        if (ToplamMesafe < endusukmaliyet)
                        {
                            endusukmaliyet = ToplamMesafe;
                            endusukmaliyetstring = YeniKromozom + " Güzergahının Maliyeti =" + endusukmaliyet.ToString();
                            enkisaguzergah = YeniKromozom;
                            string[] sondurakarray = YeniKromozom.Split(',');
                            sondurak = sondurakarray[(sondurakarray.Length-1)];
                        }
                        listBox3.Items.Add(YeniKromozom + "=" + ToplamMesafe);
                    }

                }
                else
                {
                    i--;
                }
                EskiKromozom = YeniKromozom;
                varmi = false;
            }
            MessageBox.Show(endusukmaliyetstring, "En Düşük Maliyetli Güzergah");
            textBox1.Text = endusukmaliyetstring;
            kisaguzergah(enkisaguzergah);
            
            button3.Enabled = false;
         //   ShowAnimation();
        }
        private void kisaguzergah(string enkisa)
        {

            string[] kisagüzergahdizisi = enkisa.Split(',');
            ArrayList yenikisaguzergahdizisi = new ArrayList();
            int kok = 0;
            int hedef = 0;
            foreach (string Eleman in kisagüzergahdizisi)
            {
                if (Eleman != null)
                {
                    yenikisaguzergahdizisi.Add(Eleman);
                }
            }
            for (int i = (yenikisaguzergahdizisi.Count - 1); i >= 1; i--)
            {
                Brush blue = new SolidBrush(Color.Blue);
                Pen bluePen = new Pen(blue, 2);
                kok = int.Parse((string)yenikisaguzergahdizisi[i]);
                hedef = int.Parse((string)yenikisaguzergahdizisi[i-1]);
                gObject.DrawLine(bluePen, points[(kok-1)], points[(hedef-1)]);

            }
        }
        public int faktoriyel(int k)
        {
            int fakto = 1;
            for (int i = k; i >= 2; i--)
            {
                fakto = i * fakto;
            }
            return fakto;
        }

        public string YeniKromozomOlustur(string EskiKromozom)
        {
            string YeniDizi = null;
            int Mesafe = 0;
            string[] Dizi1 = EskiKromozom.Split(',');
            ArrayList Dizi2 = new ArrayList();
            foreach (string Eleman in Dizi1)
            {
                if (Eleman != null)
                {
                    Dizi2.Add(Eleman);
                }
            }
            int IDsehir1 = -1, IDsehir2 = -1; //sıfırlamalar
            ToplamMesafe = 0;
            do
            {
                try
                {
                    if (Dizi2.Count == items.Count())
                    {
                        RastgeleSayi = 0;
                    }
                    else
                    {
                        RastgeleSayi = Rastgele.Next(0, Dizi2.Count);
                    }

                    if (Dizi2.Count == items.Count())
                    {
                        if (RastgeleSayi == 0)
                        {
                            YeniDizi = YeniDizi + Dizi2[RastgeleSayi] + ",";
                            IDsehir1 = Convert.ToInt32(Dizi2[RastgeleSayi]);
                            if (IDsehir2 != -1) //Ilk değer okunmazken.
                            {
                                Mesafe = VT_IkiSehirMesafesiniOku(IDsehir1, IDsehir2);
                                //   MessageBox.Show(Mesafe.ToString());
                                ToplamMesafe = ToplamMesafe + Mesafe;
                            }
                            IDsehir2 = IDsehir1;
                            Dizi2.RemoveAt(RastgeleSayi);
                        }
                        else
                        {
                            YeniDizi = EskiKromozom;
                            break;
                        }
                    }
                    else
                    {
                        if (Dizi2.Count == 1)
                        { YeniDizi = YeniDizi + Dizi2[RastgeleSayi]; }
                        else
                        {
                            YeniDizi = YeniDizi + Dizi2[RastgeleSayi] + ",";
                        }
                        IDsehir1 = Convert.ToInt32(Dizi2[RastgeleSayi]);
                        if (IDsehir2 != -1) //Ilk değer okunmazken.
                        {
                            Mesafe = VT_IkiSehirMesafesiniOku(IDsehir1, IDsehir2);
                            //   MessageBox.Show(Mesafe.ToString());
                            ToplamMesafe = ToplamMesafe + Mesafe;
                        }
                        IDsehir2 = IDsehir1;
                        Dizi2.RemoveAt(RastgeleSayi);
                    }

                }
                catch { }
            } while (Dizi2.Count > 0);
            return YeniDizi;
        }

        //VT İKİ ŞEHİR ARASINDAKİ MESAFEYİ OKUYOR
        public int VT_IkiSehirMesafesiniOku(int IDsehir1, int IDsehir2)
        {
            Mesafe = 0;
            // Bağlantı adresini tanımlama (Köprü kuruluyor)
            baglanti.Open();
            //Sorgu (Emir Listesi)

            MySqlCommand command = new MySqlCommand("SELECT Mesafe FROM mesafeler WHERE IDsehir1=" + IDsehir1 + " AND IDsehir2=" + IDsehir2 + "", baglanti);

            //Okuyucu nesnesi (Kamyon)
            MySqlDataReader Okuyucu = command.ExecuteReader();
            //Bilgiler Sayfaya yükleniyor
            while (Okuyucu.Read())
            {
                Mesafe = Convert.ToInt32(Okuyucu["Mesafe"]);
            }
            baglanti.Close();
            Okuyucu.Close();
            //     MessageBox.Show("İki Nokta arasındaki Mesafe"+Mesafe.ToString());
           
            return Mesafe;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_MouseClick(object sender, MouseEventArgs e)
        {
            if (checkBox1.Checked == true && textBox3.Text != "")
            {
                MakePicturebox(e);


            }
            else
            {
                MessageBox.Show("Bilgileri doldurun ve Onay verin");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            //Sorgu (Emir Listesi)
            MySqlCommand command = new MySqlCommand("TRUNCATE TABLE konumlar", baglanti);
            //Okuyucu nesnesi (Kamyon)
            MySqlDataReader Okuyucu = command.ExecuteReader();
            //Bilgiler Diziye yükleniyor
            listBox1.Items.Clear();
            listBox2.Items.Clear();

            baglanti.Close();
            baglanti.Open();
            //Sorgu (Emir Listesi)
            MySqlCommand commandd = new MySqlCommand("TRUNCATE TABLE mesafeler", baglanti);
            //Okuyucu nesnesi (Kamyon)
            commandd.ExecuteReader();
            baglanti.Close();


        }



        private void button7_Click(object sender, EventArgs e)
        {
            
            gObject = pictureBox2.CreateGraphics();
            Brush red = new SolidBrush(Color.Red);
            Brush blue = new SolidBrush(Color.Blue);
            Pen redPen = new Pen(red, 4);
            Pen bluePen = new Pen(blue, 4);
       
            for (int i = 0; i < items.Count(); i++)
            {
                for (int j = 0; j < items.Count(); j++)
                {
                    int ikinoktaarası = 0;
                    ikinoktaarası = VT_IkiSehirMesafesiniOku((i + 1), (j + 1));
                    //  MessageBox.Show(i.ToString() +"  -  "+ j.ToString()+"=Mesafesi="+ikinoktaarası.ToString());
                    if (ikinoktaarası != 0)
                    {
                        gObject.DrawLine(redPen, points[i], points[j]);
                        ikinoktaarası = 0;
                    }
                    else
                    {
                        continue;
                    }

                }
            }
            button3.Enabled = true;

        }
        public int shortestmesafe = int.MaxValue;
        public void dijkstra(int hedef, int source, int noktasayi)
        {
            baglanti.Close();
            if ((noktasayi - 1) == items.Count())
            {
                MessageBox.Show("Toplam Maliyet =" + ToplamMesafe.ToString());
                return;
            }
            int targets = 0;
            int sources = 0;
            int id = 0;
           
          
            distance[source] = 0;



            baglanti.Open();
            MySqlCommand command = new MySqlCommand("SELECT ID,IDsehir2,IDsehir1,Mesafe FROM mesafeler WHERE IDsehir1=" + (source + 1) + " AND status=" + 0 + "", baglanti);

            //Okuyucu nesnesi (Kamyon)
            MySqlDataReader Okuyucu = command.ExecuteReader();
            //Bilgiler Sayfaya yükleniyor

            while (Okuyucu.Read())
            {
                Mesafe = Convert.ToInt32(Okuyucu["Mesafe"]);
                targets = Convert.ToInt32(Okuyucu["IDsehir2"]);
                sources = Convert.ToInt32(Okuyucu["IDsehir1"]);
                id = Convert.ToInt32(Okuyucu["ID"]);
                distance[targets - 1] = Mesafe;
                if (Mesafe < shortestmesafe)
                {
                    shortestmesafe = Mesafe;
                }
            }
            baglanti.Close();
            baglanti.Open();
            for (int i = 0; i < items.Count(); ++i)
            {
                  MessageBox.Show(i.ToString() + ". noktaya mesafe" + distance[i].ToString());
            }
            for (int i = 0; i < items.Count(); ++i)
            {
                if (distance[i] == shortestmesafe)
                {
                    MySqlCommand commandD = new MySqlCommand("UPDATE mesafeler set status='" + 1 + "'where mesafe='" + shortestmesafe + "'", baglanti);
                    commandD.ExecuteNonQuery();
                    gObject = pictureBox2.CreateGraphics();
                    Brush blue = new SolidBrush(Color.Blue);
                    Pen bluePen = new Pen(blue, 4);
                    gObject.DrawLine(bluePen, points[source], points[i]);
                    source = i;
                    ToplamMesafe = ToplamMesafe + shortestmesafe;
                    noktasayi++;
                    dijkstra(0, source, noktasayi);


                }
            }
            baglanti.Close();

        }
        private void button6_Click(object sender, EventArgs e)
        {

            //  dijkstra(0, (Convert.ToInt32(sondurak)-1), 1);
            g.getObject = gObject;
            g.getpoin = points;

            g.FindPaths(sondurak);
            
        }

        private void button8_Click(object sender, EventArgs e)
        {
            VT_IkiSehirMesafesiniOku(1, 2);
        }

        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
          
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }
        private void ShowAnimation()
        {

            Bitmap original = new Bitmap(pictureBox2.Image);
            Bitmap background = new Bitmap(pictureBox2.Image);
            Bitmap temporal = new Bitmap(pictureBox2.Image);
            Bitmap animationParticle = new Bitmap(CurrentParticle, new Size((int)(CurrentParticle.Width * 0.1), (int)(CurrentParticle.Height * 0.1)));

            int xCorrection = animationParticle.Width / 2, yCorrection = animationParticle.Height / 2;


            for (int i = 0; i < (items.Count - 1); ++i)
            {


                //   Graphics.FromImage(background).DrawLine(new Pen(Color.FromArgb(54, 54, 54), 8), points[i], points[i + 1]);
                gObject.DrawLine(new Pen(Color.FromArgb(54, 54, 54), 4), points[i], points[i + 1]);
                for (int j = 0; j < points.Length; ++j)
                {
                    gObject.DrawImage(animationParticle, new Point(points[j].X - xCorrection, points[j].Y - yCorrection));


                }
            }

        }
     
        private void pictureBox2_DragDrop(object sender, DragEventArgs e)
        {
            this.CurrentParticle = (Bitmap)e.Data.GetData(DataFormats.Bitmap, true);

            MessageBox.Show("Resim yerleştirildi");
        }
      
        private void PictureBox_Particle3_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                PictureBox_Particle3.DoDragDrop(PictureBox_Particle3.Image, DragDropEffects.Copy);
        }

        private void PictureBox_Particle3_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, PictureBox_Particle3.ClientRectangle, Color.FromArgb(54, 54, 54), ButtonBorderStyle.Dotted);

        }

        private void pictureBox2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Bitmap) && (e.AllowedEffect & DragDropEffects.Copy) != 0)
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            int [,] matris=g.matrix();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    richTextBox1.AppendText(matris[i, j].ToString()+"\t");
                }
                richTextBox1.AppendText("\n");
            }
        }
        public void cizgicek(int i, int j)
        {
            Brush black = new SolidBrush(Color.Black);
            Pen redPen = new Pen(black, 4);
            gObject.DrawLine(redPen, points[i], points[j]);

        }
    }
}
