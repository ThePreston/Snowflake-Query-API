namespace Microsot.Snowflake.Services
{
    public interface IRepoService 
    {
        public IEnumerable<IDictionary<string, object>> GetData(string query);

    }
}