using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemLogic.Interfaces
{
    public interface ISCfgCommand
    {
        ConfigOperation ConfigOperationType { get; set; }
        string ServiceName { get; set; }
        Dictionary<string, string> Arguments { get; set; }
        void Create();
        void ChangeState();
        void Delete();
        void ExecCommand();
    }
}
