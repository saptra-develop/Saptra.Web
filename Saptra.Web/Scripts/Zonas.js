/* global CHR, contextPath, bootbox, Backbone, bbGrid */

//js de catalogo de Zonas.
//David Jasso
//19/Octubre/2018

var Zonas = {
    accClonar: false,
    accEscritura: false,
    accBorrar: false,
    activeForm: '',
    gridZonas: {},
    colZonas: {},
    colRegiones: {},
    CargaListaRegiones: [],
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
        if ($('#NuevoZonasForm #DescripcionCoordinacionZona').val() !== "" && $('#NuevoZonasForm #RegionId').val() !== "0") {
            FCH.botonMensaje(true, btn, 'Guardar');
            if ($("form").valid()) {
                $('#UsuarioCreacionId').val(localStorage.idUser);
                $('#CoordinacionRegionId').val($('#NuevoZonasForm #RegionId').val());
                //Se hace el post para guardar la informacion
                $.post(contextPath + "Zonas/Nuevo",
                    $("#NuevoZonasForm *").serialize(),
                    function (data) {
                        if (data.Success === true) {
                            FCH.DespliegaInformacion("La zona fue guardada con el Id: " + data.id);
                            $('#nuevo-Zonas').modal('hide');
                            Zonas.CargaGrid();
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
            if ($('#NuevoZonasForm #DescripcionCoordinacionZona').val() == "") {
                $('#NuevoZonasForm #DescripcionCoordinacionZona').addClass("input-validation-error");
                $('#NuevoZonasForm #DescripcionCoordinacionZona').attr("placeholder", "Obligatorio");
            }
            if ($('#NuevoZonasForm #RegionId').val() == "0") {
                $('#NuevoZonasForm #RegionId').addClass("has-error");
            }
        }
    },
    onActualizar: function () {
        var btn = this;

        if ($('#ActualizaZonasForm #DescripcionCoordinacionZona').val() !== "" && $('#ActualizaZonasForm #RegionId').val() !== "0" ) {
            FCH.botonMensaje(true, btn, 'Guardar');
            if ($("form").valid()) {
                //Se hace el post para guardar la informacion
                $('#CoordinacionRegionId').val($('#ActualizaZonasForm #RegionId').val());
                $.post(contextPath + "Zonas/Actualizar",
                    $("#ActualizaZonasForm *").serialize(),
                    function (data) {
                        if (data.Success === true) {
                            $('#actualiza-Zonas').modal('hide');
                            FCH.DespliegaInformacion("La zona fue Actualizada. Id: " + data.id);
                            Zonas.CargaGrid();
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
            if ($('#ActualizaZonasForm #DescripcionCoordinacionZona').val() == "") {
                $('#ActualizaZonasForm #DescripcionCoordinacionZona').addClass("input-validation-error");
                $('#ActualizaZonasForm #DescripcionCoordinacionZona').attr("placeholder", "Obligatorio");
            }
            if ($('#ActualizaZonasForm #RegionId').val() == "0") {
                $('#ActualizaZonasForm #RegionId').addClass("has-error");
            }
        }
    },
    Nuevo: function () {
        FCH.CierraMensajes();
        var url = contextPath + "Zonas/Nuevo"; // El url del controlador      
        $.get(url, function (data) {
            $('#nuevo-Zonas').html(data);
            $('#nuevo-Zonas').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');
            FCH.RedefinirValidaciones(); //para los formularios dinamicos
            Zonas.activeForm = '#NuevoZonasForm';
            $(Zonas.activeForm + ' #EstatusId').select2();
            Zonas.CargarColeccionRegiones();
        });
    },
    Editar: function (id) {
        FCH.CierraMensajes();
        var url = contextPath + "Zonas/Actualizar/" + id; // El url del controlador
        $.get(url, function (data) {
            $('#actualiza-Zonas').html(data);
            $('#actualiza-Zonas').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');

            FCH.RedefinirValidaciones(); //para los formularios dinamicos
            Zonas.activeForm = '#ActualizaZonasForm';
            $(Zonas.activeForm + ' #EstatusId').select2();
            Zonas.CargarColeccionRegiones();
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
                    var url = contextPath + "Zonas/Borrar"; // El url del controlador
                    $.post(url, { id: id }, function (data) {
                        if (data.Success === true) {
                            Zonas.colZonas.remove(id);
                            FCH.DespliegaInformacion(data.Message + "  id:" + id);
                        } else {
                            FCH.DespliegaError(data.Message);
                        }
                    }).fail(function () { FCH.DespliegaError("No se pudo eliminar la Zonas"); });
                } }
        })
    },
    CargaGrid: function () {
        $('#bbGrid-clear')[0].innerHTML = "";
        $('#cargandoInfo').show();
        var url = contextPath + "Zonas/CargarZonas"; // El url del controlador
        $.getJSON(url, function (data) {
            if (data.Success === false) { FCH.DespliegaError(data.Message); return; }
            $('#bbGrid-clear')[0].innerHTML = "";
            Zonas.colZonas = new Backbone.Collection(data);
            var bolFilter = Zonas.colZonas.length > 0 ? true : false;
            if (bolFilter) {
                Zonas.gridZonas = new bbGrid.View({
                    container: $('#bbGrid-clear'),
                    rows: 200,
                    rowList: [15, 50, 200, 1000],
                    enableSearch: true,
                    actionenable: true,
                    detalle: false,
                    clone: false,
                    editar: true,
                    borrar: true,
                    collection: Zonas.colZonas,
                    colModel: [{ title: 'Id', name: 'id', width: '8%', sorttype: 'number', index: true },
                        { title: 'Zona', name: 'nombre', index: true },
                        { title: 'Región', name: 'region', index: true },
                            { title: 'Estatus', name: 'estatus',  index: true }]

                });
                $('#cargandoInfo').hide();
            } else {
                FCH.DespliegaInformacion("No se encontraron zonas registradas");
                $('#bbGrid-clear')[0].innerHTML = "";
            }

            //getJSON fail
        }).fail(function (e) {
            FCH.DespliegaError("No se pudo cargar la informacion de Zonas");
        });
    },
    CargarColeccionRegiones: function () {
        var form = Zonas.activeForm;

        var url = contextPath + "Regiones/CargarRegiones"; // El url del controlador
        $.getJSON(url, function (data) {
            Zonas.colRegiones = data;
            Zonas.CargaListaRegiones(form);
        }).fail(function () {
            FCH.DespliegaErrorDialogo("No se pudo cargar la informacion de los roles ");
        });

    },
    CargaListaRegiones: function (form) {
        var select = $(form + ' #RegionId').empty();

        select.append('<option value="0">ELIJA UNA REGION...</option>');
        $.each(Zonas.colRegiones, function (i, item) {
            select.append('<option value="' + item.id + '">' + item.nombre + '</option>');
        });


        $(Zonas.activeForm + " #RegionId").select2({ allowClear: true });
        if ($(Zonas.activeForm + " #CoordinacionRegionId").val() !== "")
            $(Zonas.activeForm + ' #RegionId').val($(Zonas.activeForm + " #CoordinacionRegionId").val()).change();

    }
};

$(function () {
    Zonas.Inicial();
});