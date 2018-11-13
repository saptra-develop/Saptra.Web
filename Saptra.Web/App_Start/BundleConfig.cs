using System.Web;
using System.Web.Optimization;

namespace Saptra.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/lib/jquery/jquery.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                                               "~/Scripts/lib/jquery.validate/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                          "~/Scripts/lib/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/saptra").Include(
                      "~/Scripts/lib/bower_components/bootstrap/dist/js/bootstrap.js",
                      "~/Scripts/lib/bootbox/bootbox.min.js",
                      "~/Scripts/lib/chartjs/Chart.min.js",
                      "~/Scripts/lib/fusioncharts/fusioncharts.js",
                      "~/Scripts/lib/fusioncharts/fusioncharts.charts.js",
                      "~/Scripts/lib/fusioncharts/fusioncharts.widgets.js",
                      "~/Scripts/lib/fileinput/fileinput.min.js",
                      "~/Scripts/lib/underscore/underscore.js",
                      "~/Scripts/lib/backbone/backbone.js",
                      "~/Scripts/lib/bbGrid.js",
                      "~/Scripts/lib/bower_components/metisMenu/dist/metisMenu.js",
                      "~/Scripts/lib/sb-admin.js",
                      "~/Scripts/lib/modernizr-2.6.2.js",
                      "~/Scripts/lib/bower_components/moment/min/moment.min.js",
                      "~/Scripts/lib/bower_components/eonasdan-bootstrap-datetimepicker/build/js/bootstrap-datetimepicker.min.js",
                      "~/Scripts/lib/bower_components/moment/locale/es.js",
                      "~/Scripts/lib/select2-3.5.4/select2.min.js",
                      "~/Scripts/lib/tooltipster-master/dist/js/tooltipster.bundle.min.js",
                      "~/Scripts/lib/bootstrap-switch-master/dist/js/bootstrap-switch.js",
                      "~/Scripts/lib/jquery.floatThead.min.js",
                      "~/Scripts/lib/bootstrap-notify.js",
                      "~/Scripts/lib/json-viewer/jquery.json-viewer.js",
                      "~/Scripts/lib/circularProgress.js",
                      "~/Scripts/lib/toogle/bootstrap-toggle.min.js"
                      ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/OpenSans.css",
                      "~/Scripts/lib/bower_components/bootstrap/dist/css/bootstrap.css",
                      "~/Scripts/lib/bower_components/metisMenu/dist/metisMenu.min.css",
                      "~/Scripts/lib/bower_components/font-awesome/css/font-awesome.min.css",
                      "~/Scripts/lib/bower_components/eonasdan-bootstrap-datetimepicker/build/css/bootstrap-datetimepicker.min.css",
                      "~/Content/bbGrid.css",
                      "~/Scripts/lib/fileinput/fileinput.min.css",
                      "~/Scripts/lib/select2-3.5.4/select2.css",
                      "~/Scripts/lib/select2-3.5.4/select2-bootstrap.css",
                      "~/Scripts/lib/tooltipster-master/dist/css/tooltipster.bundle.css",
                      "~/Scripts/lib/bootstrap-switch-master/dist/css/bootstrap3/bootstrap-switch.css",
                      "~/Content/animate.css",
                      "~/Scripts/lib/json-viewer/jquery.json-viewer.css",
                      "~/Scripts/lib/toogle/bootstrap-toggle.min.css",
                      "~/Content/site.css",
                      "~/Content/style4.css"));

            //Logon Bundles
            bundles.Add(new ScriptBundle("~/bundles/Logon").Include(
                  "~/Scripts/lib/jquery/jquery.js",
                  "~/Scripts/lib/jquery.validate/jquery.validate*",
                  "~/Scripts/lib/bower_components/bootstrap/dist/js/bootstrap.js",
                //"~/Scripts/lib/bower_components/metisMenu/dist/metisMenu.min.js",
                //"~/Scripts/lib/sb-admin-2.js",
                //"~/Scripts/lib/modernizr.custom.86080.js",

                "~/Scripts/lib/loginv16/js/main.js",
                "~/Scripts/lib/loginv16/vendor/animsition/js/animsition.min.js",
                "~/Scripts/lib/loginv16/vendor/bootstrap/js/popper.js",
                "~/Scripts/lib/select2-3.5.4/select2.min.js",
                "~/Scripts/lib/loginv16/vendor/daterangepicker/moment.min.js",
                "~/Scripts/lib/loginv16/vendor/daterangepicker/daterangepicker.js",
                "~/Scripts/lib/loginv16/vendor/countdowntime/countdowntime.js"
                  ));

            bundles.Add(new StyleBundle("~/Content/cssLogon").Include(
                    "~/Scripts/lib/bower_components/bootstrap/dist/css/bootstrap.css",
                    //"~/Scripts/lib/bower_components/metisMenu/dist/metisMenu.min.css",
                    "~/Scripts/lib/bower_components/font-awesome/css/font-awesome.min.css",
                    //"~/Content/Site.css",

                    "~/Scripts/lib/loginv16/css/main.css",
                    "~/Scripts/lib/loginv16/css/util.css",
                    "~/Scripts/lib/loginv16/vendor/animate/animate.css",
                    "~/Scripts/lib/loginv16/vendor/animsition/css/animsition.min.css",
                    "~/Scripts/lib/loginv16/vendor/css-hamburgers/hamburgers.min.css",
                    "~/Scripts/lib/select2-3.5.4/select2.css",
                    "~/Scripts/lib/loginv16/vendor/daterangepicker/daterangepicker.css",
                    "~/Scripts/lib/loginv16/fonts/font-awesome-4.7.0/css/font-awesome.min.css",
                    "~/Scripts/lib/loginv16/fonts/Linearicons-Free-v1.0.0/icon-font.min.css"
                    ));
        }
    }
}
