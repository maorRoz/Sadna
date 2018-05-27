using System;
using System.Data.SqlClient;


namespace SadnaSrc.MarketData
{
    class ProxyMarketDB : IMarketDB
    {
        private readonly IMarketDB realMarketDB;

        public ProxyMarketDB()
        {
            try
            {
                realMarketDB = MarketDB.Instance;
            }
            catch (InvalidOperationException e)
            {
                if (!IsConnected(e.Message) || !MarketDB.ToDisable)
                {
                    throw;
                }
            }
            catch (SqlException e)
            {
                if (IsConnected(e.Message) || !MarketDB.ToDisable)
                {
                    throw;
                }
            }

        }

        public void InsertTable(string table, string tableColumns, string[] valuesNames, object[] values)
        {
            try
            {
                realMarketDB.InsertTable(table, tableColumns, valuesNames, values);
            }
            catch (InvalidOperationException e )
            {
                if (!IsConnected(e.Message) || MarketDB.ToDisable)
                {
                    throw new DataException();
                }

                throw;
            }
            catch (SqlException e)
            {
                if (!IsConnected(e.Message) || MarketDB.ToDisable)
                {
                    throw new DataException();
                }

                throw;
            }
        }

        public SqlDataReader SelectFromTable(string table, string toSelect)
        {
            try
            {
                return realMarketDB.SelectFromTable(table, toSelect);
            }
            catch (InvalidOperationException e)
            {
                if (!IsConnected(e.Message) || MarketDB.ToDisable)
                {
                    throw new DataException();
                }

                throw;
            }
            catch (SqlException e)
            {
                if (!IsConnected(e.Message) || MarketDB.ToDisable)
                {
                    throw new DataException();
                }

                throw;
            }
        }

        public SqlDataReader SelectFromTableWithCondition(string table, string toSelect, string condition)
        {
            try
            {
                return realMarketDB.SelectFromTableWithCondition(table, toSelect, condition);
            }
            catch (InvalidOperationException e)
            {
                if (!IsConnected(e.Message) || MarketDB.ToDisable)
                {
                    throw new DataException();
                }

                throw;
            }
            catch (SqlException e)
            {
                if (!IsConnected(e.Message) || MarketDB.ToDisable)
                {
                    throw new DataException();
                }

                throw;
            }
        }

        public void UpdateTable(string table, string updateCondition, string[] columnNames, string[] valuesNames, object[] values)
        {
            try
            {
                realMarketDB.UpdateTable(table, updateCondition, columnNames, valuesNames, values);
            }
            catch (InvalidOperationException)
            {
                if (!realMarketDB.IsConnected() || MarketDB.ToDisable)
                {
                    throw new DataException();
                }

                throw;
            }
            catch (SqlException e)
            {
                if (!IsConnected(e.Message) || MarketDB.ToDisable)
                {
                    throw new DataException();
                }

                throw;
            }
        }

        public void DeleteFromTable(string table, string deleteCondition)
        {
            try
            {
                realMarketDB.DeleteFromTable(table, deleteCondition);
            }
            catch (InvalidOperationException e)
            {
                if (!IsConnected(e.Message) || MarketDB.ToDisable)
                {
                    throw new DataException();
                }

                throw;
            }
            catch (SqlException e)
            {
                if (!IsConnected(e.Message) || MarketDB.ToDisable)
                {
                    throw new DataException();
                }

                throw;
            }
        }

        public SqlDataReader freeStyleSelect(string cmd)
        {
            try
            {
                return realMarketDB.freeStyleSelect(cmd);
            }
            catch (InvalidOperationException e)
            {
                if (!IsConnected(e.Message) || MarketDB.ToDisable)
                {
                    throw new DataException();
                }

                throw;
            }
            catch (SqlException e)
            {
                if (!IsConnected(e.Message) || MarketDB.ToDisable)
                {
                    throw new DataException();
                }

                throw;
            }
        }

        public bool IsConnected()
        {
            return realMarketDB.IsConnected();
        }

        private bool IsConnected(string message)
        {
            return IsConnected() &&
                   message != "The connection is broken and recovery is not possible.  The client driver attempted to recover the connection one or more times and all attempts failed.  Increase the value of ConnectRetryCount to increase the number of recovery attempts.";
        }
    }
}
