﻿using neo4j.lib.Interfaces;
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace neo4j.lib
{
    public static class EntityMapper
    {
        //public static IImportable MapToEntity(IRecord record)
        //{
        //    var type = record.GetType();
        //    record.Get()

        //    if (type == "")


        //    return type switch
        //    {
        //        "Person" => new Person
        //        {
        //            Id = record["id"].As<string>(),
        //            Name = record["name"].As<string>(),
        //            Age = record["age"].As<int>()
        //        },
        //        "Product" => new Product
        //        {
        //            Id = record["id"].As<string>(),
        //            Name = record["name"].As<string>(),
        //            Price = record["price"].As<double>()
        //        },
        //        _ => throw new Exception($"Unknown type: {type}")
        //    };
        //}
    }

}