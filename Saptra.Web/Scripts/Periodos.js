/* global CHR, contextPath, bootbox, Backbone, bbGrid */

//js de catalogo de Periodos.
//David Jasso
//19/Octubre/2018

var Periodo = {
    accClonar: false,
    accEscritura: false,
    accBorrar: false,
    activeForm: '',
    gridPeriodo: {},
    colPeriodo: {},
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
            $.post(contextPath + "Periodos/Nuevo?fIni=" + $('#fechaIni').val() + "&fFin=" + $('#fechaF').val(),
                $("#NuevoPeriodoForm *").serialize(),
                function (data) {
                    if (data.Success === true) {
                        FCH.DespliegaInformacion("El periodo fue guardado con el Id: " + data.id);
                        $('#nuevo-Periodo').modal('hide');
                        Periodo.CargaGrid();
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
            $.post(contextPath + "Periodos/Actualizar",
                $("#ActualizaPeriodoForm *").serialize(),
                function (data) {
                    if (data.Success === true) {
                        $('#actualiza-Periodo').modal('hide');
                        FCH.DespliegaInformacion("El Periodo fue Actualizado. Id: " + data.id);
                        Periodo.CargaGrid();
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
        var url = contextPath + "Periodos/Nuevo"; // El url del controlador      
        $.get(url, function (data) {
            $('#nuevo-Periodo').html(data);
            $('#nuevo-Periodo').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');
            FCH.RedefinirValidaciones(); //para los formularios dinamicos
            Periodo.activeForm = '#NuevoPeriodoForm';
            Periodo.IniciaDateControls();
            $(Periodo.activeForm + ' #EstatusId').select2();
        });
    },
    Editar: function (id) {
        FCH.CierraMensajes();
        var url = contextPath + "Periodos/Actualizar/" + id; // El url del controlador
        $.get(url, function (data) {
            $('#actualiza-Periodo').html(data);
            $('#actualiza-Periodo').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');

            FCH.RedefinirValidaciones(); //para los formularios dinamicos
            Periodo.activeForm = '#ActualizaPeriodoForm';
            Periodo.IniciaDateControls();
            $(Periodo.activeForm + ' #EstatusId').select2();
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
                    var url = contextPath + "Periodos/Borrar"; // El url del controlador
                    $.post(url, { id: id }, function (data) {
                        if (data.Success === true) {
                            Periodo.colPeriodo.remove(id);
                            FCH.DespliegaInformacion(data.Message + "  id:" + id);
                        } else {
                            FCH.DespliegaError(data.Message);
                        }
                    }).fail(function () { FCH.DespliegaError("No se pudo eliminar el periodo"); });
                } }
        })
    },
    CargaGrid: function () {
        $('#bbGrid-clear')[0].innerHTML = "";
        $('#cargandoInfo').show();
        var url = contextPath + "Periodos/CargarPeriodos"; // El url del controlador
        $.getJSON(url, function (data) {
            if (data.Success === false) { FCH.DespliegaError(data.Message); return; }
            $('#bbGrid-clear')[0].innerHTML = "";
            Periodo.colPeriodo = new Backbone.Collection(data);
            var bolFilter = Periodo.colPeriodo.length > 0 ? true : false;
            if (bolFilter) {
                Periodo.gridPeriodo = new bbGrid.View({
                    container: $('#bbGrid-clear'),
                    rows: 200,
                    rowList: [15, 50, 200, 1000],
                    enableSearch: true,
                    actionenable: true,
                    detalle: false,
                    clone: false,
                    editar: true,
                    borrar: true,
                    collection: Periodo.colPeriodo,
                    colModel: [{ title: 'Id', name: 'id', width: '8%', sorttype: 'number' },
                                { title: 'Fecha Inicio', name: 'fechaInicio', index: true },
                                { title: 'Fecha Fin', name: 'fechaFin', index: true },
                                { title: 'Descripción', name: 'nombre',  index: true },
                                { title: 'Estatus', name: 'estatus',  index: true }]

                });
                $('#cargandoInfo').hide();
            } else {
                FCH.DespliegaInformacion("No se encontraron periodos registrados");
                $('#bbGrid-clear')[0].innerHTML = "";
            }

            //getJSON fail
        }).fail(function (e) {
            FCH.DespliegaError("No se pudo cargar la informacion de los periodos");
        });
    },
    IniciaDateControls: function () {
        var form = Periodo.activeForm;
        $(form + ' #dtpfechaInicio').datetimepicker({
            useCurrent: false,
            format: 'MM/DD/YYYY'
        });

        var d = new Date();
        var strDate = (d.getMonth()+1) + "/" + d.getDate() + "/" + d.getFullYear();

        $(form + ' #dtpfechaInicio').data("DateTimePicker").minDate(strDate);


        $(form + ' #dtpfechaFin').datetimepicker({
            useCurrent: false,
            format: 'MM/DD/YYYY'
        });

        $(form + ' #dtpfechaFin').data("DateTimePicker").minDate(strDate);
    }
};

$(function () {
    Periodo.Inicial();
});