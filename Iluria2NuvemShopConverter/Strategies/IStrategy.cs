using Iluria2NuvemShopConverter.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iluria2NuvemShopConverter.Strategies
{
    public interface IStrategy
    {
        string NomeLoja { get; }
        string NomeArquivoIluria { get; }
        string NomeArquivoNuvemShop { get; }
        string[] NomesArquivosJson { get; }
        string OrganizaCategoriasProdutos(Categoria[] categorias);
        bool BaixarImagens { get; }
    }
}
