using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppOpenGate.Models.Permisos;
using Microsoft.AspNet.Identity;

namespace WebAppOpenGate.Filters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AuthorizeUser : AuthorizeAttribute
    {
        private usuario oUsuario;
        private DB_A3F19C_PruebasEntities1 db = new DB_A3F19C_PruebasEntities1();
        private int idOperacion;

        public AuthorizeUser(int IdOperacion = 0)
        {
            this.idOperacion = IdOperacion;
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            string nombreOperacion = "";
            string nombreModulo = "";

            try
            {
                string user = HttpContext.Current.Session["ua"].ToString();
                oUsuario = db.usuario.Where(x => x.Email == user).FirstOrDefault();
                var listaMisOperaciones = from m in db.roloperacion
                                          where m.rol_Id == oUsuario.rol_Id && m.operaciones_Id == idOperacion
                                          select m;

                if (listaMisOperaciones.ToList().Count() == 0)
                {
                    var operacion = db.operaciones.Find(idOperacion);
                    int? idModulo = operacion.modulo_Id;
                    nombreOperacion = ObtenerNombreOperacion(idOperacion);
                    nombreModulo = ObtenerNombreModulo(idModulo);
                    filterContext.Result = new RedirectResult("~/Error/UnauthorizedOperation?operacion=" + nombreOperacion + "&modulo=" + nombreModulo + "&msjeErrorExcepcion=");
                }
            }
            catch (Exception ex)
            {
                filterContext.Result = new RedirectResult("~/Error/UnauthorizedOperation?operacion=" + nombreOperacion + "&modulo=" + nombreModulo + "&msjeErrorExcepcion=" + ex.Message);
            }
        }


        private string ObtenerNombreOperacion(int IdOperacion)
        {
            string nombreOperacion;

            try
            {
                var operacion = from op in db.operaciones
                                where op.id == IdOperacion
                                select op.Nombre;

                nombreOperacion = operacion.First();

                return nombreOperacion;
            }
            catch (Exception)
            {
                nombreOperacion = "";
            }

            return nombreOperacion;
        }

        public string ObtenerNombreModulo(int? idModulo)
        {
            string nombreModulo;

            try
            {
                var modulo = from m in db.modulo
                             where m.Id == idModulo
                             select m.Nombre;

                nombreModulo = modulo.First();
            }
            catch (Exception)
            {
                nombreModulo = "";
            }

            return nombreModulo;
        }
    }
}