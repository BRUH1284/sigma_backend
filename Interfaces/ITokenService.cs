using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sigma_backend.Models;

namespace sigma_backend.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user, IList<string> role);
    }
}