using System;
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
        /// <summary>
        /// Adds Designator T to the reverse designator database. The database is reset after every map switch, so it should be called at least as often
        /// </summary>
        public static void RegisterDesignator<T>() where T : Designator
        {
            if (Find.ReverseDesignatorDatabase != null)
            {
                if (Find.ReverseDesignatorDatabase.Get<T>() == null)
                {
                    Find.ReverseDesignatorDatabase.AllDesignators.Add(Activator.CreateInstance<T>());
                    if (Find.ReverseDesignatorDatabase.Get<T>() == null)
                        Log($"Could not add {typeof(T).Name} to the reverse designator database!", LogLevel.Error);
                }
            }
            else Log("Designator database is null.", LogLevel.Error);
        }

        public static bool IsDesignatedForDestruction(this Thing thing) =>
            thing?.Map?.designationManager?.DesignationOn(thing, DestroyItemDefOf.Designation_DestroyItem) != null;

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
