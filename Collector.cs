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
    }


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
    }
    public class iSiteReportCollector: IReportCollector
    {
        private iSiteNonVisualClass _myiSite;

        IConfiguration configuration;
        ILogger logger;


        bool _collectingData = false;
        bool _disengage = false;
        public iSiteReportCollector(ILogger<iSiteReportCollector> log, IConfiguration config, iSiteNonVisualClass iSite)
        {
            logger = log;
            configuration = config;
            _myiSite = iSite;
            Initialize();
        }
        public  void Initialize()
        {
            //logger = Globals.LoggerFactory.CreateLogger<iSiteReportCollector>();
            //configuration = Globals.Configuration;
            try
            {
                string username = configuration["Collector:From:Authentication:UserName"];
                string password = configuration["Collector:From:Authentication:Password"];
                string server = configuration["Collector:From:Authentication:EndPoint"];

                var imageSuiteUrl = string.Format(configuration["Collector:From:iSite:ImageSuiteUrlFormat"], server);
                var imageSuiteDsn = configuration["Collector:From:iSite:ImageSuiteDSN"];
                var iSyntaxServer = configuration["Collector:From:iSite:iSyntaxServerIP"];
                var iSyntaxServerPort = configuration["Collector:From:iSite:iSyntaxServerPort"];
                var iSiteOptions = configuration["Collector:From:iSite:iSiteOptions"];


                logger.LogTrace("Initializing iSite");

                if (_myiSite != null)
                {
                    try
                    {
                        logger.LogTrace("Logout existing iSite");
                        _myiSite.Logout();
                        Marshal.ReleaseComObject(_myiSite);
                    }
                    catch (Exception)
                    {
                    }
                    finally
                    {
                        _myiSite = null;
                    }
                }

                logger.LogInformation(String.Format("Creating isite nonVisual with ImageSuiteURL:{0}, ImageSuiteDSN:{1}, iSyntaxServerIP:{2}, iSyntaxServerPort:{3}, options:{4}", imageSuiteUrl, imageSuiteDsn,iSyntaxServer, iSyntaxServerPort, iSiteOptions));
                //_myiSite = new iSiteNonVisualClass
                //               {
                //                   ImageSuiteURL = imageSuiteUrl,
                //                   ImageSuiteDSN = Settings.Default.ImageSuiteDSN,
                //                   iSyntaxServerIP = (string.IsNullOrEmpty(Settings.Default.iSyntaxServerIP)) ? server : Settings.Default.iSyntaxServerIP,
                //                   iSyntaxServerPort = Settings.Default.iSyntaxServerPort,
                //                   Options = Settings.Default.iSiteOptions
                //               };


                #region create isite object
                try
                {
                    logger.LogTrace("Creating iSite");
                    try
                    {
                        _myiSite = new iSiteNonVisualClass();
                    }
                    catch (Exception)
                    {
                        _myiSite = null;
                    }
                    _myiSite.ImageSuiteURL = imageSuiteUrl;
                    _myiSite.ImageSuiteDSN = imageSuiteDsn; // "iSite";
                    _myiSite.iSyntaxServerIP = iSyntaxServer ?? server;
                    _myiSite.iSyntaxServerPort = iSyntaxServerPort;
                    _myiSite.Options = iSiteOptions;

                    logger.LogTrace("Initializing iSite");
                    bool ret = _myiSite.Initialize();
                    int err = 0;
                    if (!ret)
                    {
                        logger.LogDebug(string.Format("Initialization Failed"));
                        err = _myiSite.GetLastErrorCode();
                        logger.LogDebug(string.Format("Error Code - {0}", err.ToString()));
                    }
                }
                catch (Exception e)
                {
                    logger.LogDebug("ISite Initialialize failed in Exception ", e);
                    throw;
                }
                logger.LogTrace($"iSite Initialized:{_myiSite.Initialized}");
                logger.LogTrace(String.Format("2.Logging in With {0}", username));
                #endregion
                #region login
                if (_myiSite.Login(username, password, "iSite", string.Empty, string.Empty))
                {
                    logger.LogTrace(String.Format("2.Logged in With {0}", username));
                }
                else
                    logger.LogInformation("Could not log in to iSite");

                logger.LogDebug("ErrorCode from iSite:" + _myiSite.GetLastErrorCode());
                #endregion


            }
            catch (Exception e)
            {
                logger.LogDebug("Error Initializing iSite Object", e);
                throw e;
            }
        }
        private static string GetUtf8Value(string value)
        {

            Encoding iso = Encoding.GetEncoding("Windows-1252");
            Encoding utf8 = Encoding.UTF8;
            byte[] isoBytes = iso.GetBytes(value);
            return utf8.GetString(isoBytes);
        }
        public  List<exam> GetOrdersinDateRange(DateTime start, DateTime end)
        {
            logger.LogDebug("ErrorCode from iSite:" + _myiSite.GetLastErrorCode());


            while (_myiSite.GetLastErrorCode() != 0)
            {
                Initialize(); //re-initailize
                Thread.Sleep(300);
            }

            DateTime _end = end;
            DateTime _start = start;

            String queryXml = string.Empty;
            String queryWithDateVariables = configuration["Collector:From:iSite:Query:QueryString"];
            int maxQueryResults = configuration.GetValue<int>("Collector:From:iSite:Query:MaxResults");

            int inervalQuery = configuration.GetValue<int>("Collector:From:iSite:Query:QueryInterval");




            String tempStr1 = string.Empty, tempStr2 = string.Empty;
            List<exam> exams = new List<exam>();
            try
            {
                bool _collectingData = true;

                while ((end > _start) && _collectingData && !_disengage)
                {
                    String convertedQuery = string.Empty;
                    _end = _start.Add(new TimeSpan(inervalQuery, 0, 0));
                    if (_end > end)
                        _end = end;


                    //UpdateStatus($"#date:{0}-{1}".FormatString(_start.ToString("MMM-dd-yyyy"), _end.ToString("MMM-dd-yyyy")));
                    //UpdateStatus("#time:{0}-{1}".FormatString(_start.ToString("HH:mm"), _end.ToString("HH:mm")));

                   
                    _end.ToString("yyyy-MM-dd HH:mm:ss");

                   
                    convertedQuery = queryWithDateVariables.Replace("START_DATE", _start.ToString("yyyy-MM-dd HH:mm:ss"));
                    convertedQuery = convertedQuery.Replace("END_DATE", _end.ToString("yyyy-MM-dd HH:mm:ss"));
                    string status = string.Format("Querying iSite for reports between {0} - {1}.", _start, _end);
                    logger.LogDebug(status);
                    // UpdateStatus(status);
                    try
                    {
                        // if (myiSite.GetLastErrorCode() == 0)
                        {
                            queryXml = _myiSite.Query(convertedQuery, "LOOKUP", maxQueryResults);
                            logger.LogDebug($"LOOKUP {convertedQuery}");

                            logger.LogDebug(queryXml);
                            if (!String.IsNullOrEmpty(queryXml))
                            {

                                XDocument doc = XDocument.Parse(queryXml); //or XDocument.Load(path)
                                string jsonText = JsonConvert.SerializeXNode(doc);
                                //dynamic resultObjs = JsonConvert.DeserializeObject<ExpandoObject>(jsonText);


                                var resultObjs = JsonConvert.DeserializeObject<QueryOutput>(jsonText);

                                var examlist = resultObjs.QueryResult.ExamList;
                                foreach (var exam in examlist.Exams)
                                {
                                    try
                                    {
                                        exams.Add(new exam
                                        {
                                            Mrn = exam.x00100020,
                                            Acc = exam.x00080050,
                                            Org = exam.OrganizationCode,
                                            InternalId = exam.IdxIntExamId,
                                            InternalPatientId = exam.IdxIntPatientId,
                                            ExamDt = DateTimeParser.ParseDateAsDateTime(exam.StudyDttm),
                                            Modality = exam.x00080060,
                                            ExamCode = exam.x00081032_1,
                                            ExamDesc = exam.x00081032_2,
                                            Subspecialty = exam.SubspecialityCode,
                                            BodyPart = exam.x00180015,
                                            Name = exam.x00100010,
                                            Gender = exam.x00100040,
                                            DateOfBirth = DateTimeParser.ParseDateAsDateTime(exam.x00100030),
                                            RefferingMd = exam.x00080090,
                                            ExamPriority = exam.IsStatExamFlag,
                                            ExamRead = exam.ExamReadFlag=="Y",
                                            PerformingResource = exam.PerformingResource,
                                            PatientLocation = exam.PatientLocation,



                                        }); ;
                                    }
                                    catch(Exception e)
                                    {
                                        logger.LogDebug(e,$"Exception in Parsing Exam:{jsonText}");
                                    }
                                }

                                //backup

                                //var results = XElement.Parse(queryXml);
                                //var examList = results.Element("ExamList");
                                //if (examList != null)
                                //{
                                //    var elements = examList.Elements();
                                //    var total = results.Element("TotalReturns").Value;
                                //    var matched = results.Element("TotalMatches").Value;

                                //    var currentReport = 0;
                                //    foreach (var exam in elements)
                                //    {


                                //        currentReport++;
                                //        status = string.Format("#currentReport:{0}", currentReport);

                                //        var e = new exam();
                                //        foreach (var examAttr in exam.Elements())
                                //        {


                                //            examAttr.Value = GetUtf8Value(examAttr.Value);

                                //            switch (examAttr.Name.ToString())
                                //            {
                                //                case "OrganizationCode":
                                //                    e.Org = examAttr.Value;
                                //                    break;
                                //                case "x00100020":
                                //                    e.Mrn = examAttr.Value;
                                //                    break;
                                //                case "x00080050":
                                //                    e.Acc = examAttr.Value;
                                //                    break;
                                //                case "x00100010":
                                //                    //var splitName = examAttr.Value.Split('^');
                                //                    //name = "{0}  {1}".FormatString(splitName[0], splitName[1]);
                                //                    e.Name = examAttr.Value;
                                //                    break;
                                //                case "x00100040":
                                //                    e.Gender = examAttr.Value;
                                //                    break;
                                //                case "x00100030":
                                //                    e.DateOfBirth = DateTime.Parse(examAttr.Value.Split('-')[0]);
                                //                    // isite dob times are in the format 19650203  (feb 3, 1965)
                                //                    //dob =
                                //                    //   Utils.DateParser.ParseDateAsDateTime(examAttr.Value.Split('-')[0]).
                                //                    //     GetValueOrDefault(DateTime.MinValue);
                                //                    break;
                                //                case "IDXExamStatus":
                                //                    //reportStatus = examAttr.Value == "T" ? "C" : examAttr.Value;
                                //                    break;
                                //                case "IDXIntExamID":
                                //                    e.InternalId = examAttr.Value;
                                //                    break;
                                //                case "IDXIntPatientID":
                                //                    e.InternalPatientId = examAttr.Value;
                                //                    break;
                                //                case "StudyDTTM":
                                //                    e.ExamDt = DateTime.Parse(examAttr.Value);
                                //                    break;
                                //                case "x00080090":
                                //                    e.RefferingMd = examAttr.Value;
                                //                    break;
                                //                case "IsStatExamFLAG":
                                //                    e.ExamPriority = examAttr.Value;//(examAttr.Value == "Y") ? 1 : 0;
                                //                    break;
                                //                case "ExamReadFLAG":
                                //                    e.ExamRead = examAttr.Value == "Y";
                                //                    break;

                                //                case "x00080060":
                                //                    e.Modality = examAttr.Value;
                                //                    break;
                                //                case "PerformingResource":
                                //                    e.PerformingResource = examAttr.Value;

                                //                    break;
                                //                case "SubspecialityCode":
                                //                    e.Subspecialty = examAttr.Value;
                                //                    //subSpecialityCode = examAttr.Value;
                                //                    break;
                                //                case "PatientLocation":
                                //                    // isite's definition of patient location is incorrect... this is ordering location
                                //                    //orderingLocation = examAttr.Value;
                                //                    e.PatientLocation = examAttr.Value;
                                //                    break;
                                //                case "x00180015":
                                //                    e.BodyPart = examAttr.Value;
                                //                    break;
                                //                case "x00081032_1":
                                //                    e.ExamCode = examAttr.Value;
                                //                    break;

                                //                case "x00081032_2":
                                //                    e.ExamDesc = examAttr.Value;
                                //                    break;
                                //            }
                                //        }
                                //        exams.Add(e);

                                //    }
                                //}



                            }
                        }
                    }
                    catch (Exception e)
                    {
                        logger.LogDebug("Exception in querying:", e);
                    }
                    status = string.Format("Parsing Reports between {0} - {1}.", _start, _end);
                   
                   
                    _start = _end;
                }

                return exams;
            }
            catch (Exception e)
            {
              
                logger.LogDebug(e,"Exception in querying.");
                return null;
                //throw (new Exception("Exception in Query Reports", e));
            }
            finally
            {
                if (_collectingData)
                    _collectingData = false;
            }
        }

        public void CollectData(DateTime startDt, DateTime endDt)
        {
            var exams = GetOrdersinDateRange(startDt, endDt);
            logger.LogDebug($"Number Of Exams- {exams.Count}");
            GetReports(exams);
        }


        private void GetReports(List<exam> exams)
        {
            foreach (var exam in exams)
            {
                try
                {

                    var report = _myiSite.GetReportData("", exam.InternalId);

                    logger.LogDebug($"Raw Output For Exam Mrn {exam.InternalPatientId}-Accession:{exam.Acc} -idxID:{exam.InternalId}--- {report}");
                    if (String.IsNullOrEmpty(report))
                    {
                        exam.Error = $"Mrn or Acc # is invalid.  Error code: {_myiSite.GetLastErrorCode()}";
                    }
                    else
                    {
                        exam.Reports = new List<Report>();
                        XDocument doc = XDocument.Parse(report); //or XDocument.Load(path)
                        string jsonText = JsonConvert.SerializeXNode(doc);
                        try
                        {
                            dynamic reps = JsonConvert.DeserializeObject(jsonText);
                            var allReports = reps.AllReports;
                            foreach (var rep in allReports)
                            {

                                var _rep = new Report
                                {
                                    Text = rep.Value["#cdata-section"].Value,
                                    Current = rep.Value["@Current"].Value,
                                    Status = rep.Value["@Status"].Value,
                                    ReportDt = DateTime.Parse(rep.Value["@DTTM"].Value)
                                };
                                //var result = rep.FirstOrDefault(x => x.key == "@Status").Value;
                                exam.Reports.Add(_rep);

                            }
                        }
                        catch (Exception e)
                        {
                            logger.LogError("Exception in report parsing", e);
                        }



                        //exam.Reports = report;
                    }
                    var store = configuration["Collector:Store:Path"];
                    var reportPath = $"{store}/Reports/{exam.ExamDt?.ToString("yyyy/MM/dd")}/{exam.Mrn}_{exam.Acc}.report.json";
                    //string fileName = (thePath + organization + "." + mrn + "." + acc.Replace(':', '_') + "." + reportStatus + ".parsed.xml").RemoveInvalidPathChars();
                    

                    using (var fileStream = CreateFileStream(reportPath))
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(exam));
                        fileStream.Write(bytes,0,bytes.Count());
                    }
                }
                catch (Exception e)
                {
                    exam.Error = $"Mrn or Acc # is invalid.  Exception: {e.ToString()}";
                    logger.LogError(e,"Exceptin in Get Reports");
                }
            }

        }
        
        private  Stream CreateFileStream(string filePath)
        {
            //try
            //{
            //    return File.Open(filePath, FileMode.Append, FileAccess.Write, FileShare.None);
            //    //return File.Create(filePath);
            //}
            //catch (DirectoryNotFoundException) { }

            string directory = Path.GetDirectoryName(filePath);
            if (directory != null)
                Directory.CreateDirectory(directory);

            return File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            //return File.Create(filePath);
        }

        public bool Authenticate(string user, string pwd)
        {
            throw new NotImplementedException();
        }

        public void Run(int defaultPastXdays = -1)
        {
            throw new NotImplementedException();
        }
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
