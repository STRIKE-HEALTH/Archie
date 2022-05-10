using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Archie.Query
{

    public partial class GetReportOutput
    {
        [JsonProperty("AllReports")]
        public AllReports AllReports { get; set; }
    }

    public partial class AllReports
    {
        [JsonProperty("Report")]
        [JsonConverter(typeof(SingleOrArrayConverter<Report>))]
        public List<Report> Reports { get; set; }
    }
    public class Report
    {
        [JsonProperty("#cdata-section")]
        public string Text { get; set; }
        [JsonProperty("@Status")]
        public string Status { get; set; }
        [JsonProperty("@Current")]
        public string Current { get; set; }
        [JsonProperty("@DTTM")]
        public DateTime ReportDt { get; set; }

        [JsonProperty("@PrincipalInterpreter")]
        public string PrincipalInterpreter { get; set; }
    }
    public partial class QueryOutput
    {
        [JsonProperty("QueryResult")]
        public QueryResult QueryResult { get; set; }
    }

    public partial class QueryResult
    {
        [JsonProperty("TotalMatches")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long TotalMatches { get; set; }

        [JsonProperty("TotalReturns")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long TotalReturns { get; set; }

        [JsonProperty("ExamList")]
        public ExamList ExamList { get; set; }
    }

    public partial class ExamList
    {
        [JsonProperty("Exam")]
        [JsonConverter(typeof(SingleOrArrayConverter<Exam>))]
        public List<Exam> Exams { get; set; }
    }

    public partial class Exam
    {
        [JsonProperty("GID")]
        public Gid Gid { get; set; }

        [JsonProperty("x00100010")]
        public string x00100010 { get; set; }

        [JsonProperty("x00100020")]
      
        public string x00100020 { get; set; }

        [JsonProperty("x00100030")]
       
        public string x00100030 { get; set; }

        [JsonProperty("x00100040")]
        public string x00100040 { get; set; }

        [JsonProperty("StudyDTTM")]
        public string StudyDttm { get; set; }

        [JsonProperty("x00080050")]
        public string x00080050 { get; set; }

        [JsonProperty("x00080090")]
        public string x00080090 { get; set; }

        [JsonProperty("x00180015")]
        public string x00180015 { get; set; }

        [JsonProperty("x00080060")]
        public string x00080060 { get; set; }

        [JsonProperty("x00081032_1")]
       
        public string x00081032_1 { get; set; }

        [JsonProperty("x00081032_2")]
        public string x00081032_2 { get; set; }

        [JsonProperty("x00081080")]
        public string x00081080 { get; set; }

        [JsonProperty("IsStatExamFLAG")]
        public string IsStatExamFlag { get; set; }

        [JsonProperty("IDXExamStatus")]
        public string IdxExamStatus { get; set; }

        [JsonProperty("LockStatus")]
        public string LockStatus { get; set; }

        [JsonProperty("LockedByName")]
        public string LockedByName { get; set; }

        [JsonProperty("PatientLocation")]
        public string PatientLocation { get; set; }

        [JsonProperty("HasImagesFLAG")]
        public string HasImagesFlag { get; set; }

        [JsonProperty("IDXIntReferringPhysID")]
        public string IdxIntReferringPhysId { get; set; }

        [JsonProperty("IDXIntPatientID")]
        public string IdxIntPatientId { get; set; }

        [JsonProperty("IDXIntExamID")]
        public string IdxIntExamId { get; set; }

        [JsonProperty("OrganizationCode")]
        public string OrganizationCode { get; set; }

        [JsonProperty("SubspecialityCode")]
        public string SubspecialityCode { get; set; }

        [JsonProperty("ExamReadFLAG")]
        public string ExamReadFlag { get; set; }

        [JsonProperty("PerformingResource")]
        public string PerformingResource { get; set; }

        [JsonProperty("HasReports")]
        public string HasReports { get; set; }

        [JsonProperty("NoteIndicatorCode")]
        public string NoteIndicatorCode { get; set; }

        [JsonProperty("IsNIAMRExam")]
        public string IsNiamrExam { get; set; }
    }

    public partial class Gid
    {
        [JsonProperty("GIDLabel")]
        public string GidLabel { get; set; }

        [JsonProperty("GIDValue")]
        public string GidValue { get; set; }
    }

    public partial class QueryOutput
    {
        public static QueryOutput FromJson(string json) => JsonConvert.DeserializeObject<QueryOutput>(json, Archie.Query.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this QueryOutput self) => JsonConvert.SerializeObject(self, Archie.Query.Converter.Settings);
        public static string ToQJson(this GetReportOutput self) => JsonConvert.SerializeObject(self, Archie.Query.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }



    public class SingleOrArrayConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(List<T>));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Array)
            {
                return token.ToObject<List<T>>();
            }
            return new List<T> { token.ToObject<T>() };
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            List<T> list = (List<T>)value;
            if (list.Count == 1)
            {
                value = list[0];
            }
            serializer.Serialize(writer, value);
        }

        public override bool CanWrite
        {
            get { return true; }
        }
    }

}
