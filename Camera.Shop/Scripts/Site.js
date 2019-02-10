$(document).ready(function () {

    /* Carousel de los productos para la pagina de inicio */

    $('#CarouselProductosEscaparate').owlCarousel({
        loop: true,
        margin: 10,
        nav: true,
        responsive: {
            0: {
                items: 1,
            },
            600: {
                items: 2,
            },
            1000: {
                items: 4,
            }
        }
    });
    
});