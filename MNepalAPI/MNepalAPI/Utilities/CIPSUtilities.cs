using MNepalAPI.Models;
using MNepalAPI.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MNepalAPI.Utilities
{
    public class CIPSUtilities
    {
        public static int ConnectIPS(CIPSResponse cIPSResponse)
        {
            var objresCIPSModel= new ConnectIPSUserModel();
            var objresCIPSInfo = new CIPSResponse
            {
                batchResponseCode = cIPSResponse.batchResponseCode,
                batchResponseMessage = cIPSResponse.batchResponseMessage,
                batchId = cIPSResponse.batchId,
                debitStatus = cIPSResponse.debitStatus,
                batchResponseId = cIPSResponse.batchResponseId,
                txnResponseCode = cIPSResponse.txnResponseCode,
                txnResponseMessage = cIPSResponse.txnResponseMessage,
                txnId = cIPSResponse.txnId,
                instructionId = cIPSResponse.instructionId,
                creditStatus = cIPSResponse.creditStatus,
                amount = cIPSResponse.amount,
                username = cIPSResponse.username,
                thailiUserName = cIPSResponse.thailiUserName,
                dateTime = cIPSResponse.dateTime
            };
            return objresCIPSModel.ResponseConnectIPSInfo(objresCIPSInfo);
        }

        public static int cipsMNRequest(MNRequestResponse cIPSResponse)
        {
            var objresCIPSModel = new ConnectIPSUserModel();
            var objresCIPSInfo = new MNRequestResponse
            {
                originId = cIPSResponse.originId,
                originType = cIPSResponse.originType,
                serviceCode = cIPSResponse.serviceCode,
                sourceBankCode = cIPSResponse.sourceBankCode,
                sourceBranchCode = cIPSResponse.sourceBranchCode,
                sourceAccountNumber = cIPSResponse.sourceAccountNumber,
                destBranchCode = cIPSResponse.destBranchCode,
                destBankCode = cIPSResponse.destBankCode,
                destAccountNumber = cIPSResponse.destAccountNumber,
                amount = cIPSResponse.amount,
                feeAmount = cIPSResponse.feeAmount,
                feeId = cIPSResponse.feeId,
                traceNo = cIPSResponse.traceNo,
                tranDate = cIPSResponse.tranDate,
                retrievalReference = cIPSResponse.retrievalReference,
                desc1 = cIPSResponse.desc1,
                desc2 = cIPSResponse.desc2,
                desc3 = cIPSResponse.desc3,
                reversalStatus = cIPSResponse.reversalStatus,
                oTraceNo = cIPSResponse.oTraceNo,
                oTranDateTime = cIPSResponse.oTranDateTime,
                isProcessed = cIPSResponse.isProcessed,
                status = cIPSResponse.status,
                fromSMS = cIPSResponse.fromSMS,
                remark = cIPSResponse.remark,
                smsAlertType = cIPSResponse.smsAlertType,
                enteredAt = cIPSResponse.enteredAt,
                merchantId = cIPSResponse.merchantId,
                uId = cIPSResponse.uId
                
            };
            return objresCIPSModel.ConnectIPSMNRequest(objresCIPSInfo);
        }

        public static int cipsMNResponse(MNRequestResponse cIPSResponse)
        {
            var objresCIPSModel = new ConnectIPSUserModel();
            var objresCIPSInfo = new MNRequestResponse
            {
                originId = cIPSResponse.originId,
                originType = cIPSResponse.originType,
                serviceCode = cIPSResponse.serviceCode,
                sourceBankCode = cIPSResponse.sourceBankCode,
                sourceBranchCode = cIPSResponse.sourceBranchCode,
                sourceAccountNumber = cIPSResponse.sourceAccountNumber,
                destBranchCode = cIPSResponse.destBranchCode,
                destBankCode = cIPSResponse.destBankCode,
                destAccountNumber = cIPSResponse.destAccountNumber,
                amount = cIPSResponse.amount,
                feeId = cIPSResponse.feeId,
                feeAmount = cIPSResponse.feeAmount,
                traceNo = cIPSResponse.traceNo,
                tranDate = cIPSResponse.tranDate,
                tranTime = cIPSResponse.tranTime,
                retrievalReference = cIPSResponse.retrievalReference,
                responseCode = cIPSResponse.responseCode,
                responseDescription = cIPSResponse.responseDescription,
                balance = cIPSResponse.balance,
                accountHolderName = cIPSResponse.accountHolderName,
                miniStmtRecord = cIPSResponse.miniStmtRecord,
                reversalStatus = cIPSResponse.reversalStatus,
                tranId = cIPSResponse.tranId,
                destUsername = cIPSResponse.destUsername

            };
            return objresCIPSModel.ConnectIPSMNResponse(objresCIPSInfo);
        }

        public static int ConnectIPSRequest(ConnectIPS cIPSResponse, string thailiUserName, DateTime dateTime)
        {
            var objresCIPSModel = new ConnectIPSUserModel();
            var objresCIPSInfo = new CIPSRequest
            {
                batchId = cIPSResponse.cipsBatchDetail.batchId,
                batchAmount = cIPSResponse.cipsBatchDetail.batchAmount,
                batchCount = cIPSResponse.cipsBatchDetail.batchCount,
                batchCrncy = cIPSResponse.cipsBatchDetail.batchCrncy,
                categoryPurpose = cIPSResponse.cipsBatchDetail.categoryPurpose,
                debtorAgent = cIPSResponse.cipsBatchDetail.debtorAgent,
                debtorBranch = cIPSResponse.cipsBatchDetail.debtorBranch,
                debtorName = cIPSResponse.cipsBatchDetail.debtorName,
                debtorAccount = cIPSResponse.cipsBatchDetail.debtorAccount,
                debtorIdType = cIPSResponse.cipsBatchDetail.debtorIdType,
                debtorIdValue = cIPSResponse.cipsBatchDetail.debtorIdValue,
                debtorAddress = cIPSResponse.cipsBatchDetail.debtorAddress,
                debtorPhone = cIPSResponse.cipsBatchDetail.debtorPhone,
                debtorMobile = cIPSResponse.cipsBatchDetail.debtorMobile,
                debtorEmail = cIPSResponse.cipsBatchDetail.debtorEmail,
                instructionId = cIPSResponse.cipsTransactionDetailList.SingleOrDefault().instructionId,
                endToEndId = cIPSResponse.cipsTransactionDetailList.SingleOrDefault().endToEndId,
                amount = cIPSResponse.cipsTransactionDetailList.SingleOrDefault().amount,
                creditorAgent = cIPSResponse.cipsTransactionDetailList.SingleOrDefault().creditorAgent,
                creditorBranch = cIPSResponse.cipsTransactionDetailList.SingleOrDefault().creditorBranch,
                creditorName = cIPSResponse.cipsTransactionDetailList.SingleOrDefault().creditorName,
                creditorAccount = cIPSResponse.cipsTransactionDetailList.SingleOrDefault().creditorAccount,
                creditorIdType = cIPSResponse.cipsTransactionDetailList.SingleOrDefault().creditorIdType,
                creditorIdValue = cIPSResponse.cipsTransactionDetailList.SingleOrDefault().creditorIdValue,
                creditorAddress = cIPSResponse.cipsTransactionDetailList.SingleOrDefault().creditorAddress,
                creditorPhone = cIPSResponse.cipsTransactionDetailList.SingleOrDefault().creditorPhone,
                creditorMobile = cIPSResponse.cipsTransactionDetailList.SingleOrDefault().creditorMobile,
                creditorEmail = cIPSResponse.cipsTransactionDetailList.SingleOrDefault().creditorEmail,
                dateTime = dateTime,
                addenda1 = cIPSResponse.cipsTransactionDetailList.SingleOrDefault().addenda1,
                addenda2 = cIPSResponse.cipsTransactionDetailList.SingleOrDefault().addenda2,
                addenda3 = cIPSResponse.cipsTransactionDetailList.SingleOrDefault().addenda3,
                addenda4 = cIPSResponse.cipsTransactionDetailList.SingleOrDefault().addenda4,
                remark = cIPSResponse.cipsTransactionDetailList.SingleOrDefault().addenda4,
                thailiUserName = thailiUserName
                
            };
            return objresCIPSModel.CIPSRequestInfo(objresCIPSInfo);
        }

        public static int ConnectIPSRequestStatement(ConnectIPS cIPSResponse, string thailiUserName, DateTime dateTime)
        {
            var objresCIPSModel = new ConnectIPSUserModel();
            var objresCIPSInfo = new CIPSRequest
            {
                batchId = cIPSResponse.cipsBatchDetail.batchId,
                batchAmount = cIPSResponse.cipsBatchDetail.batchAmount,
                batchCount = cIPSResponse.cipsBatchDetail.batchCount,
                batchCrncy = cIPSResponse.cipsBatchDetail.batchCrncy,
                categoryPurpose = cIPSResponse.cipsBatchDetail.categoryPurpose,
                debtorAgent = cIPSResponse.cipsBatchDetail.debtorAgent,
                debtorBranch = cIPSResponse.cipsBatchDetail.debtorBranch,
                debtorName = cIPSResponse.cipsBatchDetail.debtorName,
                debtorAccount = cIPSResponse.cipsBatchDetail.debtorAccount,
                debtorIdType = cIPSResponse.cipsBatchDetail.debtorIdType,
                debtorIdValue = cIPSResponse.cipsBatchDetail.debtorIdValue,
                debtorAddress = cIPSResponse.cipsBatchDetail.debtorAddress,
                debtorPhone = cIPSResponse.cipsBatchDetail.debtorPhone,
                debtorMobile = cIPSResponse.cipsBatchDetail.debtorMobile,
                debtorEmail = cIPSResponse.cipsBatchDetail.debtorEmail,
                instructionId = cIPSResponse.cipsTransactionDetailList.SingleOrDefault().instructionId,
                endToEndId = cIPSResponse.cipsTransactionDetailList.SingleOrDefault().endToEndId,
                amount = cIPSResponse.cipsTransactionDetailList.SingleOrDefault().amount,
                creditorAgent = cIPSResponse.cipsTransactionDetailList.SingleOrDefault().creditorAgent,
                creditorBranch = cIPSResponse.cipsTransactionDetailList.SingleOrDefault().creditorBranch,
                creditorName = cIPSResponse.cipsTransactionDetailList.SingleOrDefault().creditorName,
                creditorAccount = cIPSResponse.cipsTransactionDetailList.SingleOrDefault().creditorAccount,
                creditorIdType = cIPSResponse.cipsTransactionDetailList.SingleOrDefault().creditorIdType,
                creditorIdValue = cIPSResponse.cipsTransactionDetailList.SingleOrDefault().creditorIdValue,
                creditorAddress = cIPSResponse.cipsTransactionDetailList.SingleOrDefault().creditorAddress,
                creditorPhone = cIPSResponse.cipsTransactionDetailList.SingleOrDefault().creditorPhone,
                creditorMobile = cIPSResponse.cipsTransactionDetailList.SingleOrDefault().creditorMobile,
                creditorEmail = cIPSResponse.cipsTransactionDetailList.SingleOrDefault().creditorEmail,
                dateTime = dateTime,
                addenda1 = cIPSResponse.cipsTransactionDetailList.SingleOrDefault().addenda1,
                addenda2 = cIPSResponse.cipsTransactionDetailList.SingleOrDefault().addenda2,
                addenda3 = cIPSResponse.cipsTransactionDetailList.SingleOrDefault().addenda3,
                addenda4 = cIPSResponse.cipsTransactionDetailList.SingleOrDefault().addenda4,
                thailiUserName = thailiUserName

            };
            return objresCIPSModel.CIPSRequestInfo(objresCIPSInfo);
        }

        public static string GetAccessToken(string accessToken)
        {
            var objresCIPSModel = new ConnectIPSUserModel();

            return objresCIPSModel.GetAccessToken(accessToken);
        }

        #region Check T-Pin
        public static DataTable CheckPin(string UserName)
        {
            var objUserModel = new LoginUserModels();
            var objUserInfo = new CheckPin
            {
                username = UserName
            };
            return objUserModel.CheckUserPin(objUserInfo);
        }
        #endregion
    }
}