using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace JsonProject
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            try
            {
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri("https://apifootball.com/api/");
                HttpResponseMessage responseMessage = httpClient.GetAsync("?action=get_leagues&APIkey=bd44f4b34743eef4c758ddf492f2bcb4cfc0a09160ac5f54e64e18a970035a32").Result;
               
                if (Convert.ToInt32(responseMessage.StatusCode)==200)
                {
                    var contents = responseMessage.Content.ReadAsStringAsync();
                    var leagues = JsonConvert.DeserializeObject<IEnumerable<Leagues>>(contents.Result);                    
                    foreach (var item in leagues)
                    {
                        ListViewItem toadd=new ListViewItem();
                        toadd.Text = item.league_name;
                        toadd.Tag = item.league_id;
                        listView1.Items.Add(toadd);                         
                    }                   
                }
            }
            catch (Exception e)
            {

            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
        
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            HttpClient Client = new HttpClient();
            Client.BaseAddress = new Uri("https://apifootball.com/api/");
            HttpResponseMessage responseMessage2 = Client.GetAsync("?action=get_standings&league_id=" + listView1.SelectedItems[0].Tag + "&APIkey=bd44f4b34743eef4c758ddf492f2bcb4cfc0a09160ac5f54e64e18a970035a32").Result;
            var content = responseMessage2.Content.ReadAsStringAsync();
            var league = JsonConvert.DeserializeObject<IEnumerable<Leagues>>(content.Result);            
        }       

        private void listView1_Click(object sender, EventArgs e)
        {         
                listView2.Clear();
                HttpClient Client = new HttpClient();
                Client.BaseAddress = new Uri("https://apifootball.com/api/");
                HttpResponseMessage responseMessage1 = Client.GetAsync("?action=get_standings&league_id=" + listView1.SelectedItems[0].Tag + "&APIkey=bd44f4b34743eef4c758ddf492f2bcb4cfc0a09160ac5f54e64e18a970035a32").Result;
                var content = responseMessage1.Content.ReadAsStringAsync();
                var league = JsonConvert.DeserializeObject<IEnumerable<Leagues>>(content.Result);

                listView2.View = View.Details;
                listView2.GridLines = true;
                listView2.FullRowSelect = true;
                listView2.Columns.Add("Position", 50);
                listView2.Columns.Add("Team", 100);
                listView2.Columns.Add("Points", 100);
                string[] arr = new string[3];
                ListViewItem itm;

                foreach (var item in league)
                {
                    //add items to ListView
                    arr[0] = item.overall_league_position.ToString();
                    arr[1] = item.team_name;
                    arr[2] = item.overall_league_PTS.ToString();
                    itm = new ListViewItem(arr);
                    listView2.Items.Add(itm);                
                }            
            }

        private void listView2_Click(object sender, EventArgs e)
        {
            HttpClient Client = new HttpClient();
            Client.BaseAddress = new Uri("https://apifootball.com/api/");
            HttpResponseMessage responseMessage1 = Client.GetAsync("?action=get_standings&league_id=" + listView1.SelectedItems[0].Tag + "&APIkey=bd44f4b34743eef4c758ddf492f2bcb4cfc0a09160ac5f54e64e18a970035a32").Result;
            var content = responseMessage1.Content.ReadAsStringAsync();
            var league = JsonConvert.DeserializeObject<IEnumerable<Leagues>>(content.Result);
           
            if (listView2.SelectedItems.Count > 0)
            {
               string team= listView2.SelectedItems[0].SubItems[1].Text;

                foreach (var item in league)
                {
                    if (item.team_name.Equals(team))
                    {
                        MessageBox.Show(item.overall_league_GF.ToString());
                    }
                }                
            }           
        }

        private void listView2_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Get the new sorting column.
            ColumnHeader new_sorting_column = listView2.Columns[e.Column];

            // Figure out the new sorting order.
            System.Windows.Forms.SortOrder sort_order;
            if (Position == null)
            {
                // New column. Sort ascending.
                sort_order = SortOrder.Ascending;
            }
            else
            {
                // See if this is the same column.
                if (new_sorting_column == Position)
                {
                    // Same column. Switch the sort order.
                    if (Position.Text.StartsWith("> "))
                    {
                        sort_order = SortOrder.Descending;
                    }
                    else
                    {
                        sort_order = SortOrder.Ascending;
                    }
                }
                else
                {
                    // New column. Sort ascending.
                    sort_order = SortOrder.Ascending;
                }

                // Remove the old sort indicator.
                Position.Text = Position.Text.Substring(2);
            }

            // Display the new sort order.
            Position = new_sorting_column;
            if (sort_order == SortOrder.Ascending)
            {
                Position.Text = "> " + Position.Text;
            }
            else
            {
                Position.Text = "< " + Position.Text;
            }

            // Create a comparer.
            listView2.ListViewItemSorter = new ListViewComparer(e.Column, sort_order);

            // Sort.
            listView2.Sort();
        }

        public class ListViewComparer : System.Collections.IComparer
        {
            private int ColumnNumber;
            private SortOrder SortOrder;

            public ListViewComparer(int column_number,
                SortOrder sort_order)
            {
                ColumnNumber = column_number;
                SortOrder = sort_order;
            }

            // Compare two ListViewItems.
            public int Compare(object object_x, object object_y)
            {
                // Get the objects as ListViewItems.
                ListViewItem item_x = object_x as ListViewItem;
                ListViewItem item_y = object_y as ListViewItem;

                // Get the corresponding sub-item values.
                string string_x;
                if (item_x.SubItems.Count <= ColumnNumber)
                {
                    string_x = "";
                }
                else
                {
                    string_x = item_x.SubItems[ColumnNumber].Text;
                }

                string string_y;
                if (item_y.SubItems.Count <= ColumnNumber)
                {
                    string_y = "";
                }
                else
                {
                    string_y = item_y.SubItems[ColumnNumber].Text;
                }

                // Compare them.
                int result;
                double double_x, double_y;
                if (double.TryParse(string_x, out double_x) &&
                    double.TryParse(string_y, out double_y))
                {
                    // Treat as a number.
                    result = double_x.CompareTo(double_y);
                }
                else
                {
                    DateTime date_x, date_y;
                    if (DateTime.TryParse(string_x, out date_x) &&
                        DateTime.TryParse(string_y, out date_y))
                    {
                        // Treat as a date.
                        result = date_x.CompareTo(date_y);
                    }
                    else
                    {
                        // Treat as a string.
                        result = string_x.CompareTo(string_y);
                    }
                }

                // Return the correct result depending on whether
                // we're sorting ascending or descending.
                if (SortOrder == SortOrder.Ascending)
                {
                    return result;
                }
                else
                {
                    return -result;
                }
            }
        }
    }
}
