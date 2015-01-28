using System.Collections.Generic;

namespace Ista.Miramar.Interfaces
{
    public interface IMiramarMetaDataTaskFactory
    {
        bool AllowsMetaDataConfiguration(string taskId);
        IMiramarTask GetTask(IMiramarClientInfo clientInfo, string taskId, KeyValuePair<string, string>[] metaData);
    }
}