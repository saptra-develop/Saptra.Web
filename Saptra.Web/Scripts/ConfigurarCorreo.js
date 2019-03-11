/* global CHR, contextPath, bootbox, Backbone, bbGrid */

//js de configuración de correo.
//David Jasso
//1/Marzo/2019

var ConfCorreo = {
    accClonar: false,
    accEscritura: false,
    accBorrar: false,
    activeForm: '',
    gridConfCorreo: {},
    colConfCorreo: {},
    colTipoCorreo: [],
    Inicial: function () {
        $.ajaxSetup({ cache: false });
        this.CargaGrid();
        this.Eventos();
        this.ValidaPermisos();
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
    ValidaPermisos: function () {
        var permisos = localStorage.modPermisos,
            modulo = ConfCorreo;
        modulo.accEscritura = permisos.substr(1, 1) === '1' ? true : false;
        modulo.accBorrar = permisos.substr(2, 1) === '1' ? true : false;
        modulo.accClonar = permisos.substr(3, 1) === '1' ? true : false;

        if (modulo.accEscritura === true)
            $('.btnNuevo').show();
    },
    onGuardar: function () {
        var btn = this;
        if ($('#NuevoCorreoForm #Correo').val() !== "" && $('#NuevoCorreoForm #Contrasena').val() !== "" && $('#NuevoCorreoForm #Puerto').val() !== "" && $('#NuevoCorreoForm #Host').val() !== "") {
            FCH.botonMensaje(true, btn, 'Guardar');
            if ($("form").valid()) {
                $('#UsuarioCreacionId').val(localStorage.idUser);
                //Se hace el post para guardar la informacion
                $.post(contextPath + "Usuario/NuevoCorreo",
                    $("#NuevoCorreoForm *").serialize(),
                    function (data) {
                        if (data.Success === true) {
                            FCH.DespliegaInformacion("El correo fue guardado con el Id: " + data.id);
                            $('#nuevo-Correo').modal('hide');
                            ConfCorreo.CargaGrid();
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
            if ($('#NuevoCorreoForm #Correo').val() == "") {
                $('#NuevoCorreoForm #Correo').addClass("input-validation-error");
                $('#NuevoCorreoForm #Correo').attr("placeholder", "Obligatorio");
            }
            if ($('#NuevoCorreoForm #Contrasena').val() == "") {
                $('#NuevoCorreoForm #Contrasena').addClass("input-validation-error");
                $('#NuevoCorreoForm #Contrasena').attr("placeholder", "Obligatorio");
            }
            if ($('#NuevoCorreoForm #Puerto').val() == "") {
                $('#NuevoCorreoForm #Puerto').addClass("input-validation-error");
                $('#NuevoCorreoForm #Puerto').attr("placeholder", "Obligatorio");
            }
            if ($('#NuevoCorreoForm #Host').val() == "") {
                $('#NuevoCorreoForm #Host').addClass("input-validation-error");
                $('#NuevoCorreoForm #Host').attr("placeholder", "Obligatorio");
            }
        }
    },
    onActualizar: function () {
        var btn = this;

        if ($('#ActualizaCorreoForm #Correo').val() !== "" && $('#ActualizaCorreoForm #Contrasena').val() !== "" && $('#ActualizaCorreoForm #Puerto').val() !== "" && $('#ActualizaCorreoForm #Host').val() !== "") {
            FCH.botonMensaje(true, btn, 'Guardar');
            if ($("form").valid()) {
                //Se hace el post para guardar la informacion
                $.post(contextPath + "Usuario/ActualizarCorreo",
                    $("#ActualizaCorreoForm *").serialize(),
                    function (data) {
                        if (data.Success === true) {
                            $('#actualiza-Correo').modal('hide');
                            FCH.DespliegaInformacion("El correo fue Actualizado. Id: " + data.id);
                            ConfCorreo.CargaGrid();
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
            if ($('#ActualizaCorreoForm #Correo').val() == "") {
                $('#ActualizaCorreoForm #Correo').addClass("input-validation-error");
                $('#ActualizaCorreoForm #Correo').attr("placeholder", "Obligatorio");
            }
            if ($('#ActualizaCorreoForm #Contrasena').val() == "") {
                $('#ActualizaCorreoForm #Contrasena').addClass("input-validation-error");
                $('#ActualizaCorreoForm #Contrasena').attr("placeholder", "Obligatorio");
            }
            if ($('#ActualizaCorreoForm #Puerto').val() == "") {
                $('#ActualizaCorreoForm #Puerto').addClass("input-validation-error");
                $('#ActualizaCorreoForm #Puerto').attr("placeholder", "Obligatorio");
            }
            if ($('#ActualizaCorreoForm #Host').val() == "") {
                $('#ActualizaCorreoForm #Host').addClass("input-validation-error");
            }
        }
    },
    Nuevo: function () {
        FCH.CierraMensajes();
        var url = contextPath + "Usuario/NuevoCorreo"; // El url del controlador      
        $.get(url, function (data) {
            $('#nuevo-Correo').html(data);
            $('#nuevo-Correo').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');
            FCH.RedefinirValidaciones(); //para los formularios dinamicos
            ConfCorreo.activeForm = '#NuevoCorreoForm';
            $(ConfCorreo.activeForm + ' #EstatusId').select2();
            ConfCorreo.CargarColeccionTipoCorreo();
        });
    },
    Editar: function (id) {
        FCH.CierraMensajes();
        var url = contextPath + "Usuario/ActualizarCorreo/" + id; // El url del controlador
        $.get(url, function (data) {
            $('#actualiza-Correo').html(data);
            $('#actualiza-Correo').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');

            FCH.RedefinirValidaciones(); //para los formularios dinamicos
            ConfCorreo.activeForm = '#ActualizaCorreoForm';
            $(ConfCorreo.activeForm + ' #EstatusId').select2();
            ConfCorreo.CargarColeccionTipoCorreo();
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
                    var url = contextPath + "Usuario/BorrarCorreo"; // El url del controlador
                    $.post(url, { id: id }, function (data) {
                        if (data.Success === true) {
                            ConfCorreo.colConfCorreo.remove(id);
                            FCH.DespliegaInformacion(data.Message + "  id:" + id);
                        } else {
                            FCH.DespliegaError(data.Message);
                        }
                    }).fail(function () { FCH.DespliegaError("No se pudo eliminar el correo"); });
                } }
        })
    },
    CargaGrid: function () {
        $('#bbGrid-clear')[0].innerHTML = "";
        $('#cargandoInfo').show();
        var url = contextPath + "Usuario/CargarCorreos"; // El url del controlador
        $.getJSON(url, function (data) {
            if (data.Success === false) { FCH.DespliegaError(data.Message); return; }
            $('#bbGrid-clear')[0].innerHTML = "";
            ConfCorreo.colConfCorreo = new Backbone.Collection(data.datos);
            var bolFilter = ConfCorreo.colConfCorreo.length > 0 ? true : false;
            if (bolFilter) {
                ConfCorreo.gridConfCorreo = new bbGrid.View({
                    container: $('#bbGrid-clear'),
                    rows: 200,
                    rowList: [15, 50, 200, 1000],
                    enableSearch: true,
                    actionenable: true,
                    detalle: false,
                    clone: false,
                    editar: true,
                    borrar: false,
                    collection: ConfCorreo.colConfCorreo,
                    colModel: [{ title: 'Id', name: 'id', width: '8%', sorttype: 'number', index: true },
                            { title: 'Correo', name: 'correo', index: true },
                            { title: 'Contraseña', name: 'contrasena', index: true },
                            { title: 'Puerto', name: 'puerto', index: true },
                            { title: 'Host', name: 'host', index: true },
                            { title: 'Puerto', name: 'puerto', index: true },
                            { title: 'Tipo', name: 'tipo', index: true },
                            { title: 'Estatus', name: 'estatus', index: true }]

                });
                $('#cargandoInfo').hide();
            } else {
                $('#bbGrid-clear')[0].innerHTML = "<div class='alert alert-warning'>" +
                    "<button type='button' class='close' data-dismiss='alert'>x</button>No se encontraron correos registrados</div>";
            }

            //getJSON fail
        }).fail(function (e) {
            FCH.DespliegaError("No se pudo cargar la informacion de correos");
        });
    },
    CargarColeccionTipoCorreo: function () {
        var form = ConfCorreo.activeForm;
        if (ConfCorreo.colTipoCorreo.length < 1) {
            var url = contextPath + "Usuario/CargarTipoCorreo"; // El url del controlador
            $.getJSON(url, function (data) {
                ConfCorreo.colTipoCorreo = data;
                ConfCorreo.CargaListaTipoCorreo(form);
            }).fail(function () {
                FCH.DespliegaErrorDialogo("No se pudo cargar la informacion de regiones ");
            });
        } else {
            ConfCorreo.CargaListaTipoCorreo(form);
        }
    },
    CargaListaTipoCorreo: function (form) {
        var select = $(form + ' #TipoCorreoId').empty();

        //select.append('<option value="0">TIPO CORREO...</option>');
        $.each(ConfCorreo.colTipoCorreo, function (i, item) {
            select.append('<option value="' + item.id + '">' + item.tipoCorreo + '</option>');
        });

        $(ConfCorreo.activeForm + " #TipoCorreoId").select2({ allowClear: true});
        if ($(ConfCorreo.activeForm + " #selTipoCorreoId").val() !== "0")
            $(ConfCorreo.activeForm + ' #TipoCorreoId').val($(ConfCorreo.activeForm + " #selTipoCorreoId").val()).change();




    }
};

$(function () {
    ConfCorreo.Inicial();
});