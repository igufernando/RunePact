using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography;

namespace RunePact.Core
{
    public enum RewardKind { Card, Upgrade, Relic }
    public sealed class RewardOffer
    {
        public RewardKind Kind;
        public string Title, Detail;
        public CardVariant Card;
        public Relic Relic;
        public int Owner, Slot;
    }

    public sealed class Journey
    {
        public Battle Current { get; private set; }
        public int Encounter { get; private set; } = 1;
        public EnemyFormation Formation { get; private set; }
        public bool AwaitingReward => Current.Outcome == 1 && Encounter < 2;
        public bool Complete => Current.Outcome == 1 && Encounter == 2;
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
            if(encounter==2)return EnemyFormation.Regent;
            var options=new[]{EnemyFormation.Vanguard,EnemyFormation.Shieldwall,EnemyFormation.Hunt,EnemyFormation.Pyre};
            return options[(int)((uint)(seed+encounter*7)%(uint)options.Length)];
        }
        public RewardOffer[] GetOffers()
        {
            int owner=(int)((uint)(seed+Encounter)%3);
            CardVariant card=(CardVariant)(1+(uint)seed%9);
            Relic[] relics={Relic.PactMedallion,Relic.AncientEmber,Relic.LivingGrimoire,Relic.CeremonialArrow,Relic.OakBark};
            Relic chosen=relics[(int)((uint)seed%5)];
            return new[]{
                new RewardOffer {Kind=RewardKind.Card,Card=card,Owner=owner,Title=Current.Describe(new Card(owner,2,card)).Name,Detail="Adiciona esta carta à mão inicial do chefe e ao baralho."},
                new RewardOffer {Kind=RewardKind.Upgrade,Owner=owner,Slot=0,Title=Current.Describe(new Card(owner,0)).Name+" +",Detail="Aprimora somente esta habilidade: +6 de poder em todas as cópias."},
                new RewardOffer {Kind=RewardKind.Relic,Relic=chosen,Title=RelicTitle(chosen),Detail=RelicDetail(chosen)}
            };
        }
        public bool ChooseReward(int choice)
        {
            if (!AwaitingReward || choice < 0 || choice > 2) return false;
            var offer=GetOffers()[choice];
            if(offer.Kind==RewardKind.Card)ExtraCards.Add(new Card(offer.Owner,2,offer.Card));
            if(offer.Kind==RewardKind.Relic&&!Relics.Contains(offer.Relic))Relics.Add(offer.Relic);
            var previous = Current;
            Encounter++;
            Formation=PickFormation(Encounter);
            Current = new Battle(seed + Encounter, previous.Fighters.Take(3).Select(f => f.Gear).ToArray(), Encounter, Formation, Relics, ExtraCards, CardBoost);
            Array.Copy(previous.SkillBoost,Current.SkillBoost,9);
            if(offer.Kind==RewardKind.Upgrade)Current.SkillBoost[offer.Owner*3+offer.Slot]+=6;
            if(offer.Kind==RewardKind.Card){
                var rewardCard=Current.Deck.FirstOrDefault(x=>x.Variant==offer.Card&&x.Owner==offer.Owner);
                if(rewardCard!=null){Current.Deck.Remove(rewardCard);var replaced=Current.Hand[1];Current.Hand[1]=rewardCard;Current.Deck.Add(replaced);}
            }
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
            using(var stream=new MemoryStream())using(var w=new BinaryWriter(stream)){
                w.Write(4);w.Write(seed);w.Write(Encounter);w.Write((int)Formation);
                w.Write(Relics.Count);foreach(var relic in Relics)w.Write((int)relic);
                foreach(int boost in CardBoost)w.Write(boost);Battle.WriteCards(w,ExtraCards);Current.WriteSnapshot(w);w.Flush();
                byte[] data=stream.ToArray();using(var hash=SHA256.Create())return Convert.ToBase64String(hash.ComputeHash(data))+":"+Convert.ToBase64String(data);
            }
        }
        public static bool TryRestore(string snapshot,out Journey journey)
        {
            journey=null;
            try {
                if(string.IsNullOrEmpty(snapshot)||snapshot.Length>65536)return false;
                var parts=snapshot.Split(':');if(parts.Length!=2)return false;byte[] data=Convert.FromBase64String(parts[1]);
                using(var hash=SHA256.Create())if(Convert.ToBase64String(hash.ComputeHash(data))!=parts[0])return false;
                using(var stream=new MemoryStream(data))using(var r=new BinaryReader(stream)){
                    if(r.ReadInt32()!=4)return false;int seed=r.ReadInt32(),encounter=Battle.ReadNumber(r,1,2);
                    var formation=(EnemyFormation)Battle.ReadNumber(r,0,4);if((encounter==2)!=(formation==EnemyFormation.Regent))return false;
                    var candidate=new Journey(seed);candidate.Encounter=encounter;candidate.Formation=formation;
                    int count=Battle.ReadNumber(r,0,5);for(int i=0;i<count;i++)candidate.Relics.Add((Relic)Battle.ReadNumber(r,0,4));
                    if(candidate.Relics.Distinct().Count()!=count)return false;
                    for(int i=0;i<3;i++)candidate.CardBoost[i]=Battle.ReadNumber(r,0,60);
                    candidate.ExtraCards.AddRange(Battle.ReadCards(r,20));
                    candidate.Current=new Battle(seed,null,encounter,formation,candidate.Relics,candidate.ExtraCards,candidate.CardBoost);
                    candidate.Current.ReadSnapshot(r);if(stream.Position!=stream.Length)return false;journey=candidate;return true;
                }
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
