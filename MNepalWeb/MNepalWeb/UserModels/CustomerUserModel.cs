﻿using Microsoft.Practices.EnterpriseLibrary.Data;
using MNepalWeb.Connection;
using MNepalWeb.Models;
using MNepalWeb.Utilities;
using MNepalWeb.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
namespace MNepalWeb.UserModels
{
    public class CustomerUserModel
    {
        /// <summary>
        /// Retrieve the customer detail information based on mode
        /// </summary>
        /// <param name="objUserInfo">Pass an instance of User information</param>
        /// <returns>Returns the customer detail information based on customer information</returns>
        public DataTable GetCustomerDetailInformation(UserInfo objUserInfo)
        {
            Database database;
            DataTable dtableResult = null;

            try
            {
                database = DatabaseConnection.GetDatabase();
                using (var command = database.GetStoredProcCommand("[s_MNUserInfoSearch]"))
                {
                    database.AddInParameter(command, "@MobileNo", DbType.String, objUserInfo.ContactNumber1);
                    database.AddInParameter(command, "@Name", DbType.String, objUserInfo.Name);
                    database.AddInParameter(command, "@WalletNumber", DbType.String, objUserInfo.WalletNumber);
                    database.AddInParameter(command, "@ClientCode", DbType.String, objUserInfo.ClientCode);
                    database.AddInParameter(command, "@mode", DbType.String, objUserInfo.Mode);
                    using (var dataset = new DataSet())
                    {
                        database.LoadDataSet(command, dataset, "dtCustomerInfo");

                        if (dataset.Tables.Count > 0)
                        {
                            dtableResult = dataset.Tables["dtCustomerInfo"];
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

        /// <summary>
        /// Retrieve the user information based on mode
        /// </summary>
        /// <param name="objCustomerUserInfo">Pass an instance of User information</param>
        /// <returns>Returns the pin information based on user information</returns>
        public int UpdateCustomerUserInfo(UserInfo objCustomerUserInfo)
        {
            DateTime defaultdate = new DateTime(1900, 01, 01);
            SqlConnection sqlCon = null;
            int ret;
            try
            {
                using (sqlCon = new SqlConnection(Connection.DatabaseConnection.ConnectionStr()))
                {
                    sqlCon.Open();
                    using (SqlCommand sqlCmd = new SqlCommand("[s_MNBankCustomerInfoUpdate]", sqlCon))  //Procedure Changed-- Nischal
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.Add(new SqlParameter("@ClientCode", objCustomerUserInfo.ClientCode));
                        sqlCmd.Parameters.Add(new SqlParameter("@Name", objCustomerUserInfo.Name));

                        //sqlCmd.Parameters.Add(new SqlParameter("@FName"," " ));
                        //sqlCmd.Parameters.Add(new SqlParameter("@MName", " "));
                        //sqlCmd.Parameters.Add(new SqlParameter("@LName", " "));

                        //---NISCHAL
                        sqlCmd.Parameters.Add(new SqlParameter("@Gender", objCustomerUserInfo.Gender));
                        //sqlCmd.Parameters.Add(new SqlParameter("@DateOfBirth", DateTime.Parse(objCustomerUserInfo.DOB)));
                        sqlCmd.Parameters.Add(new SqlParameter("@DateOfBirth", DateTime.ParseExact(objCustomerUserInfo.DOB, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                                                .ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)));
                        sqlCmd.Parameters.Add(new SqlParameter("@Nationality", objCustomerUserInfo.Nationality));
                        sqlCmd.Parameters.Add(new SqlParameter("@CountryCode", objCustomerUserInfo.Country));
                        sqlCmd.Parameters.Add(new SqlParameter("@FathersName", objCustomerUserInfo.FatherName));
                        sqlCmd.Parameters.Add(new SqlParameter("@MothersName", objCustomerUserInfo.MotherName));
                        sqlCmd.Parameters.Add(new SqlParameter("@GFathersName", objCustomerUserInfo.GrandFatherName));
                        sqlCmd.Parameters.Add(new SqlParameter("@Occupation", objCustomerUserInfo.Occupation));
                        sqlCmd.Parameters.Add(new SqlParameter("@MaritalStatus", objCustomerUserInfo.MaritalStatus));
                        sqlCmd.Parameters.Add(new SqlParameter("@SpouseName", objCustomerUserInfo.SpouseName));
                        sqlCmd.Parameters.Add(new SqlParameter("@FatherInLaw", objCustomerUserInfo.FatherInlawName));
                        sqlCmd.Parameters.Add(new SqlParameter("@DocType", objCustomerUserInfo.Document));
                        sqlCmd.Parameters.Add(new SqlParameter("@CitizenshipNo", objCustomerUserInfo.Citizenship));
                        sqlCmd.Parameters.Add(new SqlParameter("@CitizenPlaceOfIssue", objCustomerUserInfo.CitizenshipPlaceOfIssue));
                        sqlCmd.Parameters.Add(new SqlParameter("@PassportNo", objCustomerUserInfo.Passport));
                        sqlCmd.Parameters.Add(new SqlParameter("@PassportPlaceOfIssue", objCustomerUserInfo.PassportPlaceOfIssue));
                        sqlCmd.Parameters.Add(new SqlParameter("@LicenseNo", objCustomerUserInfo.License));
                        sqlCmd.Parameters.Add(new SqlParameter("@LicensePlaceOfIssue", objCustomerUserInfo.LicensePlaceOfIssue));
                        sqlCmd.Parameters.Add(new SqlParameter("@PassportImage", objCustomerUserInfo.PassportImage));
                        sqlCmd.Parameters.Add(new SqlParameter("@FrontImage", objCustomerUserInfo.FrontImage));
                        sqlCmd.Parameters.Add(new SqlParameter("@BackImage", objCustomerUserInfo.BackImage));
                        if (objCustomerUserInfo.CitizenshipIssueDate == "" || objCustomerUserInfo.CitizenshipIssueDate == "01 Jan 1900")
                        {
                            sqlCmd.Parameters.AddWithValue("@CitizenIssueDate", defaultdate);
                        }
                        else
                        {
                            sqlCmd.Parameters.AddWithValue("@CitizenIssueDate", DateTime.ParseExact(objCustomerUserInfo.CitizenshipIssueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                                                .ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                        }

                        if (objCustomerUserInfo.LicenseExpireDate == "" || objCustomerUserInfo.LicenseIssueDate == "" || objCustomerUserInfo.LicenseExpireDate == "01 Jan 1900" || objCustomerUserInfo.LicenseIssueDate == "01 Jan 1900")
                        {
                            sqlCmd.Parameters.AddWithValue("@LicenseExpiryDate", defaultdate);
                            sqlCmd.Parameters.AddWithValue("@LicenseIssueDate", defaultdate);
                        }
                        else
                        {
                            sqlCmd.Parameters.AddWithValue("@LicenseIssueDate", DateTime.ParseExact(objCustomerUserInfo.LicenseIssueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                                                .ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                            sqlCmd.Parameters.AddWithValue("@LicenseExpiryDate", DateTime.ParseExact(objCustomerUserInfo.LicenseExpireDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                                                .ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));


                        }




                        if (objCustomerUserInfo.PassportExpireDate == "" || objCustomerUserInfo.PassportIssueDate == "" || objCustomerUserInfo.PassportExpireDate == "01 Jan 1900" || objCustomerUserInfo.PassportIssueDate == "01 Jan 1900")
                        {
                            sqlCmd.Parameters.AddWithValue("@PassportIssueDate", defaultdate);
                            sqlCmd.Parameters.AddWithValue("@PassportExpiryDate", defaultdate);
                        }
                        else
                        {
                            sqlCmd.Parameters.AddWithValue("@PassportIssueDate", DateTime.ParseExact(objCustomerUserInfo.PassportIssueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                                                .ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                            sqlCmd.Parameters.AddWithValue("@PassportExpiryDate", DateTime.ParseExact(objCustomerUserInfo.PassportExpireDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                                                .ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                        }

                        //---END--- NISCHAL--



                        sqlCmd.Parameters.Add(new SqlParameter("@ProfileCode", objCustomerUserInfo.ProfileCode));
                        sqlCmd.Parameters.Add(new SqlParameter("@EmailAddress", objCustomerUserInfo.EmailAddress));
                        sqlCmd.Parameters.Add(new SqlParameter("@AcBranchCode", objCustomerUserInfo.BranchCode));
                        sqlCmd.Parameters.Add(new SqlParameter("@BankNumber", objCustomerUserInfo.BankNo));
                        sqlCmd.Parameters.Add(new SqlParameter("@BankAccountNumber", objCustomerUserInfo.BankAccountNumber));
                        //sqlCmd.Parameters.Add(new SqlParameter("@COC", objCustomerUserInfo.COC));
                        sqlCmd.Parameters.Add(new SqlParameter("@mode", objCustomerUserInfo.Mode));
                        sqlCmd.Parameters.Add(new SqlParameter("@Address", objCustomerUserInfo.Address));

                        sqlCmd.Parameters.Add(new SqlParameter("@CProvince", objCustomerUserInfo.CProvince));
                        sqlCmd.Parameters.Add(new SqlParameter("@CDistrictID", objCustomerUserInfo.CDistrictID));
                        sqlCmd.Parameters.Add(new SqlParameter("@CMunicipalityVDC", objCustomerUserInfo.CMunicipalityVDC));
                        sqlCmd.Parameters.Add(new SqlParameter("@CWardNo", objCustomerUserInfo.CWardNo));
                        sqlCmd.Parameters.Add(new SqlParameter("@CHouseNo", objCustomerUserInfo.CHouseNo));
                        sqlCmd.Parameters.Add(new SqlParameter("@CStreet", objCustomerUserInfo.CStreet));

                        sqlCmd.Parameters.Add(new SqlParameter("@PProvince", objCustomerUserInfo.PProvince));
                        sqlCmd.Parameters.Add(new SqlParameter("@PDistrictID", objCustomerUserInfo.PDistrictID));
                        sqlCmd.Parameters.Add(new SqlParameter("@PMunicipalityVDC", objCustomerUserInfo.PMunicipalityVDC));
                        sqlCmd.Parameters.Add(new SqlParameter("@PWardNo", objCustomerUserInfo.PWardNo));
                        sqlCmd.Parameters.Add(new SqlParameter("@PHouseNo", objCustomerUserInfo.PHouseNo));
                        sqlCmd.Parameters.Add(new SqlParameter("@PStreet", objCustomerUserInfo.PStreet));

                        sqlCmd.Parameters.Add(new SqlParameter("@NewMobileNo", objCustomerUserInfo.NewMobileNo));
                        sqlCmd.Parameters.Add(new SqlParameter("@ContactNumber1", objCustomerUserInfo.ContactNumber1));
                        sqlCmd.Parameters.Add(new SqlParameter("@ContactNumber2", objCustomerUserInfo.ContactNumber2));
                        sqlCmd.Parameters.Add(new SqlParameter("@Pin", " "));
                        sqlCmd.Parameters.Add(new SqlParameter("@ClientStatus", objCustomerUserInfo.ClientStatus));
                        sqlCmd.Parameters.Add(new SqlParameter("@Status", objCustomerUserInfo.Status));
                        sqlCmd.Parameters.Add(new SqlParameter("@TxnAccounts", objCustomerUserInfo.TxnAccounts));
                        sqlCmd.Parameters.Add(new SqlParameter("@ModifyingBranch", objCustomerUserInfo.AdminBranch));
                        sqlCmd.Parameters.Add(new SqlParameter("@ModifiedBy", objCustomerUserInfo.AdminUserName));
                        sqlCmd.Parameters.Add(new SqlParameter("@IsApproved", objCustomerUserInfo.IsApproved));
                        //Added//
                        sqlCmd.Parameters.Add(new SqlParameter("@IndvTxn", objCustomerUserInfo.Transaction));
                        sqlCmd.Parameters.Add(new SqlParameter("@DateRange", objCustomerUserInfo.DateRange));
                        sqlCmd.Parameters.Add(new SqlParameter("@StartDate", objCustomerUserInfo.StartDate));
                        sqlCmd.Parameters.Add(new SqlParameter("@EndDate", objCustomerUserInfo.EndDate));
                        sqlCmd.Parameters.Add(new SqlParameter("@TransactionLimit", objCustomerUserInfo.TransactionLimit));
                        sqlCmd.Parameters.Add(new SqlParameter("@LimitType", objCustomerUserInfo.LimitType));
                        sqlCmd.Parameters.Add(new SqlParameter("@TransactionLimitMonthly", objCustomerUserInfo.TransactionLimitMonthly));
                        sqlCmd.Parameters.Add(new SqlParameter("@TransactionCount", objCustomerUserInfo.TransactionCount));
                        sqlCmd.Parameters.Add(new SqlParameter("@TransactionLimitDaily", objCustomerUserInfo.TransactionLimitDaily));
                        ////////
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





            //Database database;
            //int ret;

            //try
            //{
            //    database = DatabaseConnection.GetDatabase();
            //    using (var command = database.GetStoredProcCommand("[s_MNAdminInfoUpdate]"))
            //    {
            //        database.AddInParameter(command, "@ClientCode", DbType.String, objCustomerUserInfo.ClientCode);
            //        //database.AddInParameter(command, "@Address", DbType.String, objCustomerUserInfo.Address);
            //        //database.AddInParameter(command, "@Status", DbType.String, objCustomerUserInfo.Status);
            //        database.AddInParameter(command, "@ProfileName", DbType.String, objCustomerUserInfo.ProfileName);
            //        database.AddInParameter(command, "@Name", DbType.String, objCustomerUserInfo.Name);
            //        database.AddInParameter(command, "@EmailAddress", DbType.String, objCustomerUserInfo.EmailAddress);
            //        database.AddInParameter(command, "@BranchCode", DbType.String, objCustomerUserInfo.BranchCode);
            //        database.AddInParameter(command, "@COC", DbType.String, objCustomerUserInfo.COC);
            //        database.AddInParameter(command, "@ReturnValue", DbType.Int32, objCustomerUserInfo.ReturnValue);


            //        //database.AddInParameter(command, "@ContactNumber1", DbType.String, objCustomerUserInfo.ContactNumber1);
            //        //database.AddInParameter(command, "@ContactNumber2", DbType.String, objCustomerUserInfo.ContactNumber2);
            //        //database.AddInParameter(command, "@WalletNumber", DbType.String, objCustomerUserInfo.WalletNumber);

            //        //database.AddInParameter(command, "@BINBankCode", DbType.String, objCustomerUserInfo.BankNo);
            //        //database.AddInParameter(command, "@BankBranchCode", DbType.String, objCustomerUserInfo.BranchCode);
            //        //database.AddInParameter(command, "@BankAccountNumber", DbType.String, objCustomerUserInfo.BankAccountNumber);
            //        //database.AddInParameter(command, "@PIN", DbType.String, objCustomerUserInfo.PIN);

            //        //database.AddInParameter(command, "@IsApproved", DbType.String, objCustomerUserInfo.IsApproved);
            //        //database.AddInParameter(command, "@IsRejected", DbType.String, objCustomerUserInfo.IsRejected);

            //        database.AddInParameter(command, "@mode", DbType.String, objCustomerUserInfo.Mode);
            //        //database.AddOutParameter(command, "@RegIDOut", DbType.Int32, objCustomerUserInfo.RegIDOut);
            //        ret = database.ExecuteNonQuery(command);
            //        if (objCustomerUserInfo.Mode.Equals("UAINFO", StringComparison.InvariantCultureIgnoreCase))
            //        {
            //            ret = (int)database.GetParameterValue(command, "RegIDOut");
            //        }
            //    }

            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}
            //finally
            //{
            //    database = null;
            //    Database.ClearParameterCache();
            //}

            //    return ret;
        }


        public int UpdateAgentInfo(UserInfo objCustomerUserInfo)
        {

            SqlConnection sqlCon = null;
            int ret;
            try
            {
                using (sqlCon = new SqlConnection(Connection.DatabaseConnection.ConnectionStr()))
                {
                    sqlCon.Open();
                    using (SqlCommand sqlCmd = new SqlCommand("[s_MNAgentUpdate]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.Add(new SqlParameter("@ClientCode", objCustomerUserInfo.ClientCode));
                        sqlCmd.Parameters.Add(new SqlParameter("@Name", objCustomerUserInfo.Name));

                        sqlCmd.Parameters.Add(new SqlParameter("@Address", objCustomerUserInfo.Address));

                        sqlCmd.Parameters.Add(new SqlParameter("@EmailAddress", objCustomerUserInfo.EmailAddress));
                        sqlCmd.Parameters.Add(new SqlParameter("@ContactNumber1", objCustomerUserInfo.ContactNumber1));
                        sqlCmd.Parameters.Add(new SqlParameter("@ContactNumber2", objCustomerUserInfo.ContactNumber2));
                        sqlCmd.Parameters.Add(new SqlParameter("@BankAccountNumber", objCustomerUserInfo.BankAccountNumber));
                        sqlCmd.Parameters.Add(new SqlParameter("@BankBinNo", objCustomerUserInfo.BankNo));
                        sqlCmd.Parameters.Add(new SqlParameter("@BranchCode", objCustomerUserInfo.BranchCode));

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





            //Database database;
            //int ret;

            //try
            //{
            //    database = DatabaseConnection.GetDatabase();
            //    using (var command = database.GetStoredProcCommand("[s_MNAdminInfoUpdate]"))
            //    {
            //        database.AddInParameter(command, "@ClientCode", DbType.String, objCustomerUserInfo.ClientCode);
            //        //database.AddInParameter(command, "@Address", DbType.String, objCustomerUserInfo.Address);
            //        //database.AddInParameter(command, "@Status", DbType.String, objCustomerUserInfo.Status);
            //        database.AddInParameter(command, "@ProfileName", DbType.String, objCustomerUserInfo.ProfileName);
            //        database.AddInParameter(command, "@Name", DbType.String, objCustomerUserInfo.Name);
            //        database.AddInParameter(command, "@EmailAddress", DbType.String, objCustomerUserInfo.EmailAddress);
            //        database.AddInParameter(command, "@BranchCode", DbType.String, objCustomerUserInfo.BranchCode);
            //        database.AddInParameter(command, "@COC", DbType.String, objCustomerUserInfo.COC);
            //        database.AddInParameter(command, "@ReturnValue", DbType.Int32, objCustomerUserInfo.ReturnValue);


            //        //database.AddInParameter(command, "@ContactNumber1", DbType.String, objCustomerUserInfo.ContactNumber1);
            //        //database.AddInParameter(command, "@ContactNumber2", DbType.String, objCustomerUserInfo.ContactNumber2);
            //        //database.AddInParameter(command, "@WalletNumber", DbType.String, objCustomerUserInfo.WalletNumber);

            //        //database.AddInParameter(command, "@BINBankCode", DbType.String, objCustomerUserInfo.BankNo);
            //        //database.AddInParameter(command, "@BankBranchCode", DbType.String, objCustomerUserInfo.BranchCode);
            //        //database.AddInParameter(command, "@BankAccountNumber", DbType.String, objCustomerUserInfo.BankAccountNumber);
            //        //database.AddInParameter(command, "@PIN", DbType.String, objCustomerUserInfo.PIN);

            //        //database.AddInParameter(command, "@IsApproved", DbType.String, objCustomerUserInfo.IsApproved);
            //        //database.AddInParameter(command, "@IsRejected", DbType.String, objCustomerUserInfo.IsRejected);

            //        database.AddInParameter(command, "@mode", DbType.String, objCustomerUserInfo.Mode);
            //        //database.AddOutParameter(command, "@RegIDOut", DbType.Int32, objCustomerUserInfo.RegIDOut);
            //        ret = database.ExecuteNonQuery(command);
            //        if (objCustomerUserInfo.Mode.Equals("UAINFO", StringComparison.InvariantCultureIgnoreCase))
            //        {
            //            ret = (int)database.GetParameterValue(command, "RegIDOut");
            //        }
            //    }

            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}
            //finally
            //{
            //    database = null;
            //    Database.ClearParameterCache();
            //}

            //    return ret;
        }
        public int UpdateAdminInfo(UserInfo objCustomerUserInfo)
        {

            SqlConnection sqlCon = null;
            int ret;
            try
            {
                using (sqlCon = new SqlConnection(Connection.DatabaseConnection.ConnectionStr()))
                {
                    sqlCon.Open();
                    using (SqlCommand sqlCmd = new SqlCommand("[s_MNAdminInfoUpdate]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.Add(new SqlParameter("@ClientCode", objCustomerUserInfo.ClientCode));
                        sqlCmd.Parameters.Add(new SqlParameter("@Name", objCustomerUserInfo.Name));
                        sqlCmd.Parameters.Add(new SqlParameter("@ProfileCode", objCustomerUserInfo.ProfileCode));
                        sqlCmd.Parameters.Add(new SqlParameter("@EmailAddress", objCustomerUserInfo.EmailAddress));
                        sqlCmd.Parameters.Add(new SqlParameter("@BranchCode", objCustomerUserInfo.BranchCode));
                        sqlCmd.Parameters.Add(new SqlParameter("@COC", objCustomerUserInfo.COC));
                        sqlCmd.Parameters.Add(new SqlParameter("@Status", objCustomerUserInfo.Status));
                        sqlCmd.Parameters.Add(new SqlParameter("@mode", objCustomerUserInfo.Mode));
                        sqlCmd.Parameters.Add(new SqlParameter("@ModifyingBranch", objCustomerUserInfo.AdminBranch));
                        sqlCmd.Parameters.Add(new SqlParameter("@ModifiedBy", objCustomerUserInfo.AdminUserName));
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





            //Database database;
            //int ret;

            //try
            //{
            //    database = DatabaseConnection.GetDatabase();
            //    using (var command = database.GetStoredProcCommand("[s_MNAdminInfoUpdate]"))
            //    {
            //        database.AddInParameter(command, "@ClientCode", DbType.String, objCustomerUserInfo.ClientCode);
            //        //database.AddInParameter(command, "@Address", DbType.String, objCustomerUserInfo.Address);
            //        //database.AddInParameter(command, "@Status", DbType.String, objCustomerUserInfo.Status);
            //        database.AddInParameter(command, "@ProfileName", DbType.String, objCustomerUserInfo.ProfileName);
            //        database.AddInParameter(command, "@Name", DbType.String, objCustomerUserInfo.Name);
            //        database.AddInParameter(command, "@EmailAddress", DbType.String, objCustomerUserInfo.EmailAddress);
            //        database.AddInParameter(command, "@BranchCode", DbType.String, objCustomerUserInfo.BranchCode);
            //        database.AddInParameter(command, "@COC", DbType.String, objCustomerUserInfo.COC);
            //        database.AddInParameter(command, "@ReturnValue", DbType.Int32, objCustomerUserInfo.ReturnValue);


            //        //database.AddInParameter(command, "@ContactNumber1", DbType.String, objCustomerUserInfo.ContactNumber1);
            //        //database.AddInParameter(command, "@ContactNumber2", DbType.String, objCustomerUserInfo.ContactNumber2);
            //        //database.AddInParameter(command, "@WalletNumber", DbType.String, objCustomerUserInfo.WalletNumber);

            //        //database.AddInParameter(command, "@BINBankCode", DbType.String, objCustomerUserInfo.BankNo);
            //        //database.AddInParameter(command, "@BankBranchCode", DbType.String, objCustomerUserInfo.BranchCode);
            //        //database.AddInParameter(command, "@BankAccountNumber", DbType.String, objCustomerUserInfo.BankAccountNumber);
            //        //database.AddInParameter(command, "@PIN", DbType.String, objCustomerUserInfo.PIN);

            //        //database.AddInParameter(command, "@IsApproved", DbType.String, objCustomerUserInfo.IsApproved);
            //        //database.AddInParameter(command, "@IsRejected", DbType.String, objCustomerUserInfo.IsRejected);

            //        database.AddInParameter(command, "@mode", DbType.String, objCustomerUserInfo.Mode);
            //        //database.AddOutParameter(command, "@RegIDOut", DbType.Int32, objCustomerUserInfo.RegIDOut);
            //        ret = database.ExecuteNonQuery(command);
            //        if (objCustomerUserInfo.Mode.Equals("UAINFO", StringComparison.InvariantCultureIgnoreCase))
            //        {
            //            ret = (int)database.GetParameterValue(command, "RegIDOut");
            //        }
            //    }

            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}
            //finally
            //{
            //    database = null;
            //    Database.ClearParameterCache();
            //}

            //    return ret;
        }
        public int UpdateMerchantInfo(UserInfo objCustomerUserInfo)
        {

            SqlConnection sqlCon = null;
            int ret;
            try
            {
                using (sqlCon = new SqlConnection(Connection.DatabaseConnection.ConnectionStr()))
                {
                    sqlCon.Open();
                    using (SqlCommand sqlCmd = new SqlCommand("[s_MNMerchantInfoUpdate]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.Add(new SqlParameter("@ClientCode", objCustomerUserInfo.ClientCode));
                        sqlCmd.Parameters.Add(new SqlParameter("@Name", objCustomerUserInfo.Name));
                        sqlCmd.Parameters.Add(new SqlParameter("@Status", objCustomerUserInfo.Status));
                        sqlCmd.Parameters.Add(new SqlParameter("@IsApproved", objCustomerUserInfo.IsApproved));
                        sqlCmd.Parameters.Add(new SqlParameter("@Address", objCustomerUserInfo.Address));
                        sqlCmd.Parameters.Add(new SqlParameter("@EmailAddress", objCustomerUserInfo.EmailAddress));
                        sqlCmd.Parameters.Add(new SqlParameter("@ContactNumber1", objCustomerUserInfo.ContactNumber1));
                        sqlCmd.Parameters.Add(new SqlParameter("@ContactNumber2", objCustomerUserInfo.ContactNumber2));
                        sqlCmd.Parameters.Add(new SqlParameter("@BankAccountNumber", objCustomerUserInfo.BankAccountNumber));
                        sqlCmd.Parameters.Add(new SqlParameter("@BankCode", objCustomerUserInfo.BankNo));
                        sqlCmd.Parameters.Add(new SqlParameter("@AcBranchCode", objCustomerUserInfo.BranchCode));

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





            //Database database;
            //int ret;

            //try
            //{
            //    database = DatabaseConnection.GetDatabase();
            //    using (var command = database.GetStoredProcCommand("[s_MNAdminInfoUpdate]"))
            //    {
            //        database.AddInParameter(command, "@ClientCode", DbType.String, objCustomerUserInfo.ClientCode);
            //        //database.AddInParameter(command, "@Address", DbType.String, objCustomerUserInfo.Address);
            //        //database.AddInParameter(command, "@Status", DbType.String, objCustomerUserInfo.Status);
            //        database.AddInParameter(command, "@ProfileName", DbType.String, objCustomerUserInfo.ProfileName);
            //        database.AddInParameter(command, "@Name", DbType.String, objCustomerUserInfo.Name);
            //        database.AddInParameter(command, "@EmailAddress", DbType.String, objCustomerUserInfo.EmailAddress);
            //        database.AddInParameter(command, "@BranchCode", DbType.String, objCustomerUserInfo.BranchCode);
            //        database.AddInParameter(command, "@COC", DbType.String, objCustomerUserInfo.COC);
            //        database.AddInParameter(command, "@ReturnValue", DbType.Int32, objCustomerUserInfo.ReturnValue);


            //        //database.AddInParameter(command, "@ContactNumber1", DbType.String, objCustomerUserInfo.ContactNumber1);
            //        //database.AddInParameter(command, "@ContactNumber2", DbType.String, objCustomerUserInfo.ContactNumber2);
            //        //database.AddInParameter(command, "@WalletNumber", DbType.String, objCustomerUserInfo.WalletNumber);

            //        //database.AddInParameter(command, "@BINBankCode", DbType.String, objCustomerUserInfo.BankNo);
            //        //database.AddInParameter(command, "@BankBranchCode", DbType.String, objCustomerUserInfo.BranchCode);
            //        //database.AddInParameter(command, "@BankAccountNumber", DbType.String, objCustomerUserInfo.BankAccountNumber);
            //        //database.AddInParameter(command, "@PIN", DbType.String, objCustomerUserInfo.PIN);

            //        //database.AddInParameter(command, "@IsApproved", DbType.String, objCustomerUserInfo.IsApproved);
            //        //database.AddInParameter(command, "@IsRejected", DbType.String, objCustomerUserInfo.IsRejected);

            //        database.AddInParameter(command, "@mode", DbType.String, objCustomerUserInfo.Mode);
            //        //database.AddOutParameter(command, "@RegIDOut", DbType.Int32, objCustomerUserInfo.RegIDOut);
            //        ret = database.ExecuteNonQuery(command);
            //        if (objCustomerUserInfo.Mode.Equals("UAINFO", StringComparison.InvariantCultureIgnoreCase))
            //        {
            //            ret = (int)database.GetParameterValue(command, "RegIDOut");
            //        }
            //    }

            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}
            //finally
            //{
            //    database = null;
            //    Database.ClearParameterCache();
            //}

            //    return ret;
        }
        public DataTable GetAgentIdModel(UserInfo objUserInfo)
        {
            Database database;
            DataTable dtableResult = null;

            try
            {
                database = DatabaseConnection.GetDatabase();

                using (var command = database.GetStoredProcCommand("[s_MNUserInfoSearch]"))
                {
                    database.AddInParameter(command, "@MobileNo", DbType.String, objUserInfo.ContactNumber1);
                    database.AddInParameter(command, "@Name", DbType.String, objUserInfo.Name);
                    database.AddInParameter(command, "@WalletNumber", DbType.String, objUserInfo.WalletNumber);
                    database.AddInParameter(command, "@ClientCode", DbType.String, objUserInfo.ClientCode);
                    database.AddInParameter(command, "@mode", DbType.String, objUserInfo.Mode);
                    using (var dataset = new DataSet())
                    {
                        database.LoadDataSet(command, dataset, "dtAgentInfo");

                        if (dataset.Tables.Count > 0)
                        {
                            dtableResult = dataset.Tables["dtAgentInfo"];
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
        public ChargeVM GetCustCharge(string ClientCode)
        {
            DataSet dataset = new DataSet();
            SqlConnection conn = null;
            MNProfileChargeClass Charge = new MNProfileChargeClass();
            UserInfo User = new UserInfo();
            List<MNCustChargeLog> ChargeLogs = new List<MNCustChargeLog>();
            MNProfileClass profile = new MNProfileClass();
            ChargeVM CustCharge = new ChargeVM();


            try
            {
                using (conn = new SqlConnection(DatabaseConnection.ConnectionStr()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("dbo.s_MNCustCharge", conn))
                    {

                        cmd.Parameters.AddWithValue("@ClientCode", ClientCode);
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            // set the CommandTimeout
                            da.SelectCommand.CommandTimeout = 0;  // seconds
                            da.Fill(dataset);
                        }
                    }
                }
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                if (dataset.Tables.Count > 0)
                {
                    DataTable ProfileCharge = dataset.Tables[0];
                    DataTable ChargeLog = dataset.Tables[1];
                    if (ProfileCharge.Rows.Count > 0)
                    {
                        var row = ProfileCharge.Rows[0];

                        Charge.ProfileCode = row["ProfileCode"].ToString();
                        Charge.Registration = row["Registration"].ToString();
                        Charge.PinReset = row["PinReset"].ToString();
                        Charge.ReNew = row["ReNew"].ToString();
                        Charge.ChargeAccount = row["ChargeAccount"].ToString();
                        User.Name = row["Name"].ToString();
                        User.UserName = row["UserName"].ToString();
                        User.Address = row["Address"].ToString();
                        User.TBranchCode = row["UserBranchCode"].ToString();
                        User.BankAccountNumber = row["BankAccountNumber"].ToString();
                        User.BankNo = row["BankNo"].ToString();
                        User.BranchCode = row["BranchCode"].ToString();
                        User.Status = row["Status"].ToString();
                        User.PIN = row["PIN"].ToString();
                        User.ClientCode = row["ClientCode"].ToString();

                        profile.RenewPeriod = Convert.ToInt32(row["RenewPeriod"] == null ? "0" : row["RenewPeriod"].ToString());
                        profile.AutoRenew = row["AutoRenew"].ToString();
                        CustCharge.IsCharged = row["IsCharged"].ToString() == "T" ? true : false;


                        CustCharge.ExpiredOn = row["ExpiredOn"].ToString();
                    }
                    if (ChargeLog.Rows.Count > 0)
                    {
                        foreach (DataRow row in ChargeLog.Rows)
                        {
                            MNCustChargeLog Log = new MNCustChargeLog();
                            Log.ClientCode = row["ClientCode"].ToString();
                            Log.UserName = row["UserName"].ToString();
                            Log.ChargeAmount = row["ChargeAmount"].ToString();
                            Log.ChargeDescription = row["ChargeDescription"].ToString();
                            Log.DeductedDate = row["DeductedDate"].ToString();
                            Log.ProcessedBy = row["ProcessedBy"].ToString();
                            Log.RRNumber = row["RRNumber"].ToString();
                            ChargeLogs.Add(Log);
                        }
                    }
                }
                CustCharge.ProfileCharge = Charge;
                CustCharge.UserInfo = User;
                CustCharge.ChargeLogs = ChargeLogs;
                CustCharge.CustProfile = profile;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                dataset.Dispose();
            }

            return CustCharge;
        }
        public bool InsertSMSLog(SMSLog log)
        {
            SqlConnection conn = null;
            SqlTransaction strans = null;
            bool result = false;
            try
            {
                using (conn = new SqlConnection(Connection.DatabaseConnection.ConnectionStr()))
                {


                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = @"INSERT INTO MNSMSPinLog(UserName,Message,SentOn,SentBy,Purpose)
                                           Values (@UserName,@Message,@SentOn,@SentBy,@Purpose)";
                    cmd.Parameters.AddWithValue("@UserName", log.UserName);
                    cmd.Parameters.AddWithValue("@Message", log.Message);
                    cmd.Parameters.AddWithValue("@SentOn", log.SentOn);
                    cmd.Parameters.AddWithValue("@SentBy", log.SentBy);
                    cmd.Parameters.AddWithValue("@Purpose", log.Purpose);

                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    cmd.Connection = conn;
                    strans = conn.BeginTransaction();
                    cmd.Transaction = strans;
                    int i = cmd.ExecuteNonQuery();
                    if (i == 1)
                    {
                        strans.Commit();
                        result = true;

                    }
                    else
                    {
                        result = false;
                    }

                    cmd.Dispose();


                }
            }
            catch (Exception ex)
            {
                strans.Rollback();
                result = false;
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();

            }
            return result;
        }



        public List<UserInfo> GetRejectedUser(string UserName)
        {
            List<UserInfo> UserInfos = new List<UserInfo>();
            SqlConnection conn = null;
            string Command = string.Empty;
            SqlDataReader rdr;
            try
            {
                using (conn = new SqlConnection(DatabaseConnection.ConnectionStr()))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        Command = @"Select * from v_MNClientDetail where Status ='Expired' ";

                        if (!string.IsNullOrEmpty(UserName))
                        {
                            Command = Command + " AND UserName=@UserName";
                            cmd.Parameters.AddWithValue("UserName", UserName);
                        }
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
                            Info.CreatedDate = rdr["CreatedDate"].ToString();
                            Info.ProfileName = rdr["ProfileCode"].ToString();
                            Info.BankAccountNumber = rdr["BankAccountNumber"].ToString();
                            Info.Status = rdr["Status"].ToString();

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



        /// <summary>
        /// Inserts into MNMakerChecker,InMemMNTransactionAccount for checking 
        /// </summary>
        /// <param name="ClientCode">Customer Client Code</param>
        /// <param name="ModifyingAdmin">Modifying Admin</param>
        /// <param name="ModifyingBranch"> Modifying Admin Branch Code</param>
        /// <param name="ModifiedField">XML String of ModifiedFields</param>
        /// <param name="AccToDelete">XML String of transaction accounts to delete</param>
        /// <param name="AccToAdd">XML String of transaction accounts to add</param>
        /// <returns>-1 if insert fails,99 if no error occurs but data is not inserted,100 Successful</returns>
        public int InsertIntoMakerChecker(string ClientCode, string ModifyingAdmin, string ModifyingBranch, string ModifiedField, string AccToDelete, string AccToAdd)
        {

            SqlConnection sqlCon = null;
            int ret;
            try
            {
                using (sqlCon = new SqlConnection(Connection.DatabaseConnection.ConnectionStr()))
                {
                    sqlCon.Open();
                    using (SqlCommand sqlCmd = new SqlCommand("[s_MNClientMakerChecker]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.Add(new SqlParameter("@ClientCode", ClientCode));
                        sqlCmd.Parameters.Add(new SqlParameter("@ModifyingUser", ModifyingAdmin));
                        sqlCmd.Parameters.Add(new SqlParameter("@ModifyingBranch", ModifyingBranch));
                        sqlCmd.Parameters.Add(new SqlParameter("@ModifiedFields", ModifiedField));
                        sqlCmd.Parameters.Add(new SqlParameter("@TxnAccountsToDelete", AccToDelete));
                        sqlCmd.Parameters.Add(new SqlParameter("@TxnAccountsToAdd", AccToAdd));
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



        public int InsertIntoMakerCheckerAdmin(string ClientCode, string ModifyingAdmin, string ModifyingBranch, string ModifiedField)
        {

            SqlConnection sqlCon = null;
            int ret;
            try
            {
                using (sqlCon = new SqlConnection(Connection.DatabaseConnection.ConnectionStr()))
                {
                    sqlCon.Open();
                    using (SqlCommand sqlCmd = new SqlCommand("[s_MNAdminMakerChecker]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.Add(new SqlParameter("@ClientCode", ClientCode));
                        sqlCmd.Parameters.Add(new SqlParameter("@ModifyingUser", ModifyingAdmin));
                        sqlCmd.Parameters.Add(new SqlParameter("@ModifyingBranch", ModifyingBranch));
                        sqlCmd.Parameters.Add(new SqlParameter("@ModifiedFields", ModifiedField));
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











        #region ADMIN APPROVE REJECTED LIST

        public int AdminApprove(UserInfo objUserInfo, string Rejected, string Approve)
        {
            SqlConnection conn = null;
            int ret;
            try
            {
                using (conn = new SqlConnection(DatabaseConnection.ConnectionStr()))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = @"UPDATE MNClient SET IsRejected=@IsRejected,IsApproved=@IsApproved,ModifiedBy=@ModifiedBy,ModifyingBranch=@ModifyingBranch
                                               Where ClientCode=@ClientCode ";


                        cmd.Parameters.AddWithValue("@ClientCode", objUserInfo.ClientCode);
                        cmd.Parameters.AddWithValue("@IsRejected", Rejected);
                        cmd.Parameters.AddWithValue("@IsApproved", Approve);
                        cmd.Parameters.AddWithValue("@ModifiedBy", objUserInfo.AdminUserName);
                        cmd.Parameters.AddWithValue("@ModifyingBranch", objUserInfo.AdminBranch);
                        cmd.Connection = conn;
                        if (conn.State != ConnectionState.Open)
                        {
                            conn.Open();
                        }
                        ret = cmd.ExecuteNonQuery();
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

            return ret;
        }






        public int AdminReject(UserInfo objUserInfo, string Rejected, string Approve)
        {
            SqlConnection conn = null;
            int ret;
            try
            {
                using (conn = new SqlConnection(DatabaseConnection.ConnectionStr()))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = @"UPDATE MNClient SET IsRejected=@IsRejected,IsApproved=@IsApproved,Remarks=@Remarks,RejectedBy=@RejectedBy
                                               Where ClientCode=@ClientCode ";


                        cmd.Parameters.AddWithValue("@ClientCode", objUserInfo.ClientCode);
                        cmd.Parameters.AddWithValue("@IsRejected", Rejected);
                        cmd.Parameters.AddWithValue("@IsApproved", Approve);
                        cmd.Parameters.AddWithValue("@Remarks", objUserInfo.Remarks);
                        cmd.Parameters.AddWithValue("@RejectedBy", objUserInfo.AdminUserName);
                        cmd.Connection = conn;
                        if (conn.State != ConnectionState.Open)
                        {
                            conn.Open();
                        }
                        ret = cmd.ExecuteNonQuery();
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

            return ret;
        }

        #endregion
        public List<UserInfo> GetUserStatusList(string BranchCode, bool COC, string MobileNo)
        {
            List<UserInfo> UserInfos = new List<UserInfo>();
            SqlConnection conn = null;
            string Command = string.Empty;
            SqlDataReader rdr;
            try
            {
                using (conn = new SqlConnection(DatabaseConnection.ConnectionStr()))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        //Command = @"Select * from v_MNClientDetail where BlockStatus in (@Status1,@Status2) AND IsApproved=@IsApproved AND UserType = 'user'";
                        Command = @"Select * from v_MNClientDetail where BlockStatus in (@Status1,@Status2) AND UserType = 'user'";
                        
                        if (!COC)
                        {
                            Command = Command + " AND ModifyingBranch='" + BranchCode + "'";
                        }
                        if (!string.IsNullOrEmpty(MobileNo))
                        {
                            Command = Command + " AND UserName='" + MobileNo + "'";
                        }
                        cmd.CommandText = Command;
                        cmd.Parameters.AddWithValue("@Status1", "Blocked"); //PIN Reset
                        cmd.Parameters.AddWithValue("@Status2", "Active"); //Pass Reset
                        cmd.Parameters.AddWithValue("@IsApproved", "Blocked");
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
                            Info.ModifyingBranch = rdr["ModifyingBranch"].ToString();
                            Info.ModifyingAdmin = rdr["ModifiedBy"].ToString();
                            Info.BankAccountNumber = rdr["BankAccountNumber"].ToString();
                            //Info.Status = rdr["BlockStatus"].ToString();
                            Info.Status = rdr["Status"].ToString();
                            Info.BlockStatus = rdr["BlockStatus"].ToString();
                            Info.BlockRemarks = rdr["BlockRemarks"].ToString();

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

        #region Customer ApproveBlockUnblock/Rejected
        public int StatusApprove(string ClientCode)
        {
            SqlConnection conn = null;
            int ret = 0;
            try
            {
                using (conn = new SqlConnection(DatabaseConnection.ConnectionStr()))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = @"UPDATE MNClient SET IsApproved=@IsApproved,ModifiedBy='',ModifyingBranch=''
                                               Where ClientCode=@ClientCode ";
                        cmd.Parameters.AddWithValue("@ClientCode", ClientCode);
                        cmd.Parameters.AddWithValue("@IsApproved", "Approve");
                        cmd.Connection = conn;
                        if (conn.State != ConnectionState.Open)
                        {
                            conn.Open();
                        }
                        ret = cmd.ExecuteNonQuery();
                        if (conn.State != ConnectionState.Closed)
                        {
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                ret = 0;
            }
            finally
            {

            }
            return ret;
        }

        public int StatusReject(string ClientCode, string Status)
        {
            SqlConnection conn = null;
            int ret = 0;
            try
            {
                using (conn = new SqlConnection(DatabaseConnection.ConnectionStr()))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = @"UPDATE MNClient SET IsApproved=@IsApproved, ModifiedBy='', ModifyingBranch='',BlockStatus='',Status=@Status WHERE ClientCode=@ClientCode ";
                        cmd.Parameters.AddWithValue("@ClientCode", ClientCode);
                        cmd.Parameters.AddWithValue("@Status", Status);
                        cmd.Parameters.AddWithValue("@IsApproved", "Approve");
                        cmd.Connection = conn;
                        if (conn.State != ConnectionState.Open)
                        {
                            conn.Open();
                        }
                        ret = cmd.ExecuteNonQuery();
                        if (conn.State != ConnectionState.Closed)
                        {
                            conn.Close();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                ret = 0;
            }
            finally
            {

            }
            return ret;
        }
        #endregion

        #region Admin Registration Approve/Rejected
        // AdminRegApprove
        public int AdminRegApprove(UserInfo objUserInfo, string Approve)
        {
            SqlConnection conn = null;
            int ret;
            try
            {
                using (conn = new SqlConnection(DatabaseConnection.ConnectionStr()))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = @"UPDATE MNClient SET Status = 'Active', IsApproved = 'Approve' where ClientCode = @ClientCode";


                        cmd.Parameters.AddWithValue("@ClientCode", objUserInfo.ClientCode);
                        //cmd.Parameters.AddWithValue("@IsRejected", Rejected);
                        //cmd.Parameters.AddWithValue("@IsApproved", Approve);
                        //cmd.Parameters.AddWithValue("@ModifiedBy", objUserInfo.AdminUserName);
                        //cmd.Parameters.AddWithValue("@ModifyingBranch", objUserInfo.AdminBranch);
                        cmd.Connection = conn;
                        if (conn.State != ConnectionState.Open)
                        {
                            conn.Open();
                        }
                        ret = cmd.ExecuteNonQuery();
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

            return ret;
        }


        public int AdminRegReject(UserInfo objUserInfo, string Rejected)
        {
            SqlConnection conn = null;
            int ret;
            try
            {
                using (conn = new SqlConnection(DatabaseConnection.ConnectionStr()))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = @"UPDATE MNClient SET IsRejected=@IsRejected,Remarks=@Remarks,RejectedBy=@RejectedBy
                                               Where ClientCode=@ClientCode ";


                        cmd.Parameters.AddWithValue("@ClientCode", objUserInfo.ClientCode);
                        cmd.Parameters.AddWithValue("@IsRejected", Rejected);
                        //cmd.Parameters.AddWithValue("@IsApproved", Approve);
                        cmd.Parameters.AddWithValue("@Remarks", objUserInfo.Remarks);
                        cmd.Parameters.AddWithValue("@RejectedBy", objUserInfo.AdminUserName);
                        cmd.Connection = conn;
                        if (conn.State != ConnectionState.Open)
                        {
                            conn.Open();
                        }
                        ret = cmd.ExecuteNonQuery();
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

            return ret;
        }
        #endregion


        #region Approve and edit info of Admin from Rejected List
        public int AprvRjAdmin(UserInfo objCustomerUserInfo)
        {

            SqlConnection sqlCon = null;
            int ret;
            try
            {
                using (sqlCon = new SqlConnection(Connection.DatabaseConnection.ConnectionStr()))
                {
                    sqlCon.Open();
                    using (SqlCommand sqlCmd = new SqlCommand("[s_MNAdminInfoUpdate]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.Add(new SqlParameter("@ClientCode", objCustomerUserInfo.ClientCode));
                        sqlCmd.Parameters.Add(new SqlParameter("@Name", objCustomerUserInfo.Name));
                        sqlCmd.Parameters.Add(new SqlParameter("@ProfileCode", objCustomerUserInfo.ProfileCode));
                        sqlCmd.Parameters.Add(new SqlParameter("@EmailAddress", objCustomerUserInfo.EmailAddress));
                        sqlCmd.Parameters.Add(new SqlParameter("@BranchCode", objCustomerUserInfo.BranchCode));
                        sqlCmd.Parameters.Add(new SqlParameter("@COC", objCustomerUserInfo.COC));
                        sqlCmd.Parameters.Add(new SqlParameter("@Status", objCustomerUserInfo.Status));
                        sqlCmd.Parameters.Add(new SqlParameter("@mode", objCustomerUserInfo.Mode));
                        sqlCmd.Parameters.Add(new SqlParameter("@ModifyingBranch", objCustomerUserInfo.AdminBranch));
                        sqlCmd.Parameters.Add(new SqlParameter("@ModifiedBy", objCustomerUserInfo.AdminUserName));
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

        public int ApproveModifiedAdmin(UserInfo objCustomerUserInfo)
        {

            SqlConnection sqlCon = null;
            int ret;
            try
            {
                using (sqlCon = new SqlConnection(Connection.DatabaseConnection.ConnectionStr()))
                {
                    sqlCon.Open();
                    using (SqlCommand sqlCmd = new SqlCommand("[s_MNApproveAdmin]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.Add(new SqlParameter("@ClientCode", objCustomerUserInfo.ClientCode));
                        sqlCmd.Parameters.Add(new SqlParameter("@ApprovingBranch", objCustomerUserInfo.AdminBranch));
                        sqlCmd.Parameters.Add(new SqlParameter("@ApprovingUser", objCustomerUserInfo.AdminUserName));
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




        public int RejectModifiedAdmin(UserInfo objCustomerUserInfo)
        {

            SqlConnection sqlCon = null;
            int ret;
            try
            {
                using (sqlCon = new SqlConnection(Connection.DatabaseConnection.ConnectionStr()))
                {
                    sqlCon.Open();
                    using (SqlCommand sqlCmd = new SqlCommand("[s_MNRejectAdmin]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.Add(new SqlParameter("@ClientCode", objCustomerUserInfo.ClientCode));
                        sqlCmd.Parameters.Add(new SqlParameter("@RejectingBranch", objCustomerUserInfo.AdminBranch));
                        sqlCmd.Parameters.Add(new SqlParameter("@RejectingUser", objCustomerUserInfo.AdminUserName));
                        sqlCmd.Parameters.Add(new SqlParameter("@Remarks", objCustomerUserInfo.Remarks));
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



        //public Customer GetCustById(string Id)
        //{
        //    Database database;
        //    DataSet dtset = new DataSet();
        //    MNClient MNClient = new MNClient();
        //    MNClientContact MNClientContact = new MNClientContact();
        //    MNClientExt MNClientExt = new MNClientExt();
        //    MNBankAccountMap MNBankAccountMap = new MNBankAccountMap();
        //    List<TransactionInfo> MNTransactionAccounts = new List<TransactionInfo>();
        //    Customer Cust = new Customer
        //    {
        //        MNClient = MNClient,
        //        MNClientContact = MNClientContact,
        //        MNClientExt = MNClientExt,
        //        MNBankAccountMap = MNBankAccountMap,
        //        MNTransactionAccounts = MNTransactionAccounts
        //    };
        //    try
        //    {
        //        database = DatabaseConnection.GetDatabase();
        //        using (var command = database.GetStoredProcCommand("[s_MNGetClientByCode]"))
        //        {
        //            database.AddInParameter(command, "@ClientCode", DbType.String, Id);
        //            string[] table = new string[] { "MNClient", "MNClientExt", "MNClientContact", "MNBankAccountMap", "MNTransactionAccount" };
        //            using (var dataset = new DataSet())
        //            {
        //                database.LoadDataSet(command, dataset, table);
        //                dtset = dataset;
        //            }

        //        }
        //        Cust.MNClient = ExtraUtility.DatatableToSingleClass<MNClient>(dtset.Tables["MNClient"]);
        //        Cust.MNClientExt = ExtraUtility.DatatableToSingleClass<MNClientExt>(dtset.Tables["MNClientExt"]);
        //        Cust.MNClientContact = ExtraUtility.DatatableToSingleClass<MNClientContact>(dtset.Tables["MNClientContact"]);
        //        Cust.MNBankAccountMap = ExtraUtility.DatatableToSingleClass<MNBankAccountMap>(dtset.Tables["MNBankAccountMap"]);
        //        Cust.MNTransactionAccounts = ExtraUtility.DatatableToListClass<TransactionInfo>(dtset.Tables["MNTransactionAccount"]);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //    finally
        //    {
        //        database = null;
        //        //  Database.ClearParameterCache();
        //    }

        //    return Cust;
        //}

        //test//
        //Customer Rejected List search//
        //Displaying all the list//
        public DataTable GetCusModRejInformation()
        {
            Database database;
            DataTable dtableResult = null;

            try
            {
                database = DatabaseConnection.GetDatabase();
                using (var command = database.GetStoredProcCommand("[s_MNCustModifyRejUnappvInformation]"))
                {

                    using (var dataset = new DataSet())
                    {
                        database.LoadDataSet(command, dataset, "dtCustomerInfo");

                        if (dataset.Tables.Count > 0)
                        {
                            dtableResult = dataset.Tables["dtCustomerInfo"];
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

        //By Mobile No//
        public DataTable GetCusRejUnInformation(UserInfo objUserInfo)
        {
            Database database;
            DataTable dtableResult = null;

            try
            {
                database = DatabaseConnection.GetDatabase();
                using (var command = database.GetStoredProcCommand("[s_MNUserInfoSearch]"))
                {
                    database.AddInParameter(command, "@MobileNo", DbType.String, objUserInfo.ContactNumber1);
                    database.AddInParameter(command, "@Name", DbType.String, objUserInfo.Name);
                    database.AddInParameter(command, "@WalletNumber", DbType.String, objUserInfo.WalletNumber);
                    database.AddInParameter(command, "@ClientCode", DbType.String, objUserInfo.ClientCode);
                    database.AddInParameter(command, "@userType", DbType.String, "admin");
                    database.AddInParameter(command, "@mode", DbType.String, objUserInfo.Mode);
                    using (var dataset = new DataSet())
                    {
                        database.LoadDataSet(command, dataset, "dtCustomerInfo");

                        if (dataset.Tables.Count > 0)
                        {
                            dtableResult = dataset.Tables["dtCustomerInfo"];
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

        public DataTable GetAllCusRejUnInformation()
        {
            Database database;
            DataTable dtableResult = null;

            try
            {
                database = DatabaseConnection.GetDatabase();
                using (var command = database.GetStoredProcCommand("[s_MNCustRejectUnapprovedInformation]"))
                {


                    using (var dataset = new DataSet())
                    {
                        database.LoadDataSet(command, dataset, "dtCustomerInfo");

                        if (dataset.Tables.Count > 0)
                        {
                            dtableResult = dataset.Tables["dtCustomerInfo"];
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



        #region
        internal DataTable ResetBankLinkKYC(UserInfo objUserInfo)
        {
            Database database;
            DataTable dtableResult = null;

            try
            {
                database = DatabaseConnection.GetDatabase();
                using (var command = database.GetStoredProcCommand("[s_MNBankLinkKYCReset]"))
                {
                    database.AddInParameter(command, "@UserType", DbType.String, objUserInfo.UserType);
                    database.AddInParameter(command, "@UserName", DbType.String, objUserInfo.UserName);
                    database.AddInParameter(command, "@ClientCode", DbType.String, objUserInfo.ClientCode);
                    database.AddInParameter(command, "@Remarks", DbType.String, objUserInfo.Remarks);
                    database.AddInParameter(command, "@ResetBy", DbType.String, objUserInfo.ModifyingAdmin);
                    database.AddInParameter(command, "@ResetBranch", DbType.String, objUserInfo.ModifyingBranch);
                    database.AddInParameter(command, "@mode", DbType.String, objUserInfo.Mode);
                    database.AddInParameter(command, "@ReturnValue", DbType.String, "");
                    using (var dataset = new DataSet())
                    {
                        database.LoadDataSet(command, dataset, "dtCustomerInfoApprove");

                        if (dataset.Tables.Count > 0)
                        {
                            dtableResult = dataset.Tables["dtCustomerInfoApprove"];
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



        #region BankLink Reset
        internal int ResetBankLink(UserInfo objUserInfo)//bankLink Reset
        {
            SqlConnection sqlCon = null;
            int ret;
            try
            {
                using (sqlCon = new SqlConnection(Connection.DatabaseConnection.ConnectionStr()))
                {
                    sqlCon.Open();
                    using (SqlCommand sqlCmd = new SqlCommand("[s_MNBankLinkKYCReset]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.Add(new SqlParameter("@UserType", objUserInfo.UserType));
                        sqlCmd.Parameters.Add(new SqlParameter("@UserName", objUserInfo.UserName));
                        sqlCmd.Parameters.Add(new SqlParameter("@ClientCode", objUserInfo.ClientCode));
                        sqlCmd.Parameters.Add(new SqlParameter("@ResetBy", objUserInfo.ApprovedBy));
                        sqlCmd.Parameters.Add(new SqlParameter("@ResetBranch", objUserInfo.BranchCode));
                        sqlCmd.Parameters.Add(new SqlParameter("@Remarks", objUserInfo.Remarks));
                        sqlCmd.Parameters.Add(new SqlParameter("@mode", objUserInfo.Mode));
                        sqlCmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.Output;

                        sqlCmd.ExecuteNonQuery();
                        ret = Convert.ToInt32(sqlCmd.Parameters["@ReturnValue"].Value);
                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
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


    }
}