﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System;
using TrabalhoInterdisciplinar.Models;
using TrabalhoInterdisciplinar.DAO;
using Microsoft.AspNetCore.Mvc.Filters;
using TrabalhoInterdisciplinar.Enumeradores;
using TrabalhoInterdisciplinar.Helpers;

namespace TrabalhoInterdisciplinar.Controllers
{
    public class ConsultaListagensController : Controller
    {
        protected bool ExigeAutenticacao { get; set; } = true;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (ExigeAutenticacao && !HelperControllers.VerificaProfessorLogado(HttpContext.Session))
                context.Result = RedirectToAction("Index", "Home");
            else
            {
                if (HelperControllers.VerificaProfessorLogado(HttpContext.Session))
                    ViewBag.LogadoProfessor = true;
                else if (HelperControllers.VerificaAlunoLogado(HttpContext.Session))
                    ViewBag.LogadoAluno = true;
                base.OnActionExecuting(context);
            }
        }

        public IActionResult Index()
        {
            try
            {
                AlunoDAO alunodao = new AlunoDAO();
                MateriaDAO materiadao = new MateriaDAO();
                ProfessorDAO professordao = new ProfessorDAO();
                AulaDAO auladao = new AulaDAO();

                ViewBag.Aluno = alunodao.Listagem().Count;
                ViewBag.Professor = professordao.Listagem().Count;
                ViewBag.Materia = materiadao.Listagem().Count;
                ViewBag.Aula = auladao.Listagem().Count;




                return View("Index");
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.Message));
            }
        }

        public IActionResult ObtemDadosConsultaAvancada(EnumTipoTabela tipoTabela, int codigo)
        {
            try
            {
                int tipo = (int)tipoTabela;
                switch (tipoTabela)
                {
                    case EnumTipoTabela.Aluno:
                        return ConsultaAvancadaAluno(codigo);
                    case EnumTipoTabela.Professor:
                        return ConsultaAvancadaProfessor(codigo);
                    case EnumTipoTabela.Materia:
                        return ConsultaAvancadaMateria(codigo);
                    case EnumTipoTabela.Aula:
                        return ConsultaAvancadaAula(codigo);
                    default:
                        throw new Exception("A opção não existe");
                }
            }
            catch (Exception erro)
            {
                return Json(new { erro = true, msg = erro.Message });
            }
        }

        //Fazer herança aqui em baixo
        private IActionResult ConsultaAvancadaMateria(int codigo)
        {
            try
            {
                MateriaDAO dao = new MateriaDAO();
                if (string.IsNullOrEmpty(codigo.ToString()))
                    codigo = 0;
                var lista = dao.ConsultaAvancada(codigo);

                if (lista.Count != 0)
                {
                    return PartialView("pvGridMateria", lista);
                }
                else
                {
                    return PartialView("pvGridSemResultado");
                }
            }
            catch (Exception err)
            {
                return View("Error", new ErrorViewModel(err.ToString()));
            }
        }

        private IActionResult ConsultaAvancadaProfessor(int codigo)
        {
            try
            {
                ProfessorDAO dao = new ProfessorDAO();
                if (string.IsNullOrEmpty(codigo.ToString()))
                    codigo = 0;
                var lista = dao.ConsultaAvancada(codigo);

                if (lista.Count != 0)
                {
                    return PartialView("pvGridProfessor", lista);
                }
                else
                {
                    return PartialView("pvGridSemResultado");
                }
            }
            catch(Exception err)
            {
                return View("Error", new ErrorViewModel(err.ToString()));
            }
        }

        public IActionResult ConsultaAvancadaAluno(int codigo)
        {
            try
            {
                AlunoDAO dao = new AlunoDAO();
                if (string.IsNullOrEmpty(codigo.ToString()))
                    codigo = 0;
                var lista = dao.ConsultaAvancada(codigo);

                if (lista.Count != 0)
                {
                    return PartialView("pvGridAluno", lista);
                }
                else
                {
                    return PartialView("pvGridSemResultado");
                }
                
            }
            catch (Exception erro)
            {
                return Json(new { erro = true, msg = erro.Message });
            }
        }

        public IActionResult ConsultaAvancadaAula(int codigo)
        {
            try
            {

                AulaDAO dao = new AulaDAO();
                if (string.IsNullOrEmpty(codigo.ToString()))
                    codigo = 0;
                var lista = dao.ConsultaAvancada(codigo);

                if (lista.Count != 0)
                {
                    return PartialView("pvGridAula", lista);
                }
                else
                {
                    return PartialView("pvGridSemResultado");
                }
                
            }
            catch (Exception erro)
            {
                return Json(new { erro = true, msg = erro.Message });
            }
        }
    }
}
