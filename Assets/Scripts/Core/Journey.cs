using System;
using System.Collections.Generic;
using System.Linq;

namespace RunePact.Core
{
    public enum RewardKind { Card, Upgrade, Relic }
    public sealed class RewardOffer
    {
        public RewardKind Kind;
        public string Title, Detail;
        public CardVariant Card;
        public Relic Relic;
        public int Owner;
    }

    public sealed class Journey
    {
        public Battle Current { get; private set; }
        public int Encounter { get; private set; } = 1;
        public EnemyFormation Formation { get; private set; }
        public bool AwaitingReward => Current.Outcome == 1 && Encounter < 3;
        public bool Complete => Current.Outcome == 1 && Encounter == 3;
        public readonly List<Relic> Relics = new List<Relic>();
        public readonly List<Card> ExtraCards = new List<Card>();
        public readonly int[] CardBoost = new int[3];
        readonly int seed;

        public Journey(int seed, int[] gear = null)
        {
            this.seed = seed;
            Formation = PickFormation(1);
            Current = new Battle(seed, gear, Encounter, Formation, Relics, ExtraCards, CardBoost);
        }
        EnemyFormation PickFormation(int encounter)
        {
            if(encounter==3)return EnemyFormation.Regent;
            var options=new[]{EnemyFormation.Vanguard,EnemyFormation.Shieldwall,EnemyFormation.Hunt,EnemyFormation.Pyre};
            return options[Math.Abs(seed+encounter*7)%options.Length];
        }
        public RewardOffer[] GetOffers()
        {
            int owner=Math.Abs(seed+Encounter)%3;
            CardVariant card=Encounter==1?CardVariant.Comet:CardVariant.Renewal;
            Relic[] relics={Relic.PactMedallion,Relic.AncientEmber,Relic.LivingGrimoire,Relic.CeremonialArrow,Relic.OakBark};
            Relic chosen=relics.First(x=>!Relics.Contains(x));
            return new[]{
                new RewardOffer {Kind=RewardKind.Card,Card=card,Owner=owner,Title=card==CardVariant.Comet?"COMETA DE ÂMBAR":"RITO DE RENOVAÇÃO",Detail=card==CardVariant.Comet?"Adicione uma carta de dano em área ao baralho.":"Adicione uma carta de cura ao baralho."},
                new RewardOffer {Kind=RewardKind.Upgrade,Owner=owner,Title="APRIMORAR "+new[]{"AURA","LYRA","KAEL"}[owner],Detail="As cartas desse guerreiro recebem +6 de poder nesta jornada."},
                new RewardOffer {Kind=RewardKind.Relic,Relic=chosen,Title=RelicTitle(chosen),Detail=RelicDetail(chosen)}
            };
        }
        public bool ChooseReward(int choice)
        {
            if (!AwaitingReward || choice < 0 || choice > 2) return false;
            var offer=GetOffers()[choice];
            if(offer.Kind==RewardKind.Card)ExtraCards.Add(new Card(offer.Owner,2,offer.Card));
            if(offer.Kind==RewardKind.Upgrade)CardBoost[offer.Owner]+=6;
            if(offer.Kind==RewardKind.Relic&&!Relics.Contains(offer.Relic))Relics.Add(offer.Relic);
            var previous = Current;
            Encounter++;
            Formation=PickFormation(Encounter);
            Current = new Battle(seed + Encounter, previous.Fighters.Take(3).Select(f => f.Gear).ToArray(), Encounter, Formation, Relics, ExtraCards, CardBoost);
            Current.Shards = previous.Shards + 2;
            // Todo prêmio leva a um novo encontro com recuperação básica; níveis e equipamento persistem.
            // Every reward leads to a new encounter with basic recovery; equipment and upgrade tiers persist.
            for (int i = 0; i < 3; i++)
            {
                var hero = Current.Fighters[i];
                hero.Tier = previous.Fighters[i].Tier;
                hero.MaxHp = previous.Fighters[i].MaxHp;
                hero.Hp = Math.Min(hero.MaxHp, previous.Fighters[i].Hp + 35);
                if(hero.Hp<=0)hero.Hp=Math.Min(hero.MaxHp,45);
            }
            Current.Plan();
            return true;
        }
        public string Save()
        {
            string relics=string.Join(",",Relics.Select(x=>((int)x).ToString()).ToArray());
            string cards=string.Join(";",ExtraCards.Select(x=>x.Owner+","+x.Slot+","+(int)x.Variant).ToArray());
            string heroes=string.Join(";",Current.Fighters.Take(3).Select(x=>x.Gear+","+x.Tier+","+x.Hp+","+x.MaxHp).ToArray());
            return seed+"|"+Encounter+"|"+(int)Formation+"|"+Current.Shards+"|"+string.Join(",",CardBoost)+"|"+relics+"|"+cards+"|"+heroes;
        }
        public static bool TryRestore(string snapshot,out Journey journey)
        {
            journey=null;
            try {
                var parts=snapshot.Split('|');if(parts.Length!=8)return false;
                int seed=int.Parse(parts[0]),encounter=int.Parse(parts[1]),shards=int.Parse(parts[3]);
                var heroes=parts[7].Split(';').Select(x=>x.Split(',').Select(int.Parse).ToArray()).ToArray();
                if(heroes.Length!=3)return false;
                journey=new Journey(seed,heroes.Select(x=>x[0]).ToArray());
                journey.Encounter=encounter;journey.Formation=(EnemyFormation)int.Parse(parts[2]);
                foreach(var value in parts[5].Split(new[]{','},StringSplitOptions.RemoveEmptyEntries))journey.Relics.Add((Relic)int.Parse(value));
                foreach(var value in parts[6].Split(new[]{';'},StringSplitOptions.RemoveEmptyEntries)){var card=value.Split(',').Select(int.Parse).ToArray();journey.ExtraCards.Add(new Card(card[0],card[1],(CardVariant)card[2]));}
                var boosts=parts[4].Split(',').Select(int.Parse).ToArray();for(int i=0;i<3;i++)journey.CardBoost[i]=boosts[i];
                journey.Current=new Battle(seed+encounter,heroes.Select(x=>x[0]).ToArray(),encounter,journey.Formation,journey.Relics,journey.ExtraCards,journey.CardBoost);
                journey.Current.Shards=shards;
                for(int i=0;i<3;i++){var target=journey.Current.Fighters[i];target.Gear=heroes[i][0];target.Tier=heroes[i][1];target.Hp=heroes[i][2];target.MaxHp=heroes[i][3];}
                journey.Current.Plan();return true;
            } catch {journey=null;return false;}
        }
        public static string RelicTitle(Relic relic)
        {
            switch(relic){
                case Relic.PactMedallion:return "MEDALHÃO DO PACTO";
                case Relic.AncientEmber:return "BRASA ANTIGA";
                case Relic.LivingGrimoire:return "GRIMÓRIO VIVO";
                case Relic.CeremonialArrow:return "FLECHA CERIMONIAL";
                default:return "CASCA DE CARVALHO";
            }
        }
        public static string RelicDetail(Relic relic)
        {
            switch(relic){
                case Relic.PactMedallion:return "O Pacto também concede +8 de escudo ao grupo.";
                case Relic.AncientEmber:return "Sua queimadura causa 9 de dano por turno.";
                case Relic.LivingGrimoire:return "A primeira magia de Lyra por turno custa 1 a menos.";
                case Relic.CeremonialArrow:return "Inimigos marcados recebem +15 no próximo ataque.";
                default:return "O primeiro escudo de cada aliado recebe +10.";
            }
        }
    }
}
