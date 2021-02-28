using Microsoft.Practices.EnterpriseLibrary.Data;
using MNSuperadmin.Connection;
using MNSuperadmin.Helper;
using MNSuperadmin.InterfaceServices;
using MNSuperadmin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MNSuperadmin.UserModels
{
    public class LoginUserModels : IUserDbService
    {
        /// <summary>
        /// Retrieve the user information based on mode
        /// </summary>
        /// <param name="objUserInfo">Pass an instance of User information</param>
        /// <returns>Returns the  user information based on user information</returns>
        public DataTable GetUserInformation(UserInfo objUserInfo)
        {
            Database database;
            DataTable dtableResult = null;

            try
            {
                database = DatabaseConnection.GetDatabase();
                using (var command = database.GetStoredProcCommand("[s_MNLogin]"))
                {
                    database.AddInParameter(command, "@UserName", DbType.String, objUserInfo.UserName);
                    database.AddInParameter(command, "@Password", DbType.String, objUserInfo.Password);
                    database.AddInParameter(command, "@ClientCode", DbType.String, objUserInfo.ClientCode);
                    database.AddInParameter(command, "@mode", DbType.String, objUserInfo.Mode);
                    using (var dataset = new DataSet())
                    {
                        database.LoadDataSet(command, dataset, "dtUserInfo");

                        if (dataset.Tables.Count > 0)
                        {
                            dtableResult = dataset.Tables["dtUserInfo"];
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
                database = null;
                Database.ClearParameterCache();
            }

            return dtableResult;
        }
        public DataSet GetUserInformationDSet(UserInfo objUserInfo)
        {
            Database database;
            DataSet dtset = null;

            try
            {
                database = DatabaseConnection.GetDatabase();
                using (var command = database.GetStoredProcCommand("[s_MNLogin]"))
                {
                    database.AddInParameter(command, "@UserName", DbType.String, objUserInfo.UserName);
                    database.AddInParameter(command, "@Password", DbType.String, objUserInfo.Password);
                    database.AddInParameter(command, "@ClientCode", DbType.String, objUserInfo.ClientCode);
                    database.AddInParameter(command, "@mode", DbType.String, objUserInfo.Mode);
                    string[] tables = new string[] { "dtUserInfo", "dtTransaction" };
                    using (var dataset = new DataSet())
                    {
                        database.LoadDataSet(command, dataset, tables);

                        if (dataset.Tables.Count > 0)
                        {
                            dtset = dataset;
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
                database = null;
                Database.ClearParameterCache();
            }

            return dtset;
        }
        //Agent Profile Information
        public DataSet GetAgentInformationDSet(UserInfo objUserInfo)
        {
            Database database;
            DataSet dtset = null;

            try
            {
                database = DatabaseConnection.GetDatabase();
                using (var command = database.GetStoredProcCommand("[s_MNLogin]"))
                {
                    database.AddInParameter(command, "@UserName", DbType.String, objUserInfo.UserName);
                    database.AddInParameter(command, "@Password", DbType.String, objUserInfo.Password);
                    database.AddInParameter(command, "@ClientCode", DbType.String, objUserInfo.ClientCode);
                    database.AddInParameter(command, "@mode", DbType.String, objUserInfo.Mode);
                    string[] tables = new string[] { "dtUserInfo"};
                    using (var dataset = new DataSet())
                    {
                        database.LoadDataSet(command, dataset, tables);

                        if (dataset.Tables.Count > 0)
                        {
                            dtset = dataset;
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
                database = null;
                Database.ClearParameterCache();
            }

            return dtset;
        }


        public DataTable GetAgentInformation(UserInfo objUserInfo)
        {
            Database database;
            DataTable dtableResult = null;

            try
            {
                database = DatabaseConnection.GetDatabase();
                using (var command = database.GetStoredProcCommand("[s_MNLogin]"))
                {
                    database.AddInParameter(command, "@UserName", DbType.String, objUserInfo.UserName);
                    database.AddInParameter(command, "@Password", DbType.String, objUserInfo.Password);
                    database.AddInParameter(command, "@ClientCode", DbType.String, objUserInfo.ClientCode);
                    database.AddInParameter(command, "@mode", DbType.String, objUserInfo.Mode);
                    using (var dataset = new DataSet())
                    {
                        database.LoadDataSet(command, dataset, "dtUserInfo");

                        if (dataset.Tables.Count > 0)
                        {
                            dtableResult = dataset.Tables["dtUserInfo"];
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
                database = null;
                Database.ClearParameterCache();
            }

            return dtableResult;
        }


        public DataSet GetCustModifiedValue(UserInfo objUserInfo)
        {
            Database database;
            DataSet dtset = null;

            try
            {
                database = DatabaseConnection.GetDatabase();
                using (var command = database.GetStoredProcCommand("[s_MNGetCustModifiedValue]"))
                {

                    database.AddInParameter(command, "@ClientCode", DbType.String, objUserInfo.ClientCode);
                    string[] tables = new string[] { "MNMakerChecker", "InMemMNTransactionAccount" };
                    using (var dataset = new DataSet())
                    {
                        database.LoadDataSet(command, dataset, tables);

                        if (dataset.Tables.Count > 0)
                        {
                            dtset = dataset;
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
                database = null;
                Database.ClearParameterCache();
            }

            return dtset;
        }





        public DataSet GetSuperAdminModifiedValue(UserInfo objUserInfo)
        {
            Database database;
            DataSet dtset = null;

            try
            {
                database = DatabaseConnection.GetDatabase();
                using (var command = database.GetStoredProcCommand("[s_MNGetSuperAdminProfileModifiedValue]"))
                {

                    database.AddInParameter(command, "@ClientCode", DbType.String, objUserInfo.ClientCode);
                    string[] tables = new string[] { "MNMakerChecker" };
                    using (var dataset = new DataSet())
                    {
                        database.LoadDataSet(command, dataset, tables);
                        if (dataset.Tables.Count > 0)
                        {
                            dtset = dataset;
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
                database = null;
                Database.ClearParameterCache();
            }

            return dtset;
        }

        public DataSet GetAdminModifiedValue(UserInfo objUserInfo)
        {
            Database database;
            DataSet dtset = null;

            try
            {
                database = DatabaseConnection.GetDatabase();
                using (var command = database.GetStoredProcCommand("[s_MNGetSuperAdminModifiedValue]"))
                {

                    database.AddInParameter(command, "@ClientCode", DbType.String, objUserInfo.ClientCode);
                    string[] tables = new string[] { "MNMakerChecker" };
                    using (var dataset = new DataSet())
                    {
                        database.LoadDataSet(command, dataset, tables);
                        if (dataset.Tables.Count > 0)
                        {
                            dtset = dataset;
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
                database = null;
                Database.ClearParameterCache();
            }

            return dtset;
        }



        /// <summary>
        /// Retrieve the user information based on mode
        /// </summary>
        /// <param name="objUserInfo">Pass an instance of User information</param>
        /// <returns>Returns the  user information based on user information</returns>
        public DataTable GetAllUserInformation(UserInfo objUserInfo)
        {
            Database database;
            DataTable dtableResult = null;

            try
            {
                database = DatabaseConnection.GetDatabase();
                using (var command = database.GetStoredProcCommand("[s_MNUserInfo]"))
                {
                    database.AddInParameter(command, "@userType", DbType.String, "superadmin");
                    database.AddInParameter(command, "@mode", DbType.String, objUserInfo.Mode);
                    using (var dataset = new DataSet())
                    {
                        database.LoadDataSet(command, dataset, "dtUserInfo");

                        if (dataset.Tables.Count > 0)
                        {
                            dtableResult = dataset.Tables["dtUserInfo"];
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
                database = null;
                Database.ClearParameterCache();
            }

            return dtableResult;
        }

        #region superadmin profile except own
        public DataTable GetSuperadminExceptOwn(UserInfo objUserInfo)
        {
            Database database;
            DataTable dtableResult = null;

            try
            {
                database = DatabaseConnection.GetDatabase();
                using (var command = database.GetStoredProcCommand("[s_MNSuperadminInfo]"))
                {

                    database.AddInParameter(command, "@UserName", DbType.String, objUserInfo.UserName); 
                    database.AddInParameter(command, "@userType", DbType.String, "superadmin");
                    database.AddInParameter(command, "@mode", DbType.String, objUserInfo.Mode);
                    using (var dataset = new DataSet())
                    {
                        database.LoadDataSet(command, dataset, "dtUserInfo");

                        if (dataset.Tables.Count > 0)
                        {
                            dtableResult = dataset.Tables["dtUserInfo"];
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
                database = null;
                Database.ClearParameterCache();
            }

            return dtableResult;
        }
         #endregion

        public int ResetFirstPassword(ViewModel.ResetVM model)
        {
            int ret;
            SqlConnection conn = null;
            SqlTransaction sTrans = null;

            try
            {

                using (conn = new SqlConnection(DatabaseConnection.ConnectionStr()))
                {
                    conn.Open();
                    sTrans = conn.BeginTransaction();
                    using (SqlCommand cmd = new SqlCommand("[s_MNFPasswordReset]", conn,sTrans))
                    {
                        cmd.Parameters.AddWithValue("@ClientCode", model.ClientCode);
                        cmd.Parameters.AddWithValue("@NewPin", model.Pin);
                        cmd.Parameters.AddWithValue("@NewPassword", HashAlgo.Hash(model.Password));

                        //cmd.Parameters.AddWithValue("@NewPassword", model.Password);
                        cmd.Parameters.AddWithValue("@Mode", model.Mode);
                        cmd.Parameters.Add("@RetVal", SqlDbType.Int);
                        cmd.Parameters["@RetVal"].Direction = ParameterDirection.Output;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.ExecuteNonQuery();
                        // read output value from @NewId
                        ret = Convert.ToInt32(cmd.Parameters["@RetVal"].Value);
                        sTrans.Commit();
                        conn.Close();

                    }
                }

            }
            catch (Exception ex)
            {
                sTrans.Rollback();
                throw new Exception(ex.Message);
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }

            return ret;

        }

        public Dictionary<string, string> Checkcredential(string ClientCode)
        {
            int ret;
            SqlConnection conn = null;
            SqlTransaction sTrans = null;
            SqlDataReader rdr =null;
            Dictionary<string, string> test = new Dictionary<string, string>();
            try
            {

                using (conn = new SqlConnection(DatabaseConnection.ConnectionStr()))
                {
                    conn.Open();
                    string Query = "Select PIN,Password FROM MNClientExt (NOLOCK) where ClientCode=@ClientCode";
                    using (SqlCommand cmd = new SqlCommand(Query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ClientCode", ClientCode);
                        cmd.CommandType = CommandType.Text;
                        rdr = cmd.ExecuteReader();
                        rdr.Read();
                        test.Add("PIN", rdr["PIN"].ToString());
                        test.Add("Password", rdr["Password"].ToString());
                        conn.Close();
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
                    conn.Close();
            }
            return test;
        }


        public void LogAction(MNAdminLog adminLog)
        {
            SqlConnection conn = null;
            try
            {
                using (conn = new SqlConnection(Connection.DatabaseConnection.ConnectionStr()))
                {

                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "s_CheckAdminLOG";
                    cmd.Parameters.AddWithValue("@UserName",adminLog.UserId);
                    cmd.Parameters.AddWithValue("@UserAlias", adminLog.UserType);
                    cmd.Parameters.AddWithValue("@URL", adminLog.URL);
                    cmd.Parameters.AddWithValue("@Station", adminLog.IPAddress);
                    cmd.Parameters.AddWithValue("@Message", adminLog.Message);
                    cmd.Parameters.AddWithValue("@Branch", adminLog.Branch);
                    cmd.Parameters.AddWithValue("@Action", adminLog.Action);
                    cmd.Parameters.AddWithValue("@UniqueId", adminLog.UniqueId); 
                    cmd.Parameters.AddWithValue("@PrivateIP", adminLog.PrivateIP);
                    cmd.Parameters.AddWithValue("@ClientDetails", adminLog.ClientDetails);
                    if (conn.State != ConnectionState.Open)
                        conn.Open();
                    cmd.Connection = conn;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
        }


        #region Rejected List for Admin

        public List<UserInfo> GetAllAdminInformation(UserInfo objUserInfo, string isModified)
        {
            List<UserInfo> UserInfos = new List<UserInfo>();
            SqlConnection conn = null;
            SqlDataReader rdr;
            try
            {
                using (conn = new SqlConnection(DatabaseConnection.ConnectionStr()))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        string Command = @"SELECT * FROM v_MNClientDetail WHERE UserType = 'superadmin' AND IsApproved = 'UnApprove' AND IsRejected = 'T'
		                                  AND ISNULL(IsModified,'F')=@IsModified";


                        cmd.Parameters.AddWithValue("@IsModified", isModified) ;
                        cmd.CommandText = Command;
                        cmd.Connection = conn;
                        if (conn.State != ConnectionState.Open)
                        {
                            conn.Open();
                        }
                        rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            UserInfo Info = new UserInfo();
                            Info.UserName = rdr["UserName"].ToString();
                            Info.ClientCode = rdr["ClientCode"].ToString();
                            Info.Name = rdr["Name"].ToString();
                            Info.UserBranchName = rdr["UserBranchName"].ToString();
                            Info.UserBranchCode = rdr["UserBranchCode"].ToString();
                            Info.EmailAddress = rdr["EmailAddress"].ToString();
                            Info.AProfileName = rdr["AProfileName"].ToString();
                            UserInfos.Add(Info);
                        }
                        if (conn.State != ConnectionState.Closed)
                        {
                            conn.Close();
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


            }

            return UserInfos;
        }



        #endregion

        #region Get Self Register Customer Detail
        public DataSet GetSelfRegInfoDSet(CustomerSRInfo objSrInfo)
        {
            Database database;
            DataSet dtset = null;

            try
            {
                database = DatabaseConnection.GetDatabase();
                using (var command = database.GetStoredProcCommand("[s_MNSelfRegApprove]"))
                {

                    database.AddInParameter(command, "@ClientCode", DbType.String, objSrInfo.ClientCode);

                    string[] tables = new string[] { "dtSrInfo" };
                    using (var dataset = new DataSet())
                    {
                        database.LoadDataSet(command, dataset, tables);

                        if (dataset.Tables.Count > 0)
                        {
                            dtset = dataset;
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
                database = null;
                Database.ClearParameterCache();
            }

            return dtset;
        }
        #endregion

        #region Get Self Register Customer Detail
        public DataSet GetSelfRegInfoDSetWalletCustStatus(CustomerSRInfo objSrInfo)
        {
            Database database;
            DataSet dtset = null;

            try
            {
                database = DatabaseConnection.GetDatabase();
                //using (var command = database.GetStoredProcCommand("[s_MNSelfRegApprove]"))
                //using (var command = database.GetStoredProcCommand("[s_MNQuickCustRegStatusApprove]"))
                using (var command = database.GetStoredProcCommand("[s_MNWalletCustStatus]"))
                    
                {

                    database.AddInParameter(command, "@ClientCode", DbType.String, objSrInfo.ClientCode);

                    string[] tables = new string[] { "dtSrInfo" };
                    using (var dataset = new DataSet())
                    {
                        database.LoadDataSet(command, dataset, tables);

                        if (dataset.Tables.Count > 0)
                        {
                            dtset = dataset;
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
                database = null;
                Database.ClearParameterCache();
            }

            return dtset;
        }
        #endregion


        //for agent commission

        #region Get Self Register Customer Detail
        public DataSet GetSelfRegInfoDSetWalletCustStatusAgentCommission(CustomerSRInfo objSrInfo)
        {
            Database database;
            DataSet dtset = null;

            try
            {
                database = DatabaseConnection.GetDatabase();
                using (var command = database.GetStoredProcCommand("[s_MNQuickCustRegStatusApproveAgentCommission]"))
                {

                    database.AddInParameter(command, "@Id", DbType.String, objSrInfo.Id);

                    string[] tables = new string[] { "dtSrInfo" };
                    using (var dataset = new DataSet())
                    {
                        database.LoadDataSet(command, dataset, tables);

                        if (dataset.Tables.Count > 0)
                        {
                            dtset = dataset;
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
                database = null;
                Database.ClearParameterCache();
            }

            return dtset;
        }
        #endregion
        //start milayako 01
        #region Get Agent Register Customer Detail
        public DataSet GetAgentRegInfoDSet(CustomerSRInfo objSrInfo)
        {
            Database database;
            DataSet dtset = null;

            try
            {
                database = DatabaseConnection.GetDatabase();
                using (var command = database.GetStoredProcCommand("[s_MNAgentRegApprove]"))
                {

                    database.AddInParameter(command, "@ClientCode", DbType.String, objSrInfo.ClientCode);

                    string[] tables = new string[] { "dtSrInfo" };
                    using (var dataset = new DataSet())
                    {
                        database.LoadDataSet(command, dataset, tables);

                        if (dataset.Tables.Count > 0)
                        {
                            dtset = dataset;
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
                database = null;
                Database.ClearParameterCache();
            }

            return dtset;
        }
        #endregion
        //end milayako 01


        #region Check PassChanged  superadmin  ///for in reset sending to login after refresh
        public DataTable PassChangedSuperadmin(UserInfo objUserInfo)
        {
            DataTable dtableResult = null;
            SqlConnection conn = null;
            try
            {
                using (conn = new SqlConnection(DatabaseConnection.ConnectionStr()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("[s_MNCheckIsFirstLogin]", conn))
                    {
                        cmd.Parameters.AddWithValue("@ClientCode", objUserInfo.ClientCode);
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

        #region Create Fee

        public int CreateFee(CustomerSRInfo objUserInfo)
        {
            SqlConnection sqlCon = null;
            int ret;
            try
            {
                using (sqlCon = new SqlConnection(DatabaseConnection.ConnectionStr()))
                {
                    sqlCon.Open();
                    using (SqlCommand sqlCmd = new SqlCommand("[s_MNCreateServiceFee]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.Add(new SqlParameter("@FeeID", objUserInfo.FeeId));
                        sqlCmd.Parameters.AddWithValue("@FeeType", objUserInfo.FeeType);
                        sqlCmd.Parameters.AddWithValue("@Details", objUserInfo.Details);
                        sqlCmd.Parameters.AddWithValue("@FlatFee", objUserInfo.FlatFee);
                        sqlCmd.Parameters.AddWithValue("@BranchFee", objUserInfo.BranchFeeAcc);
                        sqlCmd.Parameters.AddWithValue("@FeeAcc", objUserInfo.FeeAcc);
                        sqlCmd.Parameters.AddWithValue("@TieredStart", objUserInfo.TieredStart);
                        sqlCmd.Parameters.AddWithValue("@TieredEnd", objUserInfo.TieredEnd);
                        sqlCmd.Parameters.AddWithValue("@MinAmt", objUserInfo.MinAmt);
                        sqlCmd.Parameters.AddWithValue("@MaxAmt", objUserInfo.MaxAmt);
                        sqlCmd.Parameters.AddWithValue("@Percentage", objUserInfo.Percentage);
                        sqlCmd.Parameters.AddWithValue("@CreatedBy", objUserInfo.CreatedBy);

                        sqlCmd.Parameters.Add("@ReturnValue", SqlDbType.Char, 500);
                        sqlCmd.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;

                        ret = sqlCmd.ExecuteNonQuery();

                        ret = Convert.ToInt32(sqlCmd.Parameters["@ReturnValue"].Value);

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

        #endregion

        #region unApprove Fee List
        internal List<UserInfo> GetFeeApproveDetailInfo()
        {
            List<UserInfo> UserInfos = new List<UserInfo>();
            SqlConnection conn = null;
            SqlDataReader rdr;
            try
            {
                using (conn = new SqlConnection(DatabaseConnection.ConnectionStr()))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        string Command = @"select FD.FeeId,FT.FeeType,FT.Details,FD.Percentage,FT.FlatFee from MNFeeDetails FD INNER JOIN MNFeeTable FT ON FD.FeeId = FT.FeeId where IsApproved IS NULL AND FD.IsRejected = 'F'";
                        
                        cmd.CommandText = Command;
                        cmd.Connection = conn;
                        if (conn.State != ConnectionState.Open)
                        {
                            conn.Open();
                        }
                        rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            UserInfo Info = new UserInfo();
                            Info.FeeID = rdr["FeeId"].ToString();
                            Info.FeeType = rdr["FeeType"].ToString();
                            Info.Details = rdr["Details"].ToString();
                            Info.Percentage = rdr["Percentage"].ToString();
                            Info.FlatFee = rdr["FlatFee"].ToString();
                            UserInfos.Add(Info);
                        }
                        if (conn.State != ConnectionState.Closed)
                        {
                            conn.Close();
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


            }

            return UserInfos;
        }
        #endregion
        
        #region View Fee Details
        public DataTable GetFeeDetail(UserInfo objUserInfo)
        {
            Database database;
            
            DataTable dtableResult = null;

            try
            {
                database = DatabaseConnection.GetDatabase();
                using (var command = database.GetStoredProcCommand("[s_MNGetServiceFeeDetail]"))
                {
                    database.AddInParameter(command, "@FeeID", DbType.String, objUserInfo.FeeID);
                    using (var dataset = new DataSet())
                    {
                        database.LoadDataSet(command, dataset, "dtUserInfo");

                        if (dataset.Tables.Count > 0)
                        {
                            dtableResult = dataset.Tables["dtUserInfo"];
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
                database = null;
                Database.ClearParameterCache();
            }

            return dtableResult;
        }
        #endregion

        #region Fee Registration Approve/Rejected
        // AdminRegApprove
        public int FeeRegApprove(UserInfo objUserInfo, string Approve)
        {
            SqlConnection sqlCon = null;
            int ret;
            try
            {
                using (sqlCon = new SqlConnection(Connection.DatabaseConnection.ConnectionStr()))
                {
                    sqlCon.Open();
                    using (SqlCommand sqlCmd = new SqlCommand("[s_MNApproveFee]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.Add(new SqlParameter("@FeeID", objUserInfo.FeeID));
                        sqlCmd.Parameters.Add(new SqlParameter("@ApprovedBy", objUserInfo.ApprovedBy));
                        sqlCmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.Output;
                        sqlCmd.ExecuteNonQuery();
                        ret = Convert.ToInt32(sqlCmd.Parameters["@ReturnValue"].Value);
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


        public int FeeRegReject(UserInfo objUserInfo, string Rejected)
        {
            SqlConnection sqlCon = null;
            int ret;
            try
            {
                using (sqlCon = new SqlConnection(Connection.DatabaseConnection.ConnectionStr()))
                {
                    sqlCon.Open();
                    using (SqlCommand sqlCmd = new SqlCommand("[s_MNRejectFee]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.Add(new SqlParameter("@FeeID", objUserInfo.FeeID));
                        sqlCmd.Parameters.Add(new SqlParameter("@RejectedBy", objUserInfo.RejectedBy));
                        sqlCmd.Parameters.Add(new SqlParameter("@Remarks", objUserInfo.Remarks));
                        sqlCmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.Output;
                        sqlCmd.ExecuteNonQuery();
                        ret = Convert.ToInt32(sqlCmd.Parameters["@ReturnValue"].Value);
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
        #endregion

        #region Rejected Fee List
        internal List<UserInfo> GetFeeRejectedDetailInfo()
        {
            List<UserInfo> UserInfos = new List<UserInfo>();
            SqlConnection conn = null;
            SqlDataReader rdr;
            try
            {
                using (conn = new SqlConnection(DatabaseConnection.ConnectionStr()))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        string Command = @"select FD.FeeId,FT.FeeType,FT.Details,FD.Percentage,FT.FlatFee from MNFeeDetails FD INNER JOIN MNFeeTable FT ON FD.FeeId = FT.FeeId where IsRejected = 'T' AND FD.IsApproved IS NULL";

                        cmd.CommandText = Command;
                        cmd.Connection = conn;
                        if (conn.State != ConnectionState.Open)
                        {
                            conn.Open();
                        }
                        rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            UserInfo Info = new UserInfo();
                            Info.FeeID = rdr["FeeId"].ToString();
                            Info.FeeType = rdr["FeeType"].ToString();
                            Info.Details = rdr["Details"].ToString();
                            Info.Percentage = rdr["Percentage"].ToString();
                            Info.FlatFee = rdr["FlatFee"].ToString();
                            UserInfos.Add(Info);
                        }
                        if (conn.State != ConnectionState.Closed)
                        {
                            conn.Close();
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


            }

            return UserInfos;
        }

        public int FeeRejectedApprove(UserInfo objUserInfo)
        {
            SqlConnection sqlCon = null;
            int ret;
            try
            {
                using (sqlCon = new SqlConnection(Connection.DatabaseConnection.ConnectionStr()))
                {
                    sqlCon.Open();
                    using (SqlCommand sqlCmd = new SqlCommand("[s_MNRejectedApproveFee]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.Add(new SqlParameter("@FeeID", objUserInfo.FeeID));
                        sqlCmd.Parameters.Add(new SqlParameter("@FeeType", objUserInfo.FeeType));
                        sqlCmd.Parameters.Add(new SqlParameter("@Details", objUserInfo.Details));
                        sqlCmd.Parameters.Add(new SqlParameter("@FlatFee", objUserInfo.FlatFee));
                        sqlCmd.Parameters.Add(new SqlParameter("@BranchFee", objUserInfo.BranchFeeAc));
                        sqlCmd.Parameters.Add(new SqlParameter("@FeeAc", objUserInfo.FeeAc));
                        sqlCmd.Parameters.Add(new SqlParameter("@TieredStart", objUserInfo.TieredStart));
                        sqlCmd.Parameters.Add(new SqlParameter("@TieredEnd", objUserInfo.TieredEnd));
                        sqlCmd.Parameters.Add(new SqlParameter("@MinAmt", objUserInfo.MinAmt));
                        sqlCmd.Parameters.Add(new SqlParameter("@MaxAmt", objUserInfo.MaxAmt));
                        sqlCmd.Parameters.Add(new SqlParameter("@Percentage", objUserInfo.Percentage));
                        sqlCmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.Output;
                        sqlCmd.ExecuteNonQuery();
                        ret = Convert.ToInt32(sqlCmd.Parameters["@ReturnValue"].Value);
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
        #endregion

        #region "Checking Mobile Number"


        public DataTable GetFeeID(UserInfo objUserMobileNoInfo)
        {
            DataTable dtableResult = null;
            SqlConnection conn = null;

            try
            {
                using (conn = new SqlConnection(DatabaseConnection.ConnectionStr()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("[s_MNCheckFeeID]", conn))
                    {
                        cmd.Parameters.AddWithValue("@FeeID", objUserMobileNoInfo.FeeID);
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            // set the CommandTimeout
                            da.SelectCommand.CommandTimeout = 60;  // seconds
                            using (DataSet dataset = new DataSet())
                            {
                                da.Fill(dataset, "dtUserMobileNoInfo");
                                if (dataset.Tables.Count > 0)
                                {
                                    dtableResult = dataset.Tables["dtUserMobileNoInfo"];
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
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