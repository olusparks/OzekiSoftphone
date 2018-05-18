using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using zoiper_sdk.DataObjects;
using NLog;
using System.Reflection;
using System.IO;

namespace zoiper_sdk.DataOP
{
    /*Written By: Bantale Babajide
     Date:11/16/2017 */
    public static class DataOP
    {
        

        //static HttpClient httpClient;
        ////TODO : Add Company to array of contactProperties
        //static string[] contactProperties { get; set; } = { "fullname", "mobilephone", "telephone1", "contactid" };
        //static string[] accountProperties = { "name" };
        //static List<DataObjects.DbContact> contactList { get; set; }
        //static DataObjects.Contact contact { get; set; }
        //static DataObjects.DbContact dbContact { get; set; }
        //static string contactUrl { get; set; } = string.Empty;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static string connectionString = string.Empty; //@"C:\Users\jbant\Desktop\crm\ZoiperSoftPhone\zoiper_sdk\SqlData\Zoiperdb.sdf";
        private static ZoiperdbContext dbContext = null;

        //static string queryOptions;  //select, expand and filter clauses
        //Entity properties to select in a request and display.
        //static string[] contactProperties = { "fullname", "jobtitle", "annualincome" };
        //static string[] accountProperties = { "name" };
        //static string[] taskProperties = { "subject", "description" };
        //private static JObject retrievedContact1;
        //private static string contact1Uri;



        /// <summary>
        /// Get All Contacts from CRM
        /// </summary>
        /// <returns></returns>
        //public static List<DataObjects.DbContact> GetContacts()
        //{
        //    try
        //    {
        //        //TODO : Change api to reliance api
        //        string apiVersion = "api/data/v9.0";
        //        httpClient = ConnectToCRM.TheHttpClient(apiVersion); //ConnectToCRM.TheHttpClient(apiVersion);
        //        string expand = "&$expand=parentcustomerid_account($select=" + String.Join(",", accountProperties) + ")";
        //        string queryOperation = "?$select=" + String.Join(",", contactProperties) + expand;

        //        HttpResponseMessage retrieveResponse = httpClient.GetAsync(httpClient.BaseAddress + "/contacts" + queryOperation).Result;
        //        if (retrieveResponse.StatusCode == System.Net.HttpStatusCode.OK) // 200
        //        {
        //            logger.Info("retrieveResponse is OK");
        //            LibraryLog.WriteErrorLog("retrieveResponse is OK");

        //            dynamic retrievedContacts = JsonConvert.DeserializeObject(retrieveResponse.Content.ReadAsStringAsync().Result);
        //            logger.Info("Looping through values");
        //            LibraryLog.WriteErrorLog("Looping through values");

        //            //DataObjects.Contact ct1 = new DataObjects.Contact();
        //            contactList = new List<DataObjects.DbContact>();

        //            foreach (var item in retrievedContacts.value)
        //            {

        //                //Pass all values to Contact
        //                DataObjects.Contact cont = new DataObjects.Contact();
        //                cont.FullName = item.fullname.Value;
        //                cont.MobilePhone = item.mobilephone.Value;
        //                cont.BusinessPhone = item.telephone1.Value;
        //                cont.Company = item.parentcustomerid_account != null ? item.parentcustomerid_account.name.Value : "Nil";
        //                cont.ContactGuid = item.contactid.Value;

        //                logger.Info("Inside the loop");
        //                logger.Info($"FullName {item.fullname.Value}:  contactid {generateContactUrl(item.contactid.Value)}");
        //                logger.Info("After logging value to Nlog");

        //                //logger.Info("populate contact");
        //                //ct1.FullName = item.fullname.Value;
        //                //ct1.Url = generateContactUrl(item.contactid.Value);
        //                //logger.Info("After populate contact");
        //                //contact = new DataObjects.Contact()
        //                //{
        //                //    FullName = item.fullname.Value,
        //                //    Url = generateContactUrl(item.contactid.Value)
        //                //};
        //                //logger.Info("Add to contactlist");
        //                //contactList.Add(ct1);
        //                //logger.Info("After Addition to contactlist");

        //                //string mobile = item.mobilephone.Value;

        //                //Check whether mobile phone contains value
        //                if (!string.IsNullOrEmpty(cont.MobilePhone))
        //                {
        //                    //TODO : Add Company using $expand
        //                    dbContact = new DbContact(cont.FullName, cont.MobilePhone, cont.Company, generateContactUrl(cont.ContactGuid));

        //                    //Add mobile phone values
        //                    //contact = new DataObjects.Contact()
        //                    //{
        //                    //    FullName = item.fullname.Value,
        //                    //    Phone = item.mobilephone.Value,
        //                    //    //Company = item.company.Value,
        //                    //    Url = generateContactUrl(item.contactid.Value)
        //                    //};
        //                    logger.Info("Add to contactlist");
        //                    logger.Info("Mobile phone is not empty: Contact Added");
        //                    contactList.Add(dbContact);
        //                    LibraryLog.WriteErrorLog("Mobile phone is not empty: Contact Added " + dbContact.Url);
        //                    logger.Info("After Addition to contactlist");

        //                }

        //                //Check whether business contains value
        //                if (!string.IsNullOrEmpty(cont.BusinessPhone))
        //                {
        //                    //TODO : Add Company using $expand
        //                    dbContact = new DbContact(cont.FullName, cont.BusinessPhone, cont.Company, generateContactUrl(cont.ContactGuid));
        //                    //contact = new DataObjects.Contact()
        //                    //{
        //                    //    FullName = item.fullname.Value,
        //                    //    Phone = item.telephone1.Value,
        //                    //    Company = item.company.Value,
        //                    //    Url = generateContactUrl(item.contactid.Value)
        //                    //};
        //                    logger.Info("Add to contactlist");
        //                    logger.Info("Business phone is not empty: Contact Added");
        //                    contactList.Add(dbContact);
        //                    LibraryLog.WriteErrorLog("Business phone is not empty: Contact Added: " + dbContact.Url);
        //                    logger.Info("After Addition to contactlist");
        //                }
        //            }
        //        }
        //        LibraryLog.WriteErrorLog("Loop is over");
        //        logger.Info("Loop is over");
        //        return contactList;
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex, ex.Message);
        //        LibraryLog.WriteErrorLog(ex);
        //        return contactList;
        //    }

        //}

        /// <summary>
        /// Generate Url for Conatcts
        /// </summary>
        /// <param name="contactGuid"></param>
        /// <returns></returns>
        //private static string generateContactUrl(string contactGuid)
        //{
        //    return $"https://abisoyeltd.crm4.dynamics.com/main.aspx?etn=contact&pagetype=entityrecord&id=%7B{contactGuid}%7D";
        //}

        /// <summary>
        /// Insert contacts into the DB
        /// </summary>
        /// <param name="contact"></param>
        //public static void InsertContactData(List<DbContact> contact)
        //{
        //    connectionString = GetAppPath(@"\Zoiperdb.sdf"); //GetPath(@"..\..\SqlData\Zoiperdb.sdf");
        //    dbContext = new ZoiperdbContext(connectionString);
        //    try
        //    {

        //        foreach (var item in contact)
        //        {
        //            TblPhoneContact tblContact = new TblPhoneContact()
        //            {
        //                ContactName = item.FullName,
        //                Organization = item.Company,
        //                Telephone = item.Phone,
        //                Url = item.Url,
        //                EntryDate = DateTime.Now
        //            };

        //            //dbContext.Tblphonecontact.InsertAllOnSubmit()
        //            dbContext.TblPhoneContact.InsertOnSubmit(tblContact);
        //            dbContext.SubmitChanges();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LibraryLog.WriteErrorLog(ex.Message);
        //    }

        //}

        /// <summary>
        /// Get contact by phone
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns>DbContact</returns>
        public static DbContact GetContactByPhone(string phoneNumber)
        {
            //connectionString = GetAppPath(@"\Zoiperdb.sdf"); // GetPath(@"..\..\SqlData\Zoiperdb.sdf");
           
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
        /// Get DB Path
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public static string GetPath(string imagePath)
        {
            string exeFile = new System.Uri(Assembly.GetEntryAssembly().CodeBase).AbsolutePath;
            string Dir = Path.GetDirectoryName(exeFile);
            string path = Path.GetFullPath(Path.Combine(Dir, imagePath));
            return path;
        }

        public static string GetAppPath(string imagePath)
        {
            string Dir2 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string path2 = Dir2 + imagePath;
            return path2;
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
                    //connectionString = GetAppPath(@"\Zoiperdb.sdf"); //GetPath(@"..\..\SqlData\Zoiperdb.sdf");
                    dbContext = new ZoiperdbContext(AuthenticateConnectionToDB());
                    ApplicationLog.WriteEventToLog("Path to ZoiperDB from SaveSettings: "+ AuthenticateConnectionToDB());
                    TblZoiperSetting zoiperSetting = new TblZoiperSetting()
                    {
                        Domain = settings.Domain,
                        Username = settings.Username,
                        CallerId = settings.CallerId,
                        ServiceDuration = settings.ServiceDuration,
                        IsRestart = Convert.ToInt32(settings.Restart),
                        Password = SharedFunctions.Hash(settings.Password)
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

        public static int GetServiceDuration()
        {
            connectionString = GetAppPath(@"\Zoiperdb.sdf"); //GetPath(@"..\..\SqlData\Zoiperdb.sdf");
            dbContext = new ZoiperdbContext(connectionString);
            int duration = 0;
            try
            {
                TblZoiperSetting setting = dbContext.TblZoiperSetting.FirstOrDefault();
                duration= setting.ServiceDuration;
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex?.Message);
                LibraryLog.WriteErrorLog(ex);
                duration = Convert.ToInt32(false);
            }
            return duration;
        }

        //public static bool TruncateTable()
        //{
        //    connectionString = GetAppPath(@"\Zoiperdb.sdf");
        //    string query = "DELETE from TblPhoneContact";
        //    string dropConstraint = @"ALTER TABLE [TblPhoneContact] DROP CONSTRAINT [PK__TblPhoneContact__0000000000000019]";
        //    string dropIdColumn = "ALTER TABLE[TblPhoneContact] DROP COLUMN[Id]";

        //    string addIdColumn = @"ALTER TABLE [TblPhoneContact] ADD [Id] int IDENTITY (1,1)  NOT NULL";
        //     string addIdConstraint = "ALTER TABLE[TblPhoneContact] ADD CONSTRAINT[PK__TblPhoneContact__0000000000000019] PRIMARY KEY ([Id])";

        //    bool result = false;
        //    try
        //    {
        //        dbContext = new ZoiperdbContext(connectionString);
                
        //        if (dbContext.DatabaseExists())
        //        {
                    

        //            int idropConstraint = dbContext.ExecuteCommand(dropConstraint);
        //            if (idropConstraint >= 0)
        //                result = true;

        //            int idropIdColumn = dbContext.ExecuteCommand(dropIdColumn);
        //            if (idropIdColumn >= 0)
        //                result = true;

        //            int iQuery = dbContext.ExecuteCommand(query);
        //            if (iQuery >= 0)
        //                result = true;

        //            int iaddIdColumn = dbContext.ExecuteCommand(addIdColumn);
        //            int iaddIdConstraint = dbContext.ExecuteCommand(addIdConstraint);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LibraryLog.WriteErrorLog(ex);
        //    }
        //    return result;
        //}

        /// <summary>
        /// Get User account for calling
        /// </summary>
        /// <returns></returns>
        public static string GetSettings()
        {
            string account = string.Empty;
            try
            {
                //AuthenticateConnectionToDB();
                dbContext = new ZoiperdbContext(AuthenticateConnectionToDB());
                TblZoiperSetting tblSetting = dbContext.TblZoiperSetting.FirstOrDefault();
                account = tblSetting.CallerId + "@" + tblSetting.Domain;
            }
            catch (Exception ex)
            {
                LibraryLog.WriteErrorLog(ex);
            }
            return account;
        }

        /// <summary>
        /// 
        /// </summary>
        private static string AuthenticateConnectionToDB()
        {
            connectionString = GetAppPath(@"\Zoiperdb.sdf");
            ApplicationLog.WriteEventToLog("Path to Zoiper.sdf: " + connectionString);
            return connectionString;
           // dbContext = new ZoiperdbContext(connectionString);
        }
        /// <summary>
        /// Retrieve Operation: Using Web API
        /// </summary>
        /// <returns></returns>
        //public static DataObjects.Contact BasicRetrieve(HttpClient httpClient, string phoneNumber)
        //{
        //    //Create properties to retrieve, query string, httpRequestMessage, httpResponseMessage, 
        //    //ChangeColor("---Basic Query---");
        //    string[] contactProperties = { "fullname", "jobtitle", "annualincome", "telephone1", "mobilephone" };
        //    string filter = @"&$filter=telephone1 eq '814'";
        //    //string filter = @"&$filter=contains(telephone1,'(814)')"; //@"&$filter=contains(telephone1 '(" + phoneNumber + ")' ) ";//+ phoneNumber; // + " or " + "mobilephone eq " + phoneNumber; //String.Join(",", contactProperties);
        //    string queryOperation = "?$select=" + String.Join(",", contactProperties) + filter;

        //    //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, httpClient.BaseAddress + "/contacts" + queryOperation);

        //    HttpResponseMessage retrieveResponse = httpClient.GetAsync(httpClient.BaseAddress + "/contacts" + queryOperation).Result;
        //    if (retrieveResponse.StatusCode == HttpStatusCode.OK) //200
        //    {
        //        dynamic retrievedContact = JsonConvert.DeserializeObject(retrieveResponse.Content.ReadAsStringAsync().Result);
        //        DataObjects.Contact contact = new DataObjects.Contact();
        //        //contact.FullName = retrievedContact.value[1].Value;
        //        foreach (var item in retrievedContact.value)
        //        {
        //            contact.FullName = item.fullname.Value;
        //        }
        //        // contact.FullName = retrievedContact1.GetValue("fullname").ToString();
        //        //contact.FullName = retrievedContact1["fullname"].ToString();
        //        //contact.Address = retrievedContact1["annualincome"].ToString();
        //        //contact.MobilePhone = retrievedContact1["mobilephone"].ToString();
        //        //contact.BusinessPhone = retrievedContact1["telephone1"].ToString();

        //        return contact;


        //        //Display retrieved values
        //        //MessageBox.Show()
        //        //Console.WriteLine("Fullname:{0}  \n AnnualIncome: {1}  \n Job Title: {2}  \n Description: {3} ", retrievedContact1.GetValue("fullname"), retrievedContact1["annualincome"], retrievedContact1["jobtitle"], retrievedContact1["description"]);
        //    }
        //    else
        //    {
        //        //ErrorColor(string.Format("Failed to create contact for reason: {0}", retrieveResponse.ReasonPhrase));
        //        throw new CrmHttpException(retrieveResponse.Content);
        //    }
        //}


        /// <summary>
        /// Retrieve Operation: Using Organization Service (SOAP)
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        //public static DataObjects.Contact RetrieveContact(string phoneNumber)
        //{
        //    ClientCredentials cred = getCredentials();
        //    uri = new Uri(urlName);
        //    DataObjects.Contact contact = new DataObjects.Contact();

        //    using (_serviceProxy = new OrganizationServiceProxy(uri, homeRealmUri, cred, deviceCredentials))
        //    {
        //        _serviceProxy.EnableProxyTypes();
        //        _service = (IOrganizationService)_serviceProxy;
        //        using (AbisoyeServiceContext context = new AbisoyeServiceContext(_service))
        //        {
        //            var queryContact = (from c in context.ContactSet
        //                                where c.Telephone1 == phoneNumber || c.MobilePhone == phoneNumber
        //                                select new
        //                                {
        //                                    fullname = c.FullName,
        //                                    email = c.EMailAddress1,
        //                                    jobtitle = c.JobTitle,
        //                                    mobilephone = c.MobilePhone,
        //                                    businessphone = c.Telephone1,
        //                                    contactGuid = c.ContactId.Value
        //                                }).FirstOrDefault();

        //            contact.FullName = queryContact.fullname;
        //            contact.MobilePhone = queryContact.mobilephone;
        //            contact.BusinessPhone = queryContact.businessphone;
        //        }
        //    }
        //    return contact;
        //}

        //Get Login Details
        //private static ClientCredentials getCredentials()
        //{
        //    ClientCredentials cred = new ClientCredentials();
        //    cred.UserName.UserName = "abisoyes@abisoyeltd.onmicrosoft.com";
        //    cred.UserName.Password = "Adejumoke1!";
        //    return cred;
        //}
    }
}
