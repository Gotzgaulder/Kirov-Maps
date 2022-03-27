using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms.ToolTips;

namespace WindowsFormsApp4
{
    public partial class Form1 : Form
    {
        private GMapOverlay markersOverlay = new GMapOverlay("markers");
        
        public Form1()
        {
            InitializeComponent();
        }

        private void gMapControl1_Load(object sender, EventArgs e)
        {
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache; //выбор подгрузки карты – онлайн или из ресурсов
            gMapControl1.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance; //какой провайдер карт используется (в нашем случае гугл) 
            gMapControl1.MinZoom = 2; //минимальный зум
            gMapControl1.MaxZoom = 24; //максимальный зум
            gMapControl1.Zoom = 12; // какой используется зум при открытии
            gMapControl1.Position = new GMap.NET.PointLatLng(58.5966, 49.6601);// точка в центре карты при открытии (Киров)
            gMapControl1.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter; // как приближает (просто в центр карты или по положению мыши)
            gMapControl1.CanDragMap = true; // перетаскивание карты мышью
            gMapControl1.DragButton = MouseButtons.Left; // какой кнопкой осуществляется перетаскивание
            gMapControl1.ShowCenter = false; //показывать или скрывать красный крестик в центре
            gMapControl1.ShowTileGridLines = false; //показывать или скрывать тайлы
            gMapControl1.PolygonsEnabled = true;//включение полигонов
            gMapControl1.NegativeMode = false;//включение негатив мода
            gMapControl1.MarkersEnabled = true;//включение маркеров
            gMapControl1.RoutesEnabled = true;//вклюечние маршрутов
            gMapControl1.Overlays.Add(markersOverlay);

            gMapControl1.MouseClick += new MouseEventHandler(gMapControl1_MouseClick);
        }

        

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
           

        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void gMapControl1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //textBox1.Text = gMapControl1.FromLocalToLatLng(e.X, e.Y).Lat.ToString();//Широта
            //textBox2.Text = gMapControl1.FromLocalToLatLng(e.X, e.Y).Lng.ToString();//Долгота
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void gMapControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                PointLatLng point = gMapControl1.FromLocalToLatLng(e.X, e.Y);
                GMapMarker marker = new GMarkerGoogle(point, GMarkerGoogleType.green);
                markersOverlay.Markers.Add(marker);
            }
        }

        private void gMapControl1_MouseMove(object sender, MouseEventArgs e) //Отслеживание положения мыши на карте, и вывод координат в левый нижний угол.
        {

            label3.Text = gMapControl1.FromLocalToLatLng(e.X, e.Y).Lat.ToString() + " " + gMapControl1.FromLocalToLatLng(e.X, e.Y).Lng.ToString();

        }

        private void label3_Click(object sender, EventArgs e)
        {
            
        }
        DataTable dtRouter;
        private void button2_Click(object sender, EventArgs e)
        {
            string url = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=true_or_false&language=ru",
            Uri.EscapeDataString(textBox6.Text), Uri.EscapeDataString(textBox7.Text));

            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);//Получаем ответ от интернет-ресурса.
            System.Net.WebResponse response = request.GetResponse();//Экземпляр класса System.IO.Stream для чтения данных из интернет-ресурса.
            System.IO.Stream dataStream = response.GetResponseStream();//Инициализируем новый экземпляр класса System.IO.StreamReader для указанного потока.
            System.IO.StreamReader sreader = new System.IO.StreamReader(dataStream);//Инициализируем новый экземпляр класса System.IO.StreamReader для указанного потока.
            string responsereader = sreader.ReadToEnd();//Считывает поток от текущего положения до конца.
            response.Close();//Закрываем поток ответа.
            System.Xml.XmlDocument xmldoc = new System.Xml.XmlDocument();
            xmldoc.LoadXml(responsereader);

            if (xmldoc.GetElementsByTagName("status")[0].ChildNodes[0].InnerText == "OK")
            {
                System.Xml.XmlNodeList nodes =
               xmldoc.SelectNodes("//leg//step");

                //Формируем строку для добавления в таблицу.
                object[] dr;
                for (int i = 0; i < nodes.Count; i++)
                {
                    //Указываем что массив будет состоять из 
                    //восьми значений.
                    dr = new object[8];
                    //Номер шага.
                    dr[0] = i;
                    //Получение координат начала отрезка.
                    dr[1] = xmldoc.SelectNodes("//start_location").Item(i).SelectNodes("lat").Item(0).InnerText.ToString();
                    dr[2] = xmldoc.SelectNodes("//start_location").Item(i).SelectNodes("lng").Item(0).InnerText.ToString();
                    //Получение координат конца отрезка.
                    dr[3] = xmldoc.SelectNodes("//end_location").Item(i).SelectNodes("lat").Item(0).InnerText.ToString();
                    dr[4] = xmldoc.SelectNodes("//end_location").Item(i).SelectNodes("lng").Item(0).InnerText.ToString();
                    //Получение времени необходимого для прохождения этого отрезка.
                    dr[5] = xmldoc.SelectNodes("//duration").Item(i).SelectNodes("text").Item(0).InnerText.ToString();
                    //Получение расстояния, охватываемое этим отрезком.
                    dr[6] = xmldoc.SelectNodes("//distance").Item(i).SelectNodes("text").Item(0).InnerText.ToString();
                    //Получение инструкций для этого шага, представленные в виде текстовой строки HTML.
                    dr[7] = HtmlToPlainText(xmldoc.SelectNodes("//html_instructions").Item(i).InnerText.ToString());
                    //Добавление шага в таблицу.
                    dtRouter.Rows.Add(dr);
                }
                //Выводим в текстовое поле адрес начала пути.
                textBox6.Text = xmldoc.SelectNodes("//leg//start_address").Item(0).InnerText.ToString();
                //Выводим в текстовое поле адрес конца пути.
                textBox7.Text = xmldoc.SelectNodes("//leg//end_address").Item(0).InnerText.ToString();
                //Выводим в текстовое поле время в пути.
                textBox4.Text = xmldoc.GetElementsByTagName("duration")[nodes.Count].ChildNodes[1].InnerText;
                //Выводим в текстовое поле расстояние от начальной до конечной точки.
                textBox4.Text = xmldoc.GetElementsByTagName("distance")[nodes.Count].ChildNodes[1].InnerText;

                //Переменные для хранения координат начала и конца пути.
                double latStart = 0.0;
                double lngStart = 0.0;
                double latEnd = 0.0;
                double lngEnd = 0.0;

                //Получение координат начала пути.
                latStart = System.Xml.XmlConvert.ToDouble(xmldoc.GetElementsByTagName("start_location")[nodes.Count].ChildNodes[0].InnerText);
                lngStart = System.Xml.XmlConvert.ToDouble(xmldoc.GetElementsByTagName("start_location")[nodes.Count].ChildNodes[1].InnerText);
                //Получение координат конечной точки.
                latEnd = System.Xml.XmlConvert.ToDouble(xmldoc.GetElementsByTagName("end_location")[nodes.Count].ChildNodes[0].InnerText);
                lngEnd = System.Xml.XmlConvert.ToDouble(xmldoc.GetElementsByTagName("end_location")[nodes.Count].ChildNodes[1].InnerText);

                //Выводим в текстовое поле координаты начала пути.
                textBox3.Text = latStart + ";" + lngStart;
                //Выводим в текстовое поле координаты конечной точки.
                textBox8.Text = latEnd + ";" + lngEnd;

                //Устанавливаем позицию карты на начало пути.
                gMapControl1.Position = new GMap.NET.PointLatLng(latStart, lngStart);

                //Очищаем все маршруты.
                markersOverlay.Routes.Clear();

                //Создаем маршрут на основе списка контрольных точек.
                GMap.NET.WindowsForms.GMapRoute r = new GMap.NET.WindowsForms.GMapRoute(list, "Route");

                //Указываем, что данный маршрут должен отображаться.
                r.IsVisible = true;

                //Устанавливаем цвет маршрута.
                r.Stroke.Color = Color.DarkGreen;

                //Добавляем маршрут.
                markersOverlay.Routes.Add(r);

                //Добавляем в компонент, список маркеров и маршрутов.
                gMapControl1.Overlays.Add(markersOverlay);

                //Указываем, что при загрузке карты будет использоваться 
                //9ти кратное приближение.
                gMapControl1.Zoom = 9;

                //Обновляем карту.
                gMapControl1.Refresh();
            
        }

            {
                //Получение широты и долготы.
                System.Xml.XmlNodeList nodes = xmldoc.SelectNodes("//location");
                //Переменные широты и долготы.
                double latitude = 0.0;
                double longitude = 0.0;
                //Получаем широту и долготу.
                foreach (System.Xml.XmlNode node in nodes)
                {
                    latitude =
                       System.Xml.XmlConvert.ToDouble(node.SelectSingleNode("lat").InnerText.ToString());
                    longitude =
                       System.Xml.XmlConvert.ToDouble(node.SelectSingleNode("lng").InnerText.ToString());
                }
                //Варианты получения информации о найденном объекте.
                //Вариант 1.
                string formatted_address =
                   xmldoc.SelectNodes("//formatted_address").Item(0).InnerText.ToString();
                //Создаем новый список маркеров, с указанием компонента 
                //в котором они будут использоваться и названием списка.
                GMapOverlay markersOverlay = new GMapOverlay("markers");//МОЁ
                GMap.NET.WindowsForms.Markers.GMarkerGoogle markerG = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(new GMap.NET.PointLatLng(latitude, longitude),GMap.NET.WindowsForms.Markers.GMarkerGoogleType.blue_pushpin);
                markerG.ToolTip = new GMap.NET.WindowsForms.ToolTips.GMapRoundedToolTip(markerG);
                markersOverlay.Markers.Add(markerG);//Добавляем маркеры в список маркеров.
                gMapControl1.Overlays.Clear();//Очищаем список маркеров компонента.
                gMapControl1.Overlays.Add(markersOverlay);//Добавляем в компонент, список маркеров.
                gMapControl1.Refresh();//Обновляем карту.
            }
        }

        private object HtmlToPlainText(string v)
        {
            throw new NotImplementedException();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            markersOverlay.Markers.Clear();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }
    }
}
