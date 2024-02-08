namespace CommandSystem.Net.Protocol.Models
{
    public class RemoteCommandResponse
    {
        public bool Ok { get; set; }
        public string ErrorMessage { get; set; }

        public int JobId { get; set; }
    }
}
