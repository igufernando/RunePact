using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RunePact.Presentation
{
    public static class FantasySkin
    {
        public static Sprite Rounded { get; private set; }
        public static Sprite Circle { get; private set; }
        public static Sprite Ring { get; private set; }
        public static Sprite Spark { get; private set; }
        public static Sprite Shield { get; private set; }
        public static Sprite Slash { get; private set; }
        public static Sprite Leaf { get; private set; }
        public static Sprite Flame { get; private set; }
        public static Sprite Arrow { get; private set; }
        public static Sprite Sword { get; private set; }
        public static Sprite Heart { get; private set; }
        public static Sprite Rune { get; private set; }
        public static Sprite Frame { get; private set; }

        public static void Initialize()
        {
            if (Rounded != null) return;
            // Formas de interface geradas em memória; os assets aprovados não são modificados.
            //
            // Interface shapes are generated in memory; approved artwork is never modified.
            Rounded = Create((x,y) => {
                float dx=Mathf.Max(Mathf.Abs(x)-.64f,0),dy=Mathf.Max(Mathf.Abs(y)-.64f,0);
                return .34f-Mathf.Sqrt(dx*dx+dy*dy);
            }, true);
            Circle = Create((x,y) => .94f-Mathf.Sqrt(x*x+y*y));
            Ring = Create((x,y) => .07f-Mathf.Abs(Mathf.Sqrt(x*x+y*y)-.80f));
            Spark = Create((x,y) => .78f-Mathf.Sqrt(Mathf.Abs(x))-Mathf.Sqrt(Mathf.Abs(y)));
            Shield = Create((x,y) => Mathf.Min(.72f-Mathf.Abs(x), Mathf.Min(.80f-y, y+.86f-Mathf.Abs(x)*.65f)));
            Slash = Create((x,y) => {
                float radius=Mathf.Sqrt(x*x+y*y);
                return Mathf.Min(.09f-Mathf.Abs(radius-.70f),x+.20f);
            });
            Leaf = Create((x,y) => .70f-(x*x*2.4f+y*y));
            Flame = Create((x,y) => {
                float width=.54f*Mathf.Clamp01((.92f-y)/1.15f);
                return Mathf.Min(width-Mathf.Abs(x-Mathf.Sin(y*4)*.07f),y+.80f);
            });
            Arrow = Create((x,y) => Mathf.Max(Mathf.Min(.74f-Mathf.Abs(x),.055f-Mathf.Abs(y)),
                Mathf.Min(x-.10f,Mathf.Min(.84f-x,(.84f-x)*.70f-Mathf.Abs(y)))));
            Sword=Pattern(new[]{"..............##",".............###","............###.","...........###..","..........###...",".........###....","........###.....","...##..###......","....#####.......",".....###........","....#####.......","...###..##......","..###...........",".###............","###.............","##.............."});
            Shield=Pattern(new[]{"................","..############..","..############..","..###......###..","..###..##..###..","..###..##..###..","..########.###..","..########.###..","...##..##..##...","...##..##..##...","....##.##.##....","....##.##.##....",".....######.....","......####......",".......##.......","................"});
            Heart=Pattern(new[]{"................","..####....####..",".######..######.","################","################","################","################",".##############.","..############..","...##########...","....########....",".....######.....","......####......",".......##.......","................","................"});
            Rune=Pattern(new[]{".......##.......","......####......",".....##..##.....","....##.##.##....","...##..##..##...","..##...##...##..",".##....##....##.","##.##########.##","##.##########.##",".##....##....##.","..##...##...##..","...##..##..##...","....##.##.##....",".....##..##.....","......####......",".......##......."});
            Frame=CreateFrame();
        }

        static Sprite Pattern(string[] rows)
        {
            int size=rows.Length;var texture=new Texture2D(size,size,TextureFormat.RGBA32,false);
            texture.filterMode=FilterMode.Point;texture.wrapMode=TextureWrapMode.Clamp;
            var colors=new Color[size*size];
            for(int y=0;y<size;y++)for(int x=0;x<size;x++)colors[(size-1-y)*size+x]=rows[y][x]=='#'?Color.white:Color.clear;
            texture.SetPixels(colors);texture.Apply(false,true);
            return Sprite.Create(texture,new Rect(0,0,size,size),new Vector2(.5f,.5f),100);
        }
        static Sprite CreateFrame()
        {
            const int size=48;var texture=new Texture2D(size,size,TextureFormat.RGBA32,false);
            texture.filterMode=FilterMode.Point;texture.wrapMode=TextureWrapMode.Clamp;
            var colors=new Color[size*size];
            for(int y=0;y<size;y++)for(int x=0;x<size;x++){
                int edge=Mathf.Min(x,y,size-1-x,size-1-y);
                int corner=Mathf.Min(x,size-1-x)+Mathf.Min(y,size-1-y);
                bool inside=edge>=1&&corner>=9;
                bool inner=edge>=5&&corner>=16;
                colors[y*size+x]=inside&&!inner?new Color(1,1,1,edge==1||corner==9?.55f:1):Color.clear;
            }
            texture.SetPixels(colors);texture.Apply(false,true);
            return Sprite.Create(texture,new Rect(0,0,size,size),new Vector2(.5f,.5f),100,0,SpriteMeshType.FullRect,new Vector4(13,13,13,13));
        }

        static Sprite Create(System.Func<float,float,float> shape,bool sliced=false)
        {
            const int size=32;
            var tex=new Texture2D(size,size,TextureFormat.RGBA32,false);
            tex.name="RunePact pixel UI";tex.filterMode=FilterMode.Point;tex.wrapMode=TextureWrapMode.Clamp;
            var pixels=new Color[size*size];
            for(int y=0;y<size;y++)for(int x=0;x<size;x++)
                pixels[y*size+x]=new Color(1,1,1,shape((x+.5f)/size*2-1,(y+.5f)/size*2-1)>=0?1:0);
            tex.SetPixels(pixels);tex.Apply(false,true);
            return Sprite.Create(tex,new Rect(0,0,size,size),new Vector2(.5f,.5f),100,0,SpriteMeshType.FullRect,
                sliced?new Vector4(8,8,8,8):Vector4.zero);
        }
    }

    public sealed class FriendlyMotion : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public float Lift;
        RectTransform rect;
        Vector2 rest;
        bool hovered,pressed;
        void Start(){rect=(RectTransform)transform;rest=rect.anchoredPosition;}
        void Update()
        {
            if(rect==null)return;
            var button=GetComponent<Button>();
            bool active=button==null||button.IsInteractable();
            float scale=active?(pressed?.97f:hovered?1.025f:1f):1f;
            rect.localScale=Vector3.Lerp(rect.localScale,Vector3.one*scale,1-Mathf.Exp(-18*Time.unscaledDeltaTime));
            rect.anchoredPosition=Vector2.Lerp(rect.anchoredPosition,rest+Vector2.up*(hovered&&active?Lift:0),1-Mathf.Exp(-18*Time.unscaledDeltaTime));
        }
        public void OnPointerEnter(PointerEventData e){hovered=true;}
        public void OnPointerExit(PointerEventData e){hovered=false;pressed=false;}
        public void OnPointerDown(PointerEventData e){pressed=true;}
        public void OnPointerUp(PointerEventData e){pressed=false;}
    }
}
