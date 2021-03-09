using MNepalAPI.Connection;
using MNepalAPI.Models;
using System;
using System.Data;
using System.Data.SqlClient;

namespace MNepalAPI.UserModels
{
    public class SMSUserModels
    {
        public DataTable GetSMSInformation(SMSMsg objUserInfo)
        {
            SqlConnection conn = null;
            DataTable dtableResult = null;

            try
            {
                using (conn = new SqlConnection(DatabaseConnection.ConnectionString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("[s_MNSMSAlert]", conn))
                    {
                        cmd.Parameters.AddWithValue("@AlertType", objUserInfo.AlertType);
                        cmd.Parameters.AddWithValue("@mode", objUserInfo.Mode);
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            using (DataSet dataset = new DataSet())
                            {
                                da.Fill(dataset, "dtSMSInfo");
                                if (dataset.Tables.Count > 0)
                                {
                                    dtableResult = dataset.Tables["dtSMSInfo"];
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