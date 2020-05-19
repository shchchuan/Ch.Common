namespace Ch.Utility.WxPay
{
    public class WxPrePayData
    {
        public string AppId { get; set; }
        public string NonceStr { get; set; }
        public string Package { get; set; }
        public string PaySign { get; set; }
        public string SignType { get; set; }
        public string TimeStamp { get; set; }
    }
}
