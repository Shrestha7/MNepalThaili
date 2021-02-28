using MNSuperadmin.Models;
using MNSuperadmin.UserModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;


namespace MNSuperadmin.Utilities
{
    public class ProfileUtils
    {
        #region "GET User Profile Information"

        public static DataTable GetUserProfileInfo(string clientCode)
        {
            var objUserModel = new LoginUserModels();
            var objUserInfo = new UserInfo
            {
                ClientCode = clientCode,
                Mode = "GCUPI" // GET USER PROFILE INFORMATION
            };
            return objUserModel.GetUserInformation(objUserInfo);
        }
        

        public static DataSet GetUserProfileInfoDS(string clientCode)
        {
            var objUserModel = new LoginUserModels();
            var objUserInfo = new UserInfo
            {
                ClientCode = clientCode,
                Mode = "GCUPI" // GET USER PROFILE INFORMATION
            };
            return objUserModel.GetUserInformationDSet(objUserInfo);
        }

        public static DataTable GetAgentProfileInfo(string clientCode)
        {
            var objUserModel = new LoginUserModels();
            var objUserInfo = new UserInfo
            {
                ClientCode = clientCode,
                Mode = "GAPI" // GET AGENT PROFILE INFORMATION
            };
            return objUserModel.GetAgentInformation(objUserInfo);
        }

        

        public static DataSet GetAgentProfileInfoDS(string clientCode)
        {
            var objUserModel = new LoginUserModels();
            var objUserInfo = new UserInfo
            {
                ClientCode = clientCode,
                Mode = "GAPI" // GET AGENT PROFILE INFORMATION
            };
            return objUserModel.GetAgentInformationDSet(objUserInfo);
        }



        public static DataSet GetCustModifiedValue(string clientCode)
        {
            var objUserModel = new LoginUserModels();
            var objUserInfo = new UserInfo
            {
                ClientCode = clientCode
               
            };
            return objUserModel.GetCustModifiedValue(objUserInfo);
        }

        public static DataSet GetSuperAdminModifiedValue(string clientCode)
        {
            var objUserModel = new LoginUserModels();
            var objUserInfo = new UserInfo
            {
                ClientCode = clientCode

            };
            return objUserModel.GetSuperAdminModifiedValue(objUserInfo);
        }
        public static DataSet GetAdminModifiedValue(string clientCode)
        {
            var objUserModel = new LoginUserModels();
            var objUserInfo = new UserInfo
            {
                ClientCode = clientCode

            };
            return objUserModel.GetAdminModifiedValue(objUserInfo);
        }
        #endregion


        #region "GET SuperAdmin Profile Information"

        public static DataTable GetSuperAdminProfile()
        {
            var objUserModel = new LoginUserModels();
            var objUserInfo = new UserInfo
            {
                Mode = "GSAP" // GET SUPER ADMIN PROFILE
            };
            return objUserModel.GetAllUserInformation(objUserInfo);
        }

        #endregion


        #region "GET Admin Profile Information"

        public static DataTable GetAdminProfile()
        {
            var objUserModel = new LoginUserModels();
            var objUserInfo = new UserInfo
            {
                Mode = "GVSP" // GET ADMIN PROFILE
            };
            return objUserModel.GetAllUserInformation(objUserInfo);
        }

        #endregion

        #region "GET Admin Profile Information"

        public static DataTable GetSuperadminExceptOwn(string userName)
        {
            var objUserModel = new LoginUserModels();
            var objUserInfo = new UserInfo
            {
                UserName = userName,
                Mode = "GVSPEO" // GET ADMIN PROFILE
            };
            return objUserModel.GetSuperadminExceptOwn(objUserInfo);
        }

        #endregion 

        #region "GET UnApprove Admin Profile Information"

        public static DataTable GetUnApproveAdminProfile()
        {
            var objUserModel = new LoginUserModels();
            var objUserInfo = new UserInfo
            {
                Mode = "GUSP" // GET UNAPPROVE ADMIN PROFILE
            };
            return objUserModel.GetAllUserInformation(objUserInfo);
        }

        #endregion


        #region "GET All Agent Profile Information"

        public static DataTable GetAllAgentProfile()
        {
            var objUserModel = new LoginUserModels();
            var objUserInfo = new UserInfo
            {
                Mode = "GAAP" // GET ALL AGENT PROFILE
            };
            return objUserModel.GetAllUserInformation(objUserInfo);
        }

        #endregion


        #region "GET All User Profile Information"

        public static DataTable GetAllUserProfile()
        {
            var objUserModel = new LoginUserModels();
            var objUserInfo = new UserInfo
            {
                Mode = "GAUP" // GET ALL USER PROFILE
            };
            return objUserModel.GetAllUserInformation(objUserInfo);
        }

        #endregion


        #region "REJECTED LIST FOR APPROVE CUSTOMER Information"

        public static DataTable GetApproveRJCustomerProfile(string userType)
        {
            var objUserModel = new LoginUserModels();
            var objUserInfo = new UserInfo
            {
                UserType = userType,
                Mode = "RLAC" // --REJECTED LIST FOR APPROVE CUSTOMER
            };
            return objUserModel.GetAllUserInformation(objUserInfo);
        }

        #endregion


        #region "REJECTED LIST FOR UNAPPROVE CUSTOMER Information"

        public static DataTable GetUnApproveRJCustomerProfile(string userType)
        {
            var objUserModel = new LoginUserModels();
            var objUserInfo = new UserInfo
            {
                UserType = userType,
                Mode = "RLUC" // --REJECTED LIST FOR UNAPPROVE CUSTOMER
            };
            return objUserModel.GetAllUserInformation(objUserInfo);
        }

        #endregion

        #region "GET Admin Modified List"

        public static DataTable GetAdminModify()
        {
            var objUserModel = new LoginUserModels();
            var objUserInfo = new UserInfo
            {

                Mode = "SAMA" // ADMIN MODIFICATION APPROVE
            };
            return objUserModel.GetAllUserInformation(objUserInfo);
        }

        #endregion

        #region "REJECTED LIST FOR APPROVE ADMIN Information"

        public static List<UserInfo> GetApproveRJAdminProfile(string IsModified)
        {
            var objUserModel = new LoginUserModels();
            var objUserInfo = new UserInfo();
           
            return objUserModel.GetAllAdminInformation(objUserInfo, IsModified);
        }

        #endregion

       

        //#region "REJECTED LIST FOR UNAPPROVE Admin Information"

        //public static DataTable GetUnApproveRJAdminProfile()
        //{
        //    var objUserModel = new LoginUserModels();
        //    var objUserInfo = new UserInfo();

        //    return objUserModel.GetAllAdminInformation(objUserInfo);
        //}

        //#endregion

        #region SELF Registration Approval
        public static DataSet GetSelfRegDetailDS(string clientCode)
        {
            var objUserModel = new LoginUserModels();
            var objUserInfo = new CustomerSRInfo
            {
                ClientCode = clientCode,
                
            };
            return objUserModel.GetSelfRegInfoDSet(objUserInfo);
        }

        #endregion

        #region Quick SELF Registration Approval
        public static DataSet GetSelfRegDetailDSWalletCustStatus(string clientCode)
        {
            var objUserModel = new LoginUserModels();
            var objUserInfo = new CustomerSRInfo
            {
                ClientCode = clientCode,

            };
            return objUserModel.GetSelfRegInfoDSetWalletCustStatus(objUserInfo);
        }

        #endregion

        //for agent commission

        #region Quick SELF Registration Approval
        public static DataSet GetSelfRegDetailDSWalletCustStatusAgentCommission(string Id)
        {
            var objUserModel = new LoginUserModels();
            var objUserInfo = new CustomerSRInfo
            {
                //ClientCode = clientCode,
                Id = Id,

            };
            return objUserModel.GetSelfRegInfoDSetWalletCustStatusAgentCommission(objUserInfo);
        }

        #endregion
        //start milayako 01
        #region AGENT Registration Approval
        public static DataSet GetAgentRegDetailDS(string clientCode)
        {
            var objUserModel = new LoginUserModels();
            var objUserInfo = new CustomerSRInfo
            {
                ClientCode = clientCode,

            };
            return objUserModel.GetAgentRegInfoDSet(objUserInfo);
        }

        #endregion
        //end milayako 01
        
        #region Check PassChanged for superadmin  ///for in reset sending to login after refresh
        public static DataTable PassChangedSuperadmin(string clientCode)
        {
            var objUserModel = new LoginUserModels();
            var objUserInfo = new UserInfo
            {
                ClientCode = clientCode
            };
            return objUserModel.PassChangedSuperadmin(objUserInfo);
        }
        #endregion

        #region Create Fee
        public static int CreateFee(CustomerSRInfo userInfo)
        {
            var objCreateFee = new LoginUserModels();
            var objCreateFeeInfo = new CustomerSRInfo
            {
                FeeId = userInfo.FeeId,
                FeeType = userInfo.FeeType,
                Details = userInfo.Details,
                FlatFee = userInfo.FlatFee,
                BranchFeeAcc=userInfo.BranchFeeAcc,
                FeeAcc = userInfo.FeeAcc,
                TieredStart = userInfo.TieredStart,
                TieredEnd = userInfo.TieredEnd,
                MinAmt = userInfo.MinAmt,
                MaxAmt =  userInfo.MaxAmt,
                Percentage = userInfo.Percentage,
                CreatedBy=userInfo.CreatedBy
        };
            return objCreateFee.CreateFee(objCreateFeeInfo);
        }
        #endregion

        #region Fee Approve List
        public static List<UserInfo> GetUnApproveFee()
        {
            var objUserModel = new LoginUserModels();
            var objUserInfo = new UserInfo{};
            return objUserModel.GetFeeApproveDetailInfo();
        }

        internal static int FeeApprove(UserInfo member, string Approve)
        {
            var objUserModel = new LoginUserModels();
            var objUserInfo = new UserInfo
            {
                FeeID = member.FeeID,
                ApprovedBy = member.ApprovedBy

            };
            return objUserModel.FeeRegApprove(objUserInfo, Approve);
        }

        internal static int FeeReject(UserInfo member, string Approve)
        {
            var objUserModel = new LoginUserModels();
            var objUserInfo = new UserInfo
            {
                FeeID = member.FeeID,
                RejectedBy = member.RejectedBy,
                Remarks = member.Remarks

            };
            return objUserModel.FeeRegReject(objUserInfo, Approve);
        }
        #endregion

        #region Approve Fee Detail
        public static DataTable GetFeeDetail(string FeeID)
        {
            var objUserModel = new LoginUserModels();
            var objUserInfo = new UserInfo
            {
                FeeID = FeeID
            };
            return objUserModel.GetFeeDetail(objUserInfo);
        }
        #endregion

        #region Fee Rejected List
        public static List<UserInfo> GetRejectedFee()
        {
            var objUserModel = new LoginUserModels();
            var objUserInfo = new UserInfo { };
            return objUserModel.GetFeeRejectedDetailInfo();
        }

        internal static int FeeRejectedApprove(UserInfo member)
        {
            var objUserModel = new LoginUserModels();
            var objUserInfo = new UserInfo
            {
                FeeID = member.FeeID,
                FeeType = member.FeeType,
                Details = member.Details,
                Percentage = member.Percentage,
                FlatFee = member.FlatFee,
                TieredStart = member.TieredStart,
                TieredEnd = member.TieredEnd,
                MinAmt = member.MinAmt,
                MaxAmt = member.MaxAmt,
                BranchFeeAc = member.BranchFeeAc,
                FeeAc = member.FeeAc,
                UserName = member.UserName

        };
            return objUserModel.FeeRejectedApprove(objUserInfo);
        }
        #endregion

        #region "check Fee ID"
        public static DataTable GetCheckFeeID(string FeeID)
        {
            var objMobileModel = new LoginUserModels();
            var objMobileInfo = new UserInfo
            {
                FeeID = FeeID
            };
            return objMobileModel.GetFeeID(objMobileInfo);
        }
        #endregion
    }
}