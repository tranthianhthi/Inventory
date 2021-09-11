namespace OnlineInventoryLib.Lazada.Responses
{
    public class ErrorResponse
    {
        public string code { get; set; }
        public string type { get; set; }
        public string message { get; set; }
        public string request_id { get; set; }

        public override string ToString()
        {
            string error = "ErrorCode: {0} - Type: {1} - Message: {2}";
            return string.Format(error, code, type, message);
        }
    }
}
