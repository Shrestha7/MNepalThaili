using MNepalAPI.Connection;
using MNepalAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MNepalAPI.UserModel
{
    public class ConnectIPSUserModel
    {
        public int CIPSTokenInfo(ConnectIPSToken objresConnectIPSInfo)
        {
            SqlConnection sqlCon = null;
            int ret;
            try
            {
                using (sqlCon = new SqlConnection(DatabaseConnection.ConnectionString()))
                {
                    sqlCon.Open();
                    using (SqlCommand sqlCmd = new SqlCommand("[s_MNConnectIPS]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.AddWithValue("@access_token", objresConnectIPSInfo.access_token);
                        sqlCmd.Parameters.AddWithValue("@token_type", objresConnectIPSInfo.token_type);
                        sqlCmd.Parameters.AddWithValue("@refresh_token", objresConnectIPSInfo.refresh_token);
                        sqlCmd.Parameters.AddWithValue("@expires_in", objresConnectIPSInfo.expires_in);
                        sqlCmd.Parameters.AddWithValue("@scope", objresConnectIPSInfo.scope);
                        sqlCmd.Parameters.AddWithValue("@customerdetails", objresConnectIPSInfo.customer_details);
                        ret = sqlCmd.ExecuteNonQuery();
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

        public int ResponseConnectIPSInfo(CIPSResponse objresConnectIPSInfo)
        {
            SqlConnection sqlCon = null;
            int ret;
            try
            {
                using (sqlCon = new SqlConnection(DatabaseConnection.ConnectionString()))
                {
                    sqlCon.Open();
                    using (SqlCommand sqlCmd = new SqlCommand("[s_MNCIPSResponse]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.AddWithValue("@batchResponseCode", objresConnectIPSInfo.batchResponseCode);
                        sqlCmd.Parameters.AddWithValue("@batchResponseMessage", objresConnectIPSInfo.batchResponseMessage);
                        sqlCmd.Parameters.AddWithValue("@batchId", objresConnectIPSInfo.batchId);
                        sqlCmd.Parameters.AddWithValue("@debitStatus", objresConnectIPSInfo.debitStatus);
                        sqlCmd.Parameters.AddWithValue("@batchResponseId", objresConnectIPSInfo.batchResponseId);
                        sqlCmd.Parameters.AddWithValue("@txnResponseCode", objresConnectIPSInfo.txnResponseCode);
                        sqlCmd.Parameters.AddWithValue("@txnResponseMessage", objresConnectIPSInfo.txnResponseMessage);
                        sqlCmd.Parameters.AddWithValue("@txnId", objresConnectIPSInfo.txnId);
                        sqlCmd.Parameters.AddWithValue("@instructionId", objresConnectIPSInfo.instructionId);
                        sqlCmd.Parameters.AddWithValue("@creditStatus", objresConnectIPSInfo.creditStatus);
                        sqlCmd.Parameters.AddWithValue("@amount", objresConnectIPSInfo.amount);
                        sqlCmd.Parameters.AddWithValue("@dateTime", objresConnectIPSInfo.dateTime);
                        sqlCmd.Parameters.AddWithValue("@username", objresConnectIPSInfo.username);
                        sqlCmd.Parameters.AddWithValue("@thailiUserName", objresConnectIPSInfo.thailiUserName);
                        ret = sqlCmd.ExecuteNonQuery();
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

        public int ConnectIPSMNRequest(MNRequestResponse objresConnectIPSInfo)
        {
            SqlConnection sqlCon = null;
            int ret;
            try
            {
                using (sqlCon = new SqlConnection(DatabaseConnection.ConnectionString()))
                {
                    sqlCon.Open();
                    using (SqlCommand sqlCmd = new SqlCommand("[s_MNRequestCIPS]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.AddWithValue("@OriginID", objresConnectIPSInfo.originId);
                        sqlCmd.Parameters.AddWithValue("@OriginType", objresConnectIPSInfo.originType);
                        sqlCmd.Parameters.AddWithValue("@ServiceCode", objresConnectIPSInfo.serviceCode);
                        sqlCmd.Parameters.AddWithValue("@SourceBankCode", objresConnectIPSInfo.sourceBankCode);
                        sqlCmd.Parameters.AddWithValue("@SourceBranchCode", objresConnectIPSInfo.sourceBranchCode);
                        sqlCmd.Parameters.AddWithValue("@SourceAccountNo", objresConnectIPSInfo.sourceAccountNumber);
                        sqlCmd.Parameters.AddWithValue("@DestBankCode", objresConnectIPSInfo.destBankCode);
                        sqlCmd.Parameters.AddWithValue("@DestBranchCode", objresConnectIPSInfo.destBranchCode);
                        sqlCmd.Parameters.AddWithValue("@DestAccountNo", objresConnectIPSInfo.destAccountNumber);
                        sqlCmd.Parameters.AddWithValue("@Amount", objresConnectIPSInfo.amount);
                        sqlCmd.Parameters.AddWithValue("@FeeId", objresConnectIPSInfo.feeId);
                        sqlCmd.Parameters.AddWithValue("@TraceNo", objresConnectIPSInfo.traceNo);
                        sqlCmd.Parameters.AddWithValue("@TranDate", objresConnectIPSInfo.tranDate);
                        sqlCmd.Parameters.AddWithValue("@RetrievalRef", objresConnectIPSInfo.retrievalReference);
                        sqlCmd.Parameters.AddWithValue("@Desc1", objresConnectIPSInfo.desc1);
                        sqlCmd.Parameters.AddWithValue("@Desc2", objresConnectIPSInfo.desc2);
                        sqlCmd.Parameters.AddWithValue("@Desc3", objresConnectIPSInfo.desc3);
                        sqlCmd.Parameters.AddWithValue("@ReversalStatus", objresConnectIPSInfo.reversalStatus);
                        sqlCmd.Parameters.AddWithValue("@OTraceNo", objresConnectIPSInfo.oTraceNo);
                        sqlCmd.Parameters.AddWithValue("@OTranDateTime", objresConnectIPSInfo.oTranDateTime);
                        sqlCmd.Parameters.AddWithValue("@IsProcessed", objresConnectIPSInfo.isProcessed);
                        sqlCmd.Parameters.AddWithValue("@Status", objresConnectIPSInfo.status);
                        sqlCmd.Parameters.AddWithValue("@FromSMS", objresConnectIPSInfo.fromSMS);
                        sqlCmd.Parameters.AddWithValue("@Remark", objresConnectIPSInfo.remark);
                        sqlCmd.Parameters.AddWithValue("@SMSAlertType", objresConnectIPSInfo.smsAlertType);
                        sqlCmd.Parameters.AddWithValue("@EnteredAt", objresConnectIPSInfo.enteredAt);
                        sqlCmd.Parameters.AddWithValue("@MerchantId", objresConnectIPSInfo.merchantId);
                        sqlCmd.Parameters.AddWithValue("@Uid", objresConnectIPSInfo.uId);
                        ret = sqlCmd.ExecuteNonQuery();
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

        public int ConnectIPSMNResponse(MNRequestResponse objresConnectIPSInfo)
        {
            SqlConnection sqlCon = null;
            int ret;
            try
            {
                using (sqlCon = new SqlConnection(DatabaseConnection.ConnectionString()))
                {
                    sqlCon.Open();
                    using (SqlCommand sqlCmd = new SqlCommand("[s_MNResponseCIPS]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.AddWithValue("@OriginID", objresConnectIPSInfo.originId);
                        sqlCmd.Parameters.AddWithValue("@OriginType", objresConnectIPSInfo.originType);
                        sqlCmd.Parameters.AddWithValue("@ServiceCode", objresConnectIPSInfo.serviceCode);
                        sqlCmd.Parameters.AddWithValue("@SourceBankCode", objresConnectIPSInfo.sourceBankCode);
                        sqlCmd.Parameters.AddWithValue("@SourceBranchCode", objresConnectIPSInfo.sourceBranchCode);
                        sqlCmd.Parameters.AddWithValue("@SourceAccountNo", objresConnectIPSInfo.sourceAccountNumber);
                        sqlCmd.Parameters.AddWithValue("@DestBankCode", objresConnectIPSInfo.destBankCode);
                        sqlCmd.Parameters.AddWithValue("@DestBranchCode", objresConnectIPSInfo.destBranchCode);
                        sqlCmd.Parameters.AddWithValue("@DestAccountNo", objresConnectIPSInfo.destAccountNumber);
                        sqlCmd.Parameters.AddWithValue("@Amount", objresConnectIPSInfo.amount);
                        sqlCmd.Parameters.AddWithValue("@FeeId", objresConnectIPSInfo.feeId);
                        sqlCmd.Parameters.AddWithValue("@FeeAmount", objresConnectIPSInfo.feeAmount);
                        sqlCmd.Parameters.AddWithValue("@TraceNo", objresConnectIPSInfo.traceNo);
                        sqlCmd.Parameters.AddWithValue("@TranDate", objresConnectIPSInfo.tranDate);
                        sqlCmd.Parameters.AddWithValue("@TranTime", objresConnectIPSInfo.tranTime);
                        sqlCmd.Parameters.AddWithValue("@RetrievalRef", objresConnectIPSInfo.retrievalReference);
                        sqlCmd.Parameters.AddWithValue("@ResponseCode", objresConnectIPSInfo.responseCode);
                        sqlCmd.Parameters.AddWithValue("@ResponseDescription", objresConnectIPSInfo.responseDescription);
                        sqlCmd.Parameters.AddWithValue("@Balance", objresConnectIPSInfo.balance);
                        sqlCmd.Parameters.AddWithValue("@AccountHolderName", objresConnectIPSInfo.accountHolderName);
                        sqlCmd.Parameters.AddWithValue("@MiniStmtRecord", objresConnectIPSInfo.miniStmtRecord);
                        sqlCmd.Parameters.AddWithValue("@ReversalStatus", objresConnectIPSInfo.reversalStatus);
                        sqlCmd.Parameters.AddWithValue("@TranId", objresConnectIPSInfo.tranId);
                        sqlCmd.Parameters.AddWithValue("@DestUsername", objresConnectIPSInfo.destUsername);
                       
                        ret = sqlCmd.ExecuteNonQuery();
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

        public string GetAccessToken(string accessToken)
        {
            SqlDataReader rdr;
            SqlConnection conn = null;
            string refreshToken = "";
            try
            {
                using (conn = new SqlConnection(DatabaseConnection.ConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {

                        cmd.CommandText = "select top 1 RefreshToken from MNConnectIPS where AccessToken="+ "'" + accessToken + "'" ;
                        cmd.Connection = conn;
                        if (conn.State != ConnectionState.Open)
                        {
                            conn.Open();
                        }
                        rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {

                            refreshToken = rdr["RefreshToken"].ToString();
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
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }

            }
            return refreshToken;
        }

        public int CIPSRequestInfo(CIPSRequest objresConnectIPSInfo)
        {
            SqlConnection sqlCon = null;
            int ret;
            try
            {
                using (sqlCon = new SqlConnection(DatabaseConnection.ConnectionString()))
                {
                    sqlCon.Open();
                    using (SqlCommand sqlCmd = new SqlCommand("[s_MNCIPSRequest]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.AddWithValue("@batchId", objresConnectIPSInfo.batchId);
                        sqlCmd.Parameters.AddWithValue("@batchAmount", objresConnectIPSInfo.batchAmount);
                        sqlCmd.Parameters.AddWithValue("@batchCount", objresConnectIPSInfo.batchCount);
                        sqlCmd.Parameters.AddWithValue("@batchCrncy", objresConnectIPSInfo.batchCrncy);
                        sqlCmd.Parameters.AddWithValue("@categoryPurpose", objresConnectIPSInfo.categoryPurpose);
                        sqlCmd.Parameters.AddWithValue("@debtorAgent", objresConnectIPSInfo.debtorAgent);
                        sqlCmd.Parameters.AddWithValue("@debtorBranch", objresConnectIPSInfo.debtorBranch);
                        sqlCmd.Parameters.AddWithValue("@debtorName", objresConnectIPSInfo.debtorName);
                        sqlCmd.Parameters.AddWithValue("@debtorAccount", objresConnectIPSInfo.debtorAccount);
                        sqlCmd.Parameters.AddWithValue("@debtorIdType", objresConnectIPSInfo.debtorIdType);
                        sqlCmd.Parameters.AddWithValue("@debtorIdValue", objresConnectIPSInfo.debtorIdValue);
                        sqlCmd.Parameters.AddWithValue("@debtorAddress", objresConnectIPSInfo.debtorAccount);
                        sqlCmd.Parameters.AddWithValue("@debtorPhone", objresConnectIPSInfo.debtorPhone);
                        sqlCmd.Parameters.AddWithValue("@debtorMobile", objresConnectIPSInfo.debtorMobile);
                        sqlCmd.Parameters.AddWithValue("@debtorEmail", objresConnectIPSInfo.debtorEmail);
                        sqlCmd.Parameters.AddWithValue("@instructionId", objresConnectIPSInfo.instructionId);
                        sqlCmd.Parameters.AddWithValue("@endToEndId", objresConnectIPSInfo.endToEndId);
                        sqlCmd.Parameters.AddWithValue("@amount", objresConnectIPSInfo.amount);
                        sqlCmd.Parameters.AddWithValue("@creditorAgent", objresConnectIPSInfo.creditorAgent);
                        sqlCmd.Parameters.AddWithValue("@creditorBranch", objresConnectIPSInfo.creditorBranch);
                        sqlCmd.Parameters.AddWithValue("@creditorName", objresConnectIPSInfo.creditorName);
                        sqlCmd.Parameters.AddWithValue("@creditorAccount", objresConnectIPSInfo.creditorAccount);
                        sqlCmd.Parameters.AddWithValue("@creditorIdType", objresConnectIPSInfo.creditorIdType);
                        sqlCmd.Parameters.AddWithValue("@creditorIdValue", objresConnectIPSInfo.creditorIdValue);
                        sqlCmd.Parameters.AddWithValue("@creditorAddress", objresConnectIPSInfo.creditorAddress);
                        sqlCmd.Parameters.AddWithValue("@creditorPhone", objresConnectIPSInfo.creditorPhone);
                        sqlCmd.Parameters.AddWithValue("@creditorMobile", objresConnectIPSInfo.creditorMobile);
                        sqlCmd.Parameters.AddWithValue("@creditorEmail", objresConnectIPSInfo.creditorEmail);
                        sqlCmd.Parameters.AddWithValue("@dateTime", objresConnectIPSInfo.dateTime);
                        sqlCmd.Parameters.AddWithValue("@addenda1", objresConnectIPSInfo.addenda1);
                        sqlCmd.Parameters.AddWithValue("@addenda2", objresConnectIPSInfo.addenda2);
                        sqlCmd.Parameters.AddWithValue("@addenda3", objresConnectIPSInfo.addenda3);
                        sqlCmd.Parameters.AddWithValue("@addenda4", objresConnectIPSInfo.addenda4);
                        sqlCmd.Parameters.AddWithValue("@thailiUserName", objresConnectIPSInfo.thailiUserName);
                       
                        ret = sqlCmd.ExecuteNonQuery();
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
    }
}