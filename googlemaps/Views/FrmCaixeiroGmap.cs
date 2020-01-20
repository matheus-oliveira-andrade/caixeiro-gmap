using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using googlemaps.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Configuration;

namespace googlemaps
{

    public partial class FrmCaixeiroGmap : Form
    {
        private List<PointLatLng> pontos;
        private double latitude, longitude;
        private GMapOverlay overlay = new GMapOverlay("Marcador");

        // Solucionador
        private Controllers.CaixeiroController controller;

        public FrmCaixeiroGmap()
        {
            InitializeComponent();

            InicializaMapa();
        }

        private void Map_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

                PointLatLng Point = Map.FromLocalToLatLng(e.X, e.Y);
                latitude = Point.Lat;
                longitude = Point.Lng;

                // adiciona marcador ao mapa
                AdicionaMarcador(Point);

                // adiciona pontos a lista
                pontos.Add(Point);

                RichTxtB.AppendText("Localização marcada com sucesso!\n");
                RichTxtB.AppendText($"Latitude: {longitude} | Longitude: {latitude}\n\n");

                AtualizarMapa();
            }
        }

        private void InicializaMapa()
        {
            pontos = new List<PointLatLng>();
            GMapProviders.GoogleMap.ApiKey = ConfigurationSettings.AppSettings["ApiKeyGMap"];
            Map.MapProvider = GMapProviders.GoogleMap;
            Map.DragButton = MouseButtons.Left;
            Map.SetPositionByKeywords("Pinhais, Paraná, Brazil");
            Map.MinZoom = 3;
            Map.MaxZoom = 15;
            Map.Zoom = 13;
        }

        private void AtualizarMapa()
        {
            Map.Zoom--;
            Map.Zoom++;
        }
        
        private void AdicionaMarcador(PointLatLng pointLatLng, GMarkerGoogleType gMarker = GMarkerGoogleType.yellow)
        {

            GMarkerGoogle Marker = new GMarkerGoogle(pointLatLng, Properties.Resources.icons8_marcador_23)
            {
                ToolTipText = $"Latitude: {Map.Position.Lat} \n Longitude: {Map.Position.Lng}"
            };

            overlay.Markers.Add(Marker);

            Map.Overlays.Add(overlay);

            AtualizarMapa();

        }

        private void BtnCalcular_Click(object sender, EventArgs e)
        {
            if (pontos.Count <= 0)
            {
                MessageBox.Show("Clique com o botão direito do mouse sobre o mapa, para marcar pontos!", "Nenhum ponto marcado!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            try
            {
                controller = new Controllers.CaixeiroController(pontos, pontos.Count, RichTxtB);

                RichTxtB.AppendText("Mostrando o caminho...\n");

                ImprimeMelhorCaminho(controller.MelhorRota, controller.MatrizPontos);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro inesperado!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void sairToolStripMenuItem_Click(object sender, EventArgs e) { Application.Exit(); }
        private void button1_Click(object sender, EventArgs e) { Application.Exit(); }
        private void button2_Click(object sender, EventArgs e) { Application.Restart(); }

        private void sobreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Matheus de Oliveira de Andrade - 2018100841", "Developers", MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        public void ImprimeMelhorCaminho(Rota[] melhorRota, MapRoute[,] MatrizdePontosadjacentes)
        {
            GMapOverlay routes = new GMapOverlay("Routes");

            for (int i = 0; i < melhorRota.Length; i++)
            {
                int y = melhorRota[i].Cidade1;
                int x = melhorRota[i].Cidade2;

                MapRoute trajeto = MatrizdePontosadjacentes[y, x];

                GMapRoute rt1 = new GMapRoute(trajeto.Points, "Rota")
                {
                    Stroke = new Pen(Color.FromArgb(50, 162, 90), 5)
                };
                routes.Routes.Add(rt1);
            }

            Map.Overlays.Add(routes);

            AtualizarMapa();
        }
    }

}






