using aUI.Automation.Enums;
using System.Collections.Generic;
using System.ComponentModel;

namespace aUI.Automation.DbObjects
{
    public enum JoinType
    {
        [DefaultValue("")] Std,
        [DefaultValue("LEFT ")] Left,
        [DefaultValue("RIGHT ")] Right,
        [DefaultValue("FULL OUTER ")] Full,
        [DefaultValue("INNER ")] Inner,
    }
    public class QueryBuilder
    {
        public string Query;
        public string Table = "";
        public List<string> Fields = new();
        public List<(JoinType type, string table, string on)> Joins = new();
        public List<(string clause, bool and)> Where = new();
        public List<string> Group = new();
        public List<string> Order = new();
        public string Limit = "";

        //create a few constructors
        public QueryBuilder() { }

        public QueryBuilder(string query)
        {
            Query = query;
        }

        public void BuildQuery()
        {
            Query = $"SELECT {string.Join(", ", Fields)} FROM {Table}";

            //go through joins
            foreach (var (type, table, on) in Joins)
            {
                Query += $" {type.DefaultValue()}JOIN {table}";
                if (!string.IsNullOrEmpty(on))
                {
                    Query += $" ON {on}";
                }
            }

            //go through where
            bool first = true;
            foreach (var where in Where)
            {
                if (first)
                {
                    first = false;
                    Query += $" Where {where.clause}";
                    continue;
                }
                var clause = where.and ? "and" : "or";

                Query += $" {clause} {where.clause}";
            }

            //go through group
            first = true;
            foreach (var grp in Group)
            {
                if (first)
                {
                    first = false;
                    Query += $" GROUP BY {grp}";
                    continue;
                }

                Query += $", {grp}";
            }

            //go through order
            first = true;
            foreach (var order in Order)
            {
                if (first)
                {
                    first = false;
                    Query += $" ORDER BY {order}";
                    continue;
                }

                Query += $", {order}";
            }

            //add limit
            if (!string.IsNullOrEmpty(Limit))
            {
                Query += $" LIMIT {Limit}";
            }
        }
    }
}
