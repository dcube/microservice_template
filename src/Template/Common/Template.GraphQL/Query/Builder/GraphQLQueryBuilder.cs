using System.Reflection;
using System.Collections;
using System.Globalization;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

using Template.GraphQL.Extensions;

namespace Template.GraphQL.Query
{
    public class GraphQLQueryBuilder : GraphQLBuilder
    {
        private bool mutation;
        private Dictionary<string, GraphQLQueryObject> queries;

        public string Query 
        { 
            get
            {
                this.BuildQuery();

                return this.builder.ToString();
            }
        }

        public JObject Variables
        {
            get
            {
                return JObject.FromObject(parameters.ToDictionary(x => x.Key, x => x.Value.Value), JsonSerializer.Create(new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }));
            }
        }

        public GraphQLRequest Request
        {
            get
            {
                return new GraphQLRequest(Query, Variables);
            }
        }

        public GraphQLQueryBuilder(bool mutation = false) : base()
        {
            this.mutation = mutation;
            this.queries = new Dictionary<string, GraphQLQueryObject>();
        }

        public GraphQLQueryBuilder AddVariable(GraphQLParameter parameter)
        {
            this.parameters[parameter.Name] = parameter;

            return this;
        }

        public GraphQLQueryBuilder AddVariables(params GraphQLParameter[] parameters)
        {
            foreach (var parameter in parameters)
            {
                this.AddVariable(parameter);
            }

            return this;
        }

        public GraphQLQueryBuilder AddVariable(string name, GraphQLParameterType type, object value)
        {
            this.AddVariable(new GraphQLParameter() { Name = name, Type = type, Value = value });

            return this;
        }

        public GraphQLQueryBuilder AddQuery<T>(GraphQLQueryObject<T> queryObject) where T : class
        {
            var queryName = queryObject.HasAliasName() ? queryObject.AliasName : queryObject.Name;

            this.queries[queryName] = queryObject;

            return this;
        }

        private void BuildQuery()
        {
            int tabCount = 1;

            this.builder.Clear();

            this.builder.AppendLine(mutation ? "mutation" : "query");

            if (parameters.Any())
            {
                this.builder.Append(" (");
                var graphQLParameters = parameters
                    .Where(parameter => parameter.Value.Value != null)
                    .Select(parameter =>
                {
                    string type = parameter.Value.Type switch
                    {
                        GraphQLParameterType.INT => "Int!",
                        GraphQLParameterType.STRING => "String!",
                        GraphQLParameterType.DATETIME => "DateTime!",
                        GraphQLParameterType.BOOLEAN => "Boolean!",
                        GraphQLParameterType.STRING_ARRAY => "[String]!",
                        GraphQLParameterType.INT_ARRAY => "[Int!]!",
                        GraphQLParameterType.DATETIME_ARRAY => "[DateTime]!",
                        GraphQLParameterType.OBJECT => parameter.Value.Value.GetType().Name,
                        _ => throw new NotImplementedException()
                    };

                    return GetTabulation(tabCount) + $"${parameter.Key}: {type}";
                });

                this.builder.Append(string.Join("," + Environment.NewLine, graphQLParameters));
                this.builder.Append(")");
            }

            this.builder.AppendLine(" {");

            foreach (var query in queries)
            {
                this.builder.Append(GetTabulation(tabCount));

                if (query.Value.HasAliasName())
                {
                    this.builder.Append($"{query.Value.AliasName}: ");
                }

                this.builder.Append($"{query.Value.Name}(");
                
                if (!(query.Value.Arguments is null))
                {
                    this.AppendArguments(query.Value.Arguments);
                }
                this.builder.AppendLine(") {");

                this.AppendFields(query.Value.Fields, tabCount + 1);

                this.builder.Append(GetTabulation(tabCount));

                this.builder.AppendLine("}");
            }

            this.builder.AppendLine("}");
        }

        private void AppendArguments(object arguments, bool fromObject = false)
        {
            var properties = arguments.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                string key = property.Name;
                object value = property.GetValue(arguments, null);

                if (value.GetType().IsArray)
                {
                    this.builder.Append($"{key}: ");

                    var array = value as Array;
                    this.builder.Append("[ ");
                    for (int i = 0; i < array.Length; i++)
                    {
                        var item = array.GetValue(i);

                        if (fromObject == false)
                        {
                            this.builder.Append("{ ");
                            this.AppendArguments(item);
                            this.builder.Append(i == (array.Length - 1) ? " }" : " },");
                        }
                        else
                        {
                            string strValue = this.FormatQueryArgument(item);

                            this.builder.Append(strValue);
                            this.builder.Append(i == (array.Length - 1) ? "" : ", ");
                        }
                    }
                    this.builder.Append(" ]");
                }
                else if (value.GetType() != typeof(string) && value.GetType().IsClass)
                {
                    if (key.Equals("OR", StringComparison.InvariantCultureIgnoreCase) || key.Equals("AND", StringComparison.InvariantCultureIgnoreCase))
                    {
                        this.builder.Append($"{key.ToUpper()}: ");
                    }
                    else
                    {
                        this.builder.Append($"{key.ToCamelCase()}: ");
                    }
                    this.builder.Append("{ ");

                    this.AppendArguments(value, true);

                    this.builder.Append(" }");
                }
                else
                {
                    string argument = FormatQueryArgument(value);

                    if (argument != null)
                    {
                        this.builder.Append($"{key.ToCamelCase()}: ");
                        this.builder.Append(argument);
                    }
                }

                this.builder.Append(", ");
            }

            if (properties.Any())
            {
                this.builder.Length -= 2;
            }
        }

        private void AppendFields(Dictionary<string, GraphQLQueryObjectField> fields, int tabCount)
        {
            foreach (var field in fields)
            {
                this.builder.Append(GetTabulation(tabCount));

                if (field.Value.HasAliasName())
                {
                    this.builder.Append($"{field.Value.AliasName}: ");
                }

                if (field.Value.Fields.Count > 0)
                {
                    if (!(field.Value.Arguments is null))
                    {
                        this.builder.Append($"{field.Value.Name.ToCamelCase()} (");
                        this.AppendArguments(field.Value.Arguments);
                        this.builder.AppendLine(") {");
                    }
                    else
                    {
                        this.builder.AppendLine($"{field.Value.Name.ToCamelCase()} {{");
                    }


                    this.AppendFields(field.Value.Fields, tabCount + 1);

                    this.builder.Append(GetTabulation(tabCount));
                    this.builder.AppendLine("}");
                }
                else
                {
                    this.builder.AppendLine($"{field.Value.Name.ToCamelCase()}");
                }
            }
        }

        private string FormatQueryArgument(object value)
        {
            string formatEnumerableValue(IEnumerable value)
            {
                var items = new List<string>();

                foreach (var item in value)
                {
                    string argument = FormatQueryArgument(item);

                    if (argument != null)
                    {
                        items.Add(argument);
                    }
                }

                return $"[{string.Join(",", items)}]";
            };

            if (value is string && parameters.ContainsKey(value as string))
            {
                if (parameters[value as string].Value != null)
                {
                    return $"${value}";
                }

                return null;
            }

            return value switch
            {
                string strValue => "\"" + strValue + "\"",
                float floatValue => floatValue.ToString(CultureInfo.CreateSpecificCulture("en-us")),
                double doubleValue => doubleValue.ToString(CultureInfo.CreateSpecificCulture("en-us")),
                decimal decimalValue => decimalValue.ToString(CultureInfo.CreateSpecificCulture("en-us")),
                IEnumerable enumerableValue => formatEnumerableValue(enumerableValue),
                _ => value.ToString()
            };
        }
    }
}
