using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketServer.Models;

namespace MarketWeb.Models
{
    public class ErrorLogModel : UserModel
    {
        public ErrorLogItemModel[] Items { get; set; }
        public ErrorLogModel(int systemId, string state, string[] logEntries) : base(systemId, state, null)
        {
            Items = new ErrorLogItemModel[logEntries.Length];
            for (int i = 0; i < Items.Length; i++)
            {
                Items[i] = new ErrorLogItemModel(logEntries[i]);
            }
        }

        public class ErrorLogItemModel
        {
            public string Id { get; set; }
            public string Date { get; set; }
            public string Type { get; set; }
            public string Description { get; set; }

            public ErrorLogItemModel(string data)
            {
                var dataParam = data.Split(new[] { "ID: ", " Date: ", " Type: ", " Description: "}, StringSplitOptions.RemoveEmptyEntries);
                Id = dataParam[0];
                Date = dataParam[1];
                Type = dataParam[2];
                Description = dataParam[3];
            }
        }
    }
}
