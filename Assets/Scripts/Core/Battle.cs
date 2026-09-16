using System;
using System.Collections.Generic;
using System.Linq;

namespace RunePact.Core
{
    public enum Effect { Strike, Guard, Mend, Burn, Pierce, Volley, Rally }
    public sealed class Fighter
    {
        public int Id, MaxHp, Hp, Shield, Burn, Weak, Tier, Gear;
        public string Name;
        public bool Enemy;
        public bool Alive => Hp > 0;
        public Fighter(int id, string name, int hp, bool enemy) { Id=id; Name=name; MaxHp=Hp=hp; Enemy=enemy; }
    }
    public sealed class Card
    {
        public int Owner, Slot;
        public Card(int owner,int slot) { Owner=owner; Slot=slot; }
    }
    public sealed class Ability
    {
        public string Name, Source, Description;
        public int Cost, Power;
        public Effect Effect;
        public bool Friendly => Effect==Effect.Guard || Effect==Effect.Mend || Effect==Effect.Rally;
    }
    public sealed class Intent
    {
        public int Owner, Target, Power;
        public Effect Effect;
        public string Label;
    }
    public sealed class Battle
    {
        public readonly List<Fighter> Fighters = new List<Fighter>();
        public readonly List<Card> Hand = new List<Card>(), Deck = new List<Card>(), Discard = new List<Card>();
        public readonly List<Intent> Intents = new List<Intent>();
        public readonly List<string> Log = new List<string>();
        readonly HashSet<int> contributors = new HashSet<int>();
        readonly Random random;
        public int Round=1, Energy=4, Shards=3, Played, TotalPlayed, CardsPlayed;
        public bool PlayerTurn=true, HarmonyUsed;
        // 0 em andamento, 1 vitória, -1 derrota, 2 empate.
        // 0 ongoing, 1 victory, -1 defeat, 2 draw.
        public int Outcome;
        public int Harmony => contributors.Count;
        public Battle(int seed=0, int[] gear=null)
        {
            random=new Random(seed);
            Fighters.Add(new Fighter(0,"AURA",145,false));
            Fighters.Add(new Fighter(1,"LYRA",115,false));
            Fighters.Add(new Fighter(2,"KAEL",125,false));
            Fighters.Add(new Fighter(3,"FERRO",155,true));
            Fighters.Add(new Fighter(4,"VÉU",120,true));
            Fighters.Add(new Fighter(5,"ESPINHO",115,true));
            for(int i=0;i<3;i++) {
                Fighters[i].Gear=gear==null?0:gear[i];
                for(int copy=0;copy<2;copy++) for(int slot=0;slot<3;slot++) Deck.Add(new Card(i,slot));
            }
            Shuffle();
            // A primeira mão ensina os três papéis e equipamentos; mãos posteriores usam o baralho embaralhado.
            // First hand teaches all three roles and equipment; later hands use the shuffled deck.
            for(int i=0;i<3;i++) foreach(int slot in new[]{0,2}) { var c=Deck.First(x=>x.Owner==i&&x.Slot==slot); Deck.Remove(c); Hand.Add(c); }
            Plan(); Say("Santuário do Crepúsculo • escolha uma carta e depois um alvo.");
        }
        public string GearName(int owner)
        {
            string[,] names={{"Escudo do pacto","Espada de brasa"},{"Chapéu da aurora","Cajado de cinzas"},{"Besta perfurante","Aljava de ecos"}};
            return names[owner,Fighters[owner].Gear];
        }
        public Ability Describe(Card card)
        {
            var f=Fighters[card.Owner];
            if(card.Slot==0) return Make(new[]{"Corte preciso","Pulso arcano","Disparo ágil"}[card.Owner],"Habilidade base",Effect.Strike,new[]{28,25,30}[card.Owner],1,"Causa {0} de dano a um inimigo.");
            if(card.Slot==1) return Make("Postura defensiva","Habilidade base",Effect.Guard,24,1,"Concede {0} de escudo a um aliado até seu próximo turno.");
            int n=f.Tier;
            if(card.Owner==0 && f.Gear==0) return Make("Bastião",GearName(0),Effect.Rally,19+n*7,2,"Todos os aliados recebem {0} de escudo. Remove fraqueza.");
            if(card.Owner==0) return Make("Corte de brasa",GearName(0),Effect.Burn,31+n*8,1,"Causa {0} de dano e aplica 2 turnos de queimadura (6/turno).");
            if(card.Owner==1 && f.Gear==0) return Make("Luz restauradora",GearName(1),Effect.Mend,32+n*9,1,"Restaura {0} de vida de um aliado e remove queimadura.");
            if(card.Owner==1) return Make("Chuva de cinzas",GearName(1),Effect.Volley,22+n*7,2,"Causa {0} de dano a todos os inimigos vivos.");
            if(f.Gear==0) return Make("Virote rúnico",GearName(2),Effect.Pierce,34+n*8,1,"Causa {0} de dano, ignorando o escudo.");
            return Make("Salva de ecos",GearName(2),Effect.Volley,23+n*7,2,"Causa {0} de dano a todos os inimigos vivos.");
        }
        static Ability Make(string name,string source,Effect fx,int power,int cost,string text) => new Ability {Name=name,Source=source,Effect=fx,Power=power,Cost=cost,Description=string.Format(text,power)};
        public bool CanTarget(Card card,int id)
        {
            return id>=0 && id<Fighters.Count && Fighters[id].Alive && Fighters[id].Enemy!=Describe(card).Friendly;
        }
        public bool Play(int index,int target,out string error)
        {
            error="";
            if(!PlayerTurn||Outcome!=0){error="Aguarde seu turno.";return false;}
            if(index<0||index>=Hand.Count){error="Selecione uma carta.";return false;}
            Card card=Hand[index]; var caster=Fighters[card.Owner]; var a=Describe(card);
            if(!caster.Alive){error="Este guerreiro foi derrotado.";return false;}
            if(Energy<a.Cost){error="Energia insuficiente.";return false;}
            if(!CanTarget(card,target)){error=a.Friendly?"Escolha um aliado vivo.":"Escolha um inimigo vivo.";return false;}
            Energy-=a.Cost; Hand.RemoveAt(index); Discard.Add(card); Played++; TotalPlayed++; CardsPlayed++;
            int power=caster.Weak>0?(int)(a.Power*0.75f):a.Power;
            Resolve(a.Effect,power,target,false);
            Say(caster.Name+" • "+a.Name+" → "+(a.Effect==Effect.Volley||a.Effect==Effect.Rally?"equipe":Fighters[target].Name));
            contributors.Add(card.Owner);
            if(contributors.Count==3&&!HarmonyUsed){ HarmonyUsed=true; Energy++; Say("PACTO! Três guerreiros agiram: +1 energia."); }
            CheckOutcome(); return true;
        }
        public void Resolve(Effect fx,int power,int target,bool enemy)
        {
            Fighter t=Fighters[target];
            switch(fx) {
                case Effect.Guard: t.Shield+=power; break;
                case Effect.Rally: foreach(var f in Fighters.Where(f=>f.Alive&&f.Enemy==enemy)){f.Shield+=power;f.Weak=0;} break;
                case Effect.Mend: t.Hp=Math.Min(t.MaxHp,t.Hp+power);t.Burn=0;break;
                case Effect.Volley: foreach(var f in Fighters.Where(f=>f.Alive&&f.Enemy!=enemy)) Damage(f,power,false);break;
                case Effect.Pierce: Damage(t,power,true);break;
                case Effect.Burn: Damage(t,power,false);if(t.Alive)t.Burn=Math.Max(2,t.Burn);break;
                default: Damage(t,power,false);break;
            }
        }
        public static void Damage(Fighter f,int amount,bool pierce)
        {
            if(!f.Alive)return;
            int absorb=pierce?0:Math.Min(f.Shield,amount);f.Shield-=absorb;f.Hp=Math.Max(0,f.Hp-(amount-absorb));
            if(!f.Alive){f.Shield=0;f.Burn=0;f.Weak=0;}
        }
        public bool Upgrade(int owner,out string error)
        {
            error="";
            if(owner<0||owner>2||!PlayerTurn||Outcome!=0){error="Não é possível forjar agora.";return false;}
            var f=Fighters[owner];int price=f.Tier+1;
            if(!f.Alive||f.Tier>=2){error="Guerreiro indisponível ou equipamento no máximo.";return false;}
            if(Energy<1||Shards<price){error="Requer 1 energia e "+price+" fragmento(s).";return false;}
            Energy--;Shards-=price;f.Tier++;TotalPlayed++;
            Say(f.Name+" melhorou "+GearName(owner)+" para nível "+(f.Tier+1)+".");return true;
        }
        public bool ChangeGear(int owner)
        {
            if(owner<0||owner>2||Round!=1||TotalPlayed!=0||!PlayerTurn||Outcome!=0)return false;
            Fighters[owner].Gear=1-Fighters[owner].Gear;return true;
        }
        void Tick(bool enemy)
        {
            foreach(var f in Fighters.Where(f=>f.Enemy==enemy&&f.Alive)) {
                f.Shield=0;
                if(f.Burn>0){ Damage(f,6,true); f.Burn=Math.Max(0,f.Burn-1);Say(f.Name+" sofreu 6 de queimadura."); }
                f.Weak=Math.Max(0,f.Weak-1);
            }
        }
        public void BeginEnemy()
        {
            if(!PlayerTurn||Outcome!=0)return;
            PlayerTurn=false;Discard.AddRange(Hand);Hand.Clear();Tick(true);CheckOutcome();
        }
        public bool ExecuteIntent(int index)
        {
            if(PlayerTurn||Outcome!=0||index<0||index>=Intents.Count)return false;
            var intent=Intents[index];var actor=Fighters[intent.Owner];if(!actor.Alive)return false;
            int target=intent.Target;
            if(!Fighters[target].Alive) target=Fighters.FindIndex(f=>f.Alive && f.Enemy==(intent.Effect==Effect.Guard));
            if(target<0){CheckOutcome();return false;}
            int power=actor.Weak>0?(int)(intent.Power*.75f):intent.Power;
            Resolve(intent.Effect,power,target,true);
            Say(actor.Name+" • "+intent.Label+" → "+Fighters[target].Name);
            CheckOutcome();return true;
        }
        public void BeginPlayer()
        {
            if(PlayerTurn||Outcome!=0)return;
            Round++;PlayerTurn=true;Energy=4;Shards++;Played=0;contributors.Clear();HarmonyUsed=false;
            Tick(false);
            // Regra própria de ritmo: fadiga, não a Blood Moon de Axie. Dano verdadeiro simétrico após a rodada 12.
            // Own pacing rule: fatigue, not Axie's Blood Moon. Symmetric true damage after round 12.
            if(Round>12){int pressure=(Round-12)*5;foreach(var f in Fighters)Damage(f,pressure,true);Say("Instabilidade do santuário: "+pressure+" de dano a todos.");}
            CheckOutcome();if(Outcome!=0)return;
            for(int i=0;i<6;i++)Draw();Plan();
        }
        void Draw()
        {
            // Cartas de guerreiros derrotados saem da circulação, evitando mãos sem opções úteis.
            // Cards belonging to fallen warriors leave circulation, avoiding dead-hand soft locks.
            Deck.RemoveAll(c=>!Fighters[c.Owner].Alive);Discard.RemoveAll(c=>!Fighters[c.Owner].Alive);
            if(Deck.Count==0){Deck.AddRange(Discard);Discard.Clear();Shuffle();}
            if(Deck.Count==0)return;var c=Deck[0];Deck.RemoveAt(0);Hand.Add(c);
        }
        void Shuffle(){for(int i=Deck.Count-1;i>0;i--){int j=random.Next(i+1);var x=Deck[i];Deck[i]=Deck[j];Deck[j]=x;}}
        public void Plan()
        {
            Intents.Clear(); var living=Fighters.Where(f=>!f.Enemy&&f.Alive).ToList();if(living.Count==0)return;
            foreach(var f in Fighters.Where(f=>f.Enemy&&f.Alive)) {
                int target=living[(Round+f.Id)%living.Count].Id;
                var fx=Effect.Strike;int p=38+(Round-1)/3*2;string label="Ataque";
                if(f.Id==3 && Round%3==0){fx=Effect.Guard;p=32;target=f.Id;label="Fortificar";}
                else if(f.Id==4){fx=Round%2==1?Effect.Burn:Effect.Volley;p=fx==Effect.Volley?18:26;label=fx==Effect.Volley?"Onda sombria":"Chama violeta";}
                else if(f.Id==5){fx=Effect.Pierce;p=27+(Round-1)/4;label="Emboscada";}
                Intents.Add(new Intent {Owner=f.Id,Target=target,Power=p,Effect=fx,Label=label});
            }
        }
        public void CheckOutcome()
        {
            bool a=Fighters.Any(f=>!f.Enemy&&f.Alive),b=Fighters.Any(f=>f.Enemy&&f.Alive);
            Outcome=!a&&!b?2:!b?1:!a?-1:0;
        }
        public void Say(string message){Log.Add(message);if(Log.Count>60)Log.RemoveAt(0);}
    }
}
