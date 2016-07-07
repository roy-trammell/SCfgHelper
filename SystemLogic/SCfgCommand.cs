using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.Threading;

using SystemLogic.Interfaces;

namespace SystemLogic
{
    public enum ConfigOperation
    {
        Create,
        Delete
    }

    public class SCfgCommand : ISCfgCommand
    {
        private ConfigOperation m_configType;
        private string m_serviceName;
        private Dictionary<string, string> m_arguments;

        private Process m_serviceCreationDeletionProcess;
        private Process m_serviceStartStopProcess;

        public ConfigOperation ConfigOperationType
        {
            get { return m_configType; }
            set { m_configType = value; }
        }

        public string ServiceName
        {
            get { return m_serviceName; }
            set { m_serviceName = value; }
        }

        public Dictionary<string, string> Arguments
        {
            get { return m_arguments ?? (m_arguments = new Dictionary<string, string>()); }
            set { m_arguments = value; }
        }

        public void Create() { }
        
        public void ChangeState()
        {
            if (m_serviceStartStopProcess != null)
            {
                if (m_serviceStartStopProcess.StartInfo != null)
                {
                    Thread.Sleep(1000);
                    m_serviceStartStopProcess.Start();
                }
            }
        }

        public void Delete() { }

        private void SetupProcessServiceStartStop(string action)
        {
            m_serviceStartStopProcess = new Process();
            m_serviceStartStopProcess.StartInfo = new ProcessStartInfo("sc.exe", string.Format("{0} \"{1}\"", action, this.ServiceName));
            m_serviceStartStopProcess.StartInfo.UseShellExecute = false;         
        }

        public void ExecCommand()
        {
            Action<string> delegateStartStopService = SetupProcessServiceStartStop;

            try
            {
                m_serviceCreationDeletionProcess = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "sc.exe";

                switch (m_configType)
                {
                    case ConfigOperation.Create:
                        Arguments.Add("start", "demand");
                        Arguments.Add("depend", "MSMQ");

                        delegateStartStopService("start");
                        break;

                    case ConfigOperation.Delete:
                        delegateStartStopService("stop");
                        break;

                    default:
                        break;
                }

                startInfo.Arguments = BuildArgumentsString(m_configType);
                m_serviceCreationDeletionProcess.StartInfo = startInfo;

                if (ConfigOperation.Delete == ConfigOperationType)
                    ChangeState();

                m_serviceCreationDeletionProcess.Start();

                if (ConfigOperation.Create == ConfigOperationType)
                    ChangeState();
            }
            catch(Exception ex)
            {
                using (var eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Service Configurator";
                    eventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
                }

                throw ex;
            }
        }

        public SCfgCommand(ConfigOperation operation)
        {
            ConfigOperationType = operation;
        }

        public string BuildArgumentsString(ConfigOperation cfgType)
        {
            var sb = new StringBuilder();

            switch(cfgType)
            {
                case ConfigOperation.Create:
                    sb.Append("create ");
                    sb.Append(string.Format("\"{0}\" ", this.ServiceName));
                    break;

                case ConfigOperation.Delete:
                    sb.Append("delete ");
                    sb.Append(string.Format("\"{0}\" ", this.ServiceName));
                    break;

                default:
                    break;
            }

            Arguments.ToList<KeyValuePair<string, string>>()
                .ForEach(kvp => sb.Append(string.Format("{0}=\"{1}\" ", kvp.Key, kvp.Value)));

            return sb.ToString().Trim();
        }

        public string AssembleArgumentsString(ConfigOperation cfgType)
        {
            var sb = new StringBuilder();

            string outStartValue = string.Empty;
            string outDisplayNameValue = string.Empty;
            string outBinPathValue = string.Empty;
            string outDependValue = string.Empty;
            string outObjValue = string.Empty;
            string outPasswordValue = string.Empty;

            switch (cfgType)
            {
                case ConfigOperation.Create:
                    sb.Append("create ");
                    sb.Append(string.Format("\"{0}\" ", this.ServiceName));

                    Arguments.TryGetValue("start", out outStartValue);
                    sb.Append(string.Format("start={0} ", outStartValue));

                    Arguments.TryGetValue("DisplayName", out outDisplayNameValue);
                    sb.Append(string.Format("DisplayName={0} ", outDisplayNameValue));

                    Arguments.TryGetValue("binPath", out outBinPathValue);
                    sb.Append(string.Format("binPath={0} ", outBinPathValue));

                    Arguments.TryGetValue("depend", out outDependValue);
                    sb.Append(string.Format("depend={0} ", outDependValue));

                    Arguments.TryGetValue("obj", out outObjValue);
                    sb.Append(string.Format("obj={0} ", outObjValue));

                    Arguments.TryGetValue("password", out outPasswordValue);
                    sb.Append(string.Format("password={0}", outPasswordValue));

                    break;

                case ConfigOperation.Delete:
                    sb.Append("delete ");                    
                    sb.Append(string.Format("\"{0}\" ", this.ServiceName));
                    break;

                default:
                    break;
            }

            return sb.ToString();
        }
    }
}
