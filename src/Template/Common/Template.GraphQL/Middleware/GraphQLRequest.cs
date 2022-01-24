using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Template.GraphQL
{
    public class GraphQLRequest
    {
        [JsonProperty("query")]
        public string Query { get; set; }

        [JsonProperty("variables")]
        public JObject Variables { get; set; }

        public GraphQLRequest()
        {

        }

        public GraphQLRequest(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentException("L'argument 'query' ne peut pas être null");
            }

            this.Query = query;
        }

        public GraphQLRequest(string query, JObject variables) : this(query)
        {
            this.Variables = variables;
        }
    }
}
