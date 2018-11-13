/* global CHR, contextPath, bootbox, Backbone, bbGrid */

//js de catalogo de Regiones.
//David Jasso
//19/Octubre/2018

var Regiones = {
    accClonar: false,
    accEscritura: false,
    accBorrar: false,
    activeForm: '',
    gridRegiones: {},
    colRegiones: {},
    Inicial: function () {
        $.ajaxSetup({ cache: false });
        this.CargaGrid();
        this.Eventos();
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
        if ($('#NuevoRegionesForm #DescripcionCoordinacionRegion').val() !== "") {
            FCH.botonMensaje(true, btn, 'Guardar');
            if ($("form").valid()) {
                $('#UsuarioCreacionId').val(localStorage.idUser);
                //Se hace el post para guardar la informacion
                $.post(contextPath + "Regiones/Nuevo",
                    $("#NuevoRegionesForm *").serialize(),
                    function (data) {
                        if (data.Success === true) {
                            FCH.DespliegaInformacion("La región fue guardada con el Id: " + data.id);
                            $('#nuevo-Regiones').modal('hide');
                            Regiones.CargaGrid();
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
            $('#NuevoRegionesForm #DescripcionCoordinacionRegion').addClass("input-validation-error");
            $('#NuevoRegionesForm #DescripcionCoordinacionRegion').attr("placeholder", "Obligatorio");
        }
    },
    onActualizar: function () {
        var btn = this;

        if ($('#ActualizaRegionesForm #DescripcionCoordinacionRegion').val() !== "") {
        FCH.botonMensaje(true, btn, 'Guardar');
        if ($("form").valid()) {
            //Se hace el post para guardar la informacion
            $.post(contextPath + "Regiones/Actualizar",
                $("#ActualizaRegionesForm *").serialize(),
                function (data) {
                    if (data.Success === true) {
                        $('#actualiza-Regiones').modal('hide');
                        FCH.DespliegaInformacion("La región fue Actualizada. Id: " + data.id);
                        Regiones.CargaGrid();
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
            $('#ActualizaRegionesForm #DescripcionCoordinacionRegion').addClass("input-validation-error");
            $('#ActualizaRegionesForm #DescripcionCoordinacionRegion').attr("placeholder", "Obligatorio");
        }
    },
    Nuevo: function () {
        FCH.CierraMensajes();
        var url = contextPath + "Regiones/Nuevo"; // El url del controlador      
        $.get(url, function (data) {
            $('#nuevo-Regiones').html(data);
            $('#nuevo-Regiones').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');
            FCH.RedefinirValidaciones(); //para los formularios dinamicos
            Regiones.activeForm = '#NuevoRegionesForm';
            $('#NuevoRegionesForm #chkRequiereCheckIn').bootstrapSwitch({
                size: 'small',
                onText: 'SI',
                offText: 'NO',
            });
            $(Regiones.activeForm + ' #EstatusId').select2();
        });
    },
    Editar: function (id) {
        FCH.CierraMensajes();
        var url = contextPath + "Regiones/Actualizar/" + id; // El url del controlador
        $.get(url, function (data) {
            $('#actualiza-Regiones').html(data);
            $('#actualiza-Regiones').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');

            FCH.RedefinirValidaciones(); //para los formularios dinamicos
            Regiones.activeForm = '#ActualizaRegionesForm';
            $(Regiones.activeForm + ' #EstatusId').select2();
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
                    var url = contextPath + "Regiones/Borrar"; // El url del controlador
                    $.post(url, { id: id }, function (data) {
                        if (data.Success === true) {
                            Regiones.colRegiones.remove(id);
                            FCH.DespliegaInformacion(data.Message + "  id:" + id);
                        } else {
                            FCH.DespliegaError(data.Message);
                        }
                    }).fail(function () { FCH.DespliegaError("No se pudo eliminar la región"); });
                } }
        })
    },
    CargaGrid: function () {
        $('#bbGrid-clear')[0].innerHTML = "";
        $('#cargandoInfo').show();
        var url = contextPath + "Regiones/CargarRegiones"; // El url del controlador
        $.getJSON(url, function (data) {
            if (data.Success === false) { FCH.DespliegaError(data.Message); return; }
            $('#bbGrid-clear')[0].innerHTML = "";
            Regiones.colRegiones = new Backbone.Collection(data);
            var bolFilter = Regiones.colRegiones.length > 0 ? true : false;
            if (bolFilter) {
                Regiones.gridRegiones = new bbGrid.View({
                    container: $('#bbGrid-clear'),
                    rows: 200,
                    rowList: [15, 50, 200, 1000],
                    enableSearch: true,
                    actionenable: true,
                    detalle: false,
                    clone: false,
                    editar: true,
                    borrar: true,
                    collection: Regiones.colRegiones,
                    colModel: [{ title: 'Id', name: 'id', width: '8%', sorttype: 'number', index: true },
                            { title: 'Región', name: 'nombre', index: true },
                            { title: 'Estatus', name: 'estatus',  index: true }]

                });
                $('#cargandoInfo').hide();
            } else {
                FCH.DespliegaInformacion("No se encontraron regiones registradas");
                $('#bbGrid-clear')[0].innerHTML = "";
            }

            //getJSON fail
        }).fail(function (e) {
            FCH.DespliegaError("No se pudo cargar la informacion de regiones");
        });
    }
};

$(function () {
    Regiones.Inicial();
});