using MNepalProject.Connection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WCF.MNepal.Models;

namespace WCF.MNepal.UserModels
{
    public class ErrorUserModel
    {
        public DataTable GetErrorInformation(ErrorInfo objErrorInfo)
        {
            SqlConnection conn = null;
            DataTable dtableResult = null;

            try
            {
                using (conn = new SqlConnection(DatabaseConnection.ConnectionString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("[s_MNErrorMsgCheck]", conn))
                    {
                        cmd.Parameters.AddWithValue("@ErrorCode", objErrorInfo.ErrorCode);
                        cmd.Parameters.AddWithValue("@Mode", objErrorInfo.Mode);
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            using (DataSet dataset = new DataSet())
                            {
                                da.Fill(dataset, "dtErrorInfo");
                                if (dataset.Tables.Count > 0)
                                {
                                    dtableResult = dataset.Tables["dtErrorInfo"];
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return dtableResult;
        }
    }
}