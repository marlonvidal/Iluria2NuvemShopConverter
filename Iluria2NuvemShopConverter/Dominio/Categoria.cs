using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iluria2NuvemShopConverter.Dominio
{

    public class Categoria
    {
        public string nome { get; set; }
        public bool pertenceCategoria { get; set; }
        public SubCategoria[] subCategorias { get; set; }
    }
}
