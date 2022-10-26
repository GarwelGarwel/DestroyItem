using RimWorld;
using Verse;

namespace DestroyItem
{
    [DefOf]
    class DestroyItemDefOf
    {
        public static DamageDef Damage_Destruction;

        public static DesignationDef Designation_DestroyItem;

        public static JobDef Job_DestroyItem;

        public static RecordDef Record_ItemsDestroyed;

        public static ThoughtDef Thought_DestroyedCorpse;
        public static ThoughtDef Thought_KnowDestroyedCorpse;
        public static ThoughtDef Thought_DestroyedEmbryo;

        public static WorkGiverDef WorkGiver_DestroyItem;
    }
}
