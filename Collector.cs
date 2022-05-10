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


    public interface IReportCollector
    {
        void Initialize();
        void CollectData(DateTime startDt, DateTime endDt);
        List<exam> CollectData(Queue<string> Accessions);
        bool Authenticate(string user, string pwd);
        void Run(int defaultPastXdays = -1);
    }
    public class exam
    {
       
        public string Org { get; set; }
        public string Mrn { get; set; }
        public string Acc { get; set; }
        public DateTime? ExamDt { get; set; }
        public string InternalId { get; set; }

        public string InternalPatientId { get; set; }


        public List<Report> Reports { get; set; }

        public string Error { get; set; }
        public string Modality { get; set; }
        public string ExamCode { get;  set; }
        public string ExamDesc { get;  set; }
        public string PerformingResource { get; set; }
        public string Subspecialty { get; set; }
        public string BodyPart { get; set; }
        public bool ExamRead { get; set; }
        public string ExamPriority { get; set; }

        public string Name { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PatientLocation { get; set; }

        public string ReportStats { get; set; }
        public string RefferingMd { get; set; }

    }
    public class Report
    {
        
        public string Text { get; set; }
       
        public string Status { get; set; }
       
        public string Current { get; set; }
       
        public DateTime ReportDt { get; set; }

       
        public string PrincipalInterpreter { get; set; }
    }


    public class LastRunDetails
    {
        public string lastRunDt;
        public string startRangeDt;
        public string endRangeDt;
        public int countOfResults;
        public List<string> accessions;



    }

}
