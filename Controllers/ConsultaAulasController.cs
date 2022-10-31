﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using TrabalhoInterdisciplinar.DAO;
using TrabalhoInterdisciplinar.Enumeradores;
using TrabalhoInterdisciplinar.Models;

namespace TrabalhoInterdisciplinar.Controllers
{
    public class ConsultaAulasController: Controller
    {
        protected bool ExigeAutenticacao { get; set; } = true;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (ExigeAutenticacao && !HelperControllers.VerificaProfessorLogado(HttpContext.Session))
                context.Result = RedirectToAction("Index", "Home");
            else
            {
                ViewBag.LogadoProfessor = true;
                base.OnActionExecuting(context);
            }
        }

        public IActionResult Index()
        {
            try
            {
                PreparaDadosParaFiltros();
                return View("Index");
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.Message));
            }
        }

        public IActionResult ConsultaAulaAvancada(DateTime dataInicial, DateTime dataFinal, int idMateria, EnumSituacaoAula situacao)
        {
            int situacaoInt = (int)situacao;
            AulaDAO aulaDAO = new AulaDAO();
            if (dataInicial.Date == Convert.ToDateTime("01/01/0001"))
                dataInicial = SqlDateTime.MinValue.Value;
            if (dataFinal.Date == Convert.ToDateTime("01/01/0001"))
                dataFinal = SqlDateTime.MaxValue.Value;
            if (string.IsNullOrEmpty(idMateria.ToString()))
                idMateria = 0;
            var lista = aulaDAO.ConsultaAvancada(dataInicial, dataFinal, idMateria);
            var listaFinal = new List<AulaViewModel>();
            foreach (var item in lista)
            {
                if(DateTime.Now  < item.DataHoraAula.AddMinutes(15) && DateTime.Now > item.DataHoraAula.AddMinutes(-15))
                {
                    item.Situacao = EnumSituacaoAula.Ativo;
                }
                else if(DateTime.Now > item.DataHoraAula.AddMinutes(15))
                {
                    item.Situacao = EnumSituacaoAula.Finalizada;
                }
                else if(DateTime.Now < item.DataHoraAula.AddMinutes(-15))
                {
                    item.Situacao = EnumSituacaoAula.Futura;
                }

                if(situacaoInt == 1 && (int)item.Situacao == 1)
                {
                    listaFinal.Add(item);
                }
                else if(situacaoInt == 2 && (int)item.Situacao == 2)
                {
                    listaFinal.Add(item);
                }
                else if (situacaoInt == 3 && (int)item.Situacao == 3)
                {
                    listaFinal.Add(item);
                }
                else
                {
                    listaFinal.Add(item);
                }
            }
           

            return PartialView("pvGridAula", listaFinal);
        }
         
        private void PreparaDadosParaFiltros()
        {
            MateriaDAO materiaDAO = new MateriaDAO();
            var  materias = materiaDAO.Listagem();
            List<SelectListItem> listaMaterias = new List<SelectListItem>();
            listaMaterias.Add(new SelectListItem("Selecione uma matéria...", "0"));
            foreach (var materia in materias)
            {
                SelectListItem item = new SelectListItem(materia.Descricao, materia.ID.ToString());
                listaMaterias.Add(item);
            }
            ViewBag.Materias = listaMaterias;
        }
    }
}
