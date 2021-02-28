using MNepalProject.Connection;
using MNepalProject.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WCF.MNepal.UserModels
{
    public class FeeAmountUserModel
    {
        #region FeeAmount Information

        public DataTable GetFeeAmountCheckInfo(MNResponse objTranIDInfo)
        {
            DataTable dtableResult = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("[s_MNRetRefCheck]", conn))
                    {
                        cmd.Parameters.AddWithValue("@RetRef", objTranIDInfo.RetrievalRef);
                        cmd.Parameters.AddWithValue("@mode", "GFAC"); //Get RetRef

                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            // set the CommandTimeout
                            da.SelectCommand.CommandTimeout = 60;  //seconds
                            using (DataSet dataset = new DataSet())
                            {
                                da.Fill(dataset, "dtTranIDInfo");
                                if (dataset.Tables.Count > 0)
                                {
                                    dtableResult = dataset.Tables["dtTranIDInfo"];
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
    }
}