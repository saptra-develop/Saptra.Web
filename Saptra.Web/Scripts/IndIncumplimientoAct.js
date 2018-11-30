/* global FCH, contextPath, bootbox, Backbone, bbGrid */

//js de Indicador de imcumlimiento de actividades por periodo.
//David Jasso
//19/Octubre/2018

var Indicador1 = {
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
    colTiposActividad: [],
    sm: 0,
    Inicial: function () {
        $.ajaxSetup({ cache: false });
        this.CargaPeriodos();
        this.CargaRegiones();
        this.CargaZonas();
        this.CargaTiposActividad();
       
        this.Eventos();
        if (window.innerWidth < 768) {
            // Extra Small Device
            Indicador1.sm = 1;
        } 
    },
    Eventos: function () {
        var that = this;
        //$('#btnFilter').click(that.CargaGrid);

        $(document).on('click', '#btnFilter', function () {
            Indicador1.CargaGrid();
            Indicador1.CargaGridRegionZona();
            Indicador1.CargaGridRegionZonaActividad();
            Indicador1.CargaGridRegionZonaActividadFigura();
            Indicador1.GraficoIncumplidasRegion();
            Indicador1.GraficoIncumplidasZona();
        });

        $(document).on("change", '#selPeriodos', that.onCambiarPeriodos);
        $(document).on("change", '#selRegiones', that.onCambiarRegiones);
        $(document).on("change", '#selZonas', that.onCambiarZonas);
        $(document).on("change", '#selTipoActividad', that.onCambiarActividades);
        $(document).on('click', '.btnExportar', function () {
            Indicador1.onExportarFormato();
        });
        $(document).on('click', '.btnExportarExcel', function () {
            Indicador1.onExportarExcel();
        });
        $(document).on('click', '.btnExportarPDF', function () {
            Indicador1.onExportarPDF();
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
        var url = contextPath + "Indicadores/ExportIndIncumplimientoExcel?idPeriodo=" + $('#selPeriodosMul').val() + "&Region=" + $('#selRegionesMul').val() + "&Zonas=" + $('#selZonasMul').val() + "&TipoActividad=" + $('#selTipoActividadMul').val() + "&idUsuario=" + localStorage.idUser; // El url del controlador

        window.open(url, '_blank');
    },
    onExportarPDF: function () {
        var url = contextPath + "Pdf/ExportIndicadorIncumplidasPDF?idPeriodo=" + $('#selPeriodosMul').val() + "&Region=" + $('#selRegionesMul').val() + "&Zonas=" + $('#selZonasMul').val() + "&TipoActividad=" + $('#selTipoActividadMul').val() + "&idUsuario=" + localStorage.idUser; // El url del controlador

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
    onCambiarActividades: function () {
        $('#selTipoActividadMul').val($('#selTipoActividad').val());
    },
    CargaGrid: function () {
        $('#bbGrid-clear')[0].innerHTML = "";
        $('#bbGrid-clear')[0].innerHTML = "<div class='alert alert-warning'>" +
            "<button type='button' class='close' data-dismiss='alert'>x</button>" +
            "<i class='fa fa-spinner fa-spin fa-3x fa-fw'></i>" + 
            "<span>Cargando...</span>" +
            "</div>";
        var url = contextPath + "Indicadores/CargarIndRegion?idPeriodo=" + $('#selPeriodosMul').val() + "&Region=" + $('#selRegionesMul').val() + "&Zonas=" + $('#selZonasMul').val() + "&TipoActividad=" + $('#selTipoActividadMul').val() + "&idUsuario=" + localStorage.idUser; // El url del controlador
        $.getJSON(url, function (data) {
            if (data.Success === false) { FCH.DespliegaError(data.Message); return; }
            $('#bbGrid-clear')[0].innerHTML = "";
            Indicador1.colRegion = new Backbone.Collection(data.datos);
            var bolFilter = Indicador1.colRegion.length > 0 ? true : false;
            if (bolFilter) {
                var table = data.datos;
                var tabIndicador = "<table class='table table-striped' style='-webkit-box-shadow: 0px 0px 22px -2px rgba(0,0,0,0.75); -moz-box-shadow: 0px 0px 22px -2px rgba(0, 0, 0, 0.75);box-shadow: 0px 0px 22px -2px rgba(0, 0, 0, 0.75);border-collapse: collapse;-webkit-border-radius: 7px;-webkit-border-bottom-right-radius: 4px;-webkit-border-bottom-left-radius: 4px;-moz-border-radius: 7px;-moz-border-radius-bottomright: 4px;-moz-border-radius-bottomleft: 4px;border-radius: 7px;border-bottom-right-radius: 4px;border-bottom-left-radius: 4px;overflow: hidden;'>";
                tabIndicador += "<thead> <tr style='color:#fff; background-color:#0F7CB9;border-bottom: 3pt solid #233246;'> <th style='text-align:center;' scope='col'>REGIÓN</th> <th style='text-align:center;' scope='col'>Total Actividades planeadas</th> <th style='text-align:center;' scope='col'>Incumplidas</th> <th style='text-align:center;' scope='col'>%Incumplimiento</th> <th style='text-align:center;' scope='col'>%Incumplimiento Promotor</th> <th style='text-align:center;' scope='col'>%Incumplimiento Tecnico docente</th> <th style='text-align:center;' scope='col'>%Incumplimiento Formador</th> </tr> </thead>";
                for (j = 0; j < table.length; j++) {
                    tabIndicador += "<tr style='text-align:center;'>";
                    tabIndicador += "<td data-label='Región'>" + table[j]['region'] + "</td>";
                    tabIndicador += "<td data-label='Total Actividades planeadas'>" + table[j]['planeadastotal'] + "</td>";
                    tabIndicador += "<td data-label='Incumplidas'>" + table[j]['incumplidastotal'] + "</td>";
                    tabIndicador += "<td data-label='%Incumplimiento'>" + table[j]['pocentajeTotal'] + "</td>";
                    tabIndicador += "<td data-label='%Incumplimiento Promotor'>" + table[j]['pocentajepromotor'] + "</td>";
                    tabIndicador += "<td data-label='%Incumplimiento Tecnico docente'>" + table[j]['pocentajetecnico'] + "</td>";
                    tabIndicador += "<td data-label='%Incumplimiento Formador'>" + table[j]['pocentajeformador'] + "</td>";
                    tabIndicador += "</tr>";
                }

                tabIndicador += "</table>";
                $('#bbGrid-clear')[0].innerHTML = tabIndicador;
                //Indicador1.gridRegion = new bbGrid.View({
                //    container: $('#bbGrid-clear'),
                //    detalle: false,
                //    clone: false,
                //    editar: true,
                //    borrar: true,
                //    collection: Indicador1.colRegion,
                //    colModel: [
                //        { title: 'REGIÓN', name: 'region',  index: true },
                //        { title: 'Total Actividades planeadas', name: 'planeadastotal',  index: true },
                //        { title: 'Incumplidas', name: 'incumplidastotal',  index: true },
                //        { title: '% Incumplimiento Promotor', name: 'pocentajepromotor',  index: true },
                //        { title: '% Incumplimiento Tecnico Docente', name: 'pocentajetecnico',  index: true },
                //        { title: '% Incumplimiento Formador', name: 'pocentajeformador',  index: true }]

                //});
                //$('#cargandoInfo').hide();
              
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
            "<span>Cargando...</span>" +
            "</div>";
        var url = contextPath + "Indicadores/CargarIndRegionZona?idPeriodo=" + $('#selPeriodosMul').val() + "&Region=" + $('#selRegionesMul').val() + "&Zonas=" + $('#selZonasMul').val() + "&TipoActividad=" + $('#selTipoActividadMul').val() + "&idUsuario=" + localStorage.idUser; // El url del controlador
        $.getJSON(url, function (data) {
            if (data.Success === false) { FCH.DespliegaError(data.Message); return; }
            $('#indicadorZona')[0].innerHTML = "";
            Indicador1.colRegionZona = new Backbone.Collection(data.datos);
            var bolFilter = Indicador1.colRegionZona.length > 0 ? true : false;
            if (bolFilter) {
                var table = data.datos;
                var tabIndicador = "<table class='table table-striped' style='-webkit-box-shadow: 0px 0px 22px -2px rgba(0,0,0,0.75); -moz-box-shadow: 0px 0px 22px -2px rgba(0, 0, 0, 0.75);box-shadow: 0px 0px 22px -2px rgba(0, 0, 0, 0.75);border-collapse: collapse;-webkit-border-radius: 7px;-webkit-border-bottom-right-radius: 4px;-webkit-border-bottom-left-radius: 4px;-moz-border-radius: 7px;-moz-border-radius-bottomright: 4px;-moz-border-radius-bottomleft: 4px;border-radius: 7px;border-bottom-right-radius: 4px;border-bottom-left-radius: 4px;overflow: hidden;'>";
                tabIndicador += "<thead> <tr style='color:#fff; background-color:#0F7CB9;border-bottom: 3pt solid #233246;'> <th style='text-align:center;'>Región</th> <th style='text-align:center;'>Zona</th> <th style='text-align:center;'>Total Actividades planeadas</th> <th style='text-align:center;' scope='col'>Incumplidas</th> <th style='text-align:center;' scope='col'>%Incumplimiento</th> <th style='text-align:center;'>%Incumplimiento Promotor</th> <th style='text-align:center;'>%Incumplimiento Tecnico docente</th> <th style='text-align:center;'>%Incumplimiento Formador</th> </tr> </thead>";
                for (j = 0; j < table.length; j++) {
                    tabIndicador += "<tr style='text-align:center;'>";
                    tabIndicador += "<td data-label='Región'>" + table[j]['region'] + "</td>";
                    tabIndicador += "<td data-label='Zona'>" + table[j]['zona'] + "</td>";
                    tabIndicador += "<td data-label='Total Actividades planeadas'>" + table[j]['planeadastotal'] + "</td>";
                    tabIndicador += "<td data-label='Incumplidas'>" + table[j]['incumplidastotal'] + "</td>";
                    tabIndicador += "<td data-label='%Incumplimiento'>" + table[j]['pocentajeTotal'] + "</td>";
                    tabIndicador += "<td data-label='%Incumplimiento Promotor'>" + table[j]['pocentajepromotor'] + "</td>";
                    tabIndicador += "<td data-label='%Incumplimiento Tecnico docente'>" + table[j]['pocentajetecnico'] + "</td>";
                    tabIndicador += "<td data-label='%Incumplimiento Formador'>" + table[j]['pocentajeformador'] + "</td>";
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
            "<span>Cargando...</span>" +
            "</div>";
        var url = contextPath + "Indicadores/CargarIndRegionZonaTipoActividad?idPeriodo=" + $('#selPeriodosMul').val() + "&Region=" + $('#selRegionesMul').val() + "&Zonas=" + $('#selZonasMul').val() + "&TipoActividad=" + $('#selTipoActividadMul').val() + "&idUsuario=" + localStorage.idUser; // El url del controlador
        $.getJSON(url, function (data) {
            if (data.Success === false) { FCH.DespliegaError(data.Message); return; }
            $('#indicadorZonaActividad')[0].innerHTML = "";
            Indicador1.colRegionZonaActividad = new Backbone.Collection(data.datos);
            var bolFilter = Indicador1.colRegionZonaActividad.length > 0 ? true : false;
            if (bolFilter) {

                var table = JSON.parse(data.datos);
                var textHeader = data.hearderText.split(',');
                //var table = data.datos;
                var tabIndicador = "<table class='table table-striped' style='-webkit-box-shadow: 0px 0px 22px -2px rgba(0,0,0,0.75); -moz-box-shadow: 0px 0px 22px -2px rgba(0, 0, 0, 0.75);box-shadow: 0px 0px 22px -2px rgba(0, 0, 0, 0.75);border-collapse: collapse;-webkit-border-radius: 7px;-webkit-border-bottom-right-radius: 4px;-webkit-border-bottom-left-radius: 4px;-moz-border-radius: 7px;-moz-border-radius-bottomright: 4px;-moz-border-radius-bottomleft: 4px;border-radius: 7px;border-bottom-right-radius: 4px;border-bottom-left-radius: 4px;overflow: hidden;'>";
                tabIndicador += "<thead><tr style='color:#fff; background-color:#0F7CB9;border-bottom: 3pt solid #233246;'>" + data.header + "</tr></thead >";// <th>#</th> <th>Región</th> <th>Zona</th> <th>Total Actividades planeadas</th> <th>% Incumplimiento Promotor</th> <th>% Incumplimiento Tecnico docente</th> <th>% Incumplimiento Formador</th> </tr> </thead>";
                for (j = 0; j < table.length; j++) {
                    tabIndicador += "<tr style='text-align:center;'>";
                    for (i = 0; i < textHeader.length; i++) {
                        tabIndicador += "<td data-label='" + textHeader[i] + "' style='background-color:" + (i == 0 ? "" : (table[j][textHeader[i]] <= 49 ? "#C6EFC9" : (table[j][textHeader[i]] >= 50 && table[j][textHeader[i]] <= 79 ? "#FFEB9C" : "#FFC7CE"))) + "'>" + 
                            table[j][textHeader[i]] + (isNaN(table[j][textHeader[i]]) == true ? "" : "%")
                            + "</td>";
                    }
             
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
            "<span>Cargando...</span>" +
            "</div>";
        var url = contextPath + "Indicadores/CargarIndRegionZonaActividad?idPeriodo=" + $('#selPeriodosMul').val() + "&Region=" + $('#selRegionesMul').val() + "&Zonas=" + $('#selZonasMul').val() + "&TipoActividad=" + $('#selTipoActividadMul').val() + "&idUsuario=" + localStorage.idUser; // El url del controlador
        $.getJSON(url, function (data) {
            if (data.Success === false) { FCH.DespliegaError(data.Message); return; }
            $('#indicadorRegionZonaActividad')[0].innerHTML = "";
            Indicador1.colRegionZona = new Backbone.Collection(data.datos);
            var bolFilter = Indicador1.colRegionZona.length > 0 ? true : false;
            if (bolFilter) {
                var table = data.datos;
                var tabIndicador = "<table class='table table-striped' style='-webkit-box-shadow: 0px 0px 22px -2px rgba(0,0,0,0.75); -moz-box-shadow: 0px 0px 22px -2px rgba(0, 0, 0, 0.75);box-shadow: 0px 0px 22px -2px rgba(0, 0, 0, 0.75);border-collapse: collapse;-webkit-border-radius: 7px;-webkit-border-bottom-right-radius: 4px;-webkit-border-bottom-left-radius: 4px;-moz-border-radius: 7px;-moz-border-radius-bottomright: 4px;-moz-border-radius-bottomleft: 4px;border-radius: 7px;border-bottom-right-radius: 4px;border-bottom-left-radius: 4px;overflow: hidden;'>";
                tabIndicador += "<thead> <tr style='color:#fff; background-color:#0F7CB9;border-bottom: 3pt solid #233246;'> <th style='text-align:center;'>Región</th> <th style='text-align:center;'>Zona</th> <th style='text-align:center;'>Actividad</th> <th style='text-align:center;'>%Promotor</th> <th style='text-align:center;'>%Tecnico docente</th> <th style='text-align:center;'>%Formador</th> </tr> </thead>";
                for (j = 0; j < table.length; j++) {
                    tabIndicador += "<tr style='text-align:center;'>";
                    tabIndicador += "<td data-label='Región'>" + table[j]['region'] + "</td>";
                    tabIndicador += "<td data-label='Zona'>" + table[j]['zona'] + "</td>";
                    tabIndicador += "<td data-label='Actividad'>" + table[j]['actividad'] + "</td>";
                    tabIndicador += "<td data-label='%Promotor' style='background-color:" + (table[j]['promotorIncumplio'] <= 49 ? "#C6EFC9" : (table[j]['promotorIncumplio'] >= 50 && table[j]['promotorIncumplio'] <= 79 ? "#FFEB9C" : "#FFC7CE")) + "'>" + table[j]['promotorIncumplio'] + "%"  + "</td>";
                    tabIndicador += "<td data-label='%Tecnico docente' style='background-color:" + (table[j]['tecnicoIncumplio'] <= 49 ? "#C6EFC9" : (table[j]['tecnicoIncumplio'] >= 50 && table[j]['tecnicoIncumplio'] <= 79 ? "#FFEB9C" : "#FFC7CE")) + "'>" + table[j]['tecnicoIncumplio'] + "%" + "</td>";
                    tabIndicador += "<td data-label='%Formador' style='background-color:" + (table[j]['formadorIncumplio'] <= 49 ? "#C6EFC9" : (table[j]['formadorIncumplio'] >= 50 && table[j]['formadorIncumplio'] <= 79 ? "#FFEB9C" : "#FFC7CE")) + "'>" + table[j]['formadorIncumplio'] + "%" + "</td>";
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
            "<span>Cargando...</span>" +
            "</div>";
        var url1 = contextPath + "Indicadores/CargarIndRegionGrafico?idPeriodo=" + $('#selPeriodosMul').val() + "&Region=" + $('#selRegionesMul').val() + "&Zonas=" + $('#selZonasMul').val() + "&TipoActividad=" + $('#selTipoActividadMul').val() + "&idUsuario=" + localStorage.idUser;      
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
                                "caption": "%Incumplimiento actividades por región",
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
                                "yAxisName": "% Incumplimiento",
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
        var url1 = contextPath + "Indicadores/CargarIndIncumplimintoZonaGrafico?idPeriodo=" + $('#selPeriodosMul').val() + "&Region=" + $('#selRegionesMul').val() + "&Zonas=" + $('#selZonasMul').val() + "&TipoActividad=" + $('#selTipoActividadMul').val() + "&idUsuario=" + localStorage.idUser;
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
                                "caption": "%Incumplimiento actividades por zona",
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
                                "yAxisName": "% Incumplimiento",
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
        if (Indicador1.colPeriodos.length < 1) {
            var url = contextPath + "Periodos/CargarPeriodos?idEstatus=5";
            $.getJSON(url, function (data) {
                Indicador1.colPeriodos = data;
                Indicador1.CargaListaPeriodos();
            }).fail(function () {
                FCH.DespliegaErrorDialogo("No se pudieron cargar los peridos");
            });
        } else {
            Indicador1.CargaListaPeriodos();
        }
    },
    CargaListaPeriodos: function () {
        var select = $(Indicador1.activeForm + ' #selPeriodos').empty();

        $.each(Indicador1.colPeriodos, function (i, item) {
            select.append('<option value="' + item.id + '">' + item.nombre + '</option>');
        });

        $("#selPeriodos").select2({ allowClear: true, placeholder: "PERIODOS" });
        $("#selPeriodosMul").val('');
    },
    CargaRegiones: function () {
        if (Indicador1.colRegiones.length < 1) {
            var url = contextPath + "Regiones/CargarRegionesPorRol?idEstatus=5&idUsuario=" + localStorage.idUser;
            $.getJSON(url, function (data) {
                Indicador1.colRegiones = data;
                Indicador1.CargaListaRegiones();
            }).fail(function () {
                FCH.DespliegaErrorDialogo("No se pudieron cargar las regiones");
            });
        } else {
            Indicador1.CargaListaRegiones();
        }
    },
    CargaListaRegiones: function () {
        var select = $(Indicador1.activeForm + ' #selRegiones').empty();

        $.each(Indicador1.colRegiones, function (i, item) {
            select.append('<option value="' + item.id + '">' + item.nombre + '</option>');
        });

        $("#selRegiones").select2({ allowClear: true, placeholder: "REGIONES" });
        $("#selRegionesMul").val('');
    },
    CargaZonas: function () {
        if (Indicador1.colZonas.length < 1) {
            var url = contextPath + "Zonas/CargarZonasPorRol?idEstatus=5&idUsuario=" + localStorage.idUser;
            $.getJSON(url, function (data) {
                Indicador1.colZonas = data;
                Indicador1.CargaListaZonas();
            }).fail(function () {
                FCH.DespliegaErrorDialogo("No se pudieron cargar las zonas");
            });
        } else {
            Indicador1.CargaListaZonas();
        }
    },
    CargaListaZonas: function () {
        var select = $(Indicador1.activeForm + ' #selZonas').empty();

        $.each(Indicador1.colZonas, function (i, item) {
            select.append('<option value="' + item.id + '">' + item.nombre + '</option>');
        });

        $("#selZonas").select2({ allowClear: true, placeholder: "ZONAS" });
        $("#selZonasMul").val('');
    },
    CargaTiposActividad: function () {
        if (Indicador1.colTiposActividad.length < 1) {
            var url = contextPath + "TipoActividades/CargarTipoActividades?idEstatus=5";
            $.getJSON(url, function (data) {
                Indicador1.colTiposActividad = data;
                Indicador1.CargaListaTipoActividad();
            }).fail(function () {
                FCH.DespliegaErrorDialogo("No se pudieron cargar las actividades");
            });
        } else {
            Indicador1.CargaListaTipoActividad();
        }
    },
    CargaListaTipoActividad: function () {
        var select = $(Indicador1.activeForm + ' #selTipoActividad').empty();

        $.each(Indicador1.colTiposActividad, function (i, item) {
            select.append('<option value="' + item.id + '">' + item.nombre + '</option>');
        });

        $("#selTipoActividad").select2({ allowClear: true, placeholder: "ACTIVIDADES" });
        $("#selTipoActividadMul").val('');
    }
};

$(function () {
    Indicador1.Inicial();
});