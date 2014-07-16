﻿using System;
using System.IO;
using System.Net;
using System.Text;

using Newtonsoft.Json;

namespace SODA
{
    public class SodaClient
    {
        public string AppToken { get; private set; }
        public string Username { get; private set; }
        private string Password { get; set; }
        
        public SodaClient(string appToken, string username = null, string password = null)
        {
            if (String.IsNullOrEmpty(appToken))
                throw new ArgumentNullException("appToken", "An AppToken is required");

            AppToken = appToken;
            Username = username;
            Password = password;
        }

        public Dataset GetDataset(string domain, string datasetId)
        {
            var uri = SodaUri.ForMetadata(domain, datasetId);

            var metadata = Get<DatasetMetadata>(uri);

            return new Dataset(domain, metadata, this);
        }

        public DatasetMetadata GetMetadata(string domain, string datasetId)
        {
            var dataset = GetDataset(domain, datasetId);

            return dataset.Metadata;
        }

        public dynamic Upsert(string domain, string datasetId, dynamic payload)
        {
            string json = JsonConvert.SerializeObject(payload);

            return Upsert(domain, datasetId, json);
        }

        public dynamic Upsert(string domain, string datasetId, string payload)
        {
            throw new NotImplementedException();
        }
        
        public TResult Get<TResult>(Uri uri)
        {
            return sendRequest<TResult>(uri);
        }
        
        public dynamic Post(Uri uri, string body)
        {
            return sendRequest(uri, "POST", body);
        }

        public dynamic Put(Uri uri, string body)
        {
            return sendRequest(uri, "PUT", body);
        }
       
        protected dynamic sendRequest(Uri uri, string method = "GET", string body = null)
        {
            return sendRequest<dynamic>(uri, method, body);
        }

        protected TResult sendRequest<TResult>(Uri uri, string method = "GET", string body = null)
        {
            var request = createRequest(uri, method, body);

            try
            {
                using (var responseStream = request.GetResponse().GetResponseStream())
                {
                    var json = new StreamReader(responseStream).ReadToEnd();
                    TResult entity = JsonConvert.DeserializeObject<TResult>(json);
                    return entity;
                }
            }
            catch (WebException webException)
            {
                throw SodaException.Wrap(webException);
            }
            catch (Exception ex)
            {
                throw SodaException.Wrap(ex);
            }
        }

        protected virtual WebRequest createRequest(Uri uri, string method = "GET", string body = null)
        {
            var request = WebRequest.Create(uri);
            request.Method = method;
            request.Headers["X-App-Token"] = AppToken;

            if (!String.IsNullOrEmpty(Username) && !String.IsNullOrEmpty(Password))
            {
                string authKVP = String.Format("{0}:{1}", Username, Password);
                byte[] authBytes = Encoding.Default.GetBytes(authKVP);
                request.Headers["Authorization"] = String.Format("Basic {0}", Convert.ToBase64String(authBytes));
            }

            if (!String.IsNullOrEmpty(body))
            {
                byte[] bodyBytes = Encoding.UTF8.GetBytes(body);

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(bodyBytes, 0, bodyBytes.Length);
                }
            }

            return request;
        }
    }
}