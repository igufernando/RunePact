using System;
using System.Collections.Generic;
using System.Linq;

namespace RunePact.Core
{
    public enum Effect { Strike, Guard, Mend, Burn, Pierce, Volley, Rally, Mark, Fortify, Channel }
    public enum EnemyFormation { Vanguard, Shieldwall, Hunt, Pyre, Regent }
    public enum EnemyRole { Brute, Acolyte, Assassin, Sentinel, Pyromancer, Regent }
    public enum Relic { PactMedallion, AncientEmber, LivingGrimoire, CeremonialArrow, OakBark }
    public enum CardVariant { Standard, Bulwark, Comet, Renewal }
    public sealed class Fighter
    {
        public int Id, MaxHp, Hp, Shield, Burn, Weak, Tier, Gear, Mark, Fortified, Channel;
        public string Name;
        public bool Enemy;
        public EnemyRole Role;
        public bool Alive => Hp > 0;
        public Fighter(int id, string name, int hp, bool enemy) { Id=id; Name=name; MaxHp=Hp=hp; Enemy=enemy; }
    }
    public sealed class Card
    {
        public int Owner, Slot;
        public CardVariant Variant;
        public Card(int owner,int slot,CardVariant variant=CardVariant.Standard) { Owner=owner; Slot=slot; Variant=variant; }
    }
    public sealed class Ability
    {
        public string Name, Source, Description;
        public int Cost, Power;
        public Effect Effect;
        public bool Friendly => Effect==Effect.Guard || Effect==Effect.Mend || Effect==Effect.Rally || Effect==Effect.Fortify || Effect==Effect.Channel;
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
        readonly HashSet<int> oakTouched = new HashSet<int>();
        public readonly List<Relic> Relics;
        public readonly int[] CardBoost;
        public int Round=1, Energy=4, Shards=3, Played, TotalPlayed, CardsPlayed;
        public bool PlayerTurn=true, HarmonyUsed;
        // 0 em andamento, 1 vitória, -1 derrota, 2 empate.
        // 0 ongoing, 1 victory, -1 defeat, 2 draw.
        public int Outcome;
        public int Harmony => contributors.Count;
        public readonly int Encounter;
        public readonly EnemyFormation Formation;
        public int BossPhase { get; private set; } = 1;
        public Battle(int seed=0, int[] gear=null, int encounter=1, EnemyFormation formation=EnemyFormation.Vanguard, IEnumerable<Relic> relics=null, IEnumerable<Card> extraCards=null, int[] cardBoost=null)
        {
            Encounter=encounter;
            Formation=formation;
            Relics=relics==null?new List<Relic>():new List<Relic>(relics);
            CardBoost=cardBoost==null?new int[3]:new[]{cardBoost[0],cardBoost[1],cardBoost[2]};
            random=new Random(seed);
            Fighters.Add(new Fighter(0,"AURA",145,false));
            Fighters.Add(new Fighter(1,"LYRA",115,false));
            Fighters.Add(new Fighter(2,"KAEL",125,false));
            CreateEnemies(formation,encounter);
            for(int i=0;i<3;i++) {
                Fighters[i].Gear=gear==null?0:gear[i];
                for(int copy=0;copy<2;copy++) for(int slot=0;slot<3;slot++) Deck.Add(new Card(i,slot));
            }
            if(extraCards!=null)foreach(var card in extraCards)Deck.Add(new Card(card.Owner,card.Slot,card.Variant));
            Shuffle();
            // A primeira mão ensina os três papéis e equipamentos; mãos posteriores usam o baralho embaralhado.
            // First hand teaches all three roles and equipment; later hands use the shuffled deck.
            for(int i=0;i<3;i++) foreach(int slot in new[]{0,2}) { var c=Deck.First(x=>x.Owner==i&&x.Slot==slot); Deck.Remove(c); Hand.Add(c); }
            Plan(); Say("Santuário do Crepúsculo • escolha uma carta e depois um alvo.");
        }
        void CreateEnemies(EnemyFormation formation,int encounter)
        {
            int scale=(encounter-1)*18;
            if(formation==EnemyFormation.Shieldwall){AddEnemy(3,"SENTINELA",EnemyRole.Sentinel,174+scale);AddEnemy(4,"SENTINELA",EnemyRole.Sentinel,150+scale);AddEnemy(5,"CANTOR",EnemyRole.Acolyte,118+scale);return;}
            if(formation==EnemyFormation.Hunt){AddEnemy(3,"CAÇADOR",EnemyRole.Assassin,132+scale);AddEnemy(4,"CAÇADOR",EnemyRole.Assassin,122+scale);AddEnemy(5,"ACÓLITO",EnemyRole.Acolyte,130+scale);return;}
            if(formation==EnemyFormation.Pyre){AddEnemy(3,"PIROMANTE",EnemyRole.Pyromancer,138+scale);AddEnemy(4,"BRUTAMONTES",EnemyRole.Brute,172+scale);AddEnemy(5,"CINZA",EnemyRole.Pyromancer,118+scale);return;}
            if(formation==EnemyFormation.Regent){AddEnemy(3,"REGENTE",EnemyRole.Regent,280);AddEnemy(4,"GUARDIÃO RÚNICO",EnemyRole.Sentinel,145);AddEnemy(5,"ACÓLITO",EnemyRole.Acolyte,135);return;}
            AddEnemy(3,"BRUTAMONTES",EnemyRole.Brute,155+scale);AddEnemy(4,"ACÓLITO",EnemyRole.Acolyte,120+scale);AddEnemy(5,"ASSASSINO",EnemyRole.Assassin,115+scale);
        }
        void AddEnemy(int id,string name,EnemyRole role,int hp){var fighter=new Fighter(id,name,hp,true);fighter.Role=role;Fighters.Add(fighter);}
        public string GearName(int owner)
        {
            string[,] names={{"Escudo do pacto","Espada de brasa"},{"Chapéu da aurora","Cajado de cinzas"},{"Besta perfurante","Aljava de ecos"}};
            return names[owner,Fighters[owner].Gear];
        }
        public Ability Describe(Card card)
        {
            var f=Fighters[card.Owner];
            if(card.Variant==CardVariant.Bulwark)return Make("Muralha viva","Carta de jornada",Effect.Fortify,24+CardBoost[card.Owner],1,"Concede {0} de escudo e preserva-o por um turno.");
            if(card.Variant==CardVariant.Comet)return Make("Cometa de âmbar","Carta de jornada",Effect.Volley,20+CardBoost[card.Owner],2,"Causa {0} de dano a todos os inimigos.");
            if(card.Variant==CardVariant.Renewal)return Make("Rito de renovação","Carta de jornada",Effect.Mend,27+CardBoost[card.Owner],1,"Restaura {0} de vida e remove queimadura.");
            if(card.Slot==0) return Make(new[]{"Corte preciso","Pulso arcano","Marca rúnica"}[card.Owner],"Habilidade base",card.Owner==2?Effect.Mark:Effect.Strike,new[]{28,25,24}[card.Owner]+CardBoost[card.Owner],1,card.Owner==2?"Causa {0} de dano. Marca: próximo ataque recebe +10 de dano.":"Causa {0} de dano a um inimigo.");
            if(card.Slot==1) {
                if(card.Owner==0)return Make("Proteção firme","Guardião",Effect.Fortify,28,1,"Concede {0} de escudo. Fortificado: preserva escudo por um turno.");
                if(card.Owner==1)return Make("Canalizar","Arcanista",Effect.Channel,14,1,"Próximo ataque do aliado recebe +{0} de dano. Não acumula.");
                return Make("Cobertura","Batedor",Effect.Guard,24,1,"Concede {0} de escudo a um aliado até seu próximo turno.");
            }
            int n=f.Tier;
            if(card.Owner==0 && f.Gear==0) return Make("Bastião",GearName(0),Effect.Rally,19+n*7,2,"Todos os aliados recebem {0} de escudo. Remove fraqueza.");
            if(card.Owner==0) return Make("Corte de brasa",GearName(0),Effect.Burn,31+n*8,1,"Causa {0} de dano e aplica 2 turnos de queimadura (6/turno).");
            if(card.Owner==1 && f.Gear==0) return Make("Luz restauradora",GearName(1),Effect.Mend,32+n*9,1,"Restaura {0} de vida de um aliado e remove queimadura.");
            if(card.Owner==1) return Make("Chuva de cinzas",GearName(1),Effect.Volley,22+n*7,2,"Causa {0} de dano a todos os inimigos vivos.");
            if(f.Gear==0) return Make("Virote rúnico",GearName(2),Effect.Pierce,34+n*8,1,"Causa {0} de dano, ignorando o escudo.");
            return Make("Salva de ecos",GearName(2),Effect.Volley,23+n*7,2,"Causa {0} de dano a todos os inimigos vivos.");
        }
        static Ability Make(string name,string source,Effect fx,int power,int cost,string text) => new Ability {Name=name,Source=source,Effect=fx,Power=power,Cost=cost,Description=string.Format(text,power)};
        public int GetCost(Card card)
        {
            var ability=Describe(card);
            return Relics.Contains(Relic.LivingGrimoire)&&card.Owner==1&&Played==0?Math.Max(0,ability.Cost-1):ability.Cost;
        }
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
            int cost=GetCost(card);
            if(Energy<cost){error="Energia insuficiente.";return false;}
            if(!CanTarget(card,target)){error=a.Friendly?"Escolha um aliado vivo.":"Escolha um inimigo vivo.";return false;}
            Energy-=cost; Hand.RemoveAt(index); Discard.Add(card); Played++; TotalPlayed++; CardsPlayed++;
            int power=caster.Weak>0?(int)(a.Power*0.75f):a.Power;
            if(!a.Friendly){power+=caster.Channel;caster.Channel=0;}
            Resolve(a.Effect,power,target,false);
            Say(caster.Name+" • "+a.Name+" → "+(a.Effect==Effect.Volley||a.Effect==Effect.Rally?"equipe":Fighters[target].Name));
            contributors.Add(card.Owner);
            if(contributors.Count==3&&!HarmonyUsed){ HarmonyUsed=true; Energy++; if(Relics.Contains(Relic.PactMedallion))foreach(var hero in Fighters.Where(x=>!x.Enemy&&x.Alive))hero.Shield+=8; Say(Relics.Contains(Relic.PactMedallion)?"PACTO! +1 energia e +8 escudo para o grupo.":"PACTO! Três guerreiros agiram: +1 energia."); }
            CheckOutcome(); return true;
        }
        public void Resolve(Effect fx,int power,int target,bool enemy)
        {
            Fighter t=Fighters[target];
            switch(fx) {
                case Effect.Guard: Shield(t,power); break;
                case Effect.Fortify: Shield(t,power);t.Fortified=1;break;
                case Effect.Channel: t.Channel=Math.Max(t.Channel,power);break;
                case Effect.Mark: Hit(t,power,false);if(t.Alive)t.Mark=1;break;
                case Effect.Rally: foreach(var f in Fighters.Where(f=>f.Alive&&f.Enemy==enemy)){Shield(f,power);f.Weak=0;} break;
                case Effect.Mend: t.Hp=Math.Min(t.MaxHp,t.Hp+power);t.Burn=0;break;
                case Effect.Volley: foreach(var f in Fighters.Where(f=>f.Alive&&f.Enemy!=enemy)) Hit(f,power,false);break;
                case Effect.Pierce: Hit(t,power,true);break;
                case Effect.Burn: Hit(t,power,false);if(t.Alive)t.Burn=Math.Max(2,t.Burn);break;
                default: Hit(t,power,false);break;
            }
        }
        void Shield(Fighter target,int power)
        {
            if(Relics.Contains(Relic.OakBark)&&!oakTouched.Contains(target.Id)){power+=10;oakTouched.Add(target.Id);}
            target.Shield+=power;
        }
        void Hit(Fighter target,int power,bool pierce)
        {
            if(target.Mark>0){power+=Relics.Contains(Relic.CeremonialArrow)?15:10;target.Mark=0;}
            Damage(target,power,pierce);
        }
        public static void Damage(Fighter f,int amount,bool pierce)
        {
            if(!f.Alive)return;
            int absorb=pierce?0:Math.Min(f.Shield,amount);f.Shield-=absorb;f.Hp=Math.Max(0,f.Hp-(amount-absorb));
            if(!f.Alive){f.Shield=0;f.Burn=0;f.Weak=0;f.Mark=0;f.Fortified=0;f.Channel=0;}
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
                if(f.Fortified>0)f.Fortified--;else f.Shield=0;
                if(f.Burn>0){ int burn=Relics.Contains(Relic.AncientEmber)&&!enemy?9:6;Damage(f,burn,true); f.Burn=Math.Max(0,f.Burn-1);Say(f.Name+" sofreu "+burn+" de queimadura."); }
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
            bool friendly=intent.Effect==Effect.Guard||intent.Effect==Effect.Mend||intent.Effect==Effect.Rally;
            if(!Fighters[target].Alive) target=Fighters.FindIndex(f=>f.Alive && f.Enemy==friendly);
            if(target<0){CheckOutcome();return false;}
            int power=actor.Weak>0?(int)(intent.Power*.75f):intent.Power;
            Resolve(intent.Effect,power,target,true);
            Say(actor.Name+" • "+intent.Label+" → "+Fighters[target].Name);
            CheckOutcome();return true;
        }
        public void BeginPlayer()
        {
            if(PlayerTurn||Outcome!=0)return;
            Round++;PlayerTurn=true;Energy=4;Shards++;Played=0;contributors.Clear();oakTouched.Clear();HarmonyUsed=false;
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
                if(f.Role==EnemyRole.Regent) {
                    if(f.Hp<=f.MaxHp/2&&BossPhase==1){BossPhase=2;f.Shield+=35;Say("O REGENTE entra na FASE 2: a runa desperta.");}
                    target=living.OrderBy(x=>x.Hp).First().Id;
                    if(BossPhase==2&&Round%2==0){fx=Effect.Volley;p=32;label="Tempestade rúnica";}
                    else if(Round%3==0){fx=Effect.Volley;p=26;label="Ruptura do regente";}
                    else {p=44+BossPhase*7;label="Lâmina do trono";}
                }
                else if(f.Role==EnemyRole.Brute) {
                    target=living.OrderByDescending(x=>x.Shield).ThenBy(x=>x.Id).First().Id;
                    if(Round%2==1){fx=Effect.Guard;p=22;target=f.Id;label="Preparar golpe";}
                    else {p=45+(Encounter-1)*3;label="Golpe pesado";}
                }
                else if(f.Role==EnemyRole.Acolyte){
                    if(Round%2==0){fx=Effect.Mend;p=24;target=Fighters.Where(x=>x.Enemy&&x.Alive).OrderBy(x=>(float)x.Hp/x.MaxHp).First().Id;label="Restaurar";}
                    else {fx=Effect.Burn;p=20;label="Chama violeta";}
                }
                else if(f.Role==EnemyRole.Assassin){fx=Effect.Pierce;p=22+(Encounter-1)*2;target=living.OrderBy(x=>x.Hp).First().Id;label="Caçar ferido";}
                else if(f.Role==EnemyRole.Sentinel){if(Round%2==1){fx=Effect.Guard;p=25;target=f.Id;label="Muralha rúnica";}else{p=30;label="Investida";}}
                else if(f.Role==EnemyRole.Pyromancer){fx=Round%2==0?Effect.Volley:Effect.Burn;p=fx==Effect.Volley?16:24;label=fx==Effect.Volley?"Chuva de brasas":"Chama cinerária";}
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
