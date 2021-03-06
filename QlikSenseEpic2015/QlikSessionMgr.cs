﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QlikSenseEpic2015;
using System.Runtime.InteropServices;
using EBIFoundation82;
using Encryptor;
using System.Security;
using System.Reflection;
using System.IO;
using System.Web;

namespace QlikSenseEpic2015
{
    [ComVisible(true)]
    [Guid("1C4243C8-FF24-4696-9AB9-281DAB2E43F6")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("QlikSenseEpic2015.QlikSessionMgr")]
    public class QlikSessionMgr:IBISessionManager
    {
        private static string KEY_FILENAME =  Path.Combine(getDirectory , "qliksenseepic2015.config");
        private AESEncrypt Encryptor;
        private EBIFoundation82.IBIConfiguration BIConfiguration;
        string QlikServerUrl = "";
        string BiUserId = "";
        string handshake = "";
		string key = "";


		public QlikSessionMgr()
        {
            using (var reader = new StreamReader(KEY_FILENAME))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("key="))
                    {
                        key = line.Substring(4);
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new FileNotFoundException(string.Format("Could not read key value pair 'key' from '{0}'.", KEY_FILENAME));
            }
            Encryptor = new AESEncrypt();
            
        }

        public static string getDirectory
        {
            get
            {
                string codeBase = AppDomain.CurrentDomain.BaseDirectory;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public string ErrorMessage
        {
            get
            {
                return "Qlik integration failed login."; 
            }
        }

        public bool LogIn()
        {

            if (String.IsNullOrEmpty(BiUserId))
            {
                BiUserId = "sqlviews\\jog";
            }

            if (String.IsNullOrEmpty(QlikServerUrl))
            {
                QlikServerUrl = "http://qliksense.112adams.local";
            }

            return true;
        }
        public bool LogOff()
        {
            //Code to implement LogOff
            return true;
        }
        public string GetURL()
        {
            string finalURL = string.Format("{0}/epic/login?token={1}", QlikServerUrl, HttpUtility.UrlEncode(Encryptor.OpenSSLEncrypt(BiUserId+"|" + handshake,key)));
            return finalURL;
        }
        public bool IsLoggedIn()
        {
            //Code to implement IsLoggedIn
            //BiUserId = BIConfiguration.get_BIUserName();
            //QlikViewServerUrl = BIConfiguration.get_BIServerName();
            return true;
        }

        public void set_Configuration(ref _IBIConfiguration value)
        {
            BIConfiguration = (EBIFoundation82.IBIConfiguration)value;
            QlikServerUrl = BIConfiguration.get_BIURL();
            BiUserId = BIConfiguration.get_BIUserName();
			handshake = BIConfiguration.get_BIClusterName();
            
        }



    }
}
