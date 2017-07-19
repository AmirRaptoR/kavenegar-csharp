using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kavenegar.NetStandard.Models;
using Kavenegar.NetStandard.Models.Enums;
using Kavenegar.NetStandard.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Refit;

namespace Kavenegar.NetStandard
{
    internal class SendRequest
    {
        public string Sender { get; set; }
        public string Receptor { get; set; }
        public string Message { get; set; }
        public long Date { get; set; }
        public MessageType Type { get; set; }
        public IEnumerable<string> LocalIds { get; set; }
    }

    internal class SendArrayRequest
    {
        public IEnumerable<string> Senders { get; set; }
        public IEnumerable<string> Receptors { get; set; }
        public IEnumerable<string> Messages { get; set; }
        public DateTime Date { get; set; }
        public IEnumerable<MessageType> Types { get; set; }
        public IEnumerable<string> LocalIds { get; set; }
    }

    internal interface IKavenegarApiJson
    {
        [Post("/sms/send.json")]
        Task<SendResult> Send([Body(BodySerializationMethod.UrlEncoded)] SendRequest request);

        [Post("/sms/sendarray.json")]
        Task<IEnumerable<SendResult>> SendArray([Body(BodySerializationMethod.UrlEncoded)] SendArrayRequest request);
    }

    public class KavenegarApi
    {
        private const string Apipath = "http://api.kavenegar.com/v1/{0}";

        private readonly IKavenegarApiJson _restService;

        public KavenegarApi(string apikey)
        {
            ApiKey = apikey;
            _restService = RestService.For<IKavenegarApiJson>(string.Format(Apipath, apikey),
                new RefitSettings
                {
                    JsonSerializerSettings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }
                });
        }

        public string ApiKey { get; }

        public async Task<SendResult> Send(string sender, IEnumerable<string> receptors, string message)
        {
            return await Send(sender, receptors, message, MessageType.MobileMemory, DateTime.MinValue);
        }

        public async Task<SendResult> SendAsync(string sender, string receptor, string message)
        {
            return await Send(sender, receptor, message, MessageType.MobileMemory, DateTime.MinValue);
        }

        public async Task<SendResult> Send(string sender, string receptor, string message, MessageType type,
            DateTime date)
        {
            var receptors = new List<string> { receptor };
            return await Send(sender, receptors, message, type, date);
        }

        public async Task<SendResult> Send(string sender, IEnumerable<string> receptors, string message, MessageType type,
            DateTime date)
        {
            return await Send(sender, receptors, message, type, date, null);
        }

        public async Task<SendResult> Send(string sender, string receptor, string message, MessageType type,
            DateTime date, string localid)
        {
            var receptors = new List<string> { receptor };
            var localids = new List<string> { localid };
            return await Send(sender, receptors, message, type, date, localids);
        }

        public async Task<SendResult> Send(string sender, string receptor, string message, string localid)
        {
            return await Send(sender, receptor, message, MessageType.MobileMemory, DateTime.MinValue, localid);
        }

        public async Task<SendResult> Send(string sender, List<string> receptors, string message, string localid)
        {
            var localids = new List<string>();
            for (var i = 0; i <= receptors.Count - 1; i++)
                localids.Add(localid);
            return await Send(sender, receptors, message, MessageType.MobileMemory, DateTime.MinValue, localids);
        }

        public async Task<SendResult> Send(string sender, IEnumerable<string> receptors, string message, MessageType type,
            DateTime date, IEnumerable<string> localids)
        {
            return await _restService.Send(new SendRequest
            {
                Date = date.ToUnixTimestamp(),
                Sender = sender,
                LocalIds = localids,
                Message = message,
                Receptor = string.Join(",", receptors),
                Type = type
            });
        }

        public async Task<IEnumerable<SendResult>> SendArray(IEnumerable<string> senders, IEnumerable<string> receptors,
            IEnumerable<string> messages)
        {
            var receptorsList = receptors.ToList();
            var types = Enumerable.Range(0, receptorsList.Count).Select(x => MessageType.MobileMemory);
            return await SendArray(senders, receptorsList, messages, types, DateTime.MinValue, null);
        }

        public async Task<IEnumerable<SendResult>> SendArray(string sender, IEnumerable<string> receptors,
            IEnumerable<string> messages,
            MessageType type, DateTime date)
        {
            var receptorsList = receptors.ToList();
            var senders = Enumerable.Range(0, receptorsList.Count).Select(x => sender);
            var types = Enumerable.Range(0, receptorsList.Count).Select(x => MessageType.MobileMemory);
            return await SendArray(senders, receptorsList, messages, types, date, null);
        }

        public async Task<IEnumerable<SendResult>> SendArray(string sender, IEnumerable<string> receptors,
            IEnumerable<string> messages,
            MessageType type, DateTime date, string localmessageids)
        {
            var receptorsList = receptors.ToList();
            var senders = Enumerable.Range(0, receptorsList.Count).Select(x => sender);
            var types = Enumerable.Range(0, receptorsList.Count).Select(x => MessageType.MobileMemory);
            return await SendArray(senders, receptorsList, messages, types, date, new List<string> { localmessageids });
        }

        public async Task<IEnumerable<SendResult>> SendArray(string sender, IEnumerable<string> receptors,
            IEnumerable<string> messages,
            string localmessageid)
        {
            var receptorsList = receptors.ToList();
            var senders = Enumerable.Range(0, receptorsList.Count).Select(x => sender);

            return await SendArray(senders, receptorsList, messages, localmessageid);
        }

        public async Task<IEnumerable<SendResult>> SendArray(IEnumerable<string> senders, IEnumerable<string> receptors,
            IEnumerable<string> messages,
            string localmessageid)
        {
            var receptorsList = receptors.ToList();
            var types = Enumerable.Range(0, receptorsList.Count).Select(x => MessageType.MobileMemory);
            var localmessageids = Enumerable.Range(0, receptorsList.Count).Select(x => localmessageid);
            return await SendArray(senders, receptorsList, messages, types, DateTime.MinValue, localmessageids);
        }

        public async Task<IEnumerable<SendResult>> SendArray(IEnumerable<string> senders, IEnumerable<string> receptors,
            IEnumerable<string> messages,
            IEnumerable<MessageType> types, DateTime date, IEnumerable<string> localmessageids)
        {
            return await _restService.SendArray(new SendArrayRequest
            {
                Date = date,
                LocalIds = localmessageids,
                Messages = messages,
                Receptors = receptors,
                Senders = senders,
                Types = types
            });
        }
    }
}