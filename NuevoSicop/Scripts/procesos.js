function mostrar_procesar() {

    $("#procesando_div").show(100);

    setTimeout("document.images['procesando_gif'].src='../img/procesando.gif'", 200);
}