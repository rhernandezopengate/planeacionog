using System.Web;
using System.Web.Optimization;

namespace WebAppOpenGate
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información. De este modo, estará
            // para la producción, use la herramienta de compilación disponible en https://modernizr.com para seleccionar solo las pruebas que necesite.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                     "~/vendors/bootstrap/dist/css/bootstrap.min.css",
                     "~/vendors/nprogress/nprogress.css",
                     "~/vendors/iCheck/skins/flat/green.css",
                     "~/build/css/custom.min.css"                     
                     ));


            bundles.Add(new ScriptBundle("~/vendors/Scripts").Include(
                      "~/vendors/jquery/dist/jquery.min.js",
                      "~/vendors/bootstrap/dist/js/bootstrap.min.js",
                      "~/vendors/fastclick/lib/fastclick.js",
                      "~/vendors/nprogress/nprogress.js",
                      "~/vendors/skycons/skycons.js",
                      "~/vendors/iCheck/icheck.js",
                      "~/build/js/custom.min.js"
                      ));      
        }
    }
}
