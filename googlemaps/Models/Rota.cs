namespace googlemaps.Models
{
    /// <summary>
    /// Classe que armazena informações referente a rota
    /// </summary>
    public class Rota
    {
        public int Cidade1 { get; set; }
        public int Cidade2 { get; set; }

        /// <summary>
        /// Custo entre as cidades 1 e cidade 2
        /// </summary>
        public double Custo { get; set; }
    }
}
