﻿using System.Linq.Expressions;
using System.Reflection;
using Template.GraphQL.Extensions;

namespace Template.GraphQL.Query
{
    public class GraphQLQueryObjectField
    {
        public string Name { get; private set; }

        public string AliasName { get; private set; }
        public object Arguments { get; set; }

        public Dictionary<string, GraphQLQueryObjectField> Fields { get; protected set; }

        protected GraphQLQueryObjectField()
        {
            this.Fields = new Dictionary<string, GraphQLQueryObjectField>();
        }

        public GraphQLQueryObjectField(string name, string aliasName) : this()
        {
            this.Name = name;

            if (!string.IsNullOrWhiteSpace(aliasName))
            {
                this.AliasName = aliasName;
            }
        }

        public bool HasAliasName()
        {
            return !string.IsNullOrWhiteSpace(this.AliasName);
        }

        public string GetPrincipalKey()
        {
            return this.HasAliasName() ? this.AliasName : this.Name;
        }
    }

    public class GraphQLQueryObjectField<T> : GraphQLQueryObjectField where T : class
    {

        public GraphQLQueryObjectField(string name, string aliasName) : base(name, aliasName)
        {
        }

        public GraphQLQueryObjectField<T> AddField<TProperty>(
            Expression<Func<T, TProperty>> propertySelector,
            string aliasName = null)
        {
            MemberExpression memberExpression = propertySelector.Body as MemberExpression;
            GraphQLQueryObjectField field = new GraphQLQueryObjectField(memberExpression.Member.Name, aliasName);

            this.Fields[field.GetPrincipalKey()] = field;

            return this;
        }

        public GraphQLQueryObjectField<T> AddEveryFields()
        {
            var properties = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => property.CanWrite && property.PropertyType.IsPrimitive());

            foreach (var property in properties)
            {
                this.Fields[property.Name] = new GraphQLQueryObjectField(property.Name, null);
            }

            return this;
        }

        public GraphQLQueryObjectField<T> AddField<TProperty>(
            Expression<Func<T, TProperty>> propertySelector,
            Func<GraphQLQueryObjectField<TProperty>, GraphQLQueryObjectField> complexPropertySelector,
            string aliasName = null) where TProperty : class
        {
            MemberExpression memberExpression = propertySelector.Body as MemberExpression;
            GraphQLQueryObjectField field = complexPropertySelector.Invoke(new GraphQLQueryObjectField<TProperty>(memberExpression.Member.Name, aliasName));

            this.Fields[field.GetPrincipalKey()] = field;

            return this;
        }

        public GraphQLQueryObjectField<T> AddCollectionField<TProperty>(
            Expression<Func<T, IEnumerable<TProperty>>> propertySelector,
            Func<GraphQLQueryObjectField<TProperty>, GraphQLQueryObjectField> complexPropertySelector,
            string aliasName = null) where TProperty : class
        {
            MemberExpression memberExpression = propertySelector.Body as MemberExpression;
            GraphQLQueryObjectField field = complexPropertySelector.Invoke(new GraphQLQueryObjectField<TProperty>(memberExpression.Member.Name, aliasName));

            this.Fields[field.GetPrincipalKey()] = field;

            return this;
        }

        public GraphQLQueryObjectField<T> AddField<TProperty, TArguments>(
            Expression<Func<T, IEnumerable<TProperty>>> propertySelector,
            TArguments arguments,
            Func<GraphQLQueryObjectField<TProperty>, GraphQLQueryObjectField> complexPropertySelector,
            string aliasName = null) 
                where TProperty : class 
                where TArguments : class
        {
            MemberExpression memberExpression = propertySelector.Body as MemberExpression;
            GraphQLQueryObjectField field = complexPropertySelector.Invoke(new GraphQLQueryObjectField<TProperty>(memberExpression.Member.Name, aliasName));

            this.Fields[field.GetPrincipalKey()] = field;
            field.Arguments = arguments;

            return this;
        }

    }
}
