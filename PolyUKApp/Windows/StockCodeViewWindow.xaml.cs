using PolyUKApp.SQL;
using PolyUKApp.SQL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PolyUKApp.Windows
{
    /// <summary>
    /// Interaction logic for StockCodeViewWindow.xaml
    /// </summary>
    public partial class StockCodeViewWindow : Window
    {
        List<Item> ItemCode = new List<Item>();
        public StockCodeViewWindow()
        {
            InitializeComponent();
            LoadCode();
            ItemDatabaseConnect();
        }
        public void LoadCode()
        {
            string ItemCodeCopy = System.Windows.Clipboard.GetText();
            CodeTextBox.Text = ItemCodeCopy;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();   
        }

        private void CodeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CodeTextBox.Text == "")
            {
                TextBlockPlsEnter.Visibility = Visibility.Visible;
            }
            else
            {
                TextBlockPlsEnter.Visibility = Visibility.Hidden;
            }
        }

        private void BtnEnter_Click(object sender, RoutedEventArgs e)
        {
            RichTextNameInfo.Document.Blocks.Clear();
            RichTextDescInfo.Document.Blocks.Clear();
            RichTextUnitInfo.Document.Blocks.Clear();
            RichTextTypeInfo.Document.Blocks.Clear();
            RichTextWeightInfo.Document.Blocks.Clear();
            RichTextFreeStockInfo.Document.Blocks.Clear();
            RichTextECcode.Document.Blocks.Clear();

            ItemDatabaseConnect();
        }

        private void RichTextDescInfo_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Clipboard.Clear();
            TextRange txtRange = new TextRange(
                RichTextDescInfo.Document.ContentStart,
                RichTextDescInfo.Document.ContentEnd
                );
            System.Windows.Clipboard.SetText(txtRange.Text);
        }
        public async void ItemDatabaseConnect()
        {
            DataAccess db = new DataAccess();

            ItemCode = db.GetItem(CodeTextBox.Text);
            if (ItemCode.Count > 0)
            {
                var Item_name = ItemCode[0].Name.ToString();
                var Item_desc = ItemCode[0].Description.ToString();
                var Item_unit = ItemCode[0].StockUnitName.ToString();
                var Item_type = ItemCode[0].ProductGroupDescription.ToString();
                var Item_weight = ItemCode[0].Weight.ToString();
                string Text_weight = Item_weight.ToString();
                var Item_stocktotal = ItemCode[0].FreeStockQuantity.ToString();
                string Text_stocktotal = Item_stocktotal.ToString();
                string Item_length = Item_desc.Split("x")[0].ToString();
                string Item_width = Item_desc.Split(" ")[2].ToString();
                RichTextNameInfo.AppendText(Item_name);
                RichTextDescInfo.AppendText(Item_desc);
                RichTextUnitInfo.AppendText(Item_unit);
                RichTextTypeInfo.AppendText(Item_type);
                RichTextWeightInfo.AppendText(Text_weight);
                RichTextFreeStockInfo.AppendText(Text_stocktotal);
                RichTextLengthInfo.AppendText(Item_length);
                RichTextWidthInfo.AppendText(Item_width);
                RichTextECcode.AppendText("3920102899");


            }
            else
            {
                TextBlockError.Visibility = Visibility.Visible;
                await Task.Delay(3000);
                TextBlockError.Visibility = Visibility.Hidden;
            }
        }
    }
}
