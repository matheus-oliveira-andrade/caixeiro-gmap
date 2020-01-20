using GMap.NET;
using GMap.NET.MapProviders;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace googlemaps.Models
{
    class Algoritmo
    {
        // Constante que diz o custo máximo que pode ter entre duas cidades
        //const int MaxCusto = 50;

        public double[,] MatrizAdjacenteCusto;
        public MapRoute[,] MatrizdePontosadjacentes;
        public int[] Permutacao;
        public Rota[] MelhorRota;
        public double MelhorCusto;
        public int NumCidades;
        public int CY = 0;

        private RichTextBox RichTxtB;

        public Algoritmo(List<PointLatLng> pontos, int numeroCidades, RichTextBox richTxtB)
        {
            NumCidades = numeroCidades;
            RichTxtB = richTxtB;

            MatrizAdjacenteCusto = MontaGrafo(pontos);
            Permutacao = new int[numeroCidades];
            MelhorRota = new Rota[numeroCidades];
            GeraEscolheCaminhos(Permutacao, MelhorRota);

            RichTxtB.AppendText("Caminho gerado com sucesso!!!\n");
        }

        // Método principal
        public void GeraEscolheCaminhos(int[] permutacao, Rota[] melhorRota)
        {
            RichTxtB.AppendText("Gerando caminho.....\n");

            int controle = -1;
            MelhorCusto = int.MaxValue;
            for (int i = 0; i < melhorRota.Length; i++)
                melhorRota[i] = new Rota();
            Permuta(permutacao, melhorRota, controle, 1);

        }

        // Gera todos os caminhos possiveis entre a cidade 0 (inicial) e as outras (n -1) envolvidas na busca do caminho
        // Estas rotas ficam armazenadas no vetor de permutação, uma por vez
        // A cada rota gerada é chama a funcção melhorCaminho que escolhe o caminho com menor custo
        public void Permuta(int[] permutacao, Rota[] melhorRota, int controle, int k)
        {
            int i;
            permutacao[k] = ++controle;

            // Se gerou um caminho então verifica se ele é melhor 
            if (controle == (melhorRota.Length - 1))
                MelhorCaminho(melhorRota, permutacao);
            else
                // Gera a rota
                for (i = 1; i < melhorRota.Length; i++)
                    if (permutacao[i] == 0)
                        Permuta(permutacao, melhorRota, controle, i);

            controle--;
            permutacao[k] = 0;
        }

        //Verifica se a permutação passada como parâmetro tem custo melhor que o custo já obtido.
        //Caso seja melhor,  então monta a rota correspondente à permutação como sendo a 
        // melhor rota(e armazena no vetor melhorRota, 
        // retornando tambem o custo total da melhor rota.
        public void MelhorCaminho(Rota[] melhorRota, int[] permutacao)
        {

            //Contadores: auxiliam na montagem das rotas 
            int j, k;

            //Cidades da melhor rota
            int cid1, cid2;

            //Custo total da melhor rota 
            double custo;

            //Vetor que armazena a sequencia de cidades que estao em uma rota
            //Cada indice: indica uma cidade 
            //O conteudo deste indice: indica a proxima cidade da rota 
            int[] proxDaRota;

            MapRoute t;

            // Inicializa o vetor de melhor rota
            proxDaRota = new int[melhorRota.Length];

            // montagem de uma rota com a permutações
            //A primeira cidade é a cidade 0 
            cid1 = 0;
            cid2 = permutacao[1];
            t = MatrizdePontosadjacentes[cid1, cid2];
            custo = t.Distance;

            proxDaRota[cid1] = cid2;

            // Percorre o array de melhor rota
            // Montando a rota e tendo um custo parcial desta rota ao final
            for (j = 2; j < melhorRota.Length; j++)
            {
                cid1 = cid2;
                cid2 = permutacao[j];
                t = MatrizdePontosadjacentes[cid1, cid2];
                custo += t.Distance;
                proxDaRota[cid1] = cid2;
            }

            // completa o ciclo da viagem voltando ao início        
            // Todas as rotas tem ligações com todas as outras rotas, ou seja, todas as rotas existem soluções
            proxDaRota[cid2] = 0;

            t = MatrizdePontosadjacentes[cid1, 0];
            custo += t.Distance;

            // Se o custo obtido nesta rota for melhor que o obtido anteriormente
            if (custo < MelhorCusto)
            {
                MelhorCusto = custo;
                cid2 = 0;

                // Armazena a rota no array de melhor rota
                for (k = 0; k < melhorRota.Length; k++)
                {
                    cid1 = cid2;
                    cid2 = proxDaRota[cid1];
                    melhorRota[k].Cidade1 = cid1;
                    melhorRota[k].Cidade2 = cid2;

                    t = MatrizdePontosadjacentes[cid1, cid2];

                    melhorRota[k].Custo = t.Distance;
                }
            }
        }

        /// <summary>
        /// Gera os pesos dos arcos do grafo com valores aleatórios
        /// Preenche a matriz(grafo) M, que é indexada pelos nomes dos vertices(cidades)        
        /// </summary>                
        public double[,] MontaGrafo(List<PointLatLng> pontos)
        {
            int numCidades = pontos.Count;
            MatrizAdjacenteCusto = new double[numCidades, numCidades];
            MatrizdePontosadjacentes = new MapRoute[numCidades, numCidades];

            RichTxtB.AppendText("Montando Grafo....\n");

            for (CY = 0; CY < numCidades; CY++)
            {
                for (int cX = 0; cX < numCidades; cX++)
                {
                    MapRoute rota = GoogleMapProvider.Instance.GetRoute(pontos[CY], pontos[cX], false, false, 12);
                    MatrizdePontosadjacentes[CY, cX] = rota;
                    double total = Math.Round(rota.Distance, 3);
                    MatrizAdjacenteCusto[CY, cX] = total;

                }
            }
            return MatrizAdjacenteCusto;
        }
    }
}
