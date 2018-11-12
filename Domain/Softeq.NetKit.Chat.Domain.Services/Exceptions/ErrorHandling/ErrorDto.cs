// // Developed by Softeq Development Corporation
// // http://www.softeq.com

using Newtonsoft.Json;

namespace Softeq.NetKit.Chat.Domain.Services.Exceptions.ErrorHandling
{
    public class ErrorDto
    {
        public ErrorDto()
        {
        }

        public ErrorDto(string code, string description)
        {
            Code = code;
            Description = description;
        }

        public string Code { get; set; }
        public string Description { get; set; }

        // other fields

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}