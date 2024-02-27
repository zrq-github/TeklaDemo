using System.Collections.Generic;
using Tekla.Structures.Model;

namespace TeklaTestApplication;

public static class TeklaModelExtensions
{
    #region Static Members

    public static List<ModelObject> ToList(this ModelObjectEnumerator enumerator)
    {
        var modelObjects = new List<ModelObject>(enumerator.GetSize() + 1);
        while (enumerator.MoveNext())
        {
            var modelObject = enumerator.Current;
            modelObjects.Add(modelObject);
        }
        return modelObjects;
    }

    #endregion
}