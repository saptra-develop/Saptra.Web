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
    colVehiculos: [],
    colPeriodos: [],
    colPeriodoActual: [],
    detalleContainer: {},
    idPlan: 0,
    Inicial: function () {
        $.ajaxSetup({ cache: false });
        this.CargaPeriodos();
        this.Eventos();
        this.CargaGrid();
        //this.ValidaPermisos();
        $('.nav-tabs > li a[title]').tooltip();
        if (localStorage.IdPlan != undefined) {
            localStorage.removeItem("IdPlan");

            this.CargaGrid();
        }
    },
    Eventos: function () {
        var that = this;

        $('#btnFilter').click(that.CargaGrid);

        //$('.btnNuevo').click(that.Nuevo);

        $(document).on("click", '.btnNuevo', that.onGuardar);

        //$(document).on("click", '.btn-GuardaNuevo', that.onGuardar);

        $(document).on('click', '.accrowProdDetalle', function () {
            var idPlan = $(this).data("idplan");
            var idEstatus = $('#btnEnvioValidacion_' + idPlan).data("idestatus");
            if (idEstatus == 10) {
                FCH.DespliegaWarning('No es posible agregar nuevas actividades, el plan se encuentra con estatus "<strong>Enviado</strong>"');
            }
            else {
                PlanSemanal.NuevoDetalle(idPlan);
            }
        });

        $(document).on('click', '.btnVehiculo', function () {
            PlanSemanal.NuevoVehiculo($(this).data("iddetalleplan"));
        });
        $(document).on("click", '.btn-GuardaVehiculo', that.onGuardarVehiculo);

        $(document).on('click', '.btn-GuardaDetalleNuevo', function () {
            PlanSemanal.CargaGrid();
        });

        $(document).on('click', '.btn-AgregarDetalle', function () {
            PlanSemanal.onGuardarDetalle(2, '.btn-AgregarDetalle');
        });
        
        $(document).on('click', '.btnEnvioValidacion', function () {
            PlanSemanal.EnviarPlan($(this).data("idplan"));
        });

        $(document).on('click', '.btnEditarDetalle', function () {
            PlanSemanal.EditarDetalle($(this).data("iddetalleplan"));
        });
        $(document).on("click", '.btn-ActualizaDetalle', that.onActualizarDetalle);

        $(document).on('click', '.btnEliminarDetalle', function () {
            PlanSemanal.EliminarDetalle($(this).data("iddetalleplan"));
        });

        $(document).on('change', '#selPeriodo', function () {
            if ($('#selPeriodo').val() == "0") {
                $('#selPeriodo').addClass("has-error");
            } else {
                $('#selPeriodo').removeClass("has-error");
            }
        });
        $(document).on('change', '#selTipoActividad', function () {
            if ($('#selTipoActividad').val() == "0") {
                $('#selTipoActividad').addClass("has-error");
            } else {
                $('#selTipoActividad').removeClass("has-error");
            }
            if ($('#selTipoActividad').val() == "6") {
                $('#divCheckIn').show();
            } else {
                $('#divCheckIn').hide();
            }
        });
        
    },
    EnviarPlan: function (idPlan) {
        var btn = $('#btnEnvioValidacion_' + idPlan),
            btnName = $('#btnEnvioValidacion_' + idPlan).text();
        FCH.botonMensaje(true, btn, btnName);

        $.post(contextPath + "PlanSemanal/EnviarPlan?idPlan=" + idPlan + "&idUsuario=" + localStorage.idUser,
            function (data) {
                if (data.Success === true) {
                    FCH.DespliegaNotificacion('success', 'Plan ', data.Message, 'glyphicon-ok', 3000);
                    FCH.botonMensaje(false, btn, "<i class='fa fa-paper-plane fa-lg fa-fw'></i>" );
                    $('#btnEnvioValidacion_' + idPlan).removeClass("btn-primary");
                    $('#btnEnvioValidacion_' + idPlan).addClass("btn-success");
                    $("#btnEnvioValidacion_" + idPlan).prop("disabled", true);
                    $("#btnEnvioValidacion_" + idPlan).tooltip('destroy');
                    $("#accrowProdDetalle_" + idPlan).prop("disabled", true);
                    if (PlanSemanal.idPlan != 0) {
                        PlanSemanal.CargaGridDetallesPackingList(PlanSemanal.idPlan, PlanSemanal.detalleContainer);
                    }
                    FCH.cargarNotificaciones();
                } else {
                    FCH.DespliegaErrorDialogo(data.Message);
                }
            }).fail(function () {
                FCH.DespliegaErrorDialogo("Error al guardar la informacion");
            }).always(function () {  });
    },
    NuevoPlan: function () {
        FCH.CierraMensajes();
        var url = contextPath + "PlanSemanal/Nuevo"; // El url del controlador      
        $.get(url, function (data) {
            $('#nuevo-PlanSemanal').html(data);
            $('#nuevo-PlanSemanal').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');
            FCH.RedefinirValidaciones(); //para los formularios dinamicos
            PlanSemanal.activeForm = '#NuevoPlanForm';
            PlanSemanal.CargaPeriodoActual();
        });
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
            $(PlanSemanal.activeForm + ' #HoraFin').select2();
            PlanSemanal.CargaGridDetallesModal(id);
        });
    },
    NuevoVehiculo: function (id) {
        FCH.CierraMensajes();
        var url = contextPath + "PlanSemanal/NuevoVehiculo/" + id; // El url del controlador      
        $.get(url, function (data) {
            $('#nuevo-Vehiculo').html(data);
            $('#nuevo-Vehiculo').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');
            FCH.RedefinirValidaciones(); //para los formularios dinamicos
            PlanSemanal.activeForm = '#NuevoVehiculoForm';
            PlanSemanal.CargaVehiculosSipae();
        });
    },
    EditarDetalle: function (id) {
        FCH.CierraMensajes();
        var url = contextPath + "PlanSemanal/ActualizaDetallePlan/" + id; // El url del controlador
        $.get(url, function (data) {
            $('#actualiza-PlanDetalle').html(data);
            $('#actualiza-PlanDetalle').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');
            FCH.RedefinirValidaciones(); //para los formularios dinamicos
            PlanSemanal.activeForm = '#ActualizaDetallePlanForm';
            PlanSemanal.IniciaDateControls();
            $(PlanSemanal.activeForm + ' #HoraActividad').select2();
            $(PlanSemanal.activeForm + ' #HoraFin').select2();
            if ($('#ActualizaDetallePlanForm #TipoActividadId').val() == "6") {
                $('#divCheckIn').hide();
            }
            PlanSemanal.CargaTipoActividad();
        });
    },
    EliminarDetalle: function (id) {
        FCH.CierraMensajes();
        bootbox.setDefaults({
            locale: "es"
        });
        bootbox.confirm({
            message: "¿Está seguro que desea borrar este registro?",

            callback: function (result) {
                if (result) {
                    var url = contextPath + "PlanSemanal/EliminarDetalle"; // El url del controlador
                    $.post(url, { id: id }, function (data) {
                        if (data.Success === true) {
                            PlanSemanal.colPlanDetalle.remove(id);
                            FCH.DespliegaInformacion(data.Message + "  id:" + id);
                        } else {
                            FCH.DespliegaError(data.Message);
                        }
                    }).fail(function () { FCH.DespliegaError("No se pudo eliminar el registro"); });
                }
            }
        })
    },
    onGuardar: function () {
        //Se hace el post para guardar la informacion
        $.post(contextPath + "PlanSemanal/Nuevo?idUsuario=" + localStorage.idUser,
            function (data) {
                if (data.Success === true) {
                    PlanSemanal.CargaGrid();
                    PlanSemanal.NuevoDetalle(data.id);
                } else {
                    FCH.DespliegaError(data.Message);
                }
            }).fail(function () {
                FCH.DespliegaErrorDialogo("Error al guardar la información");

            }).always(function () { "" });




            //if ($('#NuevoPlanForm #selPeriodo').val() !== "0") {
            //    FCH.botonMensaje(true, btn, btnName);
            //    if ($("form").valid()) {
            //        $('#UsuarioCreacionId').val(localStorage.idUser);
            //        $('#PeriodoId').val($('#selPeriodo').val());
            //        //Se hace el post para guardar la informacion
            //        $.post(contextPath + "PlanSemanal/Nuevo",
            //            $("#NuevoPlanForm *").serialize(),
            //            function (data) {
            //                if (data.Success === true) {
            //                    //if ($('#bbGrid-clear:contains("Seleccionar periodo si desea filtrar la información")').length > 0 || $('#bbGrid-clear:contains("No se encontro información")').length > 0) {
            //                        //if ($('#bbGrid-clear').has('<div id="cargandoInfo">')) {
            //                    PlanSemanal.CargaGrid();
            //                    PlanSemanal.NuevoDetalle(data.id);
            //                    //} else {
            //                    //    PlanSemanal.colPlanSemanal.add(PlanSemanal.serializaPlan(data.id, '#NuevoPlanForm'));
            //                    //}
            //                    $('#nuevo-PlanSemanal').modal('hide');
            //                    FCH.DespliegaNotificacion('success', 'Plan ', data.Message, 'glyphicon-ok', 3000);
            //                } else {
            //                    FCH.DespliegaErrorDialogo(data.Message);
            //                }
            //            }).fail(function () {
            //                FCH.DespliegaErrorDialogo("Error al guardar la información");

            //            }).always(function () { FCH.botonMensaje(false, btn, btnName); });

            //    } else {
            //        FCH.botonMensaje(false, btn, btnName);
            //    }
            //} else {
            //    $('#NuevoPlanForm #selPeriodo').addClass("has-error");
            //}
     
    },
    onGuardarVehiculo: function (e) {
        var btn = this,
            btnName = btn.innerText;


        if ($('#NuevoVehiculoForm #selVehiculo').val() !== "0") {
            FCH.botonMensaje(true, btn, btnName);
            if ($("form").valid()) {
                $('#NuevoVehiculoForm #UsuarioCreacionId').val(localStorage.idUser);
                $('#NuevoVehiculoForm  #VehiculoId').val($('#selVehiculo').val());
                //Se hace el post para guardar la informacion
                $.post(contextPath + "PlanSemanal/NuevoVehiculo",
                    $("#NuevoVehiculoForm *").serialize(),
                    function (data) {
                        if (data.Success === true) {
                            $('#nuevo-Vehiculo').modal('hide');
                            FCH.DespliegaNotificacion('success', 'Vehiculo ', data.Message, 'glyphicon-ok', 3000);
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
        } else {
            $('#NuevoVehiculoForm #selVehiculo').addClass("has-error");
        }

    },
    onGuardarDetalle: function (tipo, boton) {
        FCH.CierraMensajes();
        var btn = $(boton),
            btnName = $(boton).text();
        var url = contextPath + "PlanSemanal/ValidarActividadFecha?idPlan=" + $('#PlanSemanalId').val() + "&fecha=" + $('#fechaAct').val() + "&hInicio=" + $('#NuevoDetallePlanForm #HoraActividad').val() + "&hFin=" + $('#NuevoDetallePlanForm #HoraFin').val();
        $.getJSON(url, function (data) {
            if (data.Success === true) {
                FCH.DespliegaErrorDialogo(data.Message);
            } else {
                if ($('#NuevoDetallePlanForm #HoraActividad').val() < $('#NuevoDetallePlanForm #HoraFin').val()) {
                    if ($('#NuevoDetallePlanForm #HoraFin').val() > $('#NuevoDetallePlanForm #HoraActividad').val()) {
                        if ($('#NuevoDetallePlanForm #DescripcionActividad').val() !== "" && $('#NuevoDetallePlanForm #selTipoActividad').val() !== "0" && $('#NuevoDetallePlanForm #fechaAct').val() !== "" && $('#NuevoDetallePlanForm #LugarActividad').val() !== "") {
                            FCH.botonMensaje(true, btn, btnName);
                            if ($("form").valid()) {
                                $('#NuevoDetallePlanForm #UsuarioCreacionId').val(localStorage.idUser);
                                $('#NuevoDetallePlanForm #TipoActividadId').val($('#selTipoActividad').val());
                                $('#NuevoDetallePlanForm #FechaActividad').val($('#fechaAct').val());
                                var PlanSemanalId = $('#PlanSemanalId').val();
                                //Se hace el post para guardar la informacion
                                $.post(contextPath + "PlanSemanal/NuevoDetalle",
                                    $("#NuevoDetallePlanForm *").serialize(),
                                    function (data) {
                                        if (data.Success === true) {

                                            if (tipo == 2) {
                                                //$('#NuevoDetallePlanForm #selTipoActividad').val(0);
                                                $("#NuevoDetallePlanForm #selTipoActividad").val('0').change();
                                                $('#NuevoDetallePlanForm #DescripcionActividad').val('');
                                                $('#NuevoDetallePlanForm #LugarActividad').val('');
                                                $('#NuevoDetallePlanForm #fechaAct').val('');
                                                $('#NuevoDetallePlanForm #HoraActividad').val('08:00').change();
                                                $('#NuevoDetallePlanForm #HoraFin').val('08:00').change();
                                                $('#NuevoDetallePlanForm #CantidadCheckIn').val(1);
                                                PlanSemanal.CargaGridDetallesModal(PlanSemanalId);

                                            }
                                            else {
                                                $('#nuevo-PlanDetalle').modal('hide');
                                                PlanSemanal.CargaGrid();
                                            }
                                            //if (PlanSemanal.colPlanDetalle.length >= 1) {
                                            //    PlanSemanal.colPlanDetalle.add(PlanSemanal.serializaPlanDetalle(data.id, '#NuevoDetallePlanForm'));
                                            //} else if (PlanSemanal.detalleContainer != "") {
                                            //    PlanSemanal.CargaGridDetallesPackingList(PlanSemanalId, PlanSemanal.detalleContainer);
                                            //}

                                            FCH.DespliegaNotificacion('success', 'Actividad ', data.Message, 'glyphicon-ok', 3000);
                                            $("#btnEnvioValidacion_" + PlanSemanalId).prop("disabled", false);
                                        } else {
                                            FCH.DespliegaErrorDialogo(data.Message);
                                        }
                                    }).fail(function () {
                                        FCH.DespliegaErrorDialogo("Error al guardar la información");

                                    }).always(function () { FCH.botonMensaje(false, btn, btnName); });

                            } else {
                                FCH.botonMensaje(false, btn, btnName);
                                if ($('#selTipoActividad').val() == "0") {
                                    $('#selTipoActividad').addClass("select2.error")
                                }
                            }
                        } else {
                            if ($('#NuevoDetallePlanForm #DescripcionActividad').val() == "" && $('#NuevoDetallePlanForm #selTipoActividad').val() == "0" && $('#NuevoDetallePlanForm #fechaAct').val() == "" && $('#NuevoDetallePlanForm #LugarActividad').val() == "") {
                                $('#NuevoDetallePlanForm #selTipoActividad').addClass("has-error");
                                $('#NuevoDetallePlanForm #fechaAct').addClass("input-validation-error");
                                $('#NuevoDetallePlanForm #fechaAct').attr("placeholder", "Obligatorio");
                                $('#NuevoDetallePlanForm #DescripcionActividad').addClass("input-validation-error");
                                $('#NuevoDetallePlanForm #DescripcionActividad').attr("placeholder", "Obligatorio");
                                $('#NuevoDetallePlanForm #LugarActividad').attr("placeholder", "Obligatorio");
                                $('#NuevoDetallePlanForm #LugarActividad').addClass("input-validation-error");
                            } else {
                                if ($('#NuevoDetallePlanForm #selTipoActividad').val() == "0") {
                                    $('#NuevoDetallePlanForm #selTipoActividad').addClass("has-error");
                                } else if ($('#NuevoDetallePlanForm #DescripcionActividad').val() == "") {
                                    $('#NuevoDetallePlanForm #DescripcionActividad').addClass("input-validation-error");
                                    $('#NuevoDetallePlanForm #DescripcionActividad').attr("placeholder", "Obligatorio");
                                } else if ($('#NuevoDetallePlanForm #fechaAct').val() == "") {
                                    $('#NuevoDetallePlanForm #fechaAct').attr("placeholder", "Obligatorio");
                                    $('#NuevoDetallePlanForm #fechaAct').addClass("input-validation-error");
                                } else {
                                    $('#NuevoDetallePlanForm #LugarActividad').attr("placeholder", "Obligatorio");
                                    $('#NuevoDetallePlanForm #LugarActividad').addClass("input-validation-error");
                                }
                            }
                        }
                    } else {
                        FCH.DespliegaErrorDialogo("La hora fin debe ser mayor a la hora inicial");
                    }
                } else {
                    FCH.DespliegaErrorDialogo("La hora inicial debe ser menor a la hora fin");
                }
            }
        });
    },
    onActualizarDetalle: function (e) {
        var btn = this,
            btnName = btn.innerText;
        var url = contextPath + "PlanSemanal/ValidarActividadFechaActualizar?idPlan=" + $('#ActualizaDetallePlanForm #PlanSemanalId').val() + "&idDetallePlan=" + $('#ActualizaDetallePlanForm #DetallePlanId').val() + "&idActividad=" + $('#ActualizaDetallePlanForm #TipoActividadId').val() + "&fecha=" + $('#ActualizaDetallePlanForm #FechaActividad').val() + "&hInicio=" + $('#ActualizaDetallePlanForm #HoraActividad').val() + "&hFin=" + $('#ActualizaDetallePlanForm #HoraFin').val();
        $.getJSON(url, function (data) {
            if (data.Success === true) {
                FCH.DespliegaErrorDialogo(data.Message);
            } else {
                if ($('#ActualizaDetallePlanForm #HoraActividad').val() < $('#ActualizaDetallePlanForm #HoraFin').val()) {
                    if ($('#ActualizaDetallePlanForm #HoraFin').val() > $('#ActualizaDetallePlanForm #HoraActividad').val()) {
                        if ($('#ActualizaDetallePlanForm #DescripcionActividad').val() !== "" && $('#ActualizaDetallePlanForm #selTipoActividad').val() !== "0" && $('#ActualizaDetallePlanForm #fechaAct').val() !== "" && $('#ActualizaDetallePlanForm #LugarActividad').val() !== "") {
                            FCH.botonMensaje(true, btn, btnName);
                            if ($("form").valid()) {
                                $('#ActualizaDetallePlanForm #UsuarioCreacionId').val(localStorage.idUser);
                                $('#ActualizaDetallePlanForm #TipoActividadId').val($('#ActualizaDetallePlanForm #selTipoActividad').val());
                                var PlanSemanalId = $('#PlanSemanalId').val();
                                //Se hace el post para guardar la informacion
                                $.post(contextPath + "PlanSemanal/ActualizaDetalle",
                                    $("#ActualizaDetallePlanForm *").serialize(),
                                    function (data) {
                                        if (data.Success === true) {
                                            $('#actualiza-PlanDetalle').modal('hide');
                                            FCH.DespliegaNotificacion('success', 'Actividad ', data.Message, 'glyphicon-ok', 3000);
                                            PlanSemanal.CargaGridDetallesPackingList(PlanSemanal.idPlan, PlanSemanal.detalleContainer);
                                            // PlanSemanal.colPlanDetalle.add(PlanSemanal.serializaPlanDetalle(data.idDetallePlan, '#ActualizaDetallePlanForm'), { merge: true });
                                        } else {
                                            FCH.DespliegaErrorDialogo(data.Message);
                                        }
                                    }).fail(function () {
                                        FCH.DespliegaErrorDialogo("Error al guardar la información");
                                    }).always(function () { FCH.botonMensaje(false, btn, btnName); });
                            } else {
                                FCH.botonMensaje(false, btn, btnName);
                            }
                        } else {
                            if ($('#ActualizaDetallePlanForm #DescripcionActividad').val() == "" && $('#ActualizaDetallePlanForm #selTipoActividad').val() == "0" && $('#ActualizaDetallePlanForm #fechaAct').val() == "" && $('#ActualizaDetallePlanForm #LugarActividad').val() == "") {
                                $('#ActualizaDetallePlanForm #selTipoActividad').addClass("has-error");
                                $('#ActualizaDetallePlanForm #fechaAct').addClass("input-validation-error");
                                $('#ActualizaDetallePlanForm #fechaAct').attr("placeholder", "Obligatorio");
                                $('#ActualizaDetallePlanForm #DescripcionActividad').addClass("input-validation-error");
                                $('#ActualizaDetallePlanForm #DescripcionActividad').attr("placeholder", "Obligatorio");
                                $('#ActualizaDetallePlanForm #LugarActividad').attr("placeholder", "Obligatorio");
                                $('#ActualizaDetallePlanForm #LugarActividad').addClass("input-validation-error");
                            } else {
                                if ($('#ActualizaDetallePlanForm #selTipoActividad').val() == "0") {
                                    $('#ActualizaDetallePlanForm #selTipoActividad').addClass("has-error");
                                } else if ($('#ActualizaDetallePlanForm #DescripcionActividad').val() == "") {
                                    $('#ActualizaDetallePlanForm #DescripcionActividad').addClass("input-validation-error");
                                    $('#ActualizaDetallePlanForm #DescripcionActividad').attr("placeholder", "Obligatorio");
                                } else if ($('#ActualizaDetallePlanForm #fechaAct').val() == "") {
                                    $('#ActualizaDetallePlanForm #fechaAct').attr("placeholder", "Obligatorio");
                                    $('#ActualizaDetallePlanForm #fechaAct').addClass("input-validation-error");
                                } else {
                                    $('#ActualizaDetallePlanForm #LugarActividad').attr("placeholder", "Obligatorio");
                                    $('#ActualizaDetallePlanForm #LugarActividad').addClass("input-validation-error");
                                }
                            }
                        }
                    } else {
                        FCH.DespliegaErrorDialogo("La hora fin debe ser mayor a la hora inicial");
                    }
                } else {
                    FCH.DespliegaErrorDialogo("La hora inicial debe ser menor a la hora fin");
                }
            }
        });
    },
    CargaGrid: function () {
        $('#bbGrid-clear')[0].innerHTML = "";

        $('#cargandoInfo').show();
        $('#bbGrid-clear')[0].innerHTML = "<span class='glyphicon glyphicon-refresh glyphicon-refresh-animate'></span> Cargando Información...";
        var url = contextPath + "PlanSemanal/CargarPlanes?idUsu=" + localStorage.idUser + "&idPeriodo=" + ($('#selPeriodos').val() == null ? "0" : $('#selPeriodos').val()); // El url del controlador
        $.getJSON(url, function (data) {
            if (data.Success === false) { FCH.DespliegaError(data.Message); return; }
            $('#bbGrid-clear')[0].innerHTML = "";
            PlanSemanal.colPlanSemanal = new Backbone.Collection(data.datos);
            var bolFilter = PlanSemanal.colPlanSemanal.length > 0 ? true : false;
            if (bolFilter) {
                PlanSemanal.gridPlanSemanal = new bbGrid.View({
                    container: $('#bbGrid-clear'),
                    //rows: 200,
                    //rowList: [15, 50, 200, 1000],
                    enableSearch: true,
                    subgrid: true,
                    actionenable: false,
                    prodDetalle: true,
                    clone: false,
                    editar: false,
                    borrar: false,
                    detalle: false,
                    collection: PlanSemanal.colPlanSemanal,
                    colModel: [//{ title: 'Id', name: 'id', width: '8%', sorttype: 'number', filter: true, filterType: 'input' },
                        { title: 'Agregar actividades', name: 'accion', textalign: true },
                        { title: 'Usuario', name: 'usuario', index: true },
                        { title: 'Descripción', name: 'descripcionPlan', index: true },
                        { title: 'Periodo', name: 'periodo', index: true },
                        { title: 'Actividades registradas', name: 'actividades'},
                        { title: 'Enviar', name: 'enviar', textalign: true }
                    ],
                    onRowExpanded: function ($el, rowid) {
                        PlanSemanal.idPlan = rowid;
                        PlanSemanal.CargaGridDetallesPackingList(rowid, $el);
                    }
                    

                });
                PlanSemanal.RenderTooltips();
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
    RenderTooltips: function () {
        $.each($('.accrowProdDetalle'), function (i, item) {
            $(item).attr('title', 'Agregar actividades').tooltip();
        });
        $.each($('.btnEnvioValidacion'), function (i, item) {
            $(item).attr('title', 'Enviar a validación').tooltip();
        });
    },
    CargaGridDetallesPackingList: function (id, $container) {
        var url = contextPath + "PlanSemanal/CargarDetallePlan?idPlanSemanal=" + id; // El url del controlador
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
                            { title: ' ', name: 'accion', textalign: true },
                            { title: 'Actividad', name: 'actividad',  index: true },
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
            PlanSemanal.PintarFilasConHidden();
            if (PlanSemanal.colPlanDetalle.length < 1) {
                PlanSemanal.gridPlanDetalle.toggleLoading(false);
            }
        });
    },
    PintarFilasConHidden: function () {
        $('.hdnPintaFila').parent().parent().addClass('requestedOutTime');
    },
    CargaGridDetallesModal: function (id) {
        $('#bbGrid-Actividades')[0].innerHTML = "";
        var url = contextPath + "PlanSemanal/CargarDetallePlan?idPlanSemanal=" + id; // El url del controlador
        $('#cargandoInfo').show();
        //$('#bbGrid-Actividades')[0].innerHTML = "<span class='glyphicon glyphicon-refresh glyphicon-refresh-animate'></span> Cargando Información...";
        $.getJSON(url, function (data) {
            if (data.Success === false) { FCH.DespliegaError(data.Message); return; }
            $('#bbGrid-Actividades')[0].innerHTML = "";
            PlanSemanal.colPlanDetalle = new Backbone.Collection(data.datos);
            $('#bbGrid-Actividades')[0].innerHTML = '';
            PlanSemanal.gridPlanDetalle = new bbGrid.View({
                container: $('#bbGrid-Actividades'),
                collection: PlanSemanal.colPlanDetalle,
                colModel: [//{ title: 'id', name: 'id', index: true },
                    { title: 'Actividad', name: 'actividad', index: true },
                    { title: 'Descripción', name: 'descripcion', index: true },
                    { title: 'Lugar', name: 'lugar', index: true },
                    { title: 'Fecha', name: 'fecha', index: true },
                    { title: 'Hora Inicio', name: 'hora', index: true },
                    { title: 'Hora Fin', name: 'horaFin', index: true }],
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
                FCH.DespliegaErrorDialogo("No se pudieron cargar las actividades");
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
        if ($(PlanSemanal.activeForm + " #TipoActividadId").val() !== "")
            $(PlanSemanal.activeForm + ' #selTipoActividad').val($(PlanSemanal.activeForm + " #TipoActividadId").val()).change();
    },
    CargaVehiculosSipae: function () {
        //if (PlanSemanal.colVehiculos.length < 1) {
        var url = contextPath + "TipoActividades/CargarVehiculosSipae";
        $.getJSON(url, function (data) {
            PlanSemanal.colVehiculos = data;
            PlanSemanal.CargaListaVehiculos();
        }).fail(function () {
            FCH.DespliegaErrorDialogo("No se pudieron cargar los vehiculos");
        });
        //} else {
        //    PlanSemanal.CargaListaVehiculos();
        //}
    },
    CargaListaVehiculos: function () {
        var select = $(PlanSemanal.activeForm + ' #selVehiculo').empty();

        select.append('<option value="0">SELECCIONA VEHICULO</option>');
        $.each(PlanSemanal.colVehiculos, function (i, item) {
            select.append('<option value="' + item.id + '">' + item.nombre + '</option>');
        });

        $(PlanSemanal.activeForm + " #selVehiculo").select2({ allowClear: true });
        if ($(PlanSemanal.activeForm + " #selVehiculo").val() !== "")
            $(PlanSemanal.activeForm + ' #selVehiculo').val($(PlanSemanal.activeForm + " #VehiculoId").val()).change();
    },
    CargaPeriodoActual: function () {
        if (PlanSemanal.colPeriodoActual.length < 1) {
            var url = contextPath + "Periodos/CargarPeriodoActual?idEstatus=5";
            $.getJSON(url, function (data) {
                PlanSemanal.colPeriodoActual = data;
                PlanSemanal.CargaListaPeriodoActual();
            }).fail(function () {
                FCH.DespliegaErrorDialogo("No se pudieron cargar los peridos");
            });
        } else {
            PlanSemanal.CargaListaPeriodoActual();
        }
    },
    CargaListaPeriodoActual: function () {
        var select = $(PlanSemanal.activeForm + ' #selPeriodo').empty();

        select.append('<option value="0">SELECCIONAR</option>');
        $.each(PlanSemanal.colPeriodoActual, function (i, item) {
            select.append('<option value="' + item.id + '">' + item.nombre + '</option>');
        });

        $(PlanSemanal.activeForm + " #selPeriodo").select2({ allowClear: true });
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
    },
    IniciaDateControls: function () {
        var form = PlanSemanal.activeForm;
        $(form + ' #dtpfechaCompromiso').datetimepicker({
            useCurrent: false,
            format: 'DD/MM/YYYY'
        });
        $(form + ' #dtpfechaCompromiso').data("DateTimePicker").minDate($(form + ' #dtpfechaCompromiso').attr('data-fini'));
        $(form + ' #dtpfechaCompromiso').data("DateTimePicker").maxDate($(form + ' #dtpfechaCompromiso').attr('data-ffin'));
    },
    serializaPlan: function (id, form) {
        return ({
            'usuario': localStorage.UserName,
            'descripcionPlan': "Plan Semanal del " + $(form + ' #selPeriodo option:selected').text(),
            'periodo': $(form + ' #selPeriodo option:selected').text(),
            'accion': "<button disabled class='btn btn-xs btn-primary btnEnvioValidacion' style='border-radius: 21px;' id='btnEnvioValidacion_" + id + "' data-idplan='" + id + "'><i class='fa fa-paper-plane  fa-lg fa-fw'></i> Enviar</button>",
            'actividades': '<i class="fa fa-times-circle-o fa-2x" style="color:#F74048;"></i>',
            'id': id
        });
    },
    serializaPlanDetalle: function (id, form) {
        return ({
            'actividad': $(form + ' #selTipoActividad option:selected').text(),
            'descripcion': $(form + ' #DescripcionActividad').val(),
            'fecha': $(form + ' #fechaAct').val(),
            'hora': $(form + ' #HoraActividad').val(),
            'checkin': $(form + ' #CantidadCheckIn').val(),
            'accion': '<button class="btn btn-xs btn-warning btnEditarDetalle" style="border-radius: 21px;" id="btnEditarDetalle_' + id + '" data-iddetalleplan="' + id + '">Editar</button>',
            'id': id
        });
    },
    validarFechaHora: function (idPlan, fecha, hInicio, hFin) {
        var url = contextPath + "PlanSemanal/ValidarActividadFecha?idPlan=" + idPlan + "&fecha=" + fecha + "&hInicio=" + hInicio + "&hFin=" + hFin;
        $.getJSON(url, function (data) {
            if (data.Success === true) {
                FCH.DespliegaErrorDialogo(data.Message);
                return 0;
            } else {
                return 1;
            }
        });
    }
};

$(function () {
    PlanSemanal.Inicial();
});