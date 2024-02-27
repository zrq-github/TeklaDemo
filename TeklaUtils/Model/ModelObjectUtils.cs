using System;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures;

namespace TeklaUtils.Model;

public static class ModelObjectUtils
{
    #region Static Members

    /// <summary>
    /// ModelObjects ConvertTo Identifier
    /// </summary>
    /// <exception cref="ArgumentNullException">modelObjects is null</exception>
    public static IList<Identifier> Convert(in ICollection<TSM.ModelObject> modelObjects)
    {
        if (null == modelObjects)
            throw new ArgumentNullException(nameof(modelObjects));

        var identifiers = new List<Identifier>(modelObjects.Count);
        identifiers.AddRange(modelObjects.Select(modelObject => modelObject.Identifier));

        return identifiers;
    }

    /// <summary>
    /// Gets list of modelObject parts
    /// </summary>
    /// <exception cref="ArgumentNullException">modelObject is null</exception>
    /// <remarks>
    /// modelObject is Assembly Component Task
    /// </remarks>
    public static IList<TSM.ModelObject> GetAllParts(in TSM.ModelObject modelObject)
    {
        if (null == modelObject)
            throw new ArgumentNullException(nameof(modelObject));

        var parts = new List<TSM.ModelObject>();
        switch (modelObject)
        {
            case TSM.Assembly assembly:
                GetAssemblyParts(assembly);
                break;
            case TSM.Component component:
                GetComponentParts(component);
                break;
            case TSM.Task task:
                GetTaskParts(task);
                break;
            default:
                return null;
        }

        return parts;
    }

    /// <summary>
    /// Gets list of assembly parts
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    public static IList<TSM.ModelObject> GetAssemblyParts(in TSM.Assembly assembly)
    {
        if (null == assembly)
            throw new ArgumentNullException(nameof(assembly));

        var arrayList = assembly.GetSecondaries();
        var parts = new List<TSM.ModelObject>(arrayList.Count + 1)
        {
            assembly.GetMainPart()
        };

        var assemblyChildren = arrayList.GetEnumerator();
        while (assemblyChildren.MoveNext())
            parts.Add(assemblyChildren.Current as TSM.ModelObject);

        return parts;
    }

    /// <summary>
    /// Gets list of component parts
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    public static IList<TSM.ModelObject> GetComponentParts(in TSM.BaseComponent component)
    {
        if (null == component)
            throw new ArgumentNullException(nameof(component));

        var myChildren = component.GetChildren();
        var parts = new List<TSM.ModelObject>(myChildren.GetSize() + 1);

        while (myChildren.MoveNext())
            parts.Add(myChildren.Current);

        return parts;
    }

    /// <summary>
    /// Gets list of task parts
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    public static IList<TSM.ModelObject> GetTaskParts(in TSM.Task task)
    {
        if (null == task)
            throw new ArgumentNullException(nameof(task));

        var myMembers = task.GetChildren();

        var parts = new List<TSM.ModelObject>();

        while (myMembers.MoveNext())
            switch (myMembers.Current)
            {
                case TSM.Task current:
                    parts.AddRange(GetTaskParts(current));
                    break;
                case TSM.Part:
                    parts.Add(myMembers.Current);
                    break;
            }

        return parts;
    }

    #endregion
}