namespace TeklaUtils.Geometry3d
{
    /// <summary>
    /// 坐标系统
    /// </summary>
    public static class CoordinateSystemUtils
    {
        public static readonly TSG.Vector UpDirection = new(0.0, 0.0, 1.0);

        /// <summary>
        /// Gets coordinate system of selected object
        /// Different model objects have different way of getting a proper coordinate system, 
        /// eg. for Assemblies the main part coordinate system is used, and task has no coordinate system
        /// </summary>
        /// <param name="modelObject">Part Assembly BaseComponent Task</param>
        /// <param name="coordinateSystem">return modelObject coordinateSystem</param>
        public static void GetCoordinateSystem(TSM.ModelObject modelObject, out TSG.CoordinateSystem coordinateSystem)
        {
            if (modelObject is TSM.Part part)
            {
                coordinateSystem = part.GetCoordinateSystem();
            }
            else if (modelObject is TSM.Assembly assembly)
            {
                coordinateSystem = assembly.GetMainPart().GetCoordinateSystem();
            }
            else if (modelObject is TSM.BaseComponent baseComponent)
            {
                coordinateSystem = baseComponent.GetCoordinateSystem();
            }
            else if (modelObject is TSM.Task task)
            {
                coordinateSystem = new TSG.CoordinateSystem();
            }
            else
            {
                coordinateSystem = new TSG.CoordinateSystem();
            }
        }

        /// <summary>
        /// Gets part default front view coordinate system
        /// Gets coordinate system as it is defined in the TS core for part/component basic views, which is different than in singlepart/assembly drawings.
        /// </summary>
        public static TSG.CoordinateSystem GetBasicViewsCoordinateSystemForFrontView(TSG.CoordinateSystem objectCoordinateSystem)
        {
            var result = new TSG.CoordinateSystem
            {
                Origin = new TSG.Point(objectCoordinateSystem.Origin),
                AxisX = new TSG.Vector(objectCoordinateSystem.AxisX) * -1.0,
                AxisY = new TSG.Vector(objectCoordinateSystem.AxisY)
            };

            var tempVector = (result.AxisX.Cross(UpDirection));

            if (tempVector == new TSG.Vector())
                tempVector = (objectCoordinateSystem.AxisY.Cross(UpDirection));

            result.AxisX = tempVector.Cross(UpDirection).GetNormal();
            result.AxisY = UpDirection.GetNormal();

            return result;
        }

        /// <summary>
        /// Gets part default top view coordinate system
        /// Gets coordinate system as it is defined in the TS core for part/component basic views, which is different than in singlepart/assembly drawings.
        /// </summary>
        /// <param name="objectCoordinateSystem"></param>
        /// <returns></returns>
        public static TSG.CoordinateSystem GetBasicViewsCoordinateSystemForTopView(TSG.CoordinateSystem objectCoordinateSystem)
        {
            var result = new TSG.CoordinateSystem
            {
                Origin = new TSG.Point(objectCoordinateSystem.Origin),
                AxisX = new TSG.Vector(objectCoordinateSystem.AxisX) * -1.0,
                AxisY = new TSG.Vector(objectCoordinateSystem.AxisY)
            };

            var tempVector = (result.AxisX.Cross(UpDirection));

            if (tempVector == new TSG.Vector())
                tempVector = (objectCoordinateSystem.AxisY.Cross(UpDirection));

            result.AxisX = tempVector.Cross(UpDirection);
            result.AxisY = tempVector;

            return result;
        }

        /// <summary>
        /// Gets part default end view coordinate system
        /// Gets coordinate system as it is defined in the TS core for part/component basic views, which is different than in singlepart/assembly drawings.
        /// </summary>
        /// <param name="objectCoordinateSystem"></param>
        /// <returns></returns>
        public static TSG.CoordinateSystem GetBasicViewsCoordinateSystemForEndView(TSG.CoordinateSystem objectCoordinateSystem)
        {
            var result = new TSG.CoordinateSystem
            {
                Origin = new TSG.Point(objectCoordinateSystem.Origin),
                AxisX = new TSG.Vector(objectCoordinateSystem.AxisX) * -1.0,
                AxisY = new TSG.Vector(objectCoordinateSystem.AxisY)
            };

            var tempVector = (result.AxisX.Cross(UpDirection));

            if (tempVector == new TSG.Vector())
                tempVector = (objectCoordinateSystem.AxisY.Cross(UpDirection));

            result.AxisX = tempVector;
            result.AxisY = UpDirection;

            return result;
        }
    }
}
