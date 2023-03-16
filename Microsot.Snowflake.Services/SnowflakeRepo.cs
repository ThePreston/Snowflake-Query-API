using Microsoft.Extensions.Logging;
using Snowflake.Data.Client;
using System.Data;

namespace Microsot.Snowflake.Services
{
    public class SnowflakeRepo : IRepoService
    {

        private readonly ILogger<SnowflakeRepo> _logger;

        private readonly SnowflakeDbConnection _conn;

        public SnowflakeRepo(ILogger<SnowflakeRepo> log, SnowflakeDbConnection conn)
        {
            _logger = log;
            _conn = conn;
        }

        public int Execute(string query)
        {
            _logger.LogInformation("Entered SnowflakeRepo.Execute");

            try
            {

                _conn.Open();

                using (IDbCommand cmd = _conn.CreateCommand())
                {
                    cmd.CommandText = query;

                    return cmd.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                _conn.Close();
            }

        }

        public IEnumerable<IDictionary<string, object>> GetData(string query)
        {
            _logger.LogInformation("Entered SnowflakeRepo.GetData");
            
            try
            {

                _conn.Open();

                using (IDbCommand cmd = _conn.CreateCommand())
                {
                    cmd.CommandText = query;

                    return ConvertToDictionary(cmd.ExecuteReader());
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                _conn.Close(); 
            }
            
        }

        private IEnumerable<Dictionary<string, object>> ConvertToDictionary(IDataReader reader)
        {
            var rows = new List<Dictionary<string, object>>();

            Func<IDataReader, IList<string>> GetColumns = rdr => {
                var cols = new List<string>();
                for (var i = 0; i < reader.FieldCount; i++)
                    cols.Add(reader.GetName(i));
                return cols;
            };

            var columns = GetColumns(reader);

            while (reader.Read())
                rows.Add(columns.ToDictionary(column => column, column => reader[column]));

            return rows;

        }
    }
}
