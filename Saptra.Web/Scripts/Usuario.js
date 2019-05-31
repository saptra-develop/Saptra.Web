/* global CHR, contextPath, bootbox, Backbone, bbGrid */

//js de catalogo de Usuarios.
//David Jasso
//10/Octubre/2018

var Usuario = {
    accClonar: false,
    accEscritura: false,
    accBorrar: false,
    activeForm: '',
    colUsuarios: {},
    colRoles: [],
    colRegiones: [],
    colZonas: [],
    colUsuariosJefe: [],
    gridUsuarios: {},
    validaRegion: 0,
    validaZona: 0,
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


        $(document).on("change", '#imgUsuAct', function () {
            if (this.files && this.files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    $('#imgUpl').attr('src', e.target.result);
                };
                reader.readAsDataURL(this.files[0]);
            }
        });

        $(document).on("change", '#RolId', function () {
            if ($(Usuario.activeForm + ' #RolId').val() == "6") {
                $(Usuario.activeForm + ' #divRegion').show();
                $(Usuario.activeForm + ' #divZona').hide();
                Usuario.CargarColeccionRegiones();
                //Usuario.ValidarJefeRegion();
            } else if ($(Usuario.activeForm + ' #RolId').val() == "3" || $(Usuario.activeForm + ' #RolId').val() == "2" || $(Usuario.activeForm + ' #RolId').val() == "4" || $(Usuario.activeForm + ' #RolId').val() == "5") {
                $(Usuario.activeForm + ' #divZona').show();
                Usuario.CargarColeccionZonas();
                $(Usuario.activeForm + ' #divRegion').hide();
            }
            else if ($(Usuario.activeForm + ' #RolId').val() == "7" || $(Usuario.activeForm + ' #RolId').val() == "16" || $(Usuario.activeForm + ' #RolId').val() == "1") {
                $(Usuario.activeForm + ' #divRegion').hide();
                $(Usuario.activeForm + ' #divZona').hide();
            } else {
                $(Usuario.activeForm + ' #divZona').show();
                Usuario.CargarColeccionZonas();
            }
        });

        $(document).on("change", '#RegionId', function () {
            validaRegion = Usuario.ValidarJefeRegion();
           
        });

        $(document).on("change", '#ZonaId', function () {
            validaZona = Usuario.ValidarJefeZona();

        });




    },
    ValidarJefeRegion: function () {
        var url = contextPath + "Usuario/ValidaJefeRegion?RegionId=" + $(Usuario.activeForm + ' #RegionId').val();
        $.getJSON(url, function (respuesta) {
            if (respuesta.Success === true) {

                Usuario.validaRegion = 1;
     
            }
            else {
                Usuario.validaRegion = 0;
            }
        });
    },
    ValidarJefeZona: function () {
        var url = contextPath + "Usuario/ValidaJefeZona?ZonaId=" + $(Usuario.activeForm + ' #ZonaId').val();
        $.getJSON(url, function (respuesta) {
            if (respuesta.Success === true) {
                Usuario.validaZona = 1;
            }
            else {
                Usuario.validaZona = 0;
            }
        });
    },
    onGuardaValidado: function (botn) {
        var btn = botn;
        var atpos = $('#NuevoUsuarioForm #EmailUsuario').val().indexOf("@");
        var dotpos = $('#NuevoUsuarioForm #EmailUsuario').val().lastIndexOf(".")

        //if (valida == 0) {
            if (atpos < 1 || (dotpos - atpos < 2)) {
                FCH.DespliegaErrorDialogo("Correo invalido");
            } else {
                if ($('#NuevoUsuarioForm #NombresUsuario').val() !== "" && $('#NuevoUsuarioForm #ApellidosUsuario').val() !== "" && $('#NuevoUsuarioForm #LoginUsuario').val() !== "" && $('#NuevoUsuarioForm #PasswordUsuario').val() !== "" && $('#NuevoUsuarioForm #EmailUsuario').val() !== "" && $('#NuevoUsuarioForm #EmailUsuario').val() !== "0") {
                    FCH.botonMensaje(true, btn, 'Guardar');
                    if ($("form").valid()) {
                        //$("#imagenUsuario").val('Hola');
                        //Se hace el post para guardar la informacion
                        $.post(contextPath + "Usuario/Nuevo?RegionId=" + ($('#NuevoUsuarioForm #RegionId').val() == 0 || $('#NuevoUsuarioForm #RegionId').val() == null ? 0 : $('#NuevoUsuarioForm #RegionId').val()) + "&ZonaId=" + ($('#NuevoUsuarioForm #ZonaId').val() == null || $('#NuevoUsuarioForm #ZonaId').val() == 0  ? 0 : $('#NuevoUsuarioForm #ZonaId').val()) + "&idUsuario=" + localStorage.idUser,
                            $("#NuevoUsuarioForm *").serialize(),
                            function (data) {
                                if (data.Success === true) {
                                    Usuario.colUsuarios.add(Usuario.serializaUsuario(data.id, '#NuevoUsuarioForm'));
                                    FCH.DespliegaInformacion("El Usuario fue guardado con el Id: " + data.id);
                                    $('#nuevo-Usuario').modal('hide');
                                    if (Usuario.colUsuarios.length === 1) {
                                        Usuario.CargaGrid();
                                    }
                                } else {
                                    FCH.botonMensaje(false, btn, 'Guardar');
                                    FCH.DespliegaErrorDialogo(data.Message);

                                }
                            }).fail(function () {
                                FCH.DespliegaErrorDialogo("Error al guardar la información.");
                                FCH.botonMensaje(false, btn, 'Guardar'); 

                            }).always(function () { FCH.botonMensaje(false, btn, 'Guardar'); });

                    } else {
                        FCH.botonMensaje(false, btn, 'Guardar');
                    }
                } else {
                    if ($('#NuevoUsuarioForm #NombresUsuario').val() == "") {
                        $('#NuevoUsuarioForm #NombresUsuario').addClass("input-validation-error");
                        $('#NuevoUsuarioForm #NombresUsuario').attr("placeholder", "Obligatorio");
                    }
                    if ($('#NuevoUsuarioForm #ApellidosUsuario').val() == "") {
                        $('#NuevoUsuarioForm #ApellidosUsuario').addClass("input-validation-error");
                        $('#NuevoUsuarioForm #ApellidosUsuario').attr("placeholder", "Obligatorio");
                    }
                    if ($('#NuevoUsuarioForm #LoginUsuario').val() == "") {
                        $('#NuevoUsuarioForm #LoginUsuario').addClass("input-validation-error");
                        $('#NuevoUsuarioForm #LoginUsuario').attr("placeholder", "Obligatorio");
                    }
                    if ($('#NuevoUsuarioForm #PasswordUsuario').val() == "") {
                        $('#NuevoUsuarioForm #PasswordUsuario').addClass("input-validation-error");
                        $('#NuevoUsuarioForm #PasswordUsuario').attr("placeholder", "Obligatorio");
                    }
                    if ($('#NuevoUsuarioForm #EmailUsuario').val() == "") {
                        $('#NuevoUsuarioForm #EmailUsuario').addClass("input-validation-error");
                        $('#NuevoUsuarioForm #EmailUsuario').attr("placeholder", "Obligatorio");
                    }
                    if ($('#NuevoUsuarioForm #RolId').val() == "0") {
                        $('#NuevoUsuarioForm #RolId').addClass("has-error");
                    }

                    if ($('#NuevoUsuarioForm #EmailUsuario').val() == "") {
                        $('#NuevoUsuarioForm #EmailUsuario').addClass("input-validation-error");
                        $('#NuevoUsuarioForm #EmailUsuario').attr("placeholder", "Obligatorio");
                    }
                    if ($('#NuevoUsuarioForm #ZonaId').val() == null) {
                        $('#NuevoUsuarioForm #ZonaId').addClass("has-error");
                    }
                    if ($('#NuevoUsuarioForm #RegionId').val() == "0") {
                        $('#NuevoUsuarioForm #RegionId').addClass("has-error");
                    }
                }
            }
        //}

    
        
    },
    onGuardar: function (botn) {
        var btn = botn;
        FCH.botonMensaje(true, btn, 'Guardar');
        var estado = 0;
        if ($(Usuario.activeForm + ' #RolId').val() == "6") {
            if ($(Usuario.activeForm + ' #RolId').val() == "6" && $('#NuevoUsuarioForm #RegionId').val() == "0") {
                estado = 1;
                FCH.DespliegaErrorDialogo("Es necesario elegir una Región");
                FCH.botonMensaje(false, btn, 'Guardar');
            } else {
                estado = 0;
            }
        }

        //if ($(Usuario.activeForm + ' #RolId').val() == "3") {

            if (($(Usuario.activeForm + ' #RolId').val() == "3" || $(Usuario.activeForm + ' #RolId').val() == "2" || $(Usuario.activeForm + ' #RolId').val() == "4" || $(Usuario.activeForm + ' #RolId').val() == "5") && $('#NuevoUsuarioForm #ZonaId').val() == "0") {
                estado = 1;
                FCH.DespliegaErrorDialogo("Es necesario elegir una Zona");
                FCH.botonMensaje(false, btn, 'Guardar');
            } else {
                estado = 0;
            }
        //}

        if (estado == 0) {
            var valida = 0;
            if ($(Usuario.activeForm + ' #RolId').val() == "6" && Usuario.validaRegion == 1) {
                bootbox.confirm("¿Desea eliminar al jefe de la coordinación de región actual?", function (result) {
                    if (result) {
                        Usuario.onGuardaValidado(btn);
                    } else {
                        valida = 1;
                        FCH.botonMensaje(false, btn, 'Guardar');
                    }
                });
            } else if ($(Usuario.activeForm + ' #RolId').val() == "3" && Usuario.validaZona == 1) {
                bootbox.confirm("¿Desea eliminar al jefe de la coordinación de zona actual?", function (result) {
                    if (result) {
                        Usuario.onGuardaValidado(btn);
                    } else {
                        valida = 1;
                        FCH.botonMensaje(false, btn, 'Guardar');
                    }
                });
            }
            else {
                Usuario.onGuardaValidado(btn);
            }
           
        } 

    },
    onActualizar: function (botn) {
        var btn = botn;
        FCH.botonMensaje(true, btn, 'Guardar');

        var estado = 0;
        if ($(Usuario.activeForm + ' #EstatusId').val() == "5") {
            if ($(Usuario.activeForm + ' #RolId').val() == "6") {
                if ($(Usuario.activeForm + ' #RolId').val() == "6" && $('#ActualizaUsuarioForm #RegionId').val() == "0") {
                    estado = 1;
                    FCH.DespliegaErrorDialogo("Es necesario elegir una Región");
                    FCH.botonMensaje(false, btn, 'Guardar');
                } else {
                    estado = 0;
                }
            }

   
            if (($(Usuario.activeForm + ' #RolId').val() == "3" || $(Usuario.activeForm + ' #RolId').val() == "2" || $(Usuario.activeForm + ' #RolId').val() == "4" || $(Usuario.activeForm + ' #RolId').val() == "5") && $('#ActualizaUsuarioForm #ZonaId').val() == "0") {
                estado = 1;
                FCH.DespliegaErrorDialogo("Es necesario elegir una Zona");
                FCH.botonMensaje(false, btn, 'Guardar');
            } else {
                estado = 0;
            }
        }

        if (estado == 0) {
            var valida = 0;
            if ($(Usuario.activeForm + ' #RolId').val() == "6" && Usuario.validaRegion == 1) {
                bootbox.confirm("¿Desea eliminar al jefe de la coordinación de región actual?", function (result) {
                    if (result) {
                        Usuario.onActualizarValidado(botn);
                    } else {
                        valida = 1;
                        FCH.botonMensaje(false, btn, 'Guardar');
                    }
                });
            } else if ($(Usuario.activeForm + ' #RolId').val() == "3" && Usuario.validaZona == 1) {
                bootbox.confirm("¿Desea eliminar al jefe de la coordinación de zona actual?", function (result) {
                    if (result) {
                        Usuario.onActualizarValidado(botn);
                    } else {
                        valida = 1;
                        FCH.botonMensaje(false, btn, 'Guardar');
                    }
                });
            }
            else {
                Usuario.onActualizarValidado(botn);
            }

        } 
        
    },
    onActualizarValidado: function (botn) {
        var btn = botn;

        var atpos = $('#ActualizaUsuarioForm #EmailUsuario').val().indexOf("@");
        var dotpos = $('#ActualizaUsuarioForm #EmailUsuario').val().lastIndexOf(".");

        if (atpos < 1 || (dotpos - atpos < 2)) {
            FCH.DespliegaErrorDialogo("Correo invalido");
        } else {
            if ($('#ActualizaUsuarioForm #NombresUsuario').val() !== "" && $('#ActualizaUsuarioForm #ApellidosUsuario').val() !== "" && $('#ActualizaUsuarioForm #LoginUsuario').val() !== "" && $('#ActualizaUsuarioForm #PasswordUsuario').val() !== "" && $('#ActualizaUsuarioForm #EmailUsuario').val() !== "" && $('#ActualizaUsuarioForm #EmailUsuario').val() !== "0") {
                FCH.botonMensaje(true, btn, 'Guardar');
                if ($("form").valid()) {
                    //Se hace el post para guardar la informacion
                    $.post(contextPath + "Usuario/Actualiza?RegionId=" + $('#ActualizaUsuarioForm #RegionId').val() + "&ZonaId=" + $('#ActualizaUsuarioForm #ZonaId').val() + "&idUsuario=" + localStorage.idUser,
                        $("#ActualizaUsuarioForm *").serialize(),
                        function (data) {
                            if (data.Success === true) {
                                $('#actualiza-Usuario').modal('hide');
                                Usuario.colUsuarios.add(Usuario.serializaUsuario(data.id, '#ActualizaUsuarioForm'), { merge: true });
                                FCH.DespliegaInformacion("El Usuario fue Actualizado. Id: " + data.id);
                            } else {
                                FCH.DespliegaErrorDialogo(data.Message);
                                FCH.botonMensaje(false, btn, 'Guardar');
                            }
                        }).fail(function () {
                            FCH.DespliegaErrorDialogo("Error al actualizar la información");
                        }).always(function () { FCH.botonMensaje(false, btn, 'Guardar'); });
                } else {
                    FCH.botonMensaje(false, btn, 'Guardar');
                }
            } else {
                if ($('#ActualizaUsuarioForm #NombresUsuario').val() == "") {
                    $('#ActualizaUsuarioForm #NombresUsuario').addClass("input-validation-error");
                    $('#ActualizaUsuarioForm #NombresUsuario').attr("placeholder", "Obligatorio");
                }
                if ($('#ActualizaUsuarioForm #ApellidosUsuario').val() == "") {
                    $('#ActualizaUsuarioForm #ApellidosUsuario').addClass("input-validation-error");
                    $('#ActualizaUsuarioForm #ApellidosUsuario').attr("placeholder", "Obligatorio");
                }
                if ($('#ActualizaUsuarioForm #LoginUsuario').val() == "") {
                    $('#ActualizaUsuarioForm #LoginUsuario').addClass("input-validation-error");
                    $('#ActualizaUsuarioForm #LoginUsuario').attr("placeholder", "Obligatorio");
                }
                if ($('#ActualizaUsuarioForm #PasswordUsuario').val() == "") {
                    $('#ActualizaUsuarioForm #PasswordUsuario').addClass("input-validation-error");
                    $('#ActualizaUsuarioForm #PasswordUsuario').attr("placeholder", "Obligatorio");
                }
                if ($('#ActualizaUsuarioForm #EmailUsuario').val() == "") {
                    $('#ActualizaUsuarioForm #EmailUsuario').addClass("input-validation-error");
                    $('#ActualizaUsuarioForm #EmailUsuario').attr("placeholder", "Obligatorio");
                }
                if ($('#ActualizaUsuarioForm #RolId').val() == "0") {
                    $('#ActualizaUsuarioForm #RolId').addClass("has-error");
                }

            }
        }
    },
    onSubirArchivo: function () {
        var filesName = Usuario.activeForm === '#NuevoUsuarioForm' ? 'imgUsu' : 'imgUsuAct',
            btn = this,
            form = Usuario.activeForm,
            files;

        if ($("form").valid()) {
            FCH.botonMensaje(true, btn, 'Guardar');
            files = document.getElementById(filesName).files;
            if (files.length > 0) {
                if (window.FormData !== undefined) {
                    var data = new FormData();
                    for (var count = 0; count < files.length; count++) {
                        data.append(files[count].name, files[count]);
                    }
                    $.ajax({
                        type: "POST",
                        url: contextPath + 'Usuario/SubirArchivo', //contextPath + '/Usuario/SubirArchivo', para produccion
                        contentType: false,
                        processData: false,
                        data: data,
                        success: function (result) {

                            if (result.Success === true) {
                                $(Usuario.activeForm + ' #ImagenUsuario').val(result.Archivo);
                                localStorage.UserAvatar = result.Archivo;
                                $('#UserAvatarLayout').attr('src', localStorage.UserAvatar);
                                $('#UserAvatarMenu').attr('src', localStorage.UserAvatar);
                                $('#dropdownMenuUsuario').attr('src', localStorage.UserAvatar);
                                if (Usuario.activeForm !== '#NuevoUsuarioForm') {
                                    Usuario.onActualizar(btn);
                                } else {
                                    Usuario.onGuardar(btn);
                                }

                            } else {
                                $(Usuario.activeForm + ' #ImagenUsuario').val('');
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
                    FCH.DespliegaErrorDialogo("Este explorador no soportado por la aplicacion favor de utilizar una version mas reciente. Chrome");
                    FCH.botonMensaje(false, btn, 'Guardar');
                }
            } else {
                if (Usuario.activeForm !== '#NuevoUsuarioForm') {
                    Usuario.onActualizar(btn);
                } else {
                    $(Usuario.activeForm + ' #ImagenUsuario').val('');
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
            //Usuario.CargarColeccionRegiones();
            //Usuario.CargarColeccionZonas();
            Usuario.CargarColeccionRoles();
            //Usuario.CargarColeccionUsuarios();
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
   
            if ($('#ActualizaUsuarioForm #RolId').val() == 6) {
                Usuario.CargarColeccionRegiones();
                $(Usuario.activeForm + ' #divRegion').show();
            } else if ($('#ActualizaUsuarioForm #RolId').val() == 3 || $('#ActualizaUsuarioForm #RolId').val() == 2 || $('#ActualizaUsuarioForm #RolId').val() == 4 || $('#ActualizaUsuarioForm #RolId').val() == 5  ) {
                Usuario.CargarColeccionZonas();
             
                $(Usuario.activeForm + ' #divZona').show();
            }
            Usuario.CargarColeccionRoles();
            //Usuario.CargarColeccionUsuarios();
            Usuario.EventoNombreArchivo();

        });
    },
    Borrar: function (id) {
        FCH.CierraMensajes();

        //bootbox.confirm("¿Está seguro que desea borrar este registro?", function (result) {
            
        //    if (result) {
        //        var url = contextPath + "Usuario/Borrar"; // El url del controlador
        //        $.post(url, { id: id }, function (data) {
        //            if (data.Success === true) {
        //                Usuario.colUsuarios.remove(id);
        //                FCH.DespliegaInformacion(data.Message + "  id:" + id);
        //            } else {
        //                FCH.DespliegaError(data.Message);
        //            }
        //        }).fail(function () { FCH.DespliegaError("No se pudo eliminar el Usuario",); });
        //    }
        //});
        bootbox.setDefaults({
            locale: "es"
        });
        bootbox.confirm({
            message: "¿Está seguro que desea borrar este registro?",
         
            callback: function (result) {
                if (result) {
                    var url = contextPath + "Usuario/Borrar"; // El url del controlador
                    $.post(url, { id: id }, function (data) {
                        if (data.Success === true) {
                            Usuario.colUsuarios.remove(id);
                            FCH.DespliegaInformacion(data.Message + "  id:" + id);
                        } else {
                            FCH.DespliegaError(data.Message);
                        }
                    }).fail(function () { FCH.DespliegaError("No se pudo eliminar el Usuario"); });
                } }
        })
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
            //Usuario.CargarColeccionUsuarios();
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
                FCH.DespliegaErrorDialogo("No se pudo cargar la informacion de los roles ");
            });
        } else {
            Usuario.CargaListaRoles(form);
        }
    },
    CargaListaRoles: function (form) {
        var select = $(form + ' #RolId').empty();

        select.append('<option value="0">ELIJA UN ROL...</option>');
        $.each(Usuario.colRoles, function (i, item) {
            select.append('<option value="' + item.id + '">' + item.nombre + '</option>');
        });

        $(form + " #RolId").select2({ allowClear: true });
        if (form !== undefined)
            $(form + ' #RolId').val($(form + ' #Rol').val()).change();


    },
    CargarColeccionRegiones: function () {
        var form = Usuario.activeForm;
        if (Usuario.colRegiones.length < 1) {
            var url = contextPath + "Regiones/CargarRegiones"; // El url del controlador
            $.getJSON(url, function (data) {
                Usuario.colRegiones = data;
                Usuario.CargaListaRegiones(form);
            }).fail(function () {
                FCH.DespliegaErrorDialogo("No se pudo cargar la informacion de regiones ");
            });
        } else {
            Usuario.CargaListaRegiones(form);
        }
    },
    CargaListaRegiones: function (form) {
        var select = $(form + ' #RegionId').empty();

        select.append('<option value="0">ELIJA UNA REGION...</option>');
        $.each(Usuario.colRegiones, function (i, item) {
            select.append('<option value="' + item.id + '">' + item.nombre + '</option>');
        });


        $(Usuario.activeForm + " #RegionId").select2({ allowClear: true });
        if ($(Usuario.activeForm + " #selRegionId").val() !== "0")
            $(Usuario.activeForm + ' #RegionId').val($(Usuario.activeForm + " #selRegionId").val()).change();

      


    },
    CargarColeccionZonas: function () {
        var form = Usuario.activeForm;

        var url = contextPath + "Zonas/CargarZonasRegion"; // El url del controlador
        $.getJSON(url, function (data) {
            Usuario.colZonas = data;
            Usuario.CargaListaZonas(form);
        }).fail(function () {
            FCH.DespliegaErrorDialogo("No se pudo cargar la informacion de zonas ");
        });

    },
    CargaListaZonas: function (form) {
        var select = $(form + ' #ZonaId').empty();

        select.append('<option value="0">ELIJA UNA ZONA...</option>');
        $.each(Usuario.colZonas, function (i, item) {
            select.append('<option value="' + item.id + '">' + item.nombre + '</option>');
        });

        $(form + " #ZonaId").select2({ allowClear: true });
        if ($(Usuario.activeForm + " #selZonaId").val() !== "0")
            $(Usuario.activeForm + ' #ZonaId').val($(Usuario.activeForm + " #selZonaId").val()).change();


    },
    CargarColeccionUsuarios: function () {
        var form = Usuario.activeForm;
        if (Usuario.colUsuariosJefe.length < 1) {
            var url = contextPath + "Usuario/CargarUsuarios"; // El url del controlador
            $.getJSON(url, function (data) {
                Usuario.colUsuariosJefe = data.datos;
                Usuario.CargaListaUsuarios(form);
            }).fail(function () {
                FCH.DespliegaErrorDialogo("No se pudo cargar la informacion de los usuarios");
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
            'nombreCompleto': $(form + ' #NombresUsuario').val().toUpperCase() + ' ' + $(form + ' #ApellidosUsuario').val().toUpperCase(),
            'email': $(form + ' #EmailUsuario').val(),
            'nombreRol': $(form + ' #RolId option:selected').text().toUpperCase(),
            'NombreEstatus': $(form + ' #EstatusId option:selected').text().toUpperCase(),
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
                    clone: false,
                    editar: Usuario.accEscritura,
                    borrar: Usuario.accBorrar,
                    collection: Usuario.colUsuarios,
                    colModel: [//{ title: 'Id', name: 'id', width: '8%', sorttype: 'number', filter: true, filterType: 'input' },
                                { title: 'Usuario', name: 'nombreCompleto', filter: true, filterType: 'input', index: true },
                                { title: 'Correo', name: 'email', filter: true, filterType: 'input', index: true },
                                { title: 'Rol', name: 'nombreRol', filter: true, index: true },
                                { title: 'Estatus', name: 'NombreEstatus', filter: true, filterType: 'input', index: true }]

                });
                $('#cargandoInfo').hide();
            } else {
                FCH.DespliegaInformacion("No se encontraron Usuarios registrados");
                $('#bbGrid-clear')[0].innerHTML = "";
            }

            //getJSON fail
        }).fail(function (e) {
            FCH.DespliegaError("No se pudo cargar la informacion de los usuarios");
        });
    }
};

$(function () {
    Usuario.Inicial();
});
