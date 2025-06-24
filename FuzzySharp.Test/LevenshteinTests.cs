using NUnit.Framework;
using Raffinert.FuzzySharp.Utils;
using System.Collections.Generic;

namespace Raffinert.FuzzySharp.Test;

[TestFixture]
public class LevenshteinTests
{
    [Test]
    [TestCase(
    "I had two heart attacks, an abortion, did crack... while I was pregnant. Other than that, I'm fine.",
    "You couldn't even be a vegetable - even artichokes have a heart.",
    76)]
    [TestCase("", "", 0)]
    [TestCase("a", "", 1)]
    [TestCase("", "a", 1)]
    [TestCase("kitten", "kitten", 0)]
    [TestCase("kitten", "sitting", 3)]     // substitution + insertion + insertion
    [TestCase("flaw", "lawn", 2)]          // substitution + insertion
    [TestCase("gumbo", "gambol", 2)]       // insertion + substitution
    [TestCase("book", "back", 2)]          // two substitutions
    [TestCase("Sunday", "Saturday", 3)]    // insertion + substitution + insertion
    // Test a few boundary scenarios (longer strings, only one‐character difference)
    [TestCase("a", "b", 1)]
    [TestCase("ab", "ba", 2)]
    [TestCase("abcdef", "azced", 3)]
    [TestCase("distance", "difference", 5)]
    public void TestLevenshteinDistance(string s1, string s2, int expectedDistance)
    {
        int distance = Levenshtein.Distance(s1, s2);
        Assert.AreEqual(expectedDistance, distance);
    }

    [Test]
    public void TestLevenshteinDistance()
    {
        var wordA = new string('A', 4112);
        var wordB = new string('B', 4112);
        int maxDistance = Levenshtein.Distance(wordA, wordB);
        int zeroDistance = Levenshtein.Distance(wordA, wordA);
        Assert.AreEqual(4112, maxDistance);
        Assert.AreEqual(0, zeroDistance);
    }

    [Test, TestCaseSource(typeof(RandomWordPairs), nameof(RandomWordPairs.GetWordPairs))]
    public void Dictionary_Test(string s1, string s2)
    {
        var ds = new DictionarySlimPooled<char, long>(64);

        for (var index = 0; index < s1.Length; index++)
        {
            var c = s1[index];
            ref var val = ref ds.GetOrAddValueRef(c);
            val = index + 1;
        }

        var d = new Dictionary<char, int?>(64);

        for (var index = 0; index < s1.Length; index++)
        {
            var c = s1[index];
            d[c] = index + 1;
        }

        foreach (var c in s1)
        {
            var dc = d[c];
            Assert.True(ds.TryGetValue(c, out var value));
            Assert.AreEqual(value, dc);
        }

        var ds1 = new DictionarySlimPooled<char, long>(64);

        for (var index = 0; index < s2.Length; index++)
        {
            var c = s2[index];
            ref var val = ref ds1.GetOrAddValueRef(c);
            val = index + 1;
        }

        var d1 = new Dictionary<char, int?>(64);

        for (var index = 0; index < s2.Length; index++)
        {
            var c = s2[index];
            d1[c] = index + 1;
        }

        foreach (var c in s2)
        {
            Assert.True(ds1.TryGetValue(c, out var value));
            Assert.AreEqual(value, d1[c]);
        }
    }

    [Test, TestCaseSource(typeof(RandomWordPairs), nameof(RandomWordPairs.GetWordPairs))]
    public void Levenshtein_ShouldBeEqual(string s1, string s2)
    {
        var fd = Levenshtein.Distance(s1, s2);
        var eo = Levenshtein.GetEditOps(s1, s2);
        var mb = eo.AsMatchingBlocks(s1.Length, s2.Length);
        var cd = global::FuzzySharp.Levenshtein.GetMatchingBlocks(s1, s2);
        var qd = Quickenshtein.Levenshtein.GetDistance(s1, s2);

        Assert.That(fd, Is.EqualTo(qd));
        Assert.That(eo.Length, Is.EqualTo(qd));
    }

    [Test]
    [TestCase(
    "䅃湁攺羠Ḱ㵔齖꾛ꔯ놃낒攟푁㘽얬䐳閭邔렧鞧ʫﶡ䡓鍤䝛藻暸㯑㞽ᒳ鸶ꊯ挑ᵐ㡂篊颋ह㷴ൟ棛䨡綃툺읊喟呕涡篻抱ࢬ榭ᦢᝤ梥䊮䀼꽙勓蓌둟縓遮霭ᤂኖʎ쁭ﳲ抲蝬퇸洮꾜䥚뀙睤叛ꮇ콾䰳횙鮚닎୫佖郳勡ꆁ팗嘜좳꯱铭䰼졊镸쏙샫뭥厑煫澦ㆧ룇ᦘﭖ婯뻦阎Ʇ쨣ӱ惗匦뭈뿻봠辗멉煄ꗃ㝻ഛፒ鰼䂏뗃篰廒ラ鄎灳춷钽뢋翃艑힊퍧勗兑煽ሚ䒙楻쓴揯岕溶ೱ럙䳒আ拱覘命杀Ꭴ㔛줲㻌밂磣絫ΐ犀ᴆꢏ砝呕噹쌘馲聂㪀从ᥓ믞饭뛈멢燾ᐯ嚃多廉槑莖ᇌ糝ド疞볼მᵆ狒혝葛ᶆ푾녪韕쇺㓯ꥴ퉃軥粲䊿喗䋃裥榘尖ᗆꨊ껈㳮㻞꾇ඖ虆鱺輪㮑빞ࢡ轼汷ꍲ嘴荱ꘝꉲݲ㬝趇큛觠摚굧쪡孤귟咧藱ኹ禶炙憊ꆌ뎼怊峔쯎䎳隣욺훠쥊찧Ｋᝯ笑Ｅ鬖삒ꊫ嚸쯃㖙믯軒롇ʠ㘟䕯츜鳫酥뉘芸鐘ᆔɦ翺㷵揎똤릺㿂ꑐ훾鸘疶귾멦뷖깼锖ꋡ徭㶌ᐚ䘾씞㵹蕆媴뀽ﴼᅨ䉞幊ⲕ戾憣ݢ礫疺瞯疔찇窀ꓔ洍쉴츽孠䰱奻敜㨭ఐ찆歺徺蛳벿㓘崎鑳ไࡎ뭶㔀籿醃蜥ᳩ攫딙嵺冒㟴䟛焩黐廥뛴ϓ䲿䑳ᔸ澑輺좤㪉濇ꇊή렲䂞廡ヱ삾決옟鲍訟츑䦁忧䞩쓉臥ఝ왞쟱鲻ퟤ鱯䊜ࠋⵠᜧ檍省販䓋䧞岃㱊싞ᢖ詼贊ﶥ㢧哸뤣袍ˇ뉉왥넹쪚ﮡ䆊쬮蝙ˋ偛낎ꡛ",
    "뼔瘦酴藈꿋殏爛퐑뫸勉䜤獍ﯧ胐椅桏꿨靄ꖐ矼했བ荖먹證ۺ讌м뮴㷉ᙣ䳺鿙ꧫ䤜ꏵ隀쵔ҡ錏ꆴ礅극틲诱ʋ䏚殑뭢옢咫쎴ጯ釒䆦떌큒뚼烙ꩇ붅퓹ア閿䌜ힹ냷䪌剂ꂕ꽒쀗鏕᪇돊畒叟跧䛘褿륋㪉㾿팕웠䏉䂆ᓽ腜煒댤뽅틕儉딊갼悩ℛ꧔采䤣훑ㆬㄛⱳ爗䡮ퟆ불ꥤ穽嬻衳枒컭鮌꿇㵘깟ﻻ놴ꃲꔹ빳쳲춏쫅嶙喂ଢ଼ṥ㵣쌄郖媩뿽ঔጞ뱄涙隣䙏䢏뛇푗炸侢萒徟ⵀבֿ㶟怭ꪶ뾓菶䩬匬䏮䅒蓔좘鬇뗺鍯垨疺")]
    //[TestCase(
    //  "隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣1隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣隣",
    //  "45隣")]
    public void Levenshtein_ShouldBeEqual1(string s1, string s2)
    {
        var c = '隣';
        //var s1 = string.Join("", Enumerable.Range(0, 255).Select(_ => '1').Concat(['隣']));
        //var s2 = string.Join("", Enumerable.Range(0, 258).Select(_ => '2').Concat(['隣'])); ;
        var fd = Levenshtein.Distance(s1, s2);
        var eo = Levenshtein.GetEditOps(s1, s2);
        var mb = eo.AsMatchingBlocks(s1.Length, s2.Length);
        var cd = global::FuzzySharp.Levenshtein.GetMatchingBlocks(s1, s2);
        var qd = Quickenshtein.Levenshtein.GetDistance(s1, s2);

        Assert.That(fd, Is.EqualTo(qd));
        Assert.That(eo.Length, Is.EqualTo(qd));
    }
}
