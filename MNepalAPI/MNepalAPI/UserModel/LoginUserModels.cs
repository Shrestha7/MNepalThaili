using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using MNepalAPI.Connection;
using MNepalAPI.Models;

namespace MNepalAPI.UserModel
{
    public class LoginUserModels
    {
        //FOR PIN
        #region Set PIN Count
        public int SetPINCount(string userName, string mode) //mode 
        {
            SqlConnection sqlCon = null;
            int ret;
            try
            {
                using (sqlCon = new SqlConnection(DatabaseConnection.ConnectionString()))
                {
                    sqlCon.Open();
                    using (SqlCommand sqlCmd = new SqlCommand("[s_MNBlockUserWrongPin]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.Add(new SqlParameter("@UserName", userName));

                        sqlCmd.Parameters.AddWithValue("@mode", mode); //Set Password Count

                        //sqlCmd.Parameters.Add("@UIDOut", SqlDbType.Char, 500);
                        //sqlCmd.Parameters["@UIDOut"].Direction = ParameterDirection.Output;

                        ret = sqlCmd.ExecuteNonQuery();
                        //ret = Convert.ToInt32(sqlCmd.Parameters["@UIDOut"].Value);
                    }

                }
            }
            catch (Exception ex)
            {

                throw;
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
        #endregion

        #region CHECK PIN BLOCK TIME 
        public int GetPINBlockTime(string username)
        {
            SqlConnection conn = null;
            int ret = 1;
            DataTable dtableResult = null;
            try
            {
                using (conn = new SqlConnection(DatabaseConnection.ConnectionString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("[s_MNBlockUserWrongPin]", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserName", username);
                        cmd.Parameters.AddWithValue("@mode", "CBT"); //Check Password Tries

                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            using (DataSet dataset = new DataSet())
                            {
                                da.Fill(dataset, "dtUserInfo");
                                if (dataset.Tables.Count > 0)
                                {
                                    ret = 0;
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

            return ret;
        }
        #endregion

        #region Check Bank Link
        public DataTable CheckUserPin(CheckPin objUserInfo)
        {
            DataTable dtableResult = null;
            SqlConnection conn = null;
            try
            {
                using (conn = new SqlConnection(DatabaseConnection.ConnectionString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("[s_MNCheckTPin]", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserName", objUserInfo.username);
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            // set the CommandTimeout
                            da.SelectCommand.CommandTimeout = 60;  // seconds
                            using (DataSet dataset = new DataSet())
                            {
                                da.Fill(dataset);
                                if (dataset.Tables.Count > 0)
                                {
                                    dtableResult = dataset.Tables[0];
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
                if (conn != null)
                {
                    conn.Close();
                }
            }

            return dtableResult;
        }
        #endregion

    }
}