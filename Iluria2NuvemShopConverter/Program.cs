using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Iluria2NuvemShopConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            var dic = new Dictionary<string, dynamic>();

            using (var reader = new StreamReader(@"C:\temp\iluria-relatorio-de-estoque-dos-produtos.csv"))
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

                    dic[id].Url = RemoveDiacritics(aux[1]).Replace(' ', '-').ToLower();
                    dic[id].Nome = aux[1];
                    dic[id].Descricao = "";
                    dic[id].Tags = "";

                    var preco = Convert.ToDecimal(aux[6]).ToString("0.00").Replace(",", ".");

                    dic[id].Informacoes.Add(new { Estoque = estoque, Preco = preco, Variacao1 = aux[2], Variacao2 = aux[3], Variacao3 = aux[4] });
                }
            }

            foreach (var arquivo in new string[] { "produtos-1.json", "produtos-2.json", "produtos-3.json" })
            {
                using (StreamReader re = new StreamReader(arquivo))
                {
                    using (JsonTextReader jsonReader = new JsonTextReader(re))
                    {
                        JsonSerializer se = new JsonSerializer();

                        var produtos = se.Deserialize<Produto[]>(jsonReader);

                        foreach (var item in produtos)
                        {
                            if (dic.ContainsKey(item.codigo))
                            {
                                dic[item.codigo].Descricao = item.descricao.Replace("\n", "<br />");
                                dic[item.codigo].Tags = item.tags.Substring(0, item.tags.LastIndexOf(',')).Trim();

                                var i = 1;

                                foreach (var imagem in item.images)
                                {
                                    string diretorio = $@"C:\temp\produtos\{dic[item.codigo].Url}";

                                    if (!Directory.Exists(diretorio))
                                        Directory.CreateDirectory(diretorio);

                                    var urlImagem = imagem.Replace("loadThumb('", "");
                                    urlImagem = urlImagem.Replace("');", "");
                                    urlImagem = "https:" + urlImagem;


                                    using (WebClient client = new WebClient())
                                        client.DownloadFile(new Uri(urlImagem), $@"{diretorio}\\imagem-{i}.jpg");

                                    i++;
                                }
                            }
                        }
                    }
                }
            }

            var arquivoFinal = @"C:\temp\produtos-nuvemshop.csv";

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
                            linha = linha.Replace("{Categorias}", "");
                            linha = linha.Replace("{Nome da variação 1}", "");
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

        static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }

    public class Rootobject
    {
        public Produto[] produtos { get; set; }
    }

    public class Produto
    {
        public string codigo { get; set; }
        public string descricao { get; set; }
        public string tags { get; set; }
        public string[] images { get; set; }
    }
}
