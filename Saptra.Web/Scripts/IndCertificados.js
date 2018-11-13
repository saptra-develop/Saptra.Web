/* global FCH, contextPath, bootbox, Backbone, bbGrid */

//js de Indicador de imcumlimiento de actividades por periodo.
//David Jasso
//19/Octubre/2018

var Indicador5 = {
    accClonar: false,
    accEscritura: false,
    accBorrar: false,
    activeForm: '',
    gridRegion: {},
    colRegion: {},
    colRegionZona: {},
    colRegionZonaActividad: {},
    colPeriodos: [],
    colRegiones: [],
    colZonas: [],
    sm: 0,
    Inicial: function () {
        $.ajaxSetup({ cache: false });
        this.CargaPeriodos();
        this.CargaRegiones();
        this.CargaZonas();
        this.Eventos();
        if (window.innerWidth < 768) {
            // Extra Small Device
            Indicador5.sm = 1;
        } 
    },
    Eventos: function () {
        var that = this;
        //$('#btnFilter').click(that.CargaGrid);

        $(document).on('click', '#btnFilter', function () {
            //Indicador5.CargaGrid();
            //Indicador5.CargaGridRegionZona();
            //Indicador5.CargaGridRegionZonaActividad();
            //Indicador5.CargaGridRegionZonaActividadFigura();
            //Indicador5.GraficoIncumplidasRegion();
            //Indicador5.GraficoIncumplidasZona();
        });

        $(document).on("change", '#selPeriodos', that.onCambiarPeriodos);
        $(document).on("change", '#selRegiones', that.onCambiarRegiones);
        $(document).on("change", '#selZonas', that.onCambiarZonas);
    },
    onCambiarPeriodos: function () {
        $('#selPeriodosMul').val($('#selPeriodos').val());
    },
    onCambiarRegiones: function () {
        $('#selRegionesMul').val($('#selRegiones').val());
    },
    onCambiarZonas: function () {
        $('#selZonasMul').val($('#selZonas').val());
    },
    CargaGrid: function () {
        $('#bbGrid-clear')[0].innerHTML = "";
        $('#bbGrid-clear')[0].innerHTML = "<div class='alert alert-warning'>" +
            "<button type='button' class='close' data-dismiss='alert'>x</button>" +
            "<i class='fa fa-spinner fa-spin fa-3x fa-fw'></i>" +
            "<span>Cargando...</span>" +
            "</div>";
        var url = contextPath + "Indicadores/CargarIndPreregistradosIncRegion?idPeriodo=" + $('#selPeriodosMul').val() + "&Region=" + $('#selRegionesMul').val() + "&Zonas=" + $('#selZonasMul').val() + "&idUsuario=" + localStorage.idUser; // El url del controlador
        $.getJSON(url, function (data) {
            if (data.Success === false) { FCH.DespliegaError(data.Message); return; }
            $('#bbGrid-clear')[0].innerHTML = "";
            Indicador5.colRegion = new Backbone.Collection(data.datos);
            var bolFilter = Indicador5.colRegion.length > 0 ? true : false;
            if (bolFilter) {
                var table = data.datos;
                var tabIndicador = "<table class='table table-striped' style='-webkit-box-shadow: 0px 0px 22px -2px rgba(0,0,0,0.75); -moz-box-shadow: 0px 0px 22px -2px rgba(0, 0, 0, 0.75);box-shadow: 0px 0px 22px -2px rgba(0, 0, 0, 0.75);border-collapse: collapse;-webkit-border-radius: 7px;-webkit-border-bottom-right-radius: 4px;-webkit-border-bottom-left-radius: 4px;-moz-border-radius: 7px;-moz-border-radius-bottomright: 4px;-moz-border-radius-bottomleft: 4px;border-radius: 7px;border-bottom-right-radius: 4px;border-bottom-left-radius: 4px;overflow: hidden;'>";
                tabIndicador += "<thead> <tr style='color:#fff; background-color:#0F7CB9;border-bottom: 3pt solid #233246;'> <th style='text-align:center;' scope='col'>Región</th> <th style='text-align:center;' scope='col'>Educando preregistrados</th> <th style='text-align:center;' scope='col'>Educando incorporados</th> <th style='text-align:center;' scope='col'>%Incorporados</th> <th style='text-align:center;' scope='col'>%Promotor</th> <th style='text-align:center;' scope='col'>%Técnicos docentes</th> <th style='text-align:center;' scope='col'>%Formadores</th> </tr> </thead>";
                for (j = 0; j < table.length; j++) {
                    tabIndicador += "<tr style='text-align:center;'>";
                    tabIndicador += "<td data-label='Región'>" + table[j]['Region'] + "</td>";
                    tabIndicador += "<td data-label='Educando preregistrados'>" + table[j]['preregistrosTotal'] + "</td>";
                    tabIndicador += "<td data-label='Educando incorporados'>" + table[j]['imcorporadostotal'] + "</td>";
                    tabIndicador += "<td data-label='%Incorporados'>" + table[j]['porcentajeIncorporadosT'] + "</td>";
                    tabIndicador += "<td data-label='%Promotor'>" + table[j]['pocentajePromotor'] + "</td>";
                    tabIndicador += "<td data-label='%Técnicos docentes'>" + table[j]['pocentajeTecnico'] + "</td>";
                    tabIndicador += "<td data-label='%Formadores'>" + table[j]['pocentajeFormador'] + "</td>";
                    tabIndicador += "</tr>";
                }

                tabIndicador += "</table>";
                $('#bbGrid-clear')[0].innerHTML = tabIndicador;
            } else {
                $('#bbGrid-clear')[0].innerHTML = "<div class='alert alert-warning'>" +
                    "<button type='button' class='close' data-dismiss='alert'>x</button>No se encontro información</div>";
            }

            //getJSON fail
        }).fail(function (e) {
            FCH.DespliegaError("No se pudo cargar la informacion de los tipos de figura");
        });
    },
    CargaGridRegionZona: function () {
        $('#indicadorZona')[0].innerHTML = "";
        $('#indicadorZona')[0].innerHTML = "<div class='alert alert-warning'>" +
            "<button type='button' class='close' data-dismiss='alert'>x</button>" +
            "<i class='fa fa-spinner fa-spin fa-3x fa-fw'></i>" +
            "<span >Cargando...</span>" +
            "</div>";
        var url = contextPath + "Indicadores/CargarIndPreregistradosIncZona?idPeriodo=" + $('#selPeriodosMul').val() + "&Region=" + $('#selRegionesMul').val() + "&Zonas=" + $('#selZonasMul').val() + "&idUsuario=" + localStorage.idUser;  // El url del controlador
        $.getJSON(url, function (data) {
            if (data.Success === false) { FCH.DespliegaError(data.Message); return; }
            $('#indicadorZona')[0].innerHTML = "";
            Indicador5.colRegionZona = new Backbone.Collection(data.datos);
            var bolFilter = Indicador5.colRegionZona.length > 0 ? true : false;
            if (bolFilter) {
                var table = data.datos;
                var tabIndicador = "<table class='table table-striped' style='-webkit-box-shadow: 0px 0px 22px -2px rgba(0,0,0,0.75); -moz-box-shadow: 0px 0px 22px -2px rgba(0, 0, 0, 0.75);box-shadow: 0px 0px 22px -2px rgba(0, 0, 0, 0.75);border-collapse: collapse;-webkit-border-radius: 7px;-webkit-border-bottom-right-radius: 4px;-webkit-border-bottom-left-radius: 4px;-moz-border-radius: 7px;-moz-border-radius-bottomright: 4px;-moz-border-radius-bottomleft: 4px;border-radius: 7px;border-bottom-right-radius: 4px;border-bottom-left-radius: 4px;overflow: hidden;'>";
                tabIndicador += "<thead> <tr style='color:#fff; background-color:#0F7CB9;border-bottom: 3pt solid #233246;'> <th style='text-align:center;' scope='col'>Región</th> <th style='text-align:center;' scope='col'>Zona</th> <th style='text-align:center;' scope='col'>Educando preregistrados</th> <th style='text-align:center;' scope='col'>Educando incorporados</th> <th style='text-align:center;' scope='col'>%Incorporados</th> <th style='text-align:center;' scope='col'>%Promotor</th> <th style='text-align:center;' scope='col'>%Técnicos docentes</th> <th style='text-align:center;' scope='col'>%Formadores</th> </tr> </thead>";
                for (j = 0; j < table.length; j++) {
                    tabIndicador += "<tr style='text-align:center;'>";
                    tabIndicador += "<td data-label='Región'>" + table[j]['Region'] + "</td>";
                    tabIndicador += "<td data-label='Zona'>" + table[j]['Zona'] + "</td>";
                    tabIndicador += "<td data-label='Educando preregistrados'>" + table[j]['preregistrosTotal'] + "</td>";
                    tabIndicador += "<td data-label='Educando incorporados'>" + table[j]['imcorporadostotal'] + "</td>";
                    tabIndicador += "<td data-label='%Incorporados'>" + table[j]['porcentajeIncorporadosT'] + "</td>";
                    tabIndicador += "<td data-label='%Promotor'>" + table[j]['pocentajePromotor'] + "</td>";
                    tabIndicador += "<td data-label='%Técnicos docentes'>" + table[j]['pocentajeTecnico'] + "</td>";
                    tabIndicador += "<td data-label='%Formadores'>" + table[j]['pocentajeFormador'] + "</td>";
                    tabIndicador += "</tr>";
                }

                tabIndicador += "</table>";
                $('#indicadorZona')[0].innerHTML = tabIndicador;
            } else {
                $('#indicadorZona')[0].innerHTML = "<div class='alert alert-warning'>" +
                    "<button type='button' class='close' data-dismiss='alert'>x</button>No se encontro información</div>";
            }

            //getJSON fail
        }).fail(function (e) {
            FCH.DespliegaError("No se pudo cargar la informacion de los tipos de figura");
        });
    },
    CargaGridRegionZonaActividad: function () {
        $('#indicadorZonaActividad')[0].innerHTML = "";
        $('#indicadorZonaActividad')[0].innerHTML = "<div class='alert alert-warning'>" +
            "<button type='button' class='close' data-dismiss='alert'>x</button>" +
            "<i class='fa fa-spinner fa-spin fa-3x fa-fw'></i>" +
            "<span >Cargando...</span>" +
            "</div>";
        var url = contextPath + "Indicadores/CargarIndPreregistradosIncZonaFigura?idPeriodo=" + $('#selPeriodosMul').val() + "&Region=" + $('#selRegionesMul').val() + "&Zonas=" + $('#selZonasMul').val() + "&idUsuario=" + localStorage.idUser;  // El url del controlador
        $.getJSON(url, function (data) {
            if (data.Success === false) { FCH.DespliegaError(data.Message); return; }
            $('#indicadorZonaActividad')[0].innerHTML = "";
            Indicador5.colRegionZonaActividad = new Backbone.Collection(data.datos);
            var bolFilter = Indicador5.colRegionZonaActividad.length > 0 ? true : false;
            if (bolFilter) {
                var table = data.datos;
                var tabIndicador = "<table class='table table-striped' style='-webkit-box-shadow: 0px 0px 22px -2px rgba(0,0,0,0.75); -moz-box-shadow: 0px 0px 22px -2px rgba(0, 0, 0, 0.75);box-shadow: 0px 0px 22px -2px rgba(0, 0, 0, 0.75);border-collapse: collapse;-webkit-border-radius: 7px;-webkit-border-bottom-right-radius: 4px;-webkit-border-bottom-left-radius: 4px;-moz-border-radius: 7px;-moz-border-radius-bottomright: 4px;-moz-border-radius-bottomleft: 4px;border-radius: 7px;border-bottom-right-radius: 4px;border-bottom-left-radius: 4px;overflow: hidden;'>";
                tabIndicador += "<thead> <tr style='color:#fff; background-color:#0F7CB9;border-bottom: 3pt solid #233246;'> <th style='text-align:center;' scope='col'>Periodo</th> <th style='text-align:center;' scope='col'>Región</th> <th style='text-align:center;' scope='col'>Zona</th> <th style='text-align:center;' scope='col'>Tipo Figura</th> <th style='text-align:center;' scope='col'>Nombre Figura</th> <th style='text-align:center;' scope='col'>Educandos preregistrados</th> <th style='text-align:center;' scope='col'>Educandos incorporados</th> <th style='text-align:center;' scope='col'>%Incorporados</th> </tr> </thead>";
                for (j = 0; j < table.length; j++) {
                    tabIndicador += "<tr style='text-align:center;'>";
                    tabIndicador += "<td data-label='Periodo'>" + table[j]['Periodo'] + "</td>";
                    tabIndicador += "<td data-label='Región'>" + table[j]['Region'] + "</td>";
                    tabIndicador += "<td data-label='Zona'>" + table[j]['Zona'] + "</td>";
                    tabIndicador += "<td data-label='Tipo Figura'>" + table[j]['DescripcionTipoFigura'] + "</td>";
                    tabIndicador += "<td data-label='Nombre Figura'>" + table[j]['NombreUsuario'] + "</td>";
                    tabIndicador += "<td data-label='Educando preregistrados'>" + table[j]['preregistrosTotal'] + "</td>";
                    tabIndicador += "<td data-label='Educando incorporados'>" + table[j]['imcorporadostotal'] + "</td>";
                    tabIndicador += "<td data-label='%Incorporados'>" + table[j]['porcentajeIncorporadosT'] + "</td>";
                    tabIndicador += "</tr>";
                }

                tabIndicador += "</table>";
                $('#indicadorZonaActividad')[0].innerHTML = tabIndicador;
            } else {
                $('#indicadorZonaActividad')[0].innerHTML = "<div class='alert alert-warning'>" +
                    "<button type='button' class='close' data-dismiss='alert'>x</button>No se encontro información</div>";
            }

            //getJSON fail
        }).fail(function (e) {
            FCH.DespliegaError("No se pudo cargar la informacion de los tipos de figura");
        });
    },
    CargaGridRegionZonaActividadFigura: function () {
        $('#indicadorRegionZonaActividad')[0].innerHTML = "";
        $('#indicadorRegionZonaActividad')[0].innerHTML = "<div class='alert alert-warning'>" +
            "<button type='button' class='close' data-dismiss='alert'>x</button>" +
            "<i class='fa fa-spinner fa-spin fa-3x fa-fw'></i>" +
            "<span >Cargando...</span>" +
            "</div>";
        var url = contextPath + "Indicadores/CargarIndPreregistradosIncZonaFiguraEducando?idPeriodo=" + $('#selPeriodosMul').val() + "&Region=" + $('#selRegionesMul').val() + "&Zonas=" + $('#selZonasMul').val() + "&idUsuario=" + localStorage.idUser;  // El url del controlador
        $.getJSON(url, function (data) {
            if (data.Success === false) { FCH.DespliegaError(data.Message); return; }
            $('#indicadorRegionZonaActividad')[0].innerHTML = "";
            Indicador5.colRegionZona = new Backbone.Collection(data.datos);
            var bolFilter = Indicador5.colRegionZona.length > 0 ? true : false;
            if (bolFilter) {
                var table = data.datos;
                var tabIndicador = "<table class='table table-striped' style='-webkit-box-shadow: 0px 0px 22px -2px rgba(0,0,0,0.75); -moz-box-shadow: 0px 0px 22px -2px rgba(0, 0, 0, 0.75);box-shadow: 0px 0px 22px -2px rgba(0, 0, 0, 0.75);border-collapse: collapse;-webkit-border-radius: 7px;-webkit-border-bottom-right-radius: 4px;-webkit-border-bottom-left-radius: 4px;-moz-border-radius: 7px;-moz-border-radius-bottomright: 4px;-moz-border-radius-bottomleft: 4px;border-radius: 7px;border-bottom-right-radius: 4px;border-bottom-left-radius: 4px;overflow: hidden;'>";
                tabIndicador += "<thead> <tr style='color:#fff; background-color:#0F7CB9;border-bottom: 3pt solid #233246;'> <th style='text-align:center;' scope='col'>Periodo</th> <th style='text-align:center;' scope='col'>Región</th> <th style='text-align:center;' scope='col'>Zona</th> <th style='text-align:center;' scope='col'>Tipo Figura</th> <th style='text-align:center;' scope='col'>Nombre Figura</th> <th style='text-align:center;' scope='col'>Nombre del educando</th> <th style='text-align:center;' scope='col'>Fecha Preregistro</th> </tr> </thead>";
                for (j = 0; j < table.length; j++) {
                    tabIndicador += "<tr style='text-align:center;'>";
                    tabIndicador += "<td data-label='Periodo'>" + table[j]['Periodo'] + "</td>";
                    tabIndicador += "<td data-label='Región'>" + table[j]['Region'] + "</td>";
                    tabIndicador += "<td data-label='Zona'>" + table[j]['Zona'] + "</td>";
                    tabIndicador += "<td data-label='Tipo Figura'>" + table[j]['DescripcionTipoFigura'] + "</td>";
                    tabIndicador += "<td data-label='Nombre Figura'>" + table[j]['NombreUsuario'] + "</td>";
                    tabIndicador += "<td data-label='Nombre del educando'>" + table[j]['NombrePreregistro'] + "</td>";
                    tabIndicador += "<td data-label='Fecha Preregistro'>" + table[j]['FechaRegistro'] + "</td>";
                    tabIndicador += "</tr>";
                }

                tabIndicador += "</table>";
                $('#indicadorRegionZonaActividad')[0].innerHTML = tabIndicador;
            } else {
                $('#indicadorRegionZonaActividad')[0].innerHTML = "<div class='alert alert-warning'>" +
                    "<button type='button' class='close' data-dismiss='alert'>x</button>No se encontro información</div>";
            }

            //getJSON fail
        }).fail(function (e) {
            FCH.DespliegaError("No se pudo cargar la informacion de los tipos de figura");
        });
    },
    GraficoIncumplidasRegion: function () {
        $('#grafico1')[0].innerHTML = ""; //remove canvas from container
        $('#grafico1')[0].innerHTML = "<div class='alert alert-warning'>" +
            "<button type='button' class='close' data-dismiss='alert'>x</button>" +
            "<i class='fa fa-spinner fa-spin fa-3x fa-fw'></i>" +
            "<span >Cargando...</span>" +
            "</div>";
        var url1 = contextPath + "Indicadores/CargarIndPreregistradosIncRegionGrafico?idPeriodo=" + $('#selPeriodosMul').val() + "&Region=" + $('#selRegionesMul').val() + "&Zonas=" + $('#selZonasMul').val() + "&idUsuario=" + localStorage.idUser;    
        $.getJSON(url1, function (data) {
        if (data.Success !== undefined) {
            FCH.DespliegaError(data.Message);
            $('#grafico1')[0].innerHTML = "";
            return;
        }
            var bolFilterFobGC = data.dataCFigura.length > 0 ? true : false;
            if (bolFilterFobGC) {

                FusionCharts.ready(function () {
                    var ChartFobGC = new FusionCharts({
                        type: 'scrollColumn2d',
                        renderAt: 'grafico1',
                        width: '100%',
                        height: '400',
                        dataFormat: 'json',
                        dataSource: {
                            "chart": {
                                "caption": "% Educandos prerregistrados vs incorporados por región",
                                "subCaption": "",
                                "captionFontSize": "14",
                                "subcaptionFontSize": "14",
                                "subcaptionFontBold": "0",

                                "baseFontSize": "15",
                                "valueFontSize": "14",
                                "baseFontColor": "#333333",
                                "baseFont": "Helvetica Neue,Arial",
                                "baseFontSize": "15",
                                "valueFontSize": "14",
                                "xAxisName": "Figura",
                                "yAxisName": "% Educandos",
                                "decimals": "2",
                                "numberprefix": "%",
                                "forceDecimals": "2",
                                "paletteColors": "#2874A6,#239B56,#D4AC0D",
                                "bgColor": "#ffffff",
                                "showBorder": "0",
                                "showCanvasBorder": "0",
                                "showPlotBorder": "0",
                                "showAlternateHgridColor": "0",
                                "showXAxisLine": "1",
                                "usePlotGradientcolor": "0",
                                "numVisiblePlot": "22",
                                "placeValuesInside": "0",
                                "rotateValues": "1",
                                "LegendShadow": "0",
                                "legendBorderAlpha": "0",
                                "base": "10",
                                "axisLineAlpha": "10",
                                "toolTipColor": "#ffffff",
                                "toolTipBorderThickness": "0",
                                "toolTipBgColor": "#000000",
                                "toolTipBgAlpha": "80",
                                "toolTipBorderRadius": "2",
                                "toolTipPadding": "5",
                                "divlineAlpha": "100",
                                "divlineColor": "#999999",
                                "divlineThickness": "1",
                                "divLineIsDashed": "1",
                                "divLineDashLen": "1",
                                "divLineGapLen": "1",
                                "flatScrollBars": "1",
                                "formatNumberScale": "0",
                                "decimalSeparator": ",",
                                "thousandSeparator": ","
                            },
                            "categories": [
                                {
                                    "category": data.modelCRegion
                                }
                            ],
                            "dataset": data.dataCFigura

                        }
                    }).render();
                });
            } else {
                $('#grafico1')[0].innerHTML = "<div class='alert alert-warning'>" +
                    "<button type='button' class='close' data-dismiss='alert'>x</button>No se encontro información</div>";
            }
                //getJSON fail
            }).fail(function () {
                FCH.DespliegaError("No se encontro información");
            });
    },
    GraficoIncumplidasZona: function () {
        $('#graficozona')[0].innerHTML = ""; //remove canvas from container
        $('#graficozona')[0].innerHTML = "<div class='alert alert-warning'>" +
            "<button type='button' class='close' data-dismiss='alert'>x</button>" +
            "<i class='fa fa-spinner fa-spin fa-3x fa-fw'></i>" +
            "<span >Cargando...</span>" +
            "</div>";
        var url1 = contextPath + "Indicadores/CargarIndPreregistradosIncZonaGrafico?idPeriodo=" + $('#selPeriodosMul').val() + "&Region=" + $('#selRegionesMul').val() + "&Zonas=" + $('#selZonasMul').val() + "&idUsuario=" + localStorage.idUser;
        $.getJSON(url1, function (data) {
            if (data.Success !== undefined) {
                FCH.DespliegaError(data.Message);
                $('#graficozona')[0].innerHTML = "";
                return;
            }
            var bolFilterFobGC = data.dataCFigura.length > 0 ? true : false;
            if (bolFilterFobGC) {

                FusionCharts.ready(function () {
                    var ChartFobGC = new FusionCharts({
                        type: 'scrollColumn2d',
                        renderAt: 'graficozona',
                        width: '100%',
                        height: '400',
                        dataFormat: 'json',
                        dataSource: {
                            "chart": {
                                "caption":"% Educandos prerregistrados vs incorporados por zona",
                                "subCaption": "",
                                "captionFontSize": "14",
                                "subcaptionFontSize": "14",
                                "subcaptionFontBold": "0",

                                "baseFontSize": "15",
                                "valueFontSize": "14",
                                "baseFontColor": "#333333",
                                "baseFont": "Helvetica Neue,Arial",
                                "baseFontSize": "15",
                                "valueFontSize": "14",
                                "xAxisName": "Figura",
                                "yAxisName": "% Educandos",
                                "decimals": "2",
                                "numberprefix": "%",
                                "forceDecimals": "2",
                                "paletteColors": "#2874A6,#239B56,#D4AC0D",
                                "bgColor": "#ffffff",
                                "showBorder": "0",
                                "showCanvasBorder": "0",
                                "showPlotBorder": "0",
                                "showAlternateHgridColor": "0",
                                "showXAxisLine": "1",
                                "usePlotGradientcolor": "0",
                                "numVisiblePlot": "22",
                                "placeValuesInside": "0",
                                "rotateValues": "1",
                                "LegendShadow": "0",
                                "legendBorderAlpha": "0",
                                "base": "10",
                                "axisLineAlpha": "10",
                                "toolTipColor": "#ffffff",
                                "toolTipBorderThickness": "0",
                                "toolTipBgColor": "#000000",
                                "toolTipBgAlpha": "80",
                                "toolTipBorderRadius": "2",
                                "toolTipPadding": "5",
                                "divlineAlpha": "100",
                                "divlineColor": "#999999",
                                "divlineThickness": "1",
                                "divLineIsDashed": "1",
                                "divLineDashLen": "1",
                                "divLineGapLen": "1",
                                "flatScrollBars": "1",
                                "formatNumberScale": "0",
                                "decimalSeparator": ",",
                                "thousandSeparator": ","
                            },
                            "categories": [
                                {
                                    "category": data.modelCZona
                                }
                            ],
                            "dataset": data.dataCFigura

                        }
                    }).render();
                });
            } else {
                $('#graficozona')[0].innerHTML = "<div class='alert alert-warning'>" +
                    "<button type='button' class='close' data-dismiss='alert'>x</button>No se encontro información</div>";
            }
            //getJSON fail
        }).fail(function () {
            FCH.DespliegaError("No se encontro información");
        });
    },
    CargaPeriodos: function () {
        if (Indicador5.colPeriodos.length < 1) {
            var url = contextPath + "Periodos/CargarPeriodos?idEstatus=5";
            $.getJSON(url, function (data) {
                Indicador5.colPeriodos = data;
                Indicador5.CargaListaPeriodos();
            }).fail(function () {
                FCH.DespliegaErrorDialogo("No se pudieron cargar los peridos");
            });
        } else {
            Indicador5.CargaListaPeriodos();
        }
    },
    CargaListaPeriodos: function () {
        var select = $(Indicador5.activeForm + ' #selPeriodos').empty();

        $.each(Indicador5.colPeriodos, function (i, item) {
            select.append('<option value="' + item.id + '">' + item.nombre + '</option>');
        });

        $("#selPeriodos").select2({ allowClear: true, placeholder: "PERIODOS" });
        $("#selPeriodosMul").val('');
    },
    CargaRegiones: function () {
        if (Indicador5.colRegiones.length < 1) {
            var url = contextPath + "Regiones/CargarRegionesPorRol?idEstatus=5&idUsuario=" + localStorage.idUser;
            $.getJSON(url, function (data) {
                Indicador5.colRegiones = data;
                Indicador5.CargaListaRegiones();
            }).fail(function () {
                FCH.DespliegaErrorDialogo("No se pudieron cargar las regiones");
            });
        } else {
            Indicador5.CargaListaRegiones();
        }
    },
    CargaListaRegiones: function () {
        var select = $(Indicador5.activeForm + ' #selRegiones').empty();

        $.each(Indicador5.colRegiones, function (i, item) {
            select.append('<option value="' + item.id + '">' + item.nombre + '</option>');
        });

        $("#selRegiones").select2({ allowClear: true, placeholder: "REGIONES" });
        $("#selRegionesMul").val('');
    },
    CargaZonas: function () {
        if (Indicador5.colZonas.length < 1) {
            var url = contextPath + "Zonas/CargarZonasPorRol?idEstatus=5&idUsuario=" + localStorage.idUser;
            $.getJSON(url, function (data) {
                Indicador5.colZonas = data;
                Indicador5.CargaListaZonas();
            }).fail(function () {
                FCH.DespliegaErrorDialogo("No se pudieron cargar las zonas");
            });
        } else {
            Indicador5.CargaListaZonas();
        }
    },
    CargaListaZonas: function () {
        var select = $(Indicador5.activeForm + ' #selZonas').empty();

        $.each(Indicador5.colZonas, function (i, item) {
            select.append('<option value="' + item.id + '">' + item.nombre + '</option>');
        });

        $("#selZonas").select2({ allowClear: true, placeholder: "ZONAS" });
        $("#selZonasMul").val('');
    }
};

$(function () {
    Indicador5.Inicial();
});