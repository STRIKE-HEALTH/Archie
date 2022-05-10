using ISITELib;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Archie
{


    public class BaseReportCollector : IReportCollector
    {
       

        public virtual void Initialize()
        {
         
        }
        public void CollectData(DateTime startDt, DateTime endDt)
        {
            throw new NotImplementedException();
        }
        public virtual List<string> GetNewOrdersinDateRange(DateTime startDt, DateTime endDt)
        {
            return null;
        }

        public bool Authenticate(string user, string pwd)
        {
            throw new NotImplementedException();
        }

        public void Run(int defaultPastXdays = -1)
        {
            throw new NotImplementedException();
        }

        public void Initialize(IConfiguration configuration)
        {
            throw new NotImplementedException();
        }

      
        List<exam> IReportCollector.CollectData(Queue<string> Accessions)
        {
            throw new NotImplementedException();
        }
    }

}
