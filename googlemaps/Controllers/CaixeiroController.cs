using GMap.NET;
using googlemaps.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace googlemaps.Controllers
{

    class CaixeiroController
    {
        public Rota[] MelhorRota
        {
            get { return solver.MelhorRota; }
        }

        public MapRoute[,] MatrizPontos
        {
            get { return solver.MatrizdePontosadjacentes; }
        }

        private Algoritmo solver;

        public CaixeiroController(List<PointLatLng> pontos, int numeroCidades, RichTextBox richTxtB)
        {
            solver = new Algoritmo(pontos, numeroCidades, richTxtB);
        }
    }
}
