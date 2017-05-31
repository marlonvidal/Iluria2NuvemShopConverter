using Iluria2NuvemShopConverter.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iluria2NuvemShopConverter.Strategies
{
    public class AliaStrategy : IStrategy
    {
        public bool BaixarImagens
        {
            get { return true; }
        }

        public string NomeArquivoIluria
        {
            get { return "iluria-relatorio-de-estoque-dos-produtos.csv"; }
        }

        public string NomeArquivoNuvemShop
        {
            get { return "produtos-nuvemshop.csv"; }
        }

        public string NomeLoja
        {
            get { return "Alia"; }
        }

        public string[] NomesArquivosJson
        {
            get { return new string[] { "produtos-1.json", "produtos-2.json", "produtos-3.json" }; }
        }

        public string OrganizaCategoriasProdutos(Categoria[] categorias)
        {
            string categoriasProdutos = "";

            foreach (var categoria in categorias)
            {
                if (categoria.pertenceCategoria)
                {
                    string ctg = "";

                    const string NEWS = "NEWS";
                    const string COLECOES = "COLEÇÕES";
                    const string BLUSAS = "BLUSAS";
                    const string JEANS = "JEANS";
                    const string LIQUIDACAO = "LIQUIDAÇÃO";
                    const string PRODUTOS = "PRODUTOS";

                    switch (categoria.nome)
                    {
                        case NEWS:
                            ctg = "News";
                            break;
                        case COLECOES:
                            ctg = "Coleções";
                            break;
                        case BLUSAS:
                        case JEANS:
                            ctg = "Básicos";
                            break;
                        case LIQUIDACAO:
                            ctg = "Liquidação";
                            break;
                        case PRODUTOS:
                        default:
                            ctg = "Produtos";
                            break;
                    }

                    if (categoria.subCategorias != null && categoria.subCategorias.Length > 0)
                    {
                        foreach (var subCategoria in categoria.subCategorias)
                        {
                            if (subCategoria.pertenceSubCategoria)
                            {
                                switch (categoria.nome)
                                {
                                    case COLECOES:
                                        switch (subCategoria.nome)
                                        {
                                            case "Spring Summer":
                                                ctg += " > Spring Summer, ";
                                                break;
                                            case "Outono Inverno 2017":
                                                ctg += " > Outono/Inverno 2017, ";
                                                break;
                                            default:
                                                ctg += " > Alia Army, ";
                                                break;
                                        }
                                        break;
                                    case JEANS:
                                        switch (subCategoria.nome)
                                        {
                                            case "Saia":
                                                ctg += " > Saias, ";
                                                break;
                                            case "Calça":
                                                ctg += " > Calças";
                                                break;
                                            default:
                                                ctg += " > " + subCategoria.nome + ", ";
                                                break;
                                        }
                                        break;
                                    case PRODUTOS:
                                        ctg += " > " + subCategoria.nome + ", ";
                                        break;
                                    default:
                                        ctg = " > Todos, ";
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (ctg != "Liquidação" && ctg != "News")
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
