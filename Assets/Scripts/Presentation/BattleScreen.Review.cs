using System;
using System.Linq;
using UnityEngine;
using RunePact.Core;

namespace RunePact.Presentation
{
    public sealed partial class BattleScreen
    {
        bool reviewMode=false;
        bool TryReview()
        {
#if UNITY_EDITOR || RUNEPACT_REVIEW
            var args=Environment.GetCommandLineArgs();
            string mode=args.FirstOrDefault(a=>a.StartsWith("-runepact-review-",StringComparison.Ordinal));
#if RUNEPACT_REVIEW
            if(mode==null)mode="-runepact-review-cards";
#endif
            if(mode==null)return false;
            // Cenários visuais de QA isolados: nunca gravam ou substituem a jornada do jogador.
            //
            // Isolated visual QA scenarios: never save or replace the player's journey.
            reviewMode=true;firstLaunch=false;journey=new Journey(7241);Match=journey.Current;Time.timeScale=animationSpeed;
            if(mode=="-runepact-review-rewards"||mode=="-runepact-review-boss"){
                foreach(var f in Match.Fighters.Where(f=>f.Enemy))f.Hp=0;Match.CheckOutcome();
                if(mode=="-runepact-review-boss"){journey.ChooseReward(0);Match=journey.Current;Refresh();}
                else{Refresh();ShowRewards();}
            }else if(mode=="-runepact-review-cards"){
                Match.Hand.Clear();Match.Hand.AddRange(new[]{new Card(0,0),new Card(0,1),new Card(1,1),new Card(2,2),new Card(0,1,CardVariant.Oath),new Card(1,0,CardVariant.Tremor)});
                Match.Energy=8;Refresh();
            }else{
                Match.Resolve(Effect.Fortify,28,0,false);Match.Resolve(Effect.Regenerate,12,1,false);Match.Resolve(Effect.Channel,14,2,false);
                Match.Resolve(Effect.Burn,4,3,false);Match.Resolve(Effect.Weaken,4,4,false);Match.Resolve(Effect.Mark,4,5,false);Refresh();
            }
            return true;
#else
            return false;
#endif
        }
    }
}
