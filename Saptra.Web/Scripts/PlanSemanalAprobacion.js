/* global FCH, contextPath, Backbone, bbGrid */

//js de Plan Semanal.
//David Jasso
//08/Octubre/2018

var PlanSemanal = {
    accClonar: false,
    accEscritura: false,
    accBorrar: false,
    activeForm: '',
    colPlanSemanal: {},
    colPlanDetalle: {},
    gridPlanDetalle: {},
    gridPlanSemanal: {},
    colTiposFigura: [],
    colPeriodos: [],
    colNombreFigura: [],
    comentarios: [],
    idPlan: 0,
    idUsuarioPlan: 0,
    colCertificados: {},
    colVerEducandos: {},
    Inicial: function () {
        $.ajaxSetup({ cache: false });
        this.CargaPeriodos();
        this.CargaTiposFigura();
        this.CargaNombresFigura();
        this.Eventos();
        this.CargaGrid();
    },
    Eventos: function () {
        var that = this;
        $('#btnFilter').click(that.CargaGrid);

        $(document).on('click', '.btnAprobar', function () {
            PlanSemanal.AprobarPlan($(this).data("idplan"));
        });
        $(document).on('change', '.idComentariosCZ', function () {
            PlanSemanal.comentarios = PlanSemanal.createJSON();
        });
        $(document).on('change', '.checkComents', function () {

            if ($('#checkComents_' + $(this).data("iddetalleplan")).prop('checked')) {
                $('#idComentariosCZ_' + $(this).data("iddetalleplan")).prop("disabled", false);
            }
            else {
                $('#idComentariosCZ_' + $(this).data("iddetalleplan")).prop("disabled", true);
            }
        });
        $(document).on('click', '.btnRptPlaneacion', function () {
            PlanSemanal.onExportarPlanSemanal($(this).data("idplan"), $(this).data("idusuarioplan"));
        });
        $(document).on('click', '.btnRptActividades', function () {
            PlanSemanal.onExportarActividadesRealizadas($(this).data("idplan"), $(this).data("idusuarioplan"));
        });

        $(document).on('click', '.btnRptPlaneacionExcel', function () {
            PlanSemanal.onExportarPlanSemanalExcel();
        });
        $(document).on('click', '.btnRptPlaneacionPDF', function () {
            PlanSemanal.onExportarPlanSemanalPDF();
        });
        $(document).on('click', '.btnRptActividadesExcel', function () {
            PlanSemanal.onExportarActividadesRealizadasExcel();
        });
        $(document).on('click', '.btnRptActividadesPDF', function () {
            PlanSemanal.onExportarActividadesRealizadasPDF();
        });
        $(document).on("change", '#selPeriodos', that.onCambiarPeriodos);
        $(document).on("change", '#selTipoFigura', that.onCambiarTiposFigura);
        $(document).on("change", '#selNombreFigura', that.onCambiarNombresFigura);

        $(document).on('click', '.btnGpsVehiculo', function () {
            var placa = "",
                fecha = "",
                horaInicio = "",
                horaFin = "";

            var url = contextPath + "PlanSemanalSeguimiento/ObtenerVehiculo?idDetallePlan=" + $(this).data("iddetalleplan");
            $.getJSON(url, function (respuesta) {
                if (respuesta.Success === true) {
                    placa = respuesta.placa;
                    fecha = respuesta.fecha;
                    horaInicio = respuesta.horaInicio;
                    horaFin = respuesta.horaFin;

                    $.post("http://gpsfollowme.com/service/1.php", { sep: ',', ter: '<br>', tipo: 0, placa: placa, fecha: fecha, hini: horaInicio, hfin: horaFin })
                        .done(function (data) {
                            if (data !== "null") {
                                var datos = data.split('<br>');
                                //alert("Data Loaded: " + datos);

                                var templateURL = contextPath + "Content/template/rpt_gps.html",
                                    rptTemplate = '',
                                    tabla_html,
                                    tablatmpHe = '',
                                    tablatmp = '',
                                    tableData,
                                    tableData2,
                                    tcompleta = '',
                                    tHeader = '';

                                $.get(templateURL, function (data) {
                                    rptTemplate = data;
                                    for (i = 0; i < datos.length; i++) {
                                        var detalle = datos[i].split(',');
                                        if (detalle == "") {

                                        } else {

                                            tcompleta += "<tr>";
                                            tcompleta += "<td>" + detalle[0] + "</td>";
                                            tcompleta += "<td>" + detalle[1] + ", " + detalle[2] + "</td>";
                                            tcompleta += "</tr>";
                                        }
                                    }

                                    tablatmp += rptTemplate.replace('vrDetalle', tcompleta);

                                    var tmpElemento = document.createElement('a');
                                    var data_type = 'data:application/vnd.ms-excel';
                                    tabla_html = tablatmp.replace(/ /g, '%20');
                                    tmpElemento.href = data_type + ', ' + tabla_html;
                                    //Asignamos el nombre a nuestro EXCEL
                                    tmpElemento.download = 'Reporte_Gps.xls';
                                    // Simulamos el click al elemento creado para descargarlo
                                    tmpElemento.click();
                                });

                            } else {
                                FCH.DespliegaError("No existe informacion en el sistema de GPS, relacionada con el vehículo asignado.");
                            }

                        });
                }
            });

          
        });

        $(document).on('click', '.btnImagen', function () {
            FCH.verPopupImagen($(this).data('src'), false);
        });

        $(document).on('click', '.btnCoordenadas', function () {
            PlanSemanal.Mapa($(this).data("iddetalleplan"));
        });

        $(document).on('click', '.btnCertificados', function () {
            PlanSemanal.Certificados($(this).data("iddetalleplan"));
        });

        $(document).on('click', '.hrefVerEducandos', function () {
            PlanSemanal.VerEducandos($(this).data("iddetalleplan"));
        });
    },
    VerEducandos: function (id) {
        FCH.CierraMensajes();
        var url = contextPath + "PlanSemanalConsulta/VerEducandos"; // El url del controlador      
        $.get(url, function (data) {
            $('#nuevo-Preregistros').html(data);
            $('#nuevo-Preregistros').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');
            PlanSemanal.CargaGridVerEducandos(id);
        });
    },
    CargaGridVerEducandos: function (id) {
        $('#bbGrid-VerPreregistros')[0].innerHTML = "";

        $('#cargandoInfo').show();
        $('#bbGrid-VerPreregistros')[0].innerHTML = "<span class='glyphicon glyphicon-refresh glyphicon-refresh-animate'></span> Cargando Información...";
        var url = contextPath + "PlanSemanalConsulta/CargarVerEducandos?idDetallePlan=" + id; // El url del controlador
        $.getJSON(url, function (data) {
            if (data.Success === false) { FCH.DespliegaError(data.Message); return; }
            $('#bbGrid-VerPreregistros')[0].innerHTML = "";
            PlanSemanal.colVerEducandos = new Backbone.Collection(data.datos);
            var bolFilter = PlanSemanal.colVerEducandos.length > 0 ? true : false;
            if (bolFilter) {
                PlanSemanal.gridPlanSemanal = new bbGrid.View({
                    container: $('#bbGrid-VerPreregistros'),
                    collection: PlanSemanal.colVerEducandos,
                    colModel: [
                        { title: 'Nombre', name: 'nombre', index: true, filter: true, filterType: 'input' },
                        { title: 'CURP', name: 'curp', index: true, filter: true, filterType: 'input' },
                        { title: 'Fecha nacimiento', name: 'fecha', index: true, filter: true, filterType: 'input' }]

                });
                $('#cargandoInfo').hide();
            } else {
                $('#bbGrid-VerPreregistros')[0].innerHTML = "<div class='alert alert-warning'>" +
                    "<button type='button' class='close' data-dismiss='alert'>x</button>" +
                    "No se encontro información" +
                    "</div>";
            }

            //getJSON fail
        }).fail(function (e) {
            FCH.DespliegaError("No se pudo cargar la informacion de los pre-registros");
        });
    },
    Certificados: function (id) {
        FCH.CierraMensajes();
        var url = contextPath + "PlanSemanalSeguimiento/Certificados"; // El url del controlador      
        $.get(url, function (data) {
            $('#nuevo-Certificados').html(data);
            $('#nuevo-Certificados').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');
            FCH.RedefinirValidaciones(); //para los formularios dinamicos
            PlanSemanal.activeForm = '#NuevoCertificadosForm';
            PlanSemanal.CargaGridCertificados(id);
        });
    },
    CargaGridCertificados: function (id) {
        $('#bbGrid-Certificados')[0].innerHTML = "";

        $('#cargandoInfo').show();
        $('#bbGrid-Certificados')[0].innerHTML = "<span class='glyphicon glyphicon-refresh glyphicon-refresh-animate'></span> Cargando Información...";
        var url = contextPath + "PlanSemanalSeguimiento/CargarCertificados?id=" + id; // El url del controlador
        $.getJSON(url, function (data) {
            if (data.Success === false) { FCH.DespliegaError(data.Message); return; }
            $('#bbGrid-Certificados')[0].innerHTML = "";
            PlanSemanal.colCertificados = new Backbone.Collection(data.datos);
            var bolFilter = PlanSemanal.colCertificados.length > 0 ? true : false;
            if (bolFilter) {
                PlanSemanal.gridPlanSemanal = new bbGrid.View({
                    container: $('#bbGrid-Certificados'),
                    collection: PlanSemanal.colCertificados,
                    colModel: [
                        { title: 'Acuse Certificado', name: 'folio', index: true, filter: true, filterType: 'input' },
                    { title: 'Acción', name: 'accion'}]

                });
                $('#cargandoInfo').hide();
            } else {
                $('#bbGrid-Certificados')[0].innerHTML = "<div class='alert alert-warning'>" +
                    "<button type='button' class='close' data-dismiss='alert'>x</button>" +
                    "No se encontro información" +
                    "</div>";
            }

            //getJSON fail
        }).fail(function (e) {
            FCH.DespliegaError("No se pudo cargar la informacion de los certificados");
        });
    },
    Mapa: function (id) {
        var url = contextPath + "PlanSemanalSeguimiento/Mapas?id=" + id; // El url del controlador      
        $.get(url, function (data) {
            $('#nuevo-Mapa').html(data);
            $('#nuevo-Mapa').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');
        });
    },
    onExportarPlanSemanal: function (idPlan, idUsuarioPlan) {
        PlanSemanal.idPlan = idPlan;
        PlanSemanal.idUsuarioPlan = idUsuarioPlan;
        var htmlMessage = "<h4>Exportar</h4>";
        htmlMessage += "<h5>Formato de reporte</h5>";
        htmlMessage += "<div class='text-center'><div class='btn-group btn-group-md'>";
        htmlMessage += "<button class='btn btn-success btnRptPlaneacionExcel'><i class='fa fa-file-excel-o'></i> Excel</button>";
        htmlMessage += "<button class='btn btn-danger btnRptPlaneacionPDF'><i class='fa fa-file-pdf-o'></i> PDF </button>";
        htmlMessage += "</div></div>";

        bootbox.alert({
            message: htmlMessage,
            size: 'small'
        });
    },
    onExportarPlanSemanalExcel: function () {

        var url = contextPath + "PlanSemanalSeguimiento/ExportPlanSemanal?idUsu=" + localStorage.idUser + "&Periodos=&TiposFigura=&NombresFigura=" + PlanSemanal.idUsuarioPlan + "&idPlan=" + PlanSemanal.idPlan; // El url del controlador

        window.open(url, '_blank');
    },
    onExportarPlanSemanalPDF: function () {

        var url = contextPath + "Pdf/ExportPlaneacionSemanalPDF?idUsu=" + localStorage.idUser + "&Periodos=&TiposFigura=&NombresFigura=" + PlanSemanal.idUsuarioPlan + "&idPlan=" + PlanSemanal.idPlan; // El url del controlador

        window.open(url, '_blank');
    },
    onExportarActividadesRealizadas: function (idPlan, idUsuarioPlan) {
        PlanSemanal.idPlan = idPlan;
        PlanSemanal.idUsuarioPlan = idUsuarioPlan;
        var htmlMessage = "<h4>Exportar</h4>";
        htmlMessage += "<h5>Formato de reporte</h5>";
        htmlMessage += "<div class='text-center'><div class='btn-group btn-group-md'>";
        htmlMessage += "<button class='btn btn-success btnRptActividadesExcel'><i class='fa fa-file-excel-o'></i> Excel</button>";
        htmlMessage += "<button class='btn btn-danger btnRptActividadesPDF'><i class='fa fa-file-pdf-o'></i> PDF </button>";
        htmlMessage += "</div></div>";

        bootbox.alert({
            message: htmlMessage,
            size: 'small'
        });
    },
    onExportarActividadesRealizadasExcel: function () {

        var url = contextPath + "PlanSemanalSeguimiento/ExportActividadesRealizadas?idUsu=" + localStorage.idUser + "&Periodos=&TiposFigura=&NombresFigura=" + PlanSemanal.idUsuarioPlan + "&idPlan=" + PlanSemanal.idPlan; // El url del controlador

        window.open(url, '_blank');
    },
    onExportarActividadesRealizadasPDF: function () {

        var url = contextPath + "Pdf/ExportActividadesRealizadasPDF?idUsu=" + localStorage.idUser + "&Periodos=&TiposFigura=&NombresFigura=" + PlanSemanal.idUsuarioPlan + "&idPlan=" + PlanSemanal.idPlan; // El url del controlador

        window.open(url, '_blank');
    },
    createJSON: function () {
        jsonObj = [];
        $.each($('.idComentariosCZ'), function (i, item) {

            if ($('#checkComents_' + $(item).data("iddetalleplan")).prop('checked')) {
                var id = $(item).data("iddetalleplan");
                var coment = $(item).val();

                elem = { id: id, comentario: coment };

                jsonObj.push(elem);
            }
        });

        return JSON.stringify(jsonObj);
    },
    AprobarPlan: function (idPlan) {
        var objetoJson = PlanSemanal.comentarios;
        var btn = $('#btnAprobar_' + idPlan),
            btnName = $('#btnAprobar_' + idPlan).text();
        FCH.botonMensaje(true, btn, btnName);
        $.ajax({
            url: contextPath + "PlanSemanalSeguimiento/AprobarPlan?idPlan=" + idPlan + "&idUsuario=" + localStorage.idUser,
            type: 'post',
            data: objetoJson,
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                if (data.Success === true) {
                    FCH.DespliegaNotificacion('success', 'Plan ', data.Message, 'glyphicon-ok', 3000);
                    FCH.botonMensaje(false, btn, "<i class='fa fa-check-circle'></i> Aprobado");
                    $("#btnAprobar_" + idPlan).prop("disabled", true);
                    FCH.cargarNotificaciones();
                    PlanSemanal.CargaGrid();
                } else {
                    FCH.DespliegaError(data.Message);
                    FCH.botonMensaje(false, btn, "<i class='fa fa-check-circle'></i> Aprobar");
                }
            }
        });

    },
    onCambiarPeriodos: function (ev) {
        var linea;
        var values = ev.val;
        for (var i = 0; i < values.length; i++) {
            if (typeof linea !== 'undefined') {
                linea += ",";
                linea += values[i];
            }
            else {
                linea = values[i];
            }
        }
        $('#selecPeriodos').val(linea);
    },
    onCambiarTiposFigura: function (ev) {
        var linea;
        var values = ev.val;
        for (var i = 0; i < values.length; i++) {
            if (typeof linea !== 'undefined') {
                linea += ",";
                linea += values[i];
            }
            else {
                linea = values[i];
            }
        }
        $('#selecTipoFigura').val(linea);
    },
    onCambiarNombresFigura: function (ev) {
        var linea;
        var values = ev.val;
        for (var i = 0; i < values.length; i++) {
            if (typeof linea !== 'undefined') {
                linea += ",";
                linea += values[i];
            }
            else {
                linea = values[i];
            }
        }
        $('#selecNombreFigura').val(linea);
    },
    CargaGrid: function () {
        $('#bbGrid-clear')[0].innerHTML = "";

        $('#cargandoInfo').show();
        $('#bbGrid-clear')[0].innerHTML = "<span class='glyphicon glyphicon-refresh glyphicon-refresh-animate'></span> Cargando Información...";
        var url = contextPath + "PlanSemanalSeguimiento/CargarPlanesAprobacion?idUsu=" + localStorage.idUser + "&Periodos=" + $('#selecPeriodos').val() + "&TiposFigura=" + $('#selecTipoFigura').val() + "&NombresFigura=" + $('#selecNombreFigura').val(); // El url del controlador
        $.getJSON(url, function (data) {
            if (data.Success === false) { FCH.DespliegaError(data.Message); return; }
            $('#bbGrid-clear')[0].innerHTML = "";
            PlanSemanal.colPlanSemanal = new Backbone.Collection(data.datos);
            var bolFilter = PlanSemanal.colPlanSemanal.length > 0 ? true : false;
            if (bolFilter) {
                PlanSemanal.gridPlanSemanal = new bbGrid.View({
                    container: $('#bbGrid-clear'),
                    rows: 200,
                    rowList: [15, 50, 200, 1000],
                    enableSearch: true,
                    subgrid: true,
                    collection: PlanSemanal.colPlanSemanal,
                    colModel: [//{ title: 'Id', name: 'id', width: '8%', sorttype: 'number', filter: true, filterType: 'input' },
                        { title: 'Tipo Figura', name: 'tipoFigura', index: true },
                        { title: 'Usuario', name: 'usuario', index: true },
                        { title: 'Descripción', name: 'descripcionPlan', index: true },
                        { title: 'Periodo', name: 'periodo', index: true },
                        { title: 'Acciones', name: 'acciones', textalign: true }],
                    onRowExpanded: function ($el, rowid) {
                        PlanSemanal.CargaGridDetallesPackingList(rowid, $el);
                    }

                });
                $('#cargandoInfo').hide();
            } else {
                $('#bbGrid-clear')[0].innerHTML = "<div class='alert alert-warning'>" +
                    "<button type='button' class='close' data-dismiss='alert'>x</button>" +
                    "No se encontro información" +
                    "</div>";
            }

            //getJSON fail
        }).fail(function (e) {
            FCH.DespliegaError("No se pudo cargar la informacion de los planes");
        });
    },
    CargaGridDetallesPackingList: function (id, $container) {
        var url = contextPath + "PlanSemanalSeguimiento/CargarDetallePlanAprobacion?idPlanSemanal=" + id; // El url del controlador
        $container.innerHTML = '<label > Cargando...</label>';

        PlanSemanal.detalleContainer = $container;
        $.getJSON(url, function (data) {
            if (data.Success === false) { FCH.DespliegaError(data.Message); return; }
            PlanSemanal.colPlanDetalle = new Backbone.Collection(data.datos);
            $container.innerHTML = '';
            PlanSemanal.gridPlanDetalle = new bbGrid.View({
                container: $container,
                collection: PlanSemanal.colPlanDetalle,
                colModel: [//{ title: 'id', name: 'id', index: true },
                    { title: 'Actividad', name: 'actividad', index: true },
                    { title: 'Descripción', name: 'descripcion', index: true },
                    { title: 'Lugar', name: 'lugar', index: true },
                    { title: 'Fecha', name: 'fecha', index: true },
                    { title: 'Hora Inicio', name: 'hora', index: true },
                    { title: 'Hora Fin', name: 'horaFin', index: true },
                    { title: 'Estatus', name: 'checkin', index: true, textalign: true },
                    { title: 'Incidencias', name: 'incidencias' },
                    { title: 'Reporte GPS', name: 'gps', textalign: true },
                    { title: 'Comentarios CZ', name: 'comentariosRechazo', index: true }],
                onRowClick: function () {
                    PlanSemanal.evento = 'detalle';
                    return false;
                }
            });

            if (PlanSemanal.colPlanDetalle.length < 1) {
                PlanSemanal.gridPlanDetalle.toggleLoading(false);
            }
        });
    },
    CargaPeriodos: function () {
        if (PlanSemanal.colPeriodos.length < 1) {
            var url = contextPath + "Periodos/CargarPeriodos?idEstatus=5";
            $.getJSON(url, function (data) {
                PlanSemanal.colPeriodos = data;
                PlanSemanal.CargaListaPeriodos();
            }).fail(function () {
                FCH.DespliegaErrorDialogo("No se pudieron cargar los peridos");
            });
        } else {
            PlanSemanal.CargaListaPeriodos();
        }
    },
    CargaListaPeriodos: function () {
        var select = $(PlanSemanal.activeForm + ' #selPeriodos').empty();

        select.append('<option value="0">SELECCIONAR PERIODO</option>');
        $.each(PlanSemanal.colPeriodos, function (i, item) {
            select.append('<option value="' + item.id + '">' + item.nombre + '</option>');
        });

        $(PlanSemanal.activeForm + " #selPeriodos").select2({ allowClear: true, placeholder: "SELECCIONAR PERIODOS" });
    },
    CargaTiposFigura: function () {
        if (PlanSemanal.colTiposFigura.length < 1) {
            var url = contextPath + "TiposFigura/CargarTiposFigura?idEstatus=5";
            $.getJSON(url, function (data) {
                PlanSemanal.colTiposFigura = data;
                PlanSemanal.CargaListaTipoFigura();
            }).fail(function () {
                FCH.DespliegaErrorDialogo("No se pudieron cargar los tipo de figura");
            });
        } else {
            PlanSemanal.CargaListaTipoFigura();
        }
    },
    CargaListaTipoFigura: function () {
        var select = $(PlanSemanal.activeForm + ' #selTipoFigura').empty();

        //select.append('<option value="0">SELECCIONAR TIPO FIGURA</option>');
        $.each(PlanSemanal.colTiposFigura, function (i, item) {
            select.append('<option value="' + item.id + '">' + item.nombre + '</option>');
        });

        $(PlanSemanal.activeForm + " #selTipoFigura").select2({ allowClear: true, placeholder: "SELECCIONAR TIPO FIGURA" });
    },
    CargaNombresFigura: function () {
        if (PlanSemanal.colNombreFigura.length < 1) {
            var url = contextPath + "Usuario/CargarNombresFiguraPorCZ?idEstatus=5&idUsuarioCZ=" + localStorage.idUser;
            $.getJSON(url, function (data) {
                PlanSemanal.colNombreFigura = data;
                PlanSemanal.CargaListaNombreFigura();
            }).fail(function () {
                FCH.DespliegaErrorDialogo("No se pudieron cargar los usuarios");
            });
        } else {
            PlanSemanal.CargaListaNombreFigura();
        }
    },
    CargaListaNombreFigura: function () {
        var select = $(PlanSemanal.activeForm + ' #selNombreFigura').empty();

        //select.append('<option value="0">SELECCIONAR USUARIO</option>');
        $.each(PlanSemanal.colNombreFigura, function (i, item) {
            select.append('<option value="' + item.id + '">' + item.nombre + '</option>');
        });

        $(PlanSemanal.activeForm + " #selNombreFigura").select2({ allowClear: true, placeholder: "SELECCIONAR USUARIOS" });
    }
 
};

$(function () {
    PlanSemanal.Inicial();
});