using MNepalAPI.Connection;
using MNepalProject.Models;
using System;
using System.Data;
using System.Data.SqlClient;

namespace MNepalAPI.UserModel
{
    public class CustCheckerUserModel
    {
        #region Bank Link Customer User Information

        public DataTable GetCustUserInfo(string MobileNumber)
        {
            DataTable dtableResult = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("[s_MNLogin]", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserName", MobileNumber);
                        cmd.Parameters.AddWithValue("@Password", "");
                        cmd.Parameters.AddWithValue("@ClientCode", "");
                        cmd.Parameters.AddWithValue("@mode", "GCBUN"); //Get Check Blocked User Name

                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            // set the CommandTimeout
                            da.SelectCommand.CommandTimeout = 60;  //seconds
                            using (DataSet dataset = new DataSet())
                            {
                                da.Fill(dataset, "dtCustUserInfo");
                                if (dataset.Tables.Count > 0)
                                {
                                    dtableResult = dataset.Tables["dtCustUserInfo"];
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            return dtableResult;
        }

        #endregion

        public DataTable GetCustUserCheckInfo(MNClientExt objCustUserInfo)
        {
            DataTable dtableResult = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("[s_MNLogin]", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserName", objCustUserInfo.UserName);
                        cmd.Parameters.AddWithValue("@Password", "");
                        cmd.Parameters.AddWithValue("@ClientCode", "");
                        cmd.Parameters.AddWithValue("@mode", "GCUN"); //Get Check User Name

                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            // set the CommandTimeout
                            da.SelectCommand.CommandTimeout = 60;  //seconds
                            using (DataSet dataset = new DataSet())
                            {
                                da.Fill(dataset, "dtCustUserInfo");
                                if (dataset.Tables.Count > 0)
                                {
                                    dtableResult = dataset.Tables["dtCustUserInfo"];
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            return dtableResult;
        }

        public DataTable GetMerchantUserCheckInfo(MNClientExt objCustUserInfo)
        {
            DataTable dtableResult = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("[s_MNLogin]", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserName", objCustUserInfo.UserName);
                        cmd.Parameters.AddWithValue("@Password", "");
                        cmd.Parameters.AddWithValue("@ClientCode", "");
                        cmd.Parameters.AddWithValue("@mode", "GMC"); //Get Check User Name

                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            // set the CommandTimeout
                            da.SelectCommand.CommandTimeout = 60;  //seconds
                            using (DataSet dataset = new DataSet())
                            {
                                da.Fill(dataset, "dtCustUserInfo");
                                if (dataset.Tables.Count > 0)
                                {
                                    dtableResult = dataset.Tables["dtCustUserInfo"];
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            return dtableResult;
        }

    }
}