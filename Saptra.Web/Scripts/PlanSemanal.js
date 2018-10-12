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
    colTipoActividad: [],
    colPeriodos: [],
    colPeriodoActual: [],
    Inicial: function () {
        $.ajaxSetup({ cache: false });
        this.CargaPeriodos();
        this.Eventos();
        //this.ValidaPermisos();
    },
    Eventos: function () {
        var that = this;

        $('#btnFilter').click(that.CargaGrid);

        $('.btnNuevo').click(that.Nuevo);

        $(document).on("click", '.btnNuevo', that.NuevoPlan);

        $(document).on("click", '.btn-GuardaNuevo', that.onGuardar);

        $(document).on('click', '.accrowProdDetalle', function () {
            var idPlan = $(this).parent().parent().attr("data-modelId");
            var idEstatus = $('#btnEnvioValidacion_' + idPlan).data("idestatus");
            if (idEstatus == 10) {
                FCH.DespliegaWarning('No es posible agregar nuevas actividades, el plan se encuentra con estatus "<strong>Enviado</strong>"');
            }
            else {
                PlanSemanal.NuevoDetalle(idPlan);
            }
        });

        $(document).on("click", '.btn-GuardaDetalleNuevo', that.onGuardarDetalle);
        
        $(document).on('click', '.btnEnvioValidacion', function () {
            PlanSemanal.EnviarPlan($(this).data("idplan"));
        });

        $(document).on('click', '.btnEditarDetalle', function () {
            PlanSemanal.EditarDetalle($(this).data("iddetalleplan"));
        });
        $(document).on("click", '.btn-ActualizaDetalle', that.onActualizarDetalle);

        $(document).on('change', '#selTipoActividad', function () {
            if ($('#selTipoActividad').val() == "6") {
                $('#divCheckIn').show();
            }
        });
    },

    EnviarPlan: function (idPlan) {
        var btn = $('#btnEnvioValidacion_' + idPlan),
            btnName = $('#btnEnvioValidacion_' + idPlan).text();
        FCH.botonMensaje(true, btn, btnName);

        $.post(contextPath + "PlanSemanal/EnviarPlan?idPlan=" + idPlan,
            function (data) {
                if (data.Success === true) {
                    FCH.DespliegaNotificacion('success', 'Plan ', data.Message, 'glyphicon-ok', 3000);
                    FCH.botonMensaje(false, btn, "<i class='fa fa-paper-plane'></i> Enviado" );
                    $('#btnEnvioValidacion_' + idPlan).removeClass("btn-primary");
                    $('#btnEnvioValidacion_' + idPlan).addClass("btn-success");
                    $("#btnEnvioValidacion_" + idPlan).prop("disabled", true);
                } else {
                    FCH.DespliegaErrorDialogo(data.Message);
                }
            }).fail(function () {
                FCH.DespliegaErrorDialogo("Error al guardar la informacion");
            }).always(function () {  });
    },
    NuevoPlan: function () {
        FCH.CierraMensajes();
        var url = contextPath + "PlanSemanal/Nuevo"; // El url del controlador      
        $.get(url, function (data) {
            $('#nuevo-PlanSemanal').html(data);
            $('#nuevo-PlanSemanal').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');
            FCH.RedefinirValidaciones(); //para los formularios dinamicos
            PlanSemanal.activeForm = '#NuevoPlanForm';
            PlanSemanal.CargaPeriodoActual();
        });
    },
    NuevoDetalle: function (id) {
        FCH.CierraMensajes();
        var url = contextPath + "PlanSemanal/NuevoDetalle/" + id; // El url del controlador      
        $.get(url, function (data) {
            $('#nuevo-PlanDetalle').html(data);
            $('#nuevo-PlanDetalle').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');
            FCH.RedefinirValidaciones(); //para los formularios dinamicos
            PlanSemanal.activeForm = '#NuevoDetallePlanForm';
            PlanSemanal.CargaTipoActividad();
            PlanSemanal.IniciaDateControls();
            $(PlanSemanal.activeForm + ' #HoraActividad').select2();
        });
    },
    EditarDetalle: function (id) {
        FCH.CierraMensajes();
        var url = contextPath + "PlanSemanal/ActualizaDetallePlan/" + id; // El url del controlador
        $.get(url, function (data) {
            $('#actualiza-PlanDetalle').html(data);
            $('#actualiza-PlanDetalle').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');
            FCH.RedefinirValidaciones(); //para los formularios dinamicos
            PlanSemanal.activeForm = '#ActualizaDetallePlanForm';
            PlanSemanal.IniciaDateControls();
            $(PlanSemanal.activeForm + ' #HoraActividad').select2();
        });
    },
    onGuardar: function (e) {
        var btn = this,
            btnName = btn.innerText;

        FCH.botonMensaje(true, btn, btnName);
        if ($("form").valid()) {
            $('#UsuarioCreacionId').val(localStorage.idUser);
            $('#PeriodoId').val($('#selPeriodo').val());
            //Se hace el post para guardar la informacion
            $.post(contextPath + "PlanSemanal/Nuevo",
                $("#NuevoPlanForm *").serialize(),
                function (data) {
                    if (data.Success === true) {
                        PlanSemanal.colPlanSemanal.add(PlanSemanal.serializaPlan(data.id, '#NuevoPlanForm'));
                        $('#nuevo-PlanSemanal').modal('hide');
                        FCH.DespliegaNotificacion('success', 'Plan ', data.Message, 'glyphicon-ok', 3000);
                    } else {
                        FCH.DespliegaErrorDialogo(data.Message);
                    }
                }).fail(function () {
                    FCH.DespliegaErrorDialogo("Error al guardar la información");

                }).always(function () { FCH.botonMensaje(false, btn, btnName); });

        } else {
            FCH.botonMensaje(false, btn, btnName);
        }
    },
    onGuardarDetalle: function (e) {
        var btn = this,
            btnName = btn.innerText;

        FCH.botonMensaje(true, btn, btnName);
        if ($("form").valid()) {
            $('#NuevoDetallePlanForm #UsuarioCreacionId').val(localStorage.idUser);
            $('#NuevoDetallePlanForm #TipoActividadId').val($('#selTipoActividad').val());
            $('#NuevoDetallePlanForm #FechaActividad').val($('#fechaAct').val());
            var PlanSemanalId = $('#PlanSemanalId').val();
            //Se hace el post para guardar la informacion
            $.post(contextPath + "PlanSemanal/NuevoDetalle",
                $("#NuevoDetallePlanForm *").serialize(),
                function (data) {
                    if (data.Success === true) {
                        //PlanSemanal.colPlanDetalle.add(PlanSemanal.serializaPlanDetalle(data.id, '#NuevoDetallePlanForm'));
                        $('#nuevo-PlanDetalle').modal('hide');
                        FCH.DespliegaNotificacion('success', 'Actvidad ', data.Message, 'glyphicon-ok', 3000);
                        $("#btnEnvioValidacion_" + PlanSemanalId).prop("disabled", false);
                    } else {
                        FCH.DespliegaErrorDialogo(data.Message);
                    }
                }).fail(function () {
                    FCH.DespliegaErrorDialogo("Error al guardar la información");

                }).always(function () { FCH.botonMensaje(false, btn, btnName); });

        } else {
            FCH.botonMensaje(false, btn, btnName);
            if ($('#selTipoActividad').val() == "0") {
                $('#selTipoActividad').addClass("select2.error")
            }
        }
    },
    onActualizarDetalle: function (e) {
        var btn = this,
            btnName = btn.innerText;

        FCH.botonMensaje(true, btn, btnName);
        if ($("form").valid()) {
            $('#ActualizaDetallePlanForm #UsuarioCreacionId').val(localStorage.idUser);
            var PlanSemanalId = $('#PlanSemanalId').val();
            //Se hace el post para guardar la informacion
            $.post(contextPath + "PlanSemanal/ActualizaDetalle",
                $("#ActualizaDetallePlanForm *").serialize(),
                function (data) {
                    if (data.Success === true) {
                        $('#actualiza-PlanDetalle').modal('hide');
                        FCH.DespliegaNotificacion('success', 'Actividad ', data.Message, 'glyphicon-ok', 3000);
                        PlanSemanal.colPlanDetalle.add(PlanSemanal.serializaPlanDetalle(data.idDetallePlan, '#ActualizaDetallePlanForm'), { merge: true });
                    } else {
                        FCH.DespliegaErrorDialogo(data.Message);
                    }
                }).fail(function () {
                    FCH.DespliegaErrorDialogo("Error al guardar la información");
                }).always(function () { FCH.botonMensaje(false, btn, btnName); });
        } else {
            FCH.botonMensaje(false, btn, btnName);
        }
    },
    CargaGrid: function () {
        $('#bbGrid-clear')[0].innerHTML = "";

        $('#cargandoInfo').show();
        $('#bbGrid-clear')[0].innerHTML = "<span class='glyphicon glyphicon-refresh glyphicon-refresh-animate'></span> Cargando Información...";
        var url = contextPath + "PlanSemanal/CargarPlanes?idUsu=" + localStorage.idUser + "&idPeriodo=" + $('#selPeriodos').val(); // El url del controlador
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
                    actionenable: true,
                    prodDetalle: true,
                    clone: false,
                    editar: false,
                    borrar: false,
                    detalle: false,
                    collection: PlanSemanal.colPlanSemanal,
                    colModel: [//{ title: 'Id', name: 'id', width: '8%', sorttype: 'number', filter: true, filterType: 'input' },
                        { title: 'Usuario', name: 'usuario', index: true },
                        { title: 'Descripción', name: 'descripcionPlan', index: true },
                        { title: 'Periodo', name: 'periodo', index: true },
                        { title: 'Enviar a validación', name: 'accion', textalign: true }
                    ],
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
        var url = contextPath + "PlanSemanal/CargarDetallePlan?idPlanSemanal=" + id; // El url del controlador
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
                            { title: ' ', name: 'accion', textalign: true },
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
    CargaTipoActividad: function () {
        if (PlanSemanal.colTipoActividad.length < 1) {
            var url = contextPath + "TipoActividades/CargarTipoActividades?idEstatus=5";
            $.getJSON(url, function (data) {
                PlanSemanal.colTipoActividad = data;
                PlanSemanal.CargaListaTipoActividad();
            }).fail(function () {
                FCH.DespliegaErrorDialogo("No se pudieron cargar las actividades");
            });
        } else {
                PlanSemanal.CargaListaTipoActividad();
        }
    },
    CargaListaTipoActividad: function () {
        var select = $(PlanSemanal.activeForm + ' #selTipoActividad').empty();

        select.append('<option value="0">SELECCIONA ACTIVIDAD</option>');
        $.each(PlanSemanal.colTipoActividad, function (i, item) {
            select.append('<option value="' + item.id + '">' + item.nombre + '</option>');
        });

        $(PlanSemanal.activeForm + " #selTipoActividad").select2({ allowClear: true });
    },
    CargaPeriodoActual: function () {
        if (PlanSemanal.colPeriodoActual.length < 1) {
            var url = contextPath + "Periodos/CargarPeriodoActual?idEstatus=5";
            $.getJSON(url, function (data) {
                PlanSemanal.colPeriodoActual = data;
                PlanSemanal.CargaListaPeriodoActual();
            }).fail(function () {
                FCH.DespliegaErrorDialogo("No se pudieron cargar los peridos");
            });
        } else {
            PlanSemanal.CargaListaPeriodoActual();
        }
    },
    CargaListaPeriodoActual: function () {
        var select = $(PlanSemanal.activeForm + ' #selPeriodo').empty();

        select.append('<option value="0">SELECCIONAR</option>');
        $.each(PlanSemanal.colPeriodoActual, function (i, item) {
            select.append('<option value="' + item.id + '">' + item.nombre + '</option>');
        });

        $(PlanSemanal.activeForm + " #selPeriodo").select2({ allowClear: true });
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

        $(PlanSemanal.activeForm + " #selPeriodos").select2({ allowClear: true });
    },
    IniciaDateControls: function () {
        var form = PlanSemanal.activeForm;
        $(form + ' #dtpfechaCompromiso').datetimepicker({
            useCurrent: false,
            format: 'MM/DD/YYYY'
        });
    },
    serializaPlan: function (id, form) {
        return ({
            'usuario': localStorage.UserName,
            'descripcionPlan': $(form + ' #DescripcionPlaneacion').val(),
            'periodo': $(form + ' #selPeriodo option:selected').text().toUpperCase(),
            'accion': "<button disabled class='btn btn-xs btn-primary btnEnvioValidacion' style='border-radius: 21px;' id='btnEnvioValidacion_" + id + "' data-idplan='" + id + "'><i class='fa fa-paper-plane  fa-lg fa-fw'></i> Enviar</button>",
            'id': id
        });
    },
    serializaPlanDetalle: function (id, form) {
        return ({
            'actividad': $(form + ' #cTipoActividades_NombreActividad').val(),
            'descripcion': $(form + ' #DescripcionActividad').val(),
            'fecha': $(form + ' #fechaAct').val(),
            'hora': $(form + ' #HoraActividad').val(),
            'checkin': $(form + ' #CantidadCheckIn').val(),
            'id': id
        });
    }
};

$(function () {
    PlanSemanal.Inicial();
});