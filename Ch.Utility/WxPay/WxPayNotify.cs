using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;

namespace Ch.Utility.WxPay
{
    public class WxPayNotifyResponse
    {
        public bool Result { get; set; }
        public string OrderId { get; set; }
        public int TotalFee { get; set; }
    }
    public class WxPayNotify
    {
        /// <summary>
        /// 订单支付回调
        /// </summary>
        public static WxPayNotifyResponse PayNotify(string PostData)
        {
            WxPayNotifyResponse noticeResponse = new WxPayNotifyResponse();
            string xmlData = PostData;//获取请求数据
            if (xmlData == "")
            {

            }
            else
            {
                var dic = new Dictionary<string, string>
                {
                    {"return_code", "SUCCESS"},
                    {"return_msg","OK"}
                };
                var sb = new StringBuilder();
                sb.Append("<xml>");
                foreach (var d in dic)
                {
                    sb.Append("<" + d.Key + ">" + d.Value + "</" + d.Key + ">");
                }
                sb.Append("</xml>");
                //把数据重新返回给客户端
                DataSet ds = new DataSet();
                StringReader stram = new StringReader(xmlData);
                XmlTextReader datareader = new XmlTextReader(stram);
                ds.ReadXml(datareader);
                Log.WriteLog("PayNotice", JsonConvert.SerializeObject(ds.Tables[0]));
                if (ds.Tables[0].Rows[0]["return_code"].ToString() == "SUCCESS")
                {
                    string wx_appid = "";//微信开放平台审核通过的应用APPID
                    string wx_mch_id = "";//微信支付分配的商户号
                    string wx_nonce_str = "";// 	随机字符串，不长于32位
                    string wx_sign = "";//签名，详见签名算法
                    string wx_result_code = "";//SUCCESS/FAIL
                    string wx_return_code = "";
                    string wx_openid = "";//用户在商户appid下的唯一标识
                    string wx_is_subscribe = "";//用户是否关注公众账号，Y-关注，N-未关注，仅在公众账号类型支付有效
                    string wx_trade_type = "";// 	APP
                    string wx_bank_type = "";// 	银行类型，采用字符串类型的银行标识，银行类型见银行列表
                    string wx_fee_type = "";// 	货币类型，符合ISO4217标准的三位字母代码，默认人民币：CNY，其他值列表详见货币类型


                    string wx_transaction_id = "";//微信支付订单号
                    string wx_out_trade_no = "";//商户系统的订单号，与请求一致。
                    string wx_time_end = "";// 	支付完成时间，格式为yyyyMMddHHmmss，如2009年12月25日9点10分10秒表示为20091225091010。其他详见时间规则
                    int wx_total_fee = -1;// 	订单总金额，单位为分
                    int wx_cash_fee = -1;//现金支付金额订单现金支付金额，详见支付金额
                    #region  数据解析
                    //列 是否存在
                    string signstr = "";//需要前面的字符串
                    //wx_appid
                    if (ds.Tables[0].Columns.Contains("appid"))
                    {
                        wx_appid = ds.Tables[0].Rows[0]["appid"].ToString();
                        if (!string.IsNullOrEmpty(wx_appid))
                        {
                            signstr += "appid=" + wx_appid;
                        }
                    }
                    //wx_bank_type
                    if (ds.Tables[0].Columns.Contains("bank_type"))
                    {
                        wx_bank_type = ds.Tables[0].Rows[0]["bank_type"].ToString();
                        if (!string.IsNullOrEmpty(wx_bank_type))
                        {
                            signstr += "&bank_type=" + wx_bank_type;
                        }
                    }
                    //wx_cash_fee
                    if (ds.Tables[0].Columns.Contains("cash_fee"))
                    {
                        wx_cash_fee = Convert.ToInt32(ds.Tables[0].Rows[0]["cash_fee"].ToString());

                        signstr += "&cash_fee=" + wx_cash_fee;
                    }
                    //wx_fee_type
                    if (ds.Tables[0].Columns.Contains("fee_type"))
                    {
                        wx_fee_type = ds.Tables[0].Rows[0]["fee_type"].ToString();
                        if (!string.IsNullOrEmpty(wx_fee_type))
                        {
                            signstr += "&fee_type=" + wx_fee_type;
                        }
                    }
                    //wx_is_subscribe
                    if (ds.Tables[0].Columns.Contains("is_subscribe"))
                    {
                        wx_is_subscribe = ds.Tables[0].Rows[0]["is_subscribe"].ToString();
                        if (!string.IsNullOrEmpty(wx_is_subscribe))
                        {
                            signstr += "&is_subscribe=" + wx_is_subscribe;
                        }
                    }
                    //wx_mch_id
                    if (ds.Tables[0].Columns.Contains("mch_id"))
                    {
                        wx_mch_id = ds.Tables[0].Rows[0]["mch_id"].ToString();
                        if (!string.IsNullOrEmpty(wx_mch_id))
                        {
                            signstr += "&mch_id=" + wx_mch_id;
                        }
                    }
                    //wx_nonce_str
                    if (ds.Tables[0].Columns.Contains("nonce_str"))
                    {
                        wx_nonce_str = ds.Tables[0].Rows[0]["nonce_str"].ToString();
                        if (!string.IsNullOrEmpty(wx_nonce_str))
                        {
                            signstr += "&nonce_str=" + wx_nonce_str;
                        }
                    }
                    //wx_openid
                    if (ds.Tables[0].Columns.Contains("openid"))
                    {
                        wx_openid = ds.Tables[0].Rows[0]["openid"].ToString();
                        if (!string.IsNullOrEmpty(wx_openid))
                        {
                            signstr += "&openid=" + wx_openid;
                        }
                    }
                    //wx_out_trade_no
                    if (ds.Tables[0].Columns.Contains("out_trade_no"))
                    {
                        wx_out_trade_no = ds.Tables[0].Rows[0]["out_trade_no"].ToString();
                        if (!string.IsNullOrEmpty(wx_out_trade_no))
                        {
                            signstr += "&out_trade_no=" + wx_out_trade_no;
                            noticeResponse.OrderId = wx_out_trade_no;
                        }
                    }
                    //wx_result_code 
                    if (ds.Tables[0].Columns.Contains("result_code"))
                    {
                        wx_result_code = ds.Tables[0].Rows[0]["result_code"].ToString();
                        if (!string.IsNullOrEmpty(wx_result_code))
                        {
                            signstr += "&result_code=" + wx_result_code;
                        }
                    }
                    //wx_result_code 
                    if (ds.Tables[0].Columns.Contains("return_code"))
                    {
                        wx_return_code = ds.Tables[0].Rows[0]["return_code"].ToString();
                        if (!string.IsNullOrEmpty(wx_return_code))
                        {
                            signstr += "&return_code=" + wx_return_code;
                        }
                    }

                    //wx_sign 
                    if (ds.Tables[0].Columns.Contains("sign"))
                    {
                        wx_sign = ds.Tables[0].Rows[0]["sign"].ToString();
                        //if (!string.IsNullOrEmpty(wx_sign))
                        //{
                        //    signstr += "&sign=" + wx_sign;
                        //}
                    }

                    //wx_time_end
                    if (ds.Tables[0].Columns.Contains("time_end"))
                    {
                        wx_time_end = ds.Tables[0].Rows[0]["time_end"].ToString();
                        if (!string.IsNullOrEmpty(wx_time_end))
                        {
                            signstr += "&time_end=" + wx_time_end;
                        }
                    }

                    //wx_total_fee
                    if (ds.Tables[0].Columns.Contains("total_fee"))
                    {
                        wx_total_fee = Convert.ToInt32(ds.Tables[0].Rows[0]["total_fee"].ToString());

                        signstr += "&total_fee=" + wx_total_fee;
                    }

                    //wx_trade_type
                    if (ds.Tables[0].Columns.Contains("trade_type"))
                    {
                        wx_trade_type = ds.Tables[0].Rows[0]["trade_type"].ToString();
                        if (!string.IsNullOrEmpty(wx_trade_type))
                        {
                            signstr += "&trade_type=" + wx_trade_type;
                        }
                    }

                    //wx_transaction_id
                    if (ds.Tables[0].Columns.Contains("transaction_id"))
                    {
                        wx_transaction_id = ds.Tables[0].Rows[0]["transaction_id"].ToString();
                        if (!string.IsNullOrEmpty(wx_transaction_id))
                        {
                            signstr += "&transaction_id=" + wx_transaction_id;
                        }
                    }
                    #endregion

                    //追加key 密钥
                    signstr += "&key=" + WxConfig.WxPayKey;
                    //签名正确
                    string orderStrwhere = "ordernumber='" + wx_out_trade_no + "'";
                    Log.WriteLog("PayNotice", "sign:" + wx_sign + "," + signstr);
                    if (wx_appid == WxConfig.AppId && wx_mch_id == WxConfig.MchId)
                    {
                        //签名正确   处理订单操作逻辑
                        noticeResponse.Result = true;
                        noticeResponse.TotalFee = Convert.ToInt32(ds.Tables[0].Rows[0]["total_fee"].ToString());
                    }
                    else
                    {
                        //追加备注信息

                    }
                }
                else
                {
                    // 返回信息，如非空，为错误原因  签名失败 参数格式校验错误
                    string return_msg = ds.Tables[0].Rows[0]["return_msg"].ToString();
                }
                string return_result = sb.ToString();
            }
            return noticeResponse;
        }
    }
}
