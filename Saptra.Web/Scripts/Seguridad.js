/* global contextPath, CHR, Backbone, bbGrid, system_lang, bootbox, _ */

// js de Seguridad
// Juan Lopepe
// 10/09/2016

var Rol = {
    accClonar: false,
    accEscritura: false,
    accBorrar: false,
    accSeguridad: false,
    activeForm: '',
    rolSelect: '',
    colRoles: {},
    gridRoles: {},
    colModulos: {},
    gridSeguridad: {},
    Inicial: function () {
        $.ajaxSetup({ cache: false });
        this.CargaGrid();
        this.Eventos();
        this.ValidaPermisos();
    },
    CargaGrid: function () {
        var url = contextPath + "Rol/CargarRoles";
        $.getJSON(url, function (data) {
            $('#bbGrid-clear').html('');
            $('#cargandoInfo').show();
            if (data.Success !== undefined) { FCH.DespliegaError(data.Message); return; }
            Rol.colRoles = new Backbone.Collection(data);
            var bolFilter = Rol.colRoles.length > 0 ? true : false;
            if (bolFilter) {
                Rol.gridRoles = new bbGrid.View({
                    container: $('#bbGrid-clear'),
                    rows: 15,
                    rowList: [5, 15, 25, 50, 100],
                    enableSearch: true,
                    actionenable: true,
                    detalle: false,
                    clone: false,
                    editar: Rol.accEscritura,
                    borrar: Rol.accBorrar,
                    collection: Rol.colRoles,
                    seguridad: Rol.accSeguridad,
                    colModel: [{ title: 'Id', name: 'id', index: true },
                               { title: 'Rol', name: 'nombre', filter: true, filterType: 'input', index: true },
                        { title: 'Estatus', name: 'NombreEstatus', filter: true, index: true }]
                });
                $('#cargandoInfo').hide();
            }
            else {
                FCH.DespliegaInformacion("No se encontraron Roles registrados");
                $('#bbGrid-clear')[0].innerHTML = "";
            }
            //getJSON fail
        }).fail(function () {
            FCH.DespliegaError("No se pudo cargar la informacion de los roles ");
        });
    },
    Eventos: function () {
        var that = this;
        $('.btnNuevo').click(that.Nuevo);
        $(document).on("click", '.btn-GuardaNuevo', that.onGuardar);
        $(document).on("click", '.btn-ActualizarRol', that.onActualizar);

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

        $(document).on('click', '.accrowSeguridad', function () {
            that.Seguridad($(this).parent().parent().attr("data-modelId"));
        });

    },
    Nuevo: function () {
        FCH.CierraMensajes();
        var url = contextPath + "Rol/NuevoRol";
        $.get(url, function (data) {
            $('#nuevoRol').html(data);
            $('#nuevoRol').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');
            FCH.RedefinirValidaciones(); //para los formularios dinamicos
            $('#NuevoRolForm #chkViewAll').bootstrapSwitch({
                size: 'small',
                onText: 'YES',
                offText: 'NO',
            });
            $('#infoViewAll').attr('title', 'View products from all LBs').tooltip();
        });
    },
    onGuardar: function () {
        var btn = this,
            btnName = btn.innerText;
        FCH.botonMensaje(true, btn, btnName);
        if ($("form").valid()) {
            $('#NuevoRolForm #viewAll').val($('#NuevoRolForm #chkViewAll').prop("checked") ? 1 : 0);
            //Se hace el post para guardar la informacion
            $.post(contextPath + "Rol/NuevoRol",
                $("#NuevoRolForm *").serialize(),
                function (data) {
                    if (data.Success === true) {
                        Rol.colRoles.add(Rol.serializaRol(data.id, '#NuevoRolForm'));
                        FCH.DespliegaInformacion("El Rol fue guardado con el Id: " + data.id);
                        $('#nuevoRol').modal('hide');
                        if (Rol.colRoles.length === 1) {
                            Rol.CargaGrid();
                        }
                    } else {
                        FCH.DespliegaErrorDialogo(data.Message);
                    }
                }).fail(function () {
                    FCH.DespliegaErrorDialogo("Error al guardar la información.");
                }).always(function () { FCH.botonMensaje(false, btn, btnName); });
        } else {
            FCH.botonMensaje(false, btn, btnName);
        }
    },
    Editar: function (id) {
        FCH.CierraMensajes();
        var url = contextPath + "Rol/ActualizaRol/" + id; // El url del controlador
        $.get(url, function (data) {
            $('#actualizaRol').html(data);
            $('#actualizaRol').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');
            FCH.RedefinirValidaciones(); //para los formularios dinamicos
            $('#ActualizaRolForm #chkViewAll').prop('checked', $('#ActualizaRolForm #viewAll').val() === '1' ? true : false);
            $('#ActualizaRolForm #chkViewAll').bootstrapSwitch({
                size: 'small',
                onText: 'YES',
                offText: 'NO',
            });
            $('#infoViewAll').attr('title', 'View products from all LBs').tooltip();
        });
    },
    onActualizar: function () {
        var btn = this,
            btnName = btn.innerText;

        FCH.botonMensaje(true, btn, btnName);
        if ($("form").valid()) {
            //Se hace el post para guardar la informacion
            $('#ActualizaRolForm #viewAll').val($('#ActualizaRolForm #chkViewAll').prop("checked") ? 1 : 0);
            $.post(contextPath + "Rol/ActualizaRol",
                $("#ActualizaRolForm *").serialize(),
                function (data) {
                    if (data.Success === true) {
                        $('#actualizaRol').modal('hide');
                        Rol.CargaGrid();
                        FCH.DespliegaInformacion("Rol actualizado correctamente");
                    } else {
                        FCH.DespliegaErrorDialogo(data.Message);
                    }
                }).fail(function () {
                    FCH.DespliegaErrorDialogo("Error al actualizar la información");
                }).always(function () { FCH.botonMensaje(false, btn, btnName); });
        } else {
            FCH.botonMensaje(false, btn, btnName);
        }
    },
    serializaRol: function (id, form) {
        return ({
            'id': id,
            'nombre': $(form + ' #NombreRol').val(),
            'nombreEstatus': $(form + ' #EstatusId option:selected').text().toUpperCase()
        });
    },
    Borrar: function (id) {
        FCH.CierraMensajes();
        bootbox.confirm("Deseas eliminar el rol con id: " + id, function (result) {
            if (result) {
                var url = contextPath + "Rol/BorrarRol";
                $.post(url, { id: id }, function (data) {
                    if (data.Success === true) {
                        Rol.CargaGrid();
                        FCH.DespliegaInformacion(data.Message + "  id:" + id);
                    }
                    else {
                        FCH.DespliegaError(data.Message);
                    }
                }).fail(function () { FCH.DespliegaError("Error al eliminar el rol"); });
            }
        });
    },
    Seguridad: function (id) {
        FCH.CierraMensajes();
        var url = contextPath + "Rol/Modulos/" + id;
        $.get(url, function (data) {
            $('#permisos').html(data);
            $('#permisos').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');
            Rol.IniciaSeguridad(id);
        });
    },
    ValidaPermisos: function () {
        var permisos = localStorage.modPermisos,
            modulo = Rol;
        modulo.accEscritura = permisos.substr(1, 1) === '1' ? true : false;
        modulo.accBorrar = permisos.substr(2, 1) === '1' ? true : false;
        modulo.accClonar = permisos.substr(3, 1) === '1' ? true : false;

        if (modulo.accEscritura === true)
            $('.btnNuevo').show();

        modulo.accSeguridad = true;
    },
    IniciaSeguridad: function (pid) {
        $.ajaxSetup({ cache: false });
        Rol.colModulos = {};
        Rol.gridSeguridad = {};
        Rol.rolSelect = pid;
        Rol.CargaGridSeguridad();
        Rol.EventosSeguridad();
    },
    CargaGridSeguridad: function () {
        $('#cargandoInfoSeg').show();
        var url = contextPath + "Rol/CargarModulos/" + Rol.rolSelect;
        $.getJSON(url, function (data) {
            if (data.Success !== undefined) { FCH.DespliegaError(data.Message); return; }
            Rol.colModulos = new Backbone.Collection(data);
            var bolFilter = Rol.colModulos.length > 0 ? true : false;
            if (bolFilter) {
                Rol.gridSeguridad = new bbGrid.View({
                    container: $('#bbGrid-seguridad'),
                    enableSearch: true,
                    actionenable: false,
                    collection: Rol.colModulos,
                    colModel: [{ title: 'Modulo', name: 'nombreModulo', index: true },
                               { title: 'Lectura&nbsp;<input type="checkbox" id="chkAllLectura" />', name: 'lecturaPermiso', checkboxgen: true, textalign: true },
                               { title: 'Escritura&nbsp;<input type="checkbox" id="chkAllEscritura" />', name: 'escrituraPermiso', checkboxgen: true, textalign: true },
                               { title: 'Copiado&nbsp;<input type="checkbox" id="chkAllClonar" />', name: 'clonadoPermiso', checkboxgen: true, textalign: true },
                               { title: 'Borrar&nbsp;<input type="checkbox" id="chkAllBorrar" />', name: 'borradoPermiso', checkboxgen: true, textalign: true }]
                });
            } else {
                FCH.DespliegaInformacionDialogo("No se encontraron Modulos en base de datos.");
                $('#bbGrid-seguridad')[0].innerHTML = "";
            }
            $('#cargandoInfoSeg').hide();
            //getJSON fail
        }).fail(function () {
            FCH.DespliegaErrorDialogo("No se pudo cargar la informacion de los Modulos");
        }).always(function () { $('#cargandoInfoSeg').hide(); });
    },
    EventosSeguridad: function () {
        var that = this;

        $(document).on("click", '.btn-GuardaSeguridad', that.onGuardarSeguridad);
        $(document).on("change", '#chkAllLectura', that.onCheckAll);
        $(document).on("change", '#chkAllEscritura', that.onCheckAll);
        $(document).on("change", '#chkAllClonar', that.onCheckAll);
        $(document).on("change", '#chkAllBorrar', that.onCheckAll);
    },
    onGuardarSeguridad: function () {
        var modulos = [],
            btn = this,
            btnName = btn.innerText,
            dataPost = {
                lstModulos: modulos,
                idRol: Rol.rolSelect
            };

        FCH.botonMensaje(true, btn, btnName);

        //Se agrega la coleccion de items.
        _.each(Rol.colModulos.models, function (object) {
            object.attributes.lecturaPermiso = 0;
            object.attributes.escrituraPermiso = 0;
            object.attributes.borradoPermiso = 0;
            object.attributes.clonadoPermiso = 0;
        });

        //Se actuliza la coleccion con la informacion seleccionada
        $("#frmSeguridad").find("input:checked").each(function (index, item) {
            if (item.id.split("-").length > 1) {
                var arrModel = item.id.split("-");
                var model = Rol.colModulos.get(arrModel[0]);
                model.attributes[arrModel[1]] = 1;
                Rol.colModulos.add(model, { merge: true });
            }
        });

        //Se agrega la coleccion de items.
        _.each(Rol.colModulos.models, function (object) {
            modulos.push(object.attributes);
        });

        //Se hace el post para guardar la informacion
        $.post(contextPath + "Rol/GuardaSeguridad",
            dataPost,
            function (data) {
                var div;
                if (data.Success === true) {
                    FCH.DespliegaInformacion(data.Message);
                    $('#permisos').modal('hide');
                } else {
                    FCH.DespliegaErrorDialogo(data.Message);
                    div = document.getElementById('divMessage');
                    if (div !== null) div.scrollIntoView();
                }
            }).fail(function () {
                FCH.DespliegaErrorDialogo("Error al guardar la información.");
                var div = document.getElementById('divMessage');
                if (div !== null) div.scrollIntoView();
            }).always(function () { FCH.botonMensaje(false, btn, 'Guardar'); });

    },
    onCheckAll: function () {
        var chk = this,
            item,
            text;
        if ($(chk).attr('id') === 'chkAllLectura') { item = '#chkAllLectura'; text = 'lecturaPermiso'; }
        if ($(chk).attr('id') === 'chkAllEscritura') { item = '#chkAllEscritura'; text = 'escrituraPermiso'; }
        if ($(chk).attr('id') === 'chkAllClonar') { item = '#chkAllClonar'; text = 'clonadoPermiso'; }
        if ($(chk).attr('id') === 'chkAllBorrar') { item = '#chkAllBorrar'; text = 'borradoPermiso'; }


        var selected = [],
            id = '',
            arrId = [],
            head = $(item).prop('checked');
        $('input[type=checkbox]').each(function () {
            selected.push($(this).attr('id'));
        });
        for (var i = 0; i < selected.length; i++) {
            id = selected[i];
            arrId = id.split("-");
            if (arrId.length > 1) {
                if (arrId[1] === text) {
                    if (head === false) {
                        $('#' + id).prop('checked', true);
                        $('#' + id).removeAttr('checked');
                    } else {
                        $('#' + id).removeAttr('checked');
                        $('#' + id).prop('checked', true);
                    }

                }
            }
        }
    }
};

$(function () {
    Rol.Inicial();
});