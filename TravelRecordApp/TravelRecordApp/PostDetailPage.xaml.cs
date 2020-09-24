using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelRecordApp.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TravelRecordApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PostDetailPage : ContentPage
    {
        private Post selectedPost;

        public PostDetailPage(Post selectedPost)
        {
            InitializeComponent();
            this.selectedPost = selectedPost;
            experienceEntry.Text = selectedPost.Experience;
        }

        private void UpdateButton_Clicked(object sender, EventArgs e)
        {
            selectedPost.Experience = experienceEntry.Text;
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Post>();
                int rows = conn.Update(selectedPost);
                conn.Close();

                if (rows > 0)
                {
                    DisplayAlert("Success", "Experience updated", "Ok");
                }
                else
                {
                    DisplayAlert("Failure", "Error", "Ok");
                }
            };
        }

        private void DeleteButton_Clicked(object sender, EventArgs e)
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Post>();
                int rows = conn.Delete(selectedPost);
                conn.Close();

                if (rows > 0)
                {
                    DisplayAlert("Success", "Experience deleted", "Ok");
                }
                else
                {
                    DisplayAlert("Failure", "Error", "Ok");
                }
            };
        }
    }
}