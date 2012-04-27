using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Regalo.Core;

namespace Regalo.SqlServer
{
    public class SqlServerRepository<TAggregateRoot> : IRepository<TAggregateRoot> where TAggregateRoot : AggregateRoot, new()
    {
        private readonly string _connectionString;

        public SqlServerRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public TAggregateRoot Get(string id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT EventTypeName, EventData FROM Event WHERE AggregateId = @aggregateId";
                    command.CommandType = CommandType.Text;
                    SqlParameter idParameter = command.CreateParameter();
                    idParameter.ParameterName = "@aggregateId";
                    idParameter.SqlDbType = SqlDbType.VarChar;
                    idParameter.Value = id;

                    using (var reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (!reader.HasRows) return null;

                        var events = new List<Event>();

                        while (reader.Read())
                        {
                            var eventTypeName = (string)reader.GetValue(0);
                            var eventData = (string)reader.GetValue(1);
                            events.Add((Event)JsonConvert.DeserializeObject(eventData, Type.GetType(eventTypeName)));
                        }

                        var aggregateRoot = new TAggregateRoot();
                        aggregateRoot.ApplyAll(events);
                        return aggregateRoot;
                    }
                }
            }
        }

        public void Save(TAggregateRoot item)
        {
            throw new System.NotImplementedException();
        }
    }
}