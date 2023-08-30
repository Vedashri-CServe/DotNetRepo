using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Entity;

namespace TMS.Helper
{
    public interface IConfigurationService
    {
        public ConfigData configData { get; set; }
        public Task<int> SetConfigData(string username);
    }
}
