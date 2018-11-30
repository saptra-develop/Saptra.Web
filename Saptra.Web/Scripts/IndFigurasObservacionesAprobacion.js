/* global FCH, contextPath, bootbox, Backbone, bbGrid */

//js de Indicador de imcumlimiento de actividades por periodo.
//David Jasso
//19/Octubre/2018

var Indicador2 = {
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
            Indicador2.sm = 1;
        } 
    },
    Eventos: function () {
        var that = this;
        $('#btnFilter').click(that.CargaGrid);

        $(document).on('click', '#btnFilter', function () {
            Indicador2.CargaGrid();
            Indicador2.CargaGridRegionZona();
            Indicador2.CargaGridRegionZonaActividad();
            Indicador2.CargaGridRegionZonaActividadFigura();
            Indicador2.GraficoIncumplidasRegion();
            Indicador2.GraficoIncumplidasZona();
        });

        $(document).on("change", '#selPeriodos', that.onCambiarPeriodos);
        $(document).on("change", '#selRegiones', that.onCambiarRegiones);
        $(document).on("change", '#selZonas', that.onCambiarZonas);
        $(document).on('click', '.btnExportar', function () {
            Indicador2.onExportarFormato();
        });
        $(document).on('click', '.btnExportarExcel', function () {
            Indicador2.onExportarExcel();
        });
        $(document).on('click', '.btnExportarPDF', function () {
            Indicador2.onExportarPDF();
        });
    },
    onExportarFormato: function () {
        var htmlMessage = "<h4>Exportar</h4>";
        htmlMessage += "<h5>Formato de reporte</h5>";
        htmlMessage += "<div class='text-center'><div class='btn-group btn-group-md'>";
        htmlMessage += "<button class='btn btn-success btnExportarExcel'><i class='fa fa-file-excel-o'></i> Excel</button>";
        htmlMessage += "<button class='btn btn-danger btnExportarPDF' ><i class='fa fa-file-pdf-o'></i> PDF </button>";
        htmlMessage += "</div></div>";

        bootbox.alert({
            message: htmlMessage,
            size: 'small'
        });
    },
    onExportarExcel: function () {
        var url = contextPath + "Indicadores/ExportIndObservacionesExcel?idPeriodo=" + $('#selPeriodosMul').val() + "&Region=" + $('#selRegionesMul').val() + "&Zonas=" + $('#selZonasMul').val() + "&idUsuario=" + localStorage.idUser;  // El url del controlador

        window.open(url, '_blank');
    },
    onExportarPDF: function () {
        var url = contextPath + "Pdf/ExportIndicadorAprobacionPDF?idPeriodo=" + $('#selPeriodosMul').val() + "&Region=" + $('#selRegionesMul').val() + "&Zonas=" + $('#selZonasMul').val() + "&idUsuario=" + localStorage.idUser;  // El url del controlador

        window.open(url, '_blank');
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
            "<span >Cargando...</span>" +
            "</div>";
        var url = contextPath + "Indicadores/CargarIndFigurasObservacionesRegion?idPeriodo=" + $('#selPeriodosMul').val() + "&Region=" + $('#selRegionesMul').val() + "&Zonas=" + $('#selZonasMul').val() + "&idUsuario=" + localStorage.idUser;  // El url del controlador
        $.getJSON(url, function (data) {
            if (data.Success === false) { FCH.DespliegaError(data.Message); return; }
            $('#bbGrid-clear')[0].innerHTML = "";
            Indicador2.colRegion = new Backbone.Collection(data.datos);
            var bolFilter = Indicador2.colRegion.length > 0 ? true : false;
            if (bolFilter) {
                var table = data.datos;
                var tabIndicador = "<table class='table table-striped' style='-webkit-box-shadow: 0px 0px 22px -2px rgba(0,0,0,0.75); -moz-box-shadow: 0px 0px 22px -2px rgba(0, 0, 0, 0.75);box-shadow: 0px 0px 22px -2px rgba(0, 0, 0, 0.75);border-collapse: collapse;-webkit-border-radius: 7px;-webkit-border-bottom-right-radius: 4px;-webkit-border-bottom-left-radius: 4px;-moz-border-radius: 7px;-moz-border-radius-bottomright: 4px;-moz-border-radius-bottomleft: 4px;border-radius: 7px;border-bottom-right-radius: 4px;border-bottom-left-radius: 4px;overflow: hidden;'>";
                tabIndicador += "<thead> <tr style='color:#fff; background-color:#0F7CB9;border-bottom: 3pt solid #233246;'> <th style='text-align:center;'>Región</th> <th style='text-align:center;'>Actividades aprobadas</th> <th style='text-align:center;'>Actividades con observaciones</th> <th style='text-align:center;'>%Observaciones</th> <th style='text-align:center;'>%Promotor</th> <th style='text-align:center;'>%Tecnico docente</th> <th style='text-align:center;'>%Formador</th>  </tr> </thead>";
                for (j = 0; j < table.length; j++) {
                    tabIndicador += "<tr style='text-align:center;'>";
                    tabIndicador += "<td scope='row'>" + table[j]['Region'] + "</td>";
                    tabIndicador += "<td>" + table[j]['planeadastotal'] + "</td>";
                    tabIndicador += "<td>" + table[j]['incumplidastotal'] + "</td>";
                    tabIndicador += "<td>" + table[j]['porcentajeRechazadasT'] + "</td>";
                    tabIndicador += "<td>" + table[j]['pocentajePromotor'] + "</td>";
                    tabIndicador += "<td>" + table[j]['pocentajeTecnico'] + "</td>";
                    tabIndicador += "<td>" + table[j]['pocentajeFormador'] + "</td>";
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
        var url = contextPath + "Indicadores/CargarIndFigurasObservacionesRegionZona?idPeriodo=" + $('#selPeriodosMul').val() + "&Region=" + $('#selRegionesMul').val() + "&Zonas=" + $('#selZonasMul').val() + "&idUsuario=" + localStorage.idUser;  // El url del controlador
        $.getJSON(url, function (data) {
            if (data.Success === false) { FCH.DespliegaError(data.Message); return; }
            $('#indicadorZona')[0].innerHTML = "";
            Indicador2.colRegionZona = new Backbone.Collection(data.datos);
            var bolFilter = Indicador2.colRegionZona.length > 0 ? true : false;
            if (bolFilter) {
                var table = data.datos;
                var tabIndicador = "<table class='table table-striped' style='-webkit-box-shadow: 0px 0px 22px -2px rgba(0,0,0,0.75); -moz-box-shadow: 0px 0px 22px -2px rgba(0, 0, 0, 0.75);box-shadow: 0px 0px 22px -2px rgba(0, 0, 0, 0.75);border-collapse: collapse;-webkit-border-radius: 7px;-webkit-border-bottom-right-radius: 4px;-webkit-border-bottom-left-radius: 4px;-moz-border-radius: 7px;-moz-border-radius-bottomright: 4px;-moz-border-radius-bottomleft: 4px;border-radius: 7px;border-bottom-right-radius: 4px;border-bottom-left-radius: 4px;overflow: hidden;'>";
                tabIndicador += "<thead> <tr style='color:#fff; background-color:#0F7CB9;border-bottom: 3pt solid #233246;'> <th style='text-align:center;'>Región</th> <th style='text-align:center;'>Zona</th> <th style='text-align:center;'>Actividades aprobadas</th> <th style='text-align:center;'>Actividades con observaciones</th> <th style='text-align:center;'>%Observaciones</th> <th style='text-align:center;'>%Promotor</th> <th style='text-align:center;'>%Tecnico docente</th> <th style='text-align:center;'>%Formador</th>  </tr> </thead>";
                for (j = 0; j < table.length; j++) {
                    tabIndicador += "<tr style='text-align:center;'>";
                    tabIndicador += "<td scope='row'>" + table[j]['Region'] + "</td>";
                    tabIndicador += "<td>" + table[j]['Zona'] + "</td>";
                    tabIndicador += "<td>" + table[j]['planeadastotal'] + "</td>";
                    tabIndicador += "<td>" + table[j]['incumplidastotal'] + "</td>";
                    tabIndicador += "<td>" + table[j]['porcentajeRechazadasT'] + "</td>";
                    tabIndicador += "<td>" + table[j]['pocentajePromotor'] + "</td>";
                    tabIndicador += "<td>" + table[j]['pocentajeTecnico'] + "</td>";
                    tabIndicador += "<td>" + table[j]['pocentajeFormador'] + "</td>";
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
        var url = contextPath + "Indicadores/CargarIndFigurasObservacionesRegionZonaFigNom?idPeriodo=" + $('#selPeriodosMul').val() + "&Region=" + $('#selRegionesMul').val() + "&Zonas=" + $('#selZonasMul').val() + "&idUsuario=" + localStorage.idUser;  // El url del controlador
        $.getJSON(url, function (data) {
            if (data.Success === false) { FCH.DespliegaError(data.Message); return; }
            $('#indicadorZonaActividad')[0].innerHTML = "";
            Indicador2.colRegionZonaActividad = new Backbone.Collection(data.datos);
            var bolFilter = Indicador2.colRegionZonaActividad.length > 0 ? true : false;
            if (bolFilter) {
                var table = data.datos;
                var tabIndicador = "<table class='table table-striped' style='-webkit-box-shadow: 0px 0px 22px -2px rgba(0,0,0,0.75); -moz-box-shadow: 0px 0px 22px -2px rgba(0, 0, 0, 0.75);box-shadow: 0px 0px 22px -2px rgba(0, 0, 0, 0.75);border-collapse: collapse;-webkit-border-radius: 7px;-webkit-border-bottom-right-radius: 4px;-webkit-border-bottom-left-radius: 4px;-moz-border-radius: 7px;-moz-border-radius-bottomright: 4px;-moz-border-radius-bottomleft: 4px;border-radius: 7px;border-bottom-right-radius: 4px;border-bottom-left-radius: 4px;overflow: hidden;'>";
                tabIndicador += "<thead> <tr style='color:#fff; background-color:#0F7CB9;border-bottom: 3pt solid #233246;'> <th style='text-align:center;'>Periodo</th> <th style='text-align:center;'>Región</th> <th style='text-align:center;'>Zona</th> <th style='text-align:center;'>Tipo Figura</th> <th style='text-align:center;'>Nombre Figura</th> <th style='text-align:center;'>Actividades aprobadas</th> <th style='text-align:center;'>Actividades observadas</th> <th style='text-align:center;'>%Observaciones</th> </tr> </thead>";
                for (j = 0; j < table.length; j++) {
                    tabIndicador += "<tr style='text-align:center;'>";
                    tabIndicador += "<td scope='row'>" + table[j]['Periodo'] + "</td>";
                    tabIndicador += "<td>" + table[j]['Region'] + "</td>";
                    tabIndicador += "<td>" + table[j]['Zona'] + "</td>";
                    tabIndicador += "<td>" + table[j]['TipoFigura'] + "</td>";
                    tabIndicador += "<td>" + table[j]['NombreFigura'] + "</td>";
                    tabIndicador += "<td>" + table[j]['planeadastotal'] + "</td>";
                    tabIndicador += "<td>" + table[j]['incumplidastotal'] + "</td>";
                    tabIndicador += "<td>" + table[j]['porcentajeRechazadasT'] + "</td>";
                    //tabIndicador += "<td>" + table[j]['pocentajePromotor'] + "</td>";
                    //tabIndicador += "<td>" + table[j]['pocentajeTecnico'] + "</td>";
                    //tabIndicador += "<td>" + table[j]['pocentajeFormador'] + "</td>";
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
        var url = contextPath + "Indicadores/CargarIndFigurasObservacionesRegionZonaFigNomAct?idPeriodo=" + $('#selPeriodosMul').val() + "&Region=" + $('#selRegionesMul').val() + "&Zonas=" + $('#selZonasMul').val() + "&idUsuario=" + localStorage.idUser;  // El url del controlador
        $.getJSON(url, function (data) {
            if (data.Success === false) { FCH.DespliegaError(data.Message); return; }
            $('#indicadorRegionZonaActividad')[0].innerHTML = "";
            Indicador2.colRegionZona = new Backbone.Collection(data.datos);
            var bolFilter = Indicador2.colRegionZona.length > 0 ? true : false;
            if (bolFilter) {
                var table = data.datos;
                var tabIndicador = "<table class='table table-striped' style='-webkit-box-shadow: 0px 0px 22px -2px rgba(0,0,0,0.75); -moz-box-shadow: 0px 0px 22px -2px rgba(0, 0, 0, 0.75);box-shadow: 0px 0px 22px -2px rgba(0, 0, 0, 0.75);border-collapse: collapse;-webkit-border-radius: 7px;-webkit-border-bottom-right-radius: 4px;-webkit-border-bottom-left-radius: 4px;-moz-border-radius: 7px;-moz-border-radius-bottomright: 4px;-moz-border-radius-bottomleft: 4px;border-radius: 7px;border-bottom-right-radius: 4px;border-bottom-left-radius: 4px;overflow: hidden;'>";
                tabIndicador += "<thead> <tr style='color:#fff; background-color:#0F7CB9;border-bottom: 3pt solid #233246;'> <th style='text-align:center;'>Periodo</th> <th style='text-align:center;'>Región</th> <th style='text-align:center;'>Zona</th> <th style='text-align:center;'>Tipo Figura</th> <th style='text-align:center;'>Nombre Figura</th> <th style='text-align:center;'>Tipo de Actividad</th> <th style='text-align:center;'>Comentarios CZ</th>  </tr> </thead>";
                for (j = 0; j < table.length; j++) {
                    tabIndicador += "<tr style='text-align:center;'>";
                    tabIndicador += "<td scope='row'>" + table[j]['Periodo'] + "</td>";
                    tabIndicador += "<td>" + table[j]['Region'] + "</td>";
                    tabIndicador += "<td>" + table[j]['Zona'] + "</td>";
                    tabIndicador += "<td>" + table[j]['TipoFigura'] + "</td>";
                    tabIndicador += "<td>" + table[j]['NombreFigura'] + "</td>";
                    tabIndicador += "<td>" + table[j]['DescripcionActividad'] + "</td>";
                    tabIndicador += "<td>" + table[j]['ComentariosRechazo'] + "</td>";
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
        var url1 = contextPath + "Indicadores/CargarIndFigurasObservacionesRegionGrafico?idPeriodo=" + $('#selPeriodosMul').val() + "&Region=" + $('#selRegionesMul').val() + "&Zonas=" + $('#selZonasMul').val() + "&idUsuario=" + localStorage.idUser;    
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
                                "caption": "% Observaciones durante la aprobación por región",
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
                                "yAxisName": "% Observaciones",
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
        var url1 = contextPath + "Indicadores/CargarIndFigurasObservacionesRegionZonaGrafico?idPeriodo=" + $('#selPeriodosMul').val() + "&Region=" + $('#selRegionesMul').val() + "&Zonas=" + $('#selZonasMul').val() + "&idUsuario=" + localStorage.idUser;
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
                                "caption": "% Observaciones durante la aprobación por zona",
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
                                "yAxisName": "% Observaciones",
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
        if (Indicador2.colPeriodos.length < 1) {
            var url = contextPath + "Periodos/CargarPeriodos?idEstatus=5";
            $.getJSON(url, function (data) {
                Indicador2.colPeriodos = data;
                Indicador2.CargaListaPeriodos();
            }).fail(function () {
                FCH.DespliegaErrorDialogo("No se pudieron cargar los peridos");
            });
        } else {
            Indicador2.CargaListaPeriodos();
        }
    },
    CargaListaPeriodos: function () {
        var select = $(Indicador2.activeForm + ' #selPeriodos').empty();

        $.each(Indicador2.colPeriodos, function (i, item) {
            select.append('<option value="' + item.id + '">' + item.nombre + '</option>');
        });

        $("#selPeriodos").select2({ allowClear: true, placeholder: "PERIODOS" });
        $("#selPeriodosMul").val('');
    },
    CargaRegiones: function () {
        if (Indicador2.colRegiones.length < 1) {
            var url = contextPath + "Regiones/CargarRegionesPorRol?idEstatus=5&idUsuario=" + localStorage.idUser;
            $.getJSON(url, function (data) {
                Indicador2.colRegiones = data;
                Indicador2.CargaListaRegiones();
            }).fail(function () {
                FCH.DespliegaErrorDialogo("No se pudieron cargar las regiones");
            });
        } else {
            Indicador2.CargaListaRegiones();
        }
    },
    CargaListaRegiones: function () {
        var select = $(Indicador2.activeForm + ' #selRegiones').empty();

        $.each(Indicador2.colRegiones, function (i, item) {
            select.append('<option value="' + item.id + '">' + item.nombre + '</option>');
        });

        $("#selRegiones").select2({ allowClear: true, placeholder: "REGIONES" });
        $("#selRegionesMul").val('');
    },
    CargaZonas: function () {
        if (Indicador2.colZonas.length < 1) {
            var url = contextPath + "Zonas/CargarZonasPorRol?idEstatus=5&idUsuario=" + localStorage.idUser;
            $.getJSON(url, function (data) {
                Indicador2.colZonas = data;
                Indicador2.CargaListaZonas();
            }).fail(function () {
                FCH.DespliegaErrorDialogo("No se pudieron cargar las zonas");
            });
        } else {
            Indicador2.CargaListaZonas();
        }
    },
    CargaListaZonas: function () {
        var select = $(Indicador2.activeForm + ' #selZonas').empty();

        $.each(Indicador2.colZonas, function (i, item) {
            select.append('<option value="' + item.id + '">' + item.nombre + '</option>');
        });

        $("#selZonas").select2({ allowClear: true, placeholder: "ZONAS" });
        $("#selZonasMul").val('');
    }
};

$(function () {
    Indicador2.Inicial();
});