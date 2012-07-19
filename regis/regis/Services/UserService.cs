using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace Regis.Services
{
    [Export (typeof(IUserService))]
    public class UserService: IUserService
    {


    }
}
