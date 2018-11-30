/* global FCH, contextPath, Backbone, bbGrid */

//js de Plan Semanal.
//David Jasso
//08/Octubre/2018

var Incidencias = {
    accClonar: false,
    accEscritura: false,
    accBorrar: false,
    activeForm: '',
    colPlanSemanal: {},
    colPlanDetalle: {},
    gridPlanDetalle: {},
    gridPlanSemanal: {},
    colTiposFigura: [],
    colTiposActividad: [],
    colPeriodos: [],
    colNombreFigura: [],
    comentarios: [],
    Inicial: function () {
        $.ajaxSetup({ cache: false });
        this.CargaPeriodos();
        this.CargaTiposFigura();
        this.CargaNombresFigura();
        this.CargaTiposActividad();
        this.Eventos();
    },
    Eventos: function () {
        var that = this;
        $('#btnFilter').click(that.CargaGrid);
        $(document).on('click', '.btnExportar', function () {
            Incidencias.onExportarIncidencias();
        });
        $(document).on('click', '.btnExportarExcel', function () {
            Incidencias.onExportarIncidenciasExcel();
        });
        $(document).on('click', '.btnExportarPDF', function () {
            Incidencias.onExportarIncidenciasPDF();
        });
        $(document).on("change", '#selPeriodos', that.onCambiarPeriodos);
        $(document).on("change", '#selTipoFigura', that.onCambiarTiposFigura);
        $(document).on("change", '#selNombreFigura', that.onCambiarNombresFigura);
        $(document).on("change", '#selTipoActividad', that.onCambiarTipoActividad);
     
    },
    onExportarIncidencias: function () {
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
    onExportarIncidenciasExcel: function () {
        var url = contextPath + "Incidencias/ExportIncidenciasExcel?idUsu=" + localStorage.idUser + "&Periodos=" + $('#selecPeriodos').val() + "&TiposFigura=" + $('#selecTipoFigura').val() + "&NombresFigura=" + $('#selecNombreFigura').val() + "&TipoActividades=" + $('#selecTipoActividad').val(); // El url del controlador

        window.open(url, '_blank');
    },
    onExportarIncidenciasPDF: function () {
        var url = contextPath + "Pdf/ExportIncidenciasPDF?idUsu=" + localStorage.idUser + "&Periodos=" + $('#selecPeriodos').val() + "&TiposFigura=" + $('#selecTipoFigura').val() + "&NombresFigura=" + $('#selecNombreFigura').val() + "&TipoActividades=" + $('#selecTipoActividad').val(); // El url del controlador

        window.open(url, '_blank');
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
    onCambiarTipoActividad: function (ev) {
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
        $('#selecTipoActividad').val(linea);
    },
    CargaGrid: function () {
        $('#bbGrid-clear')[0].innerHTML = "";

        $('#cargandoInfo').show();
        $('#bbGrid-clear')[0].innerHTML = "<span class='glyphicon glyphicon-refresh glyphicon-refresh-animate'></span> Cargando Información...";
        var url = contextPath + "Incidencias/CargarIncidencias?idUsu=" + localStorage.idUser + "&Periodos=" + $('#selecPeriodos').val() + "&TiposFigura=" + $('#selecTipoFigura').val() + "&NombresFigura=" + $('#selecNombreFigura').val() + "&TipoActividades=" + $('#selecTipoActividad').val(); // El url del controlador
        $.getJSON(url, function (data) {
            if (data.Success === false) { FCH.DespliegaError(data.Message); return; }
            $('#bbGrid-clear')[0].innerHTML = "";
            Incidencias.colPlanSemanal = new Backbone.Collection(data.datos);
            var bolFilter = Incidencias.colPlanSemanal.length > 0 ? true : false;
            if (bolFilter) {
                Incidencias.gridPlanSemanal = new bbGrid.View({
                    container: $('#bbGrid-clear'),
                    rows: 200,
                    rowList: [15, 50, 200, 1000],
                    enableSearch: true,
                    collection: Incidencias.colPlanSemanal,
                    colModel: [//{ title: 'Id', name: 'id', width: '8%', sorttype: 'number', filter: true, filterType: 'input' },
                        { title: 'Periodo', name: 'periodo', index: true },
                        { title: 'Nombre Empleado', name: 'usuario', index: true },
                        { title: 'Actividad', name: 'actividad', index: true },
                        { title: 'Descripción', name: 'descripcion', index: true },
                        { title: 'Fecha', name: 'fecha' },
                        { title: 'Comentarios CZ', name: 'comentarios' },
                        { title: 'Número Empleado', name: 'numeroEmp' }]

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
            FCH.DespliegaError("No se pudo cargar la informacion de las actividades");
        });
    },
    CargaPeriodos: function () {
        if (Incidencias.colPeriodos.length < 1) {
            var url = contextPath + "Periodos/CargarPeriodos?idEstatus=5";
            $.getJSON(url, function (data) {
                Incidencias.colPeriodos = data;
                Incidencias.CargaListaPeriodos();
            }).fail(function () {
                FCH.DespliegaErrorDialogo("No se pudieron cargar los peridos");
            });
        } else {
            Incidencias.CargaListaPeriodos();
        }
    },
    CargaListaPeriodos: function () {
        var select = $(Incidencias.activeForm + ' #selPeriodos').empty();

        $.each(Incidencias.colPeriodos, function (i, item) {
            select.append('<option value="' + item.id + '">' + item.nombre + '</option>');
        });

        $(Incidencias.activeForm + " #selPeriodos").select2({ allowClear: true, placeholder: "SELECCIONAR PERIODOS" });
    },
    CargaTiposFigura: function () {
        if (Incidencias.colTiposFigura.length < 1) {
            var url = contextPath + "TiposFigura/CargarTiposFigura?idEstatus=5";
            $.getJSON(url, function (data) {
                Incidencias.colTiposFigura = data;
                Incidencias.CargaListaTipoFigura();
            }).fail(function () {
                FCH.DespliegaErrorDialogo("No se pudieron cargar los tipo de figura");
            });
        } else {
            Incidencias.CargaListaTipoFigura();
        }
    },
    CargaListaTipoFigura: function () {
        var select = $(Incidencias.activeForm + ' #selTipoFigura').empty();

        //select.append('<option value="0">SELECCIONAR TIPO FIGURA</option>');
        $.each(Incidencias.colTiposFigura, function (i, item) {
            select.append('<option value="' + item.id + '">' + item.nombre + '</option>');
        });

        $(Incidencias.activeForm + " #selTipoFigura").select2({ allowClear: true, placeholder: "SELECCIONAR TIPO FIGURA" });
    },
    CargaTiposActividad: function () {
        if (Incidencias.colTiposActividad.length < 1) {
            var url = contextPath + "TipoActividades/CargarTipoActividadesSinCheckIn?idEstatus=5";
            $.getJSON(url, function (data) {
                Incidencias.colTiposActividad = data;
                Incidencias.CargaListaTipoActividad();
            }).fail(function () {
                FCH.DespliegaErrorDialogo("No se pudieron cargar las actividades");
            });
        } else {
            Incidencias.CargaListaTipoActividad();
        }
    },
    CargaListaTipoActividad: function () {
        var select = $(Incidencias.activeForm + ' #selTipoActividad').empty();

        //select.append('<option value="0">SELECCIONAR TIPO FIGURA</option>');
        $.each(Incidencias.colTiposActividad, function (i, item) {
            select.append('<option value="' + item.id + '">' + item.nombre + '</option>');
        });

        $(Incidencias.activeForm + " #selTipoActividad").select2({ allowClear: true, placeholder: "SELECCIONAR ACTIVIDAD" });
    },
    CargaNombresFigura: function () {
        if (Incidencias.colNombreFigura.length < 1) {
            var url = contextPath + "Usuario/CargarNombresFiguraPorCZ?idEstatus=5&idUsuarioCZ=" + localStorage.idUser;
            $.getJSON(url, function (data) {
                Incidencias.colNombreFigura = data;
                Incidencias.CargaListaNombreFigura();
            }).fail(function () {
                FCH.DespliegaErrorDialogo("No se pudieron cargar los usuarios");
            });
        } else {
            Incidencias.CargaListaNombreFigura();
        }
    },
    CargaListaNombreFigura: function () {
        var select = $(Incidencias.activeForm + ' #selNombreFigura').empty();

        //select.append('<option value="0">SELECCIONAR USUARIO</option>');
        $.each(Incidencias.colNombreFigura, function (i, item) {
            select.append('<option value="' + item.id + '">' + item.nombre + '</option>');
        });

        $(Incidencias.activeForm + " #selNombreFigura").select2({ allowClear: true, placeholder: "SELECCIONAR USUARIOS" });
    }
 
};

$(function () {
    Incidencias.Inicial();
});