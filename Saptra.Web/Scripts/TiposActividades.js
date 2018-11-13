/* global CHR, contextPath, bootbox, Backbone, bbGrid */

//js de catalogo de Tipos de Actividades.
//David Jasso
//19/Octubre/2018

var TipoActividad = {
    accClonar: false,
    accEscritura: false,
    accBorrar: false,
    activeForm: '',
    gridTipoActividad: {},
    colTipoActividad: {},
    sm: 0,
    Inicial: function () {
        $.ajaxSetup({ cache: false });
        this.CargaGrid();
        this.Eventos();
        if (window.innerWidth < 768) {
            // Extra Small Device
            TipoActividad.sm = 1;  
        } 
    },
    Eventos: function () {
        var that = this;
        $('.btnNuevo').click(that.Nuevo);
        $(document).on("click", '.btn-GuardaNuevo', that.onGuardar);
        $(document).on("click", '.btn-Actualizar', that.onActualizar);
        //Eventos de los botones de Acciones del grid
        $(document).on('click', '.accrowEdit', function () {
            that.Editar($(this).parent().parent().attr("data-modelId"));
        });

        $(document).on('click', '.accrowBorrar', function () {
            that.Borrar($(this).parent().parent().attr("data-modelId"));
        });
    },
    onGuardar: function () {
        var btn = this;
        if ($('#NuevoTipoActividadForm #NombreActividad').val() !== "") {
            FCH.botonMensaje(true, btn, 'Guardar');
            if ($("form").valid()) {
                $('#UsuarioCreacionId').val(localStorage.idUser);
                $('#NuevoTipoActividadForm #RequiereCheckIn').val($('#NuevoTipoActividadForm #chkRequiereCheckIn').prop("checked") ? true : false);
                //Se hace el post para guardar la informacion
                $.post(contextPath + "TipoActividades/Nuevo",
                    $("#NuevoTipoActividadForm *").serialize(),
                    function (data) {
                        if (data.Success === true) {
                            FCH.DespliegaInformacion("La actividad fue guardada con el Id: " + data.id);
                            $('#nuevo-Actividad').modal('hide');
                            TipoActividad.CargaGrid();
                        } else {
                            FCH.botonMensaje(false, btn, 'Guardar');
                            FCH.DespliegaErrorDialogo(data.Message);

                        }
                    }).fail(function () {
                        FCH.DespliegaErrorDialogo("Error al guardar la información.");

                    }).always(function () { FCH.botonMensaje(false, btn, 'Guardar'); });

            } else {
                FCH.botonMensaje(false, btn, 'Guardar');
            }
        } else {
            $('#NuevoTipoActividadForm #NombreActividad').addClass("input-validation-error");
            $('#NuevoTipoActividadForm #NombreActividad').attr("placeholder", "Obligatorio");
        }
    },
    onActualizar: function () {
        var btn = this;
        if ($('#ActualizaTipoActividadForm #NombreActividad').val() !== "") {
            FCH.botonMensaje(true, btn, 'Guardar');
            if ($("form").valid()) {
                //Se hace el post para guardar la informacion
                $('#ActualizaTipoActividadForm #RequiereCheckIn').val($('#ActualizaTipoActividadForm #chkRequiereCheckIn').prop("checked") ? true : false);
                $.post(contextPath + "TipoActividades/Actualizar",
                    $("#ActualizaTipoActividadForm *").serialize(),
                    function (data) {
                        if (data.Success === true) {
                            $('#actualiza-Actividad').modal('hide');
                            FCH.DespliegaInformacion("La actividad fue Actualizada. Id: " + data.id);
                            TipoActividad.CargaGrid();
                        } else {
                            FCH.DespliegaErrorDialogo(data.Message);
                        }
                    }).fail(function () {
                        FCH.DespliegaErrorDialogo("Error al actualizar la información");
                    }).always(function () { FCH.botonMensaje(false, btn, 'Guardar'); });
            } else {
                FCH.botonMensaje(false, btn, 'Guardar');
            }
        } else {
            $('#ActualizaTipoActividadForm #NombreActividad').addClass("input-validation-error");
            $('#ActualizaTipoActividadForm #NombreActividad').attr("placeholder", "Obligatorio");
        }
    },
    Nuevo: function () {
        FCH.CierraMensajes();
        var url = contextPath + "TipoActividades/Nuevo"; // El url del controlador      
        $.get(url, function (data) {
            $('#nuevo-Actividad').html(data);
            $('#nuevo-Actividad').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');
            FCH.RedefinirValidaciones(); //para los formularios dinamicos
            TipoActividad.activeForm = '#NuevoTipoActividadForm';
            $('#NuevoTipoActividadForm #chkRequiereCheckIn').bootstrapSwitch({
                size: 'small',
                onText: 'SI',
                offText: 'NO',
            });
            $(TipoActividad.activeForm + ' #EstatusId').select2();
        });
    },
    Editar: function (id) {
        FCH.CierraMensajes();
        var url = contextPath + "TipoActividades/Actualizar/" + id; // El url del controlador
        $.get(url, function (data) {
            $('#actualiza-Actividad').html(data);
            $('#actualiza-Actividad').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');

            FCH.RedefinirValidaciones(); //para los formularios dinamicos
            $('#ActualizaTipoActividadForm #chkRequiereCheckIn').prop('checked', $('#ActualizaTipoActividadForm #RequiereCheckIn').val() === 'True' ? true : false);
            $('#ActualizaTipoActividadForm #chkRequiereCheckIn').bootstrapSwitch({
                size: 'small',
                onText: 'SI',
                offText: 'NO',
            });
            TipoActividad.activeForm = '#ActualizaTipoActividadForm';
            $(TipoActividad.activeForm + ' #EstatusId').select2();
        });
    },
    Borrar: function (id) {
        FCH.CierraMensajes();
        bootbox.setDefaults({
            locale: "es"
        });
        bootbox.confirm({
            message: "¿Está seguro que desea borrar este registro?",
         
            callback: function (result) {
                if (result) {
                    var url = contextPath + "TipoActividades/Borrar"; // El url del controlador
                    $.post(url, { id: id }, function (data) {
                        if (data.Success === true) {
                            //TipoActividad.colTipoActividad.remove(id);
                            TipoActividad.CargaGrid()
                            FCH.DespliegaInformacion(data.Message + "  id:" + id);
                        } else {
                            FCH.DespliegaError(data.Message);
                        }
                    }).fail(function () { FCH.DespliegaError("No se pudo eliminar la actividad"); });
                } }
        })
    },
    CargaGrid: function () {
        $('#bbGrid-clear')[0].innerHTML = "";
        $('#cargandoInfo').show();
        var url = contextPath + "TipoActividades/CargarTipoActividades"; // El url del controlador
        $.getJSON(url, function (data) {
            if (data.Success === false) { FCH.DespliegaError(data.Message); return; }
            $('#bbGrid-clear')[0].innerHTML = "";
            TipoActividad.colTipoActividad = new Backbone.Collection(data);
            var bolFilter = TipoActividad.colTipoActividad.length > 0 ? true : false;
            if (bolFilter) {
                TipoActividad.gridTipoActividad = new bbGrid.View({
                    container: $('#bbGrid-clear'),
                    //rows: TipoActividad.sm == 1 ? false : 200,
                    //rowList: [15, 50, 200, 1000],
                    //enableSearch: TipoActividad.sm == 1 ? false : true,
                    actionenable: true,
                    detalle: false,
                    clone: false,
                    editar: true,
                    borrar: true,
                    collection: TipoActividad.colTipoActividad,
                    colModel: [
                            TipoActividad.sm == 1 ? { customclass : 'ocultaColumna' } : { title: 'Id', name: 'id', width: '8%', sorttype: 'number', index: true },
                            { title: 'Actividad', name: 'nombre', index: true },
                            { title: 'Requiere CheckIn', name: 'requiereck', index: true },
                            { title: 'Estatus', name: 'estatus',  index: true }]

                });
                $('#cargandoInfo').hide();
            } else {
                FCH.DespliegaInformacion("No se encontraron actividades registradas");
                $('#bbGrid-clear')[0].innerHTML = "";
            }

            //getJSON fail
        }).fail(function (e) {
            FCH.DespliegaError("No se pudo cargar la informacion de actividades");
        });
    }
};

$(function () {
    TipoActividad.Inicial();
});