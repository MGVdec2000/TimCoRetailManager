using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMDataManager.Library.Internal.DataAccess
{
    internal class SqlDataAccess : IDisposable
    {
        public string GetConnectionString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        public List<T> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);
            using (IDbConnection cnn = new SqlConnection(connectionString))
            {
                List<T> rows = cnn.Query<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure).ToList();

                return rows;
            }
        }

        public int SaveData<T>(string storedProcedure, T parameters, string connectionStringName)
        {
            int returnId = 0;
            string connectionString = GetConnectionString(connectionStringName);
            using (IDbConnection cnn = new SqlConnection(connectionString))
            {
                returnId = cnn.ExecuteScalar<int>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
            return returnId;
        }

        private IDbConnection _connection;
        private IDbTransaction _transaction;

        public void StartTransaction(string connectionStringName)
        {
            _connection = new SqlConnection(GetConnectionString(connectionStringName));
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        public void CommitTransaction()
        {
            try
            {
                _transaction?.Commit();
            }
            catch (InvalidOperationException ex)
            {
                // This is the expected message if the transaction was already commited or rolled back
                if (ex.Message != "This SqlTransaction has completed; it is no longer usable.")
                {
                    // Otherwise, re-throw the exception
                    throw;
                }
            }
            if (_connection.State != ConnectionState.Closed)
            {
                _connection?.Close();
            }
        }

        public void RollbackTransaction()
        {
            _transaction?.Rollback();
            if (_connection.State != ConnectionState.Closed)
            {
                _connection?.Close();
            }
        }

        public void Dispose()
        {
            CommitTransaction();
        }

        public int SaveDataInTransaction<T>(string storedProcedure, T parameters)
        {
            return _connection.ExecuteScalar<int>(storedProcedure, parameters, commandType: CommandType.StoredProcedure, transaction: _transaction);
        }

        public List<T> LoadDataInTransaction<T, U>(string storedProcedure, U parameters)
        {
            List<T> rows = _connection.Query<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure, transaction: _transaction).ToList();

            return rows;
        }

        // Open Connection/Start Transaction
        // Load using the transaction
        // Save using the transaction
        // Complete transaction/Close connection
        // Dispose
    }
}
