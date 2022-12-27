using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDLight.Services
{
    internal static class ServicesResolver
    {
        public static IServiceProvider Services { get; internal set; }
    }
}
