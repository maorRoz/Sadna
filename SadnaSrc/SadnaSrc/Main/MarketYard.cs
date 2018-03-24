﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.UserSpot;

namespace SadnaSrc.Main
{
    public class MarketYard
    {
        private SQLiteConnection _dbConnection;
        public MarketYard()
        {
            initiateDb();
        }

        private void initiateDb()
        {
            var programPath = System.AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "");
            programPath = programPath.Replace("\\bin\\Debug", "");
            string[] programPathParts = programPath.Split('\\');
            programPathParts[programPathParts.Length - 1] = "SadnaSrc\\";
            programPath = String.Join("\\", programPathParts);
            var dbPath = "URI=file:" + programPath + "MarketYardDB.db";
            _dbConnection = new SQLiteConnection(dbPath);
            _dbConnection.Open();
            MarketException.InsertDbConnector(_dbConnection);
            MarketLog.InsertDbConnector(_dbConnection);
        }

        public IUserService GetUserService()
        {
            return new UserService(_dbConnection);
        }

        public ISystemAdminService GetSystemAdminService()
        {
            return new UserService(_dbConnection);
        }
    }
}
