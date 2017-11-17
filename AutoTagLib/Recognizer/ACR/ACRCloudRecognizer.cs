using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using AutoTagLib.ErrorManager;

namespace AutoTagLib.Recognizer
{
    public class ACRCloudRecognizer
    {
        public enum RECOGNIZER_TYPE { 
            acr_rec_type_audio, acr_rec_type_humming, acr_rec_type_both
        };
        public string _host;
        private string _accessKey;
        private string _accessSecret;
        private int _timeout = 5 * 1000; // ms
        private RECOGNIZER_TYPE _recType;
        private bool debug = false;
        private readonly ACRCloudExtrTool acrTool = new ACRCloudExtrTool();

        private static ACRCloudRecognizer instance = null;
        public static ACRCloudRecognizer Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new ACRCloudRecognizer();
                        }
                    }
                }
                return instance;
            }
        }

        private static readonly object padlock = new object();

        
        private ACRCloudRecognizer()
        {
            if (ConfigurationManager.GetSection("ACRCloud") is NameValueCollection acrCloudConfig)
            {
                _host = acrCloudConfig["host"];
                _accessKey = acrCloudConfig["access_key"];
                _accessSecret = acrCloudConfig["access_secret"];
                _timeout = int.Parse(acrCloudConfig["timeout"]) * 1000;
            }
            _recType = RECOGNIZER_TYPE.acr_rec_type_audio;
        }
      


        /**
          *
          *  recognize by file path of (Audio/Video file)
          *          Audio: mp3, wav, m4a, flac, aac, amr, ape, ogg ...
          *          Video: mp4, mkv, wmv, flv, ts, avi ...
          *
          *  @param filePath query file path
          *  @param startSeconds skip (startSeconds) seconds from from the beginning of (filePath)
          *  
          *  @return result 
          *
          **/
        public String RecognizeByFile(string filePath, int startSeconds)
        {
            byte[] ext_fp = null;
            IDictionary<string, Object> query_data = new Dictionary<string, Object>();
            try
            {
                switch (Instance._recType)
                {
                    case RECOGNIZER_TYPE.acr_rec_type_audio:
                        ext_fp = Instance.acrTool.CreateFingerprintByFile(filePath, startSeconds, 12, false);
                        query_data.Add("ext_fp", ext_fp);
                        break;
                    default:
                        return ACRCloudStatusCode.NO_RESULT;
                }
            }
            catch (Exception)
            {
                ((IErrorManager)Lookup.GetInstance().Get(typeof(IErrorManager))).NewError(ErrorCodes.acr_decode_audio);
                return ACRCloudStatusCode.DECODE_AUDIO_ERROR;
            }

            if (ext_fp == null)
            {
                return ACRCloudStatusCode.NO_RESULT;
            }
            return this.DoRecognize(query_data);
        }

        private string PostHttp(string url, IDictionary<string, Object> postParams)
        {
            string result = "";

            string BOUNDARYSTR = "acrcloud***copyright***2015***" + DateTime.Now.Ticks.ToString("x");
            string BOUNDARY = "--" + BOUNDARYSTR + "\r\n";
            var ENDBOUNDARY = Encoding.ASCII.GetBytes("--" + BOUNDARYSTR + "--\r\n\r\n");

            var stringKeyHeader = BOUNDARY +
                           "Content-Disposition: form-data; name=\"{0}\"" +
                           "\r\n\r\n{1}\r\n";
            var filePartHeader = BOUNDARY +
                            "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
                            "Content-Type: application/octet-stream\r\n\r\n";

            var memStream = new MemoryStream();
            foreach (var item in postParams)
            {
                if (item.Value is string)
                {
                    string tmpStr = string.Format(stringKeyHeader, item.Key, item.Value);
                    byte[] tmpBytes = Encoding.UTF8.GetBytes(tmpStr);
                    memStream.Write(tmpBytes, 0, tmpBytes.Length);
                }
                else if (item.Value is byte[])
                {
                    var header = string.Format(filePartHeader, item.Key, item.Key);
                    var headerbytes = Encoding.UTF8.GetBytes(header);
                    memStream.Write(headerbytes, 0, headerbytes.Length);
                    byte[] sample = (byte[])item.Value;
                    memStream.Write(sample, 0, sample.Length);
                    memStream.Write(Encoding.UTF8.GetBytes("\r\n"), 0, 2);
                }
            }
            memStream.Write(ENDBOUNDARY, 0, ENDBOUNDARY.Length);

            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream writer = null;
            StreamReader myReader = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = Instance._timeout;
                request.Method = "POST";
                request.ContentType = "multipart/form-data; boundary=" + BOUNDARYSTR;

                memStream.Position = 0;
                byte[] tempBuffer = new byte[memStream.Length];
                memStream.Read(tempBuffer, 0, tempBuffer.Length);

                writer = request.GetRequestStream();
                writer.Write(tempBuffer, 0, tempBuffer.Length);
                writer.Flush();
                writer.Close();
                writer = null;

                response = (HttpWebResponse)request.GetResponse();
                myReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                result = myReader.ReadToEnd();
            }
            catch (WebException)
            {            
                // HEREGUY
                ((IErrorManager)Lookup.GetInstance().Get(typeof(IErrorManager))).NewError(ErrorCodes.acr_timeout);
                
                result = ACRCloudStatusCode.HTTP_ERROR;
            }
            catch (Exception)
            {
                ((IErrorManager)Lookup.GetInstance().Get(typeof(IErrorManager))).NewError(ErrorCodes.acr_unknown);
                result = ACRCloudStatusCode.HTTP_ERROR;
            }
            finally
            {
                if (memStream != null)
                {
                    memStream.Close();
                    memStream = null;
                }
                if (writer != null)
                {
                    writer.Close();
                    writer = null;
                }
                if (myReader != null)
                {
                    myReader.Close();
                    myReader = null;
                }
                if (request != null)
                {
                    request.Abort();
                    request = null;
                }
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
            }

            return result;
        }

        private string EncryptByHMACSHA1(string input, string key)
        {
            HMACSHA1 hmac = new HMACSHA1(System.Text.Encoding.UTF8.GetBytes(key));
            byte[] stringBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashedValue = hmac.ComputeHash(stringBytes);
            return EncodeToBase64(hashedValue);
        }

        private string EncodeToBase64(byte[] input)
        {
            string res = Convert.ToBase64String(input, 0, input.Length);
            return res;
        }

        private string DoRecognize(IDictionary<string, Object> query_data)
        {
            byte[] ext_fp = null;
            string method = "POST";
            string httpURL = "/v1/identify";
            string dataType = "fingerprint";
            string sigVersion = "1";
            string timestamp = ((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds).ToString();

            string reqURL = "http://" + _host + httpURL;

            string sigStr = method + "\n" + httpURL + "\n" + _accessKey + "\n" + dataType + "\n" + sigVersion + "\n" + timestamp;
            string signature = EncryptByHMACSHA1(sigStr, _accessSecret);

            var dict = new Dictionary<string, object> { { "access_key", _accessKey } };
            if (query_data.ContainsKey("ext_fp")) {
                ext_fp = (byte[])query_data["ext_fp"];
                if (ext_fp != null)
                {
                    dict.Add("sample_bytes", ext_fp.Length.ToString());
                    dict.Add("sample", ext_fp);
                }
            }
            if (ext_fp == null)
            {
                return ACRCloudStatusCode.NO_RESULT;
            }
            dict.Add("timestamp", timestamp);
            dict.Add("signature", signature);
            dict.Add("data_type", dataType);
            dict.Add("signature_version", sigVersion);

            string res = PostHttp(reqURL, dict);

            return res;
        }
    }

}