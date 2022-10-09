﻿using System.Data;
using System.Data.SqlClient;
using TrabalhoInterdisciplinar.Models;

namespace TrabalhoInterdisciplinar.DAO
{
    public class MateriaDAO: PadraoDAO<MateriaViewModel>
    {
        protected override SqlParameter[] CriaParametros(MateriaViewModel model)
        {
            SqlParameter[] parametros = new SqlParameter[5];
            //parametros[0] = new SqlParameter("id", model.ID);
            //parametros[1] = new SqlParameter("nome", model.Nome);
            //parametros[2] = new SqlParameter("email", model.Email);
            //parametros[3] = new SqlParameter("telefone", model.Telefone);
            //parametros[4] = new SqlParameter("cpf", model.CPF);
            return parametros;
        }

        protected override MateriaViewModel MontaModel(DataRow registro)
        {
            MateriaViewModel a = new MateriaViewModel();
            //{
            //    ID = Convert.ToInt32(registro["id"]),
            //    Nome = registro["nome"].ToString(),
            //    Email = registro["email"].ToString(),
            //    Telefone = Convert.ToInt32(registro["telefone"]),
            //    CPF = Convert.ToInt32(registro["cpf"])
            //};
            return a;
        }

        protected override void SetTabela()
        {
            Tabela = "Aluno";
        }
    }
}
