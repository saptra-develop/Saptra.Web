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
    Inicial: function () {
        $.ajaxSetup({ cache: false });
        this.CargaGrid();
        this.Eventos();
        //this.ValidaPermisos();
    },
    Eventos: function () {
        var that = this;
        $('.btnNuevo').click(that.Nuevo);

        $(document).on('click', '.accrowProdDetalle', function () {
            PlanSemanal.NuevoDetalle($(this).parent().parent().attr("data-modelId"));
        });

        $(document).on("click", '.btn-GuardaDetalleNuevo', that.onGuardarDetalle);

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
    onGuardarDetalle: function (e) {
        var btn = this,
            btnName = btn.innerText;

        FCH.botonMensaje(true, btn, btnName);
        if ($("form").valid()) {
            $('#UsuarioCreacionId').val(localStorage.idUser);
            $('#TipoActividadId').val($('#selTipoActividad').val());
            //Se hace el post para guardar la informacion
            $.post(contextPath + "PlanSemanal/NuevoDetalle",
                $("#NuevoDetallePlanForm *").serialize(),
                function (data) {
                    if (data.Success === true) {
                        PlanSemanal.colPlanDetalle.add(PlanSemanal.serializaPlanDetalle(data.id, '#NuevoDetallePlanForm'));
                        $('#nuevo-PlanDetalle').modal('hide');
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
        $('#cargandoInfo').show();
        var url = contextPath + "PlanSemanal/CargarPlanes"; // El url del controlador
        $.getJSON(url, function (data) {
            if (data.Success === false) { FCH.DespliegaError(data.Message); return; }
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
                    collection: PlanSemanal.colPlanSemanal,
                    colModel: [//{ title: 'Id', name: 'id', width: '8%', sorttype: 'number', filter: true, filterType: 'input' },
                               { title: 'Usuario', name: 'usuario', filter: true, filterType: 'input', index: true },
                                { title: 'Descripción', name: 'descripcionPlan', filter: true, filterType: 'input', index: true },
                                { title: 'Periodo', name: 'periodo', filter: true, filterType: 'input', index: true }],
                    onRowExpanded: function ($el, rowid) {
                        PlanSemanal.CargaGridDetallesPackingList(rowid, $el);
                    }

                });
                $('#cargandoInfo').hide();
            } else {
                FCH.DespliegaInformacion(jsglb[system_lang].Usuario_Recond_not_found);
                $('#bbGrid-clear')[0].innerHTML = "";
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
                detalle: false,
                actionenable: true,
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
    CargaTipoActividad: function () {
        if (PlanSemanal.colTipoActividad.length < 1) {
            var url = contextPath + "TipoActividades/CargarTipoActividades?idEstatus=5";
            $.getJSON(url, function (data) {
                PlanSemanal.colTipoActividad = data;
                PlanSemanal.CargaListaTipoActividad();
            }).fail(function () {
                CHR.DespliegaErrorDialogo("No se pudieron cargar las actividades");
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
    IniciaDateControls: function () {
        var form = PlanSemanal.activeForm;
        $(form + ' #dtpfechaCompromiso').datetimepicker({
            useCurrent: false,
            format: 'YYYY/MM/DD'
        });
    },
    serializaPlanDetalle: function (id, form) {
        return ({
            'actividad': $(form + ' #selTipoActividad option:selected').text(),
            'descripcion': $(form + ' #DescripcionActividad').val(),
            'fecha': $(form + ' #FechaActividad').val(),
            'hora': $(form + ' #HoraActividad').val(),
            'checkin': $(form + ' #CantidadCheckIn').val(),
            'id': id
        });
    }
};

$(function () {
    PlanSemanal.Inicial();
});