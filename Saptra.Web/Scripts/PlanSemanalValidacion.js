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
    Inicial: function () {
        $.ajaxSetup({ cache: false });
        this.CargaPeriodos();
        this.CargaTiposFigura();
        this.CargaNombresFigura();
        this.Eventos();
    },
    Eventos: function () {
        var that = this;
        $('#btnFilter').click(that.CargaGrid);
    },
    CargaGrid: function () {
        $('#bbGrid-clear')[0].innerHTML = "";

        $('#cargandoInfo').show();
        $('#bbGrid-clear')[0].innerHTML = "<span class='glyphicon glyphicon-refresh glyphicon-refresh-animate'></span> Cargando Información...";
        var url = contextPath + "PlanSemanalSeguimiento/CargarPlanes?idUsu=" + localStorage.idUser + "&idPeriodo=" + $('#selPeriodos').val(); // El url del controlador
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
            FCH.DespliegaError(jsglb[system_lang].Proyecto_could_not_load_info_usuarios);
        });
    },
    CargaGridDetallesPackingList: function (id, $container) {
        var url = contextPath + "PlanSemanalSeguimiento/CargarDetallePlan?idPlanSemanal=" + id; // El url del controlador
        $container.innerHTML = '<label > Loading...</label>';

        PlanSemanal.detalleContainer = $container;
        $.getJSON(url, function (data) {
            if (data.Success === false) { FCH.DespliegaError(data.Message); return; }
            PlanSemanal.colPlanDetalle = new Backbone.Collection(data.datos);
            $container.innerHTML = '';
            PlanSemanal.gridPlanDetalle = new bbGrid.View({
                container: $container,
                collection: PlanSemanal.colPlanDetalle,
                colModel: [//{ title: 'id', name: 'id', index: true },
                            { title: 'Actividad', name: 'actividad',  index: true },
                            { title: 'Descipción', name: 'descripcion', index: true },
                            { title: 'Fecha', name: 'fecha',  index: true },
                            { title: 'Hora', name: 'hora',  index: true },
                            { title: 'Check In', name: 'checkin',  index: true },
                            { title: 'Comentarios CZ', name: 'comentariosNoValidacion',  index: true }],
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
            var url = contextPath + "Usuario/CargarNombresFigura?idEstatus=5";
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