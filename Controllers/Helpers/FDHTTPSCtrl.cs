using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FD.Control
{
    public class HFDHttpsRequest
    {
        //public ABESettings _Settings;
        public string _HTTPMehtod = "GET";
        public string _Url;
        public byte[] _PostBuf;


        public System.Net.HttpWebRequest _Request;


        public HFDHttpsRequest setPost(string isPostValue)
        {
            _HTTPMehtod = "POST";
            _Request.Method = _HTTPMehtod;
            byte[] fPostBuf = Encoding.UTF8.GetBytes(isPostValue);
            _PostBuf = fPostBuf;
            return this;
        }
        public HFDHttpsRequest set(string isUrlBase)
        {
            _Url = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(isUrlBase));
            System.Net.HttpWebRequest fRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(_Url);
            fRequest.Proxy = null;
            fRequest.Headers.Add("Content-Type", "application/json");
            fRequest.Credentials = CredentialCache.DefaultCredentials;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
            | (SecurityProtocolType)3072;


            fRequest.Method = _HTTPMehtod;

            fRequest.Date = DateTime.Now;
            _Request = fRequest;

            return this;
        }
        public System.Net.WebResponse getResponse()
        {
            if (_PostBuf != null)
            {
                _Request.ContentLength = _PostBuf.Length;
                System.IO.Stream fPostStream = _Request.GetRequestStream();

                fPostStream.Write(_PostBuf, 0, _PostBuf.Length);
                fPostStream.Flush();
                fPostStream.Close();
            }
            return _Request.GetResponse();
        }
        public JObject getResponseAsJson()
        {
            try
            {
                if (_PostBuf != null)
                {
                    _Request.ContentLength = _PostBuf.Length;
                    System.IO.Stream fPostStream = _Request.GetRequestStream();
                    fPostStream.Write(_PostBuf, 0, _PostBuf.Length);
                    fPostStream.Flush();
                    fPostStream.Close();
                }
                var fResponce = _Request.GetResponse();
                //Получаем ответ от интернет-ресурса.

                //Экземпляр класса System.IO.Stream 
                //для чтения данных из интернет-ресурса.
                System.IO.Stream dataStream = fResponce.GetResponseStream();

                //Инициализируем новый экземпляр класса 
                //System.IO.StreamReader для указанного потока.
                System.IO.StreamReader sreader = new System.IO.StreamReader(dataStream);

                //Считываем поток от текущего положения до конца.            
                string fsRead = sreader.ReadToEnd();

                //Закрываем поток ответа.
                fResponce.Close();

                return JsonConvert.DeserializeObject<JObject>(fsRead);
            }
            catch(Exception ex) { }
            return null;
        }
        public byte[] getResponseAsBuf()
        {
            try
            {
                if (_PostBuf != null)
                {
                    _Request.ContentLength = _PostBuf.Length;
                    System.IO.Stream fPostStream = _Request.GetRequestStream();
                    fPostStream.Write(_PostBuf, 0, _PostBuf.Length);
                    fPostStream.Flush();
                    fPostStream.Close();
                }
                var fResponce = _Request.GetResponse();
                //Получаем ответ от интернет-ресурса.

                //Экземпляр класса System.IO.Stream 
                //для чтения данных из интернет-ресурса.
                System.IO.Stream fStream = fResponce.GetResponseStream();

                //Инициализируем новый экземпляр класса 
                //System.IO.StreamReader для указанного потока.
                System.IO.StreamReader fReader = new System.IO.StreamReader(fStream);
                byte[] fBuf = null;
                using (var memstream = new MemoryStream())
                {
                    fReader.BaseStream.CopyTo(memstream);
                    fBuf = memstream.ToArray();
                }

                //Закрываем поток ответа.
                fResponce.Close();
                return fBuf;

            }
            catch (Exception ex) { }

            return null;
         }
    }

}