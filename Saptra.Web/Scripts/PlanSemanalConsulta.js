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
    colEducandos: {},
    colPlanDetalle: {},
    gridPlanDetalle: {},
    gridPlanSemanal: {},
    colPeriodos: [],
    seleccionados: [],
    idPlan: 0,
    detalleContainer: {},
    educandos: [],
    colVerEducandos: {},
    Inicial: function () {
        $.ajaxSetup({ cache: false });
        this.CargaPeriodos();
        this.Eventos();
        this.CargaGrid();
    },
    Eventos: function () {
        var that = this;
        $('#btnFilter').click(that.CargaGrid);
        $(document).on('click', '.btnAgregarEducandos', function () {
            PlanSemanal.AgregarEducandos($(this).data("iddetalleplan"));
        });
        $(document).on("change", '#chkAllEducandos', that.onCheckAll);
        //Serializa Individual
        $(document).on("change", '.chkRenglonGridPre', function () {
            var idPreregistros = $(this).attr("data-idpreregistro");
            var estado = $(this).is(':checked');
            PlanSemanal.AddRemSeleccionados(estado, idPreregistros);
        });

        $(document).on("click", '.btn-NoCaptados', that.onActualizarDetalleSinProspectos);
        $(document).on("click", '.btn-GuardaEducandos', that.onActualizarEducandos);
        
        $(document).on('click', '.hrefVerEducandos', function () {
            PlanSemanal.VerEducandos($(this).data("iddetalleplan"));
        });
    },
    createJSON: function () {
        jsonObj = [];
        var idDetalle = $('#NuevoPreregistrosForm #DetallePlanId').val();
        $.each($('.chkRenglonGridPre'), function (i, item) {
            if ($(item).is(':checked')) {
                PlanSemanal.seleccionados.push($(item).data("idpreregistro"));
                var idPreregistro = $(item).data("idpreregistro");
                elem = { idPreregistro: idPreregistro, idDetallePlan: idDetalle };

                jsonObj.push(elem);
            }
    
        });

        return JSON.stringify(jsonObj);
    },
    onActualizarEducandos: function (e) {

        //PlanSemanal.educandos = 

        var objetoJson = PlanSemanal.createJSON();
        if (objetoJson != 0) {
            var btn = this,
                btnName = btn.innerText;
            FCH.botonMensaje(true, btn, btnName);
            $.ajax({
                url: contextPath + "PlanSemanalConsulta/ActualizaEducandos",
                type: 'post',
                data: objetoJson,
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    if (data.Success === true) {
                        FCH.botonMensaje(false, btn, btnName);
                        $('#nuevo-Preregistros').modal('hide');
                        FCH.DespliegaNotificacion('success', 'Educandos ', data.Message, 'glyphicon-ok', 3000);
                        PlanSemanal.CargaGridDetallesPackingList(PlanSemanal.idPlan, PlanSemanal.detalleContainer);
                       
                    } else {
                        FCH.DespliegaErrorDialogo(data.Message);
                        FCH.botonMensaje(false, btn, btnName);
                    }
                }
            });
        }
        else {
            FCH.DespliegaError("Es necesario seleccionar educandos para vincular con la actividad seleccionada.");
        }
    },
    onActualizarDetalleSinProspectos: function (e) {
        var btn = this,
            btnName = btn.innerText;
        FCH.botonMensaje(true, btn, btnName);
        if ($("form").valid()) {
            //Se hace el post para guardar la informacion
            $.post(contextPath + "PlanSemanalConsulta/ActualizaSinProspectos",
                $("#NuevoPreregistrosForm *").serialize(),
                function (data) {
                    if (data.Success === true) {
                        $('#nuevo-Preregistros').modal('hide');
                        FCH.DespliegaNotificacion('success', 'Actividad ', data.Message, 'glyphicon-ok', 3000);
                        PlanSemanal.CargaGridDetallesPackingList(PlanSemanal.idPlan, PlanSemanal.detalleContainer);
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
    AddRemSeleccionados: function (estado, idPreregistros) {
        if (estado == true) {
            PlanSemanal.seleccionados.push(idPreregistros);
        }
        else {
            PlanSemanal.seleccionados.splice($.inArray(parseInt(idPreregistros), PlanSemanal.seleccionados), 1);
        }
        PlanSemanal.seleccionados = PlanSemanal.seleccionados.unique();
    },
    SerializaSeleccionados: function () {
        $.each(PlanSemanal.seleccionados, function (i, item) {
            $('#' + item + '-Campocheck').prop('checked', true);
        });

    },
    onCheckAll: function () {
        $('.chkRenglonGridPre').prop('checked', $('#chkAllEducandos').is(':checked'));

        $.each($('.chkRenglonGridPre'), function (i, item) {
            if ($(item).is(':checked')) {
                PlanSemanal.seleccionados.push($(item).data("idpreregistro"));
            }
            else {
                PlanSemanal.seleccionados.splice($.inArray($(item).data("idpreregistro"), PlanSemanal.seleccionados), 1);
            }
            PlanSemanal.seleccionados = PlanSemanal.seleccionados.unique();
        });
      
    },
    AgregarEducandos: function (id) {
        FCH.CierraMensajes();
        var url = contextPath + "PlanSemanalConsulta/NuevoPreregistros/" + id; // El url del controlador      
        $.get(url, function (data) {
            $('#nuevo-Preregistros').html(data);
            $('#nuevo-Preregistros').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');
            FCH.RedefinirValidaciones(); //para los formularios dinamicos
            PlanSemanal.activeForm = '#NuevoPreregistrosForm';
            PlanSemanal.CargaGridEducandos();
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
    CargaGridEducandos: function () {
        $('#bbGrid-Preregistros')[0].innerHTML = "";

        $('#cargandoInfo').show();
        $('#bbGrid-Preregistros')[0].innerHTML = "<span class='glyphicon glyphicon-refresh glyphicon-refresh-animate'></span> Cargando Información...";
        var url = contextPath + "PlanSemanalConsulta/CargarEducandos?idUsuario=" + localStorage.idUser; // El url del controlador
        $.getJSON(url, function (data) {
            if (data.Success === false) { FCH.DespliegaError(data.Message); return; }
            $('#bbGrid-Preregistros')[0].innerHTML = "";
            PlanSemanal.colEducandos = new Backbone.Collection(data.datos);
            var bolFilter = PlanSemanal.colEducandos.length > 0 ? true : false;
            if (bolFilter) {
                PlanSemanal.gridPlanSemanal = new bbGrid.View({
                    container: $('#bbGrid-Preregistros'),
                    collection: PlanSemanal.colEducandos,
                    colModel: [{ title: 'Sel.&nbsp;<input type="checkbox" id="chkAllEducandos"  />', name: 'Campocheck', textalign: true  },
                        { title: 'Nombre', name: 'nombre', index: true, filter: true, filterType: 'input' },
                        { title: 'CURP', name: 'curp', index: true, filter: true, filterType: 'input' },
                        { title: 'Fecha nacimiento', name: 'fecha', index: true, filter: true, filterType: 'input' }],
                    afterRender: function () {
                        PlanSemanal.SerializaSeleccionados();
                    }

                });
                $('#cargandoInfo').hide();
            } else {
                $('#bbGrid-Preregistros')[0].innerHTML = "<div class='alert alert-warning'>" +
                    "<button type='button' class='close' data-dismiss='alert'>x</button>" +
                    "No se encontro información" +
                    "</div>";
            }

            //getJSON fail
        }).fail(function (e) {
            FCH.DespliegaError("No se pudo cargar la informacion de los pre-registros");
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
    CargaGrid: function () {
        $('#bbGrid-clear')[0].innerHTML = "";

        $('#cargandoInfo').show();
        $('#bbGrid-clear')[0].innerHTML = "<span class='glyphicon glyphicon-refresh glyphicon-refresh-animate'></span> Cargando Información...";
        var url = contextPath + "PlanSemanalConsulta/CargarPlanes?idUsu=" + localStorage.idUser + "&idPeriodo=" + ($('#selPeriodos').val() == "Cargando..." ? "0" : $('#selPeriodos').val()); // El url del controlador
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
                        { title: 'Usuario', name: 'usuario', index: true },
                        { title: 'Descripción', name: 'descripcionPlan', index: true },
                        { title: 'Periodo', name: 'periodo', index: true }],
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
        var url = contextPath + "PlanSemanalConsulta/CargarDetallePlan?idPlanSemanal=" + id; // El url del controlador
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
                            { title: 'Actividad', name: 'actividad', index: true, textalign: true },
                            { title: 'Descripción', name: 'descripcion', index: true },
                            { title: 'Lugar', name: 'lugar', index: true },
                            { title: 'Fecha', name: 'fecha',  index: true },
                            { title: 'Hora Inicio', name: 'hora', index: true },
                            { title: 'Hora Fin', name: 'horaFin', index: true },
                            { title: 'Registro', name: 'checkin', index: true },
                            { title: 'Matricula automovil', name: 'placa', index: true },
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

        $(PlanSemanal.activeForm + " #selPeriodos").select2({ allowClear: true });
    }
 
};

$(function () {
    PlanSemanal.Inicial();
});

Array.prototype.unique = function (a) {
    return function () { return this.filter(a) }
}(function (a, b, c) {
    return c.indexOf(a, b + 1) < 0
});