using MNepalProject.Connection;
using System;
using System.Data;
using System.Data.SqlClient;
using WCF.MNepal.Models;

namespace WCF.MNepal.UserModels
{
    public class MerchantUserModel
    {
        #region Merchant Information

        public DataTable GetMerchantListInfo(MerchantModel mm)
        {
            DataTable dtableResult = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("[s_MNMerchantList]", conn))
                    {
                        cmd.Parameters.AddWithValue("@catID", mm.CatID);
                        cmd.Parameters.AddWithValue("@mode", "ML");

                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            // set the CommandTimeout
                            da.SelectCommand.CommandTimeout = 60;  //seconds
                            using (DataSet dataset = new DataSet())
                            {
                                da.Fill(dataset, "dtMerchantInfo");
                                if (dataset.Tables.Count > 0)
                                {
                                    dtableResult = dataset.Tables["dtMerchantInfo"];
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

        #region Get Date of Original transaction for topup reversal

        public string GetDate(MerchantModel mm)
        {
            string date = "";
            DataTable dtableResult = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("[s_MNGetOriginalTopupDate]", conn))
                    {

                        cmd.Parameters.AddWithValue("@RetReference", mm.RetReference); 

                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            // set the CommandTimeout
                            da.SelectCommand.CommandTimeout = 60;  //seconds
                            using (DataSet dataset = new DataSet())
                            {
                                da.Fill(dataset, "dtMerchantInfo");
                                if (dataset.Tables.Count > 0)
                                {
                                    dtableResult = dataset.Tables["dtMerchantInfo"];
                                    date = dtableResult.Rows[0]["Date"].ToString();
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

            return date;
        }

        #endregion
    }
}