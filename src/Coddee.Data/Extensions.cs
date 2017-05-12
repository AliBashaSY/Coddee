﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coddee.Data
{
    public static class Extensions
    {
        public static Task<TModel> Update<TModel, TKey>(this ICRUDRepository<TModel,TKey>repo, OperationType op, TModel item)
        {
            return op == OperationType.Add ? repo.InsertItem(item) : repo.UpdateItem(item);
        }
        public static Task<TModel> Update<TModel, TKey>(this ICRUDRepository<TModel, TKey> repo, EditorSaveArgs<TModel> args)
        {
            return repo.Update(args.OperationType, args.Item);
        }
        public static void Update<TModel, TKey>(this ICRUDRepository<TModel, TKey> repo,object sender, EditorSaveArgs<TModel> args)
        {
            repo.Update(args.OperationType, args.Item);
        }
    }
}