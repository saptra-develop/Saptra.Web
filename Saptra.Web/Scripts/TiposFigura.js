/* global CHR, contextPath, bootbox, Backbone, bbGrid */

//js de catalogo de Tipos de Figura.
//David Jasso
//19/Octubre/2018

var TipoFigura = {
    accClonar: false,
    accEscritura: false,
    accBorrar: false,
    activeForm: '',
    gridTipoFigura: {},
    colTipoFigura: {},
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

        FCH.botonMensaje(true, btn, 'Guardar');
        if ($("form").valid()) {
            $('#UsuarioCreacionId').val(localStorage.idUser);
            //Se hace el post para guardar la informacion
            $.post(contextPath + "TiposFigura/Nuevo",
                $("#NuevoTipoFiguraForm *").serialize(),
                function (data) {
                    if (data.Success === true) {
                        FCH.DespliegaInformacion("El tipo figura fue guardado con el Id: " + data.id);
                        $('#nuevo-Figura').modal('hide');
                        TipoFigura.CargaGrid();
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
    },
    onActualizar: function () {
        var btn = this;

        FCH.botonMensaje(true, btn, 'Guardar');
        if ($("form").valid()) {
            //Se hace el post para guardar la informacion
            $.post(contextPath + "TiposFigura/Actualizar",
                $("#ActualizaTipoFiguraForm *").serialize(),
                function (data) {
                    if (data.Success === true) {
                        $('#actualiza-Figura').modal('hide');
                        FCH.DespliegaInformacion("El tipo figura fue Actualizado. Id: " + data.id);
                        TipoFigura.CargaGrid();
                    } else {
                        FCH.DespliegaErrorDialogo(data.Message);
                    }
                }).fail(function () {
                    FCH.DespliegaErrorDialogo("Error al actualizar la información");
                }).always(function () { FCH.botonMensaje(false, btn, 'Guardar'); });
        } else {
            FCH.botonMensaje(false, btn, 'Guardar');
        }
    },
    Nuevo: function () {
        FCH.CierraMensajes();
        var url = contextPath + "TiposFigura/Nuevo"; // El url del controlador      
        $.get(url, function (data) {
            $('#nuevo-Figura').html(data);
            $('#nuevo-Figura').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');
            FCH.RedefinirValidaciones(); //para los formularios dinamicos
            TipoFigura.activeForm = '#NuevoTipoFiguraForm';
            $(TipoFigura.activeForm + ' #EstatusId').select2();
        });
    },
    Editar: function (id) {
        FCH.CierraMensajes();
        var url = contextPath + "TiposFigura/Actualizar/" + id; // El url del controlador
        $.get(url, function (data) {
            $('#actualiza-Figura').html(data);
            $('#actualiza-Figura').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');

            FCH.RedefinirValidaciones(); //para los formularios dinamicos
            TipoFigura.activeForm = '#ActualizaTipoFiguraForm';
            $(TipoFigura.activeForm + ' #EstatusId').select2();
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
                    var url = contextPath + "TiposFigura/Borrar"; // El url del controlador
                    $.post(url, { id: id }, function (data) {
                        if (data.Success === true) {
                            TipoFigura.colTipoFigura.remove(id);
                            FCH.DespliegaInformacion(data.Message + "  id:" + id);
                        } else {
                            FCH.DespliegaError(data.Message);
                        }
                    }).fail(function () { FCH.DespliegaError("No se pudo eliminar el tipo figura"); });
                } }
        })
    },
    CargaGrid: function () {
        $('#bbGrid-clear')[0].innerHTML = "";
        $('#cargandoInfo').show();
        var url = contextPath + "TiposFigura/CargarTiposFigura"; // El url del controlador
        $.getJSON(url, function (data) {
            if (data.Success === false) { FCH.DespliegaError(data.Message); return; }
            $('#bbGrid-clear')[0].innerHTML = "";
            TipoFigura.colTipoFigura = new Backbone.Collection(data);
            var bolFilter = TipoFigura.colTipoFigura.length > 0 ? true : false;
            if (bolFilter) {
                TipoFigura.gridTipoFigura = new bbGrid.View({
                    container: $('#bbGrid-clear'),
                    rows: 200,
                    rowList: [15, 50, 200, 1000],
                    enableSearch: true,
                    actionenable: true,
                    detalle: false,
                    clone: false,
                    editar: true,
                    borrar: false,
                    collection: TipoFigura.colTipoFigura,
                    colModel: [{ title: 'Id', name: 'id', width: '8%', sorttype: 'number' },
                               { title: 'Tipo Figura', name: 'nombre',  index: true },
                               { title: 'Estatus', name: 'estatus',  index: true }]

                });
                $('#cargandoInfo').hide();
            } else {
                FCH.DespliegaInformacion("No se encontraron tipos de figura registrados");
                $('#bbGrid-clear')[0].innerHTML = "";
            }

            //getJSON fail
        }).fail(function (e) {
            FCH.DespliegaError("No se pudo cargar la informacion de los tipos de figura");
        });
    }
};

$(function () {
    TipoFigura.Inicial();
});