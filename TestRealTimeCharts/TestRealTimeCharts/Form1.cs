using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;

namespace TestRealTimeCharts
{
    public partial class Form1 : Form
    {
        int Identificator = 0; //количество графиков
        private Thread BrokenThread;
        const int ConstNum = 64; //размер массива
        public string path;
        const int delay = 1000;
        const int timerMax = 45; //Количество секунд записи в файл

        private double[] FirstArray = new double[ConstNum];  //Массивы для данных на 3 датчика
        private double[] SecondArray = new double[ConstNum]; //Заданы размером n 
        private double[] ThirdArray = new double[ConstNum];
        private double[] FourthArray = new double[ConstNum];
        

        public string SelectedState = "COM1"; // утановим по умолчанию параметры
        public int BRate = 9600;
        public Form1() 
        {
            InitializeComponent();
            // все доступные порты закинем в comboBox1
            string[] ports = SerialPort.GetPortNames();
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(ports);

           
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
        }


        //Открываем готовый файл
        private void OpenF_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // label
            }
        }

        int tim;
        //Выбрать куда сохранять данные
        private void SaveF_Click(object sender, EventArgs e)
        {

            saveFileDialog1.FileName = "Data1";
            saveFileDialog1.Filter = "TXT files (*.txt)|*.txt*|All files (*.*) | *.*";
            saveFileDialog1.ShowDialog();
            saveFileDialog1.AddExtension = true;
            path = saveFileDialog1.FileName;

            SaveF.Visible = false;

            tim = timerMax; //Заводим таймер на 45 секунд
            timer1.Interval = 1000;
            timer1.Enabled = true;
        }

        int n = 0;

        private void timer1_Tick(object sender, EventArgs e)
        {
            secondsLeft.Visible = true;
            secondsLeft.Text=(tim--).ToString();
            if (tim < 0)
            {
                secondsLeft.Visible = false;
                timer1.Stop();
                n = 0;  //Обнуляем порядковый номер

                SaveF.Visible = true;
            }
        }

        void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           SelectedState = comboBox1.SelectedItem.ToString(); //Выбор пользователем com-порта
        }


        void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            BRate = Convert.ToInt32(comboBox2.SelectedItem.ToString()); //Выбор пользователем boud rate
        }


        private void Start_Click(object sender, EventArgs e)
        {
            BrokenThread = new Thread(new ThreadStart(this.GetPerformanceCounters));
            BrokenThread.IsBackground = true;
            BrokenThread.Start();
            Identificator = comboBox3.SelectedIndex + 1;

            if (Identificator == 2)
            {
                this.Width = 719;
                this.Height = 443;
                                        //
                Chart1.Visible = true; //Включение 2 графика
                screenShot1.Visible = true;
                Series2.Visible = true;

            }

            if (Identificator == 3)
            {
                this.Width = 719;
                this.Height = 625;
                                        //
                Chart1.Visible = true; //Включение 2, 3 графиков
                screenShot1.Visible = true;
                Series2.Visible = true;

                Chart2.Visible = true;
                screenShot2.Visible = true;
                Series3.Visible = true;


            }

            if (Identificator == 4)
            {

                this.Width = 719;
                this.Height = 806;
                                        //
                Chart1.Visible = true; //Включение 2, 3, 4 графиков
                screenShot1.Visible = true;
                Series2.Visible = true;

                Chart2.Visible = true;
                screenShot2.Visible = true;
                Series3.Visible = true;

                Series4.Visible = true;
                screenShot3.Visible = true;
                Chart3.Visible = true;

            }

           
        }
                      


        private void GetPerformanceCounters()
        {
            
            SerialPort MyPort = new SerialPort();
            
            MyPort.BaudRate = BRate;
            MyPort.PortName = SelectedState;
            try
            {
                MyPort.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           // MyPort.Close();




            //читаем данные


            //
            //1 график
            //
            if (Identificator == 1)
            {

                while (true)
                {

                    string DataRx = MyPort.ReadLine(); //Чтение данных из нужного порта

                   // ждём ввода символа - начала массива
                    //MyPort.ReadTo("$");
                   // Series1 = DataRx;
                    //string DataRx;
                   // Random rnd = new Random();

                   // int k = 5;

                    for (int i = 0; i < ConstNum; i++)
                    {

                        //DataRx = rnd.Next(k - 5, k + 5).ToString();
                       // k = Convert.ToInt32(DataRx);

                        //
                        //Запись в файл
                        //
                        if (SaveF.Visible == false)
                        {
                            if (n == 0)
                            {
                                timer1.Start(); //запускаем таймер
                            }
                            using (StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default))
                            {
                                n++;
                                sw.WriteLine(n.ToString() + "|" + " " + DataRx);
                            }
                        }
                                                    
                                                                                    //        
                        FirstArray[FirstArray.Length - 1] = Convert.ToInt32(DataRx);//заполнение 1-го массива
                        Array.Copy(FirstArray, 1, FirstArray, 0, FirstArray.Length - 1);
                    }

                    if (Chart.IsHandleCreated)
                    {
                        this.Invoke((MethodInvoker)delegate { UpdateChartFirst(); }); //Вызываем "рисование"  графика из массива
                    }
                    else
                    {
                        //...
                    }

                    Thread.Sleep(delay);//Задержка между передачами данных и рисованием графиков


                }

            }


            //
            //2 Графика
            //
            if (Identificator == 2)
            {
                while (true)
                {
                    //string DataRx;
                    //Random rnd = new Random();
                    //int k = 5;
                    for (int i = 0; i < ConstNum * Identificator; i++)
                    {
                       // DataRx = rnd.Next(k - 5, k + 5).ToString();
                       // k = Convert.ToInt32(DataRx);

                         string DataRx = MyPort.ReadLine(); //Чтение данных из нужного порта


                        if (i % 2 == 0)//Запись элемента в соответственный массив его кратности
                        {
                            if (SaveF.Visible == false)
                            {
                                if (n == 0)
                                {
                                    timer1.Start(); //запускаем таймер

                                    using (StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default))
                                    {
                                        n++;
                                        sw.Write(n.ToString()+ "|" + " " + DataRx);
                                    }
                                }
                                else
                                {
                                    using (StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default))
                                    {
                                        n++;
                                        sw.Write(n.ToString() + " " + DataRx);
                                    }
                                }
                            }

                            FirstArray[FirstArray.Length - 1] = Convert.ToInt32(DataRx);//заполнение 1-го массива
                            Array.Copy(FirstArray, 1, FirstArray, 0, FirstArray.Length - 1);
                        }
                        else
                        {
                            if (SaveF.Visible == false)
                            {

                                using (StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default))
                                {
                                    sw.WriteLine(" " + DataRx);
                                }

                            }
                            SecondArray[SecondArray.Length - 1] = Convert.ToInt32(DataRx);//заполнение 2-го массива
                            Array.Copy(SecondArray, 1, SecondArray, 0, SecondArray.Length - 1);
                        }
                    }

                    if (Chart.IsHandleCreated)
                    {
                        this.Invoke((MethodInvoker)delegate { UpdateChartFirst(); });
                        this.Invoke((MethodInvoker)delegate { UpdateChartSecond(); }); //Вызываем "рисование" 2-х графиков из массивов
                    }
                    else
                    {
                        //...
                    }

                    Thread.Sleep(delay);//Задержка между передачами данных 
                }
            }

            //
            // 3 Графика
            //
            if (Identificator == 3)
            {

                while (true)
                {
                   // string DataRx;
                 //   Random rnd = new Random();
                    //int k = 5;
                    for (int i = 0; i < ConstNum * Identificator; i++)
                    {

                       // DataRx = rnd.Next(k - 5, k + 5).ToString();
                       // k = Convert.ToInt32(DataRx);
                         string DataRx = MyPort.ReadLine(); //Чтение данных из нужного порта

                        if (i % Identificator == 0) //Запись элемента в соответственный массив его кратности
                        {
                            if (SaveF.Visible == false)
                            {
                                if (n == 0)
                                {
                                    timer1.Start(); //запускаем таймер

                                    using (StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default))
                                    {
                                        n++;
                                        sw.Write(n.ToString()+"|" + " " + DataRx);
                                    }
                                }
                                else
                                {
                                    using (StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default))
                                    {
                                        n++;
                                        sw.Write(n.ToString() + " " + DataRx);
                                    }
                                }
                            }

                            FirstArray[FirstArray.Length - 1] = Convert.ToInt32(DataRx);  //заполнение 1-го массива
                            Array.Copy(FirstArray, 1, FirstArray, 0, FirstArray.Length - 1);
                        }
                        else if (i % Identificator == 1)
                        {
                            if (SaveF.Visible == false)
                            {

                                using (StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default))
                                {
                                    sw.Write(" " + DataRx);
                                }

                            }

                            SecondArray[SecondArray.Length - 1] = Convert.ToInt32(DataRx);//заполнение 2-го массива
                            Array.Copy(SecondArray, 1, SecondArray, 0, SecondArray.Length - 1);
                        }
                        else
                        {
                            if (SaveF.Visible == false)
                            {

                                using (StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default))
                                {
                                    sw.WriteLine(" " + DataRx);
                                }

                            }

                            ThirdArray[ThirdArray.Length - 1] = Convert.ToInt32(DataRx);//заполнение 3-го массива
                            Array.Copy(ThirdArray, 1, ThirdArray, 0, ThirdArray.Length - 1);
                        }

                    }
                    if (Chart.IsHandleCreated)
                    {
                        this.Invoke((MethodInvoker)delegate { UpdateChartFirst(); });    //Вызываем "рисование" 3-х графиков из массивов
                        this.Invoke((MethodInvoker)delegate { UpdateChartSecond(); });
                        this.Invoke((MethodInvoker)delegate { UpdateChartThird(); });
                    }
                    else
                    {
                        //...
                    }

                    Thread.Sleep(delay); //Задержка между передачами данных 
                }
            }


            //
            //4 графика
            //
            if (Identificator == 4)
            {

                while (true)
                {
                   // string DataRx;
                    //Random rnd = new Random();
                   // int k = 5;
                    for (int i = 0; i < ConstNum * Identificator; i++)
                    {
                        //DataRx = rnd.Next(k - 5, k + 5).ToString();
                       // k = Convert.ToInt32(DataRx);

                        string DataRx = MyPort.ReadLine(); //Чтение данных из нужного порта
                        if (i % Identificator == 0) //Запись элемента в соответственный массив его кратности
                        {
                            if (SaveF.Visible == false)
                            {
                                if (n == 0)
                                {
                                    timer1.Start(); //запускаем таймер

                                    using (StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default))
                                    {
                                        n++;
                                        sw.Write(n.ToString() +"|" + " " + DataRx);
                                    }
                                }
                                else
                                {
                                    using (StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default))
                                    {
                                        n++;
                                        sw.Write(n.ToString() + " " + DataRx);
                                    }
                                }
                            }

                            FirstArray[FirstArray.Length - 1] = Convert.ToInt32(DataRx);  //заполнение 1-го массива
                            Array.Copy(FirstArray, 1, FirstArray, 0, FirstArray.Length - 1);
                        }
                        else if (i % Identificator == 1)
                        {
                            if (SaveF.Visible == false)
                            {

                                using (StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default))
                                {
                                    sw.Write(" " + DataRx);
                                }

                            }

                            SecondArray[SecondArray.Length - 1] = Convert.ToInt32(DataRx);//заполнение 2-го массива
                            Array.Copy(SecondArray, 1, SecondArray, 0, SecondArray.Length - 1);
                        }
                        else if (i % Identificator == 2)
                        {
                            if (SaveF.Visible == false)
                            {

                                using (StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default))
                                {
                                    sw.Write(" " + DataRx);
                                }

                            }

                            ThirdArray[ThirdArray.Length - 1] = Convert.ToInt32(DataRx);//заполнение 3-го массива
                            Array.Copy(ThirdArray, 1, ThirdArray, 0, ThirdArray.Length - 1);
                        }
                        else
                        {
                            if (SaveF.Visible == false)
                            {

                                using (StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default))
                                {
                                    sw.WriteLine(" " + DataRx);
                                }

                            }

                            FourthArray[FourthArray.Length - 1] = Convert.ToInt32(DataRx);//заполнение 3-го массива
                            Array.Copy(FourthArray, 1, FourthArray, 0, FourthArray.Length - 1);
                        }

                    }
                    if (Chart.IsHandleCreated)
                    {
                        this.Invoke((MethodInvoker)delegate { UpdateChartFirst();  });    //Вызываем "рисование" 3-х графиков из массивов
                        this.Invoke((MethodInvoker)delegate { UpdateChartSecond(); });
                        this.Invoke((MethodInvoker)delegate { UpdateChartThird();  });
                        this.Invoke((MethodInvoker)delegate { UpdateChartFourth(); });

                    }
                    else
                    {
                        //...
                    }

                    Thread.Sleep(delay); //Задержка между передачами данных 
                }
            }
        }
                   
        

        private void UpdateChartFirst()   //Обработка данных 1-го массива
        {
            Chart.Series["Series1"].Points.Clear();//очищаем предыдущие записи
            Chart.Series["Series1"].LegendText = Series1.Text;

            for (int i = 0; i < FirstArray.Length - 1; i++)
            {
                Chart.Series["Series1"].Points.AddY(FirstArray[i]);//добавление новых из 1-го массива
            }
        }
        private void UpdateChartSecond()//Обработка данных 2-го массива
        {
            Chart1.Series["Series1"].Points.Clear();//очищаем предыдущие записи
            Chart1.Series["Series1"].LegendText = Series2.Text;

            for (int i = 0; i < SecondArray.Length - 1; i++)
            {
                Chart1.Series["Series1"].Points.AddY(SecondArray[i]);//добавление новых из 2-го массива
            }
        }
        private void UpdateChartThird()//Обработка данных 3-го массива
        {
            Chart2.Series["Series1"].Points.Clear(); //очищаем предыдущие записи
            Chart2.Series["Series1"].LegendText = Series3.Text;

            for (int i = 0; i < ThirdArray.Length - 1; i++)
            {
                Chart2.Series["Series1"].Points.AddY(ThirdArray[i]); //добавление новых из 3-го массива
            }
        }
        private void UpdateChartFourth()//Обработка данных 4-го массива
        {
            Chart3.Series["Series1"].Points.Clear(); //очищаем предыдущие записи
            Chart3.Series["Series1"].LegendText = Series4.Text;


            for (int i = 0; i < FourthArray.Length - 1; i++)
            {
                Chart3.Series["Series1"].Points.AddY(FourthArray[i]); //добавление новых из 4-го массива
            }
        }

        public Bitmap GetControlScreenshot(Control control) //создаем скриншот формы 
        {
            //ресайзим контрол до возможного максимума перед скриншотом
            Size szCurrent = control.Size;
            control.AutoSize = true;

            Bitmap bmp = new Bitmap(control.Width, control.Height);//создаем картинку нужных размеров
            control.DrawToBitmap(bmp, control.ClientRectangle);//копируем изображение нужного контрола в bmp
            //
            //возвращаем размер контрола назад
            //
            control.AutoSize = false;
            control.Size = szCurrent;

            return bmp;
        }

        private void screenShotA_Click(object sender, EventArgs e)
        {
            //
            // 1 график
            //
            if (Identificator == 1)
            {
                Bitmap printscreen = GetControlScreenshot(Chart);
                saveScreenShotA.FileName = "Chart1";
                saveScreenShotA.Filter = "BMP Image (*.bmp)|*.bmp| All files (*.*) | *.*";
                saveScreenShotA.ShowDialog();
                saveScreenShotA.AddExtension = true;

                string path1 = saveScreenShotA.FileName;
                printscreen.Save(path1, System.Drawing.Imaging.ImageFormat.Bmp);
            }
            //
            //2 графика
            //
            if (Identificator == 2)
            {
                Bitmap printscreen = GetControlScreenshot(Chart);
                Bitmap printscreen1 = GetControlScreenshot(Chart1);

                //
                //Объединение
                //
                Bitmap finalScreenShot = new Bitmap (printscreen.Width, printscreen.Height * Identificator);
                Graphics g = Graphics.FromImage(finalScreenShot);
                g.DrawImage(printscreen, 0, 0, printscreen.Width, printscreen.Height);
                g.DrawImage(printscreen1, 0, printscreen.Height, printscreen.Width, printscreen.Height);

                //
                //Saving
                //
                string path1;
                saveScreenShotA.FileName = "Chart12";
                saveScreenShotA.Filter = "BMP Image (*.bmp)|*.bmp| All files (*.*) | *.*";
                saveScreenShotA.ShowDialog();
                saveScreenShotA.AddExtension = true;

                path1 = saveScreenShotA.FileName;
                finalScreenShot.Save(path1, System.Drawing.Imaging.ImageFormat.Bmp);
            }
            //
            // 3 графика
            //
            if (Identificator == 3)
            {
                Bitmap printscreen = GetControlScreenshot(Chart);
                Bitmap printscreen1 = GetControlScreenshot(Chart1);
                Bitmap printscreen2 = GetControlScreenshot(Chart2);
                //
                //Объединение
                //
                Bitmap finalScreenShot = new Bitmap(printscreen.Width, printscreen.Height * Identificator);
                Graphics g = Graphics.FromImage(finalScreenShot);
                g.DrawImage(printscreen, 0, 0, printscreen.Width, printscreen.Height);
                g.DrawImage(printscreen1, 0, printscreen.Height, printscreen.Width, printscreen.Height);
                g.DrawImage(printscreen2, 0, printscreen.Height * 2, printscreen.Width, printscreen.Height);

                //
                //Saving
                //
                string path1;
                saveScreenShotA.FileName = "Chart123";
                saveScreenShotA.Filter = "BMP Image (*.bmp)|*.bmp| All files (*.*) | *.*";
                saveScreenShotA.ShowDialog();
                saveScreenShotA.AddExtension = true;

                path1 = saveScreenShotA.FileName;
                finalScreenShot.Save(path1, System.Drawing.Imaging.ImageFormat.Bmp);
            }
            //
            //4 графика
            //
            if (Identificator == 4)
            {
                Bitmap printscreen = GetControlScreenshot(Chart);
                Bitmap printscreen1 = GetControlScreenshot(Chart1);
                Bitmap printscreen2 = GetControlScreenshot(Chart2);
                Bitmap printscreen3 = GetControlScreenshot(Chart3);
                //
                //Объединение
                //
                Bitmap finalScreenShot = new Bitmap(printscreen.Width, printscreen.Height * Identificator);
                Graphics g = Graphics.FromImage(finalScreenShot);
                g.DrawImage(printscreen, 0, 0, printscreen.Width, printscreen.Height);
                g.DrawImage(printscreen1, 0, printscreen.Height, printscreen.Width, printscreen.Height);
                g.DrawImage(printscreen2, 0, printscreen.Height * 2, printscreen.Width, printscreen.Height);
                g.DrawImage(printscreen3, 0, printscreen.Height * 3, printscreen.Width, printscreen.Height);

                //
                //Saving
                //
                string path1;
                saveScreenShotA.FileName = "Chart1234";
                saveScreenShotA.Filter = "BMP Image (*.bmp)|*.bmp| All files (*.*) | *.*";
                saveScreenShotA.ShowDialog();
                saveScreenShotA.AddExtension = true;

                path1 = saveScreenShotA.FileName;
                finalScreenShot.Save(path1, System.Drawing.Imaging.ImageFormat.Bmp);
            }
        }

        private void screenShot0_Click(object sender, EventArgs e)
        {
            string path1;

            saveScreenShot1.FileName = "Chart1";
            saveScreenShot1.Filter = "BMP Image (*.bmp)|*.bmp| All files (*.*) | *.*";
            saveScreenShot1.ShowDialog();
            saveScreenShot1.AddExtension = true;

            path1 = saveScreenShot1.FileName;
            Bitmap printscreen = GetControlScreenshot(Chart);
            printscreen.Save(path1, System.Drawing.Imaging.ImageFormat.Bmp);
        }
                
        private void screenShot1_Click(object sender, EventArgs e)
        {
            string path1;
          
            saveScreenShot2.FileName = "Chart2";
            saveScreenShot2.Filter = "BMP Image (*.bmp)|*.bmp| All files (*.*) | *.*";
            saveScreenShot2.ShowDialog();
            saveScreenShot2.AddExtension = true;

            path1 = saveScreenShot2.FileName;
            Bitmap printscreen = GetControlScreenshot(Chart1);
            printscreen.Save(path1, System.Drawing.Imaging.ImageFormat.Bmp);
         
        }            

        private void screenShot2_Click(object sender, EventArgs e)
        {
            string path1;


            saveScreenShot3.FileName = "Chart3";
            saveScreenShot3.Filter = "BMP Image (*.bmp)|*.bmp| All files (*.*) | *.*";
            saveScreenShot3.ShowDialog();
            saveScreenShot3.AddExtension = true;

            path1 = saveScreenShot3.FileName;
            Bitmap printscreen = GetControlScreenshot(Chart2);
            printscreen.Save(path1, System.Drawing.Imaging.ImageFormat.Bmp);

        }

        private void screenShot3_Click(object sender, EventArgs e)
        {
            string path1;

            saveScreenShot4.FileName = "Chart4";
            saveScreenShot4.Filter = "BMP Image (*.bmp)|*.bmp| All files (*.*) | *.*";
            saveScreenShot4.ShowDialog();
            saveScreenShot4.AddExtension = true;

            path1 = saveScreenShot4.FileName;
            Bitmap printscreen = GetControlScreenshot(Chart3);
            printscreen.Save(path1, System.Drawing.Imaging.ImageFormat.Bmp);
        }

        private void Series3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}