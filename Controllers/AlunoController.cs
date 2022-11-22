﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TrabalhoInterdisciplinar.ConexãoHelix;
using TrabalhoInterdisciplinar.DAO;
using TrabalhoInterdisciplinar.Helpers;
using TrabalhoInterdisciplinar.Models;

namespace TrabalhoInterdisciplinar.Controllers
{
    public class AlunoController : PadraoController<AlunoViewModel>
    {
        public AlunoController()
        {
            DAO = new AlunoDAO();
            GeraProximoId = true;
        }

        public byte[] ConvertImageToByte(IFormFile file)
        {
            if (file != null)
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    return ms.ToArray();
                }
            else
                return null;
        }

        public override IActionResult Save(AlunoViewModel model, string Operacao)
        {
            try
            {
                ValidaDados(model, Operacao);
                if (ModelState.IsValid == false)
                {
                    ViewBag.Operacao = Operacao;
                    PreencheDadosParaView(Operacao, model);
                    return View(NomeViewForm, model);
                }
                else
                {
                    if (Operacao == "I")
                    {
                        AlunoDAO novoAlunoDAO = new AlunoDAO();
                        model.IdBiometria = novoAlunoDAO.ProximoIdBiometria();
                        DAO.Insert(model);
                        LoginViewModel modelLogin = new LoginViewModel()
                        {
                            ID = model.ID,
                            SenhaHash = PasswordHasher.Encrypt("0001")
                        };
                        LoginDAO login = new LoginDAO();
                        login.Insert(modelLogin);
                        TempData["AlertMessage"] = "Dado salvo com sucesso...! ";
                        ConexaoMQTT conectaHelix = new ConexaoMQTT();
                        conectaHelix.ProvisionaDadosMQTT(model);
                        conectaHelix.RegistraDadosMQTT(model);
                        conectaHelix.PublishCreateMQTT(model);
                        /*  Timer
                        Stopwatch cronometro = new Stopwatch();
                        cronometro.Start();
                        do
                        {
                            Thread.Sleep(200);
                        }
                        while (cronometro.Elapsed.TotalSeconds <= 45);
                        //conectaHelix.RecebeMQTT();
                        */
                        return RedirectToAction("Create");
                    }
                    else
                    {
                        DAO.Update(model);
                        return RedirectToAction("Index", "ConsultaListagens");
                    }
                        
                }
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        protected override void ValidaDados(AlunoViewModel aluno, string operacao)
        {
            base.ValidaDados(aluno, operacao);
            ValidaDadosParteDois(aluno, operacao);
            if (aluno.Imagem == null && operacao == "I")
                ModelState.AddModelError("Imagem", "Escolha uma imagem.");
            if (aluno.Imagem != null && aluno.Imagem.Length / 1024 / 1024 >= 2)
                ModelState.AddModelError("Imagem", "Imagem limitada a 2 mb.");
            if (ModelState.IsValid)
            {
                if (operacao == "A" && aluno.Imagem == null)
                {
                    AlunoViewModel alun = DAO.Consulta(aluno.ID);
                    aluno.ImagemEmByte = alun.ImagemEmByte;
                }
                else
                {
                    aluno.ImagemEmByte = ConvertImageToByte(aluno.Imagem);
                }
            }

        }

        private void ValidaDadosParteDois(AlunoViewModel aluno, string operacao)
        {
            if (string.IsNullOrEmpty(aluno.Nome))
                ModelState.AddModelError("Nome", "Campo obrigatório.");
            if (string.IsNullOrEmpty(aluno.Email))
                ModelState.AddModelError("Email", "Campo obrigatório.");
            else
            {
                Regex validaEmailRegex = new Regex("^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$");
                if (!validaEmailRegex.IsMatch(aluno.Email))
                    ModelState.AddModelError("Email", "Email Inválido.");
            }
            if (string.IsNullOrEmpty(aluno.Telefone))
                ModelState.AddModelError("Telefone", "Campo obrigatório.");
            else
            {
                Regex validaNumeroTelefoneRegex = new Regex("^\\([1-9]{2}\\) (?:[2-8]|9[1-9])[0-9]{3}\\-[0-9]{4}$");
                if (!validaNumeroTelefoneRegex.IsMatch(aluno.Telefone))
                    ModelState.AddModelError("Telefone", "Telefone Inválido.");
            }
            if (string.IsNullOrEmpty(aluno.Cpf))
                ModelState.AddModelError("Cpf", "Campo obrigatório.");
            else
            {
                Regex validaNumeroCPFRegex = new Regex("^\\d{3}\\.\\d{3}\\.\\d{3}-\\d{2}$");
                if (!validaNumeroCPFRegex.IsMatch(aluno.Cpf) || !Auxiliares.ValidaCPF(aluno.Cpf))
                    ModelState.AddModelError("Cpf", "CPF Inválido.");
                if(operacao == "I" && !Auxiliares.VerificaCPFExistente(aluno.Cpf))
                {
                    ModelState.AddModelError("Cpf", "CPF já cadastrado.");
                }
            }
        }

        public override IActionResult Delete(int id)
        {
            try
            {
                DAO.Delete(id);
                LoginDAO login = new LoginDAO();
                login.Delete(id);
                return RedirectToAction("Index", "ConsultaListagens");
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }
    }
}
