using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace MCNetworkLoginDiscordBot
{
	public class HttpQuery
	{
		public static string Get(string url)
		{
			var request = WebRequest.Create(url);
			ServicePointManager.Expect100Continue = true;

			((HttpWebRequest)request).UserAgent = "MCNetworkLogin Discord Bot";
			request.Method = "GET";
			request.ContentType = "application/x-www-form-urlencoded";

			using (var response = request.GetResponse())
			{
				using (var dataStream = response.GetResponseStream())
				{
					using (var reader = new StreamReader(dataStream))
					{
						return reader.ReadToEnd();
					}
				}
			}
		}

		public static string Post(string url, string data)
		{
			var byteArray = new UTF8Encoding().GetBytes(data);
			var request = WebRequest.Create(url);
			ServicePointManager.Expect100Continue = true;
			//ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

			((HttpWebRequest)request).UserAgent = "MCNetworkLogin Discord Bot";
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			request.ContentLength = byteArray.Length;

			using (var dataStream = request.GetRequestStream())
			{
				dataStream.Write(byteArray, 0, byteArray.Length);
			}
			using (var response = request.GetResponse())
			{
				using (var dataStream1 = response.GetResponseStream())
				{
					using (var reader = new StreamReader(dataStream1))
					{
						return reader.ReadToEnd();
					}
				}
			}
		}

		public static string ToPostArgs(IEnumerable<string> data)
		{
			return data.Aggregate((current, a) => current + "&" + a).TrimStart('&');
		}
	}
}
