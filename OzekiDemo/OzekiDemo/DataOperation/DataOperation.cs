using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

using OzekiDemo.DataObjects;
using OzekiDemo.Utility;

using SharedProject;

namespace OzekiDemo.DataOperation
{
    public static class DataOperation
    {
        private static string connectionString = string.Empty; 
        private static ZoiperdbContext dbContext = null;


        #region DB OPERATIONS

        /// <summary>
        /// Get contact by phone
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns>DbContact</returns>
        public static DbContact GetContactByPhone(string phoneNumber)
        {
            dbContext = new ZoiperdbContext(AuthenticateConnectionToDB());
            ApplicationLog.WriteEventToLog("Path to ZoiperDB from GetContactByPhone: " + AuthenticateConnectionToDB());
            DbContact contact = new DbContact();
            try
            {
                TblPhoneContact tblContact = dbContext.TblPhoneContact.Where(x => x.Telephone == phoneNumber).FirstOrDefault();
                contact = new DbContact(tblContact.ContactName, tblContact.Telephone, tblContact.Organization, tblContact.Url);
                //return contact;
                ApplicationLog.WriteEventToLog(string.Format("Name: {0}, Phone: {1}, Organization: {2}, Url: {3}", contact.FullName, contact.Phone, contact.Company, contact.Url));
            }
            catch (Exception ex)
            {
                ApplicationLog.WriteEventToLog(ex.ToString());
                //LibraryLog.WriteErrorLog(ex.Message);
                //return contact;
            }
            return contact;
        }

        /// <summary>
        /// Get User account for calling
        /// </summary>
        /// <returns></returns>
        public static string GetAccountID()
        {
            string account = string.Empty;
            try
            {
                dbContext = new ZoiperdbContext(AuthenticateConnectionToDB());
                TblZoiperSetting tblSetting = dbContext.TblZoiperSetting.FirstOrDefault();
                account = tblSetting.CallerId + "@" + tblSetting.Domain;
            }
            catch (Exception ex)
            {
                ApplicationLog.WriteErrorToLog(ex);
                //LibraryLog.WriteErrorLog(ex);
            }
            return account;
        }


        /// <summary>
        /// Save Settings
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static bool SaveSettings(Settings settings)
        {
            bool iSuccess = false;
            try
            {
                if (settings != null)
                {
                    //Persist Data into Database
                    dbContext = new ZoiperdbContext(AuthenticateConnectionToDB());
                    ApplicationLog.WriteEventToLog("Path to ZoiperDB from SaveSettings: " + AuthenticateConnectionToDB());
                    TblZoiperSetting zoiperSetting = new TblZoiperSetting()
                    {
                        Domain = settings.Domain,
                        Username = settings.Username,
                        CallerId = settings.CallerId,
                        ServiceDuration = settings.ServiceDuration,
                        IsRestart = Convert.ToInt32(settings.Restart),
                        Password = SharedFunctions.Hash(settings.Password),
                        RegistrationStatus = settings.RegistrationStatus
                    };
                    ApplicationLog.WriteEventToLog(string.Format("Domain: {0}, Username: {1}, CallerId: {2}, Duration: {3}, Restart: {4}, Password: {5}", zoiperSetting.Domain, zoiperSetting.Username, zoiperSetting.CallerId, zoiperSetting.ServiceDuration, zoiperSetting.IsRestart, zoiperSetting.Password));
                    dbContext.TblZoiperSetting.InsertOnSubmit(zoiperSetting);
                    dbContext.SubmitChanges();
                    iSuccess = true;
                    ApplicationLog.WriteEventToLog("Settings saved into ZoiperDB");
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.WriteErrorToLog(ex.ToString());
            }
            return iSuccess;
        }

        public static bool GetSIPRegistrationStatus()
        {
            bool iSuccess = false;
            try
            {
                dbContext = new ZoiperdbContext(AuthenticateConnectionToDB());
                int? registrationStatus = dbContext.TblZoiperSetting.FirstOrDefault().RegistrationStatus;
                if (registrationStatus == 1)
                    iSuccess = true;
                ApplicationLog.WriteErrorToLog("Registration status: " + registrationStatus);
            }
            catch (Exception ex)
            {
                ApplicationLog.WriteErrorToLog(ex);
                iSuccess = false;
            }
            return iSuccess;
        }

        public static Settings GetSIPAccount()
        {
            //bool iSuccess = false;
            Settings sipSetting = new Settings();
            try
            {
                dbContext = new ZoiperdbContext(AuthenticateConnectionToDB());
                TblZoiperSetting sipAccount = dbContext.TblZoiperSetting.FirstOrDefault();
                sipSetting = new Settings(sipAccount.Domain, sipAccount.Username, sipAccount.Password, sipAccount.CallerId, sipAccount.RegistrationStatus);
                ApplicationLog.WriteErrorToLog("Success");
            }
            catch (Exception ex)
            {
                ApplicationLog.WriteErrorToLog(ex);
                //iSuccess = false;
            }
            return sipSetting;
        }

       
        #endregion


        #region HELPER METHODS

        public static string GetAppPath(string imagePath)
        {
            string Dir2 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string path2 = Dir2 + imagePath;
            return path2;
        }

        private static string AuthenticateConnectionToDB()
        {
            return RouteToDB.ConnectionToZoiperDB();
            //connectionString = GetAppPath(@"\Zoiperdb.sdf");
            //ApplicationLog.WriteEventToLog("Path to Zoiper.sdf: " + connectionString);
            //return connectionString;
            // dbContext = new ZoiperdbContext(connectionString);
        }

        #endregion
    }
}
