using MNepalAPI.Connection;
using MNepalAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MNepalAPI.UserModel
{
    public class NEAUserModel
    {

        public int NEARequestInfo(NEABranch objresNEAInfo)
        {
            SqlConnection sqlCon = null;
            int ret;
            try
            {
                using (sqlCon = new SqlConnection(DatabaseConnection.ConnectionString()))
                {
                    sqlCon.Open();
                    using (SqlCommand sqlCmd = new SqlCommand("[s_MNNEACheckBill]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.AddWithValue("@serviceId", objresNEAInfo.serviceId);
                        sqlCmd.Parameters.AddWithValue("@serviceCode", objresNEAInfo.serviceCode);
                        sqlCmd.Parameters.AddWithValue("@scn", objresNEAInfo.field1);
                        sqlCmd.Parameters.AddWithValue("@timeStamp", objresNEAInfo.field2);
                        sqlCmd.Parameters.AddWithValue("@customerId", objresNEAInfo.field3);
                        sqlCmd.Parameters.AddWithValue("@amount", objresNEAInfo.field4);
                        sqlCmd.Parameters.AddWithValue("@neaBranchCode", objresNEAInfo.field5);
                        sqlCmd.Parameters.AddWithValue("@userName", objresNEAInfo.userName);
                        sqlCmd.Parameters.AddWithValue("@retrievalReference", objresNEAInfo.retrivalReference);
                        sqlCmd.Parameters.AddWithValue("@additionalData", objresNEAInfo.additionalData);
                       

                        ret = sqlCmd.ExecuteNonQuery();
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                if (sqlCon != null)
                {
                    sqlCon.Close();
                }
            }
            return ret;
        }

    }
}