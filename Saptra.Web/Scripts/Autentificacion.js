var Autentificacion = {
    Time: 300,
    Inicial: function () {
        this.Eventos();
    },

    Eventos: function () {
       var that = this
       $(document).on("click", '#btn-olvido-password', that.MostrarFormOlvidoContraseña);
       $(document).on("click", '#btn-regresa-login', that.MostrarFormLogin);
       $(document).on("click", '#btn-enviar-email', that.EnviarEmail);
       //$(document).on("click", '#btn-reset-password', that.ResetPassword);
      
    },
    EnviarEmail: function () {
        //var btn = $('#btn-enviar-email');
        //var btnName = $('#btn-enviar-email').text();
        $("#lbl-message").html("");
        if ($("#frmEnviarEmail").valid()) {
            //CHR.botonMensaje(true, btn, btnName);
                $.post("/Autentificacion/EnviarEmail",
                    $("#frmEnviarEmail *").serialize(),
                    function (data) {
                        if (data.Success === true) {
                            Autentificacion.MostrarFormLogin();
                            $(".lbl-message").html('<div div class="alert alert-success alert-dismissible" role="alert">' +
                                                    '<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>' +
                                                    '<strong>Mensaje!</strong> ' + data.Message + '</div>');
                        } else {
                            $(".lbl-message").html('<div div class="alert alert-warning alert-dismissible" role="alert">' +
                                                    '<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>' +
                                                    '<strong>Mensaje!</strong> ' + data.Message + '</div>');
                        }
                    }).fail(function () {
                        $(".lbl-message").html('<div div class="alert alert-warning alert-dismissible" role="alert">' +
                                                '<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>' +
                                                '<strong>Mensaje!</strong> ' + data.Message + '</div>');
                    }).always(function () {
                        //CHR.botonMensaje(false, btn, btnName);
                    });
            }
    },

    //ResetPassword: function () {
    //    if ($("#frmResetPassword").valid()) {
    //        $.post("/Autentificacion/ResetPassword?NuevoPassword=" + $('#confirmaPassword').val() + "&confirmaPassword=" + $('#nuevoPassword').val(),
    //        $("#frmResetPassword *").serialize(),
    //        function () {

    //        });
    //    }
    //},
    MostrarFormOlvidoContraseña:function(){
        $(".lbl-message").html("");
        $('#lblAlert').hide();
        $('#lblSucess').hide();
        $('#frmLogin').fadeToggle(Autentificacion.Time, function () {
            $('#div-forms').animate(Autentificacion.Time, function () {
                $('#frmEnviarEmail').show();
                $('#email').val('');
                $('#email').focus();
            });
        });
       
    },
    MostrarFormLogin: function () {
        $(".lbl-message").html("");
        $('#lblAlert').html("");
        $('#lblSucess').html("");
        $('#frmEnviarEmail').fadeToggle(Autentificacion.Time, function () {
            $('#div-forms').animate(Autentificacion.Time, function () {
                $('#frmLogin').fadeToggle(Autentificacion.Time);
                $('#NombreUsuario').focus();
            });
    
        });
       
    },
};

$(function () {
    Autentificacion.Inicial();
});