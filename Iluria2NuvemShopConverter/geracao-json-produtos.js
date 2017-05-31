function httpGet(theUrl, callback, obj) {
    if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
        xmlhttp = new XMLHttpRequest();
    }
    else {// code for IE6, IE5
        xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
    }
    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {
            callback(xmlhttp.responseText, theUrl, obj);
        }
    }
    xmlhttp.open("GET", theUrl, false);
    xmlhttp.send();
}

var produtos = [];

$('.product-thumb').find('a').each(function () {
    httpGet('http://admin.iluria.com' + $(this).attr('href'), function (response, url) {
        console.log('processando pagina ' + url);

        var produto = {};
        produto.codigo = url.substr(url.indexOf('webCode') + 8, 6);
        produto.descricao = $(response).find('#description-pt-br').text().trim();
        produto.tabelaMedidas = '';

        var tabelaMedidas = $(response).find('img[src*=size_chart]');

        if (tabelaMedidas.length > 0)
            produto.tabelaMedidas = 'https:' + tabelaMedidas.attr('src')

        produto.tags = '';

        $(response).find('#tags-pt-br').find('a').each(function () {
            produto.tags += $(this).text().trim() + ', ';
        });

        produto.images = [];

        $(response).find('.handCursor').each(function () {
            produto.images.push($(this).attr('onclick'));
        });


        httpGet('http://admin.iluria.com/product.do?command=showProductCategoriesEditForm&webCode=' + produto.codigo + '&menuId=menu_products', function (response2, url2, produto2) {
            console.log('processando pagina ' + url2);

            var categorias = [];

            $(response2).find('#categoryList').find('.category').each(function () {

                var categoriaPrincipal = $(this).find('.categoryTitleContainer table');

                var pertenceCategoria = false;
                var nomeCategoria = '';

                if (categoriaPrincipal.find('td:first').find('input[type=checkbox]').is(':checked'))
                    pertenceCategoria = true;

                nomeCategoria = categoriaPrincipal.find('td:last').find('.categoryTitle').text();

                var categoria = {
                    nome: nomeCategoria,
                    pertenceCategoria: pertenceCategoria,
                    subCategorias: []
                }

                $(this).find('.subCategory').each(function () {

                    var subCategoria = $(this).find('.subCategoryTitleContainer table');

                    var pertenceSubCategoria = false;

                    if (subCategoria.find('td:first').find('input[type=checkbox]').is(':checked'))
                        pertenceSubCategoria = true;

                    var nomeSubCategoria = subCategoria.find('td:last').find('.subCategoryTitle').text();

                    var item = {
                        nome: nomeSubCategoria,
                        pertenceSubCategoria: pertenceSubCategoria
                    };

                    categoria.subCategorias.push(item);

                });

                categorias.push(categoria);
            });

            produto2.categorias = categorias;

            produtos.push(produto2);
        }, produto);
    });
});

console.log(JSON.stringify(produtos));