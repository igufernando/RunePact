using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using RunePact.Core;

namespace RunePact.Presentation
{
    public sealed class FighterDropTarget : MonoBehaviour
    {
        public int Id;
    }

    public sealed class CardDragHandler : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public int Index;
        public void OnPointerDown(PointerEventData eventData) { if(BattleScreen.Active!=null)BattleScreen.Active.BeginCardDrag(Index); }
        public void OnBeginDrag(PointerEventData eventData) { if(BattleScreen.Active!=null)BattleScreen.Active.BeginCardDrag(Index); }
        public void OnDrag(PointerEventData eventData) { }
        public void OnEndDrag(PointerEventData eventData)
        {
            var hits=new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData,hits);
            foreach(var hit in hits){
                var target=hit.gameObject.GetComponentInParent<FighterDropTarget>();
                if(target==null)continue;
                if(BattleScreen.Active!=null)BattleScreen.Active.DragCardToTarget(Index,target.Id);
                return;
            }
        }
    }

    public sealed partial class BattleScreen
    {
        RectTransform portraitPrompt;
        public void DragCardToTarget(int index,int target)
        {
            if(busy||overlay!=null||Match.Outcome!=0||index<0||index>=Match.Hand.Count)return;
            if(selected!=index)SelectCard(index);
            if(selected==index)ClickUnit(target);
        }
        public void BeginCardDrag(int index)
        {
            if(busy||overlay!=null||index<0||index>=Match.Hand.Count)return;
            var card=Match.Hand[index];if(!Match.Fighters[card.Owner].Alive||Match.Energy<Match.GetCost(card))return;
            selected=index;
            for(int i=0;i<outlines.Length;i++)outlines[i].color=Match.CanTarget(card,i)?gold:Color.clear;
            UpdateHint();
        }
        static string DifficultyName(EncounterDifficulty value)
        {
            return value==EncounterDifficulty.Calm?"CALMA":value==EncounterDifficulty.Fierce?"INTENSA":"NORMAL";
        }
        void RecordRun()
        {
            if(reviewMode||journey.StatsRecorded||journey.AwaitingReward)return;
            journey.StatsRecorded=true;
            PlayerPrefs.SetInt("RunePactRuns",PlayerPrefs.GetInt("RunePactRuns",0)+1);
            if(journey.Complete)PlayerPrefs.SetInt("RunePactWins",PlayerPrefs.GetInt("RunePactWins",0)+1);
            PlayerPrefs.SetInt("RunePactSeconds",PlayerPrefs.GetInt("RunePactSeconds",0)+(int)journey.ElapsedSeconds);
            PlayerPrefs.SetInt("RunePactCards",PlayerPrefs.GetInt("RunePactCards",0)+journey.TotalCardsPlayed);
            SaveJourney();
        }
        void UpdatePortraitPrompt(Rect safe)
        {
            bool portrait=safe.height>safe.width;
            if(!portrait){if(portraitPrompt!=null)portraitPrompt.gameObject.SetActive(false);return;}
            if(portraitPrompt==null){
                portraitPrompt=Box(root.parent,"Rotate device",0,0,900,1600,ink);
                portraitPrompt.GetComponent<Image>().raycastTarget=true;
                portraitPrompt.anchorMin=Vector2.zero;portraitPrompt.anchorMax=Vector2.one;
                portraitPrompt.pivot=new Vector2(.5f,.5f);portraitPrompt.anchoredPosition=Vector2.zero;portraitPrompt.sizeDelta=Vector2.zero;
                var label=Label(portraitPrompt,"R U N E P A C T\n\nGire o aparelho para jogar na horizontal.\nSeu progresso foi preservado.",0,0,700,260,25,cream,TextAnchor.MiddleCenter,true);
                label.rectTransform.anchorMin=label.rectTransform.anchorMax=new Vector2(.5f,.5f);
                label.rectTransform.pivot=new Vector2(.5f,.5f);label.rectTransform.anchoredPosition=Vector2.zero;
            }
            portraitPrompt.gameObject.SetActive(true);portraitPrompt.SetAsLastSibling();
        }
        void ShowCatalog()
        {
            if(busy||rewardOpen)return;
            var panelRoot=Modal("CATÁLOGO DE CARTAS",630);
            Label(panelRoot,"Cartas base, habilidades únicas e equipamentos. Recompensas ficam disponíveis durante a jornada.",32,70,1060,32,17,muted,TextAnchor.MiddleCenter);
            for(int owner=0;owner<3;owner++){
                var column=Surface(panelRoot,"Warrior cards",25+owner*365,122,350,452,panel);
                var hero=Match.Fighters[owner];
                Picture(column,sprites[owner],10,8,65,68);
                Label(column,hero.Name,83,8,250,37,23,gold,TextAnchor.MiddleLeft,true);
                var list=new[]{new Card(owner,0),new Card(owner,1),new Card(owner,2),new Card(owner,1,(CardVariant)((int)CardVariant.Oath+owner))};
                for(int index=0;index<list.Length;index++){
                    var ability=Match.Describe(list[index]);
                    int y=82+index*86;
                    var entry=Round(column,"Entry",10,y,330,78,index==3?new Color(.37f,.34f,.24f):ink);
                    Picture(entry,AbilityPortrait(owner,ability.Effect),5,4,60,65);
                    Orb(entry,AbilityIcon(ability.Effect,owner),48,51,17,17,gold);
                    Label(entry,ability.Name+(index==3?" • RARA":""),73,3,248,24,15,cream,TextAnchor.MiddleLeft,true);
                    Label(entry,ability.Cost+" energia • "+AbilityPurpose(ability.Effect,owner),73,26,248,16,10,gold,TextAnchor.MiddleLeft);
                    Label(entry,ability.Description,73,43,248,33,11,muted,TextAnchor.UpperLeft);
                }
                Label(column,"Especial de equipamento: "+Match.GearName(owner),13,433,324,18,12,gold,TextAnchor.MiddleCenter);
            }
            string unlocked=journey.ExtraCards.Count==0?"nenhuma ainda":string.Join(", ",journey.ExtraCards.ConvertAll(card=>Match.Describe(card).Name).ToArray());
            Label(panelRoot,"Cartas de jornada desbloqueadas: "+unlocked,36,586,1058,30,14,gold,TextAnchor.MiddleCenter);
        }
    }
}
