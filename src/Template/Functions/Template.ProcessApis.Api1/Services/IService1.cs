using Template.Domain;
using Template.ProcessApis.Api1.Domain;

namespace Template.ProcessApis.Api1.Services
{
    public interface IService1
    {
        Result<Command> Get(int id);
        Result Save(Command command);
    }
}
