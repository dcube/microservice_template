using Template.Domain;
using Template.ProcessApis.Api1.Domain;

namespace Template.ProcessApis.Api1.Services
{
    public class Service1 : IService1
    {
        public Result<Command> Get(int id)
        {
            return new Result<Command>()
            {
                IsSuccess = true,
                Value = new Command()
                {
                    Id = id,
                    Description = "Blabla"
                }
            };
        }

        public Result Save(Command command)
        {
            return new Result() { IsSuccess = true};
        }
    }
}
