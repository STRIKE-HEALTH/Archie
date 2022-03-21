using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archie
{
	public static class DateTimeParser
	{
		static string[] monthFormats = new string[] { "MMM yy", "MMM yyyy", "MM yy", "MM yyyy", "M yy", "M yyyy", "yyyyMM", "MM.yyyy", "M.yyyy" };
		static string[] yearFormats = new string[] { "yy", "yyyy" };
		static string[] dayFormats = new string[] { "MMM dd yy", "MMM d yy", "MM/d/yyyy", "MM/dd/yyyy", "M/d/yyyy", "M/dd/yyyy", "MMM dd yyyy", "MMM d yyyy", "MM dd yy", "MM d yy", "MM d yyyy", "MM dd yyyy", "M dd yy", "M d yy", "M dd yyyy", "M d yyyy", "yyyyMMdd", "MM.dd.yyyy", "M.d.yyyy", "MM.d.yyyy", "M.dd.yyyy", "yyyy.MM.dd", "MM-dd-yyyy", "M-d-yyyy", "MM-d-yyyy", "M-dd-yyyy", "yyyy-MM-dd" };

		static string[] dateTimeFormats = new string[] {  "MMM dd yy HH:mm","MMM dd yy hh:mm tt","MMM dd yy h:mm tt",
														 "MMM d yy HH:mm","MMM d yy hh:mm tt","MMM d yy h:mm tt",
														 "MMM dd yyyy HH:mm","MMM dd yyyy hh:mm tt","MMM dd yyyy h:mm tt",
														 "MMM d yyyy HH:mm","MMM d yyyy hh:mm tt","MMM d yyyy h:mm tt",
														 "MM dd yy HH:mm","MM dd yy hh:mm tt","MM dd yy h:mm tt",
														 "MM/dd/yy HH:mm","MM/dd/yy hh:mm tt","MM/dd/yy h:mm tt",
														 "MM/dd/yy HH:mm:ss","MM/dd/yy hh:mm:ss tt","MM/dd/yy h:mm:ss tt",
														 "MM d yy HH:mm","MM d yy hh:mm tt","MM d yy h:mm tt",
														 "MM/d/yy HH:mm","MM/d/yy hh:mm tt","MM/d/yy h:mm tt",
														 "MM d yyyy HH:mm","MM d yyyy hh:mm tt","MM d yyyy h:mm tt",
														 "MM/d/yyyy HH:mm","MM/d/yyyy hh:mm tt","MM/d/yyyy h:mm tt",
														 "MM dd yyyy HH:mm","MM dd yyyy hh:mm tt","MM dd yyyy h:mm tt",
														 "MM/dd/yyyy HH:mm","MM/dd/yyyy hh:mm tt","MM/dd/yyyy h:mm tt",
														 "MM/dd/yyyy HH:mm:ss","MM/dd/yyyy hh:mm:ss tt","MM/dd/yyyy h:mm:ss tt","M/dd/yyyy h:mm:ss",
														 "M dd yy HH:mm","M dd yy hh:mm tt","M dd yy h:mm tt",
														 "M/dd/yy HH:mm","M/dd/yy hh:mm tt","M/dd/yy h:mm tt",
														 "M d yy HH:mm","M d yy hh:mm tt","M d yy h:mm tt",
														 "M/d/yy HH:mm","M/d/yy hh:mm tt","M/d/yy h:mm tt",
														 "M dd yyyy HH:mm","M dd yyyy hh:mm tt","M dd yyyy h:mm tt",
														 "M/dd/yyyy HH:mm","M/dd/yyyy hh:mm tt","M/dd/yyyy h:mm tt",
														 "M/dd/yyyy HH:mm:ss","M/dd/yyyy hh:mm:ss tt","M/dd/yyyy h:mm:ss tt",
														 "M d yyyy HH:mm","M d yyyy hh:mm tt","M d yyyy h:mm tt",
														 "M/d/yyyy HH:mm","M/d/yyyy hh:mm tt","M/d/yyyy h:mm tt",
														 "M/d/yyyy HH:mm:ss","M/d/yyyy hh:mm:ss tt","M/d/yyyy h:mm:ss tt",
														 "yyyyMMddHHmmssffff","yyyyMMddHHmmss.ffffff","yyyyMMddHHmmssfff","yyyyMMddHHmmssff","yyyyMMddHHmmss.ff","yyyyMMddHHmmss.fff","yyyyMMddHHmmss.ffff","yyyyMMddHHmmssf","yyyyMMddHHmmss.f","yyyyMMddHHmmss", "yyyyMMddHHmm","yyyy/MM/dd HH:mm","yyyy/M/d HH:mm","yyyy/M/d h:mm","yyyy/MM/dd HH:mm:ss",
														 "yyyy-MM-dd HH:mm:ss","yyyy-MM-dd HH:mm","yyyy-MM-dd h:mm tt","yyyy-MM-dd hh:mm tt","yyyy-MM-dd hh:mm:ss.fff","yyyy-MM-dd HH:mm:ss.fff",
														 "yyyyMMddHHmmsszzzz" // 20110629092627-0500
				};
		static IFormatProvider formatProvider = System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat;
		static System.Globalization.DateTimeStyles dtStyle = System.Globalization.DateTimeStyles.AllowWhiteSpaces;

		public static bool ParseDate(string date, out DateTime dt)
		{

			//DateTime dt;
			//string[] dateformats = new String[] { "yyyyMMddHHmmss", "yyyyMMddHHmm", "yyyyMMdd", "MM/dd/yyyy HH:mm:ss tt", "M/dd/yyyy HH:mm:ss tt", "M/d/yyyy HH:mm:ss tt", "yyyy-MM-dd HH:mm:ss" };

			//DateTime.ParseExact(date, dateformats, System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None).ToString("yyyyMMddHHmmss");
			string format = "";
			if (DateTime.TryParseExact(date, yearFormats, formatProvider, dtStyle, out dt))
				format = "yyyy";
			else if (DateTime.TryParseExact(date, monthFormats, formatProvider, dtStyle, out dt))
				format = "yyyyMM";
			else if (DateTime.TryParseExact(date, dayFormats, formatProvider, dtStyle, out dt))
				format = "yyyyMMdd";
			else if (DateTime.TryParseExact(date, dateTimeFormats, formatProvider, dtStyle, out dt))
				format = "yyyyMMddHHmm";
			if (format != "")
				return true;
			else
				return false;



		}

		public static bool ParseDate(string date, out DateTime dt, out string format)
		{


			//DateTime dt;
			//string[] dateformats = new String[] { "yyyyMMddHHmmss", "yyyyMMddHHmm", "yyyyMMdd", "MM/dd/yyyy HH:mm:ss tt", "M/dd/yyyy HH:mm:ss tt", "M/d/yyyy HH:mm:ss tt", "yyyy-MM-dd HH:mm:ss" };

			//DateTime.ParseExact(date, dateformats, System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None).ToString("yyyyMMddHHmmss");
			string frmt = null;
			if (DateTime.TryParseExact(date, yearFormats, formatProvider, dtStyle, out dt))
				frmt = "yyyy";
			else if (DateTime.TryParseExact(date, monthFormats, formatProvider, dtStyle, out dt))
				frmt = "yyyyMM";
			else if (DateTime.TryParseExact(date, dayFormats, formatProvider, dtStyle, out dt))
				frmt = "yyyyMMdd";
			else if (DateTime.TryParseExact(date, dateTimeFormats, formatProvider, dtStyle, out dt))
				frmt = "yyyyMMddHHmm";
			format = frmt;
			if (frmt != "")
				return true;
			else
				return false;


		}

		public static string ParseDate(string date)
		{
			DateTime? dt = ParseDateAsDateTime(date);
			if (dt.HasValue)
				return dt.Value.ToString("MMM dd, yyyy, HH:mm");
			else
				return null;

		}
		public static DateTime? ParseDateAsDateTime(string date)
		{
			//string[] formats = new String[] { "yyyyMMddHHmmss", "yyyyMMddHHmm", "yyyyMMdd", "yyyyMM", "yyyy", "yy", "MM/dd/yyyy HH:mm:ss tt", "MM/dd/yyyy hh:mm:ss tt", "MM/dd/yyyy H:mm:ss tt", "MM/dd/yyyy h:mm:ss tt", "M/dd/yyyy HH:mm:ss tt", "M/dd/yyyy hh:mm:ss tt", "M/dd/yyyy h:mm:ss tt", "M/dd/yyyy H:mm:ss tt", "M/d/yyyy HH:mm:ss tt", "M/d/yyyy hh:mm:ss tt", "M/d/yyyy H:mm:ss tt", "M/d/yyyy h:mm:ss tt", "MMM yyyy", "MMM yy", "MMM dd yyyy", "MMM d yyyy", "MMM d yy", "MMM dd yy" };
			if (string.IsNullOrEmpty(date))
				return null;
			DateTime dt; //= DateTime.ParseExact(date, formats, System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None);
			if (ParseDate(date, out dt))
				return dt;
			else
				return null;
		}
		public static string ParseDate(string date, string format)
		{
			if (string.IsNullOrEmpty(date))
				return null;

			DateTime? dt = ParseDateAsDateTime(date);//, new String[] { "yyyyMMddHHmmss", "yyyyMMddHHmm", "yyyyMMdd", "MM/dd/yyyy HH:mm:ss tt", "MM/dd/yyyy hh:mm:ss tt", "MM/dd/yyyy H:mm:ss tt", "MM/dd/yyyy h:mm:ss tt", "M/dd/yyyy HH:mm:ss tt", "M/dd/yyyy hh:mm:ss tt", "M/dd/yyyy h:mm:ss tt", "M/dd/yyyy H:mm:ss tt", "M/d/yyyy HH:mm:ss tt", "M/d/yyyy hh:mm:ss tt", "M/d/yyyy H:mm:ss tt", "M/d/yyyy h:mm:ss tt" }, System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None);
			if (dt.HasValue)
				return dt.Value.ToString(format);
			else
				return null;
		}
	}
}
