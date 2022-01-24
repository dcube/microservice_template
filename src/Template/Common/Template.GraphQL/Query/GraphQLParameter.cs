﻿namespace Template.GraphQL.Query
{
    public enum GraphQLParameterType
    {
        INT,
        STRING,
        DATETIME,
        BOOLEAN,
        STRING_ARRAY,
        INT_ARRAY,
        DATETIME_ARRAY,
        OBJECT
    }

    public class GraphQLParameter
    {
        public string Name { get; set; }

        public object Value { get; set; }

        public GraphQLParameterType Type { get; set; }
    }
}
