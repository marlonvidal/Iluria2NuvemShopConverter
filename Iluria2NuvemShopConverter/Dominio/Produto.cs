using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iluria2NuvemShopConverter.Dominio
{

    public class Produto
    {
        public string codigo { get; set; }
        public string descricao { get; set; }
        public string tabelaMedidas { get; set; }
        public string tags { get; set; }
        public string[] images { get; set; }
        public Categoria[] categorias { get; set; }
    }
}
