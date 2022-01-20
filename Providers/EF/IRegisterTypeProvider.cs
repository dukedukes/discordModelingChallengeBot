using ModelChallengeBot.EF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelChallengeBot.Providers.EF
{
    public interface IRegisterTypeProvider<T, TEnum>
        where T : RegistryBase, new()
        where TEnum : struct, IConvertible
    {
        Task<ulong?> GetRegisterByType(TEnum registerType);
        Task<List<T>> GetRegistrations();
        Task AddOrOverwriteRegister(ulong channelId, TEnum registerType);
    }
}
