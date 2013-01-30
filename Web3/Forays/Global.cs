/*Copyright (c) 2011-2012  Derrick Creamer
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation
files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Text.RegularExpressions;
using System.Serialization;
using System.Threading.Tasks;
using jQueryApi;
using ROT;
namespace Forays{
	public static class Global{
		public const string VERSION = "version 0.7.0 ";
		public static bool LINUX = false;
		public const int SCREEN_H = 25;
		public const int SCREEN_W = 80;
		public const int ROWS = 22;
		public const int COLS = 66;
		public const int MAP_OFFSET_ROWS = 3;
		public const int MAP_OFFSET_COLS = 13;
		public const int MAX_LIGHT_RADIUS = 12; //the maximum POSSIBLE light radius. used in light calculations.
		public const int MAX_INVENTORY_SIZE = 20;
		public static bool GAME_OVER = false;
		public static bool BOSS_KILLED = false;
		public static bool QUITTING = false;
		public static bool SAVING = false;
		public static string KILLED_BY = "debugged to death";
		public static List<string> quickstartinfo = null;
        public static JsDictionary<OptionType, bool> Options = new JsDictionary<OptionType, bool>();
		public static bool Option(OptionType option){
			bool result = false;
			if(Options.ContainsKey(option))
                result = Options[option];
			return result;
		}
		//public static Random r = new Random();
		public static void SetSeed(int seed){ RNG.SetSeed(seed); }
		public static int Roll(int dice,int sides){
			int total = 0;
			for(int i=0;i<dice;++i){
                total += (int)(1 + Math.Floor(ROT.RNG.GetUniform() * sides)); //Next's maxvalue is exclusive, thus the +1
			}
			return total;
		}
		public static int Roll(int sides){ //note that Roll(0) returns 1. I think I should eventually change that.
			int total = 0;
            total += (int)(1 + Math.Floor(ROT.RNG.GetUniform() * sides)); //Next's maxvalue is exclusive, thus the +1
			return total;
		}
		public static bool OneIn(int num){
			int i = Roll(num);
			if(i == num){
				return true;
			}
			return false;
		}
		public static bool CoinFlip(){
            return (1 > Math.Floor(ROT.RNG.GetUniform() * 2));
		}
		public static int RandomDirection(){
            int result = (1 + (int)Math.Floor(ROT.RNG.GetUniform() * 8));
			if(result == 5){
				result = 9;
			}
			return result;
		}
		public static int RotateDirection(int dir,bool clockwise){ return RotateDirection(dir,clockwise,1); }
		public static int RotateDirection(int dir,bool clockwise,int num){
			for(int i=0;i<num;++i){
				switch(dir){
				case 7:
					dir = clockwise?8:4;
					break;
				case 8:
					dir = clockwise?9:7;
					break;
				case 9:
					dir = clockwise?6:8;
					break;
				case 4:
					dir = clockwise?7:1;
					break;
				case 5:
					break;
				case 6:
					dir = clockwise?3:9;
					break;
				case 1:
					dir = clockwise?4:2;
					break;
				case 2:
					dir = clockwise?1:3;
					break;
				case 3:
					dir = clockwise?2:6;
					break;
				default:
					dir = 0;
					break;
				}
			}
			return dir;
		}
		public static bool BoundsCheck(int r,int c){
			if(r>=0 && r<ROWS && c>=0 && c<COLS){
				return true;
			}
			return false;
		}
		public static async void FlushInput(){
			while(Game.Console.KeyAvailable){
				await Game.Console.ReadKey(true);
			}
		}
        public static async Task<int> EnterInt() { return await EnterInt(4); }
		public static async Task<int> EnterInt(int max_length){
			string s = "";
			ConsoleKeyInfo command;
			Game.Console.CursorVisible = true;
			bool done = false;
			int pos = Game.Console.CursorLeft;
			Screen.WriteString(Game.Console.CursorTop,pos,"".PadRight(max_length));
			while(!done){
				Game.Console.SetCursorPosition(pos,Game.Console.CursorTop);
				command = await Game.Console.ReadKey(true);
				if(command.KeyChar >= '0' && command.KeyChar <= '9'){
					if(s.Length < max_length){
						s = s + (string)command.KeyChar;
						Screen.WriteChar(Game.Console.CursorTop,pos,command.KeyChar);
						++pos;
					}
				}
				else{
					if(command.Key == ConsoleKey.Backspace && s.Length > 0){
						s = s.Substring(0,s.Length-1);
						--pos;
						Screen.WriteChar(Game.Console.CursorTop,pos,' ');
						Game.Console.SetCursorPosition(pos,Game.Console.CursorTop);
					}
					else{
						if(command.Key == ConsoleKey.Escape){
							return 0;
						}
						else{
							if(command.Key == ConsoleKey.Enter){
								if(s.Length == 0){
									return -1;
								}
								done = true;
							}
						}
					}
				}
			}
			return Int32.Parse(s);
		}
        public static async Task<string> EnterString() { return await EnterString(COLS - 1); }
		public static async Task<string> EnterString(int max_length){
			string s = "";
			ConsoleKeyInfo command;
			Game.Console.CursorVisible = true;
			bool done = false;
			int pos = Game.Console.CursorLeft;
			Screen.WriteString(Game.Console.CursorTop,pos,"".PadRight(max_length));
			while(!done){
				Game.Console.SetCursorPosition(pos,Game.Console.CursorTop);
				command = await Game.Console.ReadKey(true);
				if((command.KeyChar >= '!' && command.KeyChar <= '~') || command.KeyChar == ' '){
					if(s.Length < max_length){
                        s = s + (string)command.KeyChar;
						Screen.WriteChar(Game.Console.CursorTop,pos,command.KeyChar);
						++pos;
					}
				}
				else{
					if(command.Key == ConsoleKey.Backspace && s.Length > 0){
						s = s.Substring(0,s.Length-1);
						--pos;
						Screen.WriteChar(Game.Console.CursorTop,pos,' ');
						Game.Console.SetCursorPosition(pos,Game.Console.CursorTop);
					}
					else{
						if(command.Key == ConsoleKey.Escape){
							return "";
						}
						else{
							if(command.Key == ConsoleKey.Enter){
								if(s.Length == 0){
									return "";
								}
								done = true;
							}
						}
					}
				}
			}
			return s;
		}
		public static string[] titlescreen =  new string[]{
"                                                                                ",
"                                                                                ",
"        #######                                                                 ",
"        #######                                                                 ",
"        ##    #                                                                 ",
"        ##                                                                      ",
"        ##  #                                                                   ",
"        #####                                                                   ",
"        #####                                                                   ",
"        ##  #   ###   # ##   ###    #   #   ###                                 ",
"        ##     #   #  ##    #   #   #   #  #                                    ",
"        ##     #   #  #     #   #    # #    ##                                  ",
"        ##     #   #  #     #   #     #       #                                 ",
"        ##      ###   #      ### ##   #    ###                                  ",
"                                     #                                          ",
"                                    #                                           ",
"                                                                                ",
"                                                                                ",
"                         I N T O     N O R R E N D R I N                        ",
"                                                                                ",
"                                                                                ",
"                                                                                ",
"                                                                  " + VERSION,
"                                                             by Derrick Creamer "};
		public static string RomanNumeral(int num){
			string result = "";
			while(num > 1000){
				result = result + "M";
				num -= 1000;
			}
			result = result + RomanPattern(num/100,'C','D','M');
			num -= (num/100)*100;
			result = result + RomanPattern(num/10,'X','L','C');
			num -= (num/10)*10;
			result = result + RomanPattern(num,'I','V','X');
			return result;
		}
		private static string RomanPattern(int num,char oneI,char fiveV,char tenX){
            string one = (string)oneI;
            string five = (string)fiveV;
            string ten = (string)tenX;
			switch(num){
			case 1:
				return "" + one;
			case 2:
				return "" + one + one;
			case 3:
				return "" + one + one + one;
			case 4:
				return "" + one + five;
			case 5:
				return "" + five;
			case 6:
				return "" + five + one;
			case 7:
				return "" + five + one + one;
			case 8:
				return "" + five + one + one + one;
			case 9:
				return "" + one + ten;
			default: //0
				return "";
			}
		}
		public static void LoadOptions(){
            
			//StreamReader file = new StreamReader("options.txt");
			/*string s = "";
			while(s.Length < 2 || s.Substring(0,2) != "--"){
				s = file.ReadLine();
				if(s.Length >= 2 && s.Substring(0,2) == "--"){
					break;
				}
				string[] tokens = s.Split(' ');
				if(tokens[0].Length == 1){
					char c = (tokens[0][0]).ToString().ToUpperCase()[0];
					if(c == 'F' || c == 'T'){
						OptionType option = (OptionType)Enum.Parse(typeof(OptionType),tokens[1]);
						if(c == 'F'){
							Options[option] = false;
						}
						else{
							Options[option] = true;
						}
					}
				}
			}
			s = "";
			while(s.Length < 2 || s.Substring(0,2) != "--"){
				s = file.ReadLine();
				if(s.Length >= 2 && s.Substring(0,2) == "--"){
					break;
				}
				string[] tokens = s.Split(' ');
				if(tokens[0].Length == 1){
                    char c = (tokens[0][0]).ToString().ToUpperCase()[0];
					if(c == 'F' || c == 'T'){
						TutorialTopic topic = (TutorialTopic)Enum.Parse(typeof(TutorialTopic),tokens[1]);
						if(c == 'F' || Global.Option(OptionType.ALWAYS_RESET_TIPS)){
							Help.displayed[topic] = false;
						}
						else{
							Help.displayed[topic] = true;
						}
					}
				}
			}*/
		}
		public static void SaveOptions(){
            /*JsDictionary<string, string> sav =  new JsDictionary<string, string>() { };
			foreach(OptionType op in OptionType.ALWAYS_RESET_TIPS.GetValues()){
				if(Options[op]){
                    sav[op.ToString()] = "t" + op.ToString().ToLower();//file.Write("t ");
				}
				else{
                    sav[op.ToString()] = "f" + op.ToString().ToLower();
				}
				//file.WriteLine(Enum.GetName(typeof(OptionType),op).ToLower());
			}
			//file.WriteLine("-- Tracking which tutorial tips have been displayed:");
			foreach(TutorialTopic topic in TutorialTopic.Armor.GetValues()){
				if(Help.displayed[topic]){
                    sav[topic.ToString()] = "t" + topic.ToString().ToLower();
				}
				else{
                    sav[topic.ToString()] = "f" + topic.ToString().ToLower();
				}
				//file.WriteLine(Enum.GetName(typeof(TutorialTopic),topic).ToLower());
			}
//			file.WriteLine("--");
//			file.Close();
             */
		}
		public delegate int IDMethod(PhysicalObject o);
		public static void SaveGame(Buffer B,Map M,Queue Q){ //games are loaded in Main.cs
			/*
            FileStream file = new FileStream("forays.sav",FileMode.CreateNew);
			BinaryWriter b = new BinaryWriter(file);
            JsDictionary<PhysicalObject, int> id = new JsDictionary<PhysicalObject, int>();
			int next_id = 1;
			IDMethod GetID = delegate(PhysicalObject o){
				if(o == null){
					return 0;
				}
				if(!id.ContainsKey(o)){
					id[o] = next_id;
					++next_id;
				}
				return id[o];
			};
			b.Write(Actor.player_name);
			b.Write(M.current_level);
			for(int i=0;i<20;++i){
				b.Write((int)M.level_types[i]);
			}
			b.Write(M.wiz_lite);
			b.Write(M.wiz_dark);
			//skipping danger_sensed
			b.Write(Actor.feats_in_order.Count);
			foreach(FeatType ft in Actor.feats_in_order){
				b.Write((int)ft);
			}
			b.Write(Actor.partial_feats_in_order.Count);
			foreach(FeatType ft in Actor.partial_feats_in_order){
				b.Write((int)ft);
			}
			b.Write(Actor.spells_in_order.Count);
			foreach(SpellType sp in Actor.spells_in_order){
				b.Write((int)sp);
			}
			List<List<Actor>> groups = new List<List<Actor>>();
			b.Write(M.AllActors().Count);
			foreach(Actor a in M.AllActors()){
				b.Write(GetID(a));
				b.Write(a.row);
				b.Write(a.col);
				b.Write(a.name);
				b.Write(a.the_name);
				b.Write(a.a_name);
				b.Write(a.symbol);
				b.Write((int)a.color);
				b.Write((int)a.type);
				b.Write(a.maxhp);
				b.Write(a.curhp);
				b.Write(a.speed);
				b.Write(a.level);
				b.Write(a.light_radius);
				b.Write(GetID(a.target));
				b.Write(a.inv.Count);
				foreach(Item i in a.inv){
					b.Write(i.name);
					b.Write(i.the_name);
					b.Write(i.a_name);
					b.Write(i.symbol);
					b.Write((int)i.color);
					b.Write((int)i.type);
					b.Write(i.quantity);
					b.Write(i.ignored);
				}
				for(int i=0;i<13;++i){
					b.Write((int)a.F[i]);
				}
				b.Write(a.attrs.d.Count);
				foreach(AttrType at in a.attrs.d.Keys){
					b.Write((int)at);
					b.Write(a.attrs[at]);
				}
				b.Write(a.skills.d.Count);
				foreach(SkillType st in a.skills.d.Keys){
					b.Write((int)st);
					b.Write(a.skills[st]);
				}
				b.Write(a.feats.d.Count);
				foreach(FeatType ft in a.feats.d.Keys){
					b.Write((int)ft);
					b.Write(a.feats[ft]);
				}
				b.Write(a.spells.d.Count);
				foreach(SpellType sp in a.spells.d.Keys){
					b.Write((int)sp);
					b.Write(a.spells[sp]);
				}
				b.Write(a.magic_penalty);
				b.Write(a.time_of_last_action);
				b.Write(a.recover_time);
				b.Write(a.path.Count);
				foreach(pos p in a.path){
					b.Write(p.row);
					b.Write(p.col);
				}
				b.Write(GetID(a.target_location));
				b.Write(a.player_visibility_duration);
				if(a.group != null){
					groups.AddUnique(a.group);
				}
				b.Write(a.weapons.Count);
				foreach(WeaponType w in a.weapons){
					b.Write((int)w);
				}
				b.Write(a.armors.Count);
				foreach(ArmorType ar in a.armors){
					b.Write((int)ar);
				}
				b.Write(a.magic_items.Count);
				foreach(MagicItemType m in a.magic_items){
					b.Write((int)m);
				}
			}
			b.Write(groups.Count);
			foreach(List<Actor> group in groups){
				b.Write(group.Count);
				foreach(Actor a in group){
					b.Write(GetID(a));
				}
			}
			b.Write(M.AllTiles().Count);
			foreach(Tile t in M.AllTiles()){
				b.Write(GetID(t));
				b.Write(t.row);
				b.Write(t.col);
				b.Write(t.name);
				b.Write(t.the_name);
				b.Write(t.a_name);
				b.Write(t.symbol);
				b.Write((int)t.color);
				b.Write((int)t.type);
				b.Write(t.passable);
				b.Write(t.opaque);
				b.Write(t.seen);
				b.Write(t.solid_rock);
				b.Write(t.light_value);
				if(t.toggles_into.HasValue){
					b.Write(true);
					b.Write((int)t.toggles_into.Value);
				}
				else{
					b.Write(false);
				}
				if(t.inv != null){
					b.Write(true);
					b.Write(t.inv.name);
					b.Write(t.inv.the_name);
					b.Write(t.inv.a_name);
					b.Write(t.inv.symbol);
					b.Write((int)t.inv.color);
					b.Write((int)t.inv.type);
					b.Write(t.inv.quantity);
					b.Write(t.inv.ignored);
				}
				else{
					b.Write(false);
				}
				b.Write(t.features.Count);
				foreach(FeatureType f in t.features){
					b.Write((int)f);
				}
			}
			b.Write(Q.turn);
			b.Write(Actor.tiebreakers.Count);
			foreach(Actor a in Actor.tiebreakers){
				b.Write(GetID(a));
			}
			b.Write(Q.list.Count);
			foreach(Event e in Q.list){
				b.Write(GetID(e.target));
				if(e.area == null){
					b.Write(0);
				}
				else{
					b.Write(e.area.Count);
					foreach(Tile t in e.area){
						b.Write(GetID(t));
					}
				}
				b.Write(e.delay);
				b.Write((int)e.type);
				b.Write((int)e.attr);
				b.Write(e.value);
				b.Write(e.msg);
				if(e.msg_objs == null){
					b.Write(0);
				}
				else{
					b.Write(e.msg_objs.Count);
					foreach(PhysicalObject o in e.msg_objs){
						b.Write(GetID(o));
					}
				}
				b.Write(e.time_created);
				b.Write(e.dead);
				b.Write(e.tiebreaker);
			}
			for(int i=0;i<20;++i){
				b.Write(B.Printed(i));
			}
			b.Close();
			file.Close();
         */    
		}
		public static void Quit(){
			
				Screen.Blank();
				Screen.ResetColors();
				Game.Console.SetCursorPosition(0,0);
				Game.Console.CursorVisible = true;
			
		}
	}
	public class Dict<TKey,TValue>{
		public JsDictionary<TKey,TValue> d;// = new Dictionary<TKey,TValue>();
		public TValue this[TKey key]{
			get{
				return d.ContainsKey(key)? d[key] : default(TValue);
			}
			set{
				d[key] = value;
			}
		}
		public Dict(){ d = new JsDictionary<TKey,TValue>(); }
		public Dict(Dict<TKey,TValue> d2){ d = new JsDictionary<TKey, TValue>(d2.d); }
	}
	public class pos{
		public int row;
		public int col;
		public pos(int r,int c){
			row = r;
			col = c;
		}
		public int DistanceFrom(PhysicalObject o){ return DistanceFrom(o.row,o.col); }
		public int DistanceFrom(pos p){ return DistanceFrom(p.row,p.col); }
		//public int DistanceFrom(ICoord o){ return DistanceFrom(o.row,o.col); }
		public int DistanceFrom(int r,int c){
			int dy = Math.Abs(r-row);
			int dx = Math.Abs(c-col);
			if(dx > dy){
				return dx;
			}
			else{
				return dy;
			}
		}
		public int EstimatedEuclideanDistanceFromX10(PhysicalObject o){ return EstimatedEuclideanDistanceFromX10(o.row,o.col); }
		public int EstimatedEuclideanDistanceFromX10(pos p){ return EstimatedEuclideanDistanceFromX10(p.row,p.col); }
		public int EstimatedEuclideanDistanceFromX10(int r,int c){ // x10 so that orthogonal directions are closer than diagonals
			int dy = Math.Abs(r-row) * 10;
			int dx = Math.Abs(c-col) * 10;
			if(dx > dy){
				return dx + (dy/2);
			}
			else{
				return dy + (dx/2);
			}
		}
		public List<pos> PositionsWithinDistance(int dist){ return PositionsWithinDistance(dist,false); }
		public List<pos> PositionsWithinDistance(int dist,bool exclude_origin){
			List<pos> result = new List<pos>();
			for(int i=row-dist;i<=row+dist;++i){
				for(int j=col-dist;j<=col+dist;++j){
					if(i!=row || j!=col || exclude_origin==false){
						if(Global.BoundsCheck(i,j)){
							result.Add(new pos(i,j));
						}
					}
				}
			}
			return result;
		}
		public List<pos> PositionsAtDistance(int dist){
			List<pos> result = new List<pos>();
			for(int i=row-dist;i<=row+dist;++i){
				for(int j=col-dist;j<=col+dist;++j){
					if(DistanceFrom(i,j) == dist && Global.BoundsCheck(i,j)){
						result.Add(new pos(i,j));
					}
				}
			}
			return result;
		}
		public pos PositionInDirection(int dir){
			switch(dir){
			case 1:
				return new pos(row+1,col-1);
			case 2:
				return new pos(row+1,col);
			case 3:
				return new pos(row+1,col+1);
			case 4:
				return new pos(row,col-1);
			case 5:
				return new pos(row,col);
			case 6:
				return new pos(row,col+1);
			case 7:
				return new pos(row-1,col-1);
			case 8:
				return new pos(row-1,col);
			case 9:
				return new pos(row-1,col+1);
			default:
				return new pos(-2,-2);
			}
		}
		public bool BoundsCheck(){
			if(row>=0 && row<Global.ROWS && col>=0 && col<Global.COLS){
				return true;
			}
			return false;
		}
	}
    public static class Extensions
    {

        /*public static int ToInt(this Enum e)
        {
            Enum[] av = e.GetValues();
            for (int i = 0; i < av.Length; i++)
            {
                if (av[i] == e)
                    return i;
            }
            return -1;
        }*/
        public static int[] GetValues(this Type e)
        {
            if (Enum.Keys(e.GetType()).Length == 0)
            {
                return new int[]{};
            }
            int[] ret = new int[]{};
            for (int i = 0; i <  Type.Keys(e).Length; i++ )
            {
                ret[i] = i;
            }
            return ret;
        }
        public static Wrapper<T> Find<T>(this List<T> l, T toFind)
        {
            if (l.Count == 0 || !l.Contains(toFind))
            {
                return new Wrapper<T>();
            }
            return new Wrapper<T>(l[l.IndexOf(toFind)]);
        }
        public static T Random<T>(this List<T> l)
        {
            if (l.Count == 0)
            {
                return default(T);
            }
            return l[Global.Roll(l.Count) - 1];
        }
		public static T RemoveRandom<T>(this List<T> l){
			if(l.Count == 0){
				return default(T);
			}
			T result = l[Global.Roll(l.Count)-1];
			l.Remove(result);
			return result;
		}
		public static void AddUnique<T>(this List<T> l,T obj){
			if(!l.Contains(obj)){
				l.Add(obj);
			}
		}
        public static T Last<T>(this List<T> l)
        { //note that this doesn't work the way I wanted it to - 
            if (l.Count == 0)
            { // you can't assign to list.Last()
                return default(T);
            }
            return l[l.Count - 1];
        }
        public static void Randomize<T>(this List<T> l)
        {
            List<T> temp = new List<T>(l);
            l.Clear();
            while (temp.Count > 0)
            {
                l.Add(temp.RemoveRandom());
            }
        }
        public static List<T> GetRange<T>(this List<T> l, int min, int upper)
        {
            List<T> temp = new List<T>();
            
            for (int i = min; i < upper && i < l.Count; i++)
            {
                temp.Add(l[i]);
            }
            return temp;
        }
		public static int RotateDirection(this int dir,bool clockwise){ return dir.RotateDirection(clockwise,1); }
		public static int RotateDirection(this int dir,bool clockwise,int num){
			for(int i=0;i<num;++i){
				switch(dir){
				case 7:
					dir = clockwise?8:4;
					break;
				case 8:
					dir = clockwise?9:7;
					break;
				case 9:
					dir = clockwise?6:8;
					break;
				case 4:
					dir = clockwise?7:1;
					break;
				case 5:
					break;
				case 6:
					dir = clockwise?3:9;
					break;
				case 1:
					dir = clockwise?4:2;
					break;
				case 2:
					dir = clockwise?1:3;
					break;
				case 3:
					dir = clockwise?2:6;
					break;
				default:
					dir = 0;
					break;
				}
			}
			return dir;
		}
		public static List<string> GetWordWrappedList(this string s,int max_length){
			List<string> result = new List<string>();
			while(s.Length > max_length){
				for(int i=max_length;i>=0;--i){
					if(s.Substring(i,1) == " "){
						result.Add(s.Substring(0,i));
						s = s.Substring(i+1);
						break;
					}
				}
			}
			result.Add(s);
			return result;
		}
		public static string PadOuter(this string s,int totalWidth){
			return s.PadOuter(totalWidth,' ');
		}
		public static string PadOuter(this string s,int totalWidth,char paddingChar){
			if(s.Length >= totalWidth){
				return s;
			}
			int added = totalWidth - s.Length;
			string left = "";
			for(int i=0;i<(added+1)/2;++i){
				left = left + (string)paddingChar;
			}
			string right = "";
			for(int i=0;i<added/2;++i){
                right = right + (string)paddingChar;
			}
			return left + s + right;
		}
		public static string PadToMapSize(this string s){
			return s.PadRight(Global.COLS);
		}
		public static colorstring GetColorString(this string s){ return GetColorString(s,Color.Gray); }
		public static colorstring GetColorString(this string s,Color color){
			if(s.Search(new Regex("\\[")) > -1){
				string temp = s;
				colorstring result = new colorstring();
				while(temp.Search(new Regex("\\[")) > -1){
					int open = temp.IndexOf('[');
					int close = temp.IndexOf(']');
					if(close == -1){
						result.strings.Add(new cstr(temp,color));
						temp = "";
					}
					else{
						int hyphen = temp.IndexOf('-');
						if(hyphen != -1 && hyphen > open && hyphen < close){
							result.strings.Add(new cstr(temp.Substring(0,open+1),color));
							//result.strings.Add(new cstr(temp.Substring(open+1,(close-open)-1),Color.Cyan));
							result.strings.Add(new cstr(temp.Substring(open+1,(hyphen-open)-1),Color.Cyan));
							result.strings.Add(new cstr("-",color));
							result.strings.Add(new cstr(temp.Substring(hyphen+1,(close-hyphen)-1),Color.Cyan));
							result.strings.Add(new cstr("]",color));
							temp = temp.Substring(close+1);
						}
						else{
							result.strings.Add(new cstr(temp.Substring(0,open+1),color));
							result.strings.Add(new cstr(temp.Substring(open+1,(close-open)-1),Color.Cyan));
							result.strings.Add(new cstr("]",color));
							temp = temp.Substring(close+1);
						}
					}
				}
				if(temp != ""){
					result.strings.Add(new cstr(temp,color));
				}
				return result;
			}
			else{
				return new colorstring(s,color);
			}
		}
		public delegate void ListDelegate<T>(T t); //this one is kinda experimental and doesn't save tooo much typing, but it's here anyway
		public static void Each<T>(this List<T> l,ListDelegate<T> del){
			foreach(T t in l){
				del(t);
			}
		}
		public delegate bool BooleanDelegate<T>(T t);
		public static List<T> Where<T>(this List<T> l,BooleanDelegate<T> condition){ //now THIS one is useful. probably the same as the official version.
			List<T> result = new List<T>();
			foreach(T t in l){
				if(condition(t)){
					result.Add(t);
				}
			}
			return result;
		}
		public static bool Any<T>(this List<T> l,BooleanDelegate<T> condition){
			foreach(T t in l){
				if(condition(t)){
					return true;
				}
			}
			return false;
		}
		public delegate int IntegerDelegate<T>(T t);
		public static List<T> WhereGreatest<T>(this List<T> l,IntegerDelegate<T> value){
			List<T> result = new List<T>();
			int highest = 0;
			bool first = true;
			foreach(T t in l){
				int i = value(t);
				if(first){
					first = false;
					highest = i;
					result.Add(t);
				}
				else{
					if(i > highest){
						highest = i;
						result.Clear();
						result.Add(t);
					}
					else{
						if(i == highest){
							result.Add(t);
						}
					}
				}
			}
			return result;
		}
		public static List<T> WhereLeast<T>(this List<T> l,IntegerDelegate<T> value){
			List<T> result = new List<T>();
			int lowest = 0;
			bool first = true;
			foreach(T t in l){
				int i = value(t);
				if(first){
					first = false;
					lowest = i;
					result.Add(t);
				}
				else{
					if(i < lowest){
						lowest = i;
						result.Clear();
						result.Add(t);
					}
					else{
						if(i == lowest){
							result.Add(t);
						}
					}
				}
			}
			return result;
		}
		public static List<Tile> ToFirstSolidTile(this List<Tile> line){
			List<Tile> result = new List<Tile>();
			foreach(Tile t in line){
				result.Add(t);
				if(!t.passable){
					break;
				}
			}
			return result;
		}
		public static List<Tile> ToFirstObstruction(this List<Tile> line){ //impassible tile OR actor
			List<Tile> result = new List<Tile>();
			int idx = 0;
			foreach(Tile t in line){
				result.Add(t);
				if(idx != 0){ //skip the first, as it is assumed to be the origin
					if(!t.passable || t.actor() != null){
						break;
					}
				}
				++idx;
			}
			return result;
		}
		public static List<Tile> To(this List<Tile> line,PhysicalObject o){
			List<Tile> result = new List<Tile>();
			foreach(Tile t in line){
				result.Add(t);
				if(o.row == t.row && o.col == t.col){
					break;
				}
			}
			return result;
		}
		public static Tile LastBeforeSolidTile(this List<Tile> line){
			Tile result = null;
			foreach(Tile t in line){
				if(!t.passable){
					break;
				}
				else{
					result = t;
				}
			}
			return result;
		}
		/*public static List<ICoord> ToICoord(this List<Tile> l){
			List<ICoord> result = new List<ICoord>();
			foreach(Tile t in l){
				result.Add(t);
			}
			return result;
		}*/
	}

    public class Wrapper<T>
    {
        public T Value;
        public Wrapper()
        {
            Value = default(T);
        }
        public Wrapper(T v)
        {
            Value = v;
        }
    }
}
