namespace PokemonAPI.Models.Response
{

    public class ResponseMessageData<Tdata>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Tdata? Data { get; set; }

        public ResponseMessageData() { }
        public ResponseMessageData(bool Success, string Message)
        {
            this.Success = Success;
            this.Message = Message;
        }

    }
    public class ResponseMessage
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public ResponseMessage() { }
        public ResponseMessage(bool Success, string Message)
        {
            this.Success = Success;
            this.Message = Message;
        }

    }
}
