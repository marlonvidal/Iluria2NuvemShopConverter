using Iluria2NuvemShopConverter.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iluria2NuvemShopConverter.Strategies
{
    public class MermaidStrategy : IStrategy
    {
        public bool BaixarImagens
        {
            get { return true; }
        }

        public string NomeArquivoIluria
        {
            get
            {
                return "produtos-mermaid.csv";
            }
        }

        public string NomeArquivoNuvemShop
        {
            get
            {
                return "produtos-nuvemshop-mermaid.csv";
            }
        }

        public string NomeLoja
        {
            get
            {
                return "Mermaid";
            }
        }

        public string[] NomesArquivosJson
        {
            get
            {
                return new string[] { "produtos-1.json", "produtos-2.json", "produtos-3.json", "produtos-4.json", "produtos-5.json", "produtos-6.json", "produtos-7.json" };
            }
        }

        public string OrganizaCategoriasProdutos(Categoria[] categorias)
        {
            var categoriasProdutos = "";

            foreach (var categoria in categorias)
            {
                if (categoria.pertenceCategoria)
                {
                    string ctg = categoria.nome;

                    if (categoria.subCategorias != null && categoria.subCategorias.Length > 0)
                    {
                        foreach (var subCategoria in categoria.subCategorias)
                            if (subCategoria.pertenceSubCategoria)
                                ctg += " > " + subCategoria.nome + ", ";
                    }
                    else
                    {
                        ctg += " > Todos";
                    }

                    if (!ctg.Contains(","))
                        categoriasProdutos += ctg + ", ";
                    else
                        categoriasProdutos += ctg;
                }
            }

            if (string.IsNullOrEmpty(categoriasProdutos))
                categoriasProdutos = "Produtos > Todos";
            else
                categoriasProdutos = categoriasProdutos.Substring(0, categoriasProdutos.LastIndexOf(','));

            return categoriasProdutos;
        }
    }
}
