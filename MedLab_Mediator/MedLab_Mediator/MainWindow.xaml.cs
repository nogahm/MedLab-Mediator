using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MedLab_Mediator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //KnowledgeItems ki;
        //good before and good after
        double GB1;
        double GB2;
        double GA1;
        double GA2;
        //the data of the concept for calculate
        List<DataPoint> DB1=new List<DataPoint>();
        List<DataPoint> DB2 = new List<DataPoint>();

        Boolean kb1=false;
        Boolean kb2=false;
        Boolean op = false;

        public MainWindow()
        {
            InitializeComponent();
            KnowledgeItems ki = new KnowledgeItems();
            String[] Kitems = ki.GetKnowledgeItems();
            //List<String> items = new List<string>();

            //get items with conceptType=primitive and outputType=numeric
            foreach(String item in Kitems)
            {

                //get concept type
                /*Regex regex = new Regex(@"\<ConceptType\>(.*)\</ConceptType\>");
                var v = regex.Match(item);*/
                string ConceptType= getDataInTag("ConceptType", item);
                //get output type
                /*regex = new Regex(@"\<OutputType\>(.*)\</OutputType\>");
                v = regex.Match(item);*/
                string OutputType = getDataInTag("OutputType", item);

                //if conceptType=primitive and outputType=numeric add to items
                if (ConceptType.Equals("Primitive") && OutputType.Equals("Numeric"))
                {
                    //items.Add(item);
                    knowledgeItemsList1.Items.Add(item);
                    knowledgeItemsList2.Items.Add(item);

                }
            }

            //set operator combobox
            operator_cb.Items.Add(func.plus);
            operator_cb.Items.Add(func.minus);
            operator_cb.Items.Add(func.mult);
            operator_cb.Items.Add(func.div);

        }

        //shows selected KI1 and data items in it concept
        private void knowledgeItemsList1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //show selected knowledge
            string selectedKItem = knowledgeItemsList1.SelectedItem.ToString();
            //find concept name (title)
            string conceptName = getDataInTag("Title", selectedKItem);
            //get good befor, good after and time unit
            GB1 = double.Parse(getDataInTag("GoodBefore", selectedKItem));
            GA1 = double.Parse(getDataInTag("GoodAfter", selectedKItem));
            String timeUnit= getDataInTag("LocalPersistencyTimeUnit", selectedKItem);

            selectedKItem = selectedKItem.Replace(">", ">" + System.Environment.NewLine);
            //clear old knowledge if exist
            selectedKItem1_txt.Clear();
            selectedKItem1_txt.Text = selectedKItem;

            //find data for selected knowledge
            DataPoints dp = new DataPoints(conceptName);
            string[] data = dp.GetDataByConcept();
            //clear old data
            dataList1.Items.Clear();
            foreach (string d in data)
            {
                if (d.Contains("<DataPoint>"))
                {
                    dataList1.Items.Add(d);
                    DataPoint temp = new DataPoint(d, GB1, GA1, timeUnit);
                    DB1.Add(temp);
                }
            }

            //see if can calculate or not
            kb1 = true;
            if(kb2 && op)
            {
                calculate_btn.IsEnabled = true;
            }
        }

        //shows selected KI1 and data items in it concept
        private void knowledgeItemsList2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //show selected knowledge
            string selectedKItem = knowledgeItemsList2.SelectedItem.ToString();
            //find concept name (title)
            string conceptName = "";
            /*Match m = Regex.Match(selectedKItem, @"<Title>\s*(.+?)\s*</Title>");
            if (m.Success)
            {
                conceptName = m.Groups[1].Value;
            }*/
            conceptName = getDataInTag("Title", selectedKItem);
            //get good befor and good after
            GB2 = double.Parse(getDataInTag("GoodBefore", selectedKItem));
            GA2 = double.Parse(getDataInTag("GoodAfter", selectedKItem));
            String timeUnit = getDataInTag("LocalPersistencyTimeUnit", selectedKItem);

            selectedKItem = selectedKItem.Replace(">", ">" + System.Environment.NewLine);
            //clear old knowledge if exist
            selectedKItem2_txt.Clear();
            selectedKItem2_txt.Text = selectedKItem;

            //find data for selected knowledge
            DataPoints dp = new DataPoints(conceptName);
            string[] data = dp.GetDataByConcept();
            //clear old data if exist
            dataList2.Items.Clear();
            foreach (string d in data)
            {
                if (d.Contains("<DataPoint>"))
                {
                    dataList2.Items.Add(d);
                    DataPoint temp = new DataPoint(d, GB2, GA2, timeUnit);
                    DB2.Add(temp);
                }
            }

            //see if can calculate or not
            kb2 = true;
            if (kb1 && op)
            {
                calculate_btn.IsEnabled = true;
            }

        }

        //save operator that was choosen
        private void operator_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //see if can calculate or not
            op = true;
            if (kb1 && kb2)
            {
                calculate_btn.IsEnabled = true;
            }
        }

        //calculate and show results
        private void calculate_btn_Click(object sender, RoutedEventArgs e)
        {
            //calculate
            StateFunctionCalculator sfc = new StateFunctionCalculator(DB1, DB2, (func)operator_cb.SelectedItem);
            StringBuilder results= sfc.calculateFunction();

            //show results
            //MessageBoxResult result = MessageBox.Show(results.ToString());
            //MessageBoxResult x = MessageBox.Show("zzzz");
            Results r = new Results(results);
            r.Show();
        }

        //extract the data inside tags
        public String getDataInTag(String tag, String text)
        {
            //string temp = "< " + tag + " > (.*) </ " + tag + ">";
            Match m = Regex.Match(text, @"<" + tag + @">\s*(.+?)\s*</" + tag + @">");
            if (m.Success)
            {
                return m.Groups[1].Value;
            }
            return "";
        }
    }

}
