using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Resources;
using System.Windows.Shapes;
using Bara_Tehni.CSharpReference;
using Bara_Tehni.Methods;
using CSharpReference;

namespace Bara_Tehni
{
    /// <summary>
    /// Interaction logic for Board.xaml
    /// </summary>
    public partial class Board : UserControl
    {
        enum PLAYERS
        {
            ONE,TWO
        }
        static Random _r;
        private bool _isKilled = false;
        private bool _isMoved = false;
   
        private int _onePieces = 8;
        private int _twoPieces = 8;
        private string _turn = "ONE";
        private readonly AnimationsEffects _animationsEffects;
        private readonly Intelligence _intelligence;
        private ColorFullEllipse _selectedColorFullEllipse;
        private List<GridValues> _arrayList = new List<GridValues>();
        public Board()
        {       
            InitializeComponent();
            _animationsEffects = new AnimationsEffects(); 
            _intelligence = new Intelligence();
            Initialize();
        }   

        void Initialize()
        {
            SetPieces(PLAYERS.ONE);
            SetPieces(PLAYERS.TWO);
            SetEmptySpaces();

            ShowData("Player Turn");
            ShowData("> Player 1");
            ShowData("  Player 2");
            ShowData("");
            ShowData("Player 1: Pieces Left=> " + _onePieces);
            ShowData("Player 2: Pieces Left=> " + _twoPieces);

            UpdateList();
            
            ((TextBlock) this.DataListView.Items[1]).Foreground = new SolidColorBrush(Colors.Red);

            _r = new Random();
           // SetEllipse(new Point(0,2),new Point(0,4));
           // GenerateMoves(new Point(0, 2));
        }

        private void UpdateList()
        {
            GridValues val1 = new GridValues(),val2 = new GridValues();
            if (_turn == "ONE")
            {
                val1 = GridValues.Attacker;
                val2 = GridValues.Prey;
            }
            if (_turn == "TWO")
            {
                val1 = GridValues.Prey;
                val2 = GridValues.Attacker;
            }
            var obj = GridBoard.Children.Cast<UIElement>().OfType<ColorFullEllipse>();
            foreach (ColorFullEllipse child in obj)
            {
                switch (child.PlayerPiece)
                {
                    case "EMPTY":
                        _arrayList.Add(GridValues.Empty);
                        break;
                    case "ONE":
                        _arrayList.Add(val1);
                        break;
                    case "TWO":
                        _arrayList.Add(val2);
                        break;
                }
            }
            _intelligence.Update(_arrayList);
        }

        private void SetEmptySpaces()
        {
            MakeEmptyEllipse(0,8);
            MakeEmptyEllipse(2,8);
            MakeEmptyEllipse(8,0);
            MakeEmptyEllipse(6,0);
           
            for (var i = 0; i < 10; i=i+2)
            {
               MakeEmptyEllipse(4,i);
            }
        }

        void MakeEmptyEllipse(int row,int col)
        {
            var ellipse = new ColorFullEllipse();
            ellipse.RenderTransformOrigin = new Point(0.5, 0.5);
            ellipse.PreviewMouseLeftButtonDown += EmptyEllipseClicked;
            GridBoard.Children.Add(ellipse);
            Grid.SetColumn(ellipse,col);
            Grid.SetRow(ellipse,row);
        }

        private void EmptyEllipseClicked(object sender, MouseButtonEventArgs e)
        {
            if (_selectedColorFullEllipse == null) return;
            var element = (UIElement)e.Source;
            var ellipse = sender as ColorFullEllipse;

            if (ellipse == null) return;

            _targetcolumnGrid = Grid.GetColumn(element);
            _targetrowGrid = Grid.GetRow(element);
            if (!_isMoved )
            {              
                int diffCol = (_targetcolumnGrid/2 - _sourcecolumnGrid/2);
                int diffRow = (_targetrowGrid/2 - _sourcerowGrid/2);

                if (diffCol < 0) diffCol *= -1;
                if (diffRow < 0) diffRow *= -1;

                bool pieceInBetween = diffCol == 2 || diffRow == 2 ? true : false;

                if (!IsValidMove(pieceInBetween,false)) return;

                _isMoved = true;
                                     
            }
            else if(_isKilled && _isMoved)
            {               
                if(!IsValidMove()) return;
            }

            int tCol = Grid.GetColumn(ellipse);
            int tRow = Grid.GetRow(ellipse);

            int sCol = Grid.GetColumn(_selectedColorFullEllipse);
            int sRow = Grid.GetRow(_selectedColorFullEllipse);

            Grid.SetColumn(_selectedColorFullEllipse, tCol);
            Grid.SetRow(_selectedColorFullEllipse, tRow);

            Grid.SetColumn(ellipse, sCol);
            Grid.SetRow(ellipse, sRow);

            if (_isMoved && !_isKilled)
            {
                confirmLabel_PreviewMouseLeftButtonDown(null, null);
            }
            else
            {
                _sourcecolumnGrid = Grid.GetColumn(_selectedColorFullEllipse);
                _sourcerowGrid = Grid.GetRow(_selectedColorFullEllipse);
            }
        }

        string OponentTurn()
        {
            return (_turn == "ONE" ? "TWO" : "ONE");
        }

        void ShowData(string str)
        {       
            var menuItem = new TextBlock
            {
                Text = str,
                FontSize = 28,
                Height = 55,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontFamily = new FontFamily("Buxton Sketch"),
                Foreground = new SolidColorBrush(Colors.Black)
            };
            GeneralMethods.onMouseEnter_Scale(menuItem);
            GeneralMethods.onMouseLeave_Scale(menuItem);

            this.DataListView.Items.Add(menuItem);           
        }

        bool IsValidMove(bool pieceInBetween,bool isKill)
        {
            int jump = 2;
            bool isValid = false;
            ColorFullEllipse tempEllipse = null;
            int val1 = (_targetrowGrid + _sourcerowGrid)/2;
            if (val1%2 == 1) val1++;
            int val2 = (_targetcolumnGrid + _sourcecolumnGrid)/2;
            if (val2 % 2 == 1) val2++;

            if (pieceInBetween)
            {           
                //Get ROW and GET COLUMN SHOULD MATCH WITH VAL1 AND VAL2
                Func<ColorFullEllipse, bool> pred1 = i => Grid.GetRow(i) == val1;
                Func<ColorFullEllipse, bool> pred2 = j => Grid.GetColumn(j) == val2;
                Func<ColorFullEllipse, bool> pred3 = k => k.PlayerPiece == OponentTurn();
                Func<ColorFullEllipse, bool> combinedOr = s => ((pred1(s) && pred2(s)) && (pred3(s)));

                var itemsInFirstRow = GridBoard.Children
                    .Cast<UIElement>()
                    .OfType<ColorFullEllipse>()
                    .Where(combinedOr);
                var colorFullEllipses = itemsInFirstRow as IList<ColorFullEllipse> ?? itemsInFirstRow.ToList();
                if (colorFullEllipses.Any())
                {
                    jump = 4;
                    tempEllipse = colorFullEllipses.First();                  
                }
            }

            int elemTarget = (_targetrowGrid/2)*5 + _targetcolumnGrid/2;
            int elemSource = (_sourcerowGrid/2)*5 + _sourcecolumnGrid/2;

            bool isLimited = (elemSource + 1)%2 == 0;
            //limited

            //right
            if ((_sourcecolumnGrid) + jump == (_targetcolumnGrid) && (_sourcerowGrid) == (_targetrowGrid))
                isValid = true;
            //left
            else if ((_sourcecolumnGrid ) - jump == _targetcolumnGrid  && _sourcerowGrid  == (_targetrowGrid ))
                isValid = true;
            //bottom
            else if ((_sourcecolumnGrid) == (_targetcolumnGrid) && ((_sourcerowGrid) + jump) == (_targetrowGrid))        
                isValid = true;      
            //top
            else if ((_sourcecolumnGrid) == (_targetcolumnGrid ) && ((_sourcerowGrid ) - jump) == (_targetrowGrid ))
                isValid = true;
            //top-right
            else if (!isLimited && (_sourcecolumnGrid) + jump == (_targetcolumnGrid) && (_sourcerowGrid) - jump == (_targetrowGrid))
                isValid = true;
            //bottom-right
            else if (!isLimited && (_sourcecolumnGrid) + jump == (_targetcolumnGrid) && (_sourcerowGrid) + jump == (_targetrowGrid))
                isValid = true;
            //top-left
            else if (!isLimited && _sourcecolumnGrid - jump == (_targetcolumnGrid) && (_sourcerowGrid) - jump == (_targetrowGrid))
                isValid = true;
            //bottom-left
            else if (!isLimited && (_sourcecolumnGrid) - jump == (_targetcolumnGrid) && (_sourcerowGrid) + jump == (_targetrowGrid))
                isValid = true;
           
            if (isValid && jump ==4 )
            {
                if(isKill)
                {
                    return isValid; 
                }
                KillPiece(tempEllipse);
            }

            return isValid;
        }

        bool IsValidMove()
        {
            int jump = 0;
            bool isValid = false;
            ColorFullEllipse tempEllipse = null;
            int val1 = (_targetrowGrid + _sourcerowGrid) / 2;
            if (val1 % 2 == 1) val1++;
            int val2 = (_targetcolumnGrid + _sourcecolumnGrid) / 2;
            if (val2 % 2 == 1) val2++;

            //Get ROW and GET COLUMN SHOULD MATCH WITH VAL1 AND VAL2
            Func<ColorFullEllipse, bool> pred1 = i => Grid.GetRow(i) == val1;
            Func<ColorFullEllipse, bool> pred2 = j => Grid.GetColumn(j) == val2;
            Func<ColorFullEllipse, bool> pred3 = k => k.PlayerPiece == OponentTurn();
            Func<ColorFullEllipse, bool> combinedOr = s => ((pred1(s) && pred2(s)) && (pred3(s)));

            var itemsInFirstRow = GridBoard.Children
                .Cast<UIElement>()
                .OfType<ColorFullEllipse>()
                .Where(combinedOr);
            var colorFullEllipses = itemsInFirstRow as IList<ColorFullEllipse> ?? itemsInFirstRow.ToList();
            if (colorFullEllipses.Any())
            {
                jump = 4;
                tempEllipse = colorFullEllipses.First();
            }           
            
            int elemTarget = (_targetrowGrid / 2) * 5 + _targetcolumnGrid / 2;
            int elemSource = (_sourcerowGrid / 2) * 5 + _sourcecolumnGrid / 2;

            bool isLimited = (elemSource + 1) % 2 == 0;
            //limited

            //right
            if ((_sourcecolumnGrid) + jump == (_targetcolumnGrid) && (_sourcerowGrid) == (_targetrowGrid))
                isValid = true;
            //left
            else if ((_sourcecolumnGrid) - jump == _targetcolumnGrid && _sourcerowGrid == (_targetrowGrid))
                isValid = true;
            //bottom
            else if ((_sourcecolumnGrid) == (_targetcolumnGrid) && ((_sourcerowGrid) + jump) == (_targetrowGrid))
                isValid = true;
            //top
            else if ((_sourcecolumnGrid) == (_targetcolumnGrid) && ((_sourcerowGrid) - jump) == (_targetrowGrid))
                isValid = true;
            //top-right
            else if (!isLimited && (_sourcecolumnGrid) + jump == (_targetcolumnGrid) && (_sourcerowGrid) - jump == (_targetrowGrid))
                isValid = true;
            //bottom-right
            else if (!isLimited && (_sourcecolumnGrid) + jump == (_targetcolumnGrid) && (_sourcerowGrid) + jump == (_targetrowGrid))
                isValid = true;
            //top-left
            else if (!isLimited && _sourcecolumnGrid - jump == (_targetcolumnGrid) && (_sourcerowGrid) - jump == (_targetrowGrid))
                isValid = true;
            //bottom-left
            else if (!isLimited && (_sourcecolumnGrid) - jump == (_targetcolumnGrid) && (_sourcerowGrid) + jump == (_targetrowGrid))
                isValid = true;

            if (isValid && jump == 4)
            {
                KillPiece(tempEllipse);
            }

            return isValid;
        }

        void KillPiece(ColorFullEllipse tempEllipse)
        {
            if (tempEllipse != null)
            {
                int row = Grid.GetRow(tempEllipse);
                int col = Grid.GetColumn(tempEllipse);

                var obj = new ColorFullEllipse();
                obj.PreviewMouseLeftButtonDown += EmptyEllipseClicked;

                GridBoard.Children.Remove(tempEllipse);
                GridBoard.Children.Add(obj);
                Grid.SetColumn(obj, col);
                Grid.SetRow(obj, row);

                _isKilled = true;                
           
                if (_turn == "ONE")
                {
                    _onePieces--;
                    if (_onePieces == 0)
                    {
                        MessageBox.Show("Player 2 Wins");
                    }
                    else
                        ((TextBlock)this.DataListView.Items[4]).Text = "Player 1: Pieces Left=> " + _onePieces.ToString();
                }
                else
                {
                    _twoPieces--;
                    if (_twoPieces == 0)
                    {
                        MessageBox.Show("Player 1 Wins");
                    }
                    else
                        ((TextBlock)this.DataListView.Items[5]).Text = "Player 2: Pieces Left=> " +                                                          _twoPieces.ToString();
                }
            }
        }

        void SetLabels()
        {     
            ((TextBlock)this.DataListView.Items[1]).Foreground = new SolidColorBrush(Colors.Ivory);
            ((TextBlock)this.DataListView.Items[2]).Foreground = new SolidColorBrush(Colors.Ivory);

            if (_turn == "ONE")
            {
                ((TextBlock)this.DataListView.Items[1]).Foreground = new SolidColorBrush(Colors.Red);
                ((TextBlock)this.DataListView.Items[1]).Text = "> Player 1";
                ((TextBlock)this.DataListView.Items[2]).Text = "  Player 2";
            }
            else
            {
                ((TextBlock)this.DataListView.Items[2]).Foreground = new SolidColorBrush(Colors.Red);
                ((TextBlock)this.DataListView.Items[1]).Text = "  Player 1";
                ((TextBlock)this.DataListView.Items[2]).Text = "> Player 2";
            }
        }

        void SetSelected()
        {      
            _selectedColorFullEllipse.SetStroke(null, 0);
            _selectedColorFullEllipse = null;
        }

        void SetPieces(PLAYERS valPlayer)
        {
            switch (valPlayer)
            {
                case PLAYERS.ONE:
                {
                    var resourceUri = new Uri("/Images/P1.png", UriKind.Relative);
                    var streamInfo = Application.GetResourceStream(resourceUri);

                    var temp = BitmapFrame.Create(streamInfo.Stream);

                    for (var i = 0; i < 4; i=i+2)
                    {
                        for (var j = 0; j < 8; j=j+2)
                        {
                            var brush = new ImageBrush {ImageSource = temp};
                            PlacePiece(i, j, brush,"ONE");
                        }
                    }
                }
                    break;
                case PLAYERS.TWO:
                {
                    var resourceUri = new Uri("/Images/P2.png", UriKind.Relative);
                    var streamInfo = Application.GetResourceStream(resourceUri);

                    var temp = BitmapFrame.Create(streamInfo.Stream); 

                    for (var i = 6; i < 10; i=i+2)
                    {
                        for (var j = 2; j < 10; j=j+2)
                        {
                            var brush = new ImageBrush { ImageSource = temp };
                            PlacePiece(i, j, brush,"TWO");
                        }
                    }
                }
                    break;
            }
        }

        void PlacePiece(int row,int col,Brush brush,string playerPiece)
        {
            var effectColorFullEllipse = new ColorFullEllipse {PlayerPiece = playerPiece};
            effectColorFullEllipse.SetBrush(brush);
            var marginThickness = new Thickness(-12, -12, -12, -12);
            effectColorFullEllipse.Margin = marginThickness;
            effectColorFullEllipse.MouseEnter += effectColorFullEllipse_MouseEnter;
            effectColorFullEllipse.MouseLeave += effectColorFullEllipse_MouseLeave;
            effectColorFullEllipse.PreviewMouseLeftButtonDown += effectColorFullEllipse_PreviewMouseLeftButtonDown;
            effectColorFullEllipse.RenderTransformOrigin = new Point(0.5, 0.5);
            GridBoard.Children.Add(effectColorFullEllipse);
            Grid.SetColumn(effectColorFullEllipse,col);
            Grid.SetRow(effectColorFullEllipse,row);
        }

        private int _sourcecolumnGrid = -1;
        private int _sourcerowGrid = -1;
        void effectColorFullEllipse_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_isMoved && !_isKilled)
            {
                var colorFullEllipse = sender as ColorFullEllipse;
                if (colorFullEllipse.PlayerPiece == _turn)
                {
                    var inkStroke = new SolidColorBrush(Colors.Red);

                    if (_selectedColorFullEllipse != null)
                    {
                        _selectedColorFullEllipse.SetStroke(inkStroke, 0);
                    }
                    colorFullEllipse.SetStroke(inkStroke, 5);
                    _selectedColorFullEllipse = colorFullEllipse;

                    var element = (UIElement) e.Source;

                    _sourcecolumnGrid = Grid.GetColumn(element);
                    _sourcerowGrid = Grid.GetRow(element);

                    //GenerateMoves(new Point(_sourcecolumnGrid,_sourcerowGrid));
                }
            }
        }

        void effectColorFullEllipse_MouseLeave(object sender, MouseEventArgs e)
        {
            
            GeneralMethods.onMouseLeave_Scale(sender);
        }

        void effectColorFullEllipse_MouseEnter(object sender, MouseEventArgs e)
        {
            GeneralMethods.onMouseEnter_Scale(sender);
        }

        private int _targetcolumnGrid = -1;
        private int _targetrowGrid = -1;

        bool isKilled = false;
        private void confirmLabel_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {       
            if (_isMoved)
            {
                _turn = _turn == "ONE" ? "TWO" : "ONE";
                SetSelected();
                SetLabels();
                _isMoved = false;
                _isKilled = false;
            }
            if(_turn=="TWO")
            {
                AI_PlayerTurn(false);
            }
        }

        void CalculateMoves(ref List<BinaryNodes> pBinNodes,string _player)
        {
            var obj = GridBoard.Children.Cast<UIElement>().OfType<ColorFullEllipse>().Where(i => i.PlayerPiece == _player);
            if(obj.Any())
            {
                int counter = 0;
                do
                {
                    Point p = new Point(Grid.GetColumn(obj.ElementAt(counter)), Grid.GetRow(obj.ElementAt(counter)));
                    GenerateMoves(p, ref pBinNodes);
                    counter++;
                } while (counter != obj.Count());
            }
        }

        void AI_PlayerTurn(bool isPieceKilled)
        {                 
                List<BinaryNodes> AI_Nodes = new List<BinaryNodes>();
                List<BinaryNodes> H_Nodes = new List<BinaryNodes>();

                //for AI_Player
                CalculateMoves(ref AI_Nodes, _turn);

                var list = AI_Nodes.OrderBy(o => o.weight).Reverse();

                int count = 0;
                //for Human_Player
                //do
                //{
                //    H_Nodes.Clear();
                //    CalculateMoves(ref H_Nodes,OponentTurn());
                //    //Eradicate best Moves

                //    count++;
                //}while(H_Nodes.First().isKilled && count<4);
                             
                
                if (AI_Nodes.Count > 0)
                {
                    if(isPieceKilled && list.First().weight != 10)
                    {
                        confirmLabel_PreviewMouseLeftButtonDown(null, null);
                        return;
                    }
                    else
                        SetEllipse(list.First().init_p, list.First().fin_p);
                    if (list.First().isKilled == true)
                    {
                        AI_PlayerTurn(true);
                        return;
                    }
                    confirmLabel_PreviewMouseLeftButtonDown(null, null);
                }           
        }

        private void cancelLabel_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void confirmLabel_MouseEnter(object sender, MouseEventArgs e)
        {
            GeneralMethods.onMouseEnter_Scale(sender);
        }

        private void confirmLabel_MouseLeave(object sender, MouseEventArgs e)
        {
            GeneralMethods.onMouseLeave_Scale(sender);
        }

        //get Ellipse
        ColorFullEllipse GetEllipse(Point p)
        {
            Func<ColorFullEllipse, bool> pred1 = i => Grid.GetRow(i) == p.Y;
            Func<ColorFullEllipse, bool> pred2 = j => Grid.GetColumn(j) == p.X;

            Func<ColorFullEllipse, bool> combinedOr = s => ((pred1(s) && pred2(s)));
            var obj = GridBoard.Children.Cast<UIElement>().OfType<ColorFullEllipse>().Where(combinedOr);
            if(obj.Any())
            {
               return obj.First();
            }
            return null;
        }

        void SetEllipse(Point Init_p,Point Fin_p)
        {
            var ell = GetEllipse(Init_p);
            _selectedColorFullEllipse = ell;
            if (ell == null) return;
            
            var ellipse = GetEllipse(Fin_p);

            if (ellipse == null) return;

            _sourcecolumnGrid = (int)Init_p.X;
            _sourcerowGrid = (int)Init_p.Y;

            _targetcolumnGrid = (int)Fin_p.X;
            _targetrowGrid = (int)Fin_p.Y;
            if (!_isMoved )
            {              
                int diffCol = (_targetcolumnGrid/2 - _sourcecolumnGrid/2);
                int diffRow = (_targetrowGrid/2 - _sourcerowGrid/2);

                if (diffCol < 0) diffCol *= -1;
                if (diffRow < 0) diffRow *= -1;

                bool pieceInBetween = diffCol == 2 || diffRow == 2 ? true : false;

                if (!IsValidMove(pieceInBetween,false)) return;

                _isMoved = true;
                                     
            }
            else if(_isKilled && _isMoved)
            {               
                if(!IsValidMove()) return;
            }

            int tCol = Grid.GetColumn(ellipse);
            int tRow = Grid.GetRow(ellipse);

            int sCol = Grid.GetColumn(_selectedColorFullEllipse);
            int sRow = Grid.GetRow(_selectedColorFullEllipse);

            Grid.SetColumn(_selectedColorFullEllipse, tCol);
            Grid.SetRow(_selectedColorFullEllipse, tRow);

            Grid.SetColumn(ellipse, sCol);
            Grid.SetRow(ellipse, sRow);
            
            if (_isMoved && !_isKilled)
            {
                confirmLabel_PreviewMouseLeftButtonDown(null, null);
            }
            else
            {
                _sourcecolumnGrid = Grid.GetColumn(_selectedColorFullEllipse);
                _sourcerowGrid = Grid.GetRow(_selectedColorFullEllipse);
            }
        }

        void GenerateMoves(Point p,ref List<BinaryNodes> binNodes)
        {
            Point[] arr = new Point[] { new Point(p.X + 2, p.Y), new Point(p.X, p.Y + 2), new Point(p.X - 2, p.Y), new Point(p.X, p.Y - 2), new Point(p.X + 2, p.Y+2), new Point(p.X + 2, p.Y-2), new Point(p.X - 2, p.Y -2 ),new Point(p.X-2,p.Y+2) };
                       
            int counter = 0;
            do
            {
                isKilled = false;
                int isKill = 0;
                if (isValidGenerateMove(p,ref arr[counter], ref isKill))
                {
                   // MessageBox.Show(string.Format("{0},{1} - {2}.{3}",new string[]{p.X.ToString(),p.Y.ToString(),arr[counter].X.ToString(),arr[counter].Y.ToString()}));
                    binNodes.Add(new BinaryNodes(p, arr[counter]) { weight = isKill,isKilled=isKilled });
                }
                counter++;
            } while (counter != arr.Length);            
        }

        bool isValidGenerateMove(Point init_p,ref Point fin_p,ref int kills)
        {           
            if (init_p.X == fin_p.X && init_p.Y == fin_p.Y) return false;
            var src = GetEllipse(init_p);
            if (src == null || src.PlayerPiece=="EMPTY") return false;

            var tar = GetEllipse(fin_p);
            if (tar == null || (tar.PlayerPiece == _turn)) return false;
            if(tar.PlayerPiece==OponentTurn())
            {
                int trow = (int)fin_p.Y;
                int tcol = (int)fin_p.X;
                int diffrow = (int)(fin_p.Y - init_p.Y);    // 0
                int diffcol = (int)(fin_p.X - init_p.X);    //-2

                if (diffcol == -2) tcol = (int)fin_p.X - 2;
                if (diffcol == 2) tcol = (int)fin_p.X + 2;
                if (diffrow == -2) trow = (int)fin_p.Y - 2;
                if (diffrow == 2) trow = (int)fin_p.Y + 2;

                var target = GetEllipse(new Point(tcol,trow));
                if (target == null || target.PlayerPiece != "EMPTY") return false;

                kills = 10;
                fin_p.X = tcol;
                fin_p.Y = trow;

                _sourcecolumnGrid = (int)init_p.X;
                _sourcerowGrid = (int)init_p.Y;

                _targetcolumnGrid = (int)fin_p.X;
                _targetrowGrid = (int)fin_p.Y;

                if (!IsValidMove(true, true)) return false;

                isKilled = true;

                return true;
            }
            _sourcecolumnGrid = (int)init_p.X;
            _sourcerowGrid = (int)init_p.Y;

            _targetcolumnGrid = Grid.GetColumn(tar);
            _targetrowGrid = Grid.GetRow(tar);

            int diffCol = (_targetcolumnGrid / 2 - _sourcecolumnGrid / 2);
            int diffRow = (_targetrowGrid / 2 - _sourcerowGrid / 2);

            if (diffCol < 0) diffCol *= -1;
            if (diffRow < 0) diffRow *= -1;

            bool pieceInBetween = diffCol == 2 || diffRow == 2 ? true : false;

            if (!IsValidMove(pieceInBetween, true)) return false;

            kills = 1;
           // MessageBox.Show("chalta hai: "+_sourcerowGrid.ToString()+","+_sourcecolumnGrid.ToString()+" - "+_targetrowGrid.ToString()+","+_targetcolumnGrid.ToString());

            return true;
        }

       public void TurnOver()
        {
            confirmLabel_PreviewMouseLeftButtonDown(null, null);
        }
    }

    public class BinaryNodes
    {
        public BinaryNodes(Point i,Point f)
        {
            init_p = i;
            fin_p = f;
            weight = 0;
            isKilled = false;
        }
        public Point init_p { get; set; }
        public Point fin_p { get; set; }
        public int weight { get; set; }
        public bool isKilled { get; set; }
    }
}
