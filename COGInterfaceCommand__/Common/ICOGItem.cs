using Oracle.ManagedDataAccess.Client;
using System;

namespace COGInterfaceCommand.Common
{
    public interface ICOGItem
    {
        string COGFileName { get; set; }
        DateTime CreatedDate { get; set; }
        void ProcessObject();

        //void AddToDatabase(OracleTransaction trans = null);
        //void UpdateToDatabase();
        //void DeleteFromDatabase();
    }
}
