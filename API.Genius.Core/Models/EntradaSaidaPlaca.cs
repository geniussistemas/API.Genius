using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace API.Genius.Core.Models
{
    public class EntradaSaidaPlaca
    {
        public int Id { get; set; }
        public string Placa { get; set; } = string.Empty;
        public int IdCamera { get; set; }
        public DateTime Data { get; set; }
        public int Status { get; set; }
        public string ArquivoImagem { get; set; } = string.Empty;
    }
}