/* global CHR, contextPath, bootbox, Backbone, bbGrid */

//js de catalogo de proyectos.
//David Jasso y Juan Lopepe
//07/Septiembre/2016

var Usuario = {
    accClonar: false,
    accEscritura: false,
    accBorrar: false,
    activeForm: '',
    colUsuarios: {},
    colRoles: [],
    colUsuariosJefe: [],
    gridUsuarios: {},
    Inicial: function () {
        $.ajaxSetup({ cache: false });
        this.CargaGrid();
        this.Eventos();
        this.ValidaPermisos();
    },
    Eventos: function () {
        var that = this;
        $('.btnNuevo').click(that.Nuevo);
        $(document).on("click", '.btn-GuardaNuevo', that.onSubirArchivo);
        $(document).on("click", '.btn-ActualizarUsuario', that.onSubirArchivo);
        //$(document).on("click", '.btn-Avatar', that.onAvatar());

        //Eventos de los botones de Acciones del grid
        $(document).on('click', '.accrowEdit', function () {
            that.Editar($(this).parent().parent().attr("data-modelId"));
        });

        $(document).on('click', '.accrowBorrar', function () {
            that.Borrar($(this).parent().parent().attr("data-modelId"));
        });

        $(document).on('click', '.accrowClonar', function () {
            that.Clonar($(this).parent().parent().attr("data-modelId"));
        });

        $(document).on("change", '#imgUsu', function () {
            if (this.files && this.files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    $('#imgUpl').attr('src', e.target.result);
                };
                reader.readAsDataURL(this.files[0]);
            }
        });


    },
    onGuardar: function (e) {
        var btn = e;

        FCH.botonMensaje(true, btn, 'Save');
        if ($("form").valid()) {
            $('#NuevoUsuarioForm #liberaMatriz').val($('#NuevoUsuarioForm #chkReleasedMatrix').prop("checked") ? 1 : 0);
            //$("#imagenUsuario").val('Hola');
            //Se hace el post para guardar la informacion
            $.post(contextPath + "Usuario/Nuevo",
                $("#NuevoUsuarioForm *").serialize(),
                function (data) {
                    if (data.Success === true) {
                        Usuario.colUsuarios.add(Usuario.serializaUsuario(data.id, '#NuevoUsuarioForm'));
                        FCH.DespliegaInformacion(jsglb[system_lang].Usuario_succcess_save + data.id);
                        $('#nuevo-Usuario').modal('hide');
                        if (Usuario.colUsuarios.length === 1) {
                            Usuario.CargaGrid();
                        }
                    } else {
                        FCH.botonMensaje(false, btn, 'Save');
                        FCH.DespliegaErrorDialogo(data.Message);
                        
                    }
                }).fail(function () {
                    FCH.DespliegaErrorDialogo(jsglb[system_lang].error_saving_info);

                }).always(function () { FCH.botonMensaje(false, btn, 'Save'); });

        } else {
            FCH.botonMensaje(false, btn, 'Save');
        }
    },
    onActualizar: function (e) {
        var btn = e;

        FCH.botonMensaje(true, btn, 'Save');
        if ($("form").valid()) {
            $('#ActualizaUsuarioForm #liberaMatriz').val($('#ActualizaUsuarioForm #chkReleasedMatrix').prop("checked") ? 1 : 0);
            //Se hace el post para guardar la informacion
            $.post(contextPath + "Usuario/Actualiza",
                $("#ActualizaUsuarioForm *").serialize(),
                function (data) {
                    if (data.Success === true) {
                        $('#actualiza-Usuario').modal('hide');
                        Usuario.colUsuarios.add(Usuario.serializaUsuario(data.id, '#ActualizaUsuarioForm'), { merge: true });
                        FCH.DespliegaInformacion(jsglb[system_lang].Usuario_success_update + data.id);
                    } else {
                        FCH.DespliegaErrorDialogo(data.Message);
                    }
                }).fail(function () {
                    FCH.DespliegaErrorDialogo(jsglb[system_lang].error_updating_info);
                }).always(function () { FCH.botonMensaje(false, btn, 'Save'); });
        } else {
            FCH.botonMensaje(false, btn, 'Save');
        }
    },
    onSubirArchivo: function () {
        var filesName = Usuario.activeForm === '#NuevoUsuarioForm' ? 'imgUsu' : 'imgUsuAct',
            btn = this,
            form = Usuario.activeForm,
            files;

        if ($("form").valid()) {
            FCH.botonMensaje(true, btn, 'Save');
            files = document.getElementById(filesName).files;
            if (files.length > 0) {
                if (window.FormData !== undefined) {
                    var data = new FormData();
                    for (var count = 0; count < files.length; count++) {
                        data.append(files[count].name, files[count]);
                    }
                    $.ajax({
                        type: "POST",
                        url: '/Usuario/SubirArchivo', //contextPath + '/Usuario/SubirArchivo', para produccion
                        contentType: false,
                        processData: false,
                        data: data,
                        success: function (result) {

                            if (result.Success === true) {
                                $(Usuario.activeForm + ' #imagenUsuario').val(result.Archivo);

                                if (Usuario.activeForm !== '#NuevoUsuarioForm') {
                                    Usuario.onActualizar(btn);
                                } else {
                                    Usuario.onGuardar(btn);
                                }

                            } else {
                                $(Usuario.activeForm + ' #imagenUsuario').val('');
                                FCH.DespliegaErrorDialogo(result.Message);
                                FCH.botonMensaje(false, btn, 'Guardar');
                            }
                        },
                        error: function (xhr, status, p3, p4) {
                            var err = "Error " + " " + status + " " + p3 + " " + p4;
                            if (xhr.responseText && xhr.responseText[0] === "{") {
                                err = JSON.parse(xhr.responseText).Message;
                            }
                            FCH.DespliegaErrorDialogo(err);
                            FCH.botonMensaje(false, btn, 'Guardar');
                        }
                    });
                } else {
                    FCH.DespliegaErrorDialogo(jsglb[system_lang].browser_not_supported);
                    FCH.botonMensaje(false, btn, 'Guardar');
                }
            } else {
                if (Usuario.activeForm !== '#NuevoUsuarioForm') {
                    Usuario.onActualizar(btn);
                } else {
                    $(Usuario.activeForm + ' #imagenUsuario').val('');
                    Usuario.onGuardar(btn);
                }
            }
        }
    },
    Nuevo: function () {
        FCH.CierraMensajes();
        var url = contextPath + "Usuario/Nuevo"; // El url del controlador      
        $.get(url, function (data) {
            $('#nuevo-Usuario').html(data);
            $('#nuevo-Usuario').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');
            FCH.RedefinirValidaciones(); //para los formularios dinamicos
            Usuario.activeForm = '#NuevoUsuarioForm';
            $('#NuevoUsuarioForm #chkReleasedMatrix').bootstrapSwitch({
                size: 'small',
                onText: 'YES',
                offText: 'NO',
            });
            Usuario.CargarColeccionRoles();
            Usuario.CargarColeccionUsuarios();
            Usuario.EventoNombreArchivo();
        });
    },
    Editar: function (id) {
        FCH.CierraMensajes();
        var url = contextPath + "Usuario/Actualiza/" + id; // El url del controlador
        $.get(url, function (data) {
            $('#actualiza-Usuario').html(data);
            $('#actualiza-Usuario').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');

            FCH.RedefinirValidaciones(); //para los formularios dinamicos
            Usuario.activeForm = '#ActualizaUsuarioForm';
            $('#ActualizaUsuarioForm #chkReleasedMatrix').prop('checked', $('#ActualizaUsuarioForm #liberaMatriz').val() === '1' ? true : false);
            $('#ActualizaUsuarioForm #chkReleasedMatrix').bootstrapSwitch({
                size: 'small',
                onText: 'YES',
                offText: 'NO',
            });
            Usuario.CargarColeccionRoles();
            Usuario.CargarColeccionUsuarios();
            Usuario.EventoNombreArchivo();

        });
    },
    Borrar: function (id) {
        FCH.CierraMensajes();

        bootbox.confirm(jsglb[system_lang].global_confirm_delete, function (result) {
            if (result) {
                var url = contextPath + "Usuario/Borrar"; // El url del controlador
                $.post(url, { id: id }, function (data) {
                    if (data.Success === true) {
                        Usuario.colUsuarios.remove(id);
                        FCH.DespliegaInformacion(data.Message + "  id:" + id);
                    } else {
                        FCH.DespliegaError(data.Message);
                    }
                }).fail(function () { FCH.DespliegaError(jsglb[system_lang].Usuario_could_not_delete_user); });
            }
        });
    },
    Clonar: function (id) {
        FCH.CierraMensajes();
        var url = contextPath + "Usuario/Clonar/" + id; // El url del controlador
        $.get(url, function (data) {
            $('#nuevo-Usuario').html(data);
            $('#nuevo-Usuario').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');

            FCH.RedefinirValidaciones(); //para los formularios dinamicos
            Usuario.activeForm = '#NuevoUsuarioForm';
            Usuario.CargarColeccionRoles();
            Usuario.CargarColeccionUsuarios();
        });

    },
    CargarColeccionRoles: function () {
        var form = Usuario.activeForm;
        if (Usuario.colRoles.length < 1) {
            var url = contextPath + "Rol/CargarRoles"; // El url del controlador
            $.getJSON(url, function (data) {
                Usuario.colRoles = data;
                Usuario.CargaListaRoles(form);
            }).fail(function () {
                FCH.DespliegaErrorDialogo(jsglb[system_lang].Seguridad_could_not_load_info_Roles);
            });
        } else {
            Usuario.CargaListaRoles(form);
        }
    },
    CargaListaRoles: function (form) {
        var select = $(form + ' #idRol').empty();

        select.append('<option value="0">ELIJA UN ROL...</option>');
        $.each(Usuario.colRoles, function (i, item) {
            select.append('<option value="' + item.id + '">' + item.nombre + '</option>');
        });

        $(form + " #idRol").select2({ allowClear: true });
        if (form !== undefined)
            $(form + ' #idRol').val($(form + ' #Rol').val()).change();


    },
    CargarColeccionUsuarios: function () {
        var form = Usuario.activeForm;
        if (Usuario.colUsuariosJefe.length < 1) {
            var url = contextPath + "Usuario/CargarUsuarios"; // El url del controlador
            $.getJSON(url, function (data) {
                Usuario.colUsuariosJefe = data.datos;
                Usuario.CargaListaUsuarios(form);
            }).fail(function () {
                FCH.DespliegaErrorDialogo(jsglb[system_lang].Proyecto_could_not_load_info_usuarios);
            });
        } else {
            Usuario.CargaListaUsuarios(form);
        }
    },
    CargaListaUsuarios: function (form) {
        var select = $(form + ' #idJefeUsuario').empty();

        select.append('<option value="">ELIJA UN JEFE DIRECTO...</option>');
        $.each(Usuario.colUsuariosJefe, function (i, item) {
            select.append('<option value="' + item.id + '">' + item.nombreCompleto + '</option>');
        });

        $(form + " #idJefeUsuario").select2({ allowClear: true });
        if (form !== undefined)
            $(form + ' #idJefeUsuario').val($(form + ' #JefeUsuario').val()).change();

    },
    ValidaPermisos: function () {
        var permisos = localStorage.modPermisos,
            modulo = Usuario;
        modulo.accEscritura = permisos.substr(1, 1) === '1' ? true : false;
        modulo.accBorrar = permisos.substr(2, 1) === '1' ? true : false;
        modulo.accClonar = permisos.substr(3, 1) === '1' ? true : false;

        if (modulo.accEscritura === true)
            $('.btnNuevo').show();
    },
    EventoNombreArchivo: function () {
        var form = Usuario.activeForm;
        //Se inicializan los eventos para el formulario
        $(form + ' .btn-file :file').on('fileselect', function (event, numFiles, label) {
            var input = $(this).parents('.input-group').find(':text'),
                log = numFiles > 1 ? numFiles + ' files selected' : label;

            if (input.length) {
                input.val(log);
            } else {
                if (log) console.log(log);
            }
        });

        $(document).on('change', '.btn-file :file', function () {
            var input = $(this),
                numFiles = input.get(0).files ? input.get(0).files.length : 1,
                label = input.val().replace(/\\/g, '/').replace(/.*\//, '');
            $('#imagenUsuario').val(label);
            input.trigger('fileselect', [numFiles, label]);
        });
    },
    serializaUsuario: function (id, form) {
        return ({
            'nombreCompleto': $(form + ' #nombresUsuario').val().toUpperCase() + ' ' + $(form + ' #apellidosUsuario').val().toUpperCase(),
            'email': $(form + ' #emailUsuario').val(),
            'telefono': $(form + ' #telefonoUsuario').val(),
            'celular': $(form + ' #celularUsuario').val(),
            'departamento': $(form + ' #departamentoUsuario').val().toUpperCase(),
            'nombreRol': $(form + ' #idRol option:selected').text().toUpperCase(),
            'lang': $(form + ' #lenguajeUsuario option:selected').val(),
            'id': id
        });
    },
    CargaGrid: function () {
        $('#cargandoInfo').show();
        var url = contextPath + "Usuario/CargarUsuarios"; // El url del controlador
        $.getJSON(url, function (data) {
            if (data.Success === false) { FCH.DespliegaError(data.Message); return; }
            Usuario.colUsuarios = new Backbone.Collection(data.datos);
            var bolFilter = Usuario.colUsuarios.length > 0 ? true : false;
            if (bolFilter) {
                Usuario.gridUsuarios = new bbGrid.View({
                    container: $('#bbGrid-clear'),
                    rows: 200,
                    rowList: [15, 50, 200, 1000],
                    enableSearch: true,
                    actionenable: true,
                    detalle: false,
                    clone: Usuario.accClonar,
                    editar: Usuario.accEscritura,
                    borrar: Usuario.accBorrar,
                    collection: Usuario.colUsuarios,
                    colModel: [//{ title: 'Id', name: 'id', width: '8%', sorttype: 'number', filter: true, filterType: 'input' },
                               { title: 'User', name: 'nombreCompleto', filter: true, filterType: 'input', index: true },
                                { title: 'Email', name: 'email', filter: true, filterType: 'input', index: true },
                                { title: 'Phone number', name: 'telefono', filter: true, filterType: 'input', index: true },
                                { title: 'Cel number', name: 'celular', filter: true, filterType: 'input', index: true },
                                { title: 'Department', name: 'departamento', filter: true, filterType: 'input', index: true },
                                { title: 'Role', name: 'nombreRol', filter: true, filterType: 'input', index: true },
                                { title: 'Language', name: 'lang', filter: true, filterType: 'input', index: true }]

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
    }
};

$(function () {
    Usuario.Inicial();
});