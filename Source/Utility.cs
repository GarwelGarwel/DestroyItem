using Verse;

namespace DestroyItem
{
    internal enum LogLevel
    {
        Message = 0,
        Warning,
        Error
    };

    public static class Utility
    {
        public static bool IsDesignatedForDestruction(this Thing thing) =>
            thing?.Map?.designationManager?.DesignationOn(thing, DestroyItemDefOf.Designation_DestroyItem) != null;

        public static void DesignateForDestruction(this Thing thing)
        {
            Log($"Designating {thing.LabelCap} ({thing.def.defName}) for destruction.");
            thing.Map.designationManager.AddDesignation(new Designation(thing, DestroyItemDefOf.Designation_DestroyItem));
        }

        internal static void Log(string message, LogLevel logLevel = LogLevel.Message)
        {
            message = $"[DestroyItem] {message}";
            switch (logLevel)
            {
                case LogLevel.Message:
                    if (Prefs.DevMode)
                        Verse.Log.Message(message);
                    break;

                case LogLevel.Warning:
                    Verse.Log.Warning(message);
                    break;

                case LogLevel.Error:
                    Verse.Log.Error(message);
                    break;
            }
        }
    }
}
