﻿using MNepalWeb.Models;
using MNepalWeb.UserModels;
using MNepalWeb.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Globalization;
namespace MNepalWeb.Utilities
{
    public static class CustomerUtils
    {
        #region "GET User Profile Information By Searching Mobile Number"
        /// <summary>
        /// Get Customer Profile By Searching Mobile No
        /// </summary>
        /// <param name="mobileNo"></param>
        /// <returns></returns>
        public static DataTable GetUserProfileByMobileNo(string mobileNo)
        {
            var objUserModel = new CustomerUserModel();
            var objUserInfo = new UserInfo
            {
                ContactNumber1 = mobileNo,
                Mode = "SBMN" // SEARCH BY MobileNo
            };
            return objUserModel.GetCustomerDetailInformation(objUserInfo);
        }
        public static DataTable GetUserProfileByMobileNoA(string mobileNo)
        {
            var objUserModel = new CustomerUserModel();
            var objUserInfo = new UserInfo
            {
                ContactNumber1 = mobileNo,
                Mode = "SBMNA" // SEARCH BY MobileNo
            };
            return objUserModel.GetCustomerDetailInformation(objUserInfo);
        }
        #endregion

        #region "GET User Profile Information By Searching Name"

        /// <summary>
        /// Get Customer Profile By Searching Name
        /// </summary>
        /// <param name="mobileNo"></param>
        /// <returns></returns>
        public static DataTable GetUserProfileByName(string name)
        {
            var objUserModel = new CustomerUserModel();
            var objUserInfo = new UserInfo
            {
                Name = name,
                Mode = "SBName" // SEARCH BY Name
            };
            return objUserModel.GetCustomerDetailInformation(objUserInfo);
        }

        public static DataTable GetUserProfileByNameA(string name)
        {
            var objUserModel = new CustomerUserModel();
            var objUserInfo = new UserInfo
            {
                Name = name,
                Mode = "SBNameA" // SEARCH BY Name
            };
            return objUserModel.GetCustomerDetailInformation(objUserInfo);
        }
        #endregion

        #region "GET User Profile Information By Searching Account Number"

        /// <summary>
        /// Get Customer Profile By Searching Account Number
        /// </summary>
        /// <param name="mobileNo"></param>
        /// <returns></returns>
        public static DataTable GetUserProfileByAC(string accountNo)
        {
            var objUserModel = new CustomerUserModel();
            var objUserInfo = new UserInfo
            {
                WalletNumber = accountNo,
                Mode = "SBAC" // SEARCH BY Account Number
            };
            return objUserModel.GetCustomerDetailInformation(objUserInfo);
        }

        public static DataTable GetUserProfileByACA(string accountNo)
        {
            var objUserModel = new CustomerUserModel();
            var objUserInfo = new UserInfo
            {
                WalletNumber = accountNo,
                Mode = "SBACA" // SEARCH BY Account Number
            };
            return objUserModel.GetCustomerDetailInformation(objUserInfo);
        }

        #endregion

        #region "GET User Profile Information By Searching Name & Account Number"

        /// <summary>
        /// Get Customer Profile By Searching Name N Account Number
        /// </summary>
        /// <param name="name"></param>
        /// <param name="accountNo"></param>
        /// <returns></returns>
        public static DataTable GetUserProfileByNameAC(string name, string accountNo)
        {
            var objUserModel = new CustomerUserModel();
            var objUserInfo = new UserInfo
            {
                Name = name,
                WalletNumber = accountNo,
                Mode = "SBNAC" // SEARCH BY Name/AccountNumber
            };
            return objUserModel.GetCustomerDetailInformation(objUserInfo);
        }


        public static DataTable GetUserProfileByNameACA(string name, string accountNo)
        {
            var objUserModel = new CustomerUserModel();
            var objUserInfo = new UserInfo
            {
                Name = name,
                WalletNumber = accountNo,
                Mode = "SBNACA" // SEARCH BY Name/AccountNumber
            };
            return objUserModel.GetCustomerDetailInformation(objUserInfo);
        }


        #endregion

        #region "GET User Profile Information By Searching Name & Account Number"

        /// <summary>
        /// Get Customer Profile By Searching Name N Account Number
        /// </summary>
        /// <param name="name"></param>
        /// <param name="accountNo"></param>
        /// <param name="mobileNo"></param>
        /// <returns></returns>
        public static DataTable GetUserProfileByALL(string name, string accountNo, string mobileNo)
        {
            var objUserModel = new CustomerUserModel();
            var objUserInfo = new UserInfo
            {
                Name = name,
                WalletNumber = accountNo,
                ContactNumber1 = mobileNo,
                Mode = "SBALL" // SEARCH BY ALL
            };
            return objUserModel.GetCustomerDetailInformation(objUserInfo);
        }


        public static DataTable GetUserProfileByALLA(string name, string accountNo, string mobileNo)
        {
            var objUserModel = new CustomerUserModel();
            var objUserInfo = new UserInfo
            {
                Name = name,
                WalletNumber = accountNo,
                ContactNumber1 = mobileNo,
                Mode = "SBALLA" // SEARCH BY ALL
            };
            return objUserModel.GetCustomerDetailInformation(objUserInfo);
        }
        #endregion


        #region "GET Merchant User Profile Information By Searching Name"

        /// <summary>
        /// Get Merchant Profile By Searching Name N Account Number
        /// </summary>
        /// <param name="name"></param>
        /// <param name="accountNo"></param>
        /// <param name="mobileNo"></param>
        /// <returns></returns>
        public static DataTable GetMerchantUserProfileByALL(string name, string accountNo, string mobileNo)
        {
            var objUserModel = new CustomerUserModel();
            var objUserInfo = new UserInfo
            {
                Name = name,
                WalletNumber = accountNo,
                ContactNumber1 = mobileNo,
                Mode = "SMBALL" // SEARCH MERCHANT BY ALL
            };
            return objUserModel.GetCustomerDetailInformation(objUserInfo);
        }

        #endregion

        #region "GET Merchant User Profile Information By Searching Name"

        /// <summary>
        /// Get Merchant Profile By Searching Name
        /// </summary>
        /// <param name="mobileNo"></param>
        /// <returns></returns>
        public static DataTable GetMerchantUserProfileByName(string name)
        {
            var objUserModel = new CustomerUserModel();
            var objUserInfo = new UserInfo
            {
                Name = name,
                Mode = "SMBN" // SEARCH MERCHANT BY Name
            };
            return objUserModel.GetCustomerDetailInformation(objUserInfo);
        }

        #endregion

        #region "Customer Modification Utilities"
        public static bool UpdateCustomerUserInfo(this UserInfo member)
        {
            var objMemberInfo = new UserInfo()
            {
                ClientCode = member.ClientCode,
                Address = member.Address,
                Status = member.Status,
                Name = member.Name,
                FName = member.FName,
                MName = member.MName,
                LName = member.LName,
                ClientStatus = member.ClientStatus,
                Gender = member.Gender,
                ProfileCode = member.ProfileCode,
                EmailAddress = member.EmailAddress,
                NewMobileNo = member.NewMobileNo,
                ContactNumber1 = member.ContactNumber1,
                ContactNumber2 = member.ContactNumber2,
                //WalletNumber = member.WalletNumber,
                BankNo = member.BankNo,
                BranchCode = member.BranchCode,
                BankAccountNumber = member.BankAccountNumber,
                IsApproved = member.IsApproved,
                //IsRejected = member.IsRejected,
                PIN = member.PIN,
                //COC = member.COC,
                AcNumber = member.AcNumber,
                Alias = member.Alias,
                AcOwner = member.AcOwner,
                TBranchCode = member.TBranchCode,
                TxnEnabled = member.TxnEnabled,
                IsPrimary = member.IsPrimary,
                AcType = member.AcType,
                AdminUserName = member.AdminUserName,
                AdminBranch = member.AdminBranch,
                TxnAccounts = member.TxnAccounts,
                Mode = "UCINFO",// Modify Customer Info

            };
            return new CustomerUserModel().UpdateCustomerUserInfo(objMemberInfo) > 0;
        }
        public static bool InsertMakerChecker(string ClientCode, string ModifyingAdmin, string ModifyingBranch, string ModifiedField, string AccToDelete, string AccToAdd)
        {
            return new CustomerUserModel().InsertIntoMakerChecker(ClientCode, ModifyingAdmin, ModifyingBranch, ModifiedField, AccToDelete, AccToAdd) == 100;
        }
        public static bool InsertMakerChecker(string ClientCode, string ModifyingAdmin, string ModifyingBranch, string ModifiedField)
        {
            return new CustomerUserModel().InsertIntoMakerCheckerAdmin(ClientCode, ModifyingAdmin, ModifyingBranch, ModifiedField) == 100;
        }
        public static bool UpdateRejectedCustomerUserInfo(this UserInfo member)
        {
            var objMemberInfo = new UserInfo()
            {
                ClientCode = member.ClientCode,
                Address = member.Address,

                CProvince = member.CProvince,
                CDistrictID = member.CDistrictID,
                CMunicipalityVDC = member.CMunicipalityVDC,
                CWardNo = member.CWardNo,
                CHouseNo = member.CHouseNo,
                CStreet = member.CStreet,

                PProvince = member.PProvince,
                PDistrictID = member.PDistrictID,
                PMunicipalityVDC = member.PMunicipalityVDC,
                PWardNo = member.PWardNo,
                PHouseNo = member.PHouseNo,
                PStreet = member.PStreet,

                //---NISCHAL---
                DOB = member.DOB,                
                Country = member.Country,
                Nationality = member.Nationality,
                FatherName = member.FatherName,
                MotherName = member.MotherName,
                GrandFatherName = member.GrandFatherName,
                MaritalStatus = member.MaritalStatus,
                SpouseName = member.SpouseName,
                FatherInlawName = member.FatherInlawName,
                Occupation = member.Occupation,
                Document = member.Document,
                Citizenship = member.Citizenship,
                CitizenshipIssueDate = member.CitizenshipIssueDate,                
                CitizenshipPlaceOfIssue = member.CitizenshipPlaceOfIssue,
                License = member.License,
                LicensePlaceOfIssue = member.LicensePlaceOfIssue,                
                LicenseIssueDate = member.LicenseIssueDate,
                LicenseExpireDate = member.LicenseExpireDate,
                Passport = member.Passport,
                PassportPlaceOfIssue = member.PassportPlaceOfIssue,                
                PassportIssueDate = member.PassportIssueDate,
                PassportExpireDate = member.PassportExpireDate,
                PassportImage = member.PassportImageName,
                FrontImage = member.FrontImageName,
                BackImage = member.BackImageName,
                //---NISCHAL END---


                Status = member.Status,
                Name = member.Name,
                FName = member.FName,
                MName = member.MName,
                LName = member.LName,
                ClientStatus = member.ClientStatus,
                Gender = member.Gender,
                ProfileCode = member.ProfileCode,
                EmailAddress = member.EmailAddress,
                NewMobileNo = member.NewMobileNo,
                ContactNumber1 = member.ContactNumber1,
                ContactNumber2 = member.ContactNumber2,
                //WalletNumber = member.WalletNumber,
                BankNo = member.BankNo,
                BranchCode = member.BranchCode,
                BankAccountNumber = member.BankAccountNumber,
                IsApproved = member.IsApproved,
                //IsRejected = member.IsRejected,
                PIN = member.PIN,
                //COC = member.COC,
                AcNumber = member.AcNumber,
                Alias = member.Alias,
                AcOwner = member.AcOwner,
                TBranchCode = member.TBranchCode,
                TxnEnabled = member.TxnEnabled,
                IsPrimary = member.IsPrimary,
                AcType = member.AcType,
                AdminUserName = member.AdminUserName,
                AdminBranch = member.AdminBranch,
                TxnAccounts = member.TxnAccounts,
                //Added//
                Transaction = member.Transaction,
                DateRange = member.DateRange,
                StartDate = member.StartDate,
                EndDate = member.EndDate,
                TransactionLimit = member.TransactionLimit,
                TransactionCount = member.TransactionCount,
                TransactionLimitDaily = member.TransactionLimitDaily,
                TransactionLimitMonthly = member.TransactionLimitMonthly,
                LimitType = member.LimitType,

                Mode = "UCBRINFO",// Modify Bank Customer Info NISCHAL

            };
            return new CustomerUserModel().UpdateCustomerUserInfo(objMemberInfo) > 0;
        }
        public static bool UpdateAgentInfo(this UserInfo member)
        {
            var objMemberInfo = new UserInfo()
            {
                ClientCode = member.ClientCode,
                Address = member.Address,
                Name = member.Name,

                EmailAddress = member.EmailAddress,
                BankNo = member.BankNo,
                ContactNumber1 = member.ContactNumber1,
                ContactNumber2 = member.ContactNumber2,

                BranchCode = member.BranchCode,
                BankAccountNumber = member.BankAccountNumber




            };
            return new CustomerUserModel().UpdateAgentInfo(objMemberInfo) > 0;
        }
        public static bool UpdateAdminInfo(this UserInfo member)
        {
            var objMemberInfo = new UserInfo()
            {
                ClientCode = member.ClientCode,

                Name = member.Name,

                ProfileCode = member.ProfileName,
                EmailAddress = member.EmailAddress,
                BranchCode = member.BranchCode,
                COC = member.COC,
                Status = member.Status,
                AdminUserName = member.AdminUserName,
                AdminBranch = member.AdminBranch,



                Mode = "UAINFO",// Modify Customer Info

            };
            return new CustomerUserModel().UpdateAdminInfo(objMemberInfo) > 0;
        }
        public static bool UpdateMerchantInfo(this UserInfo member)
        {
            var objMemberInfo = new UserInfo()
            {
                ClientCode = member.ClientCode,
                Address = member.Address,
                Status = member.Status,
                Name = member.Name,
                ClientStatus = member.ClientStatus,
                EmailAddress = member.EmailAddress,
                ContactNumber1 = member.ContactNumber1,
                ContactNumber2 = member.ContactNumber2,
                BranchCode = member.BranchCode,
                BankAccountNumber = member.BankAccountNumber,
                BankNo = member.BankNo,
                IsApproved = member.IsApproved,
                IsRejected = member.IsRejected

            };
            return new CustomerUserModel().UpdateMerchantInfo(objMemberInfo) > 0;
        }
        #endregion
        #region "GET DELETE User Profile Information By Searching Mobile Number"

        /// <summary>
        /// Get Customer Profile By Searching Mobile No
        /// </summary>
        /// <param name="mobileNo"></param>
        /// <returns></returns>
        public static DataTable GetDeleteUserProfileByMobileNo(string mobileNo)
        {
            var objUserModel = new CustomerUserModel();
            var objUserInfo = new UserInfo
            {
                ContactNumber1 = mobileNo,
                Mode = "DSBMN" // SEARCH BY MobileNo
            };
            return objUserModel.GetCustomerDetailInformation(objUserInfo);
        }

        #endregion
        internal static DataTable GetUnApproveCustProfile(string userType)
        {
            var objUserModel = new CustApproveUserModels();
            var objUserInfo = new UserInfo
            {
                UserType = userType,
                Mode = "GCUA" // --APPROVE LIST FOR CUSTOMER
            };
            return objUserModel.GetCustApproveDetailInfo(objUserInfo);
        }
        #region Get BankLink UnApprove Detail
        internal static DataTable GetBankLinkApproveDetailInfo(this UserInfo member)
        {
            var objUserModel = new CustApproveUserModels();
            var objUserInfo = new UserInfo
            {
                ClientCode = member.ClientCode,
                Mode = "GABL" // --APPROVE LIST FOR CUSTOMER
            };
            return objUserModel.GetBankLinkApproveDetailInfo(objUserInfo);
        }
        #endregion

        #region Get BankLink Detail
        internal static DataTable GetBankLinkApproveDetailInformation(this UserInfo member)
        {
            var objUserModel = new CustApproveUserModels();
            var objUserInfo = new UserInfo
            {
                ClientCode = member.ClientCode,
                Mode = "GADL" // --APPROVE DETAIL FOR CUSTOMER
            };
            return objUserModel.GetBankLinkApproveDetailInfo(objUserInfo);
        }
        #endregion

        internal static int CustInfoApproval(this UserInfo member)
        {
            var objUserModel = new CustApproveUserModels();
            var objUserInfo = new UserInfo
            {
                ClientCode = member.ClientCode,
                ApprovedBy = member.ApprovedBy,
                IsApproved = "Approve",
                Mode = "UCINFO"     // SINGLE CUSTOMER INFO APPROVE
            };
            return objUserModel.CustInfoApprove(objUserInfo);
        }
        internal static bool ApproveCustModified(this UserInfo member)
        {
            var objUserModel = new CustApproveUserModels();
            var objUserInfo = new UserInfo
            {
                ClientCode = member.ClientCode,
                AdminUserName = member.AdminUserName,
                AdminBranch = member.AdminBranch

            };
            return objUserModel.ApproveCustModified(objUserInfo) == 100;
        }
        internal static bool RejectCustModified(this UserInfo member)
        {
            var objUserModel = new CustApproveUserModels();
            var objUserInfo = new UserInfo
            {
                ClientCode = member.ClientCode,
                AdminUserName = member.AdminUserName,
                AdminBranch = member.AdminBranch,
                Remarks = member.Remarks

            };
            return objUserModel.RejectCustModified(objUserInfo) == 100;
        }
        internal static int CustInfoReject(this UserInfo member)
        {
            var objUserModel = new CustApproveUserModels();
            var objUserInfo = new UserInfo
            {
                ClientCode = member.ClientCode,
                UserName = member.UserName,
                Remarks = member.Remarks

            };
            return objUserModel.CustInfoReject(objUserInfo);
        }
        internal static int CustRenew(string ClientCode, string UserName, string Amount, string AdminUserName, string RRNumber)
        {
            var objUserModel = new CustApproveUserModels();
            return objUserModel.CustRenew(ClientCode, UserName, Amount, AdminUserName, RRNumber);
        }
        internal static int ChangeCustStaus(string ClientCode, string Status)
        {
            var objUserModel = new CustApproveUserModels();
            return objUserModel.ChangeCustStatus(ClientCode, Status);
        }
        internal static int CustInfoApprovalALl(List<UserInfo> userInfoApproval)
        {
            var objUserModel = new CustApproveUserModels();
            string mode = "UCIASEL";      // MULITPLE CUSTOMER INFO APPROVE

            return objUserModel.CustInfoApproveSelected(userInfoApproval, mode);
        }

        internal static DataTable GetModifiedCustomer(string userType, string BankCode)
        {
            var objUserModel = new CustApproveUserModels();
            var objUserInfo = new UserInfo
            {
                UserType = userType,
                BankCode = BankCode,
                Mode = "GCMA" // --APPROVE LIST FOR CUSTOMER
            };
            return objUserModel.GetCustApproveDetailInfo(objUserInfo);
        }
        internal static int CustmodifyApproval(this UserInfo member)
        {
            var objUserModel = new CustApproveUserModels();
            var objUserInfo = new UserInfo
            {
                ClientCode = member.ClientCode,
                ApprovedBy = member.ApprovedBy,
                IsApproved = "Approve",
                Mode = "UCMA"     // SINGLE CUSTOMER INFO APPROVE
            };
            return objUserModel.CustInfoApprove(objUserInfo);
        }
        public static string GeneratePin()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max).ToString();
        }

        static Random rnd = new Random();
        public static string GeneratePassword()
        {
            int maxlength = 5;
            int minlength = 5;
            int length = rnd.Next(minlength, maxlength);
            const string valid = "abcdefghijklmnopqrstuvwxyz";
            StringBuilder res = new StringBuilder();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
        public static void LogSMS(SMSLog Log)
        {
            CustomerUserModel model = new CustomerUserModel();
            Log.SentOn = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt ");
            model.InsertSMSLog(Log);
        }
        public static ChargeVM GetCustCharge(string ClientCode)
        {
            CustomerUserModel model = new CustomerUserModel();
            return model.GetCustCharge(ClientCode);

        }
        public static List<UserInfo> GetRejectedUser(string UserName)
        {
            CustomerUserModel model = new CustomerUserModel();
            return model.GetRejectedUser(UserName);

        }
        internal static int AdminInfoApprove(UserInfo member, string Rejected, string Approve)
        {
            var objUserModel = new CustomerUserModel();
            var objUserInfo = new UserInfo
            {
                ClientCode = member.ClientCode,
                UserName = member.UserName,
                Remarks = member.Remarks,
                AdminUserName = member.AdminUserName,
                AdminBranch = member.AdminBranch

            };
            return objUserModel.AdminApprove(objUserInfo, Rejected, Approve);
        }
        internal static int AdminInfoReject(UserInfo member, string Rejected, string Approve)
        {
            var objUserModel = new CustomerUserModel();
            var objUserInfo = new UserInfo
            {
                ClientCode = member.ClientCode,
                UserName = member.UserName,
                Remarks = member.Remarks,
                AdminUserName = member.AdminUserName,
                AdminBranch = member.AdminBranch

            };
            return objUserModel.AdminReject(objUserInfo, Rejected, Approve);
        }
        public static List<UserInfo> GetUnApproveAdminProfile(string userType, string UserName)
        {
            var objUserModel = new CustApproveUserModels();
            var objUserInfo = new UserInfo
            {
                UserType = userType


            };
            return objUserModel.GetAdminApproveDetailInfo(objUserInfo, UserName);
        }
        internal static int AdminRegisterReject(UserInfo member, string Rejected)
        {
            var objUserModel = new CustomerUserModel();
            var objUserInfo = new UserInfo
            {
                ClientCode = member.ClientCode,
                UserName = member.UserName,
                Remarks = member.Remarks,
                AdminUserName = member.AdminUserName,
                AdminBranch = member.AdminBranch


            };
            return objUserModel.AdminRegReject(objUserInfo, Rejected);
        }
        internal static int AdminRegisterApprove(UserInfo member, string Approve)
        {
            var objUserModel = new CustomerUserModel();
            var objUserInfo = new UserInfo
            {
                ClientCode = member.ClientCode,

            };
            return objUserModel.AdminRegApprove(objUserInfo, Approve);
        }
        internal static List<UserInfo> GetUserStatus(string BranchCode, bool COC, string MobileNo)
        {
            var objUserModel = new CustomerUserModel();
            return objUserModel.GetUserStatusList(BranchCode, COC, MobileNo);

        }

        //public static bool InsertIntoMakerChecker(MNMakerChecker model)
        //{

        //    return new CustomerUserModel().InsertIntoMakerChecker(model) == 1;
        //}
        internal static bool CustStatusApprove(string ClientCode)
        {
            var objUserModel = new CustomerUserModel();

            return objUserModel.StatusApprove(ClientCode) == 1;
        }
        internal static bool CustStatusReject(string ClientCode, string Status)
        {
            var objUserModel = new CustomerUserModel();

            return objUserModel.StatusReject(ClientCode, Status) == 1;
        }
        //Approve and edit Admin info From Rejected List
        public static bool AprvRjAdmin(this UserInfo member)
        {
            var objMemberInfo = new UserInfo()
            {
                ClientCode = member.ClientCode,

                Name = member.Name,
                ProfileCode = member.ProfileName,
                EmailAddress = member.EmailAddress,
                BranchCode = member.BranchCode,
                COC = member.COC,
                Status = member.Status,
                AdminUserName = member.AdminUserName,
                AdminBranch = member.AdminBranch,



                Mode = "ARARL",

            };
            return new CustomerUserModel().AprvRjAdmin(objMemberInfo) > 0;
        }


        public static int ApproveModifiedAdmin(UserInfo model)
        {
            return new CustomerUserModel().ApproveModifiedAdmin(model);
        }

        public static int RejectModifiedAdmin(UserInfo model)
        {
            return new CustomerUserModel().RejectModifiedAdmin(model);
        }

        //internal static Customer GetByCode(string ClientCode)
        //{
        //    var objUserModel = new CustomerUserModel();

        //    return objUserModel.GetCustById(ClientCode);
        //}

        //test//
        //Customer Rejected List search//
        //Unapproved Customer

        internal static DataTable GetRejectedCustomer()
        {
            var objUserModel = new CustomerUserModel();
            var objUserInfo = new UserInfo
            {
                Mode = "GCMA"
            };
            return objUserModel.GetAllCusRejUnInformation();
        }

        //By Mobile No//
        public static DataTable GetCusRejUnByMobileNo(string mobileNo, string userType) //UnApproved Customer//
        {
            var objUserModel = new CustomerUserModel();
            var objUserInfo = new UserInfo
            {
                ContactNumber1 = mobileNo,
                Mode = "SRLUCM" // SEARCH BY MobileNo
            };
            return objUserModel.GetCusRejUnInformation(objUserInfo);
        }

        //By Name//
        public static DataTable GetCusRejUnByName(string name, string userType)
        {
            var objUserModel = new CustomerUserModel();
            var objUserInfo = new UserInfo
            {
                Name = name,
                Mode = "SRLUCN" // SEARCH BY Name
            };
            return objUserModel.GetCusRejUnInformation(objUserInfo);
        }

        //By Account Number//
        public static DataTable GetCusRejUnByAcNumber(string accountNo, string userType)
        {
            var objUserModel = new CustomerUserModel();
            var objUserInfo = new UserInfo
            {
                WalletNumber = accountNo,
                Mode = "SRLUCA" // SEARCH BY Name
            };
            return objUserModel.GetCusRejUnInformation(objUserInfo);
        }

        //Approved Customer

        internal static DataTable GetCustRejAp()
        {
            var objUserModel = new CustomerUserModel();
            var objUserInfo = new UserInfo
            {
                Mode = "GCMA"
            };
            return objUserModel.GetCusModRejInformation();
        }

        //By Mobile No//
        public static DataTable GetCusRejApByMobileNo(string mobileNo, string userType) //Search Rerjected List of Approved Customer by Mobile
        {
            var objUserModel = new CustomerUserModel();
            var objUserInfo = new UserInfo
            {
                ContactNumber1 = mobileNo,
                Mode = "SRLACM" // SEARCH BY MobileNo
            };
            return objUserModel.GetCusRejUnInformation(objUserInfo);
        }

        //By Name//
        public static DataTable GetCusRejApByName(string name, string userType)
        {
            var objUserModel = new CustomerUserModel();
            var objUserInfo = new UserInfo
            {
                Name = name,
                Mode = "SRLACN" // SEARCH BY Name
            };
            return objUserModel.GetCusRejUnInformation(objUserInfo);
        }

        //By Account Number//
        public static DataTable GetCusRejApByAcNumber(string accountNo, string userType)
        {
            var objUserModel = new CustomerUserModel();
            var objUserInfo = new UserInfo
            {
                WalletNumber = accountNo,
                Mode = "SRLACA" // SEARCH BY Name
            };
            return objUserModel.GetCusRejUnInformation(objUserInfo);
        }

        //Bank Link Approve
        internal static int BankLinkApprove(this UserInfo userInfoBankinkApprove, string bankCode)
        {
            var objUserModel = new CustApproveUserModels();
            var objUserInfo = new UserInfo
            {
                ClientCode = userInfoBankinkApprove.ClientCode,
                AcNumber = userInfoBankinkApprove.AcNumber,
                BranchCode = userInfoBankinkApprove.BranchCode,
                UserBranchCode = userInfoBankinkApprove.UserBranchCode,
                ApprovedBy = userInfoBankinkApprove.ApprovedBy,
                ApprovedDate = userInfoBankinkApprove.ApprovedDate,
                BankCode = bankCode

            };
            return objUserModel.BankLinkApprove(objUserInfo);
        }

        //Bank Link Reject
        internal static int BankLinkReject(this UserInfo userInfoBankinkApprove)
        {
            var objUserModel = new CustApproveUserModels();
            var objUserInfo = new UserInfo
            {
                ClientCode = userInfoBankinkApprove.ClientCode,
                Remarks = userInfoBankinkApprove.Remarks

            };
            return objUserModel.BankLinkReject(objUserInfo);
        }

        //retrieve AccNo
        internal static DataTable GetAccNo(this UserInfo member)
        {
            var objUserModel = new CustApproveUserModels();
            var objUserInfo = new UserInfo
            {
                ClientCode = member.ClientCode,
                Mode = "GANO" //GET ACC NO
            };
            return objUserModel.GetAccNo(objUserInfo);
        }

        //retrieve Verify Account NO
        internal static DataTable GetVerifyAccNo(this UserInfo member)
        {
            var objUserModel = new CustApproveUserModels();
            var objUserInfo = new UserInfo
            {
                ClientCode = member.ClientCode,
                Mode = "GVANO" //GET ACC NO
            };
            return objUserModel.GetAccNo(objUserInfo);
        }


        
        #region
        internal static DataTable CheckBankLink(string ContactNumber1)
        {
            var objUserModel = new CustomerUserModel();
            var objUserInfo = new UserInfo
            {               
                UserName = ContactNumber1,
                Mode = "CBL" //Check Bank Link
            };
            return objUserModel.ResetBankLinkKYC(objUserInfo);
        }
        #endregion

        public static DataTable GetUserProfileInfoDS(string clientCode)
        {
            var objUserModel = new LoginUserModels();
            var objUserInfo = new UserInfo
            {
                ClientCode = clientCode,
                Mode = "GAPI" // GET User PROFILE INFORMATION
            };
            return objUserModel.GetUserInformation(objUserInfo);
        }



        #region get CustomerKYCDetail
        internal static DataTable GetResetCustKYC(string UserType)
        {
            var objUserModel = new CustomerUserModel();
            var objUserInfo = new UserInfo
            {
                UserType = UserType,
                Mode = "GBLR" // --Bank Link Reset LIST FOR CUSTOMER
            };
            return objUserModel.ResetBankLinkKYC(objUserInfo);
        }
        #endregion


        #region
        internal static int VerifyReset(this UserInfo member)
        {
            var objUserModel = new CustomerUserModel();
            var objUserInfo = new UserInfo
            {
                ClientCode = member.ClientCode,
                ApprovedBy = member.ModifyingAdmin,
                UserName = member.UserName,
                UserType = "user",
                BranchCode = member.ModifyingBranch,
                Remarks = member.Remarks,
                Mode = "VBLR" //Verify BankLink KYC Reset
            };
            return objUserModel.ResetBankLink(objUserInfo);
        }
        #endregion

        #region get CustomerKYCDetail
        internal static DataTable GetBanklinkCustReset(string UserType)
        {
            var objUserModel = new CustomerUserModel();
            var objUserInfo = new UserInfo
            {
                UserType = UserType,
                Mode = "GBVC" // --Reset LIST FOR CUSTOMER
            };
            return objUserModel.ResetBankLinkKYC(objUserInfo);
        }
        #endregion


        internal static int RejectBankLinkReset(this UserInfo member)
        {
            var objUserModel = new CustomerUserModel();
            var objUserInfo = new UserInfo
            {
                ClientCode = member.ClientCode,
                ApprovedBy = member.ApprovedBy,
                UserName = member.UserName,
                UserType = "user",
                BranchCode = member.BranchCode,
                Remarks = member.Remarks,
                Mode = "BLRR"  //BankLink Reset Reject
            };
            return objUserModel.ResetBankLink(objUserInfo);
        }


        internal static int BLResetApprove(this UserInfo member)
        {
            var objUserModel = new CustomerUserModel();
            var objUserInfo = new UserInfo
            {
                ClientCode = member.ClientCode,
                ApprovedBy = member.ApprovedBy,
                UserName = member.UserName,
                UserType = member.UserType,
                BranchCode = member.BranchCode,
                Remarks = member.Remarks,
                Mode = "BLRA" //KYC Reset Approve
            };
            return objUserModel.ResetBankLink(objUserInfo);
        }



    }
}