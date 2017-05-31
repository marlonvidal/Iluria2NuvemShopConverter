using Iluria2NuvemShopConverter.Dominio;
using Iluria2NuvemShopConverter.Strategies;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Iluria2NuvemShopConverter
{
    class Program
    {
        static Dictionary<string, int> ContaQuantidadeRepeticoesRegistros(string arquivo)
        {
            var dic1 = new Dictionary<string, int>();

            using (var reader = new StreamReader($@"C:\temp\{ arquivo }"))
            {
                int i = 0;
                string linha = "";
                while ((linha = reader.ReadLine()) != null)
                {
                    i++;

                    if (i == 1)
                        continue;

                    var aux = linha.Split(';');

                    var id = aux[0];
                    var estoque = aux[5];

                    if (estoque == "Esgotado")
                        continue;


                    if (!dic1.ContainsKey(id))
                        dic1.Add(id, 0);

                    dic1[id]++;
                }
            }

            return dic1;
        }

        static void CompararQuantidadeArquivos()
        {
            var estrategia = new MermaidStrategy();
            var dic1 = ContaQuantidadeRepeticoesRegistros("novo-arquivo.csv");
            var dic2 = ContaQuantidadeRepeticoesRegistros(estrategia.NomeArquivoIluria);

            var asd = new List<string>();

            foreach (var item in dic2)
            {
                if (!dic1.ContainsKey(item.Key))
                    asd.Add(item.Key);
            }

            var teste = dic2.Where(x => x.Value > 1);
        }

        static void TrataArquivoFinal()
        {
            var dic = new Dictionary<string, dynamic>();
            string header = "";
            int i = 0;
            using (var reader = new StreamReader($@"C:\temp\produtos-mermaid_.csv"))
            {
                string linha = "";
                while ((linha = reader.ReadLine()) != null)
                {
                    i++;

                    if (i == 1)
                    {
                        header = linha;
                        continue;
                    }

                    var aux = linha.Split(';');

                    var id = aux[0];
                    var estoque = aux[5];

                    if (estoque == "Esgotado")
                        continue;


                    if (!dic.ContainsKey(id))
                    {
                        dynamic registro = new ExpandoObject();
                        registro.Informacoes = new List<dynamic>();

                        dic.Add(id, registro);
                    }

                    var nome = aux[1];

                    if (nome.Contains(id))
                        nome = nome.Substring(0, nome.IndexOf(id)).Trim().Replace("-", "").Trim();

                    dic[id].Nome = nome.ToTitleCase();
                    //dic[id].Url = nome.RemoveAccents().Replace(' ', '-').ToLower();
                    dic[id].Descricao = "";
                    dic[id].Tags = "";
                    dic[id].Categorias = "";


                    var preco = "0,00";

                    if (!string.IsNullOrEmpty(aux[6]))
                        preco = aux[6];

                    dic[id].Informacoes.Add(new { Estoque = estoque, Preco = preco, Variacao1 = aux[2], Variacao2 = aux[3], Variacao3 = aux[4] });
                }
            }
            using (var fileStream = new FileStream("C:\\temp\\produtos-mermaid.csv", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                using (var sw = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    sw.WriteLine(header);

                    foreach (var item in dic)
                    {
                        foreach (var info in item.Value.Informacoes)
                        {
                            sw.WriteLine(string.Format("{0};{1};{2};{3};{4};{5};{6};;",
                               item.Key,
                               item.Value.Nome,
                               info.Variacao1,
                               info.Variacao2,
                               info.Variacao3,
                               info.Estoque,
                               info.Preco));
                        }
                    }
                }
            }
        }

        static void GeraArquivoFinal()
        {
            IStrategy estrategia = null;
            estrategia = new MermaidStrategy();
            //estrategia = new AliaStrategy();

            var dic = new Dictionary<string, dynamic>();

            using (var reader = new StreamReader($@"C:\temp\{ estrategia.NomeArquivoIluria }"))
            {
                var i = 0;

                string linha = "";
                while ((linha = reader.ReadLine()) != null)
                {
                    i++;

                    if (i == 1)
                        continue;

                    var aux = linha.Split(';');

                    var id = aux[0];
                    var estoque = aux[5];

                    if (estoque == "Esgotado")
                        continue;


                    if (!dic.ContainsKey(id))
                    {
                        dynamic registro = new ExpandoObject();
                        registro.Informacoes = new List<dynamic>();

                        dic.Add(id, registro);
                    }

                    dic[id].Url = aux[1].RemoveAccents().Replace(' ', '-').ToLower();
                    dic[id].Nome = aux[1].ToTitleCase();
                    dic[id].Descricao = "";
                    dic[id].Tags = "";
                    dic[id].Categorias = "";

                    var preco = "0.00";

                    if (!string.IsNullOrEmpty(aux[6]))
                        preco = Convert.ToDecimal(aux[6]).ToString("0.00").Replace(",", ".");

                    dic[id].Informacoes.Add(new { Estoque = estoque, Preco = preco, Variacao1 = aux[2], Variacao2 = aux[3], Variacao3 = aux[4] });
                }
            }

            foreach (var arquivo in estrategia.NomesArquivosJson)
            {
                using (StreamReader re = new StreamReader($"{ estrategia.NomeLoja}\\{arquivo}"))
                {
                    using (JsonTextReader jsonReader = new JsonTextReader(re))
                    {
                        JsonSerializer se = new JsonSerializer();

                        var produtos = se.Deserialize<Produto[]>(jsonReader);

                        foreach (var item in produtos)
                        {
                            if (dic.ContainsKey(item.codigo))
                            {
                                if (!string.IsNullOrEmpty(item.tabelaMedidas))
                                {
                                    var diretorio = $@"c:\temp\{ estrategia.NomeLoja }\tabelamedidas\";

                                    if (!Directory.Exists(diretorio))
                                        Directory.CreateDirectory(diretorio);

                                    if (estrategia.BaixarImagens)
                                        using (var client = new WebClient())
                                            client.DownloadFile(new Uri(item.tabelaMedidas), $@"{diretorio}\\{ dic[item.codigo].Url }.jpg");
                                }

                                dic[item.codigo].Descricao = item.descricao.Replace("\n", "<br />");

                                if (!string.IsNullOrEmpty(item.tags))
                                    dic[item.codigo].Tags = item.tags.Substring(0, item.tags.LastIndexOf(',')).Trim();

                                if (estrategia.BaixarImagens)
                                {
                                    var i = 1;

                                    foreach (var imagem in item.images)
                                    {
                                        string diretorio = $@"C:\temp\{ estrategia.NomeLoja}\produtos\{dic[item.codigo].Url}";

                                        if (!Directory.Exists(diretorio))
                                            Directory.CreateDirectory(diretorio);

                                        var urlImagem = imagem.Replace("loadThumb('", "");
                                        urlImagem = urlImagem.Replace("');", "");
                                        urlImagem = "https:" + urlImagem;

                                        try
                                        {
                                            using (WebClient client = new WebClient())
                                                client.DownloadFile(new Uri(urlImagem), $@"{diretorio}\\imagem-{i}.jpg");
                                        }
                                        catch (UriFormatException) { }

                                        i++;
                                    }
                                }

                                dic[item.codigo].Categorias = estrategia.OrganizaCategoriasProdutos(item.categorias);
                            }
                        }
                    }
                }
            }

            var arquivoFinal = $@"C:\temp\{ estrategia.NomeArquivoNuvemShop}";

            using (var fileStream = new FileStream(arquivoFinal, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                using (var writer = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    writer.WriteLine("Identificador URL;Nome;Categorias;Nome da variação 1;Valor da variação 1;Nome da variação 2;Valor da variação 2;Nome da variação 3;Valor da variação 3;Preço;Preço promocional;Peso;Altura;Largura;Comprimento;Estoque;SKU;Exibir na loja;Frete gratis;Descrição;Tags");

                    foreach (var item in dic)
                    {
                        foreach (var informacao in item.Value.Informacoes)
                        {
                            var linha = "{Identificador URL};{Nome};{Categorias};{Nome da variação 1};{Valor da variação 1};{Nome da variação 2};{Valor da variação 2};{Nome da variação 3};{Valor da variação 3};{Preço};{Preço promocional};{Peso};{Altura};{Largura};{Comprimento};{Estoque};{SKU};{Exibir na loja};{Frete gratis};{Descrição};{Tags}";

                            linha = linha.Replace("{Identificador URL}", item.Value.Url);
                            linha = linha.Replace("{Nome}", item.Value.Nome);
                            linha = linha.Replace("{Categorias}", item.Value.Categorias);
                            linha = linha.Replace("{Nome da variação 1}", "Tamanho");
                            linha = linha.Replace("{Valor da variação 1}", informacao.Variacao1);
                            linha = linha.Replace("{Nome da variação 2}", "");
                            linha = linha.Replace("{Valor da variação 2}", informacao.Variacao2);
                            linha = linha.Replace("{Nome da variação 3}", "");
                            linha = linha.Replace("{Valor da variação 3}", informacao.Variacao3);
                            linha = linha.Replace("{Preço}", informacao.Preco);
                            linha = linha.Replace("{Preço promocional}", "");
                            linha = linha.Replace("{Peso}", "0.20");
                            linha = linha.Replace("{Altura}", "10.00");
                            linha = linha.Replace("{Largura}", "20.00");
                            linha = linha.Replace("{Comprimento}", "20.00");
                            linha = linha.Replace("{Estoque}", informacao.Estoque);
                            linha = linha.Replace("{SKU}", item.Key);
                            linha = linha.Replace("{Exibir na loja}", "SIM");
                            linha = linha.Replace("{Frete gratis}", "NÂO");
                            linha = linha.Replace("{Descrição}", item.Value.Descricao);
                            linha = linha.Replace("{Tags}", item.Value.Tags);

                            writer.WriteLine(linha);
                        }
                    }
                }
            }
        }


        static void Main(string[] args)
        {
            TrataArquivoFinal();
            //CompararQuantidadeArquivos();
            GeraArquivoFinal();
        }
    }
}
