using System;
using System.IO;
using System.Linq;

namespace RunePact.Core
{
    public sealed partial class Battle
    {
        internal void WriteSnapshot(BinaryWriter w)
        {
            w.Write(randomState);w.Write(mageUsed);w.Write(Round);w.Write(Energy);w.Write(Shards);
            w.Write(Played);w.Write(TotalPlayed);w.Write(CardsPlayed);w.Write(PlayerTurn);w.Write(HarmonyUsed);w.Write(Outcome);w.Write(BossPhase);
            for(int i=0;i<3;i++){w.Write(contributors.Contains(i));w.Write(oakTouched.Contains(i));}
            foreach(int value in SkillBoost)w.Write(value);
            foreach(var f in Fighters){
                foreach(int value in new[]{f.Hp,f.MaxHp,f.Shield,f.Burn,f.Weak,f.Tier,f.Gear,f.Mark,f.Fortified,f.Channel,f.Vulnerable,f.Regen,f.Thorns,f.Stun})w.Write(value);
            }
            WriteCards(w,Hand);WriteCards(w,Deck);WriteCards(w,Discard);
            w.Write(Intents.Count);foreach(var i in Intents){w.Write(i.Owner);w.Write(i.Target);w.Write(i.Power);w.Write((int)i.Effect);w.Write(i.Label);}
            w.Write(Log.Count);foreach(string line in Log)w.Write(line);
        }
        internal void ReadSnapshot(BinaryReader r)
        {
            randomState=r.ReadUInt32();if(randomState==0)throw new InvalidDataException();
            mageUsed=r.ReadBoolean();Round=ReadNumber(r,1,1000);Energy=ReadNumber(r,0,10);Shards=ReadNumber(r,0,10000);
            Played=ReadNumber(r,0,1000);TotalPlayed=ReadNumber(r,0,100000);CardsPlayed=ReadNumber(r,0,100000);
            PlayerTurn=r.ReadBoolean();HarmonyUsed=r.ReadBoolean();Outcome=ReadNumber(r,-1,2);BossPhase=ReadNumber(r,1,2);
            contributors.Clear();oakTouched.Clear();
            for(int i=0;i<3;i++){if(r.ReadBoolean())contributors.Add(i);if(r.ReadBoolean())oakTouched.Add(i);}
            for(int i=0;i<9;i++)SkillBoost[i]=ReadNumber(r,0,60);
            foreach(var f in Fighters){
                f.Hp=ReadNumber(r,0,10000);f.MaxHp=ReadNumber(r,1,10000);if(f.Hp>f.MaxHp)throw new InvalidDataException();
                f.Shield=ReadNumber(r,0,10000);f.Burn=ReadNumber(r,0,10);f.Weak=ReadNumber(r,0,2);f.Tier=ReadNumber(r,0,2);f.Gear=ReadNumber(r,0,1);
                f.Mark=ReadNumber(r,0,1);f.Fortified=ReadNumber(r,0,1);f.Channel=ReadNumber(r,0,100);
                f.Vulnerable=ReadNumber(r,0,2);f.Regen=ReadNumber(r,0,2);f.Thorns=ReadNumber(r,0,2);f.Stun=ReadNumber(r,0,1);
            }
            Hand.Clear();Hand.AddRange(ReadCards(r,6));Deck.Clear();Deck.AddRange(ReadCards(r,40));Discard.Clear();Discard.AddRange(ReadCards(r,40));
            Intents.Clear();int count=ReadNumber(r,0,3);
            for(int n=0;n<count;n++){
                int owner=ReadNumber(r,3,5),target=ReadNumber(r,0,5),power=ReadNumber(r,0,1000),effect=ReadNumber(r,0,(int)Effect.Execute);
                string label=r.ReadString();if(label.Length>100)throw new InvalidDataException();
                Intents.Add(new Intent{Owner=owner,Target=target,Power=power,Effect=(Effect)effect,Label=label});
            }
            Log.Clear();count=ReadNumber(r,0,60);
            for(int n=0;n<count;n++){string line=r.ReadString();if(line.Length>500)throw new InvalidDataException();Log.Add(line);}
            int expected=Outcome;CheckOutcome();if(expected!=Outcome||!PlayerTurn&&Outcome==0)throw new InvalidDataException();
        }
        internal static int ReadNumber(BinaryReader r,int min,int max){int n=r.ReadInt32();if(n<min||n>max)throw new InvalidDataException();return n;}
        internal static void WriteCards(BinaryWriter w,System.Collections.Generic.IEnumerable<Card> cards)
        {var list=cards.ToList();w.Write(list.Count);foreach(var c in list){w.Write(c.Owner);w.Write(c.Slot);w.Write((int)c.Variant);}}
        internal static Card[] ReadCards(BinaryReader r,int max)
        {int count=ReadNumber(r,0,max);var cards=new Card[count];for(int i=0;i<count;i++)cards[i]=new Card(ReadNumber(r,0,2),ReadNumber(r,0,2),(CardVariant)ReadNumber(r,0,(int)CardVariant.Execution));return cards;}
    }
}
