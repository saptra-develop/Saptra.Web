using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saptra.Web.Data
{
    public class SeguridadData
    {

        //public static List<Sispro.Web.Models.MenuGrupo> CargaUsuarioPermisos(int idUsuario)
        //{
        //    var listaMenuGrupo = new List<Sispro.Web.Models.MenuGrupo>();

        //    Sispro.Web.Models.MenuGrupo grupo = null;

        //    Sispro.Web.Datos.Usuarios user = null;

        //    using (charly_plmEntities db = new charly_plmEntities())
        //    {
        //        var result = (from U in db.mUsuarios
        //                      where U.idUsuario == idUsuario
        //                      select U).FirstOrDefault();

        //        user = result;

        //    }

        //    if (user.idRol == 1)
        //    {
        //        grupo = new Sispro.Web.Models.MenuGrupo()
        //        {
        //            nombreGrupo = "Biblioteca",
        //            iconGrupo = "fa-database",
        //            lstPermisos = new List<Sispro.Web.Models.Permisos>{
        //                new Sispro.Web.Models.Permisos() { 
        //                    nombreModulo = "menuGeneralTemporadas",
        //                    lecturaPermisos = 1,
        //                    escrituraPermisos = 1,
        //                    borradoPermisos = 0,
        //                    clonadoPermisos = 0,
        //                    urlModulo = "Temporada/Index"
        //                },
        //                new Sispro.Web.Models.Permisos() { 
        //                    nombreModulo = "menuGeneralProyectos",
        //                    lecturaPermisos = 1,
        //                    escrituraPermisos = 1,
        //                    borradoPermisos = 1,
        //                    clonadoPermisos = 1,
        //                    urlModulo = "Proyecto/Index"
        //                }
        //            }
        //        };
        //        listaMenuGrupo.Add(grupo);
        //        grupo = new Sispro.Web.Models.MenuGrupo()
        //        {
        //            nombreGrupo = "Planeación",
        //            iconGrupo = "fa-book",
        //            lstPermisos = new List<Sispro.Web.Models.Permisos>{
        //                new Sispro.Web.Models.Permisos() { 
        //                    nombreModulo = "menuPlaneacionPlanPortafolio",
        //                    lecturaPermisos = 1,
        //                    escrituraPermisos = 1,
        //                    borradoPermisos = 1,
        //                    clonadoPermisos = 1,
        //                    urlModulo = "PlanPortafolio/Admin"
        //                }
        //            }
        //        };

        //        listaMenuGrupo.Add(grupo);
        //    }
        //    else 
        //    {
        //        grupo = new Sispro.Web.Models.MenuGrupo()
        //        {
        //            nombreGrupo = "General",
        //            iconGrupo = "fa-circle-o",
        //            lstPermisos = new List<Sispro.Web.Models.Permisos>{
        //                new Sispro.Web.Models.Permisos() { 
        //                    nombreModulo = "menuPlaneacionPlanPortafolio",
        //                    lecturaPermisos = 1,
        //                    escrituraPermisos = 0,
        //                    borradoPermisos = 0,
        //                    clonadoPermisos = 0,
        //                    urlModulo = "PlanPortafolio/Index"
        //                },
        //            }
        //        };
        //        listaMenuGrupo.Add(grupo);
        //    }

        //    return listaMenuGrupo;
        //}


        public static List<Saptra.Web.Models.MenuGrupo> CargaUsuarioPermisos(int idUsuario)
        {
            var listaMenuGrupo = new List<Saptra.Web.Models.MenuGrupo>();

            Saptra.Web.Models.MenuGrupo grupo = null;

            using (Inaeba_SaptraEntities db = new Inaeba_SaptraEntities())
            {
                var result = (from P in db.mPermisos
                              join U in db.mUsuarios
                              on P.RolId equals U.RolId
                              where U.UsuarioId == idUsuario && P.LecturaPermiso == true
                                   && P.dModulos.EstatusId == 5
                              select new
                              {
                                  idGrupo = P.dModulos.MenuGrupoId,
                                  nombreGrupo = P.dModulos.mMenuGrupo.NombreMenuGrupo,
                                  P.dModulos.mMenuGrupo.IconGrupo,
                                  P.dModulos.mMenuGrupo.OrdenGrupo,
                                  P.ModuloId,
                                  P.dModulos.NombreModulo,
                                  P.dModulos.UrlModulo,
                                  P.dModulos.IconModulo,
                                  P.dModulos.OrdenModulo,
                                  P.LecturaPermiso,
                                  P.EscrituraPermiso,
                                  P.EdicionPermiso,
                                  P.ClonadoPermiso,
                                  P.BorradoPermiso
                              }).OrderBy(P => P.OrdenGrupo)
                               .ThenBy(P => P.OrdenModulo)
                               .ToList();

                string grupoAux = string.Empty;

                foreach (var item in result)
                {
                    if (item.nombreGrupo != grupoAux)
                    {
                        if (grupoAux != string.Empty)
                            listaMenuGrupo.Add(grupo);

                        grupo = new Models.MenuGrupo()
                        {
                            nombreGrupo = item.nombreGrupo,
                            iconGrupo = item.IconGrupo,
                            lstPermisos = new List<Models.Permisos>()
                        };
                        grupoAux = item.nombreGrupo;
                    }
                    grupo.lstPermisos.Add(new Models.Permisos()
                    {
                        nombreModulo = item.NombreModulo,
                        lecturaPermisos = item.LecturaPermiso.Value ? 1 : 0,
                        escrituraPermisos = item.EscrituraPermiso.Value ? 1 : 0,
                        borradoPermisos = item.BorradoPermiso.Value ? 1 : 0,
                        clonadoPermisos = item.ClonadoPermiso.Value ? 1 : 0,
                        urlModulo = item.UrlModulo
                    });
                }
            }
            listaMenuGrupo.Add(grupo);

            return listaMenuGrupo;
        }

    }
}
