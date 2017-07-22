using System.Collections.Generic;
using System.Linq;

namespace CodeBuilder.WinForm.UI
{
    using PhysicalDataModel;

    public static class ModelManager
    {
        public static void Add(string key, Model model)
        {
            if (Models.ContainsKey(key))
                Models[key] = model;
            else
                Models.Add(key, model);
        }

        public static void Clear()
        {
            Models.Clear();
        }

        public static bool Remove(string key)
        {
            if (Models.ContainsKey(key))
                return Models.Remove(key);
            return true;
        }

        private static Dictionary<string, Model> Models { get; } = new Dictionary<string, Model>(5);

        public static string GetDatabase(string key)
        {
            if (Models.ContainsKey(key))
                return Models[key].Database;
            return string.Empty;
        }

        public static IMetaData GetModelObject(Model model, string objId)
        {
            if (model == null) return null;
            if (model.Tables != null && model.Tables.ContainsKey(objId)) return model.Tables[objId];
            if (model.Views != null && model.Views.ContainsKey(objId)) return model.Views[objId];

            return null;
        }

        public static Dictionary<string, Model> Clone()
        {
            return Models.Select(x => x).ToDictionary(y => y.Key, z => z.Value);
        }
    }
}
