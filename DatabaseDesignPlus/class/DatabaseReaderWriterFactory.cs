using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;

namespace DatabaseDesignPlus
{
	public class DatabaseReaderWriterFactory
	{
        public static void FillCombox(DataTable datatable, string sField, ComboBox pCombox)
        {
            pCombox.Items.Clear();

            foreach (DataRow dr in datatable.Rows)
            {
                string pName = Convert.ToString(dr[sField]);
                pCombox.Items.Add(pName);

            }
        }
        public static void FillCombox(List<string> pFiledsList, ComboBox pCombox,List<string>sFilter)
        {
            pCombox.Items.Clear();

            if (pFiledsList != null && pFiledsList.Count > 0)
            {
                int pCount = pFiledsList.Count;
                for (int i = 0; i < pCount; i++)
                {
                    string pName = pFiledsList[i];
                    foreach (string filter in sFilter)
                    {
                        if (pName.Contains(filter))
                        {
                            pCombox.Items.Add(pName);
                        }
                    }
                }
            }
        }
        public static void FillCombox(List<string> pFiledsList, ComboBox pCombox)
        {
            pCombox.Items.Clear();

            if (pFiledsList != null && pFiledsList.Count > 0)
            {
                int pCount = pFiledsList.Count;
                for (int i = 0; i < pCount; i++)
                {
                    string pName = pFiledsList[i];
                            pCombox.Items.Add(pName);

                }
            }
        }
        public static IDatabaseReaderWriter  GetDatabaseReaderWriter(string DbReaderWriterName,string DatatbaseConnectionID)
        {
            IDatabaseReaderWriter dbReaderWriter = null;

            if (DbReaderWriterName == "PostgreSQL")
            {
                dbReaderWriter = new ClsPostgreSql(DatatbaseConnectionID);
            }else if(DbReaderWriterName == "Excel")
            {
                dbReaderWriter = new ClsExcel(DatatbaseConnectionID);
            }else if(DbReaderWriterName == "Mdb")
            {
                dbReaderWriter = new Clsmdb(DatatbaseConnectionID);
            }
            return dbReaderWriter;
        }
	}
}
