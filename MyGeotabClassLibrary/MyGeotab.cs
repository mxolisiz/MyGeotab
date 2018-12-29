using Geotab.Checkmate;
using Geotab.Checkmate.ObjectModel;
using Geotab.Checkmate.ObjectModel.Engine;
using Geotab.Checkmate.ObjectModel.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace MyGeotabClassLibrary
{
    public class MyGeotab
    {
        private User loggedInUser = (User)null;
        private API _api;

        public User LoggedInUser
        {
            get
            {
                return this.loggedInUser;
            }
            set
            {
                this.loggedInUser = value;
            }
        }

        public MyGeotab(MyCredentials _myCredentials)
        {
            this._api = new API(_myCredentials.UserName, _myCredentials.Password, _myCredentials.SessionId, _myCredentials.Database, _myCredentials.Server);
            if (!_myCredentials.Authenticate)
                return;
            this.GetLoggedInUser();
        }

        public API GetApi(bool authenticateAPI = false)
        {
            if (authenticateAPI)
                this.GetLoggedInUser();
            return this._api;
        }

        public List<Device> GetDevices(DeviceSearch deviceSearch = null, bool includeHistoric = true)
        {
            if (includeHistoric)
                return _api.Call<List<Device>>("Get", typeof(Device));
            return _api.Call<List<Device>>("Get", typeof(Device)).Where(d => d.IsActive()).ToList();
        }

        public List<Zone> GetCachedZones()
        {
            MemoryCache @default = MemoryCache.Default;
            string key = "AllZones_" + this._api.Database;
            if (@default.Contains(key, (string)null))
                return (List<Zone>)@default.Get(key, (string)null);
            DateTimeOffset absoluteExpiration = DateTimeOffset.Now.AddHours(24.0);
            List<Zone> list1 = new List<Zone>();
            List<Zone> list2 = (List<Zone>)this._api.Call<List<Zone>>("Get", typeof(Zone), (object)null);
            @default.Add(key, (object)list2, absoluteExpiration, (string)null);
            return list2;
        }

        public List<Device> GetCachedDevices()
        {
            MemoryCache @default = MemoryCache.Default;
            string key = "AllDevices_" + this._api.Database;
            if (@default.Contains(key, (string)null))
                return (List<Device>)@default.Get(key, (string)null);
            DateTimeOffset absoluteExpiration = DateTimeOffset.Now.AddHours(24.0);
            List<Device> list1 = new List<Device>();
            List<Device> list2 = (List<Device>)this._api.Call<List<Device>>("Get", typeof(Device), (object)null);
            @default.Add(key, (object)list2, absoluteExpiration, (string)null);
            return list2;
        }

        public DateTime? ToUserDate(DateTime UtcDateValue, User user = null)
        {
            if (this.loggedInUser == null && user == null)
                this.GetLoggedInUser();
            System.TimeZoneInfo systemTimeZoneById = System.TimeZoneInfo.FindSystemTimeZoneById(Geotab.Checkmate.ObjectModel.TimeZoneInfo.OlsonToMachine(this.loggedInUser != null ? this.loggedInUser.TimeZoneId : (user == null ? "" : user.TimeZoneId)));
            return new DateTime?(System.TimeZoneInfo.ConvertTimeFromUtc(UtcDateValue, systemTimeZoneById));
        }

        public FeedResult<LogRecord> GetLogRecordsFeed(LogRecordSearch logRecordSearch, int resultsLimit = 5000)
        {
            return (FeedResult<LogRecord>)this._api.Call<FeedResult<LogRecord>>("GetFeed", typeof(LogRecord), (object)new
            {
                search = logRecordSearch,
                resultsLimit = resultsLimit
            });
        }

        public FeedResult<StatusData> GetStatusDataFeed(StatusDataSearch statusDataSearch, int resultsLimit = 5000)
        {
            return (FeedResult<StatusData>)this._api.Call<FeedResult<StatusData>>("GetFeed", typeof(StatusData), (object)new
            {
                resultsLimit = resultsLimit,
                search = statusDataSearch
            });
        }

        public Id SendTextMessage(TextMessage textMessage)
        {
            return (Id)this._api.Call<Id>("Add", typeof(TextMessage), (object)new
            {
                entity = textMessage
            });
        }

        public Id AddZone(Zone zone)
        {
            return (Id)this._api.Call<Id>("Add", typeof(Zone), (object)new
            {
                entity = zone
            });
        }

        public FeedResult<Trip> GetTripsFeed(TripSearch tripSearch = null, int resultsLimit = 5000)
        {
            return (FeedResult<Trip>)this._api.Call<FeedResult<Trip>>("GetFeed", typeof(Trip), (object)new
            {
                resultsLimit = resultsLimit,
                search = tripSearch
            });
        }

        public FeedResult<ExceptionEvent> GetExceptionEventsFeed(object feedRequestParams)
        {
            return (FeedResult<ExceptionEvent>)this._api.Call<FeedResult<ExceptionEvent>>("GetFeed", typeof(ExceptionEvent), feedRequestParams);
        }

        public List<DeviceStatusInfo> GetDeviceStatusInfo(DeviceStatusInfoSearch deviceStatusInfoSearch = null)
        {
            return (List<DeviceStatusInfo>)this._api.Call<List<DeviceStatusInfo>>("Get", typeof(DeviceStatusInfo), (object)new
            {
                search = deviceStatusInfoSearch
            });
        }

        public List<object> MakeMultiCall(object[] calls)
        {
            return this._api.MultiCall(calls);
        }

        public User GetLoggedInUser()
        {
            if (this.LoggedInUser == null)
            {
                API api = this._api;
                string str = "Get";
                Type type = typeof(User);
                UserSearch userSearch = new UserSearch();
                userSearch.Name=this._api.UserName;
                var fAnonymousType0 = new
                {
                    search = userSearch
                };
                this.LoggedInUser = Enumerable.FirstOrDefault<User>((IEnumerable<User>)api.Call<List<User>>(str, type, (object)fAnonymousType0));
            }
            return this.LoggedInUser;
        }

        public List<Rule> GetRules(RuleSearch ruleSearch = null)
        {
            return (List<Rule>)this._api.Call<List<Rule>>("Get", typeof(Rule), (object)new
            {
                search = ruleSearch
            });
        }

        public List<Rule> GetCachedRules()
        {
            MemoryCache @default = MemoryCache.Default;
            string key = "AllRules_" + this._api.Database;
            if (@default.Contains(key, (string)null))
                return (List<Rule>)@default.Get(key, (string)null);
            DateTimeOffset absoluteExpiration = DateTimeOffset.Now.AddDays(7.0);
            List<Rule> list1 = new List<Rule>();
            List<Rule> list2 = (List<Rule>)this._api.Call<List<Rule>>("Get", typeof(Rule), (object)null);
            @default.Add(key, (object)list2, absoluteExpiration, (string)null);
            return list2;
        }

        public List<Trip> GetTrips(TripSearch tripSearch = null)
        {
            return (List<Trip>)this._api.Call<List<Trip>>("Get", typeof(Trip), (object)new
            {
                search = tripSearch
            });
        }

        public List<TextMessage> GetTextMessages(TextMessageSearch textMessageSearch = null)
        {
            return (List<TextMessage>)this._api.Call<List<TextMessage>>("Get", typeof(TextMessage), (object)new
            {
                search = textMessageSearch
            });
        }

        public List<Diagnostic> GetDiagnostics(DiagnosticSearch diagnosticSearch = null)
        {
            return (List<Diagnostic>)this._api.Call<List<Diagnostic>>("Get", typeof(Diagnostic), (object)new
            {
                search = diagnosticSearch
            });
        }

        public List<LogRecord> GetLogRecords(LogRecordSearch logRecordSearch)
        {
            return (List<LogRecord>)this._api.Call<List<LogRecord>>("Get", typeof(LogRecord), (object)new
            {
                search = logRecordSearch
            });
        }

        public List<Group> GetGroups(GroupSearch groupSearch = null)
        {
            return (List<Group>)this._api.Call<List<Group>>("Get", typeof(Group), (object)new
            {
                search = groupSearch
            });
        }

        public List<Zone> GetZones(ZoneSearch zoneSearch = null)
        {
            return (List<Zone>)this._api.Call<List<Zone>>("Get", typeof(Zone), (object)new
            {
                search = zoneSearch
            });
        }

        public List<User> GetUsers(UserSearch userSearch = null)
        {
            return (List<User>)this._api.Call<List<User>>("Get", typeof(User), (object)new
            {
                search = userSearch
            });
        }
    }

    public class MyCredentials: Credentials
    {
        public string Server { get; set; }
        /// <summary>
        /// Passing this as true will attempt to authenticate the MyGeotab API with the supplied credentials
        /// </summary>
        public bool Authenticate { get; set; }
    }
}
