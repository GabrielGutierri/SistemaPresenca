using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TrabalhoInterdisciplinar.Models;

namespace TrabalhoInterdisciplinar.DAO
{
    public abstract class BaseDAO<T> where T : BaseViewModel
    {
        public BaseDAO()
        {
            SetTable();
        }

        protected abstract void SetTable();
        protected string Table { get; set; }
        protected string ListingProcName { get; set; } = "spListagem";
        protected abstract SqlParameter[] CreateParameters(T model);
        protected abstract T BuildModel(DataRow row, bool withJoin);

        public virtual void Insert(T model)
        {
            HelperDAO.ExecuteProc("spInsert_" + Table, CreateParameters(model));
        }

        public virtual void Update(T model)
        {
            HelperDAO.ExecuteProc("spUpdate_" + Table, CreateParameters(model));
        }

        public virtual void Delete(int id)
        {
            var p = new SqlParameter[]
            {
                new SqlParameter("id", id),
                new SqlParameter("tabela", Table)
            };
            HelperDAO.ExecuteProc("spDelete", p);
        }

        public virtual T Query(int id)
        {
            var p = new SqlParameter[]
            {
                new SqlParameter("id", id),
                new SqlParameter("tabela", Table)
            };
            var table = HelperDAO.ExecuteProcSelect("spConsulta", p);
            if (table.Rows.Count == 0)
                return null;
            else
                return BuildModel(table.Rows[0], false);
        }

        public virtual int NextId()
        {
            var p = new SqlParameter[]
            {
                new SqlParameter("tabela", Table)
            };
            var table = HelperDAO.ExecuteProcSelect("spProximoId", p);
            return Convert.ToInt32(table.Rows[0][0]) + 1;
        }

        public virtual List<T> GetListing()
        {
            var p = new SqlParameter[]
            {
                new SqlParameter("tabela", Table),
                new SqlParameter("Ordem", "1")
            };
            var table = HelperDAO.ExecuteProcSelect(ListingProcName, p);
            List<T> list = new List<T>();
            foreach (DataRow row in table.Rows)
                list.Add(BuildModel(row, false));
            return list;
        }

        public virtual List<T> AdvancedQuery(int id)
        {
            SqlParameter[] p =
            {
                new SqlParameter("ID", id),
                new SqlParameter("tabela", Table)
            };
            var table = HelperDAO.ExecuteProcSelect("spConsultaAvancadaGenerica", p);
            var list = new List<T>();
            foreach (DataRow dr in table.Rows)
            {
                list.Add(BuildModel(dr, false));
            }
            return list;
        }
    }
}