﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DestroyItem
{
    [DefOf]
    class DestroyItemDefOf
    {
        public static DesignationDef Designation_DestroyItem;

        public static JobDef Job_DestroyItem;

        public static RecordDef Record_ItemsDestroyed;

        public static ThoughtDef Thought_DestroyedCorpse;
        public static ThoughtDef Thought_KnowDestroyedCorpse;

        public static WorkGiverDef WorkGiver_DestroyItem;
    }
}
