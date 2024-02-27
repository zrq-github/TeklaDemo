using System.Collections;
using System.Collections.Generic;

namespace TeklaUtils.Extend;

public static class TeklaEnumeratorExtend
{
    #region Static Members

    /// <summary>
    /// IEnumerator转换成ToList
    /// </summary>
    public static IList<T> ToList<T>(this IEnumerator enumerator) where T : class
    {
        if (null == enumerator)
            return null;

        var list = new List<T>();
        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;
            list.Add(current as T);
        }

        return list;
    }

    public static IList<TSD.Drawing> ToList(this TSD.DrawingEnumerator enumerator)
    {
        if (null == enumerator)
            return null;

        return enumerator.ToList<TSD.Drawing>();
    }

    public static IList<TSG.Point> ToList(this Tekla.Structures.Solid.VertexEnumerator vertexEnumerator)
    {
        if (null == vertexEnumerator)
        {
            return null;
        }

        return vertexEnumerator.ToList<TSG.Point>();
    }

    #endregion
}