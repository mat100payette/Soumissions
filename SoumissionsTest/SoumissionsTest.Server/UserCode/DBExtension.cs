using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace LightSwitchApplication.UserCode
{
    public static class DBExtension
    {
        private static void CallStoredProc(string procedure)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["_IntrinsicData"].ConnectionString;
                
                using (SqlCommand command = new SqlCommand(procedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteAll()
        {
            //CallStoredProc("dbo.DeleteAll");
        }

        public static void UnifyProduits()
        {
            CallStoredProc("dbo.UnifyProduits");
        }

        public static void UpdateModeles()
        {
            CallStoredProc("dbo.UpdateModeles");
        }

        public static void CleanProduits()
        {
            CallStoredProc("dbo.CleanProduits");
        }
    }
}