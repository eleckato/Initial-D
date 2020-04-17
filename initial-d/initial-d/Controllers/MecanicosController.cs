using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using initial_d.APICallers;
using initial_d.Common;
using initial_d.Models.APIModels;

namespace initial_d.Controllers
{
    [Authorize]
    [RoutePrefix("mecanicos")]
    public class MecanicosController : BaseController
    {
        readonly MecanicosRepository MP = new MecanicosRepository();
        readonly UsuariosRepository UP = new UsuariosRepository();

        public MecanicosController()
        {
            SetNavbar();
        }

        [HttpGet]
        [Route]
        public ActionResult MechList()
        {
            List<Mecanico> mecanicos;

            try
            {
                mecanicos = MP.GetAllMech().ToList();
                if (mecanicos == null) return Error_FailedRequest();

                var userTypeLst = UP.GetAllTypes().ToList();
                if (userTypeLst == null) return Error_FailedRequest();

                var userStatusLst = UP.GetAllStatus().ToList();
                if (userStatusLst == null) return Error_FailedRequest();

                mecanicos.ForEach(user =>
                {
                    user = (Mecanico)UP.ProcessUser(user, userTypeLst, userStatusLst);
                });
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            return View(mecanicos);
        }


        private void SetNavbar()
        {
            List<NavbarItems> InternalNavbar = new List<NavbarItems>()
            {
                new NavbarItems("Mecanicos", "MechList", "Listado de Mecánicos"),
            };

            ViewBag.InternalNavbar = InternalNavbar;
        }
    }
}