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
    detalleContainer: {},
    idPlan: 0,
    Inicial: function () {
        $.ajaxSetup({ cache: false });
        this.CargaPeriodos();
        this.CargaTiposFigura();
        this.CargaNombresFigura();
        this.Eventos();
        this.CargaGrid();
        if (localStorage.IdPlan != undefined) {
            localStorage.removeItem("IdPlan");

            PlanSemanal.CargaGrid();
        }
    },
    Eventos: function () {
        var that = this;
        $('#btnFilter').click(that.CargaGrid);

        $(document).on('click', '.btnValidar', function () {
            PlanSemanal.ValidarPlan($(this).data("idplan"));
        });
        $(document).on('click', '.btnFeedback', function () {
            PlanSemanal.FeedbackPlan($(this).data("idplan"));
        });

        $(document).on('change', '.checkComents', function () { 

            if ($('#checkComents_' + $(this).data("iddetalleplan")).prop('checked')) {
                $('#idComentariosCZ_' + $(this).data("iddetalleplan")).prop("disabled", false);
            }
            else {
                $('#idComentariosCZ_' + $(this).data("iddetalleplan")).prop("disabled", true);
            }
        });
        $(document).on('change', '.idComentariosCZ', function () {
            PlanSemanal.comentarios = PlanSemanal.createJSON();
        });

        $(document).on("change", '#selPeriodos', that.onCambiarPeriodos);
        $(document).on("change", '#selTipoFigura', that.onCambiarTiposFigura);
        $(document).on("change", '#selNombreFigura', that.onCambiarNombresFigura);

      
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
        PlanSemanal.CargaNombresFigura();
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
    ValidarPlan: function (idPlan) {
        var btn = $('#btnValidar_' + idPlan),
            btnName = $('#btnValidar_' + idPlan).text();
        FCH.botonMensaje(true, btn, btnName);

        $.post(contextPath + "PlanSemanalSeguimiento/ValidarPlan?idPlan=" + idPlan + "&idUsuario=" + localStorage.idUser,
            function (data) {
                if (data.Success === true) {
                    FCH.DespliegaNotificacion('success', 'Plan ', data.Message, 'glyphicon-ok', 3000);
                    FCH.botonMensaje(false, btn, "<i class='fa fa-check-circle'></i> Validado");
                    $("#btnValidar_" + idPlan).prop("disabled", true);
                    $("#btnFeedback_" + idPlan).prop("disabled", true);
                    FCH.cargarNotificaciones();
                } else {
                    FCH.DespliegaError(data.Message);
                    FCH.botonMensaje(false, btn, "<i class='fa fa-check-circle'></i> Validar");
                }
            }).fail(function () {
                FCH.DespliegaError("Error al guardar la informacion");
            }).always(function () { });

    },
    FeedbackPlan: function (idPlan) {
        var objetoJson = PlanSemanal.comentarios;
        if (objetoJson != 0) {
            var btn = $('#btnFeedback_' + idPlan),
                btnName = $('#btnFeedback_' + idPlan).text();
            FCH.botonMensaje(true, btn, btnName);
            $.ajax({
                url: contextPath + "PlanSemanalSeguimiento/FeedbackPlan?idPlan=" + idPlan + "&idUsuario=" + localStorage.idUser,
                type: 'post',
                data: objetoJson,
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    if (data.Success === true) {
                        FCH.DespliegaNotificacion('success', 'Plan ', data.Message, 'glyphicon-ok', 3000);
                        FCH.botonMensaje(false, btn, "<i class='fa fa-check-circle'></i> Observaciones");
                        $("#btnFeedback_" + idPlan).prop("disabled", true);
                        $("#btnValidar_" + idPlan).prop("disabled", true);
                        if (PlanSemanal.idPlan != 0) {
                            PlanSemanal.CargaGridDetallesPackingList(PlanSemanal.idPlan, PlanSemanal.detalleContainer);
                        }
                        FCH.cargarNotificaciones();
                    } else {
                        FCH.DespliegaError(data.Message);
                        FCH.botonMensaje(false, btn, "<i class='fa fa-check-circle'></i> Observaciones");
                    }
                }
            });
        }
        else {
            FCH.DespliegaError("Es necesario agregar comentarios para enviar a revisión");
        }
    },
    CargaGrid: function () {
        $('#bbGrid-clear')[0].innerHTML = "";

        $('#cargandoInfo').show();
        $('#bbGrid-clear')[0].innerHTML = "<span class='glyphicon glyphicon-refresh glyphicon-refresh-animate'></span> Cargando Información...";
        var url = contextPath + "PlanSemanalSeguimiento/CargarPlanes?idUsu=" + localStorage.idUser + "&Periodos=" + $('#selecPeriodos').val() + "&TiposFigura=" + $('#selecTipoFigura').val() + "&NombresFigura=" + $('#selecNombreFigura').val(); // El url del controlador
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
                        PlanSemanal.idPlan = rowid;
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
        var url = contextPath + "PlanSemanalSeguimiento/CargarDetallePlan?idPlanSemanal=" + id; // El url del controlador
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
                            { title: 'Actividad', name: 'actividad',  index: true },
                            { title: 'Descripción', name: 'descripcion', index: true },
                            { title: 'Lugar', name: 'lugar', index: true },
                            { title: 'Fecha', name: 'fecha',  index: true },
                            { title: 'Hora Inicio', name: 'hora', index: true },
                            { title: 'Hora Fin', name: 'horaFin', index: true },
                            { title: 'Registro', name: 'checkin',  index: true },
                            { title: 'Comentarios CZ', name: 'comentariosNoValidacion',  index: true }],
                onRowClick: function () {
                    PlanSemanal.evento = 'detalle';
                    return false;
                }
            });
            PlanSemanal.PintarFilasConHidden();
            if (PlanSemanal.colPlanDetalle.length < 1) {
                PlanSemanal.gridPlanDetalle.toggleLoading(false);
            }
        });
    },
    PintarFilasConHidden: function () {
        $('.hdnPintaFila').parent().parent().addClass('fobProtoActualizado');
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
        var idTipoFigura = $('#selTipoFigura').val() != null ? $('#selTipoFigura').val() : 0;
        //if (PlanSemanal.colNombreFigura.length < 1) {
        var url = contextPath + "Usuario/CargarNombresFiguraPorCZ?idEstatus=5&idUsuarioCZ=" + localStorage.idUser + "&TiposFiguraIds=" + idTipoFigura;
        $.getJSON(url, function (data) {
            PlanSemanal.colNombreFigura = data;
            PlanSemanal.CargaListaNombreFigura();
        }).fail(function () {
            FCH.DespliegaErrorDialogo("No se pudieron cargar los usuarios");
        });
        //} else {
        //    PlanSemanal.CargaListaNombreFigura();
        //}
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