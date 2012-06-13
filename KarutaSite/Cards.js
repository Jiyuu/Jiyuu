

var KarutaCardsArray = new Array();

function KarutaCard(cardNumber, hiregana, romanji) {
    this.number = cardNumber;
    this.hiregana = hiregana;
    this.romanji = romanji;
}

function initCards() {
    var cardIndex = 0;
    var hiregana;
    var romanji;
    var card;

    hiregana = 'わがころも|でわつゆに|ぬれつつ';
    romanji = 'Waga koromode wa|Tsuya ni nure tsutsu';
    card = new KarutaCard(cardIndex++, hiregana, romanji);
    KarutaCardsArray.push(card);
    hiregana = romanji = '';

    hiregana = 'ころもほす|ちょあまあの|かぐやま';
    romanji = 'koromo hosu cho| Ama no kaguyama';
    card = new KarutaCard(cardIndex++, hiregana, romanji);
    KarutaCardsArray.push(card);
    hiregana = romanji = '';

    hiregana = 'ながながし|よおひとり|かもねん';
    romanji = 'Naganagashi yo o|Hitori ka mo nen';
    card = new KarutaCard(cardIndex++, hiregana, romanji);
    KarutaCardsArray.push(card);
    hiregana = romanji = '';

    hiregana = 'ふじのたか|ねにゆきは|ふりつつ';
    romanji = 'Fuji no takane ni|Yuki wa furi tsutsu';
    card = new KarutaCard(cardIndex++, hiregana, romanji);
    KarutaCardsArray.push(card);
    hiregana = romanji = '';

    hiregana = 'こえいくっと|きぞあきわ|かなしき';
    romanji = 'Koe kiku toki zo|Aki wa kanashiki';
    card = new KarutaCard(cardIndex++, hiregana, romanji);
    KarutaCardsArray.push(card);
    hiregana = romanji = '';

    hiregana = 'しろきおみ|ればよぞふ|けにける';
    romanji = 'Shiroki o mireba|Yo zo fuke ni keru';
    card = new KarutaCard(cardIndex++, hiregana, romanji);
    KarutaCardsArray.push(card);
    hiregana = romanji = '';

    hiregana = 'みかさのや|まにいでし|つきかも';
    romanji = 'Mikasa no yama ni|Ideshi tsuki kamo';
    card = new KarutaCard(cardIndex++, hiregana, romanji);
    KarutaCardsArray.push(card);
    hiregana = romanji = '';

    hiregana = 'よおうじや|まとひとは|いうなり';
    romanji = 'Yo o Ujiyama to|Hito wa iu nari';
    card = new KarutaCard(cardIndex++, hiregana, romanji);
    KarutaCardsArray.push(card);
    hiregana = romanji = '';

    hiregana = 'わがみよに|ふるながめ|せしまに';
    romanji = 'Waga mi yo ni furu|Nagame seshi ma ni';
    card = new KarutaCard(cardIndex++, hiregana, romanji);
    KarutaCardsArray.push(card);
    hiregana = romanji = '';

    hiregana = 'しるもしら|ぬもおうさ|かのせき';
    romanji = 'Shiru mo shiranu mo|Osaka no seki';
    card = new KarutaCard(cardIndex++, hiregana, romanji);
    KarutaCardsArray.push(card);
    hiregana = romanji = ''; 
      
//    hiregana = '||';
//    romanji = '|';
//    card = new KarutaCard(cardIndex++, hiregana, romanji);
//    KarutaCardsArray.push(card);
//    hiregana = romanji = ''; 

}


initCards();
function getKarutaCard(CardNumber) {
    return KarutaCardsArray[CardNumber-1];
//    return KarutaCardsArray[CardNumber-1];
}