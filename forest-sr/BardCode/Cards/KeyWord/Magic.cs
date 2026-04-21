using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;
using System;

namespace Forest_Sr.BardCode.Cards.KeyWord
{
    public static class BardKeyword
    {
        [CustomEnum("Magic")]
        [KeywordProperties(AutoKeywordPosition.Before)]
        public static CardKeyword Magic;

        [CustomEnum("SONG")]
        [KeywordProperties(AutoKeywordPosition.Before)]
        public static CardKeyword SONG;

        [CustomEnum("FOREST_SR")]
        [KeywordProperties(AutoKeywordPosition.Before)]
        public static CardKeyword Harmony;
    }
}