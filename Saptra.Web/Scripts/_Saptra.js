
/*global $, jQuery, FCH, Inicio*/
var FCH = {
    theadTables: [],
    colManuales: [],
    scrollTop: 0,
    scrollRight: 0,
    colElements: {},
    urlImagePopup: {},
    inicial: function (lang) {
        try {
            if (Inicio !== undefined) {
                Inicio.CargarLocalStorage();
            }
        } catch (exp) {
        }        
        //FCH.asignaIdioma(lang);
        FCH.eventos();
        FCH.CargaSideBar();
        //if (localStorage.getItem("idProveedor") !== null) {
        //    $('#mProyectos').hide();
        //    $('#menuProyectos').hide();
        //    $('#proyectoSelected').hide();
        //}
        //else {            
        //    FCH.cargarProyectos();
        //    FCH.cargarProyectoSelected();
        //}
        FCH.cargarUsuario();
        FCH.cargarNotificaciones();
        $('#userName').text(localStorage.UserName);
        $('#UserAvatarLayout').attr('src', localStorage.UserAvatar);    
        $('#UserAvatarLayout').css('display', 'inline-block');       
    },
    eventos: function () {
        //Eventos generales
        //$('#acercade').click(FCH.onAcercaDe);
        $('#cambiaEN').click(function () {
            FCH.cambiaIdioma('');
        });
        $('#cambiaES').click(function () {
            FCH.cambiaIdioma('es');
        });        
        //$(document).on("click", '.downloadManual', FCH.seleccionarManual);
        //$(document).on("click", '.btnProjectSelect', FCH.asignaProyectoSelected);
        $(document).on("click", '#btn-actualizaUsuario', FCH.ActualizarPerfilUSuario);
        $(document).on("click", '#btn-cambiarPassword', FCH.ActualizaPassword);
        $(document).on("click", '#btn-ActualizarPerfilUsuario', FCH.onActualizarPerfilUSuario);
        $(document).on("click", '#btn-ActualizarPassword', FCH.onActualizarPassword);
        $(document).on("click", '#acercade', FCH.onAcercaDe);

        $(document).on("change", '#imgUsuMenu', function () {
            if (this.files && this.files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    $('#imgUplMenu').attr('src', e.target.result);
                };
                reader.readAsDataURL(this.files[0]);
            }
        });

        $(document).on('click', '.ahrefDropdownMenuOnNavBar', FCH.onDocumentClick);
        $(document).on('click', FCH.onDocumentClick);

        $(document).on('click', '.ahrefDropdownNotificacionesOnNavBar', FCH.onNotificacionesClick);

        $('#layoutDefaultModal').on('hide.bs.modal', function () {
            FCH.BackToItem();//$('body').animate({ scrollTop: FCH.scrollTop }, 400);
        });

        $(document).on('click', '.btnNotificacion', function () {
            localStorage["IdPlan"] = $(this).data('idplan');
        }); 
    },
    //seleccionarManual: function () {
    //    FCH.CierraMensajes();
    //    var url = contextPath + "Principal/SeleccionarManual"; // El url del controlador      
    //    $.get(url, function (data) {
    //        $('#selecciona-Manual').html(data);
    //        $('#selecciona-Manual').modal({
    //            backdrop: 'static',
    //            keyboard: true
    //        }, 'show');
    //        FCH.CargaColeccionManuales();
    //    });        
    //},
    //CargaColeccionManuales: function () {
    //    var url = contextPath + "Principal/CargaManuales";
    //    //var div = $("divListaManuales");
    //    $.getJSON(url, function (data) {
    //        if (data.Success) {
    //            FCH.colManuales = data;
    //            FCH.CargaListaManuales();
    //        }
    //        else {
    //            FCH.DespliegaErrorDialogo(data.Message);
    //        }
    //    }).fail(function () {
    //        FCH.DespliegaErrorDialogo('Could not load owner´s manuals');
    //    });
    //},
    //CargaListaManuales: function () {
    //    var select = $('#divListaManuales').empty();
        
    //    for (var i = 0, len = FCH.colManuales.data.length; i < len; i++) {
    //        select.append("<p><a href='" + FCH.colManuales.data[i].ruta + "' target='_blank'>" + FCH.colManuales.data[i].nombre) + "</a></p>";
    //    }
    //},
    BackToItem: function () {
        $('html,body').animate({
            scrollTop: FCH.scrollTop
        }, 400);
    },
    makeTheadTable: function (id) {
        FCH.theadTables.push(id);
        $('#' + id).floatThead({ top: 52 });
        FCH.ReflowTheads();
    },
    onDocumentClick: function () {
        try {
            if (FCH.theadTables.length > 0) {
                if ($('#menuUsuario').is(':visible')) {
                    $.each(FCH.theadTables, function (i, item) {
                        $('#' + item).floatThead('destroy');
                    });
                } else {
                    $.each(FCH.theadTables, function (i, item) {
                        $('#' + item).floatThead({ top: 52 });
                    });
                }
            }
        } catch (exp) {
        }
    },
    onNotificacionesClick: function () {
        //$('#menuNotificaciones').toggle("slide");
    },
    //asignaProyectoSelected: function () {
    //    var idProyecto = $(this).attr('data-idproyecto'),
    //        nombreProyecto = $(this).attr('data-nombreproyecto'),
    //        idSector = $(this).attr('data-sector'),
    //        esFutbol = $(this).attr('data-soccer'),
    //        idMarca = $(this).attr('data-marca');

    //    localStorage.idProyectoSelected = idProyecto;
    //    localStorage.nombreProyectoSelected = nombreProyecto;
    //    localStorage.idSector = idSector;
    //    localStorage.esFutbol = esFutbol;
    //    localStorage.idMarca = idMarca;
        
    //    var url = contextPath + "Usuario/ObtenerJefe?idUsuario=" + localStorage.idUser + "&idProyecto=" + localStorage.idProyectoSelected;
    //    $.getJSON(url, function (data) {
    //        if (data.Success === true) {
    //            if (data.idUsuarioJefe != localStorage.idUser) {
    //                localStorage.idJefe = data.idUsuarioJefe;
    //            }
    //        }
    //    }).always(function () {
    //        location.reload(true);
    //    });

    //    var url = contextPath + "PlanPortafolio/VerificaMatriz?idProyecto=" + localStorage.idProyectoSelected;
    //    $.getJSON(url, function (data) {
    //        localStorage.matrizLiberada = data;         
    //    });
        
    //},
    //cargarProyectos: function (){
    //    FCH.CierraMensajes();
    //    var url = contextPath + "Proyecto/CargaMenuProyecto?idUsuario=" + localStorage.idUser; // El url del controlador 
    //    $.get(url, function (data) {
    //        if (data == "") $('#menuProyectos').html("You don´t have projects");
    //        $('#menuProyectos').html(data);
    //        FCH.CargaNotificacionesRevisionesPendientes();
    //        FCH.CargaNotificacionesEstilosPendientes();
    //    });
    //},
    cargarUsuario: function () {
        FCH.CierraMensajes();
        var url = contextPath + "Usuario/CargaMenuUsuario/" + localStorage.idUser; // El url del controlador 
        $.get(url, function (data) {
            $('#menuUsuario').html(data);
            //FCH.asignaIdioma(localStorage.lang);
        });
        
    },
    cargarNotificaciones: function () {
        FCH.CierraMensajes();
        var url = contextPath + "Notificaciones/CargaNotificaciones?idUsuario=" + localStorage.idUser; // El url del controlador 
        $.get(url, function (data) {
            $('#menuNotificaciones').html(data);
            if ($('#cantNotificaciones').val() > 0) {
                $('#divNotifi').show();
                $('#cantidadNotificaciones').text($('#cantNotificaciones').val());
                $('#cNotificaciones').text('Tienes ' + $('#cantNotificaciones').val() + ' Notificaciones');
            } else {
                $('#divNotifi').hide();
                $('#cNotificaciones').text('Sin Notificaciones');
            }
        });

    },
    //CargaNotificacionesRevisionesPendientes: function () {
    //    var idUsuario = (localStorage.getItem("idJefe") !== null ? localStorage.idJefe : localStorage.idUser);
    //    $.each($('.lblReviews'), function (i, item) {
    //        var idProyecto = $(item).data('idproyecto');
    //        var url = contextPath + "RevisionProducto/CargaRevisionesPendientesxProyecto?idUsuario=" + idUsuario +
    //                                "&idProyecto=" + idProyecto;
    //        $.getJSON(url, function (respuesta) {
    //            if (respuesta.Success === true) {
    //                if (respuesta.pendientes > 0) {
    //                    $(item).html(respuesta.pendientes + ' Reviews');
    //                } else {
    //                    $(item).hide();
    //                }
    //            }
    //        });
    //    });
    //},
    //CargaNotificacionesEstilosPendientes: function () {
    //    var idUsuario = (localStorage.getItem("idJefe") !== null ? localStorage.idJefe : localStorage.idUser);
    //    $.each($('.lblStylesReview'), function (i, item) {
    //        var idProyecto = $(item).data('idproyecto');
    //        var url = contextPath + "Producto/CargaEstilosParaRevision?idProyecto=" + idProyecto + "&idUsuario=" + idUsuario;
    //        $.get(url, function (data) {
    //            if (data.cantidad > 0) {
    //                $(item).html(data.cantidad + ' Styles');
    //            } else {
    //                $(item).hide();
    //            }
    //        });
    //    });

    //    //Verifica los proyectos que están vacíos para eliminarlos
    //    $('.collapseMarca').each(function (index, value) {
    //        if ($.trim($("#" + $(this).attr('id')).html()) == '')
    //            $($("#accordionMarca_" + $(this).attr('id').slice(-2))).hide();
    //    });
    //},
    //cargarProyectoSelected: function () {
    //    if (localStorage.idProyectoSelected !== undefined) {
    //        $('#proyectoSelected').html(localStorage.nombreProyectoSelected);
    //    }
    //},
    //asignaIdioma: function (lang) {
    //    //Se agrega en el URL el idioma seleccionado
    //    localStorage.lang = lang;
    //    if (localStorage.lang !== '') {
    //        contextPath = contextPath + lang + "/";
    //        bootbox.setDefaults({ locale: "es" });
    //        system_lang = 'es';
    //    }
    //    else {
    //        bootbox.setDefaults({ locale: "en" });
    //        system_lang = 'en';
    //    }
    //    $('#language').removeClass('fa-check-circle');
    //    $('#languagees').removeClass('fa-check-circle');
    //    $('#language' + lang).addClass('fa-check-circle');
    //},
    //cambiaIdioma: function (lang) {
    //    if (lang !== localStorage.lang) {
    //        var lenguaje = lang === "" ? "en" : "es";
    //        var url = contextPath + "Usuario/CambiarLenguaje?idUsuario=" + localStorage.idUser + "&lenguaje=" + lenguaje;
    //        $.post(url, null, function (data) {
    //            if (data.Success === true) {
    //                var host = $(location).attr('host');
    //                var path = window.location.pathname;
    //                path = lang !== "" ? '/es' + path : path.substring(3, path.length);

    //                $(location).attr('href', 'http://' + host + path);
    //            }
    //        });
    //    }
    //},
    menuPrincipal: function () {
        if ($('#navBarMainTop').is(':visible') === true) {
            if ($('#divSideBar').is(":visible") === true) {
                $('#divSideBar').hide();
                $('#page-wrapper').animate({
                    marginLeft: "0"
                }, 75, function () {
                    //
                    FCH.ReflowTheads();
                });
            } else {
                $('#page-wrapper').animate({
                    marginLeft: "250px"
                }, 75, function () {
                    $('#divSideBar').show();
                    FCH.ReflowTheads();
                });
            }
        } else {
            $('#divSideBar').hide();
            $('#page-wrapper').attr('style', 'margin: 0 0 0 0;');
            FCH.ReflowTheads();
        }
        
    },   
    ReflowTheads: function () {
        try {
            if (FCH.theadTables.length > 0) {
                $.each(FCH.theadTables, function (i, item) {
                    $('#' + item).floatThead('reflow');
                });
            }
        } catch (exp) {
        }
    },
    onAcercaDe: function () {
        //bootbox.alert("<h4>Grupo CHARLY</h4> <h6>Derechos reservados</h6>", function () { });
        var htmlMessage = "<h4>INAEBA</h4>";
        htmlMessage += "<h5>Todos los derechos reservados <i class='fa fa-copyright'></i> " + moment().format('YYYY') + "</h5>";
        htmlMessage += "<div class='text-center'><div class='btn-group'><a href='https://www.youtube.com/channel/UCY2hK2O9lDw8JMn_MwIL4oA' target='_blank' class='btn btn-default'><i class='fa fa-youtube'></i></a>";
        htmlMessage += "<a href='https://www.facebook.com/soyinaeba/' target='_blank' class='btn btn-default'><i class='fa fa-facebook'></i></a>";
        htmlMessage += "<a href='https://twitter.com/soyinaeba' target='_blank' class='btn btn-default'><i class='fa fa-twitter-square'></i></a>";
        htmlMessage += "</div></div>";
        htmlMessage += "<div class='text-center'><h6><a href='http://www.inaeba.guanajuato.gob.mx/inaeba/index.php' target='_blank'>inaeba.guanajuato.gob.mx</a></h6></div>";
        
        bootbox.alert({
            message: htmlMessage,
            size: 'small'
        });
    },
    CargaSideBar: function () {
        if (localStorage.idUser === '' || localStorage.idUser === undefined) return;
        var urlEventos = contextPath + "Principal/_sideBar?id=" + localStorage.idUser + "&lang=" + localStorage.lang + "&type=default";
        $.get(urlEventos, function (data) {
            if (data.substring(0, 5) === '<!DOC') return;
            $('#divSideBar').html(data);
            $('.side-menu').metisMenu();
            FCH.SeleccionaOpcionMenu();
        });
    },
    ReiniciaLocalStorage: function (pUsuario, pNombre, pProveedor) {
        localStorage.setItem("idUser", pUsuario);
        localStorage.setItem("UserName", pNombre);
        if ($('#idProveedorAsignadoEnLogin').length) localStorage.setItem("idProveedor", pProveedor);
    },
    AsignarAvatarLocalStorage: function (pAvatar) {
        localStorage.setItem("UserAvatar", pAvatar);
    },
    SeleccionaOpcionMenu: function () {
        var url = window.location;
        var element = $('ul.nav a').filter(function () {
            return this.href === url || url.href.indexOf(this.href) === 0;
        }).addClass('active').parent().parent().addClass('in').parent();
        if (element.is('li')) {
            element.addClass('active');
        }
        return (element);
    },
    setPermisos: function (datos) {
        localStorage.modPermisos = datos;
        var item = document.getElementById('Permisos');
        if (item !== null) {
            localStorage.modSerdad = item.textContent;
        } else {
            localStorage.modSerdad = '00';
        }
    },
    ValidaFecha: function (pDate) {
        //yyyy-mm-dd
        var reg = /^\d{4}-((0\d)|(1[012]))-(([012]\d)|3[01])$/;
        return reg.test(pDate);
    },
    ValidaFechaGuiones: function (pDate) {
        //yyyy/mm/dd
        var reg = /^\d{4}\/((0\d)|(1[012]))\/(([012]\d)|3[01])$/;
        return reg.test(pDate);
    },
    ValidaCorreo: function (pCorreo) {
        //name@domain.com
        var reg = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,4})+$/;
        return reg.test(pCorreo);
    },
    ValidaMoneda: function (pValor) {
        var numNum = +pValor;
        if (isNaN(numNum)) return false;

        return true;
    },
    toFixed: function(num, fixed) {
        fixed = fixed || 0;
        fixed = Math.pow(10, fixed);
        return Math.floor(num * fixed) / fixed;
    },
    redondearDecimal: function(num, fixed) {
        return parseFloat(num).toFixed(fixed);
    },
    darFormatoNumerico: function (num, decimales) {
        return num.toLocaleString('en-US', { minimumFractionDigits: decimales, maximumFractionDigits: decimales });
    },
    RedefinirValidaciones: function () {
        $("form").removeData("validator");
        $("form").removeData("unobtrusiveValidation");
        $.validator.unobtrusive.parse("form");
        $("html, body").animate({ scrollTop: 0 }, '500', 'swing');
    },
    CierraMensajes: function () {
        $(".clientAlert").html('');
        $(".clientAlertDlg").html('');
    },
    DespliegaError: function (error) {
        $(".clientAlert").html("<div class='alert alert-danger'>" +
                                "<button type='button' class='close' data-dismiss='alert'>x</button>" +
                                  error + "</div>");
        $("html, body").animate({ scrollTop: 0 }, '500', 'swing');
        $('#cargandoInfo').hide();
    },
    DespliegaInformacion: function (msg) {
        $(".clientAlert").html("<div class='alert alert-info'>" +
                                "<button type='button' class='close' data-dismiss='alert'>x</button>" +
                                  msg + "</div>");
        $("html, body").animate({ scrollTop: 0 }, '500', 'swing');
        $('#cargandoInfo').hide();
    },
    DespliegaErrorDialogo: function (error) {
        $(".clientAlertDlg").html("<div id='divMessage' class='alert alert-danger'>" +
                               "<button type='button' class='close' data-dismiss='alert'>x</button>" +
                                 error + "</div>");
        $("html, body").animate({ scrollTop: 0 }, '500', 'swing');
        $('#cargandoInfo').hide();
    },
    DespliegaInformacionDialogo: function (msg) {
        $(".clientAlertDlg").html("<div id='divMessage' class='alert alert-info'>" +
                               "<button type='button' class='close' data-dismiss='alert'>x</button>" +
                                 msg + "</div>");
        $("html, body").animate({ scrollTop: 0 }, '500', 'swing');
        $('#cargandoInfo').hide();
    },
    DespliegaWarning: function (msg) {
        $(".clientAlert").html("<div class='alert alert-warning'>" +
                                "<button type='button' class='close' data-dismiss='alert'>x</button>" +
                                  msg + "</div>");
        $("html, body").animate({ scrollTop: 0 }, '500', 'swing');
        $('#cargandoInfo').hide();
    },
    DespliegaWarningDialogo: function (msg) {
        $(".clientAlertDlg").html("<div class='alert alert-warning'>" +
                                "<button type='button' class='close' data-dismiss='alert'>x</button>" +
                                  msg + "</div>");
        $("html, body").animate({ scrollTop: 0 }, '500', 'swing');
        $('#cargandoInfo').hide();
    },
    DespliegaNotificacion: function (type, title, message, icon, delay ) {
        $.notify({
            icon: 'glyphicon ' + icon,
            title: title,
            message: message
        }, {
            type: type,
            placement: {
                from: "top",
                align: "right"
            },
            offset: 20,
            spacing: 10,
            z_index: 1031,
            delay: delay,
            timer: 1000,
            animate: {
                enter: 'animated fadeInDown',
                exit: 'animated fadeOutUp'
            },
            template: '<div data-notify="container" class="col-xs-11 col-sm-3 alert alert-{0}" role="alert">' +
                '<button type="button" aria-hidden="true" class="close" data-notify="dismiss">×</button>' +
                '<span data-notify="icon"></span> ' +
                '<span data-notify="title"><b>{1}</b></span> ' +
                '<span data-notify="message">{2}</span>' +
                '<div class="progress" data-notify="progressbar">' +
                    '<div class="progress-bar progress-bar-{0}" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%;"></div>' +
                '</div>' +
            '</div>'
        });
    },
    DespliegaSuccessDialogo: function (msg) {
        $(".clientAlertDlg").html("<div id='divMessage' class='alert alert-success'>" +
                               "<button type='button' class='close' data-dismiss='alert'>x</button>" +
                                 msg + "</div>");
        $("html, body").animate({ scrollTop: 0 }, '500', 'swing');
        $('#cargandoInfo').hide();
    },
    MuestraFechaActual: function () {
        return moment().format('D/M/YYYY');
    },
    botonMensaje: function (agregar, boton, nombre) {
        if (agregar === true) {
            $(boton).attr("disabled", "disabled");
            $(boton).html("<span class='glyphicon glyphicon-refresh glyphicon-refresh-animate'></span> " + nombre);
        } else {
            $(boton).html(nombre);
            $(boton).removeAttr("disabled");
        }
    },
    botonMensajeIcono: function (agregar, boton, nombre, icono) {
        if (agregar === true) {
            $(boton).attr("disabled", "disabled");
            $(boton).html("<span class='glyphicon glyphicon-refresh glyphicon-refresh-animate'></span> " + nombre);
        } else {
            $(boton).html("<i class='fa " + icono + "'></i> " + nombre);
            $(boton).removeAttr("disabled");
        }
    },
    ReadAllProjects: function (e) {
        e.preventDefault();
        $('.readAllLi').hide();
        $('.hiddenLi').show();
        $(document).on('click', '#menuProyectos', function (e) {
            e.stopPropagation();
        });
    },
    slidePanel: function (btn, panel) {
        if ($('#' + panel + ':visible').length == 0) {
            $('#' + panel).slideDown(function () {
                FCH.ReflowTheads();
            });
            $(btn).find('i').removeClass('fa-angle-double-down');
            $(btn).find('i').addClass('fa-angle-double-up');
        } else {
            $('#' + panel).slideUp(function () {
                FCH.ReflowTheads();
            });
            $(btn).find('i').removeClass('fa-angle-double-up');
            $(btn).find('i').addClass('fa-angle-double-down');
        }
    },
    ActualizarPerfilUSuario : function () {
        FCH.CierraMensajes();
        var url = contextPath + "Usuario/ActualizaPerfil/" + localStorage.idUser; // El url del controlador
        $.get(url, function (data) {
            $('#actualiza-PerfilUsuario').html(data);
            $('#actualiza-PerfilUsuario').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');
        });
    },
    ActualizaPassword: function () {
        FCH.CierraMensajes();
        var url = contextPath + "Usuario/ActualizaPassword/" + localStorage.idUser; // El url del controlador
        $.get(url, function (data) {
            $('#actualiza-password').html(data);
            $('#actualiza-password').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');
        });
    },
    onActualizarPerfilUSuario: function () {
        var filesName = 'imgUsuMenu',
            btn = this,
            form = '#ActualizaPerfilUsuarioForm',
            btnName = $(this).innerText,
            files;
        if ($('#ActualizaPerfilUsuarioForm #EmailUsuario').val() !== "") {
            if ($("form").valid()) {
                FCH.botonMensaje(true, btn, btnName);
                files = document.getElementById(filesName).files;
                if (files.length > 0) {
                    if (window.FormData !== undefined) {
                        var data = new FormData();
                        for (var count = 0; count < files.length; count++) {
                            data.append(files[count].name, files[count]);
                        }
                        $.ajax({
                            type: "POST",
                            url: contextPath + "Usuario/SubirArchivo", //contextPath + '/Usuario/SubirArchivo, para produccion
                            contentType: false,
                            processData: false,
                            data: data,
                            success: function (result) {
                                if (result.Success === true) {
                                    $(form + ' #ImagenUsuario').val(result.Archivo);
                                    FCH.GuardaDatosPerfilUsuario();
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
                        FCH.botonMensaje(false, btn, btnName);
                    }
                } else {
                    FCH.GuardaDatosPerfilUsuario();
                }
            }
        } else {

            $('#ActualizaPerfilUsuarioForm #EmailUsuario').addClass("input-validation-error");
            $('#ActualizaPerfilUsuarioForm #EmailUsuario').attr("placeholder", "Obligatorio");
        }
    },
    GuardaDatosPerfilUsuario: function () {
        $.post(contextPath + "Usuario/ActualizaPerfil",
            $("#ActualizaPerfilUsuarioForm *").serialize(),
            function (data) {
                if (data.Success === true) {
                    $('#actualiza-PerfilUsuario').modal('hide');
                    $('#emailUsuarioMenu').text(data.UsuarioPerfil[0].emailUsuario);
                    localStorage.UserAvatar = data.UsuarioPerfil[0].imagenUsuario;
                    $('#UserAvatarLayout').attr('src', localStorage.UserAvatar);
                    $('#UserAvatarMenu').attr('src', localStorage.UserAvatar);
                    $('#dropdownMenuUsuario').attr('src', localStorage.UserAvatar);
                    FCH.DespliegaInformacion("Se actualizo correctamente tú perfil");
                } else {
                    FCH.DespliegaErrorDialogo(data.Message);
                }
            }).fail(function () {
                FCH.DespliegaErrorDialogo("Error al actualizar la información");
            }).always(function () { });
    },
    onActualizarPassword: function () {
        var btn = this;
        if ($("form").valid()) {
        FCH.botonMensaje(true, btn, 'Guardar');
            //Se hace el post para guardar la informacion
            $.post(contextPath + "Usuario/ActualizaPassword?idUser=" + localStorage.idUser + "&passwordActual=" + $('#passwordActual').val() + "&passwordNuevo=" + $('#passwordNuevo').val() + "&passwordConfirmar=" + $('#passwordConfirmar').val(),
                null,
                function (data) {
                    if (data.Success === true) {
                        $('#actualiza-password').modal('hide');
                        FCH.DespliegaInformacion(data.Message);
                    } else {
                        FCH.DespliegaErrorDialogo(data.Message);
                        if (data.tipo == 1) {
                            $('#passwordNuevo').addClass('input-validation-error');
                            $('#passwordConfirmar').addClass('input-validation-error');
                        } else if (data.tipo == 2){
                            $('#passwordActual').addClass('input-validation-error');
                        }
                    }
                }).fail(function () {
                    FCH.DespliegaErrorDialogo("Error al actualizar la información");
                }).always(function () { FCH.botonMensaje(false, btn, 'Guardar'); });
        } else {
            FCH.botonMensaje(false, btn, 'Guardar');
        }
    },
    goBack: function(){
        window.history.back();
    },
    downloadExcel: function(id) {
        window.open('data:application/vnd.ms-excel,' + encodeURIComponent($('#' + id).html()));
    },
    verPopupImagen: function(col, url, nav) {
        FCH.colElements = col;
        FCH.urlImagePopup = url;
        FCH.scrollTop = $(document).scrollTop();

        data = "<div class='modal-dialog modal-xxl' role='document'><div class='modal-content'>";
        data += "<div class='modal-header'><button type='button' class='close' data-dismiss='modal' aria-label='Close'><span aria-hidden='true'>&times;</span></button>";
        data += "</div>";
        data += "<div class='modal-body text-center'><div style='display: flex; justify-content: center;'>";
        data += (nav ? "<div id='previousimageLayoutPopup' style='cursor:pointer; position:absolute; left:20px; top:45%; opacity:0.1;'><i class='fa fa-2x fa-chevron-left'></i></div>" : "");
        data += "<img src='" + FCH.urlImagePopup + "' class='img-responsive' style='width:100%; height:100%;' />";
        data += (nav ? "<div id='nextimageLayoutPopup' style='cursor:pointer; position:absolute; left:97%; top:45%; opacity:0.1;'><i class='fa fa-2x fa-chevron-right'></i></div>" : "");
        data += "</div></div>";
        data += "</div></div>";

        $('#layoutDefaultModal').html(data);
        $('#layoutDefaultModal').modal({
            backdrop: 'static',
            keyboard: true
        }, 'show');

        $(document).on('click', '#previousimageLayoutPopup', function () {
            var found = FCH.colElements.find(function (item) {
                if (item.attributes.imagen != undefined) {
                    if (item.attributes.imagen.indexOf(FCH.urlImagePopup.replace('conceptos', 'miniaturas')) > 0) {
                        return item;
                    }
                }
                if (item.attributes.image != undefined) {
                    if (item.attributes.image.indexOf(FCH.urlImagePopup.replace('conceptos', 'miniaturas')) > 0) {
                        return item;
                    }
                }
                if (item.attributes.picture != undefined) {
                    if (item.attributes.picture.indexOf(FCH.urlImagePopup.replace('conceptos', 'miniaturas')) > 0) {
                        return item;
                    }
                }
            });
            var ind = FCH.colElements.indexOf(found);
            if (ind > 0) {
                var prevModel = FCH.colElements.at(ind - 1);
                var posIniImage, posEndImage, ahrefimage;
                if (prevModel.attributes.imagen != undefined) {
                    posIniImage = prevModel.attributes.imagen.indexOf("http");
                    posEndImage = prevModel.attributes.imagen.indexOf("'", posIniImage) -1;
                    ahrefimage = prevModel.attributes.imagen.replace('miniaturas', 'conceptos').substring(posIniImage, posEndImage);
                }
                if (prevModel.attributes.image != undefined) {
                    posIniImage = prevModel.attributes.image.indexOf("http");
                    posEndImage = prevModel.attributes.image.indexOf("'", posIniImage);
                    ahrefimage = prevModel.attributes.image.replace('miniaturas', 'conceptos').substring(posIniImage, posEndImage);
                }
                if (prevModel.attributes.picture != undefined) {
                    posIniImage = prevModel.attributes.picture.indexOf("http");
                    posEndImage = prevModel.attributes.picture.indexOf("'", posIniImage);
                    ahrefimage = prevModel.attributes.picture.replace('miniaturas', 'conceptos').substring(posIniImage, posEndImage);
                }
                FCH.urlImagePopup = ahrefimage;
                $('#layoutDefaultModal').find('img').attr('src', FCH.urlImagePopup);
            }
        });
        $(document).on('click', '#nextimageLayoutPopup', function () {
            var found = FCH.colElements.find(function (item) {
                if (item.attributes.imagen != undefined) {
                    if (item.attributes.imagen.indexOf(FCH.urlImagePopup.replace('conceptos', 'miniaturas')) > 0) {
                        return item;
                    }
                }
                if (item.attributes.image != undefined) {
                    if (item.attributes.image.indexOf(FCH.urlImagePopup.replace('conceptos', 'miniaturas')) > 0) {
                        return item;
                    }
                }
                if (item.attributes.picture != undefined) {
                    if (item.attributes.picture.indexOf(FCH.urlImagePopup.replace('conceptos', 'miniaturas')) > 0) {
                        return item;
                    }
                }
            });
            var ind = FCH.colElements.indexOf(found);
            if (ind + 1 < FCH.colElements.length) {
                var nextModel = FCH.colElements.at(ind + 1);
                var posIniImage, posEndImage, ahrefimage;
                if (nextModel.attributes.imagen != undefined) {
                    posIniImage = nextModel.attributes.imagen.indexOf("http");
                    posEndImage = nextModel.attributes.imagen.indexOf("'", posIniImage) - 1;
                    ahrefimage = nextModel.attributes.imagen.replace('miniaturas', 'conceptos').substring(posIniImage, posEndImage);
                }
                if (nextModel.attributes.image != undefined) {
                    posIniImage = nextModel.attributes.image.indexOf("http");
                    posEndImage = nextModel.attributes.image.indexOf("'", posIniImage);
                    ahrefimage = nextModel.attributes.image.replace('miniaturas', 'conceptos').substring(posIniImage, posEndImage);
                }
                if (nextModel.attributes.picture != undefined) {
                    posIniImage = nextModel.attributes.picture.indexOf("http");
                    posEndImage = nextModel.attributes.picture.indexOf("'", posIniImage);
                    ahrefimage = nextModel.attributes.picture.replace('miniaturas', 'conceptos').substring(posIniImage, posEndImage);
                }
                FCH.urlImagePopup = ahrefimage;
                $('#layoutDefaultModal').find('img').attr('src', FCH.urlImagePopup);
            }
        });
    },
    obtenerAlturaPantalla: function () {
        var w = window,
            d = document,
            e = d.documentElement,
            g = d.getElementsByTagName('body')[0],
            x = w.innerWidth || e.clientWidth || g.clientWidth,
            y = w.innerHeight || e.clientHeight || g.clientHeight;
      return y;
    },
    buscarIndexPorAtributo: function (array, attr, value) {
        for (var i = 0; i < array.length; i += 1) {
            if (array[i][attr] === value) {
                return i;
            }
        }
        return -1;
    }
};

(function ($) {
    //Verifica el ancho de la pantalla y agrega o quita la clase para la selección
    //de proyectos
    var $window = $(window);

    function resize() {
        if ($window.width() < 514) {            
            $("#navtoplinks").removeClass("navbar-top-links");
            $("#navtoplinks").addClass("navbar-nav");
        }
        else {
            $("#navtoplinks").removeClass("navbar-nav");
            $("#navtoplinks").addClass("navbar-top-links");
        }
    }
    $window
        .resize(resize)
        .trigger('resize');    
})(jQuery);